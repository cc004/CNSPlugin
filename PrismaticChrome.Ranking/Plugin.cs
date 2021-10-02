using LazyUtils;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace PrismaticChrome.Ranking
{
    [ApiVersion(2, 1)]
    public class Plugin : LazyPlugin
    {
        public override string Name => "PrismaticChrome.Ranking";

        public Plugin(Main game) : base(game)
        {
        }

        public override void Initialize()
        {
            TShock.RestApi.RegisterRedirect("/v1/onlinetime/rankboard", "/ranking/totalonline");
            TShock.RestApi.RegisterRedirect("/v1/dailyonlinetime/rankboard", "/ranking/dailyonline");
            TShock.RestApi.RegisterRedirect("/v1/questrank/rankboard", "/ranking/quest");
            TShock.RestApi.RegisterRedirect("/v1/itemrank/rankboard", "/ranking/item");
            TShock.RestApi.RegisterRedirect("/v1/character/query", "/ranking/query");
        }

    }
}