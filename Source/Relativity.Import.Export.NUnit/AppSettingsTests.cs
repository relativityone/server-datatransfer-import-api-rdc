// -----------------------------------------------------------------------------------------------------
// <copyright file="AppSettingsTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents tests for all application settings.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Import.Export.NUnit
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.Runtime.Serialization;
	using System.Runtime.Serialization.Formatters.Binary;

	using global::NUnit.Framework;

	using Relativity.Import.Export;
	using Relativity.Import.Export.Io;
	using Relativity.Import.Export.TestFramework;

	/// <summary>
	/// Represents tests for all application settings.
	/// </summary>
	[TestFixture]
	[System.Diagnostics.CodeAnalysis.SuppressMessage(
		"Microsoft.Design",
		"CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
		Justification = "The test class handles the disposal.")]
	public class AppSettingsTests
	{
		private const string TestRegistrySubKey = @"Software\Relativity\ImportExportTests";
		private IAppSettings settings;

		[SetUp]
		public void Setup()
		{
			AppSettingsManager.RegistrySubKeyName = TestRegistrySubKey;
			DeleteTestSubKey();
			this.settings = AppSettingsManager.Create(false);
		}

		[TearDown]
		public void Teardown()
		{
			AppSettingsManager.RegistrySubKeyName = TestRegistrySubKey;
			DeleteTestSubKey();
			this.settings = null;
		}

		[Test]
		public void ShouldDumpTheMappingInfo()
		{
			this.settings = null;
			AppSettingsManager.BuildAttributeDictionary();
			var groups = AppSettingsManager.AppSettingAttributes.GroupBy(x => x.Value.Section).ToList();
			var appSettingsType = typeof(IAppSettings);
			foreach (var group in groups)
			{
				string sectionName = !string.IsNullOrEmpty(group.Key) ? group.Key : "Unmapped";
				Console.WriteLine($"Dumping settings from the '{sectionName}' section.");
				foreach (var item in group)
				{
					string key = item.Key.Replace($"{appSettingsType.FullName}.", string.Empty);
					Console.WriteLine($"{key}={item.Value.DefaultValue} (default)");
				}

				Console.WriteLine(string.Empty);
			}
		}

		[Test]
		public void ShouldGetAndSetTheApplicationNameSetting()
		{
			Assert.That(this.settings.ApplicationName, Is.Null.Or.Empty);
			string expectedValue = RandomHelper.NextString(10, 20);
			this.settings.ApplicationName = expectedValue;
			Assert.That(this.settings.ApplicationName, Is.EqualTo(expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheAuditLevelSetting()
		{
			Assert.That(this.settings.AuditLevel, Is.EqualTo(AppSettingsConstants.AuditLevelDefaultValue));
			string expectedValue = RandomHelper.NextString(10, 20);
			this.settings.AuditLevel = expectedValue;
			Assert.That(this.settings.AuditLevel, Is.EqualTo(expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheCreateErrorForInvalidDateSetting()
		{
			Assert.That(
				this.settings.CreateErrorForInvalidDate,
				Is.EqualTo(AppSettingsConstants.CreateErrorForInvalidDateDefaultValue));
			bool expectedValue = RandomHelper.NextBoolean();
			this.settings.CreateErrorForInvalidDate = expectedValue;
			Assert.That(this.settings.CreateErrorForInvalidDate, Is.EqualTo(expectedValue));
			this.settings.CreateErrorForInvalidDate = !expectedValue;
			Assert.That(this.settings.CreateErrorForInvalidDate, Is.EqualTo(!expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheCreateFoldersInWebApiSetting()
		{
			Assert.That(
				this.settings.CreateFoldersInWebApi,
				Is.EqualTo(AppSettingsConstants.CreateFoldersInWebApiDefaultValue));
			bool expectedValue = RandomHelper.NextBoolean();
			this.settings.CreateFoldersInWebApi = expectedValue;
			Assert.That(this.settings.CreateFoldersInWebApi, Is.EqualTo(expectedValue));
			this.settings.CreateFoldersInWebApi = !expectedValue;
			Assert.That(this.settings.CreateFoldersInWebApi, Is.EqualTo(!expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheCreateErrorForEmptyNativeFileSetting()
		{
			Assert.That(
				this.settings.CreateErrorForEmptyNativeFile,
				Is.EqualTo(AppSettingsConstants.CreateErrorForEmptyNativeFileDefaultValue));
			bool expectedValue = RandomHelper.NextBoolean();
			this.settings.CreateErrorForEmptyNativeFile = expectedValue;
			Assert.That(this.settings.CreateErrorForEmptyNativeFile, Is.EqualTo(expectedValue));
			this.settings.CreateErrorForEmptyNativeFile = !expectedValue;
			Assert.That(this.settings.CreateErrorForEmptyNativeFile, Is.EqualTo(!expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheDefaultMaxErrorCountSetting()
		{
			Assert.That(
				this.settings.DefaultMaxErrorCount,
				Is.EqualTo(AppSettingsConstants.DefaultMaxErrorCountDefaultValue));
			int expectedValue = RandomHelper.NextInt32(1, 1000);
			this.settings.DefaultMaxErrorCount = expectedValue;
			Assert.That(this.settings.DefaultMaxErrorCount, Is.EqualTo(expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheDisableImageLocationValidationSetting()
		{
			Assert.That(
				this.settings.DisableImageLocationValidation,
				Is.EqualTo(AppSettingsConstants.DisableImageLocationValidationDefaultValue));
			bool expectedValue = RandomHelper.NextBoolean();
			this.settings.DisableImageLocationValidation = expectedValue;
			Assert.That(this.settings.DisableImageLocationValidation, Is.EqualTo(expectedValue));
			this.settings.DisableImageLocationValidation = !expectedValue;
			Assert.That(this.settings.DisableImageLocationValidation, Is.EqualTo(!expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheDisableImageTypeValidationSetting()
		{
			Assert.That(
				this.settings.DisableImageTypeValidation,
				Is.EqualTo(AppSettingsConstants.DisableImageTypeValidationDefaultValue));
			bool expectedValue = RandomHelper.NextBoolean();
			this.settings.DisableImageTypeValidation = expectedValue;
			Assert.That(this.settings.DisableImageTypeValidation, Is.EqualTo(expectedValue));
			this.settings.DisableImageTypeValidation = !expectedValue;
			Assert.That(this.settings.DisableImageTypeValidation, Is.EqualTo(!expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheDisableTextFileEncodingCheckSetting()
		{
			Assert.That(
				this.settings.DisableTextFileEncodingCheck,
				Is.EqualTo(AppSettingsConstants.DisableTextFileEncodingCheckDefaultValue));
			bool expectedValue = RandomHelper.NextBoolean();
			this.settings.DisableTextFileEncodingCheck = expectedValue;
			Assert.That(this.settings.DisableTextFileEncodingCheck, Is.EqualTo(expectedValue));
			this.settings.DisableTextFileEncodingCheck = !expectedValue;
			Assert.That(this.settings.DisableTextFileEncodingCheck, Is.EqualTo(!expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheDisableThrowOnIllegalCharactersSetting()
		{
			Assert.That(
				this.settings.DisableThrowOnIllegalCharacters,
				Is.EqualTo(AppSettingsConstants.DisableThrowOnIllegalCharactersDefaultValue));
			bool expectedValue = RandomHelper.NextBoolean();
			this.settings.DisableThrowOnIllegalCharacters = expectedValue;
			Assert.That(this.settings.DisableThrowOnIllegalCharacters, Is.EqualTo(expectedValue));
			this.settings.DisableThrowOnIllegalCharacters = !expectedValue;
			Assert.That(this.settings.DisableThrowOnIllegalCharacters, Is.EqualTo(!expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheDynamicBatchResizingOnSetting()
		{
			Assert.That(
				this.settings.DynamicBatchResizingOn,
				Is.EqualTo(AppSettingsConstants.DynamicBatchResizingOnDefaultValue));
			bool expectedValue = RandomHelper.NextBoolean();
			this.settings.DynamicBatchResizingOn = expectedValue;
			Assert.That(this.settings.DynamicBatchResizingOn, Is.EqualTo(expectedValue));
			this.settings.DynamicBatchResizingOn = !expectedValue;
			Assert.That(this.settings.DynamicBatchResizingOn, Is.EqualTo(!expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheEnableCaseSensitiveSearchOnImportSetting()
		{
			Assert.That(
				this.settings.EnableCaseSensitiveSearchOnImport,
				Is.EqualTo(AppSettingsConstants.EnableCaseSensitiveSearchOnImportDefaultValue));
			bool expectedValue = RandomHelper.NextBoolean();
			this.settings.EnableCaseSensitiveSearchOnImport = expectedValue;
			Assert.That(this.settings.EnableCaseSensitiveSearchOnImport, Is.EqualTo(expectedValue));
			this.settings.EnableCaseSensitiveSearchOnImport = !expectedValue;
			Assert.That(this.settings.EnableCaseSensitiveSearchOnImport, Is.EqualTo(!expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheEnableSingleModeImportSetting()
		{
			Assert.That(
				this.settings.EnableSingleModeImport,
				Is.EqualTo(AppSettingsConstants.EnableSingleModeImportDefaultValue));
			bool expectedValue = RandomHelper.NextBoolean();
			this.settings.EnableSingleModeImport = expectedValue;
			Assert.That(this.settings.EnableSingleModeImport, Is.EqualTo(expectedValue));
			this.settings.EnableSingleModeImport = !expectedValue;
			Assert.That(this.settings.EnableSingleModeImport, Is.EqualTo(!expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheEnforceVersionCompatibilityCheckSetting()
		{
			Assert.That(
				this.settings.EnforceVersionCompatibilityCheck,
				Is.EqualTo(AppSettingsConstants.EnforceVersionCompatibilityCheckDefaultValue));
			bool expectedValue = RandomHelper.NextBoolean();
			this.settings.EnforceVersionCompatibilityCheck = expectedValue;
			Assert.That(this.settings.EnforceVersionCompatibilityCheck, Is.EqualTo(expectedValue));
			this.settings.EnforceVersionCompatibilityCheck = !expectedValue;
			Assert.That(this.settings.EnforceVersionCompatibilityCheck, Is.EqualTo(!expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheExportBatchSizeSetting()
		{
			Assert.That(
				this.settings.ExportBatchSize,
				Is.EqualTo(AppSettingsConstants.ExportBatchSizeDefaultValue));
			int expectedValue = RandomHelper.NextInt32(1, 1000);
			this.settings.ExportBatchSize = expectedValue;
			Assert.That(this.settings.ExportBatchSize, Is.EqualTo(expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheExportErrorNumberOfRetriesSetting()
		{
			Assert.That(
				this.settings.ExportErrorNumberOfRetries,
				Is.EqualTo(AppSettingsConstants.ExportErrorNumberOfRetriesDefaultValue));
			int expectedValue = RandomHelper.NextInt32(1, 1000);
			this.settings.ExportErrorNumberOfRetries = expectedValue;
			Assert.That(this.settings.ExportErrorNumberOfRetries, Is.EqualTo(expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheExportErrorWaitTimeInSecondsSetting()
		{
			Assert.That(
				this.settings.ExportErrorWaitTimeInSeconds,
				Is.EqualTo(AppSettingsConstants.ExportErrorWaitTimeInSecondsDefaultValue));
			int expectedValue = RandomHelper.NextInt32(1, 1000);
			this.settings.ExportErrorWaitTimeInSeconds = expectedValue;
			Assert.That(this.settings.ExportErrorWaitTimeInSeconds, Is.EqualTo(expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheExportThreadCountSetting()
		{
			Assert.That(
				this.settings.ExportThreadCount,
				Is.EqualTo(AppSettingsConstants.ExportThreadCountDefaultValue));
			int expectedValue = RandomHelper.NextInt32(1, 1000);
			this.settings.ExportThreadCount = expectedValue;
			Assert.That(this.settings.ExportThreadCount, Is.EqualTo(expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheForceFolderPreviewSetting()
		{
			DeleteTestSubKey();
			Assert.That(
				this.settings.ForceFolderPreview,
				Is.EqualTo(AppSettingsConstants.ForceFolderPreviewDefaultValue));
			bool expectedValue = RandomHelper.NextBoolean();
			this.settings.ForceFolderPreview = expectedValue;
			Assert.That(this.settings.ForceFolderPreview, Is.EqualTo(expectedValue));
			this.settings.ForceFolderPreview = !expectedValue;
			Assert.That(this.settings.ForceFolderPreview, Is.EqualTo(!expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheForceParallelismInNewExportSetting()
		{
			Assert.That(
				this.settings.ForceParallelismInNewExport,
				Is.EqualTo(AppSettingsConstants.ForceParallelismInNewExportDefaultValue));
			bool expectedValue = RandomHelper.NextBoolean();
			this.settings.ForceParallelismInNewExport = expectedValue;
			Assert.That(this.settings.ForceParallelismInNewExport, Is.EqualTo(expectedValue));
			this.settings.ForceParallelismInNewExport = !expectedValue;
			Assert.That(this.settings.ForceParallelismInNewExport, Is.EqualTo(!expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheForceWebUploadSetting()
		{
			Assert.That(
				this.settings.ForceWebUpload,
				Is.EqualTo(AppSettingsConstants.ForceWebUploadDefaultValue));
			bool expectedValue = RandomHelper.NextBoolean();
			this.settings.ForceWebUpload = expectedValue;
			Assert.That(this.settings.ForceWebUpload, Is.EqualTo(expectedValue));
			this.settings.ForceWebUpload = !expectedValue;
			Assert.That(this.settings.ForceWebUpload, Is.EqualTo(!expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheHttpTimeoutSecondsSetting()
		{
			Assert.That(
				this.settings.HttpTimeoutSeconds,
				Is.EqualTo(AppSettingsConstants.HttpTimeoutSecondsDefaultValue));
			int expectedValue = RandomHelper.NextInt32(1, 1000);
			this.settings.HttpTimeoutSeconds = expectedValue;
			Assert.That(this.settings.HttpTimeoutSeconds, Is.EqualTo(expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheImportBatchMaxVolumeSetting()
		{
			Assert.That(
				this.settings.ImportBatchMaxVolume,
				Is.EqualTo(AppSettingsConstants.ImportBatchMaxVolumeDefaultValue));
			int expectedValue = RandomHelper.NextInt32(1, 1000);
			this.settings.ImportBatchMaxVolume = expectedValue;
			Assert.That(this.settings.ImportBatchMaxVolume, Is.EqualTo(expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheImportBatchSizeSetting()
		{
			Assert.That(
				this.settings.ImportBatchSize,
				Is.EqualTo(AppSettingsConstants.ImportBatchSizeDefaultValue));
			int expectedValue = RandomHelper.NextInt32(1, 1000);
			this.settings.ImportBatchSize = expectedValue;
			Assert.That(this.settings.ImportBatchSize, Is.EqualTo(expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheIoErrorNumberOfRetriesSetting()
		{
			Assert.That(
				this.settings.IoErrorNumberOfRetries,
				Is.EqualTo(AppSettingsConstants.IoErrorNumberOfRetriesDefaultValue));
			int expectedValue = RandomHelper.NextInt32(1, 1000);
			this.settings.IoErrorNumberOfRetries = expectedValue;
			Assert.That(this.settings.IoErrorNumberOfRetries, Is.EqualTo(expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheIoErrorWaitTimeInSecondsSetting()
		{
			Assert.That(
				this.settings.IoErrorWaitTimeInSeconds,
				Is.EqualTo(AppSettingsConstants.IoErrorWaitTimeInSecondsDefaultValue));
			int expectedValue = RandomHelper.NextInt32(1, 1000);
			this.settings.IoErrorWaitTimeInSeconds = expectedValue;
			Assert.That(this.settings.IoErrorWaitTimeInSeconds, Is.EqualTo(expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheJobCompleteBatchSizeSetting()
		{
			Assert.That(
				this.settings.JobCompleteBatchSize,
				Is.EqualTo(AppSettingsConstants.JobCompleteBatchSizeDefaultValue));
			int expectedValue = RandomHelper.NextInt32(1, 1000);
			this.settings.JobCompleteBatchSize = expectedValue;
			Assert.That(this.settings.JobCompleteBatchSize, Is.EqualTo(expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheLoadImportedFullTextFromServeretting()
		{
			Assert.That(
				this.settings.LoadImportedFullTextFromServer,
				Is.EqualTo(AppSettingsConstants.LoadImportedFullTextFromServerDefaultValue));
			bool expectedValue = RandomHelper.NextBoolean();
			this.settings.LoadImportedFullTextFromServer = expectedValue;
			Assert.That(this.settings.LoadImportedFullTextFromServer, Is.EqualTo(expectedValue));
			this.settings.LoadImportedFullTextFromServer = !expectedValue;
			Assert.That(this.settings.LoadImportedFullTextFromServer, Is.EqualTo(!expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheLogAllEventsSetting()
		{
			Assert.That(
				this.settings.LogAllEvents,
				Is.EqualTo(AppSettingsConstants.LogAllEventsDefaultValue));
			bool expectedValue = RandomHelper.NextBoolean();
			this.settings.LogAllEvents = expectedValue;
			Assert.That(this.settings.LogAllEvents, Is.EqualTo(expectedValue));
			this.settings.LogAllEvents = !expectedValue;
			Assert.That(this.settings.LogAllEvents, Is.EqualTo(!expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheLogConfigXmlFileNameSetting()
		{
			Assert.That(
				this.settings.LogConfigXmlFileName,
				Is.EqualTo(AppSettingsConstants.LogConfigXmlFileNameDefaultValue));
			string expectedValue = RandomHelper.NextString(10, 100);
			this.settings.LogConfigXmlFileName = expectedValue;
			Assert.That(this.settings.LogConfigXmlFileName, Is.EqualTo(expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheMaxFilesForTapiBridgeSetting()
		{
			Assert.That(
				this.settings.MaxFilesForTapiBridge,
				Is.EqualTo(AppSettingsConstants.MaxFilesForTapiBridgeDefaultValue));
			int expectedValue = RandomHelper.NextInt32(1, 1000);
			this.settings.MaxFilesForTapiBridge = expectedValue;
			Assert.That(this.settings.MaxFilesForTapiBridge, Is.EqualTo(expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheMaxNumberOfFileExportTasksSetting()
		{
			Assert.That(
				this.settings.MaxNumberOfFileExportTasks,
				Is.EqualTo(AppSettingsConstants.MaxNumberOfFileExportTasksDefaultValue));
			int expectedValue = RandomHelper.NextInt32(1, 1000);
			this.settings.MaxNumberOfFileExportTasks = expectedValue;
			Assert.That(this.settings.MaxNumberOfFileExportTasks, Is.EqualTo(expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheMaxReloginTriesSetting()
		{
			Assert.That(
				this.settings.MaxReloginTries,
				Is.EqualTo(AppSettingsConstants.MaximumReloginTriesDefaultValue));
			int expectedValue = RandomHelper.NextInt32(1, 1000);
			this.settings.MaxReloginTries = expectedValue;
			Assert.That(this.settings.MaxReloginTries, Is.EqualTo(expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheMinBatchSizeSetting()
		{
			Assert.That(
				this.settings.MinBatchSize,
				Is.EqualTo(AppSettingsConstants.MinBatchSizeDefaultValue));
			int expectedValue = RandomHelper.NextInt32(1, 1000);
			this.settings.MinBatchSize = expectedValue;
			Assert.That(this.settings.MinBatchSize, Is.EqualTo(expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheObjectFieldIdListContainsArtifactIdSetting()
		{
			List<int> testList = new List<int> { 1, 2, 3, 4, 5 };
			Assert.That(this.settings.ObjectFieldIdListContainsArtifactId, Is.Empty);
			this.settings.ObjectFieldIdListContainsArtifactId = testList;
			Assert.That(this.settings.ObjectFieldIdListContainsArtifactId, Is.EquivalentTo(testList));
			this.settings.ObjectFieldIdListContainsArtifactId = new List<int>();
			Assert.That(this.settings.ObjectFieldIdListContainsArtifactId, Is.Empty);
		}

		[Test]
		public void ShouldGetTheOpenIdConnectHomeRealmDiscoveryHintSetting()
		{
			Assert.That(this.settings.OpenIdConnectHomeRealmDiscoveryHint, Is.Empty);
			AppSettingsManager.SetRegistryKeyValue(AppSettingsConstants.OpenIdConnectHomeRealmDiscoveryHintKey, string.Empty);
			Assert.That(this.settings.OpenIdConnectHomeRealmDiscoveryHint, Is.Empty);
			AppSettingsManager.SetRegistryKeyValue(AppSettingsConstants.OpenIdConnectHomeRealmDiscoveryHintKey, "HRD-HINT-VALUE");
			Assert.That(this.settings.OpenIdConnectHomeRealmDiscoveryHint, Is.EqualTo("HRD-HINT-VALUE"));

			// Ensure that we don't fail when the key/value doesn't exist.
			DeleteTestSubKey();
			Assert.That(this.settings.OpenIdConnectHomeRealmDiscoveryHint, Is.Empty);
		}

		[Test]
		public void ShouldGetAndSetThePermissionErrorsRetrySetting()
		{
			// Note: This value also enables and disables the RetryOptions.Permissions value.
			Assert.That(this.settings.PermissionErrorsRetry, Is.EqualTo(AppSettingsConstants.PermissionErrorsRetryDefaultValue));
			bool expectedValue = RandomHelper.NextBoolean();
			this.settings.PermissionErrorsRetry = expectedValue;
			Assert.That(this.settings.PermissionErrorsRetry, Is.EqualTo(expectedValue));
			Assert.That(this.settings.RetryOptions.HasFlag(RetryOptions.Permissions), Is.EqualTo(expectedValue));
			this.settings.PermissionErrorsRetry = !expectedValue;
			Assert.That(this.settings.PermissionErrorsRetry, Is.EqualTo(!expectedValue));
			Assert.That(this.settings.RetryOptions.HasFlag(RetryOptions.Permissions), Is.EqualTo(!expectedValue));
		}

		[Test]
		public void ShouldGetAndSetThePreviewThresholdSetting()
		{
			Assert.That(
				this.settings.PreviewThreshold,
				Is.EqualTo(AppSettingsConstants.PreviewThresholdDefaultValue));
			int expectedValue = RandomHelper.NextInt32(1, 1000);
			this.settings.PreviewThreshold = expectedValue;
			Assert.That(this.settings.PreviewThreshold, Is.EqualTo(expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheProcessFormRefreshRateSetting()
		{
			Assert.That(
				this.settings.ProcessFormRefreshRate,
				Is.EqualTo(AppSettingsConstants.ProcessFormRefreshRateDefaultValue));
			int expectedValue = RandomHelper.NextInt32(1, 1000);
			this.settings.ProcessFormRefreshRate = expectedValue;
			Assert.That(this.settings.ProcessFormRefreshRate, Is.EqualTo(expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheProgrammaticWebApiServiceUrlSetting()
		{
			Assert.That(this.settings.ProgrammaticWebApiServiceUrl, Is.Null);
			this.settings.ProgrammaticWebApiServiceUrl = "https://relativity.one.com";
			Assert.That(this.settings.ProgrammaticWebApiServiceUrl, Is.EqualTo("https://relativity.one.com/"));
			this.settings.ProgrammaticWebApiServiceUrl = null;
			Assert.That(this.settings.ProgrammaticWebApiServiceUrl, Is.Empty);
		}

		[Test]
		public void ShouldGetAndSetTheRestUrlSetting()
		{
			Assert.That(this.settings.RestUrl, Is.EqualTo(AppSettingsConstants.RestUrlDefaultValue));
			Uri expectedValue = RandomHelper.NextUri();
			this.settings.RestUrl = expectedValue.ToString();
			Assert.That(this.settings.RestUrl, Is.EqualTo(expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheServicesUrlSetting()
		{
			Assert.That(this.settings.ServicesUrl, Is.EqualTo(AppSettingsConstants.ServicesUrlDefaultValue));
			Uri expectedValue = RandomHelper.NextUri();
			this.settings.ServicesUrl = expectedValue.ToString();
			Assert.That(this.settings.ServicesUrl, Is.EqualTo(expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheSuppressServerCertificateValidationSetting()
		{
			Assert.That(
				this.settings.SuppressServerCertificateValidation,
				Is.EqualTo(AppSettingsConstants.SuppressServerCertificateValidationDefaultValue));
			bool expectedValue = RandomHelper.NextBoolean();
			this.settings.SuppressServerCertificateValidation = expectedValue;
			Assert.That(this.settings.SuppressServerCertificateValidation, Is.EqualTo(expectedValue));
			this.settings.SuppressServerCertificateValidation = !expectedValue;
			Assert.That(this.settings.SuppressServerCertificateValidation, Is.EqualTo(!expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheTapiAsperaBcpRootFolderSetting()
		{
			Assert.That(this.settings.TapiAsperaBcpRootFolder, Is.Null.Or.Empty);
			int expectedValue = RandomHelper.NextInt32(1, 1000);
			this.settings.TapiBridgeExportTransferWaitingTimeInSeconds = expectedValue;
			Assert.That(this.settings.TapiBridgeExportTransferWaitingTimeInSeconds, Is.EqualTo(expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheTapiAsperaNativeDocRootLevelsSetting()
		{
			Assert.That(
				this.settings.TapiAsperaNativeDocRootLevels,
				Is.EqualTo(AppSettingsConstants.TapiAsperaNativeDocRootLevelsDefaultValue));
			int expectedValue = RandomHelper.NextInt32(1, 1000);
			this.settings.TapiAsperaNativeDocRootLevels = expectedValue;
			Assert.That(this.settings.TapiAsperaNativeDocRootLevels, Is.EqualTo(expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheTapiBadPathErrorsRetrySetting()
		{
			Assert.That(
				this.settings.TapiBadPathErrorsRetry,
				Is.EqualTo(AppSettingsConstants.TapiBadPathErrorsRetryDefaultValue));
			bool expectedValue = RandomHelper.NextBoolean();
			this.settings.TapiBadPathErrorsRetry = expectedValue;
			Assert.That(this.settings.TapiBadPathErrorsRetry, Is.EqualTo(expectedValue));
			this.settings.TapiBadPathErrorsRetry = !expectedValue;
			Assert.That(this.settings.TapiBadPathErrorsRetry, Is.EqualTo(!expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheTapiBridgeExportTransferWaitingTimeInSecondsSetting()
		{
			Assert.That(
				this.settings.TapiBridgeExportTransferWaitingTimeInSeconds,
				Is.EqualTo(AppSettingsConstants.TapiBridgeExportTransferWaitingTimeInSecondsDefaultValue));
			int expectedValue = RandomHelper.NextInt32(1, 1000);
			this.settings.TapiBridgeExportTransferWaitingTimeInSeconds = expectedValue;
			Assert.That(this.settings.TapiBridgeExportTransferWaitingTimeInSeconds, Is.EqualTo(expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheTapiForceAsperaClientSetting()
		{
			Assert.That(
				this.settings.TapiForceAsperaClient,
				Is.EqualTo(AppSettingsConstants.TapiForceAsperaClientDefaultValue));
			bool expectedValue = RandomHelper.NextBoolean();
			this.settings.TapiForceAsperaClient = expectedValue;
			Assert.That(this.settings.TapiForceAsperaClient, Is.EqualTo(expectedValue));
			this.settings.TapiForceAsperaClient = !expectedValue;
			Assert.That(this.settings.TapiForceAsperaClient, Is.EqualTo(!expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheTapiForceBcpHttpClientSetting()
		{
			Assert.That(
				this.settings.TapiForceBcpHttpClient,
				Is.EqualTo(AppSettingsConstants.TapiForceBcpHttpClientDefaultValue));
			bool expectedValue = RandomHelper.NextBoolean();
			this.settings.TapiForceBcpHttpClient = expectedValue;
			Assert.That(this.settings.TapiForceBcpHttpClient, Is.EqualTo(expectedValue));
			this.settings.TapiForceBcpHttpClient = !expectedValue;
			Assert.That(this.settings.TapiForceBcpHttpClient, Is.EqualTo(!expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheTapiForceClientCandidatesSetting()
		{
			Assert.That(this.settings.TapiForceClientCandidates, Is.Null.Or.Empty);
			string expectedValue = RandomHelper.NextString(1, 100);
			this.settings.TapiForceClientCandidates = expectedValue;
			Assert.That(this.settings.TapiForceClientCandidates, Is.EqualTo(expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheTapiForceFileShareClientSetting()
		{
			Assert.That(
				this.settings.TapiForceFileShareClient,
				Is.EqualTo(AppSettingsConstants.TapiForceFileShareClientDefaultValue));
			bool expectedValue = RandomHelper.NextBoolean();
			this.settings.TapiForceFileShareClient = expectedValue;
			Assert.That(this.settings.TapiForceFileShareClient, Is.EqualTo(expectedValue));
			this.settings.TapiForceFileShareClient = !expectedValue;
			Assert.That(this.settings.TapiForceFileShareClient, Is.EqualTo(!expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheTapiForceHttpClientSetting()
		{
			Assert.That(
				this.settings.TapiForceHttpClient,
				Is.EqualTo(AppSettingsConstants.TapiForceHttpClientDefaultValue));
			bool expectedValue = RandomHelper.NextBoolean();
			this.settings.TapiForceHttpClient = expectedValue;
			Assert.That(this.settings.TapiForceHttpClient, Is.EqualTo(expectedValue));
			this.settings.TapiForceHttpClient = !expectedValue;
			Assert.That(this.settings.TapiForceHttpClient, Is.EqualTo(!expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheTapiLargeFileProgressEnabledSetting()
		{
			Assert.That(
				this.settings.TapiLargeFileProgressEnabled,
				Is.EqualTo(AppSettingsConstants.TapiLargeFileProgressEnabledDefaultValue));
			bool expectedValue = RandomHelper.NextBoolean();
			this.settings.TapiLargeFileProgressEnabled = expectedValue;
			Assert.That(this.settings.TapiLargeFileProgressEnabled, Is.EqualTo(expectedValue));
			this.settings.TapiLargeFileProgressEnabled = !expectedValue;
			Assert.That(this.settings.TapiLargeFileProgressEnabled, Is.EqualTo(!expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheTapiMaxJobParallelismSetting()
		{
			Assert.That(
				this.settings.TapiMaxJobParallelism,
				Is.EqualTo(AppSettingsConstants.TapiMaxJobParallelismDefaultValue));
			int expectedValue = RandomHelper.NextInt32(1, 16);
			this.settings.TapiMaxJobParallelism = expectedValue;
			Assert.That(this.settings.TapiMaxJobParallelism, Is.EqualTo(expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheTapiMinDataRateMbpsSetting()
		{
			Assert.That(
				this.settings.TapiMinDataRateMbps,
				Is.EqualTo(AppSettingsConstants.TapiMinDataRateMbpsDefaultValue));
			int expectedValue = RandomHelper.NextInt32(1, 16);
			this.settings.TapiMinDataRateMbps = expectedValue;
			Assert.That(this.settings.TapiMinDataRateMbps, Is.EqualTo(expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheTapiPreserveFileTimestampsSetting()
		{
			Assert.That(
				this.settings.TapiPreserveFileTimestamps,
				Is.EqualTo(AppSettingsConstants.TapiPreserveFileTimestampsDefaultValue));
			bool expectedValue = RandomHelper.NextBoolean();
			this.settings.TapiPreserveFileTimestamps = expectedValue;
			Assert.That(this.settings.TapiPreserveFileTimestamps, Is.EqualTo(expectedValue));
			this.settings.TapiPreserveFileTimestamps = !expectedValue;
			Assert.That(this.settings.TapiPreserveFileTimestamps, Is.EqualTo(!expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheTapiSubmitApmMetricsSetting()
		{
			Assert.That(
				this.settings.TapiSubmitApmMetrics,
				Is.EqualTo(AppSettingsConstants.TapiSubmitApmMetricsDefaultValue));
			bool expectedValue = RandomHelper.NextBoolean();
			this.settings.TapiSubmitApmMetrics = expectedValue;
			Assert.That(this.settings.TapiSubmitApmMetrics, Is.EqualTo(expectedValue));
			this.settings.TapiSubmitApmMetrics = !expectedValue;
			Assert.That(this.settings.TapiSubmitApmMetrics, Is.EqualTo(!expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheTapiTargetDataRateMbpsSetting()
		{
			Assert.That(
				this.settings.TapiTargetDataRateMbps,
				Is.EqualTo(AppSettingsConstants.TapiTargetDataRateMbpsDefaultValue));
			int expectedValue = RandomHelper.NextInt32(10, 600);
			this.settings.TapiTargetDataRateMbps = expectedValue;
			Assert.That(this.settings.TapiTargetDataRateMbps, Is.EqualTo(expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheTapiTransferLogDirectorySetting()
		{
			Assert.That(this.settings.TapiTransferLogDirectory, Is.Null.Or.Empty);
			string expectedValue = RandomHelper.NextString(1, 255);
			this.settings.TapiTransferLogDirectory = expectedValue;
			Assert.That(this.settings.TapiTransferLogDirectory, Is.EqualTo(expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheTempDirectorySetting()
		{
			Assert.That(this.settings.TempDirectory, Is.Null.Or.Empty);
			string expectedValue = RandomHelper.NextString(1, 255);
			this.settings.TempDirectory = expectedValue;
			Assert.That(this.settings.TempDirectory, Is.EqualTo(expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheUseOldExportSetting()
		{
			Assert.That(
				this.settings.UseOldExport,
				Is.EqualTo(AppSettingsConstants.UseOldExportDefaultValue));
			bool expectedValue = RandomHelper.NextBoolean();
			this.settings.UseOldExport = expectedValue;
			Assert.That(this.settings.UseOldExport, Is.EqualTo(expectedValue));
			this.settings.UseOldExport = !expectedValue;
			Assert.That(this.settings.UseOldExport, Is.EqualTo(!expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheUsePipeliningForNativeAndObjectImportsSetting()
		{
			Assert.That(
				this.settings.UsePipeliningForNativeAndObjectImports,
				Is.EqualTo(AppSettingsConstants.UsePipeliningForNativeAndObjectImportsDefaultValue));
			bool expectedValue = RandomHelper.NextBoolean();
			this.settings.UsePipeliningForNativeAndObjectImports = expectedValue;
			Assert.That(this.settings.UsePipeliningForNativeAndObjectImports, Is.EqualTo(expectedValue));
			this.settings.UsePipeliningForNativeAndObjectImports = !expectedValue;
			Assert.That(this.settings.UsePipeliningForNativeAndObjectImports, Is.EqualTo(!expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheWaitBeforeReconnectSetting()
		{
			Assert.That(
				this.settings.WaitBeforeReconnect,
				Is.EqualTo(AppSettingsConstants.WaitBeforeReconnectDefaultValue));
			int expectedValue = RandomHelper.NextInt32(1, 1000);
			this.settings.WaitBeforeReconnect = expectedValue;
			Assert.That(this.settings.WaitBeforeReconnect, Is.EqualTo(expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheWebApiOperationTimeoutSetting()
		{
			Assert.That(
				this.settings.WebApiOperationTimeout,
				Is.EqualTo(AppSettingsConstants.WebApiOperationTimeoutDefaultValue));
			int expectedValue = RandomHelper.NextInt32(1, 1000);
			this.settings.WebApiOperationTimeout = expectedValue;
			Assert.That(this.settings.WebApiOperationTimeout, Is.EqualTo(expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheWebApiServiceUrlSetting()
		{
			DeleteTestSubKey();
			this.settings.ProgrammaticWebApiServiceUrl = null;
			this.settings.WebApiServiceUrl = null;
			Assert.That(this.settings.ProgrammaticWebApiServiceUrl, Is.Empty);
			Assert.That(this.settings.WebApiServiceUrl, Is.Empty);
			this.settings.WebApiServiceUrl = "https://relativity.one.com";

			// The trailing slash is automatically added.
			Assert.That(this.settings.WebApiServiceUrl, Is.EqualTo("https://relativity.one.com/"));
			this.settings.WebApiServiceUrl = null;
			Assert.That(this.settings.ProgrammaticWebApiServiceUrl, Is.Empty);
			Assert.That(this.settings.WebApiServiceUrl, Is.Empty);

			// This simulates the scenario where the URL comes from the RDC/Registry.
			AppSettingsManager.SetRegistryKeyValue(AppSettingsConstants.WebApiServiceUrlRegistryKey, "https://relativity-2.one.com");
			Assert.That(this.settings.ProgrammaticWebApiServiceUrl, Is.Empty);
			Assert.That(this.settings.WebApiServiceUrl, Is.EqualTo("https://relativity-2.one.com/"));
			DeleteTestSubKey();
			Assert.That(this.settings.ProgrammaticWebApiServiceUrl, Is.Empty);
			Assert.That(this.settings.WebApiServiceUrl, Is.Empty);

			// This simulates the scenario where the URL comes from programmatic means.
			AppSettingsManager.SetRegistryKeyValue(AppSettingsConstants.WebApiServiceUrlRegistryKey, "https://relativity-3.one.com");
			this.settings.ProgrammaticWebApiServiceUrl = "https://relativity-3.one.com";
			Assert.That(this.settings.ProgrammaticWebApiServiceUrl, Is.EqualTo("https://relativity-3.one.com/"));
			Assert.That(this.settings.WebApiServiceUrl, Is.EqualTo("https://relativity-3.one.com/"));
		}

		[Test]
		public void ShouldGetAndSetTheWebBasedFileDownloadChunkSizeSetting()
		{
			Assert.That(
				this.settings.WebBasedFileDownloadChunkSize,
				Is.EqualTo(AppSettingsConstants.WebBasedFileDownloadChunkSizeDefaultValue));
			int expectedValue = RandomHelper.NextInt32(
				AppSettingsConstants.WebBasedFileDownloadChunkSizeMinValue,
				AppSettingsConstants.WebBasedFileDownloadChunkSizeMinValue + 100000);
			this.settings.WebBasedFileDownloadChunkSize = expectedValue;
			Assert.That(this.settings.WebBasedFileDownloadChunkSize, Is.EqualTo(expectedValue));
		}

		[Test]
		public void ShouldMakeSettingsDeepCopy()
		{
			AssignRandomValues(this.settings);
			IAppSettings copy = this.settings.DeepCopy();
			CompareAllSettingValues(this.settings, copy);
		}

		[Test]
		public void ShouldSerializeAndDeserializeTheSettings()
		{
			AssignRandomValues(this.settings);
			IFormatter formatter = new BinaryFormatter();
			using (MemoryStream stream = new MemoryStream())
			{
				formatter.Serialize(stream, this.settings);
				stream.Seek(0, SeekOrigin.Begin);
				IAppSettings deserializedSettings = (IAppSettings)formatter.Deserialize(stream);
				Assert.IsNotNull(deserializedSettings);
				CompareAllSettingValues(this.settings, deserializedSettings);
			}
		}

		[Test]
		public void ShouldGetTheActualAppConfigSettings()
		{
			// Make sure the entire design works against a real App.config file that was ripped from the RDC but with different values.
			const bool Refresh = false;
			this.settings = AppSettingsManager.Create(Refresh);
			for (int i = 0; i < 3; i++)
			{
				AppSettingsManager.Refresh(this.settings);

				// The kCura.Utility section asserts go here.
				Assert.That(this.settings.CreateErrorForInvalidDate, Is.True);
				Assert.That(this.settings.ExportErrorNumberOfRetries, Is.EqualTo(5));
				Assert.That(this.settings.ExportErrorWaitTimeInSeconds, Is.EqualTo(15));
				Assert.That(this.settings.IoErrorNumberOfRetries, Is.EqualTo(8));
				Assert.That(this.settings.IoErrorWaitTimeInSeconds, Is.EqualTo(16));
				Assert.That(this.settings.MaxNumberOfFileExportTasks, Is.EqualTo(4));

				// The kCura.WinEDDS section asserts go here.
				Assert.That(this.settings.ApplicationName, Is.EqualTo("Custom App"));
				Assert.That(this.settings.TapiBadPathErrorsRetry, Is.False);
				Assert.That(this.settings.CreateErrorForEmptyNativeFile, Is.True);
				Assert.That(this.settings.DefaultMaxErrorCount, Is.EqualTo(555));
				Assert.That(this.settings.DisableImageLocationValidation, Is.True);
				Assert.That(this.settings.DisableImageTypeValidation, Is.True);
				Assert.That(this.settings.DisableTextFileEncodingCheck, Is.True);
				Assert.That(this.settings.DisableThrowOnIllegalCharacters, Is.True);
				Assert.That(this.settings.DynamicBatchResizingOn, Is.False);
				Assert.That(this.settings.ExportBatchSize, Is.EqualTo(255));
				Assert.That(this.settings.ExportThreadCount, Is.EqualTo(3));
				Assert.That(this.settings.ForceParallelismInNewExport, Is.False);
				Assert.That(this.settings.ForceWebUpload, Is.True);
				Assert.That(this.settings.ImportBatchMaxVolume, Is.EqualTo(12345));
				Assert.That(this.settings.ImportBatchSize, Is.EqualTo(102));
				Assert.That(this.settings.JobCompleteBatchSize, Is.EqualTo(999));
				Assert.That(this.settings.LogConfigXmlFileName, Is.EqualTo("CustomLog.xml"));
				Assert.That(this.settings.MaxFilesForTapiBridge, Is.EqualTo(333));
				Assert.That(this.settings.MaxReloginTries, Is.EqualTo(42));
				Assert.That(this.settings.MinBatchSize, Is.EqualTo(29));
				Assert.That(this.settings.PermissionErrorsRetry, Is.False);
				Assert.That(this.settings.PreviewThreshold, Is.EqualTo(49));
				Assert.That(this.settings.SuppressServerCertificateValidation, Is.True);
				Assert.That(this.settings.TapiAsperaBcpRootFolder, Is.EqualTo("Root"));
				Assert.That(this.settings.TapiForceAsperaClient, Is.True);
				Assert.That(this.settings.TapiForceBcpHttpClient, Is.True);
				Assert.That(this.settings.TapiForceFileShareClient, Is.True);
				Assert.That(this.settings.TapiForceHttpClient, Is.True);
				Assert.That(this.settings.TapiForceClientCandidates, Is.EqualTo("FileShare;Aspera;Http"));
				Assert.That(this.settings.TapiLargeFileProgressEnabled, Is.True);
				Assert.That(this.settings.TapiMaxJobParallelism, Is.EqualTo(5));
				Assert.That(this.settings.TapiMinDataRateMbps, Is.EqualTo(30));
				Assert.That(this.settings.TapiSubmitApmMetrics, Is.False);
				Assert.That(this.settings.TapiTargetDataRateMbps, Is.EqualTo(50));
				Assert.That(this.settings.TapiTransferLogDirectory, Is.EqualTo(@"%temp%\IAPI_log\"));
				Assert.That(this.settings.TempDirectory, Is.EqualTo(@"C:\"));
				Assert.That(this.settings.UseOldExport, Is.True);
				Assert.That(this.settings.UsePipeliningForNativeAndObjectImports, Is.True);
				Assert.That(this.settings.WebApiServiceUrl, Is.EqualTo("https://relativity.one.com/"));
				Assert.That(this.settings.WebApiOperationTimeout, Is.EqualTo(333));
				Assert.That(this.settings.WebBasedFileDownloadChunkSize, Is.EqualTo(11111));

				// The kCura.Windows.Process section asserts go here.
				Assert.That(this.settings.LogAllEvents, Is.True);
			}
		}

		[Test]
		public void ShouldGetTheAppDictionaryDirectly()
		{
			const bool Refresh = true;
			this.settings = AppSettingsManager.Create(Refresh);
			AppSettingsDictionary dictionary = new AppSettingsDictionary(this.settings);
			for (int i = 0; i < 3; i++)
			{
				// The kCura.Utility section asserts go here.
				Assert.That(dictionary[AppSettingsConstants.CreateErrorForInvalidDateKey], Is.True);
				Assert.That(dictionary[AppSettingsConstants.ExportErrorNumberOfRetriesKey], Is.EqualTo(5));
				Assert.That(dictionary[AppSettingsConstants.ExportErrorWaitTimeInSecondsKey], Is.EqualTo(15));
				Assert.That(dictionary[AppSettingsConstants.IoErrorNumberOfRetriesKey], Is.EqualTo(8));
				Assert.That(dictionary[AppSettingsConstants.IoErrorWaitTimeInSecondsKey], Is.EqualTo(16));
				Assert.That(dictionary[AppSettingsConstants.MaxNumberOfFileExportTasksKey], Is.EqualTo(4));

				// The kCura.WinEDDS section asserts go here.
				Assert.That(dictionary[AppSettingsConstants.ApplicationNameKey], Is.EqualTo("Custom App"));
				Assert.That(dictionary[AppSettingsConstants.CreateErrorForEmptyNativeFileKey], Is.True);
				Assert.That(dictionary[AppSettingsConstants.DefaultMaxErrorCountKey], Is.EqualTo(555));
				Assert.That(dictionary[AppSettingsConstants.DisableImageLocationValidationKey], Is.True);
				Assert.That(dictionary[AppSettingsConstants.DisableImageTypeValidationKey], Is.True);
				Assert.That(dictionary[AppSettingsConstants.DisableTextFileEncodingCheckKey], Is.True);
				Assert.That(dictionary[AppSettingsConstants.DisableThrowOnIllegalCharactersKey], Is.True);
				Assert.That(dictionary[AppSettingsConstants.DynamicBatchResizingOnKey], Is.False);
				Assert.That(dictionary[AppSettingsConstants.ExportBatchSizeKey], Is.EqualTo(255));
				Assert.That(dictionary[AppSettingsConstants.ExportThreadCountKey], Is.EqualTo(3));
				Assert.That(dictionary[AppSettingsConstants.ForceParallelismInNewExportKey], Is.False);
				Assert.That(dictionary[AppSettingsConstants.ForceWebUploadKey], Is.True);
				Assert.That(dictionary[AppSettingsConstants.ImportBatchMaxVolumeKey], Is.EqualTo(12345));
				Assert.That(dictionary[AppSettingsConstants.ImportBatchSizeKey], Is.EqualTo(102));
				Assert.That(dictionary[AppSettingsConstants.LogConfigXmlFileNameKey], Is.EqualTo("CustomLog.xml"));
				Assert.That(dictionary[AppSettingsConstants.MaxFilesForTapiBridgeKey], Is.EqualTo(333));
				Assert.That(dictionary[AppSettingsConstants.MaximumReloginTriesKey], Is.EqualTo(42));
				Assert.That(dictionary[AppSettingsConstants.MinBatchSizeKey], Is.EqualTo(29));
				Assert.That(dictionary[AppSettingsConstants.PermissionErrorsRetryKey], Is.False);
				Assert.That(dictionary[AppSettingsConstants.PreviewThresholdKey], Is.EqualTo(49));
				Assert.That(dictionary[AppSettingsConstants.RestUrlKey], Is.EqualTo("/Relativity.One.REST/api/"));
				Assert.That(dictionary[AppSettingsConstants.ServicesUrlKey], Is.EqualTo("/Relativity.One.Services/"));
				Assert.That(dictionary[AppSettingsConstants.SuppressServerCertificateValidationKey], Is.True);
				Assert.That(dictionary[AppSettingsConstants.TapiAsperaBcpRootFolderKey], Is.EqualTo("Root"));
				Assert.That(dictionary[AppSettingsConstants.TapiBadPathErrorsRetryKey], Is.False);
				Assert.That(dictionary[AppSettingsConstants.TapiForceAsperaClientKey], Is.True);
				Assert.That(dictionary[AppSettingsConstants.TapiForceBcpHttpClientKey], Is.True);
				Assert.That(dictionary[AppSettingsConstants.TapiForceFileShareClientKey], Is.True);
				Assert.That(dictionary[AppSettingsConstants.TapiForceHttpClientKey], Is.True);
				Assert.That(dictionary[AppSettingsConstants.TapiForceClientCandidatesKey], Is.EqualTo("FileShare;Aspera;Http"));
				Assert.That(dictionary[AppSettingsConstants.TapiLargeFileProgressEnabledKey], Is.True);
				Assert.That(dictionary[AppSettingsConstants.TapiMaxJobParallelismKey], Is.EqualTo(5));
				Assert.That(dictionary[AppSettingsConstants.TapiMinDataRateMbpsKey], Is.EqualTo(30));
				Assert.That(dictionary[AppSettingsConstants.TapiSubmitApmMetricsKey], Is.False);
				Assert.That(dictionary[AppSettingsConstants.TapiTargetDataRateMbpsKey], Is.EqualTo(50));
				Assert.That(dictionary[AppSettingsConstants.TapiTransferLogDirectoryKey], Is.EqualTo(@"%temp%\IAPI_log\"));
				Assert.That(dictionary[AppSettingsConstants.TempDirectoryKey], Is.EqualTo(@"C:\"));
				Assert.That(dictionary[AppSettingsConstants.UseOldExportKey], Is.True);
				Assert.That(dictionary[AppSettingsConstants.UsePipeliningForNativeAndObjectImportsKey], Is.True);
				Assert.That(dictionary[AppSettingsConstants.WaitBeforeReconnectKey], Is.EqualTo(64));
				Assert.That(dictionary[AppSettingsConstants.WebApiOperationTimeoutKey], Is.EqualTo(333));
				Assert.That(dictionary[AppSettingsConstants.WebApiServiceUrlKey], Is.EqualTo("https://relativity.one.com/"));
				Assert.That(dictionary[AppSettingsConstants.WebBasedFileDownloadChunkSizeKey], Is.EqualTo(11111));

				// The kCura.Windows.Process section asserts go here.
				Assert.That(dictionary[AppSettingsConstants.LogAllEventsKey], Is.True);
			}
		}

		[Test]
		public void ShouldSetTheAppDictionaryDirectly()
		{
			const bool Refresh = false;
			this.settings = AppSettingsManager.Create(Refresh);
			AppSettingsDictionary dictionary = new AppSettingsDictionary(this.settings);
			for (int i = 0; i < 3; i++)
			{
				// The kCura.Utility section asserts go here.
				dictionary[AppSettingsConstants.CreateErrorForInvalidDateKey] = true;
				Assert.That(this.settings.CreateErrorForInvalidDate, Is.True);
				dictionary[AppSettingsConstants.ExportErrorNumberOfRetriesKey] = 3;
				Assert.That(this.settings.ExportErrorNumberOfRetries, Is.EqualTo(3));
				dictionary[AppSettingsConstants.ExportErrorWaitTimeInSecondsKey] = 4;
				Assert.That(this.settings.ExportErrorWaitTimeInSeconds, Is.EqualTo(4));
				dictionary[AppSettingsConstants.IoErrorNumberOfRetriesKey] = 5;
				Assert.That(this.settings.IoErrorNumberOfRetries, Is.EqualTo(5));
				dictionary[AppSettingsConstants.IoErrorWaitTimeInSecondsKey] = 6;
				Assert.That(this.settings.IoErrorWaitTimeInSeconds, Is.EqualTo(6));
				dictionary[AppSettingsConstants.MaxNumberOfFileExportTasksKey] = 7;
				Assert.That(this.settings.MaxNumberOfFileExportTasks, Is.EqualTo(7));

				// The kCura.WinEDDS section asserts go here.
				dictionary[AppSettingsConstants.ApplicationNameKey] = "The App";
				Assert.That(this.settings.ApplicationName, Is.EqualTo("The App"));
				dictionary[AppSettingsConstants.CreateErrorForEmptyNativeFileKey] = true;
				Assert.That(this.settings.CreateErrorForEmptyNativeFile, Is.True);
				dictionary[AppSettingsConstants.DefaultMaxErrorCountKey] = 8;
				Assert.That(this.settings.DefaultMaxErrorCount, Is.EqualTo(8));
				dictionary[AppSettingsConstants.DisableImageLocationValidationKey] = true;
				Assert.That(this.settings.DisableImageLocationValidation, Is.True);
				dictionary[AppSettingsConstants.DisableImageTypeValidationKey] = true;
				Assert.That(this.settings.DisableImageTypeValidation, Is.True);
				dictionary[AppSettingsConstants.DisableTextFileEncodingCheckKey] = true;
				Assert.That(this.settings.DisableTextFileEncodingCheck, Is.True);
				dictionary[AppSettingsConstants.DisableThrowOnIllegalCharactersKey] = true;
				Assert.That(this.settings.DisableThrowOnIllegalCharacters, Is.True);
				dictionary[AppSettingsConstants.DynamicBatchResizingOnKey] = true;
				Assert.That(this.settings.DynamicBatchResizingOn, Is.True);
				dictionary[AppSettingsConstants.ExportBatchSizeKey] = 9;
				Assert.That(this.settings.ExportBatchSize, Is.EqualTo(9));
				dictionary[AppSettingsConstants.ExportThreadCountKey] = 10;
				Assert.That(this.settings.ExportThreadCount, Is.EqualTo(10));
				dictionary[AppSettingsConstants.ForceParallelismInNewExportKey] = true;
				Assert.That(this.settings.ForceParallelismInNewExport, Is.True);
				dictionary[AppSettingsConstants.ForceWebUploadKey] = true;
				Assert.That(this.settings.ForceWebUpload, Is.True);
				dictionary[AppSettingsConstants.ImportBatchMaxVolumeKey] = 11;
				Assert.That(this.settings.ImportBatchMaxVolume, Is.EqualTo(11));
				dictionary[AppSettingsConstants.ImportBatchSizeKey] = 12;
				Assert.That(this.settings.ImportBatchSize, Is.EqualTo(12));
				dictionary[AppSettingsConstants.LogConfigXmlFileNameKey] = "abc";
				Assert.That(this.settings.LogConfigXmlFileName, Is.EqualTo("abc"));
				dictionary[AppSettingsConstants.MaxFilesForTapiBridgeKey] = 13;
				Assert.That(this.settings.MaxFilesForTapiBridge, Is.EqualTo(13));
				dictionary[AppSettingsConstants.MaximumReloginTriesKey] = 14;
				Assert.That(this.settings.MaxReloginTries, Is.EqualTo(14));
				dictionary[AppSettingsConstants.MinBatchSizeKey] = 15;
				Assert.That(this.settings.MinBatchSize, Is.EqualTo(15));
				dictionary[AppSettingsConstants.PermissionErrorsRetryKey] = true;
				Assert.That(this.settings.PermissionErrorsRetry, Is.True);
				dictionary[AppSettingsConstants.PreviewThresholdKey] = 16;
				Assert.That(this.settings.PreviewThreshold, Is.EqualTo(16));
				dictionary[AppSettingsConstants.RestUrlKey] = "/Relativity.One.Two.REST/api/";
				Assert.That(this.settings.RestUrl, Is.EqualTo("/Relativity.One.Two.REST/api/"));
				dictionary[AppSettingsConstants.ServicesUrlKey] = "/Relativity.One.Two.Services/";
				Assert.That(this.settings.ServicesUrl, Is.EqualTo("/Relativity.One.Two.Services/"));
				dictionary[AppSettingsConstants.SuppressServerCertificateValidationKey] = true;
				Assert.That(this.settings.SuppressServerCertificateValidation, Is.True);
				dictionary[AppSettingsConstants.TapiAsperaBcpRootFolderKey] = "def";
				Assert.That(this.settings.TapiAsperaBcpRootFolder, Is.EqualTo("def"));
				dictionary[AppSettingsConstants.TapiBadPathErrorsRetryKey] = true;
				Assert.That(this.settings.TapiBadPathErrorsRetry, Is.True);
				dictionary[AppSettingsConstants.TapiForceAsperaClientKey] = true;
				Assert.That(this.settings.TapiForceAsperaClient, Is.True);
				dictionary[AppSettingsConstants.TapiForceBcpHttpClientKey] = true;
				Assert.That(this.settings.TapiForceBcpHttpClient, Is.True);
				dictionary[AppSettingsConstants.TapiForceFileShareClientKey] = true;
				Assert.That(this.settings.TapiForceFileShareClient, Is.True);
				dictionary[AppSettingsConstants.TapiForceHttpClientKey] = true;
				Assert.That(this.settings.TapiForceHttpClient, Is.True);
				dictionary[AppSettingsConstants.TapiForceClientCandidatesKey] = "ghi";
				Assert.That(this.settings.TapiForceClientCandidates, Is.EqualTo("ghi"));
				dictionary[AppSettingsConstants.TapiLargeFileProgressEnabledKey] = true;
				Assert.That(this.settings.TapiLargeFileProgressEnabled, Is.True);
				dictionary[AppSettingsConstants.TapiMaxJobParallelismKey] = 17;
				Assert.That(this.settings.TapiMaxJobParallelism, Is.EqualTo(17));
				dictionary[AppSettingsConstants.TapiMinDataRateMbpsKey] = 18;
				Assert.That(this.settings.TapiMinDataRateMbps, Is.EqualTo(18));
				dictionary[AppSettingsConstants.TapiSubmitApmMetricsKey] = true;
				Assert.That(this.settings.TapiSubmitApmMetrics, Is.True);
				dictionary[AppSettingsConstants.TapiTargetDataRateMbpsKey] = 19;
				Assert.That(this.settings.TapiTargetDataRateMbps, Is.EqualTo(19));
				dictionary[AppSettingsConstants.TapiTransferLogDirectoryKey] = "jkl";
				Assert.That(this.settings.TapiTransferLogDirectory, Is.EqualTo("jkl"));
				dictionary[AppSettingsConstants.TempDirectoryKey] = "mno";
				Assert.That(this.settings.TempDirectory, Is.EqualTo("mno"));
				dictionary[AppSettingsConstants.UseOldExportKey] = true;
				Assert.That(this.settings.UseOldExport, Is.True);
				dictionary[AppSettingsConstants.UsePipeliningForNativeAndObjectImportsKey] = true;
				Assert.That(this.settings.UsePipeliningForNativeAndObjectImports, Is.True);
				dictionary[AppSettingsConstants.WaitBeforeReconnectKey] = 20;
				Assert.That(this.settings.WaitBeforeReconnect, Is.EqualTo(20));
				dictionary[AppSettingsConstants.WebApiOperationTimeoutKey] = 21;
				Assert.That(this.settings.WebApiOperationTimeout, Is.EqualTo(21));
				dictionary[AppSettingsConstants.WebApiServiceUrlKey] = "https://relativity.one.com";
				Assert.That(this.settings.WebApiServiceUrl, Is.EqualTo("https://relativity.one.com/"));
				dictionary[AppSettingsConstants.WebBasedFileDownloadChunkSizeKey] = 3000;
				Assert.That(this.settings.WebBasedFileDownloadChunkSize, Is.EqualTo(3000));

				// The kCura.Windows.Process section asserts go here.
				dictionary[AppSettingsConstants.LogAllEventsKey] = true;
				Assert.That(this.settings.LogAllEvents, Is.True);
			}
		}

		[Test]
		[TestCase(null, "")]
		[TestCase("", "")]
		[TestCase("/sample.txt", "")]
		[TestCase("/sample.txt/", "")]
		[TestCase("/relativity.one.com", "")]
		[TestCase("/relativity.one.com/", "")]
		[TestCase("//relativity.one.com", "//relativity.one.com/")]
		[TestCase("//relativity.one.com/", "//relativity.one.com/")]
		[TestCase("https://relativity.one.com", "https://relativity.one.com/")]
		[TestCase("https://relativity.one.com/", "https://relativity.one.com/")]
		public void ShouldValidateTheUriFormat(string value, string expected)
		{
			// Note: 2 leading forward slashes are considered file-based URI's.
			string actual = this.settings.ValidateUriFormat(value);
			Assert.That(actual, Is.EqualTo(expected));
		}

		private static void AssignRandomValues(IAppSettings settings)
		{
			AppSettingsManager.BuildAttributeDictionary();
			foreach (PropertyInfo prop in AppSettingsManager.GetProperties())
			{
				string key = AppSettingsManager.GetPropertyKey(prop);
				if (!AppSettingsManager.AppSettingAttributes.ContainsKey(key) || prop.SetMethod == null)
				{
					continue;
				}

				AppSettingAttribute attribute = AppSettingsManager.AppSettingAttributes[key];
				if (!attribute.IsMapped)
				{
					continue;
				}

				if (prop.PropertyType == typeof(string))
				{
					prop.SetValue(settings, RandomHelper.NextString(10, 100));
				}
				else if (prop.PropertyType == typeof(bool))
				{
					prop.SetValue(settings, RandomHelper.NextBoolean());
				}
				else if (prop.PropertyType == typeof(int))
				{
					prop.SetValue(settings, RandomHelper.NextInt32(1, 1000));
				}
				else if (prop.PropertyType == typeof(long))
				{
					prop.SetValue(settings, RandomHelper.NextInt64(1, 1000));
				}
				else if (prop.PropertyType == typeof(Uri))
				{
					Uri host = RandomHelper.NextUri();
					prop.SetValue(settings, host);
				}
				else if (prop.PropertyType.IsEnum)
				{
					int enumValue = RandomHelper.NextEnum(prop.PropertyType);
					prop.SetValue(settings, enumValue);
				}
				else
				{
					throw new InvalidOperationException($"The '{prop.Name}' property of type '{prop.PropertyType}' is not supported by the random generator.");
				}
			}

			settings.ProgrammaticWebApiServiceUrl = RandomHelper.NextUri().ToString();
			settings.WebApiServiceUrl = RandomHelper.NextUri().ToString();
		}

		private static void CompareAllSettingValues(IAppSettings settings1, IAppSettings settings2)
		{
			AppSettingsManager.BuildAttributeDictionary();
			foreach (PropertyInfo prop in AppSettingsManager.GetProperties())
			{
				if (prop.PropertyType == typeof(string))
				{
					string value1 = (string)prop.GetValue(settings1);
					string value2 = (string)prop.GetValue(settings2);
					Assert.That(value1, Is.EqualTo(value2), () => $"The property {prop.Name} is misconfigured.");
				}
				else if (prop.PropertyType == typeof(bool))
				{
					bool value1 = (bool)prop.GetValue(settings1);
					bool value2 = (bool)prop.GetValue(settings2);
					Assert.That(value1, Is.EqualTo(value2));
				}
				else if (prop.PropertyType == typeof(int))
				{
					int value1 = (int)prop.GetValue(settings1);
					int value2 = (int)prop.GetValue(settings2);
					Assert.That(value1, Is.EqualTo(value2));
				}
				else if (prop.PropertyType == typeof(long))
				{
					long value1 = (long)prop.GetValue(settings1);
					long value2 = (long)prop.GetValue(settings2);
					Assert.That(value1, Is.EqualTo(value2));
				}
				else if (prop.PropertyType == typeof(Uri))
				{
					Uri value1 = (Uri)prop.GetValue(settings1);
					Uri value2 = (Uri)prop.GetValue(settings2);
					Assert.That(value1, Is.EqualTo(value2));
				}
				else if (prop.PropertyType.IsEnum)
				{
					Enum value1 = (Enum)prop.GetValue(settings1);
					Enum value2 = (Enum)prop.GetValue(settings2);
					Assert.That(value1, Is.EqualTo(value2));
				}
				else if (prop.PropertyType == typeof(IList<int>))
				{
					IList<int> value1 = (IList<int>)prop.GetValue(settings1);
					IList<int> value2 = (IList<int>)prop.GetValue(settings2);
					Assert.That(value1, Is.EquivalentTo(value2));
				}
				else
				{
					throw new InvalidOperationException($"The '{prop.Name}' property of type '{prop.PropertyType}' is not supported by the test class.");
				}
			}
		}

		private static void DeleteTestSubKey()
		{
			const bool ThrowOnMissingSubKey = false;
			Microsoft.Win32.Registry.CurrentUser.DeleteSubKey(TestRegistrySubKey, ThrowOnMissingSubKey);
		}
	}
}