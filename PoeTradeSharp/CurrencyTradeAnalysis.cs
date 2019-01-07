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
        /// <returns>
        ///  average exchange rate of a currency in double
        /// </returns>
        public static double GetAvergeExchangeRate(dynamic currencySellerList)
        {
            List<double> currencyRatio = new List<double>(256);
            double wantAmount = 0;
            double haveAmount = 0;
            foreach (var currencySeller in currencySellerList["result"])
            {
                // Ignoring Offline Or AFK accounts
                if (currencySeller.listing.account.online == null ||
                    currencySeller.listing.account.online.status == "afk")
                {
                    continue;
                }

                wantAmount = currencySeller.listing.price.exchange.amount;
                haveAmount = currencySeller.listing.price.item.amount;
                currencyRatio.Add(haveAmount / wantAmount);
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
