using System.Linq;
using LazyUtils;
using LinqToDB;
using TShockAPI;

namespace PrismaticChrome.AutoRevive
{
    [Command("arc", "复活币")]
    public static class Commands
    {
        [Alias("给予"), Permission("autorevive.admin")]
        public static void give(CommandArgs args, string player, int amount)
        {
            using (var query = Db.Get<AutoReviveCoin>(player))
            {
                query.Set(d => d.count, d => d.count + amount).Update();
                args.Player.SendSuccessMessage(amount >= 0 ? $"成功给予{player} {amount}$" : $"成功扣除{player} {-amount}$");
            }
        }
        [Alias("查看"), Permission("autorevive.admin")]
        public static void check(CommandArgs args, string player)
        {
            using (var query = Db.Get<AutoReviveCoin>(player))
            {
                args.Player.SendInfoMessage($"目标玩家的货币数:{query.Single().count}$");
            }
        }
        [Permission("autorevive.player"), RealPlayer]
        public static void Main(CommandArgs args)
        {
            using (var query = args.Player.Get<AutoReviveCoin>())
            {
                args.Player.SendInfoMessage($"当前货币总额:{query.Single().count}$");
            }
        }
        [Permission("autorevive.player")]
        public static void Default(CommandArgs args)
        {
            args.Player.SendInfoMessage("/arc give <玩家> <数量>\n" +
                                        "/arc check <玩家>");
        }

    }
}
