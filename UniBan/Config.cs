using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LazyUtils;

namespace UniBan
{
    [Config]
    internal class Config : Config<Config>
    {
        public string MySqlHost, MySqlPort, MySqlDbName, MySqlUsername, MySqlPassword;
    }
}
