using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LazyUtils;
using Newtonsoft.Json;
using Terraria;
using TerrariaApi.Server;
using TrProtocol;

namespace NetDataRecorder
{
    [ApiVersion(2, 1)]
    public class Plugin : LazyPlugin
    {
        public Plugin(Main game) : base(game)
        {
        }

        public override void Initialize()
        {
            var serializer = new PacketSerializer(false);
            var logger = new StreamWriter(File.Open("netdump.log", FileMode.Append, FileAccess.Write));
            ServerApi.Hooks.NetGetData.Register(this, args =>
            {
                using (var ms = new MemoryStream(args.Msg.readBuffer, args.Index - 3, args.Length))
                using (var br = new BinaryReader(ms))
                {
                    try
                    {
                        var packet = serializer.Deserialize(br);
                        logger.WriteLine($"[{DateTime.Now}] " +
                                         $"Index: {args.Msg.whoAmI} " +
                                         $"Remote: {Netplay.Clients[args.Msg.whoAmI].Socket.GetRemoteAddress().GetFriendlyName()} " +
                                         $"Name: {Main.player[args.Msg.whoAmI]?.name} " +
                                         $"Type: {packet.GetType()} " +
                                         $"Data: {JsonConvert.SerializeObject(packet)}");
                    }
                    catch
                    {
                        logger.WriteLine($"[{DateTime.Now}] " +
                                         $"Index: {args.Msg.whoAmI} " +
                                         $"Remote: {Netplay.Clients[args.Msg.whoAmI].Socket.GetRemoteAddress().GetFriendlyName()} " +
                                         $"Name: {Main.player[args.Msg.whoAmI]?.name} " +
                                         $"Raw: {string.Join(" ", ms.ToArray().Select(b => b.ToString("x2")))}");
                    }
                }
            });
        }
    }
}
