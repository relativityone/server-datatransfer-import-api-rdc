// <copyright file="TapiClientModeAvailabilityChecker.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework
{
	using System.Collections.Generic;
	using System.Linq;
	using NUnit.Framework;
	using NUnit.Framework.Internal;
	using Relativity.DataExchange.Transfer;

	public static class TapiClientModeAvailabilityChecker
	{
		public static void InitializeTapiClient(IntegrationTestParameters parameters)
		{
			// default value is TapiClient.None
			TapiClient client = TestExecutionContext.CurrentContext.CurrentTest.Arguments.OfType<TapiClient>().SingleOrDefault();
			InitializeTapiClient(client, parameters);
		}

		public static TapiClient[] GetAvailableTapiClients()
		{
			var availableClients = new List<TapiClient> { TapiClient.Web };
			if (!IntegrationTestHelper.IntegrationTestParameters.SkipDirectModeTests)
			{
				availableClients.Add(TapiClient.Direct);
			}

			if (!IntegrationTestHelper.IntegrationTestParameters.SkipAsperaModeTests)
			{
				availableClients.Add(TapiClient.Aspera);
			}

			return availableClients.ToArray();
		}

		private static void InitializeTapiClient(TapiClient tapiClient, IntegrationTestParameters parameters)
		{
			SkipTestIfModeNotAvailable(tapiClient, parameters);

			if (tapiClient == TapiClient.None && parameters.SkipAsperaModeTests && parameters.SkipDirectModeTests)
			{
				ForceClient(TapiClient.Web);
			}
			else
			{
				ForceClient(tapiClient);
			}
		}

		private static void SkipTestIfModeNotAvailable(TapiClient client, IntegrationTestParameters parameters)
		{
			if (client == TapiClient.Aspera && parameters.SkipAsperaModeTests)
			{
				Assert.Ignore(TestStrings.SkipTestMessage, TapiClient.Aspera);
			}

			if (client == TapiClient.Direct && parameters.SkipDirectModeTests)
			{
				Assert.Ignore(TestStrings.SkipTestMessage, TapiClient.Direct);
			}
		}

		private static void ForceClient(TapiClient tapiClient)
		{
			AppSettings.Instance.TapiForceAsperaClient = tapiClient == TapiClient.Aspera;
			AppSettings.Instance.TapiForceFileShareClient = tapiClient == TapiClient.Direct;
			AppSettings.Instance.TapiForceHttpClient = tapiClient == TapiClient.Web;
		}
	}
}
