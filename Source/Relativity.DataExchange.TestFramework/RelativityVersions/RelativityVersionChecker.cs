// ----------------------------------------------------------------------------
// <copyright file="RelativityVersionChecker.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework.RelativityVersions
{
	using System;
	using System.Collections.Generic;
	using NUnit.Framework;

	public static class RelativityVersionChecker
	{
		private static readonly Dictionary<RelativityVersion, Version> VersionsSource = new Dictionary<RelativityVersion, Version>()
		{
			[RelativityVersion.Larkspur] = new Version(10, 0, 1),
			[RelativityVersion.Blazingstar] = new Version(10, 1, 1),
			[RelativityVersion.Foxglove] = new Version(10, 2, 1),
			[RelativityVersion.Goatsbeard] = new Version(10, 3, 1),
			[RelativityVersion.Indigo] = new Version(11, 0, 1),
			[RelativityVersion.Juniper] = new Version(11, 1, 1),
			[RelativityVersion.Lanceleaf] = new Version(11, 2, 1),
			[RelativityVersion.LanceleafAA1] = new Version(11, 2, 94),
			[RelativityVersion.LanceleafREL425922] = new Version(11, 2, 158),
			[RelativityVersion.Mayapple] = new Version(11, 3, 1),
			[RelativityVersion.MayappleExportPDFs] = new Version(11, 3, 16),
		};

		private static Version cachedRelativityVersion;

		public static bool VersionIsLowerThan(IntegrationTestParameters testParameters, RelativityVersion version)
		{
			return RelativityVersionChecker.GetCurrentRelativityVersion(testParameters)
			       < RelativityVersionChecker.VersionsSource[version];
		}

		public static void SkipTestIfRelativityVersionIsLowerThan(
			IntegrationTestParameters testParameters,
			RelativityVersion version)
		{
			testParameters = testParameters ?? throw new ArgumentNullException(nameof(testParameters));

			Version currentVersion = GetCurrentRelativityVersion(testParameters);
			Version firstCompatibleVersion = VersionsSource[version];

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