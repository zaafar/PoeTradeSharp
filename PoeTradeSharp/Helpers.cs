// <copyright file="Helpers.cs" company="Zaafar Ahmed">
//     Zaafar
// </copyright>

namespace PoeTradeSharp
{
    /// <summary>
    /// A bunch of helper functions extracted from the pathofexile JS code
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// This list is manually extracted from
        /// following JavaScript file returned by the server
        /// -> go to pathofexile.com/trade
        /// -> open inspect
        /// -> go to Debugger
        /// -> go to following file
        ///       web.poecdn.com -> js -> main.xxxx.js
        ///       -> search for typeToField or data-field
        /// </summary>
        private static string[] propertyTypeToFieldName = new string[]
        {
            string.Empty,
            "map_tier",
            "map_iiq",
            "map_iir",
            "map_packsize",
            "gem_level",
            "quality",
            string.Empty,
            string.Empty,
            "pdamage",
            "edamage",
            "cdamage",
            "crit",
            "aps",
            string.Empty,
            "block",
            "ar",
            "ev",
            "es",
            string.Empty,
            "gem_level_progress",
            string.Empty,
            string.Empty,
            string.Empty
        };

        /// <summary>
        /// This function converts the result -> 0 -> item -> properties ---select property-> type value
        /// to the Field that should be send to the server for sorting asc/dec.
        /// </summary>
        public static string[] PropertyTypeToFieldName => propertyTypeToFieldName;
    }
}
