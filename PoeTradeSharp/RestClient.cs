// <copyright file="RestClient.cs" company="Zaafar Ahmed">
//     Zaafar
// </copyright>

namespace PoeTradeSharp
{
    using System.Collections.Generic;
    using System.Net;
    using System.Text;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Poe REST connection class to get the new item data
    /// </summary>
    public static class RestClient
    {
        /// <summary>
        /// Currently active pathofexile Leagues
        /// </summary>
        public static readonly dynamic Leagues;

        /// <summary>
        /// Currently available pathofexile Items
        /// </summary>
        public static readonly dynamic Items;

        /// <summary>
        /// Currently available pathofexile Bulk Item list
        /// </summary>
        public static readonly dynamic Static;

        /// <summary>
        /// Currently available pathofexile Mods
        /// </summary>
        public static readonly dynamic Stats;

        /// <summary>
        /// Poe REST protocol address for fetching item details
        /// </summary>
        private const string PoeHTTPSFetchURL = "https://www.pathofexile.com/api/trade/fetch";

        /// <summary>
        /// Poe REST protocol address for searching for an items
        /// </summary>
        private const string PoeSearchItemURL = "https://www.pathofexile.com/api/trade/search";

        /// <summary>
        /// Poe REST protocol address for searching for bulk items
        /// </summary>
        private const string PoeSearchBulkItemURL = "https://www.pathofexile.com/api/trade/exchange";

        /// <summary>
        /// Poe REST protocol address for getting metadata
        /// </summary>
        private const string PoeMetaDataURL = "https://www.pathofexile.com/api/trade/data";

        /// <summary>
        ///  Initializes static members of the <see cref="RestClient" /> class.
        /// </summary>
        static RestClient()
        {
            Leagues = GetMetaData($"{PoeMetaDataURL}/leagues");
            Items = GetMetaData($"{PoeMetaDataURL}/items");
            Static = GetMetaData($"{PoeMetaDataURL}/static");
            Stats = GetMetaData($"{PoeMetaDataURL}/stats");
        }

        /// <summary>
        ///     gets the new item data synchronously.
        ///     Depends on the dataId (which we get from websocket protocal)
        ///     and pattern to figure out which new item are we requesting for.
        /// </summary>
        /// <param name="pattern">
        ///     Wierd hash associated with a poe.trade search
        ///     e.g. Ab3LSL in https://www.pathofexile.com/trade/search/Standard/Ab3LSL
        /// </param>
        /// <param name="dataIds">
        ///     comma seperated dataIds related to the items we want to fetch
        ///     e.g. c0847c74c32fc2f884fc8445413e96574248ece6a665e21535283938164234b2,
        ///     023d61ba6f18913d41d80427c6a3e8403d3a84aec3900eca200aa415c5a326ce
        ///     If using websockets, this will be returned by the websocket server.
        ///     There is a maximum limit of 30 items. If there are more than 30 items
        ///     You have to make a new call every 30 item.
        /// </param>
        /// <param name="isExchange">
        ///     Getting information from Bulk Item Exchange?
        /// </param>
        /// <returns>
        ///     RestCallData object containing response and error code
        /// </returns>
        public static dynamic GetNewItems(string pattern, string dataIds, bool isExchange = false)
        {
            WebClient webClient = new WebClient() { Encoding = Encoding.UTF8 };
            webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
            webClient.QueryString.Set("query", pattern);
            if (isExchange)
            {
                webClient.QueryString.Set("exchange", string.Empty);
            }

            return MakeRequest(webClient, $"{PoeHTTPSFetchURL}/{dataIds}", "GET");
        }

        /// <summary>
        /// Downloads png file from a URL.
        /// </summary>
        /// <param name="url">
        ///    url of the image to download
        /// </param>
        /// <returns>
        /// png file in byte[] format
        /// </returns>
        public static byte[] GetItemImage(string url)
        {
            WebClient webClient = new WebClient();
            return webClient.DownloadData(url);
        }

        /// <summary>
        /// This function search for an item in the pathofexile database
        /// and returns the item pattern and hash of the matches.
        /// </summary>
        /// <param name="league">
        ///   League in which that item belongs to
        /// </param>
        /// <param name="jsonQuery">
        ///   Query string in json format to search for the item
        /// </param>
        /// <param name="isBulk">
        ///   Is this a bulk search or normal search.
        ///   Bulk = https://www.pathofexile.com/trade/exchange/
        ///   Normal = https://www.pathofexile.com/trade/search/
        /// </param>
        /// <returns>
        ///   A JSON object containing item search result hashes and item pattern
        /// </returns>
        public static dynamic SearchForItem(string league, string jsonQuery, bool isBulk = false)
        {
            WebClient webClient = new WebClient() { Encoding = Encoding.UTF8 };
            webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
            string url = isBulk ? PoeSearchBulkItemURL : PoeSearchItemURL;
            return MakeRequest(webClient, $"{url}/{league}", "POST", jsonQuery);
        }

