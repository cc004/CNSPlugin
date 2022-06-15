using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LazyUtils;
using Newtonsoft.Json;

namespace Chrome.RPG
{
    [Config]
    public class Config : Config<Config>
    {
        public int DeathPenaltyLimit = 0;
        public float DeathPenalty = .2f;
        public bool AllowGainMoneyFromStatueMobs;
        public float BaseMoney = 1f;
        public float FloatMoneyMax = .2f;
        public float FloatMoneyMin = -.2f;
        public Dictionary<int, float> multiplier;
    }
}
