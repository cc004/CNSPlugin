using System.Linq;
using System.Text;
using LazyUtils;
using TShockAPI;

namespace Chrome.PVP
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

        private float GetPercent() => killcount * 100f / (killcount + deathcount);

        public string GetQueryString()
        {
            var groupname = TShock.UserAccounts.GetUserAccountByName(name)?.Group;
            var group = TShock.Groups.FirstOrDefault(g => g.Name == groupname);
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("--- ����֮��-����PVP ---");
            stringBuilder.AppendLine($"- ��ң�{name} ��λ��{((@group != null) ? @group.Prefix : "δ֪")}");
            stringBuilder.AppendLine($"- ����: {score}");
            stringBuilder.AppendLine(
                $"- ս����{killcount.Color("A2D883")} ʤ {deathcount.Color("D8A183")} �� ʤ�ʣ�{GetPercent():0.00}��");
            return stringBuilder.ToString();
        }

        public string GetShortMessage()
        {
            return $"{name}: {killcount} ʤ {deathcount} ��, ʤ�� {GetPercent():0.00}��";
        }
	}
}
