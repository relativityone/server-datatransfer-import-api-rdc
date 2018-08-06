﻿using System;

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
			int workspaceId;
			bool isValidNumber = int.TryParse(workspaceIdAsString, out workspaceId);
			if (!isValidNumber)
			{
				throw new Exception($"Incorrect '{keyName}' value in config file.");
			}

			return workspaceId;
		}

		public static string getWebServiceUrl()
		{
			var webServiceUrlKey = "webServiceUrl";
			var host = GetConfigurationValue(webServiceUrlKey);
			var uri = new Uri(host);
			return $"{uri}RelativityWebApi";
		}

		public static string getWebServiceUrlIntegrated()
		{
			var webServiceUrlKey = "webServiceUrl";
			var host = GetConfigurationValue(webServiceUrlKey);
			var uri = new Uri(host);
			return $"{uri}WindowsAuthWebAPI";
		}
	}
}
