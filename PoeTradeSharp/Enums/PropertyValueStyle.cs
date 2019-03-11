// <copyright file="PropertyValueStyle.cs" company="Zaafar Ahmed">
//     Zaafar
// </copyright>

namespace PoeTradeSharp.Enums
{
    /// <summary>
    /// Different types of item property
    /// i.e. result -> 0 -> item -> properties list --select property--> Values ---select one from the list---> 1
    /// This is used for colowing the item property value
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
    public enum PropertyValueStyle : int
    {
        /// <summary>
        /// White Color
        /// </summary>
        Default = 0x00,

        /// <summary>
        /// Light Blue Color
        /// </summary>
        Augmented,

        /// <summary>
        /// Unknown Color
        /// </summary>
        Unmet,

        /// <summary>
        /// Unknown Color
        /// </summary>
        PhysicalDamage,

        /// <summary>
        /// Dark Red Color
        /// </summary>
        FireDamage,

        /// <summary>
        /// Dark Blue Color
        /// </summary>
        ColdDamage,

        /// <summary>
        /// Yellow Color
        /// </summary>
        LightningDamage,

        /// <summary>
        /// Unknwon Color
        /// </summary>
        ChaosDamage,

        /// <summary>
        /// No idea what is this for
        /// </summary>
        MagicItem,

        /// <summary>
        /// No idea why we need this
        /// </summary>
        RareItem,

        /// <summary>
        /// No idea why we need this
        /// </summary>
        UniqueItem
    }
}