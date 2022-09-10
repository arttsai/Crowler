using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using HtmlAgilityPack;

namespace Crowler
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            StartToCrawl();
        }

        private static void StartToCrawl()
        {
            var url = @"http://app2.atmovies.com.tw/boxoffice/twweekend/";

            HtmlWeb web = new HtmlWeb();

            var htmlDoc = web.Load(url);

            var contentNode = htmlDoc.DocumentNode
                .Descendants("div")
                .FirstOrDefault(x => x.GetAttributeValue("class", "")
                    .Equals("content content-left"));

            var table = contentNode.Descendants("table").ToList()[1];
            var trs = table.Descendants("tr").ToList();
            trs.RemoveAt(0);

            var movies = new List<Movie>();
            for (int i = 0; i < trs.Count; i+=2)
            {
                var name = trs[i].Descendants("td").ToList()[1].InnerText;
                
                var saleTd = trs[i + 1].Descendants("td").ToList();
                var weeklySaleStr = saleTd[2].InnerText.MakeNumber();
                var accumulatedSaleStr = saleTd[3].InnerText.MakeNumber();
                var weeklySale = decimal.Parse(weeklySaleStr, NumberStyles.Currency, CultureInfo.CurrentCulture);
                var accumulatedSale = decimal.Parse(accumulatedSaleStr, NumberStyles.Currency, CultureInfo.CurrentCulture);

                var movie = new Movie
                {
                    Name = name,
                    WeekSale = (int)weeklySale,
                    AccumulatedSale = (int)accumulatedSale
                };
                movies.Add(movie);

                Console.WriteLine($"{(i/2+1):00} {movie.ToString()}");
            }
        }
    }

    public static class StringExtension
    {
        public static string MakeNumber(this string str)
        {
            var newStr = "";
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '$' || str[i] == '.') continue;
                newStr += str[i];
            }

            return newStr;
        }
    }

    internal class Movie
    {
        public string Name { get; set; }
        public int WeekSale { get; set; }
        public int AccumulatedSale { get; set; }

        public override string ToString()
        {
            return $"Name: {Name}, 本週: {WeekSale}, 累計: {AccumulatedSale}";
        }
    }
}