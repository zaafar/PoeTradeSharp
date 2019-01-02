namespace DriverProgram
{
    using System;
    using System.Threading;
    using PoeTradeSharp;

    class Program
    {
        static void Main(string[] args)
        {
            WebsocketProtocol p = new WebsocketProtocol("Uniq Item", "https://www.pathofexile.com/trade/search/Standard/Abn2fL/live", "");
            p.LOG += P_LOG;
            p.ItemAdded += P_ItemAdded;
            string league = RestClient.Leagues["result"][0]["id"];
            var items = RestClient.SearchForBulkItem(league, new System.Collections.Generic.List<string>() { "exa" }, new System.Collections.Generic.List<string>() { "chaos" });
            Console.WriteLine(items);
            Console.WriteLine(league);
            Thread.Sleep(10000000);
            Console.WriteLine("Closing...");
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
