using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using LazyUtils;
using PrismaticChrome.Shop;
using Terraria;

namespace PrismaticChrome.ProgressedShop
{
    public class Config : Config<Config>
    {
        public ProgressItem[] items;
    }
    public class ProgressItem
    {
        public string[] include;
        public string[] exclude;
        public ProtoItemWithPrice[] items;

        private Func<bool> predict;
        internal bool lastpred;
        
        internal bool Predict
        {
            get
            {
                if (predict == null) predict = LazyUtils.Utils.Eval(include, exclude);
                return predict();
            }
        }
    }
}
