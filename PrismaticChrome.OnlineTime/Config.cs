using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LazyUtils;

namespace PrismaticChrome.OnlineTime
{
    public class Config : Config<Config>
    {
        public List<string> groups;
        public string admingroup;
    }
}
