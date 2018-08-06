namespace kCura.Relativity.ImportAPI.IntegrationTests.Helpers
{
	internal static class ImportApiCreator
	{
		public static ImportAPI CreateImportApiWithPasswordAuthentication()
		{
			var username = Utils.GetConfigurationValue("username");
			var password = Utils.GetConfigurationValue("password");

			var webServiceUrl = Utils.getWebServiceUrl();
			var importApi = new ImportAPI(username, password, webServiceUrl);
			return importApi;
		}

		public static ImportAPI CreateImportApiWithIntegratedAuthentication()
		{
			var webServiceUrl = Utils.getWebServiceUrlIntegrated();
			var importApi = new ImportAPI(webServiceUrl);
			return importApi;
		}
	}
}
