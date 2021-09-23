using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LazyUtils;
using LinqToDB;
using Microsoft.Xna.Framework;
using PrismaticChrome.Core;
using Terraria;
using Terraria.Localization;
using TShockAPI;

namespace PrismaticChrome.RPG
{
    public class MoneyAllocator
    {
        private static Random rnd = new Random();
        private static float FloatingCoefficient()
        {
            var cfg = Config.Instance;
            return ((float)rnd.NextDouble() * (cfg.FloatMoneyMax - cfg.FloatMoneyMin) + cfg.FloatMoneyMin + 1f) * cfg.BaseMoney;
        }

        private readonly Dictionary<NPC, Dictionary<string, float>> damageDictionary =
            new Dictionary<NPC, Dictionary<string, float>>();

        public void AddDamage(NPC npc, string account, float damage)
        {
            if (npc.realLife > 0) npc = Main.npc[npc.realLife];
            if (!damageDictionary.ContainsKey(npc))
                damageDictionary.Add(npc, new Dictionary<string, float>());
            if (!damageDictionary[npc].ContainsKey(account))
                damageDictionary[npc].Add(account, damage);
            else
                damageDictionary[npc][account] += damage;
        }

        public void SettleNPC(NPC npc)
        {
            if (!damageDictionary.ContainsKey(npc)) return;
            var coeff = npc.lifeMax / damageDictionary[npc].Sum(p => p.Value);
            using (var context = Db.Context<Money>())
            {
                foreach (var pair in damageDictionary[npc])
                {
                    var val = (int) (pair.Value * coeff * FloatingCoefficient());
                    context.Config.Where(d => d.name == pair.Key).Set(d => d.money, d => d.money + val).Update();
                    pair.Key.NoticeChange(val);
                }
            }

            damageDictionary.Remove(npc);
        }

        public void Update()
        {
            foreach (var npc in damageDictionary.Keys.ToArray())
                if (!Main.npc.Contains(npc) || !npc.active) damageDictionary.Remove(npc);
        }
    }
}
