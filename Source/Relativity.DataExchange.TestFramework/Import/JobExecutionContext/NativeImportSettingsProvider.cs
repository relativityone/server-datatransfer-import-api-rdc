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
		public static Settings NativeFilePathSourceDocumentImportSettings
		{
			get
			{
				Settings settings = new Settings();
				SetDefaultNativeDocumentImportSettings(settings);
				SetNativeFilePathSourceDocumentImportSettings(settings);
				return settings;
			}
		}

		public static Settings DefaultNativeDocumentImportSettings
		{
			get
			{
				Settings settings = new Settings();
				SetDefaultNativeDocumentImportSettings(settings);
				return settings;
			}
		}

		public static Settings NativeControlNumberIdentifierObjectImportSettings(int artifactTypeId)
		{
			Settings settings = new Settings();
			SetDefaultNativeDocumentImportSettings(settings);
			settings.ArtifactTypeId = artifactTypeId;
			return settings;
		}

		public static Settings DefaultNativeObjectImportSettings(int artifactTypeId)
		{
			Settings settings = new Settings
			{
				SelectedIdentifierFieldName = WellKnownFields.RdoIdentifier,
				OverwriteMode = OverwriteModeEnum.Append,
				ArtifactTypeId = artifactTypeId,
			};
			return settings;
		}

		private static void SetDefaultNativeDocumentImportSettings(Settings settings)
		{
			settings.SelectedIdentifierFieldName = WellKnownFields.ControlNumber;
			settings.OverwriteMode = OverwriteModeEnum.Append;
		}

		private static void SetNativeFilePathSourceDocumentImportSettings(Settings settings)
		{
			settings.NativeFilePathSourceFieldName = WellKnownFields.FilePath;
			settings.NativeFileCopyMode = NativeFileCopyModeEnum.CopyFiles;

			settings.OIFileIdMapped = true;
			settings.OIFileIdColumnName = WellKnownFields.OutsideInFileId;
			settings.OIFileTypeColumnName = WellKnownFields.OutsideInFileType;

			settings.FileSizeMapped = true;
			settings.FileSizeColumn = WellKnownFields.NativeFileSize;
		}
	}
}
