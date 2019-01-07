// <copyright file="CurrencyTradeAnalysis.cs" company="Zaafar Ahmed">
//     Zaafar
// </copyright>

namespace PoeTradeSharp
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// This class provides some basic trade analysis functions
    /// </summary>
    public static class CurrencyTradeAnalysis
    {
        /// <summary>
        /// Get average exchange rate of an item excluding the people
        /// who are either Offline or AFK.
        /// For now this function only
        /// supports one item to buy and one item to sell.
        /// </summary>
        /// <param name="currencySellerList">
        ///     List of sellers data (in JSON format) from pathofexile trading website.
        /// </param>
        /// <param name="start">
        ///     Starting point of average.
        /// </param>
        /// <param name="length">
        ///     Total elements included in the average ( excluding "AFK"/ "Offline" elements )
        /// </param>
        /// <returns>
        ///  average exchange rate of a currency in double
        /// </returns>
        public static double GetAvergeExchangeRate(dynamic currencySellerList, int start, int length)
        {
            List<double> currencyRatio = new List<double>(256);
            double wantAmount = 0;
            double haveAmount = 0;
            int counter = 0;
            int currentLength = 0;
            foreach (var currencySeller in currencySellerList["result"])
            {
                // Ignoring Offline Or AFK accounts
                if (currencySeller == null ||
                    currencySeller.listing.account.online == null ||
                    currencySeller.listing.account.online.status == "afk")
                {
                    continue;
                }

                if (counter < start)
                {
                    counter++;
                    continue;
                }

                if (currentLength >= length)
                {
                    break;
                }

                wantAmount = currencySeller.listing.price.exchange.amount;
                haveAmount = currencySeller.listing.price.item.amount;
                currencyRatio.Add(haveAmount / wantAmount);
                currentLength++;
            }

            if (currencyRatio.Count > 0)
            {
                return currencyRatio.Average();
            }
            else 
            {
                return 0.0f;
            }
        }
    }
}
