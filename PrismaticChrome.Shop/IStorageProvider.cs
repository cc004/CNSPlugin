using TShockAPI;

namespace PrismaticChrome.Shop
{
    public interface IStorageProvider
    {
        string Name { get; }
        bool TryGiveTo(TSPlayer player, string content);
        bool TryTakeFrom(TSPlayer player, int count, out string content);
        string SerializeToText(string content);
    }
}