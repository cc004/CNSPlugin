using LazyUtils;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace PrismaticChrome.DeathTimes
{
    [ApiVersion(2, 1)]
    public class Plugin : LazyPlugin
    {
        public Plugin(Main game) : base(game)
        {
        }

        public override void Initialize()
        {
            TShock.RestApi.RegisterRedirect("/v1/deathtimes/rankboard", "/deathtimes/rankboard");
        }

    }
}