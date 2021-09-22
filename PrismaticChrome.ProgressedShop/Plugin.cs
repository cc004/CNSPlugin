using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LazyUtils;
using LinqToDB;
using PrismaticChrome.Shop;
using Terraria;
using TerrariaApi.Server;

namespace PrismaticChrome.ProgressedShop
{
    [ApiVersion(2, 1)]
    public class Plugin : TerrariaPlugin
    {
        public override string Name => "PrismaticChrome.ProgressedShop";
        private bool loaded;

        public Plugin(Main game) : base(game)
        {
        }

        public override void Initialize()
        {
            ServerApi.Hooks.GamePostUpdate.Register(this, PostUpdate);
        }

        private void PostUpdate(EventArgs _)
        {
            if (!loaded)
            {
                foreach (var config in Config.Instance.items)
                    config.lastpred = config.Predict;
                loaded = true;
            }

            foreach (var config in Config.Instance.items)
            {
                if (config.lastpred || !config.Predict) continue;
                using (var context = Db.Context<ShopItem>())
                    foreach (var item in config.items)
                        context.Insert(item);
                config.lastpred = true;
            }
        }
    }
}
