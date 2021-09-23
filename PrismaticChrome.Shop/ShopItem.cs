using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LazyUtils;
using LinqToDB.Mapping;
using Terraria;
using Terraria.ID;
using TShockAPI;

namespace PrismaticChrome.Shop
{
    public class ShopItem : ConfigBase<ShopItem>
    {
        internal IStorageProvider Provider => Plugin.GetProvider(provider);

        [Identity, PrimaryKey]
        public int id { get; set; }
        [Column]
        public string provider { get; set; }
        [Column]
        public string content { get; set; }
        [Column]
        public int price { get; set; }
        [Column]
        public bool infinity { get; set; }
        [Column]
        public string owner { get; set; }

        public override string ToString()
        {
            return $"{id:D3}.{Plugin.GetProvider(provider).SerializeToText(content)} {price}$({(infinity ? "无限" : $"由{owner}出售")})";
        }

        internal bool TryGiveTo(TSPlayer player) => Provider.TryGiveTo(player, content);

    }
}
