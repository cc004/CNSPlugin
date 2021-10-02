using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LazyUtils;
using LinqToDB;
using TShockAPI;
using TShockAPI.DB;

namespace PrismaticChrome.OnlineTime
{
    [Command("onlinetime")]
    public static class Commands
    {
        [Permission("onlinetime.admin")]
        public static void reset(CommandArgs args)
        {
            using (var context = Db.Context<OnlineTimeR>())
            {
                var time = Config.Instance.timetoadmin;
                var time2 = Config.Instance.timetodowgrade;
                var times = Config.Instance.timestodowgrade;
                var ups = context.Config.Where(d => d.total > time).Select(d => d.name).ToArray();
                context.Config.Where(d => d.total < time2)
                    .Set(d => d.downgrade_count, d => d.downgrade_count + 1)
                    .Update();
                context.Config.Where(d => d.total >= time2).Set(d => d.downgrade_count, _ => 0).Update();
                var downs = context.Config.Where(d => d.downgrade_count >= times).Select(d => d.name).ToArray();

                context.Config.Set(d => d.total, _ => 0).Update();

                foreach (var name in ups)
                {
                    var account = TShock.UserAccounts.GetUserAccountByName(name);
                    var nowgroup = Config.Instance.groups.IndexOf(account.Group);
                    if (nowgroup < 0 || nowgroup == Config.Instance.groups.Count - 1) continue;
                    TShock.UserAccounts.SetUserGroup(account, Config.Instance.groups[nowgroup + 1]);
                }

                foreach (var name in downs)
                {
                    var account = TShock.UserAccounts.GetUserAccountByName(name);
                    var nowgroup = Config.Instance.groups.IndexOf(account.Group);
                    if (nowgroup < 0 || nowgroup == 0) continue;
                    TShock.UserAccounts.SetUserGroup(account, Config.Instance.groups[nowgroup - 1]);
                }
            }
        }
    }
}
