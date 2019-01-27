// <copyright file="WebsocketProtocol.cs" company="Zaafar Ahmed">
//     Zaafar
// </copyright>

namespace PoeTradeSharp
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using WebSocketSharp;

    /// <summary>
    /// Poe Trade websocket connection class
    /// </summary>
    public class WebsocketProtocol
    {
        /// <summary>
        /// User Friendly name given to the pattern.
        /// </summary>
        public readonly string ItemIdentifier;

        /// <summary>
        /// Wierd hash associated with a pathofexile trade search e.g. Ab3LSL in https://www.pathofexile.com/trade/search/Standard/Ab3LSL
        /// </summary>
        public readonly string ItemPattern;

        /// <summary>
        /// league we are searching item in e.g. Standard in https://www.pathofexile.com/trade/search/Standard/Ab3LSL
        /// </summary>
        public readonly string League;

        /// <summary>
        /// Poe Trade websocket protocol address
        /// </summary>
        private const string PoeTradeWebsocketServer = "ws://www.pathofexile.com/api/trade/live/";

        /// <summary>
        /// WebSocket class to connect to poe.trade websocket server
        /// </summary>
        private readonly WebSocket webSocket;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebsocketProtocol" /> class.
        /// </summary>
        /// <param name="itemIdentifier">
        ///     User friendly name given to the pattern
        ///     e.g. Astramentis
        /// </param>
        /// <param name="searchUrl">
        ///    Url of the search item.
        ///     e.g. https://www.pathofexile.com/trade/search/Standard/Ab3LSL
        ///     e.g. https://www.pathofexile.com/trade/search/Standard/Ab3LSL/live
        /// </param>
        /// <param name="userCookie">
        ///    User secret cookie so that we can do live search on pathofexile website.
        ///    This cookie have a very large timeout as long as user doesn't logout from the web.
        ///    Do not share this cookie with anyone OR on a file or anything. Only keep it
        ///    in the RAM. Also, display a warning when asking for this cookie. This is account
        ///    specific cookie.
        /// </param>
        public WebsocketProtocol(string itemIdentifier, string searchUrl, string userCookie)
        {
            this.ItemIdentifier = itemIdentifier;
            var tmp = searchUrl.Split(new char[] { '/' });
            if (tmp[tmp.Length - 1] == "live")
            {
                this.ItemPattern = tmp[tmp.Length - 2];
                this.League = tmp[tmp.Length - 3];
            }
            else
            {
                this.ItemPattern = tmp[tmp.Length - 1];
                this.League = tmp[tmp.Length - 2];
            }

            var url = $"{PoeTradeWebsocketServer}{League}/{ItemPattern}";
            this.webSocket = new WebSocket(url);
            this.webSocket.SetCookie(new WebSocketSharp.Net.Cookie("POESESSID", userCookie));
            this.webSocket.OnOpen += this.OnOpen;
            this.webSocket.OnError += this.OnError;
            this.webSocket.OnMessage += this.OnMessage;
            this.webSocket.OnClose += this.OnClose;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="WebsocketProtocol" /> class
        /// Closes the webSocket connection
        /// </summary>
        ~WebsocketProtocol()
        {
            this.Close();
        }

        /// <summary>
        /// Called when a new item related to a pattern is added.
        /// Contains Item information as arg.
        /// </summary>
        public event EventHandler<ItemInfo> ItemAdded;

        /// <summary>
        /// For sending log message
        /// </summary>
        public event EventHandler<string> LOG;

        /// <summary>
        /// Gets a value indicating whether websocket is connected or not.
        /// </summary>
        public bool IsConnected { get; private set; }

        /// <summary>
        /// Gets or sets the Maximum number of items a client can ask in a single request
        /// </summary>
        public int MaxItemLimit { get; set; } = 10;

        /// <summary>
        /// Async Connect to the Websocket server.
        /// </summary>
        public void ConnectAsync()
        {
            this.webSocket.ConnectAsync();
        }

        /// <summary>
        /// Closes the websocket connection.
        /// </summary>
        public void Close()
        {
            if (this.IsConnected)
            {
                this.webSocket.Close();
            }
        }

        /// <summary>
        /// Called when websocket connection successfully opened
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void OnOpen(object sender, EventArgs e)
        {
            this.IsConnected = true;
            this.LOG?.Invoke(this, $"[INFO] Successfully Connected to {PoeTradeWebsocketServer}{League}/{ItemPattern} url");
        }

        /// <summary>
        /// Called when websocket connection is unsuccessfull
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void OnError(object sender, ErrorEventArgs e)
        {
            this.IsConnected = false;
            this.LOG?.Invoke(this, $"[ERROR] Connecting to {PoeTradeWebsocketServer}{League}/{ItemPattern} url due to  {e.Exception} {e.Message}");
        }

        /// <summary>
        /// Called when the websocket connection closes
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void OnClose(object sender, CloseEventArgs e)
        {
            this.IsConnected = false;
            string niceMessage = $"[INFO] Connection Closed. URL: " +
                $"{PoeTradeWebsocketServer}{League}/{ItemPattern}, ERROR CODE: {e.Code}";
            switch (e.Code)
            {
                case 1000:
                    this.LOG?.Invoke(this, niceMessage + ", REASON: Invalid Cookie");
                    break;
                case 1005:
                    this.LOG?.Invoke(this, niceMessage + ", REASON: On User Request");
                    break;
                default:
                    this.LOG?.Invoke(this, niceMessage + ", REASON: Unknown");
                    break;
            }
        }

        /// <summary>
        /// Called when there is a new message from websocket server
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">
        ///     This parameter contains the message and
        ///     the metadata associated with the message.
        /// </param>
        private void OnMessage(object sender, MessageEventArgs e)
        {
            Dictionary<string, Stack<string>> p = JsonConvert.DeserializeObject<Dictionary<string, Stack<string>>>(e.Data);
            if (!p.ContainsKey("new") && p["new"].Count == 0)
            {
                this.LOG?.Invoke(this, $"[ERROR] Invalid Websocket Message: {p.ToString()}");
            }

            this.GetNewItems(p["new"]);
        }

        /// <summary>
        /// to get new items based on the IDs.
        /// </summary>
        /// <param name="ids">
        /// List of string containing the item IDs from websocket message
        /// </param>
        private void GetNewItems(Stack<string> ids)
        {
            string csvIds = string.Empty;
            int counter = 0;
            while (ids.Count > 0)
            {
                csvIds += ids.Pop();
                counter++;
                if (counter >= this.MaxItemLimit)
                {
                    this.ParseResponse(RestClient.GetNewItems(this.ItemPattern, csvIds));
                    counter = 0;
                    csvIds = string.Empty;
                }
                else if (ids.Count > 0)
                {
                    csvIds += ",";
                }
            }

            if (!string.IsNullOrEmpty(csvIds))
            {
                this.ParseResponse(RestClient.GetNewItems(this.ItemPattern, csvIds));
            }
        }

        /// <summary>
        /// Parse the response and call the "ItemAdded" or "LOG" event
        /// </summary>
        /// <param name="response">
        /// JSON object OR Error code returned by the WebClient
        /// </param>
        private void ParseResponse(dynamic response)
        {
            int errorCode = response.error;
            switch (errorCode)
            {
                case 0:
                    foreach (var item in response.result)
                    {
                        if (item != null)
                        {
                            var definedItem = new ItemInfo(string.Empty);
                            definedItem.Pattern = this.ItemPattern;
                            definedItem.Id = item.id;
                            definedItem.Whisper = item.listing.whisper;
                            if (item.listing.price != null)
                            {
                                var tmpPrice = item.listing.price;
                                definedItem.Price = $"{tmpPrice.type} {tmpPrice.amount} {tmpPrice.currency}";
                            }

                            if (item.listing.account != null)
                            {
                                var accountInfo = item.listing.account;
                                definedItem.SellerAccountInfo = $"{accountInfo.name} {accountInfo.lastCharacterName}";
                                if (accountInfo.online != null)
                                {
                                    definedItem.SellerAccountInfo += $" {accountInfo.online.league}";
                                    if (accountInfo.online.status != null)
                                    {
                                        definedItem.SellerAccountInfo += $" {accountInfo.online.status}";
                                    }
                                }
                                else
                                {
                                    definedItem.SellerAccountInfo += " Offline";
                                }
                            }

                            definedItem.ItemLevel = item.item.ilvl;
                            definedItem.Verified = item.item.verified;
                            definedItem.IconUrl = item.item.icon;
                            definedItem.Name = $"{item.item.name} {item.item.typeLine}";
                            definedItem.Identified = item.item.identified;
                            if (item.corrupted != null)
                            {
                                definedItem.IsCorrupted = true;
                            }

                            if (item.item.stackSize != null)
                            {
                                definedItem.StackSize = item.item.stackSize;
                            }

                            if (item.item.implicitMods != null)
                            {
                                definedItem.ImplicitMods = item.item.implicitMods.ToObject<List<string>>();
                            }

                            if (item.item.explicitMods != null)
                            {
                                definedItem.ExplicitMods = item.item.explicitMods.ToObject<List<string>>();
                            }

                            this.ItemAdded?.Invoke(this, definedItem);
                        }
                    }

                    break;
                case 429:
                    this.LOG?.Invoke(this, $"[ERROR] Too Many Requests to Poe Website. CODE: {errorCode}");
                    break;
                case 400:
                    var errorMsg = "[ERROR] Invalid Request. Maybe sending too many Ids ";
                        errorMsg += $"(i.e. {MaxItemLimit}) in a single request. CODE: {errorCode}";
                    this.LOG?.Invoke(this, errorMsg);
                    break;
                default:
                    this.LOG?.Invoke(this, $"[ERROR] requesting poe for item info. CODE: {errorCode}");
                    break;
            }
        }
    }
}
