using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LazyUtils;
using Terraria;
using TShockAPI;

namespace PrismaticChrome.Shop
{
    public class ProtoItem
    {
        public int type, stack, prefix;
    }

    internal class InventoryProvider : StorageProvider<ProtoItem>
    {
        public override string Name => "物品";
        protected override bool TryGiveTo(TSPlayer player, ProtoItem content)
        {
            if (!player.InventorySlotAvailable) return false;
            player.GiveItem(content.type, content.stack, content.prefix);
            return true;
        }

        protected override bool TryTakeFrom(TSPlayer player, int count, out ProtoItem content)
        {
            var item = player.SelectedItem;
            content = null;
            if (item.IsAir || item.stack < count) return false;
            item.stack -= count;
            content = new ProtoItem()
            {
                prefix = item.prefix, stack = count, type = item.type
            };
            if (item.stack == 0) item.TurnToAir();
            item.Send();
            return true;
        }

        protected override string SerializeToText(ProtoItem content)
        {
            return $"{(content.prefix > 0 ? $"[i/p{content.prefix}:{content.type}]" : $"[i/s{content.stack}:{content.type}]")}{Lang.GetItemNameValue(content.type)}";
        }
    }
}
