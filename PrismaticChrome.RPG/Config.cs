using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LazyUtils;
using Newtonsoft.Json;

namespace PrismaticChrome.RPG
{
    public class Config : Config<Config>
    {
        protected override string Filename => "PrismaticChrome.RPG.json";
        [JsonProperty("死亡掉落系数")] public float DeathPenalty = .2f;
        [JsonProperty("允许从雕像怪获得经验")] public bool AllowGainMoneyFromStatueMobs;
        [JsonProperty("基础货币获取系数")] public float BaseMoney = 1f;
        [JsonProperty("最大货币获取浮动系数")] public float FloatMoneyMax = .2f;
        [JsonProperty("最小货币获取浮动系数")] public float FloatMoneyMin = -.2f;
    }
}
