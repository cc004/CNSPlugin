using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LazyUtils;
using LinqToDB;
using LinqToDB.Mapping;
using Microsoft.Xna.Framework;
using TShockAPI;

namespace TempGroup
{
    public class TempGroupServerData : ConfigBase<TempGroupServerData>
    {
        [Identity, PrimaryKey]
        public int id { get; set; }
        [Column]
        public string name { get; set; }
        [Column]
        public string group { get; set; }
        [Column]
        public DateTime end { get; set; }
    }
    public class TempGroupUserData : PlayerConfigBase<TempGroupUserData>
    {
        public string groupPrefix { get; set; }
        public string groupSuffix { get; set; }
        public string groupColor { get; set; }
    }

    public class TempGroupCache
    {
        public readonly string prefix;
        public readonly string suffix;
        public readonly Color chatColor;
        public readonly HashSet<string> permissions;

        private TempGroupCache(TSPlayer player)
        {
            TempGroupUserData userData;
            using (var query = player.Get<TempGroupUserData>())
                userData = query.Single();

            var cfg = Config.Instance;
            var acc = player.Account?.Name;
            var now = DateTime.Now;
            using (var query = Db.Context<TempGroupServerData>())
            {
                permissions = query.Config.Where(g => g.name == acc && g.end >= now).Select(g => g.group)
                    .AsEnumerable().SelectMany(g => cfg.groups[g].permissions).ToHashSet();

                var groupDefault = query.Config.FirstOrDefault(g => g.name == acc && g.end >= now);

                if (groupDefault == null) return;

                var @default = cfg.groups[groupDefault.group];
                prefix = @default.prefix;
                suffix = @default.suffix;
                chatColor = @default.chatColor;

                var group = query.Config
                    .FirstOrDefault(g => g.name == acc && g.group == userData.groupPrefix && g.end >= now);
                if (group != null) prefix = cfg.groups[group.group].prefix;
                group = query.Config
                    .FirstOrDefault(g => g.name == acc && g.group == userData.groupSuffix && g.end >= now);
                if (group != null) suffix = cfg.groups[group.group].suffix;
                group = query.Config
                    .FirstOrDefault(g => g.name == acc && g.group == userData.groupColor && g.end >= now);
                if (group != null) chatColor = cfg.groups[group.group].chatColor;

            }
        }

        public static TempGroupCache Get(TSPlayer player)
        {
            if (!player.ContainsData(nameof(TempGroupCache)))
                player.SetData(nameof(TempGroupCache), new TempGroupCache(player));
            return player.GetData<TempGroupCache>(nameof(TempGroupCache));
        }

        public static void RemoveCache(TSPlayer player)
        {
            if (player.ContainsData(nameof(TempGroupCache)))
                player.RemoveData(nameof(TempGroupCache));
        }
    }
}