        /// <summary>
        /// For doing a bulk search in pathofexile trading website.
        /// </summary>
        /// <param name="league">
        ///   League to perform the bulk search in.
        /// </param>
        /// <param name="want">
        ///   List of items you want to buy
        /// </param>
        /// <param name="have">
        ///   List of items you want to sell
        /// </param>
        /// <param name="isOnlineRequired">
        ///   Should the seller by online.
        /// </param>
        /// <param name="minimum">
        ///   Minimum stock of that item. 0 means don't care, show all
        /// </param>
        /// <returns>
        ///   Returns a list of items in json format with the seller/misc information.
        /// </returns>
        public static dynamic SearchForBulkItem(string league, List<string> want, List<string> have, bool isOnlineRequired = true, int minimum = 0)
        {
            string fullQuery = "{ \"exchange\" : { \"status\" : { \"option\" : \"online\" }, \"have\" : [ ], \"want\" : [ ] } }";
            JObject query = JObject.Parse(fullQuery);
            if (!isOnlineRequired)
            {
                query["exchange"]["status"]["option"] = "any";
            }

            if (minimum > 0)
            {
                query["exchange"]["minimum"] = minimum;
            }

            if (have.Count > 0)
            {
                query["exchange"]["have"] = new JArray(have);
            }

            if (want.Count > 0)
            {
                query["exchange"]["want"] = new JArray(want);
            }

            return SearchForItem(league, query.ToString(), true);
        }

        /// <summary>
        /// A private function to dry out the fetching of pathofexile trade metadata
        /// </summary>
        /// <param name="url">
        ///   metadata url to fetch
        /// </param>
        /// <returns>
        ///   A JSON object containing pathofexile trade meta data results.
        /// </returns>
        private static dynamic GetMetaData(string url)
        {
            WebClient webClient = new WebClient() { Encoding = Encoding.UTF8 };
            byte[] response = Encoding.UTF8.GetBytes("{\"error\": 0}");
            int errorCode = 0;
            try
            {
                response = webClient.DownloadData(url);
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    var res = e.Response as HttpWebResponse;
                    if (res != null)
                    {
                        errorCode = (int)res.StatusCode;
                    }
                }
            }

            dynamic jsonResponse = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(response));
            jsonResponse.error = errorCode;
            return jsonResponse;
        }

        /// <summary>
        /// This private function reads the PathOfExile response header and Sleep in case
        /// the request is reaching the rate-limit.
        /// </summary>
        /// <param name="responseHeader">
        ///     Web Request Response Header to extract Rate Limit information
        /// </param>
        private static void WaitForRateLimit(WebHeaderCollection responseHeader)
        {
            string[] rateLimitInfo = responseHeader["x-rate-limit-ip"].Split(':');
            string[] rateLimitState = responseHeader["x-rate-limit-ip-state"].Split(':');
            if (int.Parse(rateLimitInfo[0]) - int.Parse(rateLimitState[0]) < 1)
            {
                System.Threading.Thread.Sleep(2000);
            }
        }

        /// <summary>
        /// Private function to send the requests to pathofexile trading website
        /// and lookout for the rate limit issues or any other errors.
        /// </summary>
        /// <param name="webClient">
        ///     WebClient to use for sending the request
        /// </param>
        /// <param name="url">
        ///     Requesting URL
        /// </param>
        /// <param name="method">
        ///     HTTP method. currently supported methods are GET and POST
        /// </param>
        /// <param name="data">
        ///     Body associated with the POST method. In GET method this argument is ignored.
        /// </param>
        /// <returns>
        ///     Response send by the pathofexile server
        /// </returns>
        private static dynamic MakeRequest(WebClient webClient, string url, string method, string data = "")
        {
            byte[] response = Encoding.UTF8.GetBytes("{\"error\": 0}");
            int errorCode = 0;
            try
            {
                if (method == "GET")
                {
                    response = webClient.DownloadData(url);
                }
                else
                {
                    response = webClient.UploadData(url, method, Encoding.ASCII.GetBytes(data));
                }

                WaitForRateLimit(webClient.ResponseHeaders);
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse res = e.Response as HttpWebResponse;
                    if (res != null)
                    {
                        errorCode = (int)res.StatusCode;
                    }
                }
            }

            dynamic jsonResponse = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(response));
            jsonResponse.error = errorCode;
            return jsonResponse;
        }
    }
}
