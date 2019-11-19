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

	using kCura.Relativity.DataReaderClient;
	using kCura.Relativity.ImportAPI;

	public static class ImportHelper
	{
		public static void ImportDefaultTestData(ImportAPI importApi, int workspaceId)
		{
			if (importApi == null)
			{
				throw new ArgumentNullException(nameof(importApi));
			}

			ImportBulkArtifactJob job = importApi.NewNativeDocumentImportJob();
			Settings settings = job.Settings;
			settings.ArtifactTypeId = WellKnownArtifactTypes.DocumentArtifactTypeId;
			settings.Billable = false;
			settings.BulkLoadFileFieldDelimiter = ";";
			settings.CaseArtifactId = workspaceId;
			settings.CopyFilesToDocumentRepository = true;
			settings.DisableControlNumberCompatibilityMode = true;
			settings.DisableExtractedTextFileLocationValidation = false;
			settings.DisableNativeLocationValidation = false;
			settings.DisableNativeValidation = false;
			settings.ExtractedTextEncoding = System.Text.Encoding.Unicode;
			settings.ExtractedTextFieldContainsFilePath = false;
			settings.FileSizeColumn = WellKnownFields.NativeFileSize;
			settings.FileSizeMapped = true;
			settings.FolderPathSourceFieldName = WellKnownFields.FolderName;
			settings.IdentityFieldId = WellKnownFields.ControlNumberId;
			settings.LoadImportedFullTextFromServer = false;
			settings.MaximumErrorCount = int.MaxValue - 1;
			settings.MoveDocumentsInAppendOverlayMode = false;
			settings.NativeFileCopyMode = NativeFileCopyModeEnum.CopyFiles;
			settings.NativeFilePathSourceFieldName = WellKnownFields.FilePath;
			settings.OIFileIdColumnName = WellKnownFields.OutsideInFileId;
			settings.OIFileIdMapped = true;
			settings.OIFileTypeColumnName = WellKnownFields.OutsideInFileType;
			settings.OverwriteMode = OverwriteModeEnum.Append;
			settings.SelectedIdentifierFieldName = WellKnownFields.ControlNumber;
			settings.StartRecordNumber = 0;
			using (var dataSource = new DataTable("Input Data"))
			{
				dataSource.Locale = CultureInfo.InvariantCulture;
				dataSource.Columns.Add(WellKnownFields.ControlNumber);
				dataSource.Columns.Add(WellKnownFields.FilePath);
				dataSource.Columns.Add(WellKnownFields.FolderName);
				foreach (var file in TestData.SampleFiles)
				{
					DataRow dr = dataSource.NewRow();
					dr[WellKnownFields.ControlNumber] = "REL-" + Guid.NewGuid();
					dr[WellKnownFields.FilePath] = file;
					dr[WellKnownFields.FolderName] = null;
					dataSource.Rows.Add(dr);
				}

				job.SourceData.SourceData = dataSource.CreateDataReader();
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

				job.Execute();
			}
		}
	}
}
