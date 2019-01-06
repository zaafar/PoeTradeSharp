namespace DriverProgram
{
    using System;
    using System.Collections.Generic;
    using PoeTradeSharp;

    class Program
    {
        static void Main(string[] args)
        {
            //WebsocketProtocol p = new WebsocketProtocol("Uniq Item", "https://www.pathofexile.com/trade/search/Standard/Abn2fL/live", "");
            //p.LOG += P_LOG;
            //p.ItemAdded += P_ItemAdded;
            //Thread.Sleep(10000000);
            //Console.WriteLine("Closing...");
            string league = RestClient.Leagues["result"][0]["id"];
            var items1 = RestClient.SearchForBulkItem(league,
                                                     new List<string>() { "exa" },
                                                     new List<string>() { "chaos" });
            var chaosToExaSellerList = items1.result.ToObject<List<string>>();
            string chaosToExaPattern = items1.id;
            var result1 = CurrencyTradeAnalysis.GetAvergeExchangeRate(GetNewItems(chaosToExaSellerList, chaosToExaPattern));
            Console.WriteLine($"result: {result1} opposite {1/result1}");

            var items2 = RestClient.SearchForBulkItem(league,
                                         new List<string>() { "chaos" },
                                         new List<string>() { "exa" });
            var exaToChaosSellerList = items2.result.ToObject<List<string>>();
            string exaToChaosPattern = items2.id;
            var result2 = CurrencyTradeAnalysis.GetAvergeExchangeRate(GetNewItems(exaToChaosSellerList, exaToChaosPattern));
            Console.WriteLine($"result: {result2} opposite {1 / result2}");

        }

        private static void P_ItemAdded(object sender, ItemInfo e)
        {
            Console.WriteLine(e);
        }

        private static void P_LOG(object sender, string e)
        {
            Console.WriteLine(e);
        }

        private static dynamic GetNewItems(List<string> ids, string itemPattern)
        {
            dynamic result = new Newtonsoft.Json.Linq.JObject();
            string csvIds = string.Empty;
            int counter = 0;
            int itemCount = 0;
            int rateLimitCounter = 0;
            foreach (var item in ids)
            {
                csvIds += item;
                counter++;
                if (counter >= 20)
                {
                    var data = RestClient.GetNewItems(itemPattern, csvIds, true);
                    result.Merge(data);
                    rateLimitCounter++;
                    //if (rateLimitCounter > 3)
                    //    return data;
                    counter = 0;
                    csvIds = string.Empty;
                }
                else if (itemCount < ids.Count - 1)
                {
                    csvIds += ",";
                }
                itemCount++;
            }
            if (!string.IsNullOrEmpty(csvIds))
            {
                result.Merge(RestClient.GetNewItems(itemPattern, csvIds, true));
            }
            return result;
        }
    }
}
