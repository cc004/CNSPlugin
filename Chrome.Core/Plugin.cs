using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LazyUtils;
using Terraria;
using TerrariaApi.Server;

namespace Chrome.Core
{
    [ApiVersion(2, 1)]
    public class Plugin : LazyPlugin
    {
        public Plugin(Main game) : base(game)
        {
        }
    }
}
