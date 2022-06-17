using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LazyUtils;
using Microsoft.Xna.Framework;

namespace TempGroup
{
    public class TempGroupInfo
    {
        public string[] permissions;
        public string prefix;
        public string suffix;
        public Color chatColor;
    }

    [Config]
    public class Config : Config<Config>
    {
        public Dictionary<string, TempGroupInfo> groups;
    }
}
