using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LazyUtils;
using MySql.Data.MySqlClient;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;

namespace UniBan
{
    [ApiVersion(2, 1)]
    public class Plugin : LazyPlugin
    {
        private IDbConnection db;

        public Plugin(Main game) : base(game)
        {
        }

        public override void Initialize()
        {
            db = new MySqlConnection();
            var cfg = Config.Instance;
            db.ConnectionString =
                $"Server={cfg.MySqlHost}; Port={cfg.MySqlPort}; Database={cfg.MySqlDbName}; Uid={cfg.MySqlUsername}; Pwd={cfg.MySqlPassword};";
            TShock.Bans = new BanManager(db);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            db.Dispose();
        }
    }
}
