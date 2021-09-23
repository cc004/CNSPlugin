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
        public ShopItem[] items;

        private Func<bool> predict;
        internal bool lastpred;
        private static Func<bool> GenExp(string exp)
        {
            var s = exp.Split('.');
            var @class = string.Join(".", s.Take(s.Length - 1));
            var field = typeof(Main).Assembly.GetType(@class).GetField(s.Last(), BindingFlags.Static | BindingFlags.Public);
            return () => (bool)field.GetValue(null);
        }
        private Func<bool> Compile()
        {
            var exps = new List<Func<bool>>();
            foreach (var exp in include)
            {
                var e = GenExp(exp);
                exps.Add(() => e());
            }
            foreach (var exp in exclude)
            {
                var e = GenExp(exp);
                exps.Add(() => !e());
            }

            return () => exps.All(f => f());
        }

        internal bool Predict
        {
            get
            {
                if (predict == null) predict = Compile();
                return predict();
            }
        }
    }
}
