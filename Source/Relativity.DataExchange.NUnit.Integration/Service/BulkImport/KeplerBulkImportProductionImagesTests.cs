// ----------------------------------------------------------------------------
// <copyright file="KeplerBulkImportProductionImagesTests.cs" company="Relativity ODA LLC">
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
	public class KeplerBulkImportProductionImagesTests : KeplerBulkImportManagerBase
	{
		private int productionId;

		public KeplerBulkImportProductionImagesTests(bool useKepler)
			: base(useKepler)
		{
		}

		[SetUp]
		public new async Task SetupAsync()
		{
			productionId = await ProductionHelper.CreateProductionAsync(this.TestParameters, $"Prod-{Guid.NewGuid()}", "PROD").ConfigureAwait(false);
		}

		[TearDown]
		public new async Task TearDownAsync()
		{
			await ProductionHelper.DeleteProductionAsync(this.TestParameters, this.productionId).ConfigureAwait(false);
		}

		[IdentifiedTest("958BCC3C-B5C9-48B1-B8D4-CC5B9E4FEACE")]
		[IgnoreIfVersionLowerThan(RelativityVersion.Lanceleaf)]
		public async Task ShouldImportProductionImages()
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
				var result = sut.BulkImportProductionImage(this.TestParameters.WorkspaceId, loadInfo, inRepository: true, productionKeyFieldArtifactID: this.productionId);
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

		[IdentifiedTest("BACBC978-44B4-42F5-BD6D-2F40335CBE76")]
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
					() => sut.BulkImportProductionImage(NonExistingWorkspaceId, loadInfo, inRepository: true, productionKeyFieldArtifactID: this.productionId),
					this.GetExpectedExceptionConstraintForNonExistingWorkspace(NonExistingWorkspaceId));
			}
		}

		[IdentifiedTest("F09B4303-C201-4A2F-982A-9D8A1AE732DC")]
		public async Task ShouldThrowWhenInvalidProductionId()
		{
			// arrange
			using (IBulkImportManager sut = ManagerFactory.CreateBulkImportManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				var fieldsInfo = this.GetFieldsForImages(NumberOfElements);
				var loadInfo = await this.GetImageLoadInfoAsync(fieldsInfo).ConfigureAwait(false);

				// This is using non standard NonExistingProductionId = 0 because MassImport will create and not remove table [ProductionDocumentFile_0]
				// what will cause other tests to fail https://jira.kcura.com/browse/REL-589346
				// act & assert
				Assert.That(
					() => sut.BulkImportProductionImage(this.TestParameters.WorkspaceId, loadInfo, inRepository: true, productionKeyFieldArtifactID: 111),
					Throws.Exception.InstanceOf<kCura.WinEDDS.Service.BulkImportManager.BulkImportSqlException>()
						.With.Message.Contains("Error: The artifact 111 does not exist.").Or.Message.Contains("Error: Cannot find the object"));

				sut.DisposeTempTables(this.TestParameters.WorkspaceId, loadInfo.RunID);
			}
		}

		[IdentifiedTest("4D54C3FA-E0A9-4641-8EFC-A7BA3D5BAD06")]
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
				var result = sut.BulkImportProductionImage(this.TestParameters.WorkspaceId, loadInfo, inRepository: true, productionKeyFieldArtifactID: this.productionId);
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

		[IdentifiedTest("0F644EA3-A04D-440A-AFD5-B41C24845A4F")]
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
					() => sut.BulkImportProductionImage(this.TestParameters.WorkspaceId, loadInfo, inRepository: true, productionKeyFieldArtifactID: this.productionId),
					Throws.Exception.TypeOf<BulkImportManager.BulkImportSqlException>().And.Message
						.StartWith("Error: SQL Statement Failed"));
			}
		}

		[IdentifiedTest("F1C392E5-1166-49F0-A9F9-24C32D72B413")]
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
					() => sut.BulkImportProductionImage(
						this.TestParameters.WorkspaceId,
						loadInfo,
						inRepository: true,
						productionKeyFieldArtifactID: this.productionId),
					Throws.Exception.TypeOf<BulkImportManager.BulkImportSqlException>().And.Message
						.StartWith("Error: SQL Statement Failed"));
			}
		}

		private ContainsConstraint GetExpectedException(string method, string message, string exceptionType)
		{
			if (this.UseKepler)
			{
				return Throws.Exception.InstanceOf<SoapException>()
					.With.Message.Contains(message)
					.And.Message.Contains($"InnerExceptionType: {exceptionType}")
					.And.Message.Contains("Error during call")
					.And.Message.Contains(method);
			}

			return Throws.Exception.InstanceOf<SoapException>().With.Message.Contains(message);
		}
	}
}