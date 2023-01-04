// <copyright file="ImageSettingsBuilder.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport
{
	using System;
	using System.Text;

	using kCura.Relativity.DataReaderClient;

	/// <summary>
	/// That class represents builder of image import settings.
	/// </summary>
	/// <remarks>It is serializable because it needs to be used across AppDomains.</remarks>
	[Serializable]
	public class ImageSettingsBuilder : ISettingsBuilder<ImageSettings>
	{
		private bool withDefaultFieldNames;
		private bool withWellKnownFieldNames;
		private bool withExtractedText;
		private Encoding extractedTextEncoding;
		private bool disableExtractedTextEncodingCheck;
		private OverwriteModeEnum overwriteMode = OverwriteModeEnum.Append;
		private int? productionArtifactId;
		private int? beginBatesFieldArtifactId;
		private int? identityFieldArtifactId;

		public ImageSettings Build()
		{
			var settings = new ImageSettings();

			SetDefaultSettings(settings);

			settings.OverwriteMode = this.overwriteMode;

			settings.BatesNumberField = "Bates_Number";
			settings.DocumentIdentifierField = "Document_Identifier";
			settings.FileLocationField = "File_Location";
			settings.FileNameField = "File_Name";

			if (this.withDefaultFieldNames)
			{
				SetDefaultFieldNames(settings);
			}

			if (this.withWellKnownFieldNames)
			{
				SetWellKnownFieldNames(settings);
			}

			if (this.withExtractedText)
			{
				settings.ExtractedTextFieldContainsFilePath = true;
				settings.ExtractedTextEncoding = this.extractedTextEncoding;
				settings.DisableExtractedTextEncodingCheck = this.disableExtractedTextEncodingCheck;
			}

			if (this.productionArtifactId.HasValue)
			{
				settings.ForProduction = true;
				settings.ProductionArtifactID = this.productionArtifactId.Value;
			}

			if (this.beginBatesFieldArtifactId.HasValue)
			{
				settings.BeginBatesFieldArtifactID = this.beginBatesFieldArtifactId.Value;
			}

			if (this.identityFieldArtifactId.HasValue)
			{
				settings.IdentityFieldId = this.identityFieldArtifactId.Value;
			}

			return settings;
		}

		public ImageSettingsBuilder ForProduction(int artifactId)
		{
			this.productionArtifactId = artifactId;
			return this;
		}

		public ImageSettingsBuilder WithOverlayMode(OverwriteModeEnum mode)
		{
			this.overwriteMode = mode;
			return this;
		}

		public ImageSettingsBuilder WithBeginBatesField(int fieldArtifactId)
		{
			this.beginBatesFieldArtifactId = fieldArtifactId;
			return this;
		}

		public ImageSettingsBuilder WithIdentityField(int fieldArtifactId)
		{
			this.identityFieldArtifactId = fieldArtifactId;
			return this;
		}

		public ImageSettingsBuilder WithWellKnownFieldNames()
		{
			this.withWellKnownFieldNames = true;
			this.withDefaultFieldNames = false;
			return this;
		}

		public ImageSettingsBuilder WithDefaultFieldNames()
		{
			this.withDefaultFieldNames = true;
			this.withWellKnownFieldNames = false;
			return this;
		}

		public ImageSettingsBuilder WithExtractedText(Encoding encoding, bool disableEncodingCheck)
		{
			this.withExtractedText = true;
			this.extractedTextEncoding = encoding;
			this.disableExtractedTextEncodingCheck = disableEncodingCheck;
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

		private static void SetDefaultFieldNames(ImageSettings settings)
		{
			settings.BatesNumberField = null;
			settings.DocumentIdentifierField = null;
			settings.FileLocationField = null;
			settings.FileNameField = null;
		}

		private static void SetWellKnownFieldNames(ImageSettings settings)
		{
			settings.BatesNumberField = WellKnownFields.BatesNumber;
			settings.DocumentIdentifierField = WellKnownFields.DocumentIdentifier;
			settings.FileLocationField = WellKnownFields.FileLocation;
			settings.FileNameField = WellKnownFields.FileName;
		}
	}
}
