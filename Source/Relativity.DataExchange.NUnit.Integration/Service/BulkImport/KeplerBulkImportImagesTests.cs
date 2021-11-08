// ----------------------------------------------------------------------------
// <copyright file="KeplerBulkImportImagesTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit.Integration.Service.BulkImport
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using System.Web.Services.Protocols;

	using global::NUnit.Framework;
	using global::NUnit.Framework.Constraints;

	using kCura.WinEDDS.Service;
	using kCura.WinEDDS.Service.Replacement;

	using Relativity.DataExchange.TestFramework.NUnitExtensions;
	using Relativity.DataExchange.TestFramework.RelativityHelpers;
	using Relativity.DataExchange.TestFramework.RelativityVersions;
	using Relativity.Testing.Identification;

	[TestFixture(true)]
	[TestFixture(false)]
	[Feature.DataTransfer.ImportApi.Operations.ImportDocuments]
	[IgnoreIfVersionLowerThan(RelativityVersion.Indigo)] // No IFileSystemManager in older versions
	public class KeplerBulkImportImagesTests : KeplerBulkImportManagerBase
	{
		public KeplerBulkImportImagesTests(bool useKepler)
			: base(useKepler)
		{
		}

		[IdentifiedTest("C888DAC1-38F2-4E06-A4F5-48F4BA1FF4AC")]
		[IgnoreIfVersionLowerThan(RelativityVersion.Lanceleaf)]
		public async Task ShouldImportImages()
		{
			// arrange
			using (IBulkImportManager sut = ManagerFactory.CreateBulkImportManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				var fieldsInfo = this.GetFieldsForImages(NumberOfElements);
				var loadInfo = await this.GetImageLoadInfoAsync(fieldsInfo).ConfigureAwait(false);

				// act
				var result = sut.BulkImportImage(this.TestParameters.WorkspaceId, loadInfo, inRepository: true);
				var hasErrors = sut.ImageRunHasErrors(this.TestParameters.WorkspaceId, loadInfo.RunID);

				// act & assert
				Assert.That(
					() => sut.GenerateImageErrorFiles(
						this.TestParameters.WorkspaceId,
						loadInfo.RunID,
						writeHeader: true,
						keyFieldID: loadInfo.KeyFieldArtifactID),
					this.GetExpectedException(
						"GenerateImageErrorFilesAsync",
						"SQL Statement Failed",
						"kCura.Data.RowDataGateway.ExecuteSQLStatementFailedException"));

				// assert
				Assert.That(result.ExceptionDetail, Is.Null, $"An error occurred when running import: {result.ExceptionDetail?.ExceptionMessage}");
				Assert.That(result.ArtifactsCreated, Is.EqualTo(NumberOfElements));
				Assert.That(result.ArtifactsUpdated, Is.EqualTo(0));
				Assert.That(result.FilesProcessed, Is.EqualTo(NumberOfElements));

				Assert.That(hasErrors, Is.False);

				await this.AssertDocuments(NumberOfElements, fieldsInfo).ConfigureAwait(false);

				// act
				var disposeResult = sut.DisposeTempTables(this.TestParameters.WorkspaceId, loadInfo.RunID);
				Assert.That(disposeResult, Is.Null);
			}
		}

		[IdentifiedTest("12EDAB8F-ED04-4372-B310-E6E9EE368181")]
		public async Task ShouldThrowWhenInvalidWorkspace()
		{
			// arrange
			using (IBulkImportManager sut = ManagerFactory.CreateBulkImportManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				var fieldsInfo = this.GetFieldsForImages(NumberOfElements);
				var loadInfo = await this.GetImageLoadInfoAsync(fieldsInfo).ConfigureAwait(false);

				// act & assert
				Assert.That(
					() => sut.BulkImportImage(NonExistingWorkspaceId, loadInfo, inRepository: true),
					this.GetExpectedExceptionConstraintForNonExistingWorkspace(NonExistingWorkspaceId));
			}
		}

		[IdentifiedTest("90697834-F013-4831-8B05-32653BAA4DA7")]
		[IgnoreIfVersionLowerThan(RelativityVersion.Lanceleaf)]
		public async Task ShouldImportZeroImagesWhenBulkFileContentIsEmpty()
		{
			// arrange
			using (IBulkImportManager sut = ManagerFactory.CreateBulkImportManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				var fieldsInfo = this.GetFieldsForImages(NumberOfElements);
				var loadInfo = await this.GetImageLoadInfoAsync(fieldsInfo).ConfigureAwait(false);
				loadInfo.BulkFileName =
					await BcpFileHelper.CreateEmptyAsync(this.TestParameters, BcpPath).ConfigureAwait(false);

				// act
				var result = sut.BulkImportImage(this.TestParameters.WorkspaceId, loadInfo, inRepository: true);
				var hasErrors = sut.ImageRunHasErrors(this.TestParameters.WorkspaceId, loadInfo.RunID);

				// assert
				Assert.That(result.ExceptionDetail, Is.Null, $"An error occurred when running import: {result.ExceptionDetail?.ExceptionMessage}");
				Assert.That(result.ArtifactsCreated, Is.EqualTo(0));
				Assert.That(result.ArtifactsUpdated, Is.EqualTo(0));
				Assert.That(result.FilesProcessed, Is.EqualTo(0));

				Assert.That(hasErrors, Is.False);

				await this.AssertDocuments(0, new List<Dictionary<string, string>>(0)).ConfigureAwait(false);

				// act
				var disposeResult = sut.DisposeTempTables(this.TestParameters.WorkspaceId, loadInfo.RunID);
				Assert.That(disposeResult, Is.Null);
			}
		}

		[IdentifiedTest("8F5F286A-0226-4990-BA85-481EAA3BE0A0")]
		public async Task ShouldThrowWhenBulkFileNameIsEmpty()
		{
			// arrange
			using (IBulkImportManager sut = ManagerFactory.CreateBulkImportManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				var fieldsInfo = this.GetFieldsForImages(NumberOfElements);
				var loadInfo = await this.GetImageLoadInfoAsync(fieldsInfo).ConfigureAwait(false);
				loadInfo.BulkFileName = string.Empty;

				// act & assert
				Assert.That(
					() => sut.BulkImportImage(this.TestParameters.WorkspaceId, loadInfo, inRepository: true),
					Throws.Exception.TypeOf<BulkImportManager.BulkImportSqlException>().And.Message
						.StartWith("Error: SQL Statement Failed").And.Message.Contains(
							$"Error: Cannot bulk load because the file \"{BcpPath}\" could not be opened. Operating system error code 123(The filename, directory name, or volume label syntax is incorrect.).")
						.Or.Message.Contains("Error: Cannot find the object"));
			}
		}

		[IdentifiedTest("BFC1FC46-7A53-4850-9C8B-4F70CEF95A7F")]
		public async Task ShouldThrowWhenKeyFieldArtifactIdIsInvalid()
		{
			// arrange
			using (IBulkImportManager sut = ManagerFactory.CreateBulkImportManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				var fieldsInfo = this.GetFieldsForImages(NumberOfElements);
				var loadInfo = await this.GetImageLoadInfoAsync(fieldsInfo).ConfigureAwait(false);
				loadInfo.KeyFieldArtifactID = NonExistingArtifactId;

				// act & assert
				Assert.That(
					() => sut.BulkImportImage(this.TestParameters.WorkspaceId, loadInfo, inRepository: true),
					Throws.Exception.TypeOf<BulkImportManager.BulkImportSqlException>().And.Message
						.StartWith("Error: Object reference not set to an instance of an object.").Or.Message
						.StartWith("Error: SQL Statement Failed"));
			}
		}

		private EqualConstraint GetExpectedException(string method, string message, string exceptionType)
		{
			if (this.UseKepler)
			{
				message =
					$"Error during call {method}. InnerExceptionType: {exceptionType}, InnerExceptionMessage: {message}";
			}

			return Throws.Exception.InstanceOf<SoapException>().With.Message.EqualTo(message);
		}
	}
}