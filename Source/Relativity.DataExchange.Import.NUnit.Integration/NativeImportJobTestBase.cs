// -----------------------------------------------------------------------------------------------------
// <copyright file="NativeImportJobTestBase.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents an abstract load-file base class.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit.Integration
{
	using kCura.Relativity.DataReaderClient;

	using Relativity.DataExchange.Import.NUnit.Integration.SetUp;
	using Relativity.DataExchange.TestFramework;

	public abstract class NativeImportJobTestBase : ImportJobTestBase<ImportBulkArtifactJob, Settings>
	{
		protected Settings GetDefaultNativeDocumentImportSettings()
		{
			return new Settings
			{
			   SelectedIdentifierFieldName = WellKnownFields.ControlNumber,
			   OverwriteMode = OverwriteModeEnum.AppendOverlay,
			};
		}

		protected Settings GetNativeFilePathSourceDocumentImportSettings()
		{
			return new Settings
			{
				NativeFilePathSourceFieldName = WellKnownFields.FilePath,
				NativeFileCopyMode = NativeFileCopyModeEnum.CopyFiles,

				OIFileIdMapped = true,
				OIFileIdColumnName = WellKnownFields.OutsideInFileId,
				OIFileTypeColumnName = WellKnownFields.OutsideInFileType,

				FileSizeMapped = true,
				FileSizeColumn = WellKnownFields.NativeFileSize,
			};
		}

		protected override ImportApiSetUp<ImportBulkArtifactJob, Settings> CreateImportApiSetUp()
		{
			return new NativeImportApiSetUp();
		}
	}
}
