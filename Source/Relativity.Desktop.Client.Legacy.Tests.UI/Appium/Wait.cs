namespace Relativity.Desktop.Client.Legacy.Tests.UI.Appium
{
	using System.Threading;

	internal static class Wait
	{
		public static void Tiny()
		{
			Thread.Sleep(50);
		}

		public static void Small()
		{
			Thread.Sleep(200);
		}

		public static void HalfASecond()
		{
			Thread.Sleep(500);
		}

		public static void Second()
		{
			Thread.Sleep(1000);
		}
	}
}