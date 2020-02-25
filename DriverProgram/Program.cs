namespace DriverProgram
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Newtonsoft.Json.Linq;
    using PoeTradeSharp;

    class Program
    {
        static void Main(string[] args)
        {
            /*WebsocketProtocol p = new WebsocketProtocol("Uniq Item", "https://www.pathofexile.com/trade/search/Metamorph/Abn2fL/live", "22d4a54aec6563d144c740d1666a1642");
            p.LOG += P_LOG;
            p.ItemAdded += P_ItemAdded;
            p.ConnectAsync();
            Thread.Sleep(100 * 1000);
            p.ItemAdded -= P_ItemAdded;
            return;*/

            //string data = System.IO.File.ReadAllText(@"C:\Users\Zaafar Ahmed\Desktop\sampleOutput.txt");
            //var k = FormData.Parse(JArray.Parse(data));
            //var k = RestClient.GetPoeImage(@"https://web.poecdn.com/image/Art/2DItems/Jewels/PoisonDamageModifier.png?scale=1&w=1&h=1&v=70643080a1ee44e3b496bfde4ed08430");
            //var k = RestClient.SearchForItems("Standard", data);
            //Console.WriteLine(k.Length);
            //Console.WriteLine("Closing...");

            /*string league = RestClient.Leagues["result"][0]["id"].ToObject<string>();
            var items1 = RestClient.SearchForBulkItem(league,
                                                     new List<string>() { "chaos" },
                                                     new List<string>() { "exalt" });
            Console.WriteLine(items1["total"]);
            for (int i = 0; i < (int)items1["total"].ToObject<int>(); i+=10)
            {
                var result1 = CurrencyTradeAnalysis.GetAvergeExchangeRate(items1, i, 10);
                Console.WriteLine($" start {i} end {i+10} result: {result1} opposite {1 / result1}");
            }
            */
            //var items2 = RestClient.SearchForBulkItem(league,
            //                             new List<string>() { "chaos" },
            //                             new List<string>() { "scour" });
            //var result2 = CurrencyTradeAnalysis.GetAvergeExchangeRate(items2);
            //Console.WriteLine($"result: {result2} opposite {1 / result2}");
            
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
