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
        /// Poe REST protocol address
        /// </summary>
        private const string PoeHTTPSServer = "https://www.pathofexile.com/api/trade/fetch/";

        /// <summary>
        /// Poe REST method to use
        /// </summary>
        private const string RestMethod = "GET";

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
        ///     dataIds related to the items we want to fetch
        ///     e.g. c0847c74c32fc2f884fc8445413e96574248ece6a665e21535283938164234b2,
        ///     023d61ba6f18913d41d80427c6a3e8403d3a84aec3900eca200aa415c5a326ce
        ///     If using websockets, this will be returned by the websocket server.
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
                response = webClient.DownloadData($"{PoeHTTPSServer}{dataIds}");
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
        public static byte[] GetImage(string url)
        {
            WebClient webClient = new WebClient();
            return webClient.DownloadData(url);
        }
    }
}
