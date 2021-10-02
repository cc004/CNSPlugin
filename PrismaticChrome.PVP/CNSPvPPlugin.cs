using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using LazyUtils;
using LinqToDB;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Utilities;
using TerrariaApi.Server;
using TShockAPI;

namespace PrismaticChrome.PVP
{
	[ApiVersion(2, 1)]
	public class CNSPvPPlugin : LazyPlugin
    {
		private Dictionary<int, Config.WeaponDebuffInfo> SpecialProj;

		public static CNSPvPPlugin Instance
		{
			get;
			set;
		}

		public override string Name => "CNSPvP";

		public override string Author => "Megghy";

		public override string Description => "一个插件";
        
		public CNSPvPPlugin(Main game)
			: base(game)
		{
			Instance = this;
		}

		public override void Initialize()
		{
			ServerApi.Hooks.NetGreetPlayer.Register(this, OnGreetPlayer);
            ServerApi.Hooks.GamePostUpdate.Register(this, PostUpdate);
            GetDataHandlers.NewProjectile += OnProjCreate;
            GetDataHandlers.ProjectileKill += OnProjDestory;
			GetDataHandlers.PlayerDamage += OnPlayerDamage;
			GetDataHandlers.PlayerSlot += OnSlotChange;
			GetDataHandlers.KillMe += OnPlayerDeath;
			GetDataHandlers.Teleport += OnPlayerTeleport;
            GetDataHandlers.PlayerSpawn += OnPlayerSpawn;
			GetDataHandlers.TogglePvp += OnPVPChange;
			SpecialProj = new Dictionary<int, Config.WeaponDebuffInfo>();
		}

        private static void OnPlayerSpawn(object _, GetDataHandlers.SpawnEventArgs args)
        {
            if (args.SpawnContext == PlayerSpawnContext.ReviveFromDeath) args.Player.SendBag();
        }

        private static void OnGreetPlayer(GreetPlayerEventArgs args)
        {
            TShock.Players[args.Who].SendBag();
        }

        private void OnPlayerDamage(object o, GetDataHandlers.PlayerDamageEventArgs args)
        {
            if (!args.PVP) return;
            var player = TShock.Players[args.ID];
            if (player == null || !SpecialProj.TryGetValue(
                    Main.projectile[args.PlayerDeathReason._sourceProjectileIndex].identity, out var value) ||
                args.PlayerDeathReason._sourceItemType != value.ID) return;

            value.Debuff.ForEach(buff => TShock.Players[args.ID].SetBuff(buff.Key, buff.Value));
            using (var query = player.Get<PvpInfo>())
                query.Set(i => i.getdamage, i => i.getdamage + args.Damage).Update();
            using (var query = args.Player.Get<PvpInfo>())
                query.Set(i => i.causedamage, i => i.causedamage + args.Damage).Update();
        }

        private static void OnPlayerDeath(object o, GetDataHandlers.KillMeEventArgs args)
        {
            var sourcePlayerIndex = args.PlayerDeathReason._sourcePlayerIndex;
            var player = ((sourcePlayerIndex < 255 && sourcePlayerIndex >= 0) ? TShock.Players[args.PlayerDeathReason._sourcePlayerIndex] : null);
            if (player == null) return;
            var num = player.Kill(args.Player);
            args.Handled = true;
            var playerDeathReason = PlayerDeathReason.ByCustomReason("- 玩家 [" + player.Name + "] 使用 " + TShock.Utils.ItemTag(args.PlayerDeathReason.GetItemFromDeathReason()) + " 淘汰了 [" + args.Player.Name + "]\r\n" + $"- 获得积分 {num}".Color("F1FFDC"));
            args.Player.TPlayer.KillMe(playerDeathReason, args.Damage, args.Direction, args.Pvp);
            NetMessage.SendPlayerDeath(args.PlayerId, playerDeathReason, args.Damage, args.Direction, args.Pvp, -1, args.PlayerId);
            args.Player.TPlayer.hostile = false;
            NetMessage.SendData(30, -1, -1, null, args.Player.Index, false.GetHashCode());
        }

        private static void OnPlayerTeleport(object o, GetDataHandlers.TeleportEventArgs args)
        {
            if (!args.Player.IsInPvPArea()) return;
            args.Handled = true;
            args.Player.SendInfoMessage("当前区域不允许进行传送");
            args.Player.Teleport(args.Player.X, args.Player.Y, 1);
        }

        private static void OnPVPChange(object o, GetDataHandlers.TogglePvpEventArgs args)
        {
            args.Handled = true;
            NetMessage.SendData(30, -1, -1, null, args.Player.Index, args.Player.TPlayer.hostile.GetHashCode());
        }

        private static void OnSlotChange(object o, GetDataHandlers.PlayerSlotEventArgs args)
        {
            if (!args.Player.IsLoggedIn) return;
            var netItem = args.Player.PlayerData.inventory[args.Slot];
            if ((args.Slot >= 59 && args.Slot <= 99) || (args.Type != 0 && TShock.ServerSideCharacterConfig.Settings.StartingInventory.All(i => i.NetId != args.Type)))
            {
                args.Handled = true;
                args.Player.SendData(PacketTypes.PlayerSlot, "", args.Player.Index, args.Slot);
                args.Player.TPlayer.inventory[58].SetDefaults();
                args.Player.SendData(PacketTypes.PlayerSlot, "", args.Player.Index, 58f);
            }
            else if (args.Slot < 58 && netItem.NetId == args.Type && args.Stack <= 1)
            {
                args.Handled = true;
                args.Player.TPlayer.inventory[args.Slot].stack = args.Player.TPlayer.inventory[args.Slot].maxStack;
                args.Player.SendData(PacketTypes.PlayerSlot, "", args.Player.Index, args.Slot);
            }
        }

        private void OnProjCreate(object o, GetDataHandlers.NewProjectileEventArgs args)
        {
            if (Config<Config>.Instance.BanndProj.Contains(args.Type))
            {
                args.Handled = true;
                Main.projectile[args.Index].active = false;
                args.Player.SendData(PacketTypes.ProjectileDestroy, "", args.Identity, (int)args.Owner);
                return;
            }
            var weaponDebuffInfo = Config<Config>.Instance.WeaponDebuff.FirstOrDefault(w =>
                args.Player.SelectedItem.type == w.ID && (w.AllowedProj?.Contains(args.Type) ?? true));
            if (weaponDebuffInfo != null && !SpecialProj.ContainsKey(args.Identity))
            {
                SpecialProj.Add(args.Identity, weaponDebuffInfo);
            }
        }

        private void OnProjDestory(object o, GetDataHandlers.ProjectileKillEventArgs args)
        {
            if (SpecialProj.ContainsKey(args.ProjectileIndex))
            {
                SpecialProj.Remove(args.ProjectileIndex);
            }
        }

        private const string Blank = "                                                                                                                 \r\n";

        private static void PostUpdate(EventArgs _)
        {
            void OnEnterArea(TSPlayer plr)
            {
                plr.TPlayer.hostile = true;
                NetMessage.SendData(30, -1, -1, null, plr.Index, true.GetHashCode());
                TShock.Utils.Broadcast($"玩家 [{plr.Name}] 加入流光PVP战场!", Color.White);
            }

            void OnLeaveArea(TSPlayer plr)
            {
                plr.TPlayer.hostile = false;
                NetMessage.SendData(30, -1, -1, null, plr.Index, false.GetHashCode());
            }

            if (LazyPlugin.timer % 30 != 0) return;
            
            foreach (var p in TShock.Players.Where(p => p?.Active ?? false))
            {
                p.SendData(PacketTypes.Status, Blank + p.GetQueryString() + Blank + new string('\n', 30));
                foreach (var type in p.TPlayer.inventory.Where(item => item.buffType > 0 && item.consumable).Select(item => item.buffType))
                    p.SetBuff(type, 100);

                var flag = p.IsInPvPArea();
                if (flag ^ p.IsLastInArea())
                    if (flag) OnEnterArea(p); else OnLeaveArea(p);

                if (flag)
                    foreach (var buff in Config<Config>.Instance.RegionBuff)
                        p.SetBuff(buff, 100);

                using (var query = p.Get<PvpInfo>())
                {
                    var score = query.Single().score;
                    var groups = Config<Config>.Instance.Groups;
                    if (groups.Any(g => g.name == p.Account.Group))
                    {
                        var target = groups.Single(g => g.lower <= score && g.upper > score).name;
                        if (target != p.Account.Group)
                            TShock.UserAccounts.SetUserGroup(p.Account, target);
                    }
                }
                p.UpdateLastArea();
            }
        }
    }
}
