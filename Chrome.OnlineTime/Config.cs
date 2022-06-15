using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LazyUtils;

namespace Chrome.OnlineTime
{
    [Config]
    public class Config : Config<Config>
    {
        public List<string> groups;
        public string admingroup;
        public int timetoadmin = 5000;
        public int timetodowgrade = 600;
        public int timestodowgrade = 2;
    }
}
