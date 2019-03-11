// <copyright file="PropertyDisplayMode.cs" company="Zaafar Ahmed">
//     Zaafar
// </copyright>

namespace PoeTradeSharp.Enums
{
    /// <summary>
    /// Different ways of displaying an item property
    /// i.e. result -> 0 -> item -> properties list -> displayMode
    /// It is hard coded because pathofexile.com does not provide a public API
    /// to get these values. These lists are manually extracted from
    /// following JavaScript files returned by the server
    /// -> go to pathofexile.com/trade
    /// -> open inspect
    /// -> go to Debugger
    /// -> go to following file
    ///       web.poecdn.com -> js -> main.xxxx.js
    ///       -> search for DisplayMode
    /// </summary>
    public enum PropertyDisplayMode : int
    {
        /// <summary>
        /// Shows Name then the value
        /// </summary>
        NameValue = 0x00,

        /// <summary>
        /// Shows Value then the name
        /// </summary>
        ValueName,

        /// <summary>
        /// Shows value as progressbar
        /// </summary>
        Progress,

        /// <summary>
        /// Shows value within the text e.g. foo 25 and bar 54
        /// </summary>
        Inject
    }
}
