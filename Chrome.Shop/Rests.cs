using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LazyUtils;
using Newtonsoft.Json.Linq;
using Rests;
using TShockAPI;

namespace Chrome.Shop
{
    [Rest("shop")]
    public static class Rests
    {
        [Permission("economy.shop.player")]
        public static JToken getshopitems(RestRequestArgs args)
        {
            using (var context = Db.Context<ShopItem>())
                return JToken.FromObject(context.Config.OrderByDescending(d => d.id).ToArray());
        }
    }
}
