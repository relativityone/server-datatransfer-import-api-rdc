// -----------------------------------------------------------------------------------------------------
// <copyright file="AppSettingsTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents tests for all application settings.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.Runtime.Serialization;
	using System.Runtime.Serialization.Formatters.Binary;

	using global::NUnit.Framework;

	using Relativity.DataExchange;
	using Relativity.DataExchange.Io;
	using Relativity.DataExchange.TestFramework;

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
			DataExchange.AppSettingsManager.RegistrySubKeyName = TestRegistrySubKey;
			DeleteTestSubKey();
			this.settings = DataExchange.AppSettingsManager.Create(false);
		}

		[TearDown]
		public void Teardown()
		{
			DataExchange.AppSettingsManager.RegistrySubKeyName = TestRegistrySubKey;
			DeleteTestSubKey();
			this.settings = null;
		}

		[Test]
		public void ShouldDumpTheMappingInfo()
		{
			this.settings = null;
			AppSettingsManager.BuildAttributeDictionary();
			var groups = DataExchange.AppSettingsManager.AppSettingAttributes.GroupBy(x => x.Value.Section).ToList();
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
			Assert.That(this.settings.AuditLevel, Is.EqualTo(DataExchange.AppSettingsConstants.AuditLevelDefaultValue));
			string expectedValue = RandomHelper.NextString(10, 20);
			this.settings.AuditLevel = expectedValue;
			Assert.That(this.settings.AuditLevel, Is.EqualTo(expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheCreateErrorForInvalidDateSetting()
		{
			Assert.That(
				this.settings.CreateErrorForInvalidDate,
				Is.EqualTo(DataExchange.AppSettingsConstants.CreateErrorForInvalidDateDefaultValue));
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
				Is.EqualTo(DataExchange.AppSettingsConstants.CreateFoldersInWebApiDefaultValue));
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
				Is.EqualTo(DataExchange.AppSettingsConstants.CreateErrorForEmptyNativeFileDefaultValue));
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
				Is.EqualTo(DataExchange.AppSettingsConstants.DefaultMaxErrorCountDefaultValue));
			int expectedValue = RandomHelper.NextInt32(1, 1000);
			this.settings.DefaultMaxErrorCount = expectedValue;
			Assert.That(this.settings.DefaultMaxErrorCount, Is.EqualTo(expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheDisableImageLocationValidationSetting()
		{
			Assert.That(
				this.settings.DisableImageLocationValidation,
				Is.EqualTo(DataExchange.AppSettingsConstants.DisableImageLocationValidationDefaultValue));
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
				Is.EqualTo(DataExchange.AppSettingsConstants.DisableImageTypeValidationDefaultValue));
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
				Is.EqualTo(DataExchange.AppSettingsConstants.DisableTextFileEncodingCheckDefaultValue));
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
				Is.EqualTo(DataExchange.AppSettingsConstants.DisableThrowOnIllegalCharactersDefaultValue));
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
				Is.EqualTo(DataExchange.AppSettingsConstants.DynamicBatchResizingOnDefaultValue));
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
				Is.EqualTo(DataExchange.AppSettingsConstants.EnableCaseSensitiveSearchOnImportDefaultValue));
			bool expectedValue = RandomHelper.NextBoolean();
			this.settings.EnableCaseSensitiveSearchOnImport = expectedValue;
			Assert.That(this.settings.EnableCaseSensitiveSearchOnImport, Is.EqualTo(expectedValue));
			this.settings.EnableCaseSensitiveSearchOnImport = !expectedValue;
			Assert.That(this.settings.EnableCaseSensitiveSearchOnImport, Is.EqualTo(!expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheEnforceVersionCompatibilityCheckSetting()
		{
			IAppSettingsInternal backdoorAppSettings = this.settings as IAppSettingsInternal;
			Assert.That(backdoorAppSettings, Is.Not.Null);
			Assert.That(
				backdoorAppSettings.EnforceVersionCompatibilityCheck,
				Is.EqualTo(DataExchange.AppSettingsConstants.EnforceVersionCompatibilityCheckDefaultValue));
			bool expectedValue = RandomHelper.NextBoolean();
			backdoorAppSettings.EnforceVersionCompatibilityCheck = expectedValue;
			Assert.That(backdoorAppSettings.EnforceVersionCompatibilityCheck, Is.EqualTo(expectedValue));
			backdoorAppSettings.EnforceVersionCompatibilityCheck = !expectedValue;
			Assert.That(backdoorAppSettings.EnforceVersionCompatibilityCheck, Is.EqualTo(!expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheExportBatchSizeSetting()
		{
			Assert.That(
				this.settings.ExportBatchSize,
				Is.EqualTo(DataExchange.AppSettingsConstants.ExportBatchSizeDefaultValue));
			int expectedValue = RandomHelper.NextInt32(1, 1000);
			this.settings.ExportBatchSize = expectedValue;
			Assert.That(this.settings.ExportBatchSize, Is.EqualTo(expectedValue));
		}

		[TestCase(true, 0, AppSettingsConstants.ExportErrorNumberOfRetriesDefaultValue)]
		[TestCase(true, -1, AppSettingsConstants.ExportErrorNumberOfRetriesDefaultValue)]
		[TestCase(true, int.MinValue, AppSettingsConstants.ExportErrorNumberOfRetriesDefaultValue)]
		[TestCase(true, 5, 5)]
		[TestCase(true, 1, 1)]
		[TestCase(true, 2, 2)]
		[TestCase(true, 100, 100)]
		[TestCase(true, int.MaxValue, int.MaxValue)]
		[TestCase(false, 0, 0)]
		[TestCase(false, -1, -1)]
		[TestCase(false, int.MinValue, int.MinValue)]
		[TestCase(false, 5, 5)]
		[TestCase(false, 1, 1)]
		[TestCase(false, 2, 2)]
		[TestCase(false, 100, 100)]
		[TestCase(false, int.MaxValue, int.MaxValue)]
		public void ShouldGetAndSetTheExportErrorNumberOfRetriesSetting(bool enforceMinRetryCount, int input, int expectedValue)
		{
			Assert.That(
				this.settings.ExportErrorNumberOfRetries,
				Is.EqualTo(DataExchange.AppSettingsConstants.ExportErrorNumberOfRetriesDefaultValue));
			this.settings.EnforceMinRetryCount = enforceMinRetryCount;
			this.settings.ExportErrorNumberOfRetries = input;
			Assert.That(this.settings.ExportErrorNumberOfRetries, Is.EqualTo(expectedValue));
		}

		[TestCase(true, 0, AppSettingsConstants.ExportErrorWaitTimeInSecondsDefaultValue)]
		[TestCase(true, -1, AppSettingsConstants.ExportErrorWaitTimeInSecondsDefaultValue)]
		[TestCase(true, int.MinValue, AppSettingsConstants.ExportErrorWaitTimeInSecondsDefaultValue)]
		[TestCase(true, 5, 5)]
		[TestCase(true, 1, 1)]
		[TestCase(true, 2, 2)]
		[TestCase(true, 100, 100)]
		[TestCase(true, int.MaxValue, int.MaxValue)]
		[TestCase(false, 0, 0)]
		[TestCase(false, -1, -1)]
		[TestCase(false, int.MinValue, int.MinValue)]
		[TestCase(false, 5, 5)]
		[TestCase(false, 1, 1)]
		[TestCase(false, 2, 2)]
		[TestCase(false, 100, 100)]
		[TestCase(false, int.MaxValue, int.MaxValue)]
		public void ShouldGetAndSetTheExportErrorWaitTimeInSecondsSetting(bool enforceMinWaitTime, int input, int expectedValue)
		{
			Assert.That(
				this.settings.ExportErrorWaitTimeInSeconds,
				Is.EqualTo(DataExchange.AppSettingsConstants.ExportErrorWaitTimeInSecondsDefaultValue));
			this.settings.EnforceMinWaitTime = enforceMinWaitTime;
			this.settings.ExportErrorWaitTimeInSeconds = input;
			Assert.That(this.settings.ExportErrorWaitTimeInSeconds, Is.EqualTo(expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheExportThreadCountSetting()
		{
			Assert.That(
				this.settings.ExportThreadCount,
				Is.EqualTo(DataExchange.AppSettingsConstants.ExportThreadCountDefaultValue));
			int expectedValue = RandomHelper.NextInt32(1, 1000);
			this.settings.ExportThreadCount = expectedValue;
			Assert.That(this.settings.ExportThreadCount, Is.EqualTo(expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheFileTypeIdentifierTimeoutSecondsSetting()
		{
			Assert.That(
				this.settings.FileTypeIdentifyTimeoutSeconds,
				Is.EqualTo(AppSettingsConstants.FileTypeIdentifyTimeoutSecondsDefaultValue));
			int expectedValue = RandomHelper.NextInt32(5000, 30000);
			this.settings.FileTypeIdentifyTimeoutSeconds = expectedValue;
			Assert.That(this.settings.FileTypeIdentifyTimeoutSeconds, Is.EqualTo(expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheForceFolderPreviewSetting()
		{
			DeleteTestSubKey();
			Assert.That(
				this.settings.ForceFolderPreview,
				Is.EqualTo(DataExchange.AppSettingsConstants.ForceFolderPreviewDefaultValue));
			bool expectedValue = RandomHelper.NextBoolean();
			this.settings.ForceFolderPreview = expectedValue;
			Assert.That(this.settings.ForceFolderPreview, Is.EqualTo(expectedValue));
			this.settings.ForceFolderPreview = !expectedValue;
			Assert.That(this.settings.ForceFolderPreview, Is.EqualTo(!expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheForceWebUploadSetting()
		{
			Assert.That(
				this.settings.ForceWebUpload,
				Is.EqualTo(DataExchange.AppSettingsConstants.ForceWebUploadDefaultValue));
			bool expectedValue = RandomHelper.NextBoolean();
			this.settings.ForceWebUpload = expectedValue;
			Assert.That(this.settings.ForceWebUpload, Is.EqualTo(expectedValue));
			this.settings.ForceWebUpload = !expectedValue;
			Assert.That(this.settings.ForceWebUpload, Is.EqualTo(!expectedValue));
		}

		[TestCase(1, 1)]
		[TestCase(0, AppSettingsConstants.HttpTimeoutSecondsDefaultValue)]
		[TestCase(-1, AppSettingsConstants.HttpTimeoutSecondsDefaultValue)]
		[TestCase(2, 2)]
		[TestCase(5, 5)]
		[TestCase(100, 100)]
		[TestCase(int.MaxValue, int.MaxValue)]
		[TestCase(int.MinValue, AppSettingsConstants.HttpTimeoutSecondsDefaultValue)]
		public void ShouldGetAndSetTheHttpTimeoutSecondsSetting(int input, int expectedValue)
		{
			Assert.That(
				this.settings.HttpTimeoutSeconds,
				Is.EqualTo(DataExchange.AppSettingsConstants.HttpTimeoutSecondsDefaultValue));
			this.settings.HttpTimeoutSeconds = input;
			Assert.That(this.settings.HttpTimeoutSeconds, Is.EqualTo(expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheHttpExtractedTextTimeoutSecondsSetting()
		{
			Assert.That(
				this.settings.HttpExtractedTextTimeoutSeconds,
				Is.EqualTo(DataExchange.AppSettingsConstants.HttpExtractedTextTimeoutSecondsDefaultValue));
			int expectedValue = RandomHelper.NextInt32(1, 1000);
			this.settings.HttpExtractedTextTimeoutSeconds = expectedValue;
			Assert.That(this.settings.HttpExtractedTextTimeoutSeconds, Is.EqualTo(expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheImportBatchMaxVolumeSetting()
		{
			Assert.That(
				this.settings.ImportBatchMaxVolume,
				Is.EqualTo(DataExchange.AppSettingsConstants.ImportBatchMaxVolumeDefaultValue));
			int expectedValue = RandomHelper.NextInt32(1, 1000);
			this.settings.ImportBatchMaxVolume = expectedValue;
			Assert.That(this.settings.ImportBatchMaxVolume, Is.EqualTo(expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheImportBatchSizeSetting()
		{
			Assert.That(
				this.settings.ImportBatchSize,
				Is.EqualTo(DataExchange.AppSettingsConstants.ImportBatchSizeDefaultValue));
			int expectedValue = RandomHelper.NextInt32(1, 1000);
			this.settings.ImportBatchSize = expectedValue;
			Assert.That(this.settings.ImportBatchSize, Is.EqualTo(expectedValue));
		}

		[TestCase(true, 0, AppSettingsConstants.IoErrorNumberOfRetriesDefaultValue)]
		[TestCase(true, -1, AppSettingsConstants.IoErrorNumberOfRetriesDefaultValue)]
		[TestCase(true, int.MinValue, AppSettingsConstants.IoErrorNumberOfRetriesDefaultValue)]
		[TestCase(true, 5, 5)]
		[TestCase(true, 1, 1)]
		[TestCase(true, 2, 2)]
		[TestCase(true, 100, 100)]
		[TestCase(true, int.MaxValue, int.MaxValue)]
		[TestCase(false, 0, 0)]
		[TestCase(false, -1, -1)]
		[TestCase(false, int.MinValue, int.MinValue)]
		[TestCase(false, 5, 5)]
		[TestCase(false, 1, 1)]
		[TestCase(false, 2, 2)]
		[TestCase(false, 100, 100)]
		[TestCase(false, int.MaxValue, int.MaxValue)]
		public void ShouldGetAndSetTheIoErrorNumberOfRetriesSetting(bool enforceMinRetryCount, int input, int expectedValue)
		{
			Assert.That(
				this.settings.IoErrorNumberOfRetries,
				Is.EqualTo(DataExchange.AppSettingsConstants.IoErrorNumberOfRetriesDefaultValue));
			this.settings.EnforceMinRetryCount = enforceMinRetryCount;
			this.settings.IoErrorNumberOfRetries = input;
			Assert.That(this.settings.IoErrorNumberOfRetries, Is.EqualTo(expectedValue));
		}

		[TestCase(true, 0, AppSettingsConstants.IoErrorWaitTimeInSecondsDefaultValue)]
		[TestCase(true, -1, AppSettingsConstants.IoErrorWaitTimeInSecondsDefaultValue)]
		[TestCase(true, int.MinValue, AppSettingsConstants.IoErrorWaitTimeInSecondsDefaultValue)]
		[TestCase(true, 5, 5)]
		[TestCase(true, 1, 1)]
		[TestCase(true, 2, 2)]
		[TestCase(true, 100, 100)]
		[TestCase(true, int.MaxValue, int.MaxValue)]
		[TestCase(false, 0, 0)]
		[TestCase(false, -1, -1)]
		[TestCase(false, int.MinValue, int.MinValue)]
		[TestCase(false, 5, 5)]
		[TestCase(false, 1, 1)]
		[TestCase(false, 2, 2)]
		[TestCase(false, 100, 100)]
		[TestCase(false, int.MaxValue, int.MaxValue)]
		public void ShouldGetAndSetTheIoErrorWaitTimeInSecondsSetting(bool enforceMinWaitTime, int input, int expectedValue)
		{
			Assert.That(
				this.settings.IoErrorWaitTimeInSeconds,
				Is.EqualTo(DataExchange.AppSettingsConstants.IoErrorWaitTimeInSecondsDefaultValue));
			this.settings.EnforceMinWaitTime = enforceMinWaitTime;
			this.settings.IoErrorWaitTimeInSeconds = input;
			Assert.That(this.settings.IoErrorWaitTimeInSeconds, Is.EqualTo(expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheJobCompleteBatchSizeSetting()
		{
			Assert.That(
				this.settings.JobCompleteBatchSize,
				Is.EqualTo(DataExchange.AppSettingsConstants.JobCompleteBatchSizeDefaultValue));
			int expectedValue = RandomHelper.NextInt32(1, 1000);
			this.settings.JobCompleteBatchSize = expectedValue;
			Assert.That(this.settings.JobCompleteBatchSize, Is.EqualTo(expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheLoadImportedFullTextFromServerSetting()
		{
			Assert.That(
				this.settings.LoadImportedFullTextFromServer,
				Is.EqualTo(DataExchange.AppSettingsConstants.LoadImportedFullTextFromServerDefaultValue));
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
				Is.EqualTo(DataExchange.AppSettingsConstants.LogAllEventsDefaultValue));
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
				Is.EqualTo(DataExchange.AppSettingsConstants.LogConfigXmlFileNameDefaultValue));
			string expectedValue = RandomHelper.NextString(10, 100);
			this.settings.LogConfigXmlFileName = expectedValue;
			Assert.That(this.settings.LogConfigXmlFileName, Is.EqualTo(expectedValue));
		}

		[TestCase(1, 1)]
		[TestCase(0, AppSettingsConstants.MaximumReloginTriesDefaultValue)]
		[TestCase(-1, AppSettingsConstants.MaximumReloginTriesDefaultValue)]
		[TestCase(2, 2)]
		[TestCase(5, 5)]
		[TestCase(100, 100)]
		[TestCase(int.MaxValue, int.MaxValue)]
		[TestCase(int.MinValue, AppSettingsConstants.MaximumReloginTriesDefaultValue)]
		public void ShouldGetAndSetTheMaxReloginTriesSetting(int input, int expectedValue)
		{
			Assert.That(
				this.settings.MaxReloginTries,
				Is.EqualTo(DataExchange.AppSettingsConstants.MaximumReloginTriesDefaultValue));
			this.settings.MaxReloginTries = input;
			Assert.That(this.settings.MaxReloginTries, Is.EqualTo(expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheOAuth2ImplicitCredentialRedirectUrlSetting()
		{
			Assert.That(
				this.settings.OAuth2ImplicitCredentialRedirectUrl,
				Is.EqualTo(AppSettingsConstants.OAuth2ImplicitCredentialRedirectUrlDefaultValue));
			Uri expectedValue = RandomHelper.NextUri();
			this.settings.OAuth2ImplicitCredentialRedirectUrl = expectedValue.ToString();
			Assert.That(this.settings.OAuth2ImplicitCredentialRedirectUrl, Is.EqualTo(expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheMinBatchSizeSetting()
		{
			Assert.That(
				this.settings.MinBatchSize,
				Is.EqualTo(DataExchange.AppSettingsConstants.MinBatchSizeDefaultValue));
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
			DataExchange.AppSettingsManager.SetRegistryKeyValue(DataExchange.AppSettingsConstants.OpenIdConnectHomeRealmDiscoveryHintKey, string.Empty);
			Assert.That(this.settings.OpenIdConnectHomeRealmDiscoveryHint, Is.Empty);
			DataExchange.AppSettingsManager.SetRegistryKeyValue(DataExchange.AppSettingsConstants.OpenIdConnectHomeRealmDiscoveryHintKey, "HRD-HINT-VALUE");
			Assert.That(this.settings.OpenIdConnectHomeRealmDiscoveryHint, Is.EqualTo("HRD-HINT-VALUE"));

			// Ensure that we don't fail when the key/value doesn't exist.
			DeleteTestSubKey();
			Assert.That(this.settings.OpenIdConnectHomeRealmDiscoveryHint, Is.Empty);
		}

		[Test]
		public void ShouldGetAndSetThePermissionErrorsRetrySetting()
		{
			// Note: This value also enables and disables the RetryOptions.Permissions value.
			Assert.That(this.settings.PermissionErrorsRetry, Is.EqualTo(DataExchange.AppSettingsConstants.PermissionErrorsRetryDefaultValue));
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
				Is.EqualTo(DataExchange.AppSettingsConstants.PreviewThresholdDefaultValue));
			int expectedValue = RandomHelper.NextInt32(1, 1000);
			this.settings.PreviewThreshold = expectedValue;
			Assert.That(this.settings.PreviewThreshold, Is.EqualTo(expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheProcessFormRefreshRateSetting()
		{
			Assert.That(
				this.settings.ProcessFormRefreshRate,
				Is.EqualTo(DataExchange.AppSettingsConstants.ProcessFormRefreshRateDefaultValue));
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
		public void ShouldGetAndSetTheSuppressServerCertificateValidationSetting()
		{
			Assert.That(
				this.settings.SuppressServerCertificateValidation,
				Is.EqualTo(DataExchange.AppSettingsConstants.SuppressServerCertificateValidationDefaultValue));
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
			string expectedValue = RandomHelper.NextString(25, 50);
			this.settings.TapiAsperaBcpRootFolder = expectedValue;
			Assert.That(this.settings.TapiAsperaBcpRootFolder, Is.EqualTo(expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheTapiAsperaNativeDocRootLevelsSetting()
		{
			Assert.That(
				this.settings.TapiAsperaNativeDocRootLevels,
				Is.EqualTo(DataExchange.AppSettingsConstants.TapiAsperaNativeDocRootLevelsDefaultValue));
			int expectedValue = RandomHelper.NextInt32(1, 1000);
			this.settings.TapiAsperaNativeDocRootLevels = expectedValue;
			Assert.That(this.settings.TapiAsperaNativeDocRootLevels, Is.EqualTo(expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheTapiBadPathErrorsRetrySetting()
		{
			Assert.That(
				this.settings.TapiBadPathErrorsRetry,
				Is.EqualTo(DataExchange.AppSettingsConstants.TapiBadPathErrorsRetryDefaultValue));
			bool expectedValue = RandomHelper.NextBoolean();
			this.settings.TapiBadPathErrorsRetry = expectedValue;
			Assert.That(this.settings.TapiBadPathErrorsRetry, Is.EqualTo(expectedValue));
			this.settings.TapiBadPathErrorsRetry = !expectedValue;
			Assert.That(this.settings.TapiBadPathErrorsRetry, Is.EqualTo(!expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheTapiFileNotFoundErrorsDisabledSetting()
		{
			Assert.That(
				this.settings.TapiFileNotFoundErrorsDisabled,
				Is.EqualTo(DataExchange.AppSettingsConstants.TapiFileNotFoundErrorsDisabledDefaultValue));
			bool expectedValue = RandomHelper.NextBoolean();
			this.settings.TapiFileNotFoundErrorsDisabled = expectedValue;
			Assert.That(this.settings.TapiFileNotFoundErrorsDisabled, Is.EqualTo(expectedValue));
			this.settings.TapiFileNotFoundErrorsDisabled = !expectedValue;
			Assert.That(this.settings.TapiFileNotFoundErrorsDisabled, Is.EqualTo(!expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheTapiFileNotFoundErrorsRetrySetting()
		{
			Assert.That(
				this.settings.TapiFileNotFoundErrorsRetry,
				Is.EqualTo(DataExchange.AppSettingsConstants.TapiFileNotFoundErrorsRetryDefaultValue));
			bool expectedValue = RandomHelper.NextBoolean();
			this.settings.TapiFileNotFoundErrorsRetry = expectedValue;
			Assert.That(this.settings.TapiFileNotFoundErrorsRetry, Is.EqualTo(expectedValue));
			this.settings.TapiFileNotFoundErrorsRetry = !expectedValue;
			Assert.That(this.settings.TapiFileNotFoundErrorsRetry, Is.EqualTo(!expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheTapiForceAsperaClientSetting()
		{
			Assert.That(
				this.settings.TapiForceAsperaClient,
				Is.EqualTo(DataExchange.AppSettingsConstants.TapiForceAsperaClientDefaultValue));
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
				Is.EqualTo(DataExchange.AppSettingsConstants.TapiForceBcpHttpClientDefaultValue));
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
				Is.EqualTo(DataExchange.AppSettingsConstants.TapiForceFileShareClientDefaultValue));
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
				Is.EqualTo(DataExchange.AppSettingsConstants.TapiForceHttpClientDefaultValue));
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
				Is.EqualTo(DataExchange.AppSettingsConstants.TapiLargeFileProgressEnabledDefaultValue));
			bool expectedValue = RandomHelper.NextBoolean();
			this.settings.TapiLargeFileProgressEnabled = expectedValue;
			Assert.That(this.settings.TapiLargeFileProgressEnabled, Is.EqualTo(expectedValue));
			this.settings.TapiLargeFileProgressEnabled = !expectedValue;
			Assert.That(this.settings.TapiLargeFileProgressEnabled, Is.EqualTo(!expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheTapiMaxInactivitySecondsSetting()
		{
			Assert.That(
				this.settings.TapiMaxInactivitySeconds,
				Is.EqualTo(DataExchange.AppSettingsConstants.TapiMaxInactivitySecondsDefaultValue));
			int expectedValue = RandomHelper.NextInt32(1, 1000);
			this.settings.TapiMaxInactivitySeconds = expectedValue;
			Assert.That(this.settings.TapiMaxInactivitySeconds, Is.EqualTo(expectedValue));
		}

		[TestCase(2, 2)]
		[TestCase(100, 100)]
		[TestCase(int.MaxValue, int.MaxValue)]
		[TestCase(int.MinValue, AppSettingsConstants.TapiMaxJobParallelismDefaultValue)]
		public void ShouldGetAndSetTheTapiMaxJobParallelismSetting(int input, int expectedValue)
		{
			Assert.That(
				this.settings.TapiMaxJobParallelism,
				Is.EqualTo(DataExchange.AppSettingsConstants.TapiMaxJobParallelismDefaultValue));
			this.settings.TapiMaxJobParallelism = input;
			Assert.That(this.settings.TapiMaxJobParallelism, Is.EqualTo(expectedValue));
		}

		[TestCase(1, 1)]
		[TestCase(0, AppSettingsConstants.TapiMinDataRateMbpsDefaultValue)]
		[TestCase(-1, AppSettingsConstants.TapiMinDataRateMbpsDefaultValue)]
		[TestCase(2, 2)]
		[TestCase(5, 5)]
		[TestCase(100, 100)]
		[TestCase(int.MaxValue, int.MaxValue)]
		[TestCase(int.MinValue, AppSettingsConstants.TapiMinDataRateMbpsDefaultValue)]
		public void ShouldGetAndSetTheTapiMinDataRateMbpsSetting(int input, int expectedValue)
		{
			Assert.That(
				this.settings.TapiMinDataRateMbps,
				Is.EqualTo(DataExchange.AppSettingsConstants.TapiMinDataRateMbpsDefaultValue));
			this.settings.TapiMinDataRateMbps = input;
			Assert.That(this.settings.TapiMinDataRateMbps, Is.EqualTo(expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheTapiPreserveFileTimestampsSetting()
		{
			Assert.That(
				this.settings.TapiPreserveFileTimestamps,
				Is.EqualTo(DataExchange.AppSettingsConstants.TapiPreserveFileTimestampsDefaultValue));
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
				Is.EqualTo(DataExchange.AppSettingsConstants.TapiSubmitApmMetricsDefaultValue));
			bool expectedValue = RandomHelper.NextBoolean();
			this.settings.TapiSubmitApmMetrics = expectedValue;
			Assert.That(this.settings.TapiSubmitApmMetrics, Is.EqualTo(expectedValue));
			this.settings.TapiSubmitApmMetrics = !expectedValue;
			Assert.That(this.settings.TapiSubmitApmMetrics, Is.EqualTo(!expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheTelemetrySubmitApmMetricsSetting()
		{
			Assert.That(
				this.settings.TelemetrySubmitApmMetrics,
				Is.EqualTo(DataExchange.AppSettingsConstants.TelemetrySubmitApmMetricsDefaultValue));
			bool expectedValue = RandomHelper.NextBoolean();
			this.settings.TelemetrySubmitApmMetrics = expectedValue;
			Assert.That(this.settings.TelemetrySubmitApmMetrics, Is.EqualTo(expectedValue));
			this.settings.TelemetrySubmitApmMetrics = !expectedValue;
			Assert.That(this.settings.TelemetrySubmitApmMetrics, Is.EqualTo(!expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheTelemetrySubmitSumMetricsSetting()
		{
			Assert.That(
				this.settings.TelemetrySubmitSumMetrics,
				Is.EqualTo(DataExchange.AppSettingsConstants.TelemetrySubmitSumMetricsDefaultValue));
			bool expectedValue = RandomHelper.NextBoolean();
			this.settings.TelemetrySubmitSumMetrics = expectedValue;
			Assert.That(this.settings.TelemetrySubmitSumMetrics, Is.EqualTo(expectedValue));
			this.settings.TelemetrySubmitSumMetrics = !expectedValue;
			Assert.That(this.settings.TelemetrySubmitSumMetrics, Is.EqualTo(!expectedValue));
		}

		[TestCase(1, 1)]
		[TestCase(0, AppSettingsConstants.TelemetryMetricsThrottlingSecondsDefaultValue)]
		[TestCase(-1, AppSettingsConstants.TelemetryMetricsThrottlingSecondsDefaultValue)]
		[TestCase(2, 2)]
		[TestCase(5, 5)]
		[TestCase(100, 100)]
		[TestCase(int.MaxValue, int.MaxValue)]
		[TestCase(int.MinValue, AppSettingsConstants.TelemetryMetricsThrottlingSecondsDefaultValue)]
		public void ShouldGetAndSetTheTelemetryMetricsThrottlingSecondsSetting(int input, int expectedValue)
		{
			Assert.That(
				this.settings.TelemetryMetricsThrottlingSeconds,
				Is.EqualTo(DataExchange.AppSettingsConstants.TelemetryMetricsThrottlingSecondsDefaultValue));
			this.settings.TelemetryMetricsThrottlingSeconds = input;
			Assert.That(this.settings.TelemetryMetricsThrottlingSeconds, Is.EqualTo(expectedValue));
		}

		[TestCase(1, 1)]
		[TestCase(0, AppSettingsConstants.TapiTargetDataRateMbpsDefaultValue)]
		[TestCase(-1, AppSettingsConstants.TapiTargetDataRateMbpsDefaultValue)]
		[TestCase(2, 2)]
		[TestCase(5, 5)]
		[TestCase(100, 100)]
		[TestCase(int.MaxValue, int.MaxValue)]
		[TestCase(int.MinValue, AppSettingsConstants.TapiTargetDataRateMbpsDefaultValue)]
		public void ShouldGetAndSetTheTapiTargetDataRateMbpsSetting(int input, int expectedValue)
		{
			Assert.That(
				this.settings.TapiTargetDataRateMbps,
				Is.EqualTo(DataExchange.AppSettingsConstants.TapiTargetDataRateMbpsDefaultValue));
			this.settings.TapiTargetDataRateMbps = input;
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
				Is.EqualTo(DataExchange.AppSettingsConstants.UseOldExportDefaultValue));
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
				Is.EqualTo(DataExchange.AppSettingsConstants.UsePipeliningForNativeAndObjectImportsDefaultValue));
			bool expectedValue = RandomHelper.NextBoolean();
			this.settings.UsePipeliningForNativeAndObjectImports = expectedValue;
			Assert.That(this.settings.UsePipeliningForNativeAndObjectImports, Is.EqualTo(expectedValue));
			this.settings.UsePipeliningForNativeAndObjectImports = !expectedValue;
			Assert.That(this.settings.UsePipeliningForNativeAndObjectImports, Is.EqualTo(!expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheValueRefreshThresholdSetting()
		{
			Assert.That(
				this.settings.ValueRefreshThreshold,
				Is.EqualTo(DataExchange.AppSettingsConstants.ValueRefreshThresholdDefaultValue));
			int expectedValue = RandomHelper.NextInt32(100, 1000);
			this.settings.ValueRefreshThreshold = expectedValue;
			Assert.That(this.settings.ValueRefreshThreshold, Is.EqualTo(expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheWaitBeforeReconnectSetting()
		{
			Assert.That(
				this.settings.WaitBeforeReconnect,
				Is.EqualTo(DataExchange.AppSettingsConstants.WaitBeforeReconnectDefaultValue));
			int expectedValue = RandomHelper.NextInt32(1, 1000);
			this.settings.WaitBeforeReconnect = expectedValue;
			Assert.That(this.settings.WaitBeforeReconnect, Is.EqualTo(expectedValue));
		}

		[Test]
		public void ShouldGetAndSetTheWebApiOperationTimeoutSetting()
		{
			Assert.That(
				this.settings.WebApiOperationTimeout,
				Is.EqualTo(DataExchange.AppSettingsConstants.WebApiOperationTimeoutDefaultValue));
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
			DataExchange.AppSettingsManager.SetRegistryKeyValue(DataExchange.AppSettingsConstants.WebApiServiceUrlRegistryKey, "https://relativity-2.one.com");
			Assert.That(this.settings.ProgrammaticWebApiServiceUrl, Is.Empty);
			Assert.That(this.settings.WebApiServiceUrl, Is.EqualTo("https://relativity-2.one.com/"));
			DeleteTestSubKey();
			Assert.That(this.settings.ProgrammaticWebApiServiceUrl, Is.Empty);
			Assert.That(this.settings.WebApiServiceUrl, Is.Empty);

			// This simulates the scenario where the URL comes from programmatic means.
			DataExchange.AppSettingsManager.SetRegistryKeyValue(DataExchange.AppSettingsConstants.WebApiServiceUrlRegistryKey, "https://relativity-3.one.com");
			this.settings.ProgrammaticWebApiServiceUrl = "https://relativity-3.one.com";
			Assert.That(this.settings.ProgrammaticWebApiServiceUrl, Is.EqualTo("https://relativity-3.one.com/"));
			Assert.That(this.settings.WebApiServiceUrl, Is.EqualTo("https://relativity-3.one.com/"));
		}

		[Test]
		public void ShouldGetAndSetTheWebBasedFileDownloadChunkSizeSetting()
		{
			Assert.That(
				this.settings.WebBasedFileDownloadChunkSize,
				Is.EqualTo(DataExchange.AppSettingsConstants.WebBasedFileDownloadChunkSizeDefaultValue));
			int expectedValue = RandomHelper.NextInt32(
				DataExchange.AppSettingsConstants.WebBasedFileDownloadChunkSizeMinValue,
				DataExchange.AppSettingsConstants.WebBasedFileDownloadChunkSizeMinValue + 100000);
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
			// Make sure the entire design works against a real app.config file that was ripped from the RDC but with different values.
			const bool Refresh = false;
			this.settings = DataExchange.AppSettingsManager.Create(Refresh);
			for (int i = 0; i < 3; i++)
			{
				DataExchange.AppSettingsManager.Refresh(this.settings);

				// The kCura.Utility section asserts go here.
				Assert.That(this.settings.CreateErrorForInvalidDate, Is.True);
				Assert.That(this.settings.ExportErrorNumberOfRetries, Is.EqualTo(5));
				Assert.That(this.settings.ExportErrorWaitTimeInSeconds, Is.EqualTo(15));
				Assert.That(this.settings.IoErrorNumberOfRetries, Is.EqualTo(8));
				Assert.That(this.settings.IoErrorWaitTimeInSeconds, Is.EqualTo(16));

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
				Assert.That(this.settings.ForceWebUpload, Is.True);
				Assert.That(this.settings.HttpTimeoutSeconds, Is.EqualTo(23));
				Assert.That(this.settings.ImportBatchMaxVolume, Is.EqualTo(12345));
				Assert.That(this.settings.ImportBatchSize, Is.EqualTo(102));
				Assert.That(this.settings.JobCompleteBatchSize, Is.EqualTo(999));
				Assert.That(this.settings.LogConfigXmlFileName, Is.EqualTo("CustomLog.xml"));
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

				// The Relativity.DataExchange section asserts go here.
				Assert.That(this.settings.TapiFileNotFoundErrorsRetry, Is.True);
				Assert.That(this.settings.TapiFileNotFoundErrorsRetry, Is.True);
				Assert.That(this.settings.TapiMaxInactivitySeconds, Is.EqualTo(99));
				Assert.That(this.settings.FileTypeIdentifyTimeoutSeconds, Is.EqualTo(456));
				Assert.That(this.settings.OAuth2ImplicitCredentialRedirectUrl, Is.EqualTo("http://relativity"));
				Assert.That(this.settings.HttpExtractedTextTimeoutSeconds, Is.EqualTo(96));
			}
		}

		[Test]
		public void ShouldGetTheAppDictionaryDirectly()
		{
			const bool Refresh = true;
			this.settings = DataExchange.AppSettingsManager.Create(Refresh);
			AppSettingsDictionary dictionary = new AppSettingsDictionary(this.settings);
			for (int i = 0; i < 3; i++)
			{
				// The kCure.Config section asserts go here.
				Assert.That(dictionary[DataExchange.AppSettingsConstants.ValueRefreshThresholdKey], Is.EqualTo(123456789));

				// The kCura.Utility section asserts go here.
				Assert.That(dictionary[DataExchange.AppSettingsConstants.CreateErrorForInvalidDateKey], Is.True);
				Assert.That(dictionary[DataExchange.AppSettingsConstants.ExportErrorNumberOfRetriesKey], Is.EqualTo(5));
				Assert.That(dictionary[DataExchange.AppSettingsConstants.ExportErrorWaitTimeInSecondsKey], Is.EqualTo(15));
				Assert.That(dictionary[DataExchange.AppSettingsConstants.IoErrorNumberOfRetriesKey], Is.EqualTo(8));
				Assert.That(dictionary[DataExchange.AppSettingsConstants.IoErrorWaitTimeInSecondsKey], Is.EqualTo(16));

				// The kCura.WinEDDS section asserts go here.
				Assert.That(dictionary[DataExchange.AppSettingsConstants.ApplicationNameKey], Is.EqualTo("Custom App"));
				Assert.That(dictionary[DataExchange.AppSettingsConstants.CreateErrorForEmptyNativeFileKey], Is.True);
				Assert.That(dictionary[DataExchange.AppSettingsConstants.DefaultMaxErrorCountKey], Is.EqualTo(555));
				Assert.That(dictionary[DataExchange.AppSettingsConstants.DisableImageLocationValidationKey], Is.True);
				Assert.That(dictionary[DataExchange.AppSettingsConstants.DisableImageTypeValidationKey], Is.True);
				Assert.That(dictionary[DataExchange.AppSettingsConstants.DisableTextFileEncodingCheckKey], Is.True);
				Assert.That(dictionary[DataExchange.AppSettingsConstants.DisableThrowOnIllegalCharactersKey], Is.True);
				Assert.That(dictionary[DataExchange.AppSettingsConstants.DynamicBatchResizingOnKey], Is.False);
				Assert.That(dictionary[DataExchange.AppSettingsConstants.ExportBatchSizeKey], Is.EqualTo(255));
				Assert.That(dictionary[DataExchange.AppSettingsConstants.ExportThreadCountKey], Is.EqualTo(3));
				Assert.That(dictionary[DataExchange.AppSettingsConstants.ForceWebUploadKey], Is.True);
				Assert.That(dictionary[DataExchange.AppSettingsConstants.HttpTimeoutSecondsKey], Is.EqualTo(23));
				Assert.That(dictionary[DataExchange.AppSettingsConstants.ImportBatchMaxVolumeKey], Is.EqualTo(12345));
				Assert.That(dictionary[DataExchange.AppSettingsConstants.ImportBatchSizeKey], Is.EqualTo(102));
				Assert.That(dictionary[DataExchange.AppSettingsConstants.LogConfigXmlFileNameKey], Is.EqualTo("CustomLog.xml"));
				Assert.That(dictionary[DataExchange.AppSettingsConstants.MaximumReloginTriesKey], Is.EqualTo(42));
				Assert.That(dictionary[DataExchange.AppSettingsConstants.MinBatchSizeKey], Is.EqualTo(29));
				Assert.That(dictionary[DataExchange.AppSettingsConstants.PermissionErrorsRetryKey], Is.False);
				Assert.That(dictionary[DataExchange.AppSettingsConstants.PreviewThresholdKey], Is.EqualTo(49));
				Assert.That(dictionary[DataExchange.AppSettingsConstants.SuppressServerCertificateValidationKey], Is.True);
				Assert.That(dictionary[DataExchange.AppSettingsConstants.TapiAsperaBcpRootFolderKey], Is.EqualTo("Root"));
				Assert.That(dictionary[DataExchange.AppSettingsConstants.TapiBadPathErrorsRetryKey], Is.False);
				Assert.That(dictionary[DataExchange.AppSettingsConstants.TapiForceAsperaClientKey], Is.True);
				Assert.That(dictionary[DataExchange.AppSettingsConstants.TapiForceBcpHttpClientKey], Is.True);
				Assert.That(dictionary[DataExchange.AppSettingsConstants.TapiForceFileShareClientKey], Is.True);
				Assert.That(dictionary[DataExchange.AppSettingsConstants.TapiForceHttpClientKey], Is.True);
				Assert.That(dictionary[DataExchange.AppSettingsConstants.TapiForceClientCandidatesKey], Is.EqualTo("FileShare;Aspera;Http"));
				Assert.That(dictionary[DataExchange.AppSettingsConstants.TapiLargeFileProgressEnabledKey], Is.True);
				Assert.That(dictionary[DataExchange.AppSettingsConstants.TapiMaxJobParallelismKey], Is.EqualTo(5));
				Assert.That(dictionary[DataExchange.AppSettingsConstants.TapiMinDataRateMbpsKey], Is.EqualTo(30));
				Assert.That(dictionary[DataExchange.AppSettingsConstants.TapiSubmitApmMetricsKey], Is.False);
				Assert.That(dictionary[DataExchange.AppSettingsConstants.TapiTargetDataRateMbpsKey], Is.EqualTo(50));
				Assert.That(dictionary[DataExchange.AppSettingsConstants.TapiTransferLogDirectoryKey], Is.EqualTo(@"%temp%\IAPI_log\"));
				Assert.That(dictionary[DataExchange.AppSettingsConstants.TempDirectoryKey], Is.EqualTo(@"C:\"));
				Assert.That(dictionary[DataExchange.AppSettingsConstants.UseOldExportKey], Is.True);
				Assert.That(dictionary[DataExchange.AppSettingsConstants.UsePipeliningForNativeAndObjectImportsKey], Is.True);
				Assert.That(dictionary[DataExchange.AppSettingsConstants.WaitBeforeReconnectKey], Is.EqualTo(64));
				Assert.That(dictionary[DataExchange.AppSettingsConstants.WebApiOperationTimeoutKey], Is.EqualTo(333));
				Assert.That(dictionary[DataExchange.AppSettingsConstants.WebApiServiceUrlKey], Is.EqualTo("https://relativity.one.com/"));
				Assert.That(dictionary[DataExchange.AppSettingsConstants.WebBasedFileDownloadChunkSizeKey], Is.EqualTo(11111));

				// The kCura.Windows.Process section asserts go here.
				Assert.That(dictionary[DataExchange.AppSettingsConstants.LogAllEventsKey], Is.True);

				// The new Relativity.DataExchange section asserts go here.
				Assert.That(dictionary[DataExchange.AppSettingsConstants.EnforceVersionCompatibilityCheckKey], Is.True);
				Assert.That(dictionary[DataExchange.AppSettingsConstants.FileTypeIdentifyTimeoutSecondsKey], Is.EqualTo(456));
				Assert.That(dictionary[DataExchange.AppSettingsConstants.HttpExtractedTextTimeoutSecondsKey], Is.EqualTo(96));
				Assert.That(dictionary[DataExchange.AppSettingsConstants.OAuth2ImplicitCredentialRedirectUrlKey], Is.EqualTo("http://relativity"));
				Assert.That(dictionary[DataExchange.AppSettingsConstants.TapiFileNotFoundErrorsDisabledKey], Is.True);
				Assert.That(dictionary[DataExchange.AppSettingsConstants.TapiFileNotFoundErrorsRetryKey], Is.True);
				Assert.That(dictionary[DataExchange.AppSettingsConstants.TelemetrySubmitApmMetricsKey], Is.False);
				Assert.That(dictionary[DataExchange.AppSettingsConstants.TelemetrySubmitSumMetricsKey], Is.False);
			}
		}

		[Test]
		public void ShouldSetTheAppDictionaryDirectly()
		{
			const bool Refresh = false;
			this.settings = DataExchange.AppSettingsManager.Create(Refresh);
			AppSettingsDictionary dictionary = new AppSettingsDictionary(this.settings);
			for (int i = 0; i < 3; i++)
			{
				// The kCure.Config section asserts go here.
				dictionary[DataExchange.AppSettingsConstants.ValueRefreshThresholdKey] = 876543210;
				Assert.That(this.settings.ValueRefreshThreshold, Is.EqualTo(876543210));

				// The kCura.Utility section asserts go here.
				dictionary[DataExchange.AppSettingsConstants.CreateErrorForInvalidDateKey] = true;
				Assert.That(this.settings.CreateErrorForInvalidDate, Is.True);
				dictionary[DataExchange.AppSettingsConstants.ExportErrorNumberOfRetriesKey] = 3;
				Assert.That(this.settings.ExportErrorNumberOfRetries, Is.EqualTo(3));
				dictionary[DataExchange.AppSettingsConstants.ExportErrorWaitTimeInSecondsKey] = 4;
				Assert.That(this.settings.ExportErrorWaitTimeInSeconds, Is.EqualTo(4));
				dictionary[DataExchange.AppSettingsConstants.IoErrorNumberOfRetriesKey] = 5;
				Assert.That(this.settings.IoErrorNumberOfRetries, Is.EqualTo(5));
				dictionary[DataExchange.AppSettingsConstants.IoErrorWaitTimeInSecondsKey] = 6;
				Assert.That(this.settings.IoErrorWaitTimeInSeconds, Is.EqualTo(6));

				// The kCura.WinEDDS section asserts go here.
				dictionary[DataExchange.AppSettingsConstants.ApplicationNameKey] = "The App";
				Assert.That(this.settings.ApplicationName, Is.EqualTo("The App"));
				dictionary[DataExchange.AppSettingsConstants.CreateErrorForEmptyNativeFileKey] = true;
				Assert.That(this.settings.CreateErrorForEmptyNativeFile, Is.True);
				dictionary[DataExchange.AppSettingsConstants.DefaultMaxErrorCountKey] = 8;
				Assert.That(this.settings.DefaultMaxErrorCount, Is.EqualTo(8));
				dictionary[DataExchange.AppSettingsConstants.DisableImageLocationValidationKey] = true;
				Assert.That(this.settings.DisableImageLocationValidation, Is.True);
				dictionary[DataExchange.AppSettingsConstants.DisableImageTypeValidationKey] = true;
				Assert.That(this.settings.DisableImageTypeValidation, Is.True);
				dictionary[DataExchange.AppSettingsConstants.DisableTextFileEncodingCheckKey] = true;
				Assert.That(this.settings.DisableTextFileEncodingCheck, Is.True);
				dictionary[DataExchange.AppSettingsConstants.DisableThrowOnIllegalCharactersKey] = true;
				Assert.That(this.settings.DisableThrowOnIllegalCharacters, Is.True);
				dictionary[DataExchange.AppSettingsConstants.DynamicBatchResizingOnKey] = true;
				Assert.That(this.settings.DynamicBatchResizingOn, Is.True);
				dictionary[DataExchange.AppSettingsConstants.ExportBatchSizeKey] = 9;
				Assert.That(this.settings.ExportBatchSize, Is.EqualTo(9));
				dictionary[DataExchange.AppSettingsConstants.ExportThreadCountKey] = 10;
				Assert.That(this.settings.ExportThreadCount, Is.EqualTo(10));
				dictionary[DataExchange.AppSettingsConstants.ForceWebUploadKey] = true;
				Assert.That(this.settings.ForceWebUpload, Is.True);
				dictionary[DataExchange.AppSettingsConstants.ImportBatchMaxVolumeKey] = 11;
				Assert.That(this.settings.ImportBatchMaxVolume, Is.EqualTo(11));
				dictionary[DataExchange.AppSettingsConstants.HttpExtractedTextTimeoutSecondsKey] = 77;
				Assert.That(this.settings.HttpExtractedTextTimeoutSeconds, Is.EqualTo(77));
				dictionary[DataExchange.AppSettingsConstants.HttpTimeoutSecondsKey] = 134;
				Assert.That(this.settings.HttpTimeoutSeconds, Is.EqualTo(134));
				dictionary[DataExchange.AppSettingsConstants.ImportBatchSizeKey] = 12;
				Assert.That(this.settings.ImportBatchSize, Is.EqualTo(12));
				dictionary[DataExchange.AppSettingsConstants.LogConfigXmlFileNameKey] = "abc";
				Assert.That(this.settings.LogConfigXmlFileName, Is.EqualTo("abc"));
				dictionary[DataExchange.AppSettingsConstants.MaximumReloginTriesKey] = 14;
				Assert.That(this.settings.MaxReloginTries, Is.EqualTo(14));
				dictionary[DataExchange.AppSettingsConstants.MinBatchSizeKey] = 15;
				Assert.That(this.settings.MinBatchSize, Is.EqualTo(15));
				dictionary[DataExchange.AppSettingsConstants.PermissionErrorsRetryKey] = true;
				Assert.That(this.settings.PermissionErrorsRetry, Is.True);
				dictionary[DataExchange.AppSettingsConstants.PreviewThresholdKey] = 16;
				Assert.That(this.settings.PreviewThreshold, Is.EqualTo(16));
				dictionary[DataExchange.AppSettingsConstants.SuppressServerCertificateValidationKey] = true;
				Assert.That(this.settings.SuppressServerCertificateValidation, Is.True);
				dictionary[DataExchange.AppSettingsConstants.TapiAsperaBcpRootFolderKey] = "def";
				Assert.That(this.settings.TapiAsperaBcpRootFolder, Is.EqualTo("def"));
				dictionary[DataExchange.AppSettingsConstants.TapiBadPathErrorsRetryKey] = true;
				Assert.That(this.settings.TapiBadPathErrorsRetry, Is.True);
				dictionary[DataExchange.AppSettingsConstants.TapiFileNotFoundErrorsRetryKey] = true;
				Assert.That(this.settings.TapiFileNotFoundErrorsRetry, Is.True);
				dictionary[DataExchange.AppSettingsConstants.TapiFileNotFoundErrorsDisabledKey] = true;
				Assert.That(this.settings.TapiFileNotFoundErrorsDisabled, Is.True);
				dictionary[DataExchange.AppSettingsConstants.TapiForceAsperaClientKey] = true;
				Assert.That(this.settings.TapiForceAsperaClient, Is.True);
				dictionary[DataExchange.AppSettingsConstants.TapiForceBcpHttpClientKey] = true;
				Assert.That(this.settings.TapiForceBcpHttpClient, Is.True);
				dictionary[DataExchange.AppSettingsConstants.TapiForceFileShareClientKey] = true;
				Assert.That(this.settings.TapiForceFileShareClient, Is.True);
				dictionary[DataExchange.AppSettingsConstants.TapiForceHttpClientKey] = true;
				Assert.That(this.settings.TapiForceHttpClient, Is.True);
				dictionary[DataExchange.AppSettingsConstants.TapiForceClientCandidatesKey] = "ghi";
				Assert.That(this.settings.TapiForceClientCandidates, Is.EqualTo("ghi"));
				dictionary[DataExchange.AppSettingsConstants.TapiLargeFileProgressEnabledKey] = true;
				Assert.That(this.settings.TapiLargeFileProgressEnabled, Is.True);
				dictionary[DataExchange.AppSettingsConstants.TapiMaxJobParallelismKey] = 17;
				Assert.That(this.settings.TapiMaxJobParallelism, Is.EqualTo(17));
				dictionary[DataExchange.AppSettingsConstants.TapiMinDataRateMbpsKey] = 18;
				Assert.That(this.settings.TapiMinDataRateMbps, Is.EqualTo(18));
				dictionary[DataExchange.AppSettingsConstants.TapiSubmitApmMetricsKey] = true;
				Assert.That(this.settings.TapiSubmitApmMetrics, Is.True);
				dictionary[DataExchange.AppSettingsConstants.TapiTargetDataRateMbpsKey] = 19;
				Assert.That(this.settings.TapiTargetDataRateMbps, Is.EqualTo(19));
				dictionary[DataExchange.AppSettingsConstants.TapiTransferLogDirectoryKey] = "jkl";
				Assert.That(this.settings.TapiTransferLogDirectory, Is.EqualTo("jkl"));
				dictionary[DataExchange.AppSettingsConstants.TempDirectoryKey] = "mno";
				Assert.That(this.settings.TempDirectory, Is.EqualTo("mno"));
				dictionary[DataExchange.AppSettingsConstants.UseOldExportKey] = true;
				Assert.That(this.settings.UseOldExport, Is.True);
				dictionary[DataExchange.AppSettingsConstants.UsePipeliningForNativeAndObjectImportsKey] = true;
				Assert.That(this.settings.UsePipeliningForNativeAndObjectImports, Is.True);
				dictionary[DataExchange.AppSettingsConstants.WaitBeforeReconnectKey] = 20;
				Assert.That(this.settings.WaitBeforeReconnect, Is.EqualTo(20));
				dictionary[DataExchange.AppSettingsConstants.WebApiOperationTimeoutKey] = 21;
				Assert.That(this.settings.WebApiOperationTimeout, Is.EqualTo(21));
				dictionary[DataExchange.AppSettingsConstants.WebApiServiceUrlKey] = "https://relativity.one.com";
				Assert.That(this.settings.WebApiServiceUrl, Is.EqualTo("https://relativity.one.com/"));
				dictionary[DataExchange.AppSettingsConstants.WebBasedFileDownloadChunkSizeKey] = 3000;
				Assert.That(this.settings.WebBasedFileDownloadChunkSize, Is.EqualTo(3000));

				// The kCura.Windows.Process section asserts go here.
				dictionary[DataExchange.AppSettingsConstants.LogAllEventsKey] = true;
				Assert.That(this.settings.LogAllEvents, Is.True);

				// The new Relativity.DataExchange section asserts go here.
				dictionary[DataExchange.AppSettingsConstants.TapiFileNotFoundErrorsDisabledKey] = true;
				Assert.That(this.settings.TapiFileNotFoundErrorsDisabled, Is.True);
				dictionary[DataExchange.AppSettingsConstants.TapiFileNotFoundErrorsRetryKey] = true;
				Assert.That(this.settings.TapiFileNotFoundErrorsRetry, Is.True);
				dictionary[DataExchange.AppSettingsConstants.TapiMaxInactivitySecondsKey] = 999;
				Assert.That(this.settings.TapiMaxInactivitySeconds, Is.EqualTo(999));
				dictionary[DataExchange.AppSettingsConstants.EnforceVersionCompatibilityCheckKey] = true;
				Assert.That(((IAppSettingsInternal)this.settings).EnforceVersionCompatibilityCheck, Is.True);
				dictionary[DataExchange.AppSettingsConstants.FileTypeIdentifyTimeoutSecondsKey] = 999;
				Assert.That(this.settings.FileTypeIdentifyTimeoutSeconds, Is.EqualTo(999));
				dictionary[DataExchange.AppSettingsConstants.OAuth2ImplicitCredentialRedirectUrlKey] = "http://dummy";
				Assert.That(this.settings.OAuth2ImplicitCredentialRedirectUrl, Is.EqualTo("http://dummy"));
				dictionary[DataExchange.AppSettingsConstants.TelemetrySubmitApmMetricsKey] = true;
				Assert.That(this.settings.TelemetrySubmitApmMetrics, Is.True);
				dictionary[DataExchange.AppSettingsConstants.TelemetrySubmitSumMetricsKey] = true;
				Assert.That(this.settings.TelemetrySubmitSumMetrics, Is.True);
				dictionary[DataExchange.AppSettingsConstants.TelemetryMetricsThrottlingSecondsKey] = 99;
				Assert.That(this.settings.TelemetryMetricsThrottlingSeconds, Is.EqualTo(99));
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
			foreach (PropertyInfo prop in DataExchange.AppSettingsManager.GetProperties())
			{
				string key = DataExchange.AppSettingsManager.GetPropertyKey(prop);
				if (!DataExchange.AppSettingsManager.AppSettingAttributes.ContainsKey(key) || prop.SetMethod == null)
				{
					continue;
				}

				AppSettingAttribute attribute = DataExchange.AppSettingsManager.AppSettingAttributes[key];
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
			foreach (PropertyInfo prop in DataExchange.AppSettingsManager.GetProperties())
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