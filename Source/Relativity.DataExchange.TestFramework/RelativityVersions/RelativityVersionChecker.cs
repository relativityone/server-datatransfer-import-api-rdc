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
			[RelativityVersion.LanceleafEAU] = new Version(11, 2, 158),
			[RelativityVersion.LanceleafREL438573] = new Version(11, 2, 170),
			[RelativityVersion.Mayapple] = new Version(11, 3, 1),
			[RelativityVersion.MayappleEAU] = new Version(11, 3, 101, 10),
			[RelativityVersion.MayappleExportPDFs] = new Version(11, 3, 16),
			[RelativityVersion.MayappleToggleOff] = new Version(11, 3, 170, 2),
			[RelativityVersion.Ninebark] = new Version(12, 0),
			[RelativityVersion.NinebarkFolderMove] = new Version(12, 0, 111, 3),
			[RelativityVersion.NinebarkToggleOff] = new Version(12, 0, 126, 1),
			[RelativityVersion.Osier] = new Version(12, 1),
			[RelativityVersion.PrairieSmoke] = new Version(12, 2),
			[RelativityVersion.PrairieSmoke0] = new Version(12, 2, 168),
			[RelativityVersion.TigerLily] = new Version(13, 0),
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

		public static void SkipTestIfRelativityVersionIsEqualTo(
			IntegrationTestParameters testParameters,
			RelativityVersion version)
		{
			testParameters = testParameters ?? throw new ArgumentNullException(nameof(testParameters));

			Version currentVersion = GetCurrentRelativityVersion(testParameters);
			Version versionToSkip = VersionsSource[version];

			if (currentVersion == versionToSkip)
			{
				Assert.Ignore(TestStrings.SkipTestMessage, $"current Relativity version {currentVersion} is equal to {versionToSkip} so this test is skipped (most likely there is a bug in this version)");
			}
		}

		public static void SkipTestIfRelativityVersionIsGreaterOrEqual(
			IntegrationTestParameters testParameters,
			RelativityVersion version)
		{
			testParameters = testParameters ?? throw new ArgumentNullException(nameof(testParameters));

			Version currentVersion = GetCurrentRelativityVersion(testParameters);
			Version firstIncompatibleVersion = VersionsSource[version];

			if (currentVersion >= firstIncompatibleVersion)
			{
				Assert.Ignore(TestStrings.SkipTestMessage, $"current Relativity version {currentVersion} is greater than or equal to {firstIncompatibleVersion}");
			}
		}

		public static Version GetCurrentRelativityVersion(IntegrationTestParameters testParameters)
		{
			testParameters = testParameters ?? throw new ArgumentNullException(nameof(testParameters));

			if (cachedRelativityVersion == null)
			{
				IRunningContext runningContext = new RunningContext();
				ImportCredentialManager.WebServiceURL = testParameters.RelativityWebApiUrl.AbsoluteUri;
				ImportCredentialManager.GetCredentials(testParameters.RelativityUserName, testParameters.RelativityPassword, runningContext, () => nameof(RelativityVersionChecker));
				cachedRelativityVersion = runningContext.RelativityVersion;
				Console.WriteLine($"Relativity version of {ImportCredentialManager.WebServiceURL} is {cachedRelativityVersion}");
			}

			return cachedRelativityVersion;
		}
	}
}