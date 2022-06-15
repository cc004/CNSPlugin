﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LazyUtils;
using LinqToDB;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace Chrome.AutoRevive
{
    [ApiVersion(2, 1)]
    public class Plugin : LazyPlugin
    {
        public Plugin(Main game) : base(game)
        {
        }

        public override void Initialize()
        {
            Shop.Plugin.RegisterProvider(new AutoReviveCoinProvider());
            GetDataHandlers.KillMe.Register(OnKillMe, HandlerPriority.Highest);
            GetDataHandlers.PlayerSpawn.Register(OnPlayerSpawn, HandlerPriority.Highest);
        }

        private void OnPlayerSpawn(object _, GetDataHandlers.SpawnEventArgs args)
        {
            if (args.Player.ContainsData("handle_one_spawn"))
            {
                args.Handled = true;
                args.Player.RemoveData("handle_one_spawn");
            }
        }

        private const string TIMER_KEY = "AutoRevive.Timer";

        private static void OnKillMe(object _, GetDataHandlers.KillMeEventArgs args)
        {
            if (args.Player.ContainsData(TIMER_KEY) &&
                args.Player.GetData<long>(TIMER_KEY) + Config.Instance.cooldown > timer)
            {
                args.Player.SendInfoMessage("复活冷却中");
                return;
            }

            var teleport = args.Player.HasPermission("autorevive.teleport");
            using (var query = args.Player.Get<AutoReviveCoin>())
            {
                var count = query.Single().count;
                if (count > 0)
                {
                    query.Set(d => d.count, d => d.count - 1).Update();
                    teleport = true;
                    args.Player.SendSuccessMessage("成功使用一枚复活币复活，剩余复活币：" + (count - 1));
                }
            }

            var isrevive = teleport || args.Player.HasPermission("autorevive.revive");
            if (!isrevive) return;
            var pos = args.Player.TPlayer.position;
            args.Player.SetData<object>("handle_one_spawn", null);
            args.Player.Spawn(PlayerSpawnContext.ReviveFromDeath);
            args.Player.SetData(TIMER_KEY, timer);
            args.Handled = true;
            if (teleport)
            {
                args.Player.Teleport(pos.X, pos.Y);
            }
        }
    }
}
