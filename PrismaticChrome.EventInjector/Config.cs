using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LazyUtils;
using Terraria;
using TerrariaApi.Server;

namespace PrismaticChrome.EventInjector
{
    public class Config : Config<Config>
    {
        public Dictionary<string, string> events;
    }
}
