using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HttpServer;
using LazyUtils;
using LinqToDB;
using Microsoft.Xna.Framework;
using PrismaticChrome.Core;
using Terraria;
using Terraria.Localization;
using TerrariaApi.Server;
using TShockAPI;

namespace PrismaticChrome.RPG
{
    [ApiVersion(2, 1)]
    public class Plugin : TerrariaPlugin
    {
        private static void SendCombatText(Vector2 pos, string text, Color color, int remoteClient = -1, int ignoreClient = -1)
        {
            NetMessage.SendData(119, remoteClient, ignoreClient, NetworkText.FromLiteral(text), (int)color.PackedValue, pos.X, pos.Y);
        }

        public override string Name => "PrismaticChrome.RPG";

        public Plugin(Main game) : base(game)
        {
        }

        private static Random rnd = new Random();
        private static float FloatingCoefficient()
        {
            var cfg = Config.Instance;
            return ((float)rnd.NextDouble() * (cfg.FloatMoneyMax - cfg.FloatMoneyMin) + cfg.FloatMoneyMin) * cfg.BaseMoney;
        }

        private static double CalcRealDmg(NPC npc, int damage, bool crit)
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
            return num;
        }
        public override void Initialize()
        {
            GetDataHandlers.KillMe.Register((_, args) =>
            {
                if (!(Config.Instance.DeathPenalty > 0)) return;
                using (var query = args.Player.Get<Money>())
                {
                    var money = query.Single().money;
                    var loss = (int)(money * Config.Instance.DeathPenalty);
                    query.Set(d => d.money, d => d.money - loss).Update();
                    args.Player.SendMessage($"你因死亡失去{loss}$", Color.MediumBlue);
                }
            });

            GetDataHandlers.NPCStrike.Register((_, args) =>
            {
                var npc = Main.npc[args.ID];
                var val = (int) (FloatingCoefficient() *
                                 Math.Min(CalcRealDmg(npc, args.Damage, args.Critical > 0),
                                     (npc.realLife > 0 ? Main.npc[npc.realLife] : npc).life));
                using (var query = args.Player.Get<Money>())
                {
                    query.Set(d => d.money, d => d.money + val);


                    if (args.Player.GetData<long>("spamTimer") - LazyPlugin.timer < 60) return;
                    args.Player.SetData("spamTimer", LazyPlugin.timer);

                    SendCombatText(args.Player.TPlayer.Top, $"+{val}$", Color.Yellow, args.Player.Index);
                }
            });
        }
    }
}
