using System;
namespace BiscaNet.Desktop.Data
{
	public class PlayerData
	{
		private PlayerData()
		{ }

		private static PlayerData instance = new PlayerData();
		public static PlayerData GetInstance()
		{
			return instance;
		}

		public string Name;
	}
}
