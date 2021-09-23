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
        [Identity, PrimaryKey]
        public int id { get; set; }
        [Column]
        public string owner { get; set; }
        [Column]
        public int type { get; set; }
        [Column]
        public byte prefix { get; set; }
        [Column]
        public int stack { get; set; }
        [Column]
        public int price { get; set; }
        [Column]
        public bool infinity { get; set; }

        public override string ToString()
        {
            return $"{id:03}.{(prefix > 0 ? $"[i/p{prefix}:{type}]" : $"[i/s{stack}:{type}]")}{Lang.GetItemNameValue(type)} {price}$({(infinity ? "无限" : $"由{owner}出售")})";
        }

        public void GiveTo(TSPlayer player)
        {
            player.GiveItem(type, stack, prefix);
        }
    }
}
