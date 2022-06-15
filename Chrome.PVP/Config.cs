using System.Collections.Generic;
using LazyUtils;
using Microsoft.Xna.Framework;
using TShockAPI;

namespace Chrome.PVP
{
    [Config]
	public class Config : Config<Config>
	{
        public class Group
        {
            public string name;
            public int lower;
            public int upper;
        }

        public Group[] Groups = new Group[0];

		public class WeaponDebuffInfo
        {
            public int ID;

            public int[] AllowedProj;

            public Dictionary<int, int> Debuff;
        }
        
		public int Life = 800;


		public Point AreaLeftTop = new Point(2000, 250);


		public Point AreaRightButtom = new Point(2100, 350);


		public int[] BanndProj  =
		{
			502,
			503,
			132,
			684,
			634,
			635,
			459,
			460,
			461
		};


		public WeaponDebuffInfo[] WeaponDebuff =
		{
			new WeaponDebuffInfo
			{
				ID = 1254,
				AllowedProj = new []
				{
					242
				},
				Debuff = new Dictionary<int, int>
				{
					{
						33,
						600
					},
					{
						36,
						600
					}
				}
			}
		};

        public int[] RegionBuff = new int[1]
        {
            199
        };

        public Dictionary<int, NetItem> Accessory = new Dictionary<int, NetItem>
        {
            {
                59,
                new NetItem(2763, 1, 65)
            },
            {
                60,
                new NetItem(2764, 1, 65)
            },
            {
                61,
                new NetItem(2765, 1, 65)
            },
            {
                62,
                new NetItem(3998, 1, 42)
            },
            {
                63,
                new NetItem(2423, 1, 42)
            },
            {
                64,
                new NetItem(1165, 1, 42)
            },
            {
                65,
                new NetItem(3015, 1, 72)
            },
            {
                66,
                new NetItem(4005, 1, 72)
            },
            {
                67,
                new NetItem(4989, 1, 72)
            },
            {
                69,
                new NetItem(3583, 1, 0)
            },
            {
                70,
                new NetItem(1567, 1, 0)
            },
            {
                71,
                new NetItem(668, 1, 0)
            },
            {
                72,
                new NetItem(3580, 1, 0)
            },
            {
                73,
                new NetItem(156, 1, 0)
            },
            {
                74,
                new NetItem(3200, 1, 0)
            },
            {
                79,
                new NetItem(1050, 1, 0)
            },
            {
                81,
                new NetItem(3559, 1, 0)
            },
            {
                84,
                new NetItem(1050, 1, 0)
            },
            {
                86,
                new NetItem(1050, 1, 0)
            },
            {
                87,
                new NetItem(3559, 1, 0)
            },
            {
                92,
                new NetItem(2430, 1, 0)
            },
            {
                93,
                new NetItem(3572, 1, 0)
            }
        };

        public Rectangle GetArea()
        {
            return new Rectangle(AreaLeftTop.X, AreaLeftTop.Y, AreaRightButtom.X - AreaLeftTop.X,
                AreaRightButtom.Y - AreaLeftTop.Y);
        }
    }
}
