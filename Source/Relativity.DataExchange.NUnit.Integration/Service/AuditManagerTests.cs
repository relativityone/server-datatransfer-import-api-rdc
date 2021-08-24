// ----------------------------------------------------------------------------
// <copyright file="AuditManagerTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit.Integration.Service
{
	using System;
	using System.Web.Services.Protocols;

	using global::NUnit.Framework;
	using global::NUnit.Framework.Constraints;

	using kCura.EDDS.WebAPI.AuditManagerBase;
	using kCura.WinEDDS.Service;
	using kCura.WinEDDS.Service.Replacement;

	using Relativity.DataExchange.TestFramework;
	using Relativity.Testing.Identification;

	[TestFixture(true)]
	[TestFixture(false)]
	[Feature.DataTransfer.ImportApi]
	public class AuditManagerTests : KeplerServiceTestBase
	{
		public AuditManagerTests(bool useKepler)
			: base(useKepler)
		{
		}

		[IdentifiedTest("69dd3fa0-eaee-4684-9b5e-ec9354d185b5")]
		public void ShouldAuditImageImport()
		{
			// Arrange
			const bool IsFatalError = false;
			string runId = Guid.NewGuid().ToString();

			ImageImportStatistics statistics = new ImageImportStatistics
			{
				BatchSizes = new int[0],
				DestinationFolderArtifactID = WorkspaceRootFolderId,
				DestinationProductionArtifactID = 0,
				ExtractedTextDefaultEncodingCodePageID = 0,
				ExtractedTextReplaced = false,
				FilesCopiedToRepository = @"\emttest\DefaultFileRepository",
				LoadFileName = "loadFile.opt",
				NumberOfDocumentsCreated = 10,
				NumberOfDocumentsUpdated = 0,
				NumberOfErrors = 0,
				NumberOfFilesLoaded = 20,
				NumberOfWarnings = 0,
				OverlayBehavior = null,
				OverlayIdentifierFieldArtifactID = WellKnownFields.ControlNumberId,
				Overwrite = OverwriteType.Append,
				RepositoryConnection = RepositoryConnectionType.Web,
				RunTimeInMilliseconds = 1_000,
				SendNotification = false,
				TotalFileSize = 1_000,
				TotalMetadataBytes = 1_000,
			};

			using (IAuditManager sut = ManagerFactory.CreateAuditManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// Act
				bool notificationSent = sut.AuditImageImport(this.TestParameters.WorkspaceId, runId, IsFatalError, statistics);

				// Assert
				Assert.False(notificationSent, "Should not send the notification.");
			}
		}

		[IdentifiedTest("d96c218f-13ff-41cf-beb6-97c2d7844754")]
		public void ShouldThrowExceptionWhenCallingAuditImageImportWithInvalidWorkspaceId()
		{
			// Arrange
			const int InvalidWorkspaceId = 42;
			const bool IsFatalError = false;
			string runId = Guid.NewGuid().ToString();

			IAuditManager sut = ManagerFactory.CreateAuditManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc);

			// Act & Assert
			Assert.That(() => sut.AuditImageImport(InvalidWorkspaceId, runId, IsFatalError, new ImageImportStatistics()), this.GetExpectedExceptionConstraintForNonExistingWorkspace(InvalidWorkspaceId));
		}

		[IdentifiedTest("613513fb-1828-4895-82d6-2ba072672488")]
		public void ShouldThrowExceptionWhenCallingAuditImageImportWithNullImportStats()
		{
			// Arrange
			const bool IsFatalError = false;
			string runId = Guid.NewGuid().ToString();
			IAuditManager sut = ManagerFactory.CreateAuditManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc);

			// Act & Assert
			Assert.That(() => sut.AuditImageImport(this.TestParameters.WorkspaceId, runId, IsFatalError, null), this.GetExceptionConstraintForNullReferenceException("AuditImageImportAsync"));
		}

		[IdentifiedTest("97869acf-02df-4750-84be-3c1d98ef00f3")]
		public void ShouldThrowExceptionWhenCallingAuditImageImportWithNullRunId()
		{
			// Arrange
			const bool IsFatalError = false;
			IAuditManager sut = ManagerFactory.CreateAuditManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc);

			// Act & Assert
			Assert.That(() => sut.AuditImageImport(this.TestParameters.WorkspaceId, null, IsFatalError, new ImageImportStatistics()), this.GetExceptionConstraintForNullReferenceException("AuditImageImportAsync"));
		}

		[IdentifiedTest("7dae96af-28c8-4764-963e-084f4389daa0")]
		public void ShouldAuditObjectImport()
		{
			// Arrange
			const bool IsFatalError = false;
			string runId = Guid.NewGuid().ToString();

			ObjectImportStatistics statistics = new ObjectImportStatistics()
			{
				ArtifactTypeID = 10,
				BatchSizes = new[] { 1_000 },
				Bound = '^',
				Delimiter = '|',
				DestinationFolderArtifactID = WorkspaceRootFolderId,
				ExtractedTextFileEncodingCodePageID = 0,
				ExtractedTextPointsToFile = false,

				// FieldsMapped =
				FileFieldColumnName = string.Empty,
				FilesCopiedToRepository = @"\emttest\DefaultFileRepository",
				FolderColumnName = string.Empty,
				LoadFileEncodingCodePageID = 65001,
				LoadFileName = "loadFile.dat",
				MultiValueDelimiter = ';',
				NestedValueDelimiter = '\\',
				NewlineProxy = '®',
				NumberOfChoicesCreated = 0,
				NumberOfDocumentsCreated = 10,
				NumberOfDocumentsUpdated = 0,
				NumberOfErrors = 0,
				NumberOfFilesLoaded = 0,
				NumberOfWarnings = 0,
				OverlayBehavior = null,
				OverlayIdentifierFieldArtifactID = WellKnownFields.ControlNumberId,
				Overwrite = OverwriteType.Append,
				RepositoryConnection = RepositoryConnectionType.Web,
				RunTimeInMilliseconds = 1_000,
				SendNotification = false,
				StartLine = 0,
				TotalFileSize = 0,
				TotalMetadataBytes = 1_000,
			};

			using (IAuditManager sut = ManagerFactory.CreateAuditManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// Act
				bool notificationSent = sut.AuditObjectImport(this.TestParameters.WorkspaceId, runId, IsFatalError, statistics);

				// Assert
				Assert.False(notificationSent, "Should not send the notification.");
			}
		}

		[IdentifiedTest("bc1f131b-53bb-4af0-84ab-b3e3d3e63776")]
		public void ShouldThrowExceptionWhenCallingAuditObjectImportWithInvalidWorkspaceId()
		{
			// Arrange
			const int InvalidWorkspaceId = 42;
			const bool IsFatalError = false;
			string runId = Guid.NewGuid().ToString();

			IAuditManager sut = ManagerFactory.CreateAuditManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc);

			// Act & Assert
			Assert.That(() => sut.AuditObjectImport(InvalidWorkspaceId, runId, IsFatalError, new ObjectImportStatistics()), this.GetExpectedExceptionConstraintForNonExistingWorkspace(InvalidWorkspaceId));
		}

		[IdentifiedTest("28492990-5a58-463e-abb2-97f32e2ff218")]
		public void ShouldThrowExceptionWhenCallingAuditObjectImportWithNullImportStats()
		{
			// Arrange
			const bool IsFatalError = false;
			string runId = Guid.NewGuid().ToString();
			IAuditManager sut = ManagerFactory.CreateAuditManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc);

			// Act & Assert
			Assert.That(() => sut.AuditObjectImport(this.TestParameters.WorkspaceId, runId, IsFatalError, null), this.GetExceptionConstraintForNullReferenceException("AuditObjectImportAsync"));
		}

		[IdentifiedTest("65fe783f-4173-4009-b23f-540ee2c2dee1")]
		public void ShouldThrowExceptionWhenCallingAuditObjectImportWithNullRunId()
		{
			// Arrange
			const bool IsFatalError = false;
			IAuditManager sut = ManagerFactory.CreateAuditManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc);

			// Act & Assert
			Assert.That(() => sut.AuditObjectImport(this.TestParameters.WorkspaceId, null, IsFatalError, new ObjectImportStatistics()), this.GetExceptionConstraintForNullReferenceException("AuditObjectImportAsync"));
		}

		[IdentifiedTest("012765dc-3900-48df-b42a-e7840ea7c94a")]
		public void ShouldAuditExport()
		{
			// Arrange
			const bool IsFatalError = false;
			ExportStatistics exportStatistics = new ExportStatistics
			{
				AppendOriginalFilenames = false,
				ArtifactTypeID = 10,
				Bound = '^',
				CopyFilesFromRepository = false,
				DataSourceArtifactID = 1038123,
				Delimiter = '|',
				DestinationFilesystemFolder = @"path\to\export\destination\",
				DocumentExportCount = 2,
				ErrorCount = 0,
				ExportImages = false,
				ExportMultipleChoiceFieldsAsNested = true,
				ExportNativeFiles = false,
				ExportSearchablePDFs = false,
				ExportTextFieldAsFiles = false,
				ExportedTextFieldID = 0,
				ExportedTextFileEncodingCodePage = 0,
				Fields = new[] { WellKnownFields.ControlNumberId },
				FileExportCount = 0,
				FilePathSettings = "Use Relative Paths",
				ImageFileType = ImageFileExportType.SinglePage,
				ImageLoadFileFormat = ImageLoadFileFormatType.Opticon,
				ImagesToExport = ImagesToExportType.Original,
				MetadataLoadFileEncodingCodePage = 65001,
				MetadataLoadFileFormat = LoadFileFormat.Dat,
				MultiValueDelimiter = ';',
				NestedValueDelimiter = '\\',
				NewlineProxy = '®',
				OverwriteFiles = false,
				ProductionPrecedence = new[] { -1 },
				RunTimeInMilliseconds = 1_000,
				SourceRootFolderID = WorkspaceRootFolderId,
				StartExportAtDocumentNumber = 1,
				SubdirectoryImagePrefix = "IMG",
				SubdirectoryMaxFileCount = 500,
				SubdirectoryNativePrefix = "NATIVE",
				SubdirectoryPDFPrefix = "PDF",
				SubdirectoryStartNumber = 1,
				SubdirectoryTextPrefix = "TEXT",
				TextAndNativeFilesNamedAfterFieldID = WellKnownFields.ControlNumberId,
				TotalFileBytesExported = 0,
				TotalMetadataBytesExported = 1_000,
				Type = "Folder",
				VolumeMaxSize = 650,
				VolumePrefix = "VOL",
				VolumeStartNumber = 1,
				WarningCount = 0,
			};

			using (IAuditManager sut = ManagerFactory.CreateAuditManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// Act
				bool result = sut.AuditExport(this.TestParameters.WorkspaceId, IsFatalError, exportStatistics);

				// Assert
				Assert.False(result, "Audit Export always returns false.");
			}
		}

		[IdentifiedTest("2d1a8986-00a9-43e7-b23a-4c5e815acdc0")]
		public void ShouldThrowExceptionWhenCallingAuditExportWithInvalidWorkspaceId()
		{
			// Arrange
			const int InvalidWorkspaceId = 42;
			const bool IsFatalError = false;

			IAuditManager sut = ManagerFactory.CreateAuditManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc);

			// Act & Assert
			Assert.That(() => sut.AuditExport(InvalidWorkspaceId, IsFatalError, new ExportStatistics()), this.GetExpectedExceptionConstraintForNonExistingWorkspace(InvalidWorkspaceId));
		}

		[IdentifiedTest("d279ff5c-ebd8-4f00-8121-9cc83c12061f")]
		public void ShouldThrowExceptionWhenCallingAuditExportWithNullExportStats()
		{
			// Arrange
			const bool IsFatalError = false;
			IAuditManager sut = ManagerFactory.CreateAuditManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc);

			// Act & Assert
			Assert.That(() => sut.AuditExport(this.TestParameters.WorkspaceId, IsFatalError, null), this.GetExceptionConstraintForNullReferenceException("AuditExportAsync"));
		}

		[IdentifiedTest("18729c37-06e1-4a79-80ea-82e7fb6781ab")]
		public void ShouldNotThrowExceptionWhenCallingDeleteAuditTokenWithNull()
		{
			// Arrange
			IAuditManager sut = ManagerFactory.CreateAuditManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc);

			// Act & Assert
			Assert.DoesNotThrow(() => sut.DeleteAuditToken(null));
		}

		[IdentifiedTest("2c481b50-6ea3-48cc-ad1f-d6a917b493c8")]
		public void ShouldNotThrowExceptionWhenCallingDeleteAuditTokenWithInvalidGuid()
		{
			// Arrange
			IAuditManager sut = ManagerFactory.CreateAuditManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc);

			// Act & Assert
			Assert.DoesNotThrow(() => sut.DeleteAuditToken("invalid GUID"));
		}

		private IResolveConstraint GetExceptionConstraintForNullReferenceException(string methodName)
		{
			string expectedErrorMessage;
			if (this.UseKepler)
			{
				expectedErrorMessage =
					$"Error during call {methodName}." +
					" InnerExceptionType: System.NullReferenceException," +
					" InnerExceptionMessage: Object reference not set to an instance of an object.";
			}
			else
			{
				expectedErrorMessage = "Object reference not set to an instance of an object.";
			}

			return Throws.Exception.InstanceOf<SoapException>().With.Message.EqualTo(expectedErrorMessage);
		}
	}
}