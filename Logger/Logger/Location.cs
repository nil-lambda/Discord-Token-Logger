using System;
using Newtonsoft;
using Newtonsoft.Json;
using System.Net.Http;
using System.Collections.Generic;

namespace Logger
{
    public class Location
    {
        HttpClient hp = new HttpClient();

        public class GeoLocationInformation
        {
            public string Continent { get; set; }
            public string Country { get; set; }
            public string Region { get; set; }
            public string ISP { get; set; }
            public string ASN { get; set; }
        }

        public string GrabIPAddress() => hp.GetStringAsync("https://api64.ipify.org/").GetAwaiter().GetResult();

        public List<string> GetGEOInformation()
        {
            List<string> infoList = new List<string>();

            try
            {
                var response = hp.GetStringAsync($"https://ipwhois.app/json/{GrabIPAddress()}").GetAwaiter().GetResult();
                var convertedInformation = JsonConvert.DeserializeObject<GeoLocationInformation>(response);
                infoList.Add("  -Continent: " + convertedInformation.Continent);
                infoList.Add("  -Country: " + convertedInformation.Country);
                infoList.Add("  -Region: " + convertedInformation.Region);
                infoList.Add("  -ISP: " + convertedInformation.ISP);
                infoList.Add("  -ASN: " + convertedInformation.ASN);
                infoList.Add("  -IP: " + GrabIPAddress());

                Console.WriteLine("[+] Got geolocation successfully!");
            }
            catch
            {
                Console.WriteLine("[!] Couldn't get geolocation...");
                return infoList;
            }

            return infoList;
        }
    }
}
