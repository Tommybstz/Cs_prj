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
            var httpClient = new HttpClient();//creates an object to send web req.
            var htmlDocument= new HtmlDocument();//html parser

            //gets all the info on your position using ip-api.com
            var json = httpClient.GetStringAsync("http://ip-api.com/json").Result;//sends GET request to ip-api.com and returns the response as a string
            var info = JsonSerializer.Deserialize<PositionInfo>(json);//get only the lat and lon


            string url = $"https://weather.com/it-IT/tempo/oggi/l/{info.lat},{info.lon}";//build url with the coordinates
            var html = httpClient.GetStringAsync(url).Result;//same as Line 17
            htmlDocument.LoadHtml(html);//loads the html into the parser

            //get the temperature
            var temperatureElement = htmlDocument.DocumentNode.SelectSingleNode("//span[@class='CurrentConditions--tempValue--zUBSz']");//searches for the first element matching the Xpath expression
            var temperature = temperatureElement.InnerText.Trim();//gets teh text of the element and removes whitespace
            Console.WriteLine("temperature: "+temperature);
            //get the conditions 
            var conditionElement = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='CurrentConditions--phraseValue---VS-k']");
            var condition = conditionElement.InnerText;
            Console.WriteLine("condition: " + condition);

            //get the location
            var title= htmlDocument.DocumentNode.SelectSingleNode("//title")?.InnerText.Trim();

            var city = title.Split("per ")[1].Split(" - ")[0].Trim() ?? "N/A";//splits the title and takes the second part in the first split, then splits on - and takes the first part. so you have just the city name left
            Console.WriteLine("city: "+city);
        }
    }
    //DTO data transfer object
    class PositionInfo {
        public double lat { get; set; }
        public double lon { get; set; }
    }
}