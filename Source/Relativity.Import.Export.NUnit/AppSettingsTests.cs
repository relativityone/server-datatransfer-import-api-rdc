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
	using System.Runtime.Serialization;
	using System.Runtime.Serialization.Formatters.Binary;

	using global::NUnit.Framework;

	using Relativity.Import.Export;
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
			AppSettings.RegistrySubKeyName = TestRegistrySubKey;
			DeleteTestSubKey();
			AppSettings.Refresh();
			this.settings = new AppSettings();
		}

		[TearDown]
		public void Teardown()
		{
			AppSettings.RegistrySubKeyName = TestRegistrySubKey;
			DeleteTestSubKey();
			this.settings = null;
		}

		[Test]
		public void ShouldGetAndSetTheCreateErrorForInvalidDateSetting()
		{
			// Verify global settings.
			Assert.That(
				AppSettings.CreateErrorForInvalidDate,
				Is.EqualTo(AppSettingsConstants.CreateErrorForInvalidDateDefaultValue));
			AppSettings.CreateErrorForInvalidDate = true;
			Assert.That(AppSettings.CreateErrorForInvalidDate, Is.True);
			AppSettings.CreateErrorForInvalidDate = false;
			Assert.That(AppSettings.CreateErrorForInvalidDate, Is.False);

			// Verify interface settings.
			Assert.That(
				this.settings.CreateErrorForInvalidDate,
				Is.EqualTo(AppSettingsConstants.CreateErrorForInvalidDateDefaultValue));
			this.settings.CreateErrorForInvalidDate = true;
			Assert.That(this.settings.CreateErrorForInvalidDate, Is.True);
			this.settings.CreateErrorForInvalidDate = false;
			Assert.That(this.settings.CreateErrorForInvalidDate, Is.False);
		}

		[Test]
		public void ShouldGetAndSetTheExportErrorNumberOfRetriesSetting()
		{
			// Verify global settings.
			Assert.That(
				AppSettings.ExportErrorNumberOfRetries,
				Is.EqualTo(AppSettingsConstants.ExportErrorNumberOfRetriesDefaultValue));
			int expectedGlobalValue = RandomHelper.NextInt32(10, 1000);
			AppSettings.ExportErrorNumberOfRetries = expectedGlobalValue;
			Assert.That(AppSettings.ExportErrorNumberOfRetries, Is.EqualTo(expectedGlobalValue));

			// Verify interface settings.
			Assert.That(
				this.settings.ExportErrorNumberOfRetries,
				Is.EqualTo(AppSettingsConstants.ExportErrorNumberOfRetriesDefaultValue));
			int expectedInterfaceValue = RandomHelper.NextInt32(10, 1000);
			this.settings.ExportErrorNumberOfRetries = expectedInterfaceValue;
			Assert.That(this.settings.ExportErrorNumberOfRetries, Is.EqualTo(expectedInterfaceValue));
		}

		[Test]
		public void ShouldGetAndSetTheExportErrorWaitTimeInSecondsSetting()
		{
			// Verify global settings.
			Assert.That(
				AppSettings.ExportErrorWaitTimeInSeconds,
				Is.EqualTo(AppSettingsConstants.ExportErrorWaitTimeInSecondsDefaultValue));
			int expectedGlobalValue = RandomHelper.NextInt32(10, 1000);
			AppSettings.ExportErrorWaitTimeInSeconds = expectedGlobalValue;
			Assert.That(AppSettings.ExportErrorWaitTimeInSeconds, Is.EqualTo(expectedGlobalValue));

			// Verify interface settings.
			Assert.That(
				this.settings.ExportErrorWaitTimeInSeconds,
				Is.EqualTo(AppSettingsConstants.ExportErrorWaitTimeInSecondsDefaultValue));
			int expectedInterfaceValue = RandomHelper.NextInt32(10, 1000);
			this.settings.ExportErrorWaitTimeInSeconds = expectedInterfaceValue;
			Assert.That(this.settings.ExportErrorWaitTimeInSeconds, Is.EqualTo(expectedInterfaceValue));
		}

		[Test]
		public void ShouldGetAndSetTheIoErrorNumberOfRetriesSetting()
		{
			// Verify global settings.
			Assert.That(
				AppSettings.IoErrorNumberOfRetries,
				Is.EqualTo(AppSettingsConstants.IoErrorNumberOfRetriesDefaultValue));
			int expectedGlobalValue = RandomHelper.NextInt32(10, 1000);
			AppSettings.IoErrorNumberOfRetries = expectedGlobalValue;
			Assert.That(AppSettings.IoErrorNumberOfRetries, Is.EqualTo(expectedGlobalValue));

			// Verify interface settings.
			Assert.That(
				this.settings.IoErrorNumberOfRetries,
				Is.EqualTo(AppSettingsConstants.IoErrorNumberOfRetriesDefaultValue));
			int expectedInterfaceValue = RandomHelper.NextInt32(10, 1000);
			this.settings.IoErrorNumberOfRetries = expectedInterfaceValue;
			Assert.That(this.settings.IoErrorNumberOfRetries, Is.EqualTo(expectedInterfaceValue));
		}

		[Test]
		public void ShouldGetAndSetTheIoErrorWaitTimeInSecondsSetting()
		{
			// Verify global settings.
			Assert.That(
				AppSettings.IoErrorWaitTimeInSeconds,
				Is.EqualTo(AppSettingsConstants.IoErrorWaitTimeInSecondsDefaultValue));
			int expectedGlobalValue = RandomHelper.NextInt32(10, 1000);
			AppSettings.IoErrorWaitTimeInSeconds = expectedGlobalValue;
			Assert.That(AppSettings.IoErrorWaitTimeInSeconds, Is.EqualTo(expectedGlobalValue));

			// Verify interface settings.
			Assert.That(
				this.settings.IoErrorWaitTimeInSeconds,
				Is.EqualTo(AppSettingsConstants.IoErrorWaitTimeInSecondsDefaultValue));
			int expectedInterfaceValue = RandomHelper.NextInt32(10, 1000);
			this.settings.IoErrorWaitTimeInSeconds = expectedInterfaceValue;
			Assert.That(this.settings.IoErrorWaitTimeInSeconds, Is.EqualTo(expectedInterfaceValue));
		}

		[Test]
		public void ShouldGetAndSetTheForceFolderPreviewSetting()
		{
			// Verify global settings.
			Assert.That(
				AppSettings.ForceFolderPreview,
				Is.EqualTo(AppSettingsConstants.ForceFolderPreviewDefaultValue));
			AppSettings.ForceFolderPreview = true;
			Assert.That(AppSettings.ForceFolderPreview, Is.True);
			AppSettings.ForceFolderPreview = false;
			Assert.That(AppSettings.ForceFolderPreview, Is.False);

			// Verify interface settings.
			DeleteTestSubKey();
			Assert.That(
				this.settings.ForceFolderPreview,
				Is.EqualTo(AppSettingsConstants.ForceFolderPreviewDefaultValue));
			this.settings.ForceFolderPreview = true;
			Assert.That(this.settings.ForceFolderPreview, Is.True);
			this.settings.ForceFolderPreview = false;
			Assert.That(this.settings.ForceFolderPreview, Is.False);
		}

		[Test]
		public void ShouldGetAndSetTheLogAllEventsSetting()
		{
			// Verify global settings.
			Assert.That(
				AppSettings.LogAllEvents,
				Is.EqualTo(AppSettingsConstants.LogAllEventsDefaultValue));
			AppSettings.LogAllEvents = true;
			Assert.That(AppSettings.LogAllEvents, Is.True);
			AppSettings.LogAllEvents = false;
			Assert.That(AppSettings.LogAllEvents, Is.False);

			// Verify interface settings.
			Assert.That(
				this.settings.LogAllEvents,
				Is.EqualTo(AppSettingsConstants.LogAllEventsDefaultValue));
			this.settings.LogAllEvents = true;
			Assert.That(this.settings.LogAllEvents, Is.True);
			this.settings.LogAllEvents = false;
			Assert.That(this.settings.LogAllEvents, Is.False);
		}

		[Test]
		public void ShouldGetAndSetTheMaxNumberOfFileExportTasksSetting()
		{
			// Verify global settings.
			Assert.That(
				AppSettings.MaxNumberOfFileExportTasks,
				Is.EqualTo(AppSettingsConstants.MaxNumberOfFileExportTasksDefaultValue));
			int expectedGlobalValue = RandomHelper.NextInt32(10, 1000);
			AppSettings.MaxNumberOfFileExportTasks = expectedGlobalValue;
			Assert.That(AppSettings.MaxNumberOfFileExportTasks, Is.EqualTo(expectedGlobalValue));

			// Verify interface settings.
			Assert.That(
				this.settings.MaxNumberOfFileExportTasks,
				Is.EqualTo(AppSettingsConstants.MaxNumberOfFileExportTasksDefaultValue));
			int expectedInterfaceValue = RandomHelper.NextInt32(10, 1000);
			this.settings.MaxNumberOfFileExportTasks = expectedInterfaceValue;
			Assert.That(this.settings.MaxNumberOfFileExportTasks, Is.EqualTo(expectedInterfaceValue));
		}

		[Test]
		public void ShouldGetAndSetTheMaximumFilesForTapiBridgeSetting()
		{
			// Verify global settings.
			Assert.That(
				AppSettings.MaximumFilesForTapiBridge,
				Is.EqualTo(AppSettingsConstants.MaximumFilesForTapiBridgeDefaultValue));
			int expectedGlobalValue = RandomHelper.NextInt32(10, 1000);
			AppSettings.MaximumFilesForTapiBridge = expectedGlobalValue;
			Assert.That(AppSettings.MaximumFilesForTapiBridge, Is.EqualTo(expectedGlobalValue));

			// Verify interface settings.
			Assert.That(
				this.settings.MaximumFilesForTapiBridge,
				Is.EqualTo(AppSettingsConstants.MaximumFilesForTapiBridgeDefaultValue));
			int expectedInterfaceValue = RandomHelper.NextInt32(10, 1000);
			this.settings.MaximumFilesForTapiBridge = expectedInterfaceValue;
			Assert.That(this.settings.MaximumFilesForTapiBridge, Is.EqualTo(expectedInterfaceValue));
		}

		[Test]
		public void ShouldGetAndSetTheObjectFieldIdListContainsArtifactIdSetting()
		{
			List<int> testList = new List<int> { 1, 2, 3, 4, 5 };

			// Verify global settings.
			Assert.That(AppSettings.ObjectFieldIdListContainsArtifactId, Is.Empty);
			AppSettings.ObjectFieldIdListContainsArtifactId = testList;
			Assert.That(AppSettings.ObjectFieldIdListContainsArtifactId, Is.EquivalentTo(testList));
			AppSettings.ObjectFieldIdListContainsArtifactId = new List<int>();
			Assert.That(AppSettings.ObjectFieldIdListContainsArtifactId, Is.Empty);

			// Verify interface settings.
			Assert.That(this.settings.ObjectFieldIdListContainsArtifactId, Is.Empty);
			this.settings.ObjectFieldIdListContainsArtifactId = testList;
			Assert.That(this.settings.ObjectFieldIdListContainsArtifactId, Is.EquivalentTo(testList));
			this.settings.ObjectFieldIdListContainsArtifactId = new List<int>();
			Assert.That(this.settings.ObjectFieldIdListContainsArtifactId, Is.Empty);
		}

		[Test]
		public void ShouldGetAndSetTheProgrammaticWebApiServiceUrlSetting()
		{
			// Verify global settings.
			Assert.That(AppSettings.ProgrammaticWebApiServiceUrl, Is.Null);
			AppSettings.ProgrammaticWebApiServiceUrl = new Uri("http://www.cnn.com");
			Assert.That(AppSettings.ProgrammaticWebApiServiceUrl, Is.EqualTo(new Uri("http://www.cnn.com")));
			AppSettings.ProgrammaticWebApiServiceUrl = null;
			Assert.That(AppSettings.ProgrammaticWebApiServiceUrl, Is.Null);

			// Verify interface settings.
			Assert.That(this.settings.ProgrammaticWebApiServiceUrl, Is.Null);
			this.settings.ProgrammaticWebApiServiceUrl = new Uri("http://www.cnn.com");
			Assert.That(this.settings.ProgrammaticWebApiServiceUrl, Is.EqualTo(new Uri("http://www.cnn.com")));
			this.settings.ProgrammaticWebApiServiceUrl = null;
			Assert.That(this.settings.ProgrammaticWebApiServiceUrl, Is.Null);
		}

		[Test]
		public void ShouldGetAndSetTheTapiBridgeExportTransferWaitingTimeInSecondsSetting()
		{
			// Verify global settings.
			Assert.That(
				AppSettings.TapiBridgeExportTransferWaitingTimeInSeconds,
				Is.EqualTo(AppSettingsConstants.TapiBridgeExportTransferWaitingTimeInSecondsDefaultValue));
			int expectedGlobalValue = RandomHelper.NextInt32(10, 1000);
			AppSettings.TapiBridgeExportTransferWaitingTimeInSeconds = expectedGlobalValue;
			Assert.That(AppSettings.TapiBridgeExportTransferWaitingTimeInSeconds, Is.EqualTo(expectedGlobalValue));

			// Verify interface settings.
			Assert.That(
				this.settings.TapiBridgeExportTransferWaitingTimeInSeconds,
				Is.EqualTo(AppSettingsConstants.TapiBridgeExportTransferWaitingTimeInSecondsDefaultValue));
			int expectedInterfaceValue = RandomHelper.NextInt32(10, 1000);
			this.settings.TapiBridgeExportTransferWaitingTimeInSeconds = expectedInterfaceValue;
			Assert.That(this.settings.TapiBridgeExportTransferWaitingTimeInSeconds, Is.EqualTo(expectedInterfaceValue));
		}

		[Test]
		public void ShouldGetAndSetTheWebApiServiceUrlSetting()
		{
			// Verify global settings.
			AppSettings.WebApiServiceUrl = null;
			Assert.That(AppSettings.WebApiServiceUrl, Is.Null);
			AppSettings.ProgrammaticWebApiServiceUrl = new Uri("http://www.cnn.com");
			Assert.That(AppSettings.WebApiServiceUrl, Is.EqualTo(new Uri("http://www.cnn.com")));
			AppSettings.ProgrammaticWebApiServiceUrl = null;
			Assert.That(AppSettings.WebApiServiceUrl, Is.Null);

			// This simulates the scenario where the URL comes from the RDC/Registry.
			AppSettings.SetRegistryKeyValue(AppSettingsConstants.WebApiServiceUrl, "http://www.espn.com");
			Assert.That(AppSettings.WebApiServiceUrl, Is.EqualTo(new Uri("http://www.espn.com")));
			DeleteTestSubKey();
			Assert.That(AppSettings.WebApiServiceUrl, Is.Null);

			// Verify interface settings.
			this.settings.WebApiServiceUrl = null;
			Assert.That(this.settings.WebApiServiceUrl, Is.Null);
			this.settings.ProgrammaticWebApiServiceUrl = new Uri("http://www.cnn.com");
			Assert.That(this.settings.WebApiServiceUrl, Is.EqualTo(new Uri("http://www.cnn.com")));
			this.settings.ProgrammaticWebApiServiceUrl = null;
			Assert.That(this.settings.WebApiServiceUrl, Is.Null);

			// This simulates the scenario where the URL comes from the RDC/Registry.
			AppSettings.SetRegistryKeyValue(AppSettingsConstants.WebApiServiceUrl, "http://www.espn.com");
			Assert.That(this.settings.WebApiServiceUrl, Is.EqualTo(new Uri("http://www.espn.com")));
			DeleteTestSubKey();
			Assert.That(this.settings.WebApiServiceUrl, Is.Null);
		}

		[Test]
		public void ShouldMakeSettingsDeepCopy()
		{
			this.settings.CreateErrorForInvalidDate = RandomHelper.NextBoolean();
			this.settings.ExportErrorNumberOfRetries = RandomHelper.NextInt32(10, 100);
			this.settings.ExportErrorWaitTimeInSeconds = RandomHelper.NextInt32(10, 100);
			this.settings.IoErrorNumberOfRetries = RandomHelper.NextInt32(10, 100);
			this.settings.IoErrorWaitTimeInSeconds = RandomHelper.NextInt32(10, 100);
			this.settings.MaximumFilesForTapiBridge = RandomHelper.NextInt32(10, 100);
			this.settings.MaxNumberOfFileExportTasks = RandomHelper.NextInt32(10, 100);
			this.settings.LogAllEvents = RandomHelper.NextBoolean();
			this.settings.TapiBridgeExportTransferWaitingTimeInSeconds = RandomHelper.NextInt32(10, 100);
			AppSettingsDto copy = this.settings.DeepCopy();
			Assert.That(copy.CreateErrorForInvalidDate, Is.EqualTo(this.settings.CreateErrorForInvalidDate));
			Assert.That(copy.ExportErrorNumberOfRetries, Is.EqualTo(this.settings.ExportErrorNumberOfRetries));
			Assert.That(copy.ExportErrorWaitTimeInSeconds, Is.EqualTo(this.settings.ExportErrorWaitTimeInSeconds));
			Assert.That(copy.IoErrorNumberOfRetries, Is.EqualTo(this.settings.IoErrorNumberOfRetries));
			Assert.That(copy.IoErrorWaitTimeInSeconds, Is.EqualTo(this.settings.IoErrorWaitTimeInSeconds));
			Assert.That(copy.MaximumFilesForTapiBridge, Is.EqualTo(this.settings.MaximumFilesForTapiBridge));
			Assert.That(copy.MaxNumberOfFileExportTasks, Is.EqualTo(this.settings.MaxNumberOfFileExportTasks));
			Assert.That(copy.LogAllEvents, Is.EqualTo(this.settings.LogAllEvents));
			Assert.That(copy.TapiBridgeExportTransferWaitingTimeInSeconds, Is.EqualTo(this.settings.TapiBridgeExportTransferWaitingTimeInSeconds));
		}

		[Test]
		public void ShouldSerializeAndDeserializeTheSettings()
		{
			this.settings.CreateErrorForInvalidDate = true;
			this.settings.ExportErrorNumberOfRetries = int.MaxValue - 1;
			this.settings.ExportErrorWaitTimeInSeconds = int.MaxValue - 2;
			this.settings.IoErrorNumberOfRetries = int.MaxValue - 3;
			this.settings.IoErrorWaitTimeInSeconds = int.MaxValue - 4;
			this.settings.MaximumFilesForTapiBridge = int.MaxValue - 5;
			this.settings.MaxNumberOfFileExportTasks = int.MaxValue - 6;
			this.settings.LogAllEvents = true;
			this.settings.TapiBridgeExportTransferWaitingTimeInSeconds = int.MaxValue - 7;
			IFormatter formatter = new BinaryFormatter();
			using (MemoryStream stream = new MemoryStream())
			{
				formatter.Serialize(stream, this.settings);
				stream.Seek(0, SeekOrigin.Begin);
				IAppSettings deserializedGeneralConfiguration = (IAppSettings)formatter.Deserialize(stream);
				Assert.IsNotNull(deserializedGeneralConfiguration);
				Assert.That(deserializedGeneralConfiguration.CreateErrorForInvalidDate, Is.True);
				Assert.That(deserializedGeneralConfiguration.ExportErrorNumberOfRetries, Is.EqualTo(int.MaxValue - 1));
				Assert.That(deserializedGeneralConfiguration.ExportErrorWaitTimeInSeconds, Is.EqualTo(int.MaxValue - 2));
				Assert.That(deserializedGeneralConfiguration.IoErrorNumberOfRetries, Is.EqualTo(int.MaxValue - 3));
				Assert.That(deserializedGeneralConfiguration.IoErrorWaitTimeInSeconds, Is.EqualTo(int.MaxValue - 4));
				Assert.That(deserializedGeneralConfiguration.MaximumFilesForTapiBridge, Is.EqualTo(int.MaxValue - 5));
				Assert.That(deserializedGeneralConfiguration.MaxNumberOfFileExportTasks, Is.EqualTo(int.MaxValue - 6));
				Assert.That(deserializedGeneralConfiguration.LogAllEvents, Is.True);
				Assert.That(deserializedGeneralConfiguration.TapiBridgeExportTransferWaitingTimeInSeconds, Is.EqualTo(int.MaxValue - 7));
			}
		}

		private static void DeleteTestSubKey()
		{
			const bool ThrowOnMissingSubKey = false;
			Microsoft.Win32.Registry.CurrentUser.DeleteSubKey(TestRegistrySubKey, ThrowOnMissingSubKey);
		}
	}
}