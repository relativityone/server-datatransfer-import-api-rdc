// -----------------------------------------------------------------------------------------------------
// <copyright file="NativeImportSettingsProvider.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents an abstract load-file base class.
// </summary>
// -----------------------------------------------------------------------------------------------------
namespace Relativity.DataExchange.Import.NUnit.Integration.SetUp
{
	using kCura.Relativity.DataReaderClient;

	using Relativity.DataExchange.TestFramework;

	public static class NativeImportSettingsProvider
	{
		public static Settings GetNativeFilePathSourceDocumentImportSettings()
		{
			Settings settings = new Settings();
			SetDefaultNativeDocumentImportSettings(settings);
			SetNativeFilePathSourceDocumentImportSettings(settings);
			return settings;
		}

		public static Settings GetDefaultNativeDocumentImportSettings()
		{
			Settings settings = new Settings();
			SetDefaultNativeDocumentImportSettings(settings);
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
