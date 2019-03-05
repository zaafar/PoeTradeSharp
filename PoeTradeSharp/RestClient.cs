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
        public static readonly JObject Leagues;

        /// <summary>
        /// Currently available pathofexile Items
        /// </summary>
        public static readonly JObject Items;

        /// <summary>
        /// Currently available pathofexile Bulk Item list
        /// </summary>
        public static readonly JObject Static;

        /// <summary>
        /// Currently available pathofexile Mods
        /// </summary>
        public static readonly JObject Stats;

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
        /// Gets the maximum number of items a client can ask in a single request
        /// </summary>
        public static int MaxItemLimit { get; } = 10;

        /// <summary>
        /// Gets the maximum number of bulk items a client can ask in a single request
        /// </summary>
        public static int MaxBulkItemLimit { get; } = 20;

        /// <summary>
        /// This function converts the list of hashes into the list of csv strings.
        /// Where each csv string is up to the maximum number of allowed hashes by
        /// pathofexile trading website.
        /// </summary>
        /// <param name="hashes">
        /// List of item hashes
        /// </param>
        /// <param name="maxHashesLimit">
        /// Maximum number of hashes allowed in a single csv string.
        /// This value is stored in RestClient.MaxItemLimit or RestClient.MaxBulkItemLimit.
        /// </param>
        /// <returns>
        /// List of hashes in csv string format
        /// </returns>
        public static List<string> ConvertHashesListToCSVStringList(JArray hashes, int maxHashesLimit)
        {
            string tmpHashes = string.Empty;
            List<string> toReturn = new List<string>();
            for (int i = 0; i < hashes.Count; i++)
            {
                tmpHashes += hashes[i];
                if ((i + 1) % maxHashesLimit == 0)
                {
                    toReturn.Add(tmpHashes);
                    tmpHashes = string.Empty;
                }
                else if ((i + 1) < hashes.Count)
                {
                    // It's not the last hash in the hashes list
                    tmpHashes += ",";
                }
                else
                {
                    toReturn.Add(tmpHashes);
                    tmpHashes = string.Empty;
                }
            }

            return toReturn;
        }

        /// <summary>
        ///     Given the comma seperated hashes, this function gets the item information
        ///     corrsponding to those hashes.
        /// </summary>
        /// <param name="pattern">
        ///     Pattern associated with a pathofexile trade search
        ///     e.g. Ab3LSL in https://www.pathofexile.com/trade/search/Standard/Ab3LSL
        /// </param>
        /// <param name="csvHashes">
        ///     comma seperated item hashes
        ///     e.g. c0847c74c32fc2f884fc8445413e96574248ece6a665e21535283938164234b2,
        ///     023d61ba6f18913d41d80427c6a3e8403d3a84aec3900eca200aa415c5a326ce
        ///     If using websockets, this will be returned by the websocket server.
        ///     There is a maximum limit set by the server on the number of hashes
        ///     a caller can ask for in a single request. Re-call this function again
        ///     in case caller has to ask for more number of hashes than allowed.
        /// </param>
        /// <param name="isBulk">
        ///     Getting information from Bulk Item Exchange?
        /// </param>
        /// <returns>
        ///     RestCallData object containing response and error code
        /// </returns>
        public static JObject GetItemInfoFromHashes(string pattern, string csvHashes, bool isBulk)
        {
            WebClient webClient = new WebClient() { Encoding = Encoding.UTF8 };
            webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
            webClient.QueryString.Set("query", pattern);
            if (isBulk)
            {
                webClient.QueryString.Set("exchange", string.Empty);
            }

            return MakeRequest(webClient, $"{PoeHTTPSFetchURL}/{csvHashes}", "GET");
        }

        /// <summary>
        /// This function post a Json query in the pathofexile database
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
        public static JObject FetchHashesFromQuery(string league, string jsonQuery, bool isBulk)
        {
            WebClient webClient = new WebClient() { Encoding = Encoding.UTF8 };
            webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
            string url = isBulk ? PoeSearchBulkItemURL : PoeSearchItemURL;
            return MakeRequest(webClient, $"{url}/{league}", "POST", jsonQuery);
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
        private static JObject MakeRequest(WebClient webClient, string url, string method, string data = "")
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
                    if (e.Response is HttpWebResponse res)
                    {
                        errorCode = (int)res.StatusCode;
                        WaitForRateLimit(res.Headers);
                    }
                }
            }

            JObject jsonResponse = JsonConvert.DeserializeObject<JObject>(Encoding.UTF8.GetString(response));
            jsonResponse["error"] = errorCode;
            return jsonResponse;
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
        private static JObject GetMetaData(string url)
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

            JObject jsonResponse = JsonConvert.DeserializeObject<JObject>(Encoding.UTF8.GetString(response));
            jsonResponse["error"] = errorCode;
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
            if (int.Parse(rateLimitInfo[0]) - int.Parse(rateLimitState[0]) < 2)
            {
                System.Threading.Thread.Sleep(2000);
            }
        }
    }
}