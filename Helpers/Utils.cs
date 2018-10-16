namespace kCura.Relativity.ImportAPI.IntegrationTests.Helpers
{
	public static class Utils
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
