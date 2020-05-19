// <copyright file="TapiClientModeAvailabilityChecker.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework
{
	using System;
	using System.Linq;
	using NUnit.Framework;
	using NUnit.Framework.Internal;
	using Relativity.DataExchange.Transfer;

	public static class TapiClientModeAvailabilityChecker
	{
		public static void SkipTestIfTestParameterTransferModeNotAvailable(IntegrationTestParameters parameters)
		{
			foreach (var testArgument in TestExecutionContext.CurrentContext.CurrentTest.Arguments.OfType<TapiClient>())
			{
				TapiClientModeAvailabilityChecker.SkipTestIfModeNotAvailable(parameters, testArgument);
				return;
			}
		}

		public static void SkipTestIfModeNotAvailable(IntegrationTestParameters parameters, TapiClient client)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

			if ((client == TapiClient.Aspera && parameters.SkipAsperaModeTests) ||
				(client == TapiClient.Direct && parameters.SkipDirectModeTests))
			{
				Assert.Ignore(TestStrings.SkipTestMessage, $"{client}");
			}
		}
	}
}
