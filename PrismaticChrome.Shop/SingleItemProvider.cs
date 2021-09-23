using TShockAPI;

namespace PrismaticChrome.Shop
{
    public abstract class SingleItemProvider : IStorageProvider
    {
        public abstract string Name { get; }
        protected abstract bool TryGiveTo(TSPlayer player, int stack);
        protected abstract bool TryTakeFrom(TSPlayer player, int count);

        public bool TryGiveTo(TSPlayer player, string content) =>
            TryGiveTo(player, int.Parse(content));

        public bool TryTakeFrom(TSPlayer player, int count, out string content, bool inf)
        {
            content = count.ToString();
            return inf || TryTakeFrom(player, count);
        }

        public string SerializeToText(string content) => $"{Name}*{content}";
    }
}