// -----------------------------------------------------------------------------------------------------
// <copyright file="FileLockedImportJobTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents file lock related import tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit.Integration
{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Security.AccessControl;
	using System.Security.Principal;

	using global::NUnit.Framework;

	using Relativity.DataExchange.Import.NUnit.Integration.Dto;
	using Relativity.DataExchange.TestFramework.Import.JobExecutionContext;
	using Relativity.DataExchange.Transfer;
	using Relativity.Testing.Identification;

	[TestFixture]
	[Feature.DataTransfer.ImportApi.Operations.ImportDocuments]
	[Feature.DataTransfer.TransferApi]
	[TestType.Error]
	[TestExecutionCategory.CI]
	public class FileLockedImportJobTests : ImportJobTestBase<NativeImportExecutionContext>
	{
		[IdentifiedTestCase("e41a8da9-c562-467e-a638-5afa05b59224", TapiClient.Direct, false, true)]
		[IdentifiedTestCase("4bfa1999-7ba2-4c66-bd07-cfb9dbe56b23", TapiClient.Direct, true, true)]
		[IdentifiedTestCase("1b7e9bbc-eb2e-4eae-9d07-11a03778fa7d", TapiClient.Web, false, true)]
		[IdentifiedTestCase("267f10e9-418e-4ec7-83ca-19dadf604531", TapiClient.Web, true, true)]
		[IdentifiedTestCase("269aca91-7acd-42bd-96f2-2422bf9f4c4b", TapiClient.Aspera, false, true)]
		[IdentifiedTestCase("f3661eb4-ca4a-4b36-b0c5-0042780c184a", TapiClient.Aspera, true, true)]
		public void ShouldFailWhenTheFileIsLocked(TapiClient client, bool disableNativeLocationValidation, bool disableNativeValidation)
		{
			// ARRANGE
			kCura.WinEDDS.Config.ConfigSettings["DisableNativeLocationValidation"] = disableNativeLocationValidation;
			kCura.WinEDDS.Config.ConfigSettings["DisableNativeValidation"] = disableNativeValidation;

			AppSettings.Instance.IoErrorNumberOfRetries = 3;
			AppSettings.Instance.IoErrorWaitTimeInSeconds = 10;

			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, NativeImportSettingsProvider.GetFileCopySettings((int)ArtifactType.Document));

			const int NumberOfFilesToImport = 5;
			IEnumerable<DefaultImportDto> importData = DefaultImportDto.GetRandomTextFiles(this.TempDirectory.Directory, NumberOfFilesToImport)
				.Select(SetReadOnlyFlag)
				.Select(DenyAccessToFile);

			// ACT
			ImportTestJobResult results = this.JobExecutionContext.Execute(importData);

			this.ThenTheImportJobFailedWithFatalError(results, 0, NumberOfFilesToImport);

			// ASSERT
			// The exact value is impossible to predict.
			Assert.That(results.NumberOfCompletedRows, Is.GreaterThan(0));
			Assert.That(results.NumberOfJobMessages, Is.GreaterThan(0));
			ThenTheJobCompletedInCorrectTransferMode(results, client);
		}

		[TearDown]
		public void TeardownFileLockedImportJobTests()
		{
			string[] files = Directory.GetFiles(this.TempDirectory.Directory, "*");
			foreach (var file in files)
			{
				RestoreFileAccess(file);
			}
		}

		protected static void DenyFileAccess(string path)
		{
			FileSecurity accessControl = File.GetAccessControl(path);
			var sid = new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null);
			accessControl.AddAccessRule(
				new FileSystemAccessRule(
					sid,
					FileSystemRights.Read | FileSystemRights.Write,
					AccessControlType.Deny));
			File.SetAccessControl(path, accessControl);
		}

		protected static void RestoreFileAccess(string path)
		{
			FileSecurity accessControl = File.GetAccessControl(path);
			var sid = new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null);
			accessControl.RemoveAccessRule(
				new FileSystemAccessRule(
					sid,
					FileSystemRights.Read | FileSystemRights.Write,
					AccessControlType.Deny));
			File.SetAccessControl(path, accessControl);
		}

		private static DefaultImportDto SetReadOnlyFlag(DefaultImportDto p, int i)
		{
			if (i % 2 == 0)
			{
				var fileAttributes = File.GetAttributes(p.FilePath);
				File.SetAttributes(p.FilePath, fileAttributes | FileAttributes.ReadOnly);
			}

			return p;
		}

		private static DefaultImportDto DenyAccessToFile(DefaultImportDto p, int i)
		{
			if (i == 2)
			{
				DenyFileAccess(p.FilePath);
			}

			return p;
		}
	}
}