using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LazyUtils;
using LinqToDB;
using TShockAPI;

namespace TempGroup
{
    [Command("tguser")]
    public static class TempGroupUserCommand
    {
        public static void Default(CommandArgs args)
        {
            args.Player.SendInfoMessage("/tguser list\n" +
                                        "/tguser set {prefix|suffix|color} <group>");
        }

        [Permission("tempgroup.user")]
        public static void list(CommandArgs args)
        {
            TempGroupAdminCommand.query(args, args.Player?.Account?.Name);
        }

        public static class set
        {
            [Permission("tempgroup.user")]
            public static void prefix(CommandArgs args, string group)
            {
                using (var query = args.Player.Get<TempGroupUserData>())
                    query.Update(g => g.groupPrefix, _ => group);
                args.Player.SendInfoMessage($"prefix set to {group}");
            }
            [Permission("tempgroup.user")]
            public static void suffix(CommandArgs args, string group)
            {
                using (var query = args.Player.Get<TempGroupUserData>())
                    query.Update(g => g.groupSuffix, _ => group);
                args.Player.SendInfoMessage($"suffix set to {group}");
            }
            [Permission("tempgroup.user")]
            public static void color(CommandArgs args, string group)
            {
                using (var query = args.Player.Get<TempGroupUserData>())
                    query.Update(g => g.groupColor, _ => group);
                args.Player.SendInfoMessage($"chat color set to {group}");
            }
        }
    }

    [Command("tgadmin")]
    public static class TempGroupAdminCommand
    {
        public static void Default(CommandArgs args)
        {
            args.Player.SendInfoMessage("/tgadmin add <user> <group> <duration in hours>\n" +
                                        "/tgadmin remove <user> [group]\n" +
                                        "/tgadmin query <user> [group]");
        }

        [Permission("tempgroup.admin")]
        public static void add(CommandArgs args, string user, string group, int duration)
        {
            var now = DateTime.Now;
            using (var context = Db.Context<TempGroupServerData>())
            {
                var prev = context.Config.FirstOrDefault(g => g.name == user && g.group == group)?.end ??
                           DateTime.MinValue;
                if (now > prev) prev = now;
                context.Config.Delete(g => g.name == user && g.group == group);
                context.Config.Insert(() => new TempGroupServerData
                {
                    end = prev.AddHours(duration),
                    group = group,
                    name = user
                });
            }
            query(args, user, group);
        }

        [Permission("tempgroup.admin")]
        public static void query(CommandArgs args, string user, string group)
        {
            using (var context = Db.Context<TempGroupServerData>())
            {
                var prev = context.Config.FirstOrDefault(g => g.name == user && g.group == group);
                if (prev != null) args.Player.SendInfoMessage($"{group} of {user} lasting to {prev.end}");
                else args.Player.SendErrorMessage($"{group} of {user} info not found");
            }
        }

        [Permission("tempgroup.admin")]
        public static void query(CommandArgs args, string user)
        {
            using (var context = Db.Context<TempGroupServerData>())
            {
                var flag = true;
                foreach (var group in context.Config.Where(g => g.name == user))
                {
                    args.Player.SendInfoMessage($"{group.group} of {user} lasting to {group.end}");
                    flag = false;
                }
                if (flag) args.Player.SendErrorMessage($"{user} info not found");
            }
        }

        [Permission("tempgroup.admin")]
        public static void remove(CommandArgs args, string user, string group)
        {
            using (var context = Db.Context<TempGroupServerData>())
            {
                context.Config.Delete(g => g.name == user && g.group == group);
                args.Player.SendInfoMessage($"{group} of {user} removed");
            }
        }

        [Permission("tempgroup.admin")]
        public static void remove(CommandArgs args, string user)
        {
            using (var context = Db.Context<TempGroupServerData>())
            {
                context.Config.Delete(g => g.name == user);
                args.Player.SendInfoMessage($"all groups of {user} removed");
            }
        }
    }
}
