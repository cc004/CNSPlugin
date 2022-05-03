using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LazyUtils;
using LinqToDB;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace PrismaticChrome.OnlineTime
{
    [ApiVersion(2, 1)]
    public class Plugin : LazyPlugin
    {
        private static readonly TimeSpan DayZeroOffset = TimeSpan.FromHours(5);
        
        private int nowday = 0;
        
        public Plugin(Main game) : base(game)
        {
        }

        public override void Initialize()
        {
            ServerApi.Hooks.GamePostUpdate.Register(this, OnUpdate);
            PlayerHooks.PlayerPostLogin += PlayerHooksOnPlayerPostLogin;
            nowday = (DateTime.Now - DayZeroOffset).DayOfYear;
        }

        private void PlayerHooksOnPlayerPostLogin(PlayerPostLoginEventArgs args)
        {
            using (var query = args.Player.Get<OnlineTimeR>())
                if (query.Single().is_admin)
                    args.Player.tempGroup = TShock.Groups.GetGroupByName(Config.Instance.admingroup);
        }

        private void OnUpdate(EventArgs __)
        {
            if (timer % (60 * 60) != 0) return;
            using (var context = Db.Context<OnlineTimeR>())
            {
                foreach (var plr in TShock.Players)
                {
                    if (plr?.Account?.Name == null) continue;
                    if (!Netplay.Clients[plr.Index].IsActive) continue;
                    context.Config.Where(d => d.name == plr.Account.Name)
                        .Set(d => d.total, d => d.total + 1)
                        .Set(d => d.daily, d => d.daily + 1)
                        .Update();
                }
            }

            var now = (DateTime.Now - DayZeroOffset).DayOfYear;
            if (now == nowday) return;
            nowday = now;

            using (var context = Db.Context<OnlineTimeR>())
            {
                context.Config.Set(d => d.is_admin, _ => false).Update();
                foreach (var name in context.Config.OrderByDescending(d => d.daily).AsEnumerable()
                    .Where(d => Config.Instance.groups.Contains(TShock.UserAccounts.GetUserAccountByName(d.name).Group))
                    .Take(3).Select(d => d.name).ToArray())
                    context.Config.Where(d => d.name == name)
                        .Set(d => d.is_admin, _ => true).Update();

                context.Config.Set(d => d.daily, _ => 0).Update();

                foreach (var plr in TShock.Players)
                {
                    if (plr?.Account?.Name == null) continue;
                    plr.tempGroup = context.Config.Single(d => d.name == plr.Account.Name).is_admin ?
                        TShock.Groups.GetGroupByName(Config.Instance.admingroup) :
                        null;
                }
            }
        }
    }
}
