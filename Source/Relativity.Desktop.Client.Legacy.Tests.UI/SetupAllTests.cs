using System.Configuration;
using System.Linq;
using Microsoft.Win32;
using NUnit.Framework;

namespace Relativity.Desktop.Client.Legacy.Tests.UI
{
	[SetUpFixture]
	public class SetupAllTests
	{
		private const string RegistryKeyName = @"Software\kCura\Relativity";
		private const string WebApiUrlRegistryKeyValue = "WebServiceURL";
		private string originalRelativityWebApiUrl;
		private bool originalRegistryValueExists;
		private WinAppDriverRunner driverRunner;

		[OneTimeSetUp]
		public void TestFixtureSetUp()
		{
			SetWebApiUrlToRegistry();
			driverRunner = new WinAppDriverRunner();
			driverRunner.Run();
		}

		[OneTimeTearDown]
		public void TestFixtureTearDown()
		{
			RestoreOriginalWebApiUrlRegistryValue();
			driverRunner?.Dispose();
			driverRunner = null;
		}

		private void SetWebApiUrlToRegistry()
		{
			using (var relativityKey = Registry.CurrentUser.OpenSubKey(RegistryKeyName, true) ??
			                           Registry.CurrentUser.CreateSubKey(RegistryKeyName,
				                           RegistryKeyPermissionCheck.ReadWriteSubTree))
			{
				if (relativityKey.GetValueNames().Contains(WebApiUrlRegistryKeyValue))
				{
					originalRegistryValueExists = true;
					originalRelativityWebApiUrl = relativityKey.GetValue(WebApiUrlRegistryKeyValue) as string;
				}

				relativityKey.SetValue(WebApiUrlRegistryKeyValue,
					ConfigurationManager.AppSettings["RelativityWebApiUrl"], RegistryValueKind.String);
			}
		}

		private void RestoreOriginalWebApiUrlRegistryValue()
		{
			if (originalRegistryValueExists)
			{
				using (var relativityKey = Registry.CurrentUser.OpenSubKey(RegistryKeyName, true))
				{
					relativityKey.SetValue(WebApiUrlRegistryKeyValue, originalRelativityWebApiUrl,
						RegistryValueKind.String);
				}
			}
		}
	}
}