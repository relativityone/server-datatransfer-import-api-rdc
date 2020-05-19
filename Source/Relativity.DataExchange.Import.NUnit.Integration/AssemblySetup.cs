// ----------------------------------------------------------------------------
// <copyright file="AssemblySetup.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit.Integration
{
	using System.Collections.Generic;
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.RelativityHelpers;

	[SetUpFixture]
	[System.Diagnostics.CodeAnalysis.SuppressMessage(
		"Microsoft.Design",
		"CA1053:StaticHolderTypesShouldNotHaveConstructors",
		Justification = "NUnit requires AssemblySetup to be non static class.")]
	public class AssemblySetup
	{
		public static IntegrationTestParameters TestParameters
		{
			get;
			set;
		}

		[OneTimeSetUp]
		public static async Task SetupAsync()
		{
			TestParameters = IntegrationTestHelper.Create();
			if (TestParameters.SkipIntegrationTests)
			{
				return;
			}

			await UserSetup().ConfigureAwait(false);

			// await SecuritySetup().ConfigureAwait(false); TODO : REL-398159
		}

		[OneTimeTearDown]
		public static void TearDown()
		{
			IntegrationTestHelper.Destroy(TestParameters);
			TestParameters = null;
		}

		public static async Task<IntegrationTestParameters> ResetContextAsync()
		{
			TearDown();
			await SetupAsync().ConfigureAwait(false);
			return TestParameters;
		}

		private static async Task UserSetup()
		{
			// int level1GroupId = 1015028;
			// int level2GroupId = 1015029;
			// int level3GroupId = 1015030;
			// await UsersHelper.EnsureUser(TestParameters, "Level1", "User", "Level1User!", new[] { level1GroupId }).ConfigureAwait(false);
			// await UsersHelper.EnsureUser(TestParameters, "Level2", "User", "Level2User!", new[] { level2GroupId }).ConfigureAwait(false);
			// await UsersHelper.EnsureUser(TestParameters, "Level3", "User", "Level3User!", new[] { level3GroupId }).ConfigureAwait(false);
			await Task.Delay(1).ConfigureAwait(false);
		}

		// private static async Task SecuritySetup()
		// {
		// 	List<int> folderIds = await FolderHelper.CreateFolders(TestParameters, new[] { "Level1 Permissions", "Level2 Permissions", "Level3 Permissions", "Aaa", "aaa", "Aaa ", "aaa   " }).ConfigureAwait(false);
		// 	await FolderHelper.SetItemLevelSecurity(TestParameters, folderIds[0], "Level 1").ConfigureAwait(false);
		// 	await FolderHelper.SetItemLevelSecurity(TestParameters, folderIds[1], "Level 2").ConfigureAwait(false);
		// 	await FolderHelper.SetItemLevelSecurity(TestParameters, folderIds[2], "Level 3").ConfigureAwait(false);
		// }
	}
}