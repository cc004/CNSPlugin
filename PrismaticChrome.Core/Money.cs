using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LazyUtils;
using LinqToDB.Mapping;

namespace PrismaticChrome.Core
{
    public class Money : PlayerConfigBase<Money>
    {
        [Column]
        public int money { get; set; }
    }
}
