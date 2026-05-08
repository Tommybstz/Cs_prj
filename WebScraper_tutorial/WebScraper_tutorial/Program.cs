using HtmlAgilityPack;
using System;
using System.Net.Http;
using System.Text.Json;

namespace WebScraper
{
    class Program
    {
        static void Main()
        {
            //send get request to weather.com
            var httpClient = new HttpClient();
            var htmlDocument= new HtmlDocument();

            //gets all the info on your position using ip-api.com
            var json = httpClient.GetStringAsync("http://ip-api.com/json").Result;
            var info = JsonSerializer.Deserialize<PositionInfo>(json);//get only the lat and lon


            string url = $"https://weather.com/it-IT/tempo/oggi/l/{info.lat},{info.lon}";
            var html = httpClient.GetStringAsync(url).Result;
            htmlDocument.LoadHtml(html);

            //get the temperature
            var temperatureElement = htmlDocument.DocumentNode.SelectSingleNode("//span[@class='CurrentConditions--tempValue--zUBSz']");
            var temperature = temperatureElement.InnerText.Trim();
            Console.WriteLine("temperature: "+temperature);
            //get the conditions 
            var conditionElement = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='CurrentConditions--phraseValue---VS-k']");
            var condition = conditionElement.InnerText;
            Console.WriteLine("condition: " + condition);

            //get the location
            var title= htmlDocument.DocumentNode.SelectSingleNode("//title")?.InnerText.Trim();

            var city = title.Split("per ")[1].Split(" - ")[0].Trim() ?? "N/A";//splits the title and takes the second part in the first split, then splits on - and takes the first part
            Console.WriteLine("city: "+city);
        }
    }
    //DTO data transfer object
    class PositionInfo {
        public double lat { get; set; }
        public double lon { get; set; }
    }
}