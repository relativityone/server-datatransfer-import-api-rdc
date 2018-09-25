namespace kCura.Relativity.ImportAPI.IntegrationTests.Helpers
{
	internal static class ImportApiCreator
	{
		public static ImportAPI CreateImportApiWithPasswordAuthentication()
		{
			string username = Utils.GetConfigurationValue("username");
			string password = Utils.GetConfigurationValue("password");

			string webServiceUrl = Utils.GetWebServiceUrl();
			var importApi = new ImportAPI(username, password, webServiceUrl);
			return importApi;
		}

		public static ImportAPI CreateImportApiWithIntegratedAuthentication()
		{
			string webServiceUrl = Utils.GetWebServiceUrlIntegrated();
			var importApi = new ImportAPI(webServiceUrl);
			return importApi;
		}
	}
}
