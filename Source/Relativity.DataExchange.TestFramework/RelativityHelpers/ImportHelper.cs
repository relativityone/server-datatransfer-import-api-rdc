// ----------------------------------------------------------------------------
// <copyright file="ImportHelper.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework.RelativityHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using kCura.EDDS.WebAPI.BulkImportManagerBase;
    using kCura.Relativity.DataReaderClient;
    using kCura.Relativity.ImportAPI;
    using Relativity.DataExchange.TestFramework.Extensions;

    public static class ImportHelper
	{
		public static void ImportDefaultTestData(ImportAPI importApi, int workspaceId)
		{
			importApi.ThrowIfNull(nameof(importApi));

			ImportBulkArtifactJob job = importApi.NewNativeDocumentImportJob();
			Settings settings = job.Settings;

			settings.CaseArtifactId = workspaceId;
			ApplyDefaultsForDocuments(settings);
			ConfigureJobErrorEvents(job);

			using (var dataSource = new DataTable("Input Data"))
			{
				dataSource.Locale = CultureInfo.InvariantCulture;
				dataSource.WithControlNumber()
					.WithFilePath()
					.WithFolderName();

				foreach (var file in TestData.SampleFiles)
				{
					DataRow dr = dataSource.NewRow()
						.GenerateControlNumber()
						.SetFilePath(file)
						.SetFolderName(null);

					dataSource.Rows.Add(dr);
				}

				job.SourceData.SourceData = dataSource.CreateDataReader();
				job.Execute();
			}
		}

		public static IReadOnlyCollection<string> ImportDocuments(IntegrationTestParameters parameters)
		{
			parameters.ThrowIfNull(nameof(parameters));

			var importApi = IntegrationTestHelper.CreateImportApi(parameters);
			var job = importApi.NewNativeDocumentImportJob();
			var settings = job.Settings;

			settings.CaseArtifactId = parameters.WorkspaceId;
			ApplyDefaultsForDocuments(settings);
			ConfigureJobErrorEvents(job);

			using (var dataSource = new DataTable("Input Data"))
			{
				dataSource.Locale = CultureInfo.InvariantCulture;
				dataSource.WithControlNumber()
					.WithFilePath()
					.WithFolderName()
					.WithExtractedText();

				var controlNumbers = new List<string>();

				foreach (var file in TestData.SampleDocFiles)
				{
					var dr = dataSource.NewRow()
						.GenerateControlNumber()
						.SetFilePath(file)
						.SetFolderName(@"A\B")
						.SetExtractedText($"Example extracted text from file {file}");

					controlNumbers.Add(dr.GetControlNumber());
					dataSource.Rows.Add(dr);
				}

				job.SourceData.SourceData = dataSource.CreateDataReader();
				job.Execute();

				return controlNumbers;
			}
		}

		public static void ImportImagesForDocuments(IntegrationTestParameters parameters, IEnumerable<string> documentsControlNumbers)
		{
			parameters.ThrowIfNull(nameof(parameters));
			documentsControlNumbers.ThrowIfNull(nameof(documentsControlNumbers));

			var importApi = IntegrationTestHelper.CreateImportApi(parameters);
			var job = importApi.NewImageImportJob();
			var settings = job.Settings;

			settings.CaseArtifactId = parameters.WorkspaceId;
			ApplyDefaultsForImageImport(settings);
			ConfigureJobErrorEvents(job);

			using (var dataSource = new DataTable("Input Data"))
			{
				dataSource.Locale = CultureInfo.InvariantCulture;
				dataSource.WithControlNumber()
					.WithFileLocation()
					.WithBatesNumber();

				var images = TestData.SampleImageFiles.ToList();

				foreach (var documentControlNumber in documentsControlNumbers)
				{
					string batesNumber = documentControlNumber;
					int batesNumberSuffix = 1;

					foreach (var image in images)
					{
						var row = dataSource.NewRow()
							.SetControlNumber(documentControlNumber)
							.SetFileLocation(image)
							.SetBatesNumber(batesNumber);

						dataSource.Rows.Add(row);
						batesNumber = $"{documentControlNumber}-{batesNumberSuffix:D2}";
						batesNumberSuffix++;
					}
				}

				job.SourceData.SourceData = dataSource;
				job.Execute();
			}
		}

		public static void ImportProduction(IntegrationTestParameters parameters, string productionName, IEnumerable<string> documentsControlNumbers)
		{
			parameters.ThrowIfNull(nameof(parameters));
			documentsControlNumbers.ThrowIfNull(nameof(documentsControlNumbers));
			productionName.ThrowIfNullOrEmpty(nameof(parameters));

			int productionId = ProductionHelper.CreateProduction(parameters, productionName, "BATES", IntegrationTestHelper.Logger);
			var importApi = IntegrationTestHelper.CreateImportApi(parameters);
			var job = importApi.NewProductionImportJob(productionId);
			var settings = job.Settings;

			settings.CaseArtifactId = parameters.WorkspaceId;
			ApplyDefaultsForProductionImport(settings);
			ConfigureJobErrorEvents(job);

			using (var dataSource = new DataTable("Input Data"))
			{
				dataSource.Locale = CultureInfo.InvariantCulture;
				dataSource.WithControlNumber()
					.WithBatesNumber()
					.WithFileLocation();

				var images = TestData.SampleImageFiles.ToList();
				var docNumber = 1;

				foreach (var documentControlNumber in documentsControlNumbers)
				{
					int batesNumberSuffix = 1;

					foreach (var image in images)
					{
						var row = dataSource.NewRow()
							.SetControlNumber(documentControlNumber)
							.SetBatesNumber($"PROD{docNumber:D4}-{batesNumberSuffix:D4}")
							.SetFileLocation(image);

						dataSource.Rows.Add(row);
						batesNumberSuffix++;
					}

					docNumber++;
				}

				job.SourceData.SourceData = dataSource;
				job.Execute();
			}
		}

		private static void ConfigureJobErrorEvents(IImportNotifier job)
		{
			job.OnFatalException += report => throw report.FatalException;
			job.OnComplete += report =>
			{
				if (report.FatalException != null)
				{
					throw report.FatalException;
				}

				if (report.ErrorRowCount > 0)
				{
					IEnumerable<string> errors = report.ErrorRows.Select(x => $"{x.Identifier} - {x.Message}");
					throw new InvalidOperationException(string.Join("\n", errors));
				}
			};
		}

		private static void ApplyDefaultsForDocuments(Settings settings)
		{
			settings.ArtifactTypeId = WellKnownArtifactTypes.DocumentArtifactTypeId;
			settings.IdentityFieldId = WellKnownFields.ControlNumberId;
			settings.SelectedIdentifierFieldName = WellKnownFields.ControlNumber;
			settings.Billable = false;
			settings.BulkLoadFileFieldDelimiter = ";";
			settings.CopyFilesToDocumentRepository = true;
			settings.DisableControlNumberCompatibilityMode = true;
			settings.DisableExtractedTextFileLocationValidation = false;
			settings.DisableNativeLocationValidation = false;
			settings.DisableNativeValidation = false;
			settings.ExtractedTextEncoding = Encoding.Unicode;
			settings.ExtractedTextFieldContainsFilePath = false;
			settings.FileSizeColumn = WellKnownFields.NativeFileSize;
			settings.FileSizeMapped = true;
			settings.FolderPathSourceFieldName = WellKnownFields.FolderName;
			settings.LoadImportedFullTextFromServer = false;
			settings.MaximumErrorCount = int.MaxValue - 1;
			settings.MoveDocumentsInAppendOverlayMode = false;
			settings.NativeFileCopyMode = NativeFileCopyModeEnum.CopyFiles;
			settings.NativeFilePathSourceFieldName = WellKnownFields.FilePath;
			settings.OIFileIdColumnName = WellKnownFields.OutsideInFileId;
			settings.OIFileIdMapped = true;
			settings.OIFileTypeColumnName = WellKnownFields.OutsideInFileType;
			settings.OverwriteMode = OverwriteModeEnum.Append;
			settings.StartRecordNumber = 0;
		}

		private static void ApplyDefaultsForProductionImport(ImageSettings settings)
		{
			settings.ArtifactTypeId = WellKnownArtifactTypes.DocumentArtifactTypeId;
			settings.IdentityFieldId = WellKnownFields.ControlNumberId;
			settings.SelectedIdentifierFieldName = WellKnownFields.ControlNumber;
			settings.DocumentIdentifierField = WellKnownFields.ControlNumber;
			settings.AutoNumberImages = false;
			settings.BatesNumberField = WellKnownFields.BatesNumber;
			settings.Billable = false;
			settings.CopyFilesToDocumentRepository = true;
			settings.DisableExtractedTextEncodingCheck = false;
			settings.DisableImageLocationValidation = false;
			settings.DisableImageTypeValidation = false;
			settings.DisableUserSecurityCheck = true;
			settings.ExtractedTextEncoding = Encoding.Unicode;
			settings.ExtractedTextFieldContainsFilePath = false;
			settings.FileLocationField = WellKnownFields.FileLocation;
			settings.FolderPathSourceFieldName = null;
			settings.ImageFilePathSourceFieldName = WellKnownFields.FileLocation;
			settings.LoadImportedFullTextFromServer = false;
			settings.MaximumErrorCount = int.MaxValue - 1;
			settings.MoveDocumentsInAppendOverlayMode = false;
			settings.NativeFileCopyMode = NativeFileCopyModeEnum.DoNotImportNativeFiles;
			settings.OverlayBehavior = OverlayBehavior.MergeAll;
			settings.OverwriteMode = OverwriteModeEnum.Append;
			settings.StartRecordNumber = 0;
		}

		private static void ApplyDefaultsForImageImport(ImageSettings settings)
		{
			settings.ArtifactTypeId = WellKnownArtifactTypes.DocumentArtifactTypeId;
			settings.IdentityFieldId = WellKnownFields.ControlNumberId;
			settings.SelectedIdentifierFieldName = WellKnownFields.ControlNumber;
			settings.DocumentIdentifierField = WellKnownFields.ControlNumber;
			settings.AutoNumberImages = false;
			settings.BatesNumberField = WellKnownFields.BatesNumber;
			settings.Billable = false;
			settings.CopyFilesToDocumentRepository = true;
			settings.DisableExtractedTextEncodingCheck = true;
			settings.DisableImageLocationValidation = false;
			settings.DisableImageTypeValidation = false;
			settings.DisableUserSecurityCheck = true;
			settings.ExtractedTextEncoding = Encoding.Unicode;
			settings.ExtractedTextFieldContainsFilePath = false;
			settings.FileLocationField = WellKnownFields.FileLocation;
			settings.FolderPathSourceFieldName = null;
			settings.ImageFilePathSourceFieldName = WellKnownFields.FileLocation;
			settings.LoadImportedFullTextFromServer = false;
			settings.MaximumErrorCount = int.MaxValue - 1;
			settings.MoveDocumentsInAppendOverlayMode = false;
			settings.NativeFileCopyMode = NativeFileCopyModeEnum.CopyFiles;
			settings.OverlayBehavior = OverlayBehavior.MergeAll;
			settings.OverwriteMode = OverwriteModeEnum.AppendOverlay;
			settings.StartRecordNumber = 0;
		}
	}
}
