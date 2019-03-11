// <copyright file="FrameType.cs" company="Zaafar Ahmed">
//     Zaafar
// </copyright>

namespace PoeTradeSharp.Enums
{
    /// <summary>
    /// Different permutation of Item FrameType i.e. result -> 0 -> item -> frameType
    /// This is used for coloring the item name/type ( header )
    /// It is hard coded because pathofexile.com does not provide a public API
    /// to get these values. These lists are manually extracted from
    /// following JavaScript files returned by the server
    /// -> go to pathofexile.com/trade
    /// -> open inspect
    /// -> go to Debugger
    /// -> go to following file
    ///       web.poecdn.com -> js -> main.xxxx.js
    ///       -> search for FrameType
    /// </summary>
    public enum FrameType : int
    {
        /// <summary>
        /// White Color
        /// </summary>
        Normal = 0x00,

        /// <summary>
        /// Blue Color
        /// </summary>
        Magic,

        /// <summary>
        /// Yellow Color
        /// </summary>
        Rare,

        /// <summary>
        /// Golden Color
        /// </summary>
        Unique,

        /// <summary>
        /// Green Color
        /// </summary>
        Gem,

        /// <summary>
        /// Dark Grey Color
        /// </summary>
        Currency,

        /// <summary>
        /// blackish Color
        /// </summary>
        DivinationCard,

        /// <summary>
        /// unknown Color
        /// </summary>
        Quest,

        /// <summary>
        /// purple Color
        /// </summary>
        Prophecy,

        /// <summary>
        /// unknown Color
        /// </summary>
        Relic
    }
}
