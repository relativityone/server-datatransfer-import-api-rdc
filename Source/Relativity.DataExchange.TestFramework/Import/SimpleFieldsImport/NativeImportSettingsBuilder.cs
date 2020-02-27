// <copyright file="NativeImportSettingsBuilder.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport
{
	using System;

	using kCura.Relativity.DataReaderClient;

	/// <summary>
	/// That class represents builder of Native import settings.
	/// </summary>
	/// <remarks>It is serializable because it needs to be used across AppDomains.</remarks>
	[Serializable]
	public class NativeImportSettingsBuilder : ISettingsBuilder<Settings>
	{
		private bool withDefaultSettings;
		private bool withNativeFilePath;
		private string folderPathSourceFieldName;
		private string identifierField;

		private int destinationArtifactTypeId = (int)ArtifactType.Document;

		public Settings Build()
		{
			var settings = new Settings();

			if (this.withDefaultSettings)
			{
				SetDefaultSettings(settings);
			}

			if (this.withNativeFilePath)
			{
				SetFilePathSource(settings);
			}

			if (this.folderPathSourceFieldName != null)
			{
				settings.FolderPathSourceFieldName = this.folderPathSourceFieldName;
			}

			settings.ArtifactTypeId = this.destinationArtifactTypeId;
			settings.SelectedIdentifierFieldName = this.identifierField;

			return settings;
		}

		public NativeImportSettingsBuilder WithFolderPath(string folderFieldName)
		{
			this.folderPathSourceFieldName = folderFieldName;
			return this;
		}

		public NativeImportSettingsBuilder WithNativeFilePath()
		{
			this.withNativeFilePath = true;
			return this;
		}

		public NativeImportSettingsBuilder WithDestinationType(int artifactTypeId, string identifierFieldName)
		{
			this.destinationArtifactTypeId = artifactTypeId;
			this.identifierField = identifierFieldName;
			return this;
		}

		/// <summary>
		/// Append mode with control number as an identifier.
		/// </summary>
		/// <returns>settings builder.</returns>
		public NativeImportSettingsBuilder WithDefaultSettings()
		{
			this.withDefaultSettings = true;
			return this;
		}

		private static void SetDefaultSettings(Settings settings)
		{
			settings.SelectedIdentifierFieldName = WellKnownFields.ControlNumber;
			settings.OverwriteMode = OverwriteModeEnum.Append;
		}

		private static void SetFilePathSource(Settings settings)
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
