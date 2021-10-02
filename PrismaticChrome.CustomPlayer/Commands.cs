using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LazyUtils;
using LinqToDB;
using PrismaticChrome.CustomPlayer;
using TShockAPI;

namespace PrismaticChrome.CustomPlayer
{
    [Command("custom")]
    public static class Commands
    {
        [Permission("custom.admin")]
        public static void color(CommandArgs args, string player, string color)
        {
            using (var query = Db.Get<Customized>(player))
            {
                var color0 = uint.Parse(color, NumberStyles.HexNumber);
                query.Set(d => d.color, d => color).Update();
                args.Player.SendInfoMessage($"目标玩家的颜色:{color0:X6}");
            }
        }

        [Permission("custom.admin")]
        public static void suffix(CommandArgs args, string player, string text)
        {
            using (var query = Db.Get<Customized>(player))
            {
                query.Set(d => d.suffix, d => text).Update();
                args.Player.SendInfoMessage($"目标玩家的后缀:{text}");
            }
        }


        [Permission("custom.admin")]
        public static void prefix(CommandArgs args, string player, string text)
        {
            using (var query = Db.Get<Customized>(player))
            {
                query.Set(d => d.prefix, d => text).Update();
                args.Player.SendInfoMessage($"目标玩家的前缀:{text}");
            }
        }

        [Permission("custom.admin")]
        public static void del(CommandArgs args, string player, string perm)
        {
            using (var query = Db.Get<Customized>(player))
            {
                var perm0 = query.Single().permission.Replace(perm + "\n", "");
                query.Set(d => d.permission, d => perm0).Update();
                args.Player.SendInfoMessage($"目标玩家的权限:\n{perm0}");
                Plugin.ClearCache();
            }
        }
        [Permission("custom.admin")]
        public static void add(CommandArgs args, string player, string perm)
        {
            perm += "\n";
            using (var query = Db.Get<Customized>(player))
            {
                query.Set(d => d.permission, d => (d.permission ?? "") + perm).Update();
                args.Player.SendInfoMessage($"目标玩家的权限:\n{query.Single().permission}");
                Plugin.ClearCache();
            }
        }
        [Permission("custom.admin")]
        public static void list(CommandArgs args, string player)
        {
            using (var query = Db.Get<Customized>(player))
            {
                args.Player.SendInfoMessage($"目标玩家的权限:\n{query.Single().permission}");
            }
        }
        [Permission("custom.admin")]
        public static void Default(CommandArgs args)
        {
            args.Player.SendInfoMessage("/custom prefix <玩家> [前缀]\n" +
                                        "/custom suffix <玩家> [后缀]\n" +
                                        "/custom add <玩家> <权限>\n" +
                                        "/custom del <玩家> <权限>\n" +
                                        "/custom list <玩家>\n" +
                                        ">");
        }

    }
}
