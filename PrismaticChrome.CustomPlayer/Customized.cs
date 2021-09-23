using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LazyUtils;

namespace PrismaticChrome.CustomPlayer
{
    public class Customized : PlayerConfigBase<Customized>
    {
        public string permission { get; set; }
        public string prefix { get; set; }
        public string suffix { get; set; }
        public string color { get; set; }
    }
}
