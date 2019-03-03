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
    /// </summary>
    public static class FormData
    {
        /// <summary>
        /// List of options available in pathofexile trading website for specific categories
        /// ## seperates the text to display and id to send to the pathofexile server
        /// In case of empty id, don't send anything to the server.
        /// It is hard coded because pathofexile.com does not provide a public API
        /// to get these values. These lists are manually extracted from
        /// following JavaScript files returned by the server
        /// -> go to pathofexile.com/trade
        /// -> open inspect
        /// -> go to Debugger
        /// -> go to following file
        ///       web.poecdn.com -> js -> trade.xxxx.js
        /// </summary>
        private static readonly Dictionary<string, List<string>> Options = new Dictionary<string, List<string>>()
        {
            { "ItemCategories", new List<string>()
                {
                    "Any##", "Any Weapon##weapon", "One-Handed Weapon##weapon.one",
                    "One-Handed Melee Weapon##weapon.onemelee",
                    "Two-Handed Melee Weapon##weapon.twomelee", "Bow##weapon.bow",
                    "Claw##weapon.claw", "Dagger##weapon.dagger", "One-Handed Axe##weapon.oneaxe",
                    "One-Handed Mace##weapon.onemace", "One-Handed Sword##weapon.onesword",
                    "Sceptre##weapon.sceptre", "Staff##weapon.staff",
                    "Two-Handed Axe##weapon.twoaxe", "Two-Handed Mace##weapon.twomace",
                    "Two-Handed Sword##weapon.twosword", "Wand##weapon.wand",
                    "Fishing Rod##weapon.rod", "Any Armour##armour", "Body Armour##armour.chest",
                    "Boots##armour.boots", "Gloves##armour.gloves", "Helmet##armour.helmet",
                    "Shield##armour.shield", "Quiver##armour.quiver", "Any Accessory##accessory",
                    "Amulet##accessory.amulet", "Belt##accessory.belt", "Ring##accessory.ring",
                    "Any Gem##gem", "Skill Gem##gem.activegem", "Support Gem##gem.supportgem",
                    "Any Jewel##jewel", "Abyss Jewel##jewel.abyss", "Flask##flask", "Map##map",
                    "Leaguestone##leaguestone", "Prophecy##prophecy", "Card##card",
                    "Captured Beast##monster", "Any Currency##currency",
                    "Unique Fragment##currency.piece", "Resonator##currency.resonator",
                    "Fossil##currency.fossil"
                }
            },
            { "ItemRarity", new List<string>()
                {
                    "Any##", "Normal##normal", "Magic##magic", "Rare##rare", "Unique##unique",
                    "Unique (Relic)##uniquefoil", "Any Non-Unique##nonunique"
                }
            },
            { "MapSeries", new List<string>()
                {
                    "Any##", "Betrayal##betrayal", "War for the Atlas##warfortheatlas",
                    "Atlas of Worlds##atlasofworlds", "The Awakening##theawakening",
                    "Legacy##original"
                }
            },
            { "Boolean", new List<string>()
                {
                    "Any##", "Yes##true", "No##false"
                }
            },
            { "ItemAge", new List<string>()
                {
                    "Any Time##", "Up to a Day Ago##1day", "Up to 3 Days Ago##3days",
                    "Up to a Week Ago##1week", "Up to 2 Weeks Ago##2weeks",
                    "Up to 1 Month Ago##1month", "Up to 2 Month Ago##2months"
                }
            },
            { "PriceType", new List<string>()
                {
                    "Any##", "Buyout or Fixed Price##priced", "No Listed Price##unpriced"
                }
            },
            {
                "UserStatus", new List<string>()
                {
                    "Any##any", "Online Only##online"
                }
            },
            {
                "BuyoutCurrencyType", new List<string>()
            },
        };

        /// <summary>
        /// Maps the options to the Json data keys
        /// </summary>
        private static readonly Dictionary<string, string> OptionMapper = new Dictionary<string, string>()
        {
            { "Status", "UserStatus" },
            { "Category", "ItemCategories" },
            { "Rarity", "ItemRarity" }
        };

        /// <summary>
        /// This structure stores the path in which the lib specific Json data should go to.
        /// It looks at the lib specific Json data key to return a path
        /// </summary>
        private static readonly Dictionary<string, string[]> ParentsInfo = new Dictionary<string, string[]>()
        {
            { "Status", new string[] { "status" } },
            { "Category", new string[] { "filters", "type_filters", "filters", "category" } },
            { "Rarity", new string[] { "filters", "type_filters", "filters", "rarity" } },
            { "Damage", new string[] { "filters", "weapon_filters", "filters", "damage" } },
            { "Attacks Per Second", new string[] { "filters", "weapon_filters", "filters", "aps" } },
            { "Critical Chance", new string[] { "filters", "weapon_filters", "filters", "crit" } },
            { "Damage Per Second", new string[] { "filters", "weapon_filters", "filters", "dps" } },
            { "Physical Dps", new string[] { "filters", "weapon_filters", "filters", "pdps" } },
            { "Elemental Dps", new string[] { "filters", "weapon_filters", "filters", "edps" } },
            { "Armour", new string[] { "filters", "armour_filters", "filters", "ar" } },
            { "Evasion", new string[] { "filters", "armour_filters", "filters", "ev" } },
            { "Energy Shield", new string[] { "filters", "armour_filters", "filters", "es" } },
            { "Block", new string[] { "filters", "armour_filters", "filters", "block" } },
        };

        /// <summary>
        /// This class seperate what to show on the UI (text) and
        /// what to send to the pathofexile server (id) via ## symbol.
        /// This is mainly because ImGui (main user of this class for now)
        /// auto hide everything after ## symbol.
        /// </summary>
        private static readonly string[] SeperatorTextId = new string[] { "##" };

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

                Options["BuyoutCurrencyType"].Add($"{tmpData["text"]}##{tmpData["id"]}");
            }

            Options["BuyoutCurrencyType"].Sort();
            Options["BuyoutCurrencyType"].Insert(0, "Chaos Orb Equivalent##");
        }

        /// <summary>
        /// Gets the item rarity options
        /// </summary>
        public static List<string> ItemRarityOptions { get => Options["ItemRarity"]; }

        /// <summary>
        /// Gets the item category options
        /// </summary>
        public static List<string> ItemCategoriesOptions { get => Options["ItemCategories"]; }

        /// <summary>
        /// Gets the map series options
        /// </summary>
        public static List<string> MapSeriesOptions { get => Options["MapSeries"]; }

        /// <summary>
        /// Gets the boolean options
        /// </summary>
        public static List<string> BooleanOptions { get => Options["Boolean"]; }

        /// <summary>
        /// Gets the item age options
        /// </summary>
        public static List<string> ItemAgeOptions { get => Options["ItemAge"]; }

        /// <summary>
        /// Gets the sale price type options
        /// </summary>
        public static List<string> PriceTypeOptions { get => Options["PriceType"]; }

        /// <summary>
        /// Gets the user status options
        /// </summary>
        public static List<string> UserStatusOptions { get => Options["UserStatus"]; }

        /// <summary>
        /// Gets the basic currency type options for buyout prices
        /// </summary>
        public static List<string> BuyoutCurrencyTypeOptions { get => Options["BuyoutCurrencyType"]; }

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
                    case "Category":
                    case "Rarity":
                        ComboBoxParser(key, ref toReturn.query, obj["value"].ToString());
                        break;
                    case "Damage":
                    case "Attacks Per Second":
                    case "Critical Chance":
                    case "Damage Per Second":
                    case "Physical Dps":
                    case "Elemental Dps":
                    case "Armour":
                    case "Evasion":
                    case "Energy Shield":
                    case "Block":
                        MinMaxParser(key, ref toReturn.query, obj["value"].ToObject<int[]>());
                        break;
                    case "Sockets":
                    case "Links":
                        SocketParser(key, ref toReturn.query, obj["value"].ToObject<int[]>());
                        break;
                    default:
                        System.Console.WriteLine(toReturn);
                        throw new Exception($"Unknown Key: {obj}");
                }
            }

            return toReturn;
        }

        /// <summary>
        /// A helper function to create the parent JObjects if they do not exist
        /// </summary>
        /// <param name="data">
        /// JObject to save the result in, should be passed by reference
        /// </param>
        /// <param name="parents">
        /// List of keys (in string format) to create in sorted order
        /// </param>
        /// <returns>
        /// the last created ( leaf ) parent object
        /// </returns>
        private static dynamic CreateParentJObjectIfNotExists(ref dynamic data, string[] parents)
        {
            dynamic p = data;

            // This works as it's pass by reference
            foreach (var parent in parents)
            {
                if (!p.ContainsKey(parent))
                {
                    p[parent] = new JObject();
                }

                p = p[parent];
            }

            return p;
        }

        /// <summary>
        /// A Helper function to parse the data associated with the UI key "Item Name"
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

            value = value.Split(SeperatorTextId, 2, StringSplitOptions.None)[1];

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
        /// A helper function to parse the data associated with the Type Filters
        /// </summary>
        /// <param name="filterName">
        /// name of the filter, for debugging purposes
        /// </param>
        /// <param name="data">
        /// JObject to save the result in, should be passed by reference
        /// </param>
        /// <param name="value">
        /// value to parse and associate in string format
        /// </param>
        private static void ComboBoxParser(string filterName, ref dynamic data, string value)
        {
            List<string> db = Options[OptionMapper[filterName]];
            string[] parents = ParentsInfo[filterName];
            if (!db.Contains(value))
            {
                throw new Exception($"Invalid ${filterName} Option: {value}");
            }

            string id = value.Split(SeperatorTextId, StringSplitOptions.None)[1];
            if (!string.IsNullOrWhiteSpace(id))
            {
                CreateParentJObjectIfNotExists(ref data, parents).option = id;

                // This is an assumption, it might change in the future
                if (parents.Length >= 2)
                {
                    data[parents[0]][parents[1]].disabled = false;
                }
            }
        }

        /// <summary>
        /// A Helper function to parse the data containing minimum and maximum number
        /// </summary>
        /// <param name="filterName">
        /// name of the filter, for debugging purposes
        /// </param>
        /// <param name="data">
        /// JObject to save the result in, should be passed by reference
        /// </param>
        /// <param name="value">
        /// array containing the minimum and maximum on index 0 and 1 respectively.
        /// </param>
        private static void MinMaxParser(string filterName, ref dynamic data, int[] value)
        {
            string[] parents = ParentsInfo[filterName];
            if (value.Length != 2)
            {
                throw new Exception($"Invalid ${filterName} value length: {value.Length}");
            }

            dynamic tmpData = data;
            if (value[0] > 0)
            {
                CreateParentJObjectIfNotExists(ref data, parents).min = value[0];
                data[parents[0]][parents[1]].disabled = false;
            }

            if (value[1] > 0)
            {
                CreateParentJObjectIfNotExists(ref data, parents).max = value[1];
                data[parents[0]][parents[1]].disabled = false;
            }
        }

        /// <summary>
        /// A helper function to parse the data associated with the sockets/links
        /// </summary>
        /// <param name="filterName">
        /// name of the filter, for debugging purposes
        /// </param>
        /// <param name="data">
        /// JObject to save the result in, should be passed by reference
        /// </param>
        /// <param name="value">
        /// array containing the minimum and maximum on index 0 and 1 respectively.
        /// </param>
        private static void SocketParser(string filterName, ref dynamic data, int[] value)
        {
        }
    }
}
