// <copyright file="BulkFormData.cs" company="Zaafar Ahmed">
//     Zaafar
// </copyright>

namespace PoeTradeSharp
{
    using System.Collections.Generic;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Helps creating the pathofexile trading website specific Json data
    /// for bulk trading.
    /// </summary>
    public static class BulkFormData
    {
        /// <summary>
        /// Convert the bulk item exchange requirement to pathofexile trading website
        /// acceptable Json string
        /// </summary>
        /// <param name="want">
        /// Bulk items you want
        /// </param>
        /// <param name="have">
        /// Bulk items you give
        /// </param>
        /// <param name="onlineOnly">
        /// Should the trader be online
        /// </param>
        /// <param name="minimumStack">
        /// Minimum ammount of items available on the trader
        /// </param>
        /// <returns>
        /// a string capturing all the requirements
        /// </returns>
        public static string CreateQuery(
            List<string> want, List<string> have, bool onlineOnly = true, int minimumStack = 0)
        {
            string fullQuery = "{ \"exchange\" : { \"status\" : { \"option\" : \"online\" }, \"have\" : [ ], \"want\" : [ ] } }";
            JObject query = JObject.Parse(fullQuery);
            if (!onlineOnly)
            {
                query["exchange"]["status"]["option"] = "any";
            }

            if (minimumStack > 0)
            {
                query["exchange"]["minimum"] = minimumStack;
            }

            if (have.Count > 0)
            {
                query["exchange"]["have"] = new JArray(have);
            }

            if (want.Count > 0)
            {
                query["exchange"]["want"] = new JArray(want);
            }

            return query.ToString();
        }
    }
}
