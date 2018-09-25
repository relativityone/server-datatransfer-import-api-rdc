using System;
using kCura.Relativity.ImportAPI.IntegrationTests.Tests;

namespace kCura.Relativity.ImportAPI.IntegrationTests.Helpers
{
	public static class Utils
	{
		public static string GetConfigurationValue(string key)
		{
			return System.Configuration.ConfigurationManager.AppSettings.Get(key);
		}

		public static int GetWorkspaceId()
		{
			const string keyName = "workspaceId";
			string workspaceIdAsString = GetConfigurationValue(keyName);
			bool isValidNumber = int.TryParse(workspaceIdAsString, out int workspaceId);
			if (!isValidNumber)
			{
				throw new ImportApiTestException($"Incorrect '{keyName}' value in config file.");
			}

			return workspaceId;
		}

		public static string GetWebServiceUrl()
		{
			string webServiceUrlKey = "webServiceUrl";
			string host = GetConfigurationValue(webServiceUrlKey);
			var uri = new Uri(host);
			return $"{uri}RelativityWebApi";
		}

		public static string GetWebServiceUrlIntegrated()
		{
			string webServiceUrlKey = "webServiceUrl";
			string host = GetConfigurationValue(webServiceUrlKey);
			var uri = new Uri(host);
			return $"{uri}WindowsAuthWebAPI";
		}
	}
}
