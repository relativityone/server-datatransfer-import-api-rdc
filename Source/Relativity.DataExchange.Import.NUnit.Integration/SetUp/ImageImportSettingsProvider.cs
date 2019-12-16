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
		public static ImageSettings GetImageFilePathSourceDocumentImportSettings()
		{
			ImageSettings settings = new ImageSettings();
			SetDefaultImageDocumentImportSettings(settings);
			SetImageFilePathSourceDocumentImportSettings(settings);
			return settings;
		}

		private static void SetDefaultImageDocumentImportSettings(ImageSettings settings)
		{
			settings.SelectedIdentifierFieldName = WellKnownFields.ControlNumber;
			settings.OverwriteMode = OverwriteModeEnum.Append;
		}

		private static void SetImageFilePathSourceDocumentImportSettings(ImageSettings settings)
		{
			settings.ImageFilePathSourceFieldName = WellKnownFields.FilePath;
			settings.AutoNumberImages = false;

			settings.BatesNumberField = "BatesNumber";
			settings.DocumentIdentifierField = "DocumentIdentifier";
			settings.FileLocationField = "FileLocation";
			settings.CopyFilesToDocumentRepository = true;
		}
	}
}