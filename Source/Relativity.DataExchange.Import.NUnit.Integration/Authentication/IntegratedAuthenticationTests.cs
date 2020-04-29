// -----------------------------------------------------------------------------------------------------
// <copyright file="IntegratedAuthenticationTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents tests for authentication in ImportAPI.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit.Integration.Authentication
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using kCura.Relativity.ImportAPI;
	using kCura.WinEDDS.Exceptions;

	using Relativity.DataExchange.Import.NUnit.Integration.Dto;
	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.Import.JobExecutionContext;
	using Relativity.DataExchange.TestFramework.RelativityHelpers;
	using Relativity.DataExchange.Transfer;
	using Relativity.Testing.Identification;

	[TestFixture]
	[Feature.DataTransfer.ImportApi.Authentication]
	[Explicit("These tests don't work on Trident, because they are executed there in a non-interactive process.")]
	public class IntegratedAuthenticationTests : ImportJobTestBase<NativeImportExecutionContext>
	{
		private const int _WAIT_TIME_FOR_INSTANCE_SETTING_CHANGE_IN_MS = 30 * 1000;

		/// <summary>
		/// it's 30s for cache invalidation and 40s for tests execution.
		/// in negative case we need to wait 30s for implicit flow timeout, 10s is for other calls.
		/// in positive cases 40s seems to be reasonable limit for importing 5 small files.
		/// </summary>
		private const int _TIMEOUT_IN_MS = 70 * 1000;

		[OneTimeTearDown]
		public static Task OneTimeTearDown()
		{
			return Task.WhenAll(
				UsersHelper.SwitchIntegratedAuthenticationForCurrentUserAsync(AssemblySetup.TestParameters, isEnabled: false),
				ChangeStateOfIntegratedAuthentication(isEnabled: false));
		}

		[Category(TestCategories.ImportDoc)]
		[IdentifiedTest("9ad67f7d-d2f0-4a32-96ca-ea40c1631c74")]
		[Timeout(_TIMEOUT_IN_MS)]
		public static async Task ShouldFailWhenIntegratedAuthenticationIsDisabled()
		{
			// ARRANGE
			await ChangeStateOfIntegratedAuthentication(false).ConfigureAwait(false);
			await UsersHelper.SwitchIntegratedAuthenticationForCurrentUserAsync(AssemblySetup.TestParameters, isEnabled: true).ConfigureAwait(false);

			// ACT
			TestDelegate createImportApiUsingIntegratedAuthenticationAction =
				() => new ImportAPI(AssemblySetup.TestParameters.RelativityWebApiUrl.AbsoluteUri);

			// ASSERT
			Assert.Throws<InvalidLoginException>(createImportApiUsingIntegratedAuthenticationAction);
		}

		[Category(TestCategories.ImportDoc)]
		[IdentifiedTestCase("511fb3d4-4f31-42d2-90a6-f70d17dce82f", TapiClient.Direct)]
		[IdentifiedTestCase("2f1bd998-e5bf-40be-93d1-5274f53c6aa5", TapiClient.Aspera)]
		[IdentifiedTestCase("47ff5882-2a0b-408d-89e4-63ac23ab0130", TapiClient.Web)]
		[Timeout(_TIMEOUT_IN_MS)]
		public async Task ShouldImportDocumentsWithIntegratedAuthentication(TapiClient client)
		{
			TapiClientModeAvailabilityChecker.SkipTestIfModeNotAvailable(AssemblySetup.TestParameters, client);

			// ARRANGE
			await ChangeStateOfIntegratedAuthentication(true).ConfigureAwait(false);
			await UsersHelper.SwitchIntegratedAuthenticationForCurrentUserAsync(AssemblySetup.TestParameters, isEnabled: true).ConfigureAwait(false);
			await RdoHelper.DeleteAllObjectsByTypeAsync(AssemblySetup.TestParameters, (int)ArtifactType.Document).ConfigureAwait(false);

			ForceClient(client);
			kCura.WinEDDS.Config.ConfigSettings["DisableNativeLocationValidation"] = true;
			kCura.WinEDDS.Config.ConfigSettings["DisableNativeValidation"] = true;

			this.JobExecutionContext.InitializeImportApiWithIntegratedAuthentication(this.TestParameters, NativeImportSettingsProvider.NativeFilePathSourceDocumentImportSettings);

			const int NumberOfFilesToImport = 5;
			IEnumerable<DefaultImportDto> importData = DefaultImportDto.GetRandomTextFiles(this.TempDirectory.Directory, NumberOfFilesToImport);

			// ACT
			ImportTestJobResult results = this.JobExecutionContext.Execute(importData);

			// ASSERT
			this.ThenTheImportJobIsSuccessful(results, NumberOfFilesToImport);
			Assert.That(results.NumberOfJobMessages, Is.GreaterThan(0));
			Assert.That(results.NumberOfCompletedRows, Is.EqualTo(NumberOfFilesToImport));
			ThenTheJobCompletedInCorrectTransferMode(results, client);
		}

		private static async Task ChangeStateOfIntegratedAuthentication(bool isEnabled)
		{
			const string section = "Relativity.Authentication";
			const string setting = "UseWindowsAuthentication";
			string newValue = isEnabled.ToString();

			bool wasChanged = await InstanceSettingsHelper
								  .ChangeInstanceSetting(AssemblySetup.TestParameters, section, setting, newValue)
								  .ConfigureAwait(false);

			if (wasChanged)
			{
				await Task.Delay(TimeSpan.FromMilliseconds(_WAIT_TIME_FOR_INSTANCE_SETTING_CHANGE_IN_MS))
					.ConfigureAwait(false);
			}
		}
	}
}