using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LazyUtils;
using Newtonsoft.Json.Linq;
using PrismaticChrome.Core;
using Rests;
using TShockAPI;

namespace PrismaticChrome.Market
{
    internal class Rests
    {
        [Permission("economy.market.player")]
        public static JToken getshopitems(RestRequestArgs args)
        {
            using (var context = Db.Context<ShopItem>())
                return JToken.FromObject(context.Config.OrderByDescending(d => d.id).ToArray());
        }
    }
}
