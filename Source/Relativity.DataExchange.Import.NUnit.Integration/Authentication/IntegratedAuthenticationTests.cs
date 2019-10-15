﻿// -----------------------------------------------------------------------------------------------------
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
	using System.Threading.Tasks;

	using global::NUnit.Framework;
	using kCura.Relativity.ImportAPI;
	using kCura.WinEDDS.Exceptions;
	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.RelativityHelpers;
	using Relativity.DataExchange.Transfer;
	using Relativity.Testing.Identification;

	/// <summary>
	/// Represents tests for integrated authentication.
	/// </summary>
	[TestFixture]
	[Feature.DataTransfer.ImportApi.Authentication]
	public class IntegratedAuthenticationTests : ImportJobTestBase
	{
		private const int _TIMEOUT_IN_MS = 70 * 1000;
		private const int _WAIT_TIME_FOR_INSTANCE_SETTING_CHANGE_IN_MS = 30 * 1000;

		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.Integration)]
		[IdentifiedTestCase("511fb3d4-4f31-42d2-90a6-f70d17dce82f", TapiClient.Direct)]
		[IdentifiedTestCase("2f1bd998-e5bf-40be-93d1-5274f53c6aa5", TapiClient.Aspera)]
		[IdentifiedTestCase("47ff5882-2a0b-408d-89e4-63ac23ab0130", TapiClient.Web)]
		[Timeout(_TIMEOUT_IN_MS)]
		public async Task ShouldImportDocumentsWithIntegratedAuthentication(TapiClient client)
		{
			TapiClientModeAvailabilityChecker.SkipTestIfModeNotAvailable(this.TestParameters, client);

			// arrange
			await this.GivenIntegratedAuthenticationIsEnabled().ConfigureAwait(false);
			await this.GivenCurrentUserHasIntegratedAuthenticationProvider().ConfigureAwait(false);
			await this.GivenNoDocumentsArePresentInWorkspace().ConfigureAwait(false);
			this.GivenTheAutoGeneratedDatasetToImport(maxFiles: 5, includeReadOnlyFiles: true);
			this.GivenTheImportJobWithIntegratedAuthentication();
			GivenTheStandardConfigSettings(client, disableNativeLocationValidation: true, disableNativeValidation: true);

			// act
			this.WhenExecutingTheJob();

			// assert
			this.ThenTheImportJobIsSuccessful();
			this.ThenTheImportMessageCountIsNonZero();
			this.ThenTheImportProgressEventsAreRaised();
		}

		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.Integration)]
		[IdentifiedTest("9ad67f7d-d2f0-4a32-96ca-ea40c1631c74")]
		[Timeout(_TIMEOUT_IN_MS)]
		public async Task ShouldFailWhenIntegratedAuthenticationIsDisabled()
		{
			// arrange
			await this.GivenIntegratedAuthenticationIsDisabled().ConfigureAwait(false);
			await this.GivenCurrentUserHasIntegratedAuthenticationProvider().ConfigureAwait(false);

			// act & assert
			this.ThenIntegratedAuthenticationConstructorThrowsInvalidLoginException();
		}

		private Task GivenIntegratedAuthenticationIsEnabled()
		{
			return this.ChangeStateOfIntegratedAuthentication(isEnabled: true);
		}

		private Task GivenIntegratedAuthenticationIsDisabled()
		{
			return this.ChangeStateOfIntegratedAuthentication(isEnabled: false);
		}

		private Task GivenCurrentUserHasIntegratedAuthenticationProvider()
		{
			return UsersHelper.SwitchIntegratedAuthenticationForCurrentUser(this.TestParameters, isEnabled: true);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "kCura.Relativity.ImportAPI.ImportAPI", Justification = "We need to verify that constructor throws exception")]
		private void ThenIntegratedAuthenticationConstructorThrowsInvalidLoginException()
		{
			TestDelegate createImportApiUsingIntegratedAuthenticationAction =
				() => new ImportAPI(this.TestParameters.RelativityWebApiUrl.AbsoluteUri);

			Assert.Throws<InvalidLoginException>(createImportApiUsingIntegratedAuthenticationAction);
		}

		private async Task ChangeStateOfIntegratedAuthentication(bool isEnabled)
		{
			const string section = "Relativity.Authentication";
			const string setting = "UseWindowsAuthentication";
			string newValue = isEnabled.ToString();

			bool wasChanged = await InstanceSettingsHelper
								  .ChangeInstanceSetting(this.TestParameters, section, setting, newValue)
								  .ConfigureAwait(false);

			if (wasChanged)
			{
				await Task.Delay(TimeSpan.FromMilliseconds(_WAIT_TIME_FOR_INSTANCE_SETTING_CHANGE_IN_MS))
					.ConfigureAwait(false);
			}
		}
	}
}