using System;
using System.Linq;
using System.Text;
using LazyUtils;
using LinqToDB;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using TShockAPI;
using TShockAPI.DB;

namespace PrismaticChrome.PVP
{
	internal static class Utils
	{
        public static int Kill(this TSPlayer killer, TSPlayer killee)
        {

            using (var query = killee.Get<PvpInfo>())
            {
                var info = query.Single();
                var supdate = (int)Math.Max(0, info.score - Math.Pow(2.0, info.death / 5f));
                query.Set(i => i.death, i => i.death + 1)
                    .Set(i => i.deathcount, i => i.deathcount + 1)
                    .Set(i => i.kill, i => 0)
                    .Set(i => i.score, i => supdate).Update();
			}

			using (var query = killer.Get<PvpInfo>())
            {
                var kill = query.Single().kill;
                var supdate = kill >= 20 ? 12 : kill >= 10 ? 6 : kill >= 5 ? 4 : 2;
				query.Set(i => i.kill, i => i.kill + 1)
                    .Set(i => i.killcount, i => i.killcount + 1)
                    .Set(i => i.death, i => 0)
                    .Set(i => i.score, i => i.score + supdate).Update();
                return supdate;
            }
		}

		private static Rectangle? area;

		public static Rectangle PvPArea => (area = area ?? Config<Config>.Instance.GetArea()).Value;

        public static string GetQueryString(this TSPlayer plr)
		{
			return GetQueryString(plr.Name);
		}

        public static string GetQueryString(string name)
        {
            using (var query = Db.Get<PvpInfo>(name))
                return query.Single().GetQueryString();
        }

		public static Item GetItemFromDeathReason(this PlayerDeathReason reason)
		{
			var item = new Item();
			item.SetDefaults(reason._sourceItemType);
			item.prefix = (byte)reason._sourceItemPrefix;
			return item;
		}

		public static void SendBag(this TSPlayer plr)
		{
			Array.Clear(plr.PlayerData.inventory, 0, plr.PlayerData.inventory.Length);
			TShock.ServerSideCharacterConfig.Settings.StartingInventory.CopyTo(plr.PlayerData.inventory);
            for (var i = 59; i < 99; i++)
            {
                Config<Config>.Instance.Accessory.TryGetValue(i, out var value);
                plr.PlayerData.inventory[i] = value;
            }
			plr.SendServerCharacter();
			plr.TPlayer.statLifeMax = Config<Config>.Instance.Life;
			plr.TPlayer.statLife = Config<Config>.Instance.Life;
			NetMessage.SendData(16, -1, -1, null, plr.Index);
		}

        public static void UpdateLastArea(this TSPlayer plr)
		{
			plr.SetData("CNSPvP.IsInArea", plr.IsInPvPArea());
        }

        public static bool IsLastInArea(this TSPlayer plr) => plr.GetData<bool>("CNSPvP.IsInArea");
		public static bool IsInPvPArea(this TSPlayer plr)
		{
            return PvPArea.Contains(plr.TileX, plr.TileY);
		}
	}
}
