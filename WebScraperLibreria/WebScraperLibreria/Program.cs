using HtmlAgilityPack;
using System.Net.Http;
using System;
using System.Net.WebSockets;

namespace WebScraperLibreria
{
    class Program
    {
        static void Main()
        {
            var httpClient = new HttpClient();
            var htmlDocument = new HtmlDocument();

            var url = "https://www.bortoloso.it/?srsltid=AfmBOoq6BYAgo-Cez8kFP06wKWuKghMFLB0Ci7ALlWbJVv74O_Ksf6t2";
            var html= httpClient.GetStringAsync(url).Result;
            htmlDocument.LoadHtml(html);

            //get the categories
            var categories = htmlDocument.DocumentNode.SelectNodes("//ul[@class='tab']/li");
            List<string> urlsCat=new List<string>();
            
            foreach(var category in categories)
            {
                //Console.WriteLine(category.InnerText);
                var urlCat = $"https://www.bortoloso.it/genere.php?gen={Uri.EscapeDataString(category.InnerText)}";
                urlsCat.Add(urlCat);
                
            }

            //make the new url to get the books titles in each category
            var Products = new List<HtmlNodeCollection>();

            int i = 0;
            foreach (string urlSinglecat in urlsCat)
            {
                try
                {
                    var htmlCat = httpClient.GetStringAsync(urlSinglecat).Result;
                    var htmlDocumentCat = new HtmlDocument(); // create a fresh one each time
                    htmlDocumentCat.LoadHtml(htmlCat);
                    var bookElement = htmlDocumentCat.DocumentNode.SelectNodes("//a[contains(@class,'product_title')]");
                    Products.Add(bookElement);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            var books = new List<string>();
            Console.WriteLine($"Products count: {Products.Count}");
            foreach (var item in Products)
            {
                if (item == null) continue;
                foreach (var book in item)
                {
                    try
                    {
                        var text = System.Text.RegularExpressions.Regex.Replace(book.InnerHtml, "<.*?>", "").Trim();
                        if (!string.IsNullOrWhiteSpace(text))
                            Console.WriteLine(text);
                    }
                    catch
                    {
                        // skip broken nodes
                    }
                }
            }
        }
    }
}