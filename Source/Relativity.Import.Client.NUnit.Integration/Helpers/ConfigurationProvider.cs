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
			return GetConfigurationValue("integratedAuthWebApiUrl");
		}

		public static string GetEDDSDatabaseConnectionString()
		{
			return GetConfigurationValue("connectionString");
		}
	}
}