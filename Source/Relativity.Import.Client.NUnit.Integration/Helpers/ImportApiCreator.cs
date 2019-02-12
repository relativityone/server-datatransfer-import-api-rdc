using kCura.NUnit.Integration;

namespace kCura.Relativity.ImportAPI.IntegrationTests.Helpers
{
	internal static class ImportApiCreator
	{
		public static ImportAPI CreateImportApiWithPasswordAuthentication()
		{
			string url = SharedTestVariables.SERVER_BINDING_TYPE + "://" + SharedTestVariables.RSAPI_SERVER_ADDRESS + "/RelativityWebApi";
			var importApi = new ImportAPI(SharedTestVariables.ADMIN_USERNAME, SharedTestVariables.DEFAULT_PASSWORD, url);
			return importApi;
		}

		public static ImportAPI CreateImportApiWithIntegratedAuthentication()
		{
			string webServiceUrl = ConfigurationProvider.GetWebServiceUrlIntegrated();
			var importApi = new ImportAPI(webServiceUrl);
			return importApi;
		}
	}
}
