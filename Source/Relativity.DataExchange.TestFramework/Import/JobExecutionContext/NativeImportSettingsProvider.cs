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
		public static Settings FileCopySettings
		{
			get
			{
				Settings settings = DefaultSettings();
				settings.NativeFilePathSourceFieldName = WellKnownFields.FilePath;
				settings.NativeFileCopyMode = NativeFileCopyModeEnum.CopyFiles;

				settings.OIFileIdMapped = true;
				settings.OIFileIdColumnName = WellKnownFields.OutsideInFileId;
				settings.OIFileTypeColumnName = WellKnownFields.OutsideInFileType;

				settings.FileSizeMapped = true;
				settings.FileSizeColumn = WellKnownFields.NativeFileSize;
				return settings;
			}
		}

		public static Settings DefaultSettings()
		{
			return DefaultSettings((int)ArtifactType.Document, WellKnownFields.ControlNumber);
		}

		public static Settings DefaultSettings(int artifactTypeId)
		{
			return DefaultSettings(artifactTypeId, WellKnownFields.ControlNumber);
		}

		public static Settings DefaultSettings(int artifactTypeId, string selectedIdentifierFieldName)
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