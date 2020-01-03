// -----------------------------------------------------------------------------------------------------
// <copyright file="ImageImportSettingsProvider.cs" company="Relativity ODA LLC">
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

	public static class ImageImportSettingsProvider
	{
		public static ImageSettings GetImageFilePathSourceDocumentImportSettings(bool useDefaultFieldNames)
		{
			ImageSettings settings = new ImageSettings();
			SetDefaultImageDocumentImportSettings(settings);
			SetImageFilePathSourceDocumentImportSettings(settings, useDefaultFieldNames);
			return settings;
		}

		private static void SetDefaultImageDocumentImportSettings(ImageSettings settings)
		{
			settings.SelectedIdentifierFieldName = WellKnownFields.ControlNumber;
			settings.OverwriteMode = OverwriteModeEnum.Append;
		}

		private static void SetImageFilePathSourceDocumentImportSettings(ImageSettings settings, bool useDefaultFieldNames)
		{
			settings.ImageFilePathSourceFieldName = WellKnownFields.FilePath;
			settings.AutoNumberImages = false;

			settings.BatesNumberField = useDefaultFieldNames ? null : "Bates_Number";
			settings.DocumentIdentifierField = useDefaultFieldNames ? null : "Document_Identifier";
			settings.FileLocationField = useDefaultFieldNames ? null : "File_Location";
			settings.FileNameField = useDefaultFieldNames ? null : "File_Name";

			settings.CopyFilesToDocumentRepository = true;
		}
	}
}