using NUnit.Framework;

namespace Relativity.Desktop.Client.Legacy.Tests.UI
{
	[SetUpFixture]
	public class SetupAllTests
	{
		private WinAppDriverRunner driverRunner;

		[OneTimeSetUp]
		public void TestFixtureSetUp()
		{
			driverRunner = new WinAppDriverRunner();
			driverRunner.Run();
		}

		[OneTimeTearDown]
		public void TestFixtureTearDown()
		{
			driverRunner?.Dispose();
			driverRunner = null;
		}
	}
}