// <copyright file="RestClient.cs" company="Zaafar Ahmed">
//     Zaafar
// </copyright>

namespace PoeTradeSharp
{
    using System.Net;
    using System.Text;
    using Newtonsoft.Json;

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
        /// <returns>
        ///     RestCallData object containing response and error code
        /// </returns>
        public static dynamic GetNewItems(string pattern, string dataIds)
        {
            WebClient webClient = new WebClient() { Encoding = Encoding.UTF8 };
            byte[] response = Encoding.UTF8.GetBytes("{\"error\": 0}");
            int errorCode = 0;
            webClient.QueryString.Set("query", pattern);
            try
            {
                response = webClient.DownloadData($"{PoeHTTPSFetchURL}/{dataIds}");
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
            byte[] response = Encoding.UTF8.GetBytes("{\"error\": 0}");
            int errorCode = 0;
            string url = isBulk ? PoeSearchBulkItemURL : PoeSearchItemURL;
            try
            {
                response = webClient.UploadData($"{url}/{league}", "POST", Encoding.ASCII.GetBytes(jsonQuery));
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
    }
}
