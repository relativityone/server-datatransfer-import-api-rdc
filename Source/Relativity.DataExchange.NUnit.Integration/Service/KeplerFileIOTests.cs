// <copyright file="KeplerFileIOTests.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.NUnit.Integration.Service
{
	using System;
	using System.IO;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Web.Services.Protocols;

	using global::NUnit.Framework;
	using global::NUnit.Framework.Constraints;

	using kCura.WinEDDS.Service;
	using kCura.WinEDDS.Service.Replacement;

	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.RelativityHelpers;
	using Relativity.Kepler.Transport;
	using Relativity.Services.FileSystem;
	using Relativity.Testing.Identification;

	[TestFixture(true)]
	[TestFixture(false)]
	[Feature.DataTransfer.ImportApi]
	public class KeplerFileIOTests : KeplerServiceTestBase
	{
		public KeplerFileIOTests(bool useKepler)
			: base(useKepler)
		{
		}

		[TestType.MainFlow]
		[IdentifiedTest("B2CAC921-27AA-4E51-8DE5-97FCECEFC004")]
		public void ShouldRetrieveBcpSharePath()
		{
			// arrange
			using (IFileIO sut = ManagerFactory.CreateFileIO(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act & assert
				var bcpSharePath = sut.GetBcpSharePath(this.TestParameters.WorkspaceId);
				Assert.That(bcpSharePath, Is.Not.Null.Or.Empty);
			}
		}

		[TestType.Error]
		[IdentifiedTest("0B74AC45-E55E-42EA-9F31-6E60ACD69F73")]
		public void ShouldThrowExceptionWhenRetrieveBcpSharePathForNonExistingWorkspace()
		{
			// arrange
			using (IFileIO sut = ManagerFactory.CreateFileIO(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act & assert
				Assert.That(
					() => sut.GetBcpSharePath(NonExistingWorkspaceId),
					this.GetExpectedExceptionConstraintForGetBcpSharePath(NonExistingWorkspaceId));
			}
		}

		[TestType.MainFlow]
		[IdentifiedTest("DF0CD2CB-EB74-4E27-94B4-F7F1E480D49A")]
		public void ShouldValidateBcpSharePath()
		{
			// arrange
			using (IFileIO sut = ManagerFactory.CreateFileIO(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act & assert
				Assert.That(sut.ValidateBcpShare(this.TestParameters.WorkspaceId), Is.True);
			}
		}

		[TestType.Error]
		[IdentifiedTest("45A51A36-AA14-401F-9941-40771283FE0D")]
		public void ShouldThrowExceptionWhenValidateBcpSharePathForNonExistingWorkspace()
		{
			// arrange
			using (IFileIO sut = ManagerFactory.CreateFileIO(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act & assert
				Assert.That(
					() => sut.ValidateBcpShare(NonExistingWorkspaceId),
					this.GetExpectedExceptionConstraintForNonExistingWorkspace(NonExistingWorkspaceId));
			}
		}

		[TestType.MainFlow]
		[IdentifiedTest("AB55F513-F9FC-429B-AAE6-99294EC4EA65")]
		public async Task ShouldRetrieveRepositoryVolumeMax()
		{
			var instanceSettingValue = await InstanceSettingsHelper.QueryInstanceSetting(
										   this.TestParameters,
										   "Relativity.Core",
										   "RepositoryVolumeMax").ConfigureAwait(false);

			// arrange
			using (IFileIO sut = ManagerFactory.CreateFileIO(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act & assert
				Assert.That(sut.RepositoryVolumeMax().ToString(), Is.EqualTo(instanceSettingValue));
			}
		}

		[TestType.MainFlow]
		[IdentifiedTest("63C6FCD5-702D-417A-AAC9-9CC41FCA3215")]
		public void ShouldRetrieveBcpShareSpaceReport()
		{
			// arrange
			using (IFileIO sut = ManagerFactory.CreateFileIO(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act & assert
				var report = sut.GetBcpShareSpaceReport(this.TestParameters.WorkspaceId);
				Assert.That(report, Is.Not.Empty);
				Assert.That(report.Length, Is.EqualTo(6));
				Assert.That(report[0][0], Is.EqualTo("Drive Name"));
				Assert.That(report[0][1], Is.Not.Empty);
				Assert.That(report[1][0], Is.EqualTo("Free Space"));
				Assert.That(report[1][1], Is.Not.Empty);
				Assert.That(report[2][0], Is.EqualTo("Used Space"));
				Assert.That(report[2][1], Is.Not.Empty);
				Assert.That(report[3][0], Is.EqualTo("Total Space"));
				Assert.That(report[3][1], Is.Not.Empty);
				Assert.That(report[4][0], Is.EqualTo("%Free"));
				Assert.That(report[4][1], Is.Not.Empty);
				Assert.That(report[5][0], Is.EqualTo("%Used"));
				Assert.That(report[5][1], Is.Not.Empty);
			}
		}

		[TestType.Error]
		[IdentifiedTest("0EEA646B-82FC-4138-9F2F-CD3D81373979")]
		public void ShouldThrowExceptionWhenRetrieveBcpShareSpaceReportForNonExistingWorkspace()
		{
			// arrange
			using (IFileIO sut = ManagerFactory.CreateFileIO(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act & assert
				Assert.That(
					() => sut.GetBcpShareSpaceReport(NonExistingWorkspaceId),
					this.GetExpectedExceptionConstraintForNonExistingWorkspace(NonExistingWorkspaceId));
			}
		}

		[TestType.MainFlow]
		[IdentifiedTest("FB4E34B8-7D62-40BD-9D06-F695AB7EF001")]
		public async Task ShouldRemoveTempFile()
		{
			// arrange
			string remoteFileName = Guid.NewGuid().ToString();
			string documentFolder = await WorkspaceHelper.GetDefaultFileRepositoryAsync(this.TestParameters).ConfigureAwait(false);
			string remoteFilePath = Path.Combine(documentFolder, remoteFileName);
			var uploadedFilePath = ResourceFileHelper.GetResourceFilePath("Media", "AZIPPER_0011111.jpg");

			using (var fileSystemManager = ServiceHelper.GetServiceProxy<IFileSystemManager>(this.TestParameters))
			{
				using (var fileStream = File.Open(uploadedFilePath, FileMode.Open, FileAccess.Read))
				using (var keplerStream = new KeplerStream(fileStream))
				{
					await fileSystemManager.UploadFileAsync(keplerStream, remoteFilePath).ConfigureAwait(false);
				}

				var files = await fileSystemManager.ListAsync(documentFolder, recursive: false).ConfigureAwait(false);
				Assert.That(files, Does.Contain(remoteFilePath), $"File {remoteFilePath} should exist");

				using (IFileIO sut = ManagerFactory.CreateFileIO(
					this.Credential,
					this.CookieContainer,
					this.CorrelationIdFunc))
				{
					// act
					sut.RemoveTempFile(this.TestParameters.WorkspaceId, remoteFileName);
				}

				var timeout = DateTime.UtcNow.AddSeconds(60);
				bool fileExist = true;

				while (fileExist)
				{
					if (DateTime.UtcNow > timeout)
					{
						Assert.Fail($"File {remoteFilePath} was not removed in 60 seconds");
					}

					files = await fileSystemManager.ListAsync(documentFolder, recursive: false).ConfigureAwait(false);
					fileExist = files.Contains(remoteFilePath);
					if (fileExist)
					{
						Thread.Sleep(2000);
					}
				}
			}
		}

		[TestType.Error]
		[IdentifiedTest("617617FA-1C75-452D-AE36-D0B45C9BE2BB")]
		public void ShouldThrowExceptionWhenRemoveTempFileForNonExistingWorkspace()
		{
			// arrange
			using (IFileIO sut = ManagerFactory.CreateFileIO(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act & assert
				Assert.That(
					() => sut.RemoveTempFile(NonExistingWorkspaceId, "nonExistingFileGuid"),
					this.GetExpectedExceptionConstraintForNonExistingWorkspace(NonExistingWorkspaceId));
			}
		}

		[TestType.MainFlow]
		[IdentifiedTest("19CBC301-D1B2-4903-9242-534C8D21DD55")]
		public void ShouldRemoveTempFileForNonExistingFile()
		{
			// arrange
			using (IFileIO sut = ManagerFactory.CreateFileIO(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// Service will not throw exception when file does not exist
				// act & assert
				sut.RemoveTempFile(this.TestParameters.WorkspaceId, "nonExistingFileGuid");
			}
		}

		private IResolveConstraint GetExpectedExceptionConstraintForGetBcpSharePath(int workspaceId)
		{
			string expectedExceptionMessage;
			if (this.UseKepler)
			{
				expectedExceptionMessage =
					"Error during call PermissionCheckInterceptor." +
					" InnerExceptionType: Relativity.Core.Exception.InvalidAppArtifactID," +
					$" InnerExceptionMessage: Could not retrieve ApplicationID #{workspaceId}.";
				return Throws.Exception.InstanceOf<SoapException>()
					.With.Message.EqualTo(expectedExceptionMessage);
			}
			else
			{
				expectedExceptionMessage = $"Error: Could not retrieve ApplicationID #{workspaceId}.";
				return Throws.Exception.InstanceOf<FileIO.CustomException>()
					.With.Message.StartWith(expectedExceptionMessage);
			}
		}
	}
}
