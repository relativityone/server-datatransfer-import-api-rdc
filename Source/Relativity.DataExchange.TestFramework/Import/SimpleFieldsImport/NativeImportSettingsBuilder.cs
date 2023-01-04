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
		private bool withNativeFilePath;
		private bool withMove;
		private string folderPathSourceFieldName;
		private string identifierField;
		private kCura.EDDS.WebAPI.BulkImportManagerBase.OverlayBehavior? overlayBehavior;
		private OverwriteModeEnum? overwriteMode;
		private int destinationFolderArtifactId = -1;
		private int destinationArtifactTypeId = (int)ArtifactType.Document;

		public static NativeImportSettingsBuilder New()
		{
			return new NativeImportSettingsBuilder();
		}

		public Settings Build()
		{
			var settings = new Settings();

			if (this.withNativeFilePath)
			{
				SetFilePathSource(settings);
			}

			if (this.folderPathSourceFieldName != null)
			{
				settings.FolderPathSourceFieldName = this.folderPathSourceFieldName;
			}

			if (this.overlayBehavior.HasValue)
			{
				settings.OverlayBehavior = this.overlayBehavior.Value;
			}

			if (this.overwriteMode.HasValue)
			{
				settings.OverwriteMode = this.overwriteMode.Value;
			}

			if (this.destinationFolderArtifactId != -1)
			{
				settings.DestinationFolderArtifactID = this.destinationFolderArtifactId;
			}

			settings.MoveDocumentsInAppendOverlayMode = this.withMove;
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

		public NativeImportSettingsBuilder WithDestinationType(int artifactTypeId)
		{
			this.destinationArtifactTypeId = artifactTypeId;
			return this;
		}

		public NativeImportSettingsBuilder WithOverwriteMode(OverwriteModeEnum overwriteModeValue)
		{
			this.overwriteMode = overwriteModeValue;
			return this;
		}

		public NativeImportSettingsBuilder WithFieldOverlayMode(kCura.EDDS.WebAPI.BulkImportManagerBase.OverlayBehavior overlayType)
		{
			this.overlayBehavior = overlayType;
			return this;
		}

		public NativeImportSettingsBuilder WithIdentifierField(string identifierFieldName)
		{
			this.identifierField = identifierFieldName;
			return this;
		}

		public NativeImportSettingsBuilder WithMove(bool move)
		{
			this.withMove = move;
			return this;
		}

		public NativeImportSettingsBuilder WithDestinationFolderArtifactId(int folderArtifactId)
		{
			this.destinationFolderArtifactId = folderArtifactId;
			return this;
		}

		/// <summary>
		/// Append mode with control number as an identifier.
		/// </summary>
		/// <returns>settings builder.</returns>
		public NativeImportSettingsBuilder WithDefaultSettings()
		{
			return this
				.WithOverwriteMode(OverwriteModeEnum.Append)
				.WithIdentifierField(WellKnownFields.ControlNumber);
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
