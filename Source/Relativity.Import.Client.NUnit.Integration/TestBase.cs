// -----------------------------------------------------------------------------------------------------
// <copyright file="TestBase.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents an abstract integration test base class.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Import.Client.NUnit.Integration
{
	using System;
	using System.IO;
	using System.Net;
	using System.Security.AccessControl;
	using System.Security.Principal;
	
	using kCura.WinEDDS.TApi;

	using global::NUnit.Framework;

	using Relativity.ImportExport.UnitTestFramework;
	using Relativity.Transfer;

	/// <summary>
	/// Represents an abstract integration test base class.
	/// </summary>
	public abstract class TestBase
	{
		/// <summary>
		/// Gets or sets the temp directory.
		/// </summary>
		/// <value>
		/// The temp directory.
		/// </value>
		protected TempDirectory TempDirectory
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the data transfer test parameters.
		/// </summary>
		/// <value>
		/// The <see cref="DtxTestParameters"/> value.
		/// </value>
		protected DtxTestParameters TestParameters
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets or sets the test timestamp.
		/// </summary>
		/// <value>
		/// The <see cref="DateTime"/> instance.
		/// </value>
		protected DateTime Timestamp
		{
			get;
			private set;
		}

		/// <summary>
		/// The test setup.
		/// </summary>
		[SetUp]
		public void Setup()
		{
			ServicePointManager.SecurityProtocol =
				SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11
				| SecurityProtocolType.Tls12;
			this.AssignTestSettings();
			Assert.That(
				this.TestParameters.WorkspaceId,
				Is.Positive,
				() => "The test workspace must be created or specified in order to run this integration test.");
			this.Timestamp = DateTime.Now;
			this.TempDirectory = new TempDirectory { ClearReadOnlyAttributes = true };
			this.TempDirectory.Create();
			this.OnSetup();
		}

		/// <summary>
		/// The test tear down.
		/// </summary>
		[TearDown]
		public void TearDown()
		{
			this.OnPreTearDown();
			if (this.TempDirectory != null)
			{
				try
				{
					string[] files = System.IO.Directory.GetFiles(this.TempDirectory.Directory, "*");
					foreach (var file in files)
					{
						RestoreFileFullPermissions(file);
					}
				}
				finally
				{
					this.TempDirectory?.Dispose();
					this.TempDirectory = null;
				}
			}
			
			this.OnTearDown();
		}

		/// <summary>
		/// Changes the file full permissions.
		/// </summary>
		/// <param name="path">
		/// The path.
		/// </param>
		/// <param name="grant">
		/// Specify whether to grant or deny access.
		/// </param>
		protected static void ChangeFileFullPermissions(string path, bool grant)
		{
			var accessControl = File.GetAccessControl(path);
			var sid = new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null);
			accessControl.AddAccessRule(
				new FileSystemAccessRule(
					sid,
					FileSystemRights.FullControl,
					grant ? AccessControlType.Allow : AccessControlType.Deny));
			File.SetAccessControl(path, accessControl);
		}

		/// <summary>
		/// Restores the file full permissions.
		/// </summary>
		/// <param name="path">
		/// The path.
		/// </param>
		protected static void RestoreFileFullPermissions(string path)
		{
			var accessControl = File.GetAccessControl(path);
			var sid = new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null);
			foreach (FileSystemAccessRule rule in accessControl.GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount)))
			{
				if (rule.AccessControlType == AccessControlType.Deny)
				{
					accessControl.RemoveAccessRule(rule);
				}
			}

			accessControl.AddAccessRule(
				new FileSystemAccessRule(
					sid,
					FileSystemRights.FullControl,
					AccessControlType.Allow));
			File.SetAccessControl(path, accessControl);
		}

		/// <summary>
		/// Given the standard configuration parameters.
		/// </summary>
		protected static void GivenTheStandardConfigSettings()
		{
			const TapiClient DefaultTapiClient = TapiClient.None;
			const bool DefaultDisableNativeLocationValidation = false;
			const bool DefaultDisableNativeValidation = false;
			GivenTheStandardConfigSettings(
				DefaultTapiClient,
				DefaultDisableNativeLocationValidation,
				DefaultDisableNativeValidation);
		}

		/// <summary>
		/// Given the standard configuration parameters.
		/// </summary>
		/// <param name="forceClient">
		/// Specify which client to force.
		/// </param>
		/// <param name="disableNativeLocationValidation">
		/// Specify whether to disable native location validation.
		/// </param>
		/// <param name="disableNativeValidation">
		/// Specify whether to disable native validation.
		/// </param>
		protected static void GivenTheStandardConfigSettings(
			TapiClient forceClient,
			bool disableNativeLocationValidation,
			bool disableNativeValidation)
		{
			GivenTheBadPathErrorsRetrySetting(false);
			GivenTheForceWebUploadSetting(false);
			GivenThePermissionErrorsRetrySetting(false);
			GivenTheTapiForceAsperaClientSetting(forceClient == TapiClient.Aspera);
			GivenTheTapiForceFileShareClientSetting(forceClient == TapiClient.Direct);
			GivenTheTapiForceHttpClientSetting(forceClient == TapiClient.Web);
			GivenTheTapiMaxJobRetryAttemptsSetting(1);
			GivenTheTapiMaxJobParallelismSetting(1);
			GivenTheTapiLogEnabledSetting(true);
			GivenTheTapiSubmitApmMetricsSetting(false);
			GivenTheIoErrorWaitTimeInSeconds(0);
			GivenTheUsePipeliningForFileIdAndCopySetting(false);
			GivenTheDisableNativeLocationValidationSetting(disableNativeLocationValidation);
			GivenTheDisableNativeValidationSetting(disableNativeValidation);

			// Note: there's no longer a BCP sub-folder.
			GivenTheTapiAsperaBcpRootFolder(string.Empty);
			GivenTheTapiAsperaNativeDocRootLevels(1);
		}

		/// <summary>
		/// Given the bad path errors retry setting.
		/// </summary>
		/// <param name="value">
		/// The path errors retry value.
		/// </param>
		protected static void GivenTheBadPathErrorsRetrySetting(bool value)
		{
			kCura.WinEDDS.Config.ConfigSettings["BadPathErrorsRetry"] = value;
		}

		/// <summary>
		/// Given the import batch size setting.
		/// </summary>
		/// <param name="value">
		/// The import batch size.
		/// </param>
		protected static void GivenTheImportBatchSizeSetting(int value)
		{
			kCura.WinEDDS.Config.ConfigSettings["ImportBatchSize"] = value;
		}

		/// <summary>
		/// Given the force web upload setting.
		/// </summary>
		/// <param name="value">
		/// The setting value.
		/// </param>
		protected static void GivenTheForceWebUploadSetting(bool value)
		{
			kCura.WinEDDS.Config.ConfigSettings["ForceWebUpload"] = value;
		}

		/// <summary>
		/// Given the permission errors retry setting.
		/// </summary>
		/// <param name="value">
		/// The permission errors retry value.
		/// </param>
		protected static void GivenThePermissionErrorsRetrySetting(bool value)
		{
			kCura.WinEDDS.Config.ConfigSettings["PermissionErrorsRetry"] = value;
		}

		/// <summary>
		/// Given the TAPI force Aspera client setting.
		/// </summary>
		/// <param name="value">
		/// The force setting.
		/// </param>
		protected static void GivenTheTapiForceAsperaClientSetting(bool value)
		{
			kCura.WinEDDS.Config.ConfigSettings["TapiForceAsperaClient"] = value.ToString();
		}

		/// <summary>
		/// Given the TAPI force file share client setting.
		/// </summary>
		/// <param name="value">
		/// The force setting.
		/// </param>
		protected static void GivenTheTapiForceFileShareClientSetting(bool value)
		{
			kCura.WinEDDS.Config.ConfigSettings["TapiForceFileShareClient"] = value.ToString();
		}

		/// <summary>
		/// Given the TAPI force HTTP client setting.
		/// </summary>
		/// <param name="value">
		/// The force setting.
		/// </param>
		protected static void GivenTheTapiForceHttpClientSetting(bool value)
		{
			kCura.WinEDDS.Config.ConfigSettings["TapiForceHttpClient"] = value.ToString();
		}

		/// <summary>
		/// Given the TAPI max job parallelism setting.
		/// </summary>
		/// <param name="value">
		/// The setting value.
		/// </param>
		protected static void GivenTheTapiMaxJobParallelismSetting(int value)
		{
			kCura.WinEDDS.Config.ConfigSettings["TapiMaxJobParallelism"] = value;
		}

		/// <summary>
		/// Given the TAPI log enabled setting.
		/// </summary>
		/// <param name="value">
		/// The setting value.
		/// </param>
		protected static void GivenTheTapiLogEnabledSetting(bool value)
		{
			kCura.WinEDDS.Config.ConfigSettings["TapiLogEnabled"] = value;
		}

		/// <summary>
		/// Given the TAPI Aspera native files doc-root levels.
		/// </summary>
		/// <param name="value">
		/// The setting value.
		/// </param>
		protected static void GivenTheTapiAsperaNativeDocRootLevels(int value)
		{
			kCura.WinEDDS.Config.ConfigSettings["TapiAsperaNativeDocRootLevels"] = value;
		}

		/// <summary>
		/// Given the TAPI Aspera BCP root folder.
		/// </summary>
		/// <param name="value">
		/// The setting value.
		/// </param>
		protected static void GivenTheTapiAsperaBcpRootFolder(string value)
		{
			kCura.WinEDDS.Config.ConfigSettings["TapiAsperaBcpRootFolder"] = value;
		}

		/// <summary>
		/// Given the TAPI max job retry attempts setting.
		/// </summary>
		/// <param name="value">
		/// The setting value.
		/// </param>
		protected static void GivenTheTapiMaxJobRetryAttemptsSetting(int value)
		{
			kCura.WinEDDS.Config.ConfigSettings["TapiMaxJobRetryAttempts"] = value;
		}

		/// <summary>
		/// Given the TAPI submit APM setting.
		/// </summary>
		/// <param name="value">
		/// The setting value.
		/// </param>
		protected static void GivenTheTapiSubmitApmMetricsSetting(bool value)
		{
			kCura.WinEDDS.Config.ConfigSettings["TapiSubmitApmMetrics"] = value;
		}

		/// <summary>
		/// Given the disable native location validation setting.
		/// </summary>
		/// <param name="value">
		/// The setting value.
		/// </param>
		protected static void GivenTheDisableNativeLocationValidationSetting(bool value)
		{
			kCura.WinEDDS.Config.ConfigSettings["DisableNativeLocationValidation"] = value;
		}

		/// <summary>
		/// Given the disable native validation setting.
		/// </summary>
		/// <param name="value">
		/// The setting value.
		/// </param>
		protected static void GivenTheDisableNativeValidationSetting(bool value)
		{
			kCura.WinEDDS.Config.ConfigSettings["DisableNativeValidation"] = value;
		}

		/// <summary>
		/// Given the pipelining for file id and copy setting.
		/// </summary>
		/// <param name="value">
		/// The setting value.
		/// </param>
		/// <remarks>
		/// This is the key configuration setting that has created instability with negative test cases.
		/// </remarks>
		protected static void GivenTheUsePipeliningForFileIdAndCopySetting(bool value)
		{
			kCura.WinEDDS.Config.ConfigSettings["UsePipeliningForFileIdAndCopy"] = value;
		}

		/// <summary>
		/// Given the time to wait when an I/O error occurs setting.
		/// </summary>
		/// <param name="value">
		/// The setting value.
		/// </param>
		protected static void GivenTheIoErrorWaitTimeInSeconds(int value)
		{
			kCura.Utility.Config.ConfigSettings["IOErrorWaitTimeInSeconds"] = value;
			if (value == 0)
			{
				kCura.Utility.Config.ConfigSettings["IOErrorNumberOfRetries"] = 0;
			}
		}

		/// <summary>
		/// Assign the test parameters. This should always be called from methods with <see cref="SetUpAttribute"/> or <see cref="OneTimeSetUpAttribute"/>.
		/// </summary>
		protected void AssignTestSettings()
		{
			if (this.TestParameters == null)
			{
				this.TestParameters = AssemblySetup.GlobalDtxTestParameters.DeepCopy();
			}
		}

		/// <summary>
		/// Called when the test setup occurs.
		/// </summary>
		protected virtual void OnSetup()
		{
		}

		/// <summary>
		/// Called before the test tear down occurs.
		/// </summary>
		protected virtual void OnPreTearDown()
		{
		}

		/// <summary>
		/// Called when the test tear down occurs.
		/// </summary>
		protected virtual void OnTearDown()
		{
		}
	}
}