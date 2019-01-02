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
