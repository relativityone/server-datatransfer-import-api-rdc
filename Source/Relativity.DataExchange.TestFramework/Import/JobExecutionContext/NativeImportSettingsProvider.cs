// -----------------------------------------------------------------------------------------------------
// <copyright file="NativeImportSettingsProvider.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents an abstract load-file base class.
// </summary>
// -----------------------------------------------------------------------------------------------------
namespace Relativity.DataExchange.TestFramework.Import.JobExecutionContext
{
	using kCura.Relativity.DataReaderClient;

	using Relativity.DataExchange.TestFramework;

	public static class NativeImportSettingsProvider
	{
		public static Settings GetFileCopySettings(int artifactTypeId)
		{
			Settings settings = GetDefaultSettings(artifactTypeId);
			settings.NativeFilePathSourceFieldName = WellKnownFields.FilePath;
			settings.NativeFileCopyMode = NativeFileCopyModeEnum.CopyFiles;

			settings.OIFileIdMapped = true;
			settings.OIFileIdColumnName = WellKnownFields.OutsideInFileId;
			settings.OIFileTypeColumnName = WellKnownFields.OutsideInFileType;

			settings.FileSizeMapped = true;
			settings.FileSizeColumn = WellKnownFields.NativeFileSize;
			return settings;
		}

		public static Settings GetRsmfSettings()
		{
			Settings settings = GetDefaultSettings();
			settings.MetadataFileIdColumn = WellKnownFields.MetadataFileId;
			return settings;
		}

		public static Settings GetDefaultSettings()
		{
			return GetDefaultSettings((int)ArtifactType.Document, WellKnownFields.ControlNumber);
		}

		public static Settings GetDefaultSettings(int artifactTypeId)
		{
			return GetDefaultSettings(artifactTypeId, WellKnownFields.ControlNumber);
		}

		public static Settings GetDefaultSettings(int artifactTypeId, string selectedIdentifierFieldName)
		{
			Settings settings = new Settings
			{
				SelectedIdentifierFieldName = selectedIdentifierFieldName,
				OverwriteMode = OverwriteModeEnum.Append,
				ArtifactTypeId = artifactTypeId,
			};
			return settings;
		}
	}
}