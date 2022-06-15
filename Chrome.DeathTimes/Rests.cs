﻿using System.Linq;
using LazyUtils;
using Newtonsoft.Json.Linq;
using Rests;
using TShockAPI;

namespace Chrome.DeathTimes
{
    [Rest("deathtimes")]
    public static class Rests
    {
        [Permission("deathtimes.admin")]
        public static JToken rankboard(RestRequestArgs args)
        {
            int i = 0;
            using (var context = Db.Context<DeathTimes>())
                return new JArray
                (
                    context.Config.OrderByDescending((tuple) => tuple.times)
                        .AsEnumerable().Select((tuple) => new JObject
                        {
                            ["times"] = (long)tuple.times,
                            ["rank"] = ++i,
                            ["name"] = tuple.name
                        })
                );
        }
    }
}
