using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LazyUtils;

namespace PrismaticChrome.AutoRevive
{
    [Config]
    public class Config : Config<Config>
    {
        public int cooldown = 600;
    }
}
