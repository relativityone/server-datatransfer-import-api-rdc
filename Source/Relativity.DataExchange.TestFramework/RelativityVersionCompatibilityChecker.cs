// ----------------------------------------------------------------------------
// <copyright file="RelativityVersionCompatibilityChecker.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework
{
	using System;

	using NUnit.Framework;

	public static class RelativityVersionCompatibilityChecker
	{
		private static Version cachedRelativityVersion;

		public static void SkipTestIfRelativityVersionIsNotCompatible(
			IntegrationTestParameters testParameters,
			Version firstCompatibleVersion)
		{
			testParameters = testParameters ?? throw new ArgumentNullException(nameof(testParameters));

			Version currentVersion = GetCurrentRelativityVersion(testParameters);
			if (currentVersion < firstCompatibleVersion)
			{
				Assert.Ignore(TestStrings.SkipTestMessage, $"current Relativity version {currentVersion} is lower than required {firstCompatibleVersion}");
			}
		}

		public static Version GetCurrentRelativityVersion(IntegrationTestParameters testParameters)
		{
			testParameters = testParameters ?? throw new ArgumentNullException(nameof(testParameters));

			if (cachedRelativityVersion == null)
			{
				IRunningContext runningContext = new RunningContext();
				ImportCredentialManager.WebServiceURL = testParameters.RelativityWebApiUrl.AbsoluteUri;
				ImportCredentialManager.GetCredentials(testParameters.RelativityUserName, testParameters.RelativityPassword, runningContext);
				cachedRelativityVersion = runningContext.RelativityVersion;
			}

			return cachedRelativityVersion;
		}
	}
}