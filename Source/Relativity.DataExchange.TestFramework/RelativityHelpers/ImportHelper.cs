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
		public static IEnumerable<string> ImportDefaultTestData(IntegrationTestParameters parameters)
		{
			return ImportDocuments(parameters, SetupColumns, SetValues);

			void SetupColumns(DataTable dataTable)
			{
				dataTable.WithFolderName();
			}

			void SetValues(DataRow dataRow, string file)
			{
			}
		}

		public static IEnumerable<string> ImportDocuments(IntegrationTestParameters parameters)
		{
			return ImportDocuments(parameters, SetupColumns, SetValues);

			void SetupColumns(DataTable dataTable)
			{
				dataTable.WithFolderName()
					.WithExtractedText();
			}

			void SetValues(DataRow dataRow, string file)
			{
				dataRow.SetFolderName(@"A\B")
					.SetExtractedText($"Example extracted text from file {file}");
			}
		}

		public static void ImportImagesForDocuments(
			IntegrationTestParameters parameters,
			IEnumerable<string> documentsControlNumbers)
		{
			ImportImagesForDocuments(parameters, GetImportJob, SetData);

			ImageImportBulkArtifactJob GetImportJob(ImportAPI importApi)
			{
				var job = importApi.NewImageImportJob();
				ApplyDefaultsForImageImport(job.Settings);
				return job;
			}

			void SetData(DataTable dataSource)
			{
				var images = TestData.SampleImageFiles.ToList();

				foreach (var documentControlNumber in documentsControlNumbers)
				{
					var batesNumber = documentControlNumber;
					var batesNumberSuffix = 1;

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
			}
		}

		public static void ImportProduction(
			IntegrationTestParameters parameters,
			string productionName,
			IEnumerable<string> documentsControlNumbers)
		{
			var productionId =
				ProductionHelper.CreateProduction(parameters, productionName, "BATES", IntegrationTestHelper.Logger);

			ImportImagesForDocuments(parameters, GetImportJob, SetData);

			ImageImportBulkArtifactJob GetImportJob(ImportAPI importApi)
			{
				var job = importApi.NewProductionImportJob(productionId);
				ApplyDefaultsForProductionImport(job.Settings);
				return job;
			}

			void SetData(DataTable dataSource)
			{
				var images = TestData.SampleImageFiles.ToList();
				var docNumber = 1;

				foreach (var documentControlNumber in documentsControlNumbers)
				{
					var batesNumberSuffix = 1;

					foreach (var image in images)
					{
						var row = dataSource.NewRow()
							.SetControlNumber(documentControlNumber)
							.SetFileLocation(image)
							.SetBatesNumber($"PROD{docNumber:D4}-{batesNumberSuffix:D4}");

						dataSource.Rows.Add(row);
						batesNumberSuffix++;
					}

					docNumber++;
				}
			}
		}

		private static IEnumerable<string> ImportDocuments(
			IntegrationTestParameters parameters,
			Action<DataTable> setupColumns,
			Action<DataRow, string> setValues)
		{
			parameters.ThrowIfNull(nameof(parameters));

			var importApi = CreateImportApi(parameters);
			var job = importApi.NewNativeDocumentImportJob();
			var settings = job.Settings;

			settings.CaseArtifactId = parameters.WorkspaceId;
			ApplyDefaultsForDocuments(settings);
			ConfigureJobErrorEvents(job);

			using (var dataSource = new DataTable("Input Data"))
			{
				dataSource.Locale = CultureInfo.InvariantCulture;
				dataSource.WithControlNumber()
					.WithFilePath();

				setupColumns(dataSource);

				var controlNumbers = new List<string>();

				foreach (var file in TestData.SampleDocFiles)
				{
					var dr = dataSource.NewRow()
						.GenerateControlNumber()
						.SetFilePath(file);

					setValues(dr, file);

					controlNumbers.Add(dr.GetControlNumber());
					dataSource.Rows.Add(dr);
				}

				job.SourceData.SourceData = dataSource.CreateDataReader();
				job.Execute();

				return controlNumbers;
			}
		}

		private static void ImportImagesForDocuments(
			IntegrationTestParameters parameters,
			Func<ImportAPI, ImageImportBulkArtifactJob> getImportJob,
			Action<DataTable> setData)
		{
			var importApi = CreateImportApi(parameters);
			var job = getImportJob(importApi);
			job.Settings.CaseArtifactId = parameters.WorkspaceId;
			ConfigureJobErrorEvents(job);

			using (var dataSource = new DataTable("Input Data"))
			{
				dataSource.Locale = CultureInfo.InvariantCulture;
				dataSource.WithControlNumber()
					.WithFileLocation()
					.WithBatesNumber();

				setData(dataSource);

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
					var errors = report.ErrorRows.Select(x => $"{x.Identifier} - {x.Message}");
					throw new InvalidOperationException(string.Join("\n", errors));
				}
			};
		}

		private static ImportAPI CreateImportApi(IntegrationTestParameters parameters)
		{
			return new ImportAPI(
				parameters.RelativityUserName,
				parameters.RelativityPassword,
				parameters.RelativityWebApiUrl.ToString());
		}

		private static void ApplyDefaultsForDocuments(Settings settings)
		{
			ApplyDefaultBaseSettings(settings);
			settings.ArtifactTypeId = WellKnownArtifactTypes.DocumentArtifactTypeId;
			settings.BulkLoadFileFieldDelimiter = ";";
			settings.DisableControlNumberCompatibilityMode = true;
			settings.DisableExtractedTextFileLocationValidation = false;
			settings.DisableNativeLocationValidation = false;
			settings.DisableNativeValidation = false;
			settings.FileSizeColumn = WellKnownFields.NativeFileSize;
			settings.FileSizeMapped = true;
			settings.FolderPathSourceFieldName = WellKnownFields.FolderName;
			settings.NativeFileCopyMode = NativeFileCopyModeEnum.CopyFiles;
			settings.NativeFilePathSourceFieldName = WellKnownFields.FilePath;
			settings.OIFileIdColumnName = WellKnownFields.OutsideInFileId;
			settings.OIFileIdMapped = true;
			settings.OIFileTypeColumnName = WellKnownFields.OutsideInFileType;
			settings.OverwriteMode = OverwriteModeEnum.Append;
		}

		private static void ApplyDefaultsForProductionImport(ImageSettings settings)
		{
			ApplyDefaultBaseSettings(settings);
			ApplyDefaultImageSettings(settings);
			settings.OverwriteMode = OverwriteModeEnum.Append;
		}

		private static void ApplyDefaultsForImageImport(ImageSettings settings)
		{
			ApplyDefaultBaseSettings(settings);
			ApplyDefaultImageSettings(settings);
			settings.OverwriteMode = OverwriteModeEnum.AppendOverlay;
		}

		private static void ApplyDefaultImageSettings(ImageSettings settings)
		{
			settings.ArtifactTypeId = WellKnownArtifactTypes.DocumentArtifactTypeId;
			settings.DocumentIdentifierField = WellKnownFields.ControlNumber;
			settings.AutoNumberImages = false;
			settings.BatesNumberField = WellKnownFields.BatesNumber;
			settings.DisableExtractedTextEncodingCheck = false;
			settings.DisableImageLocationValidation = false;
			settings.DisableImageTypeValidation = false;
			settings.DisableUserSecurityCheck = true;
			settings.FileLocationField = WellKnownFields.FileLocation;
			settings.FolderPathSourceFieldName = null;
			settings.ImageFilePathSourceFieldName = WellKnownFields.FileLocation;
			settings.OverlayBehavior = OverlayBehavior.MergeAll;
			settings.NativeFileCopyMode = NativeFileCopyModeEnum.DoNotImportNativeFiles;
		}

		private static void ApplyDefaultBaseSettings(ImportSettingsBase settings)
		{
			settings.IdentityFieldId = WellKnownFields.ControlNumberId;
			settings.SelectedIdentifierFieldName = WellKnownFields.ControlNumber;
			settings.Billable = false;
			settings.CopyFilesToDocumentRepository = true;
			settings.ExtractedTextEncoding = Encoding.Unicode;
			settings.ExtractedTextFieldContainsFilePath = false;
			settings.LoadImportedFullTextFromServer = false;
			settings.MaximumErrorCount = int.MaxValue - 1;
			settings.MoveDocumentsInAppendOverlayMode = false;
			settings.StartRecordNumber = 0;
		}
	}
}