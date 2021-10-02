using System.Linq;
using System.Text;
using LazyUtils;
using TShockAPI;

namespace PrismaticChrome.PVP
{
    internal class PvpInfo : PlayerConfigBase<PvpInfo>
    {
		public int score { get; set; }
		public int killcount { get; set; }
		public int deathcount { get; set; }
		public long causedamage { get; set; }
		public long getdamage { get; set; }
		public int kill { get; set; }
		public int death { get; set; }

        private float GetPercent() => (float) killcount / (killcount + deathcount);

        public string GetQueryString()
        {
            var groupname = TShock.UserAccounts.GetUserAccountByName(name)?.Group;
            var group = TShock.Groups.FirstOrDefault(g => g.Name == groupname);
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("--- 流光之城-诸仙PVP ---");
            stringBuilder.AppendLine($"- 玩家：{name} 段位：{((@group != null) ? @group.Prefix : "未知")}");
            stringBuilder.AppendLine($"- 积分: {score}");
            stringBuilder.AppendLine(
                $"- 战绩：{killcount.Color("A2D883")} 胜 {deathcount.Color("D8A183")} 负 胜率：{GetPercent():0.00}％");
            return stringBuilder.ToString();
        }

        public string GetShortMessage()
        {
            return $"{name}: {killcount} 胜 {deathcount} 负, 胜率 {GetPercent():0.00}％";
        }
	}
}
