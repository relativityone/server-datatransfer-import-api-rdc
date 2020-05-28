namespace Relativity.DataExchange.Export.VolumeManagerV2.DataSize
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;

	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Text;
	using Relativity.DataExchange.Service;

	public class ObjectExportableSize : IObjectExportableSize
	{
		private readonly ExportFile exportSettings;
		private readonly LongTextHelper longTextHelper;
		private readonly IFieldService fieldService;

		public ObjectExportableSize(ExportFile exportSettings, LongTextHelper longTextHelper, IFieldService fieldService)
		{
			this.exportSettings = exportSettings;
			this.longTextHelper = longTextHelper;
			this.fieldService = fieldService;
		}

		public void FinalizeSizeCalculations(ObjectExportInfo artifact, VolumePredictions volumeSize)
		{
			this.CalculateNativesSize(volumeSize);
			this.CalculatePdfSize(volumeSize);
			this.CalculateImagesSize(volumeSize);
			this.CalculateTextSize(artifact, volumeSize);
		}

		private void CalculateNativesSize(VolumePredictions volumeSize)
		{
			bool isNativeFileBeingExported =
				this.exportSettings.ExportNative && this.exportSettings.VolumeInfo.CopyNativeFilesFromRepository;
			if (!isNativeFileBeingExported)
			{
				volumeSize.NativeFileCount = 0;
				volumeSize.NativeFilesSize = 0;
			}
		}

		private void CalculatePdfSize(VolumePredictions volumeSize)
		{
			bool isPdfFileBeingExported = this.exportSettings.ExportPdf && this.exportSettings.VolumeInfo.CopyPdfFilesFromRepository;
			if (!isPdfFileBeingExported)
			{
				volumeSize.PdfFileCount = 0;
				volumeSize.PdfFileSize = 0;
			}
		}

		private void CalculateImagesSize(VolumePredictions volumeSize)
		{
			const double PdfMergeSizeErrorThreshold = 1.03;

			bool areImageFilesBeingExported = exportSettings.ExportImages && exportSettings.VolumeInfo.CopyImageFilesFromRepository;
			if (areImageFilesBeingExported)
			{
				bool areImagesBeingMergedIntoMultiPage = volumeSize.ImageFileCount > 0 &&
														 (exportSettings.TypeOfImage == ExportFile.ImageType.MultiPageTiff || exportSettings.TypeOfImage == ExportFile.ImageType.Pdf);
				if (areImagesBeingMergedIntoMultiPage)
				{
					//TODO REL-185531 image size will probably be changed after merging - another issue with size calculations? REL-185531
					//After merging size will be probably smaller, so calculation isn't precise, but we can live with that

					volumeSize.ImageFileCount = 1;

					if (exportSettings.TypeOfImage == ExportFile.ImageType.Pdf)
					{
						//TODO REL-185531 images merge to PDF will be a little bigger than single tiffs so we're applying 3% factor
						volumeSize.ImageFilesSize = (long)Math.Ceiling(volumeSize.ImageFilesSize * PdfMergeSizeErrorThreshold);
					}
				}
			}
			else
			{
				volumeSize.ImageFileCount = 0;
				volumeSize.ImageFilesSize = 0;
			}
		}

		private void CalculateTextSize(ObjectExportInfo artifact, VolumePredictions volumeSize)
		{
			bool isTextBeingExportedToFile = exportSettings.ExportFullText && exportSettings.ExportFullTextAsFile && exportSettings.SelectedTextFields != null;
			if (isTextBeingExportedToFile)
			{
				List<kCura.WinEDDS.ViewFieldInfo> fields = this.fieldService.GetColumns().Where(IsTextPrecedenceField).ToList();

				foreach (var field in fields)
				{
					volumeSize.TextFileCount += 1;

					string columnName = field.AvfColumnName;
					string textValue = this.longTextHelper.GetTextFromField(artifact, columnName);

					this.CalculateTextSizeForSingleField(artifact, volumeSize, textValue);
				}
			}
			else
			{
				volumeSize.TextFileCount = 0;
				volumeSize.TextFilesSize = 0;
			}
		}

		private void CalculateTextSizeForSingleField(ObjectExportInfo artifact, VolumePredictions volumeSize, string textValue)
		{
			const long ExtractedTextSizeNaive = 2097152;

			if (textValue == ServiceConstants.LONG_TEXT_EXCEEDS_MAX_LENGTH_FOR_LIST_TOKEN)
			{
				if (!this.fieldService.ContainsFieldName(ServiceConstants.TEXT_PRECEDENCE_AWARE_TEXT_SIZE))
				{
					//This is just for backward compatibility
					volumeSize.TextFilesSize += ExtractedTextSizeNaive;
				}
				else
				{
					int columnWithSizeIndex = this.fieldService.GetOrdinalIndex(ServiceConstants.TEXT_PRECEDENCE_AWARE_TEXT_SIZE);
					long sizeInUnicode = (long)artifact.Metadata[columnWithSizeIndex];
					if (exportSettings.TextFileEncoding.Equals(Encoding.Unicode))
					{
						volumeSize.TextFilesSize += sizeInUnicode;
					}
					else
					{
						long maxBytesForCharacters = EncodingFileSize.CalculateLongTextFileSize(sizeInUnicode, exportSettings.TextFileEncoding);
						volumeSize.TextFilesSize += maxBytesForCharacters;
					}
				}
			}
			else
			{
				volumeSize.TextFilesSize += exportSettings.TextFileEncoding.GetByteCount(textValue);
			}
		}

		private bool IsTextPrecedenceField(kCura.WinEDDS.ViewFieldInfo field)
		{
			return (field.FieldType == FieldType.Text || field.FieldType == FieldType.OffTableText) && field is CoalescedTextViewField;
		}
	}
}