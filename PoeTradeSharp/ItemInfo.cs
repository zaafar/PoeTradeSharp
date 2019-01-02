// <copyright file="ItemInfo.cs" company="Zaafar Ahmed">
//     Zaafar
// </copyright>

namespace PoeTradeSharp
{
    using System.Collections.Generic;

    /// <summary>
    /// Struct to transfer Item information
    /// It's not complete. More information can be added later on
    /// </summary>
    public struct ItemInfo
    {
        /// <summary>
        /// Item Pattern
        /// </summary>
        public string Pattern;

        /// <summary>
        /// Item Id
        /// </summary>
        public string Id;

        /// <summary>
        /// Whisper message
        /// </summary>
        public string Whisper;

        /// <summary>
        /// Item Price
        /// </summary>
        public string Price;

        /// <summary>
        /// Is Item Verified
        /// </summary>
        public bool Verified;

        /// <summary>
        /// Item Icon Url
        /// </summary>
        public string IconUrl;

        /// <summary>
        /// Item Name
        /// </summary>
        public string Name;

        /// <summary>
        /// Is Item Identified
        /// </summary>
        public bool Identified;

        /// <summary>
        /// List of Item Explicit Mods
        /// </summary>
        public List<string> ExplicitMods;

        /// <summary>
        /// List of Item Implicit Mods
        /// </summary>
        public List<string> ImplicitMods;

        /// <summary>
        /// Item Stack Size
        /// </summary>
        public int StackSize;

        /// <summary>
        /// Item level
        /// </summary>
        public int ItemLevel;

        /// <summary>
        /// Item Seller Account Info
        /// i.e. Account Name, Charactor Name, League Name
        /// </summary>
        public string SellerAccountInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemInfo" /> struct with empty/default values.
        /// </summary>
        /// <param name="id">
        /// Id associated with the Item
        /// </param>
        public ItemInfo(string id)
        {
            this.SellerAccountInfo = string.Empty;
            this.Pattern = string.Empty;
            this.Id = string.Empty;
            this.Whisper = string.Empty;
            this.Price = string.Empty;
            this.Verified = false;
            this.IconUrl = string.Empty;
            this.Name = string.Empty;
            this.Identified = false;
            this.ExplicitMods = new List<string>();
            this.ImplicitMods = new List<string>();
            this.ItemLevel = 0;
            this.StackSize = 1;
        }

        /// <summary>
        /// Better String format for Item Info.
        /// </summary>
        /// <returns>
        /// string of item info.
        /// </returns>
        public override string ToString()
        {
            return $"Name: {Name}\n" +
                   $"ID: {Id}\n" +
                   $"Whisper: {Whisper}\n" +
                   $"Price: {Price}\n" +
                   $"Verified: {Verified}\n" +
                   $"IconUrl: {IconUrl}\n" +
                   $"Identified: {Identified}\n" +
                   $"ImplicitMods: {string.Join("\n", ImplicitMods)}\n" +
                   $"ExplicitMods: {string.Join("\n", ExplicitMods)}\n" +
                   $"Stack Size: {StackSize}\n" +
                   $"ilvl {ItemLevel}";
        }
    }
}
