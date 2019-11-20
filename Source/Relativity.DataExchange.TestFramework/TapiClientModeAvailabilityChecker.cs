// <copyright file="TapiClientModeAvailabilityChecker.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework
{
	using System;

	using NUnit.Framework;

	using Relativity.DataExchange.Transfer;

	public static class TapiClientModeAvailabilityChecker
	{
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
