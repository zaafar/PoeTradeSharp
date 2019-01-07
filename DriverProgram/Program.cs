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
                                                     new List<string>() { "scour" },
                                                     new List<string>() { "chaos" });
            var result1 = CurrencyTradeAnalysis.GetAvergeExchangeRate(items1);
            Console.WriteLine($"result: {result1} opposite {1/result1}");

            var items2 = RestClient.SearchForBulkItem(league,
                                         new List<string>() { "chaos" },
                                         new List<string>() { "scour" });
            var result2 = CurrencyTradeAnalysis.GetAvergeExchangeRate(items2);
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
    }
}
