using System;
using System.Collections.Concurrent;
namespace BiscaNet.Desktop.Systems
{
	public static class GameSync
	{
		private static ConcurrentQueue<Action> actions = new ConcurrentQueue<Action>();

		public static void Defer(Action a)
		{
			actions.Enqueue(a);
		}

		public static void Update()
		{
			Action ac;
			while (actions.TryDequeue(out ac))
			{
				ac.Invoke();
			}
		}
	}
}
