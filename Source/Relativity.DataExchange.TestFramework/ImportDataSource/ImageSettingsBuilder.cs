// <copyright file="ImageSettingsBuilder.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.ImportDataSource
{
	using System;

	using kCura.Relativity.DataReaderClient;

	/// <summary>
	/// That class represents builder of image import settings.
	/// </summary>
	/// <remarks>It is serializable because it needs to be used across AppDomains.</remarks>
	[Serializable]
	public class ImageSettingsBuilder : ISettingsBuilder<ImageSettings>
	{
		private bool withDefaultFieldNames;
		private OverwriteModeEnum overwriteMode = OverwriteModeEnum.Append;

		public ImageSettings Build()
		{
			var settings = new ImageSettings();

			SetDefaultSettings(settings);

			settings.OverwriteMode = this.overwriteMode;

			settings.BatesNumberField = this.withDefaultFieldNames ? null : "Bates_Number";
			settings.DocumentIdentifierField = this.withDefaultFieldNames ? null : "Document_Identifier";
			settings.FileLocationField = this.withDefaultFieldNames ? null : "File_Location";
			settings.FileNameField = this.withDefaultFieldNames ? null : "File_Name";

			return settings;
		}

		public ImageSettingsBuilder WithOverlayMode(OverwriteModeEnum mode)
		{
			this.overwriteMode = mode;
			return this;
		}

		public ImageSettingsBuilder WithDefaultFieldNames()
		{
			this.withDefaultFieldNames = true;
			return this;
		}

		private static void SetDefaultSettings(ImageSettings settings)
		{
			settings.ImageFilePathSourceFieldName = WellKnownFields.FilePath;
			settings.AutoNumberImages = false;
			settings.CopyFilesToDocumentRepository = true;

			settings.SelectedIdentifierFieldName = WellKnownFields.ControlNumber;
			settings.IdentityFieldId = WellKnownFields.ControlNumberId;
			settings.MaximumErrorCount = WellKnownFields.MaximumErrorCount;
		}
	}
}
