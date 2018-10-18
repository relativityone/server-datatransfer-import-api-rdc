namespace kCura.Relativity.ImportAPI.IntegrationTests.Helpers
{
	internal static class ConfigurationProvider
	{
		public static string GetConfigurationValue(string key)
		{
			return System.Configuration.ConfigurationManager.AppSettings.Get(key);
		}

		public static string GetWebServiceUrlIntegrated()
		{
			string webServiceUrlKey = "integratedAuthWebApiUrl";
			return GetConfigurationValue(webServiceUrlKey);
		}
	}
}
