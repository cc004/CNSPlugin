using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HttpServer;
using LazyUtils;
using LinqToDB;
using Microsoft.Xna.Framework;
using OTAPI;
using PrismaticChrome.Core;
using Terraria;
using Terraria.Localization;
using TerrariaApi.Server;
using TShockAPI;

namespace PrismaticChrome.RPG
{
    [ApiVersion(2, 1)]
    public class Plugin : LazyPlugin
    {

        public override string Name => "PrismaticChrome.RPG";

        public Plugin(Main game) : base(game)
        {
        }

        private static float CalcRealDmg(NPC npc, int damage, bool crit)
        {

            double num = damage;
            int num2 = npc.defense;
            if (npc.ichor)
            {
                num2 -= 15;
            }
            if (npc.betsysCurse)
            {
                num2 -= 40;
            }
            if (num2 < 0)
            {
                num2 = 0;
            }
            num = Main.CalculateDamageNPCsTake((int)num, num2);
            if (crit)
            {
                num *= 2.0;
            }
            if (npc.takenDamageMultiplier > 1f)
            {
                num *= (double)npc.takenDamageMultiplier;
            }
            if (npc.type == 371 || npc.SpawnedFromStatue && Config.Instance.AllowGainMoneyFromStatueMobs)
            {
                num = 0.0;
            }
            return (float)num;
        }
        public override void Initialize()
        {
            GetDataHandlers.KillMe.Register(OnKillMe);
            GetDataHandlers.NPCStrike.Register(OnNPCStrike);
            ServerApi.Hooks.NpcKilled.Register(this, args => allocator.SettleNPC(args.npc));
            ServerApi.Hooks.GamePostUpdate.Register(this, _ => allocator.Update());
        }

        private static readonly MoneyAllocator allocator = new MoneyAllocator();

        private static void OnNPCStrike(object _, GetDataHandlers.NPCStrikeEventArgs args)
        {
            var npc = Main.npc[args.ID];
            var val = CalcRealDmg(npc, args.Damage, args.Critical > 0);
            if (Config.Instance.multiplier.TryGetValue(npc.type, out var multiplier))
                val *= multiplier;
            allocator.AddDamage(Main.npc[args.ID], args.Player?.Account?.Name, val);
        }

        private static void OnKillMe(object _, GetDataHandlers.KillMeEventArgs args)
        {
            using (var query = args.Player.Get<Money>())
            {
                var money = query.Single().money;
                var loss = (int) ((money - Config.Instance.DeathPenaltyLimit) * Config.Instance.DeathPenalty);
                if (money <= 0) return;
                query.Set(d => d.money, d => d.money - loss).Update();
                args.Player.NoticeChange(-loss);
                args.Player.SendMessage($"你因死亡失去{loss}$", Color.OrangeRed);
            }
        }
    }
}
