// <copyright file="FormData.cs" company="Zaafar Ahmed">
//     Zaafar
// </copyright>

namespace PoeTradeSharp
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Convert Lib specific Json data to pathofexile trading website specific Json data.
    /// Hard coded list found in this class are there because pathofexile.com does not
    /// provide a public API to get these values. These lists are manually extracted from
    /// following JavaScript files returned by the server
    /// -> go to pathofexile.com/trade
    /// -> open inspect
    /// -> go to Debugger
    /// -> go to following file
    ///       web.poecdn.com -> js -> trade.xxxx.js
    /// </summary>
    public static class FormData
    {
        /// <summary>
        /// List of options available in pathofexile trading website item category.
        /// ## seperates the text to display and id to send to the pathofexile server
        /// In case of empty id, don't send anything to the server
        /// </summary>
        private static List<string> itemCategoriesOptions = new List<string>()
        {
            "Any##", "Any Weapon##weapon", "One-Handed Weapon##weapon.one",
            "One-Handed Melee Weapon##weapon.onemelee", "Two-Handed Melee Weapon##weapon.twomelee",
            "Bow##weapon.bow", "Claw##weapon.claw", "Dagger##weapon.dagger",
            "One-Handed Axe##weapon.oneaxe", "One-Handed Mace##weapon.onemace",
            "One-Handed Sword##weapon.onesword", "Sceptre##weapon.sceptre", "Staff##weapon.staff",
            "Two-Handed Axe##weapon.twoaxe", "Two-Handed Mace##weapon.twomace",
            "Two-Handed Sword##weapon.twosword", "Wand##weapon.wand", "Fishing Rod##weapon.rod",
            "Any Armour##armour", "Body Armour##armour.chest", "Boots##armour.boots",
            "Gloves##armour.gloves", "Helmet##armour.helmet", "Shield##armour.shield",
            "Quiver##armour.quiver", "Any Accessory##accessory", "Amulet##accessory.amulet",
            "Belt##accessory.belt", "Ring##accessory.ring", "Any Gem##gem",
            "Skill Gem##gem.activegem", "Support Gem##gem.supportgem",
            "Any Jewel##jewel", "Abyss Jewel##jewel.abyss", "Flask##flask", "Map##map",
            "Leaguestone##leaguestone", "Prophecy##prophecy", "Card##card",
            "Captured Beast##monster", "Any Currency##currency", "Unique Fragment##currency.piece",
            "Resonator##currency.resonator", "Fossil##currency.fossil"
        };

        /// <summary>
        /// List of options available in pathofexile trading website item rarity.
        /// ## seperates the text to display and data to send to the pathofexile server
        /// In case of empty id, don't send anything to the server
        /// </summary>
        private static List<string> itemRarityOptions = new List<string>()
        {
            "Any##", "Normal##normal", "Magic##magic", "Rare##rare", "Unique##unique",
            "Unique (Relic)##uniquefoil", "Any Non-Unique##nonunique"
        };

        /// <summary>
        /// List of options available in pathofexile trading website map series.
        /// ## seperates the text to display and data to send to the pathofexile server
        /// In case of empty id, don't send anything to the server
        /// </summary>
        private static List<string> mapSeriesOptions = new List<string>()
        {
            "Any##", "Betrayal##betrayal", "War for the Atlas##warfortheatlas",
            "Atlas of Worlds##atlasofworlds", "The Awakening##theawakening", "Legacy##original"
        };

        /// <summary>
        /// List of options available in pathofexile trading website boolean.
        /// ## seperates the text to display and data to send to the pathofexile server
        /// In case of empty id, don't send anything to the server
        /// </summary>
        private static List<string> booleanOptions = new List<string>()
        {
            "Any##", "Yes##true", "No##false"
        };

        /// <summary>
        /// List of options available in pathofexile trading website item age.
        /// ## seperates the text to display and data to send to the pathofexile server
        /// In case of empty id, don't send anything to the server
        /// </summary>
        private static List<string> itemAgeOptions = new List<string>()
        {
            "Any Time##", "Up to a Day Ago##1day", "Up to 3 Days Ago##3days",
            "Up to a Week Ago##1week", "Up to 2 Weeks Ago##2weeks",
            "Up to 1 Month Ago##1month", "Up to 2 Month Ago##2months"
        };

        /// <summary>
        /// List of options available in pathofexile trading website sale type.
        /// ## seperates the text to display and data to send to the pathofexile server
        /// In case of empty id, don't send anything to the server
        /// </summary>
        private static List<string> priceTypeOptions = new List<string>()
        {
            "Any##", "Buyout or Fixed Price##priced", "No Listed Price##unpriced"
        };

        /// <summary>
        /// List of options available in pathofexile trading website user status.
        /// ## seperates the text to display and data to send to the pathofexile server
        /// In case of empty id, don't send anything to the server
        /// </summary>
        private static List<string> userStatusOptions = new List<string>()
        {
            "Any##", "Online Only##online"
        };

        /// <summary>
        /// List of options available in pathofexile trading website basic price.
        /// ## seperates the text to display and data to send to the pathofexile server
        /// In case of empty id, don't send anything to the server
        /// </summary>
        private static List<string> buyoutCurrencyTypeOptions = new List<string>();

        /// <summary>
        /// Initializes static members of the <see cref="FormData" /> class.
        /// </summary>
        static FormData()
        {
            JObject tmpData;
            for (int i = 0; i < 17; i++)
            {
                tmpData = RestClient.Static["result"]["currency"][i].ToObject<JObject>();
                if (tmpData["text"].ToString() == "Engineer's Orb")
                {
                    continue;
                }

                buyoutCurrencyTypeOptions.Add($"{tmpData["text"]}##{tmpData["id"]}");
            }

            buyoutCurrencyTypeOptions.Sort();
            buyoutCurrencyTypeOptions.Insert(0, "Chaos Orb Equivalent##");
        }

        /// <summary>
        /// Gets the item rarity options
        /// </summary>
        public static List<string> ItemRarityOptions { get => itemRarityOptions; }

        /// <summary>
        /// Gets the item category options
        /// </summary>
        public static List<string> ItemCategoriesOptions { get => itemCategoriesOptions; }

        /// <summary>
        /// Gets the map series options
        /// </summary>
        public static List<string> MapSeriesOptions { get => mapSeriesOptions; }

        /// <summary>
        /// Gets the boolean options
        /// </summary>
        public static List<string> BooleanOptions { get => booleanOptions; }

        /// <summary>
        /// Gets the item age options
        /// </summary>
        public static List<string> ItemAgeOptions { get => itemAgeOptions; }

        /// <summary>
        /// Gets the sale price type options
        /// </summary>
        public static List<string> PriceTypeOptions { get => priceTypeOptions; }

        /// <summary>
        /// Gets the user status options
        /// </summary>
        public static List<string> UserStatusOptions { get => userStatusOptions; }

        /// <summary>
        /// Gets the basic currency type options for buyout prices
        /// </summary>
        public static List<string> BuyoutCurrencyTypeOptions { get => buyoutCurrencyTypeOptions; }

        /// <summary>
        /// Converts the form data to pathofexile trading website specific data
        /// </summary>
        /// <param name="data">
        /// Form data in JArray format
        /// </param>
        /// <returns>
        /// pathofexile trading website specific Json data
        /// </returns>
        public static JObject Parse(JArray data)
        {
            string league = string.Empty;

            dynamic toReturn = new JObject();
            toReturn.sort = new JObject();
            toReturn.sort.price = "asc";

            toReturn.query = new JObject();

            // TODO: this class implements a statParser function
            toReturn.query.stats = JArray.Parse("[{\"filters\": [], \"type\": \"and\"}]");
            string key = string.Empty;
            foreach (var obj in data)
            {
                key = obj["key"].ToString();

                if (string.IsNullOrEmpty(key))
                {
                    throw new Exception($"Cannot have empty string in key: {obj}");
                }

                switch (key)
                {
                    case "Item Name":
                        NameParser(ref toReturn.query, obj["value"].ToString());
                        break;
                    case "League":
                        league = obj["value"].ToString();
                        break;
                    case "Status":
                        StatusParser(ref toReturn.query, obj["value"].ToString());
                        break;
                    case "Category":
                        break;
                    default:
                        System.Console.WriteLine(toReturn);
                        throw new Exception($"Unknown Key: {obj}");
                }
            }

            return toReturn;
        }

        /// <summary>
        /// Helper function to parse the data associated with the UI key "Item Name"
        /// </summary>
        /// <param name="data">
        /// JObject to save the result in, should be passed by reference
        /// </param>
        /// <param name="value">
        /// Data associated with the key in string format. Following are the 2 acceptable options.
        /// All the other option considers as invalid and throws an exception.
        ///     1: An empty string
        ///     2: Either a name, type or both seperated by a ':'.
        ///         In this case this function expects '##' and ':' in the string.
        /// </param>
        private static void NameParser(ref dynamic data, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            string[] sep = new string[1] { "##" };
            value = value.Split(sep, 2, StringSplitOptions.None)[1];

            string[] details = value.Split(':');
            if (details.Length != 2)
            {
                throw new Exception($"Item name ({value}) must contain one : in it.");
            }

            bool isNameEmpty = string.IsNullOrEmpty(details[0]);
            bool isTypeEmpty = string.IsNullOrEmpty(details[0]);
            if (!isNameEmpty)
            {
                data.name = details[0];
            }

            if (!isTypeEmpty)
            {
                data.type = details[1];
            }

            if (!(isTypeEmpty && isTypeEmpty))
            {
                throw new Exception($"Item name ({value}) must either contains a name or a type.");
            }
        }

        /// <summary>
        /// Helper function to parse the data associated with the UI key "Status"
        /// </summary>
        /// <param name="data">
        /// JObject to save the result in, should be passed by reference
        /// </param>
        /// <param name="value">
        /// data associated with the key in string format
        /// </param>
        private static void StatusParser(ref dynamic data, string value)
        {
            data.status = new JObject();
            switch (value)
            {
                case "Any":
                    data.status.option = "any";
                    break;
                case "Online Only":
                    data.status.option = "online";
                    break;
                default:
                    throw new Exception($"Invalid Status Option: {value}");
            }
        }

        /// <summary>
        /// Helper function to parse the data associated with the UI key "Category"
        /// </summary>
        /// <param name="data">
        /// JObject to save the result in, should be passed by reference
        /// </param>
        /// <param name="value">
        /// An element of ItemCategoriesOptions list.
        /// </param>
        private static void CategoryParser(ref dynamic data, string value)
        {
        }

        /// <summary>
        /// A helper function to create parent JObject path if they does not exist
        /// </summary>
        /// <param name="data">
        /// JObject to save the result in, should be passed by reference
        /// </param>
        /// <param name="parents">
        /// List of keys (in string format) to create in sorted order
        /// </param>
        private static void CreateParentJObjectIfNotExists(ref dynamic data, string[] parents)
        {
            dynamic p = data;

            // This works as it's pass by reference
            foreach (var parent in parents)
            {
                if (!p.ContainsKey(parent))
                {
                    p[parent] = new JObject();
                    p = p[parent];
                }
            }
        }
    }
}
