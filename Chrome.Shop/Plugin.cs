using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LazyUtils;
using Terraria;
using TerrariaApi.Server;

namespace Chrome.Shop
{
    [ApiVersion(2, 1)]
    public class Plugin : LazyPlugin
    {
        private static Dictionary<string, IStorageProvider> providers = new Dictionary<string, IStorageProvider>();
        public static void RegisterProvider(IStorageProvider provider) => providers.Add(provider.Name, provider);
        public static IStorageProvider GetProvider(string name) => providers[name];
        
        public Plugin(Main game) : base(game)
        {

        }

        public override void Initialize()
        {
            RegisterProvider(new InventoryProvider());
        }
    }
}
