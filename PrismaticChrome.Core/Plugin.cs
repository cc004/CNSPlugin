using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LazyUtils;
using Terraria;
using TerrariaApi.Server;

namespace PrismaticChrome.Core
{
    [ApiVersion(2, 1)]
    public class Plugin : TerrariaPlugin
    {
        public override string Name => "PrismaticChrome.Core";

        public Plugin(Main game) : base(game)
        {
        }

        public override void Initialize()
        {
            RestHelper.Register<Rests>("economy");
        }
    }
}
