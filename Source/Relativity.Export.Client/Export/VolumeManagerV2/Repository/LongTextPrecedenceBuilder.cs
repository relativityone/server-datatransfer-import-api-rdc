using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Castle.Core;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics;
using kCura.WinEDDS.Exporters;
using Relativity;
using Relativity.Logging;
using Constants = Relativity.Export.Constants;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Repository
{
	public class LongTextPrecedenceBuilder : ILongTextBuilder
	{
		private readonly ExportFile _exportSettings;
		private readonly IFilePathProvider _filePathProvider;
		private readonly IFieldService _fieldService;
		private readonly LongTextHelper _longTextHelper;
		private readonly IFileNameProvider _fileNameProvider;
		private readonly ILog _logger;
		private readonly IExportFileValidator _exportFileValidator;
		private readonly IMetadataProcessingStatistics _metadataProcessingStatistics;

		public LongTextPrecedenceBuilder(ExportFile exportSettings, LongTextFilePathProvider filePathProvider, IFieldService fieldService, LongTextHelper longTextHelper,
			IFileNameProvider fileNameProvider, ILog logger, IExportFileValidator exportFileValidator, IMetadataProcessingStatistics metadataProcessingStatistics) : this(exportSettings,
			(IFilePathProvider) filePathProvider, fieldService, longTextHelper, fileNameProvider, logger, exportFileValidator, metadataProcessingStatistics)
		{
		}

		/// <summary>
		///     Used for testing
		/// </summary>
		/// <param name="exportSettings"></param>
		/// <param name="filePathProvider"></param>
		/// <param name="fieldService"></param>
		/// <param name="longTextHelper"></param>
		/// <param name="fileNameProvider"></param>
		/// <param name="logger"></param>
		/// <param name="exportFileValidator"></param>
		/// <param name="metadataProcessingStatistics"></param>
		[DoNotSelect]
		public LongTextPrecedenceBuilder(ExportFile exportSettings, IFilePathProvider filePathProvider, IFieldService fieldService, LongTextHelper longTextHelper,
			IFileNameProvider fileNameProvider, ILog logger, IExportFileValidator exportFileValidator, IMetadataProcessingStatistics metadataProcessingStatistics)
		{
			_exportSettings = exportSettings;
			_filePathProvider = filePathProvider;
			_fieldService = fieldService;
			_longTextHelper = longTextHelper;
			_fileNameProvider = fileNameProvider;
			_logger = logger;
			_exportFileValidator = exportFileValidator;
			_metadataProcessingStatistics = metadataProcessingStatistics;
		}

		public IList<LongText> CreateLongText(ObjectExportInfo artifact, CancellationToken cancellationToken)
		{
			_logger.LogVerbose("Attempting to create LongText for TextPrecedence field.");

			if (cancellationToken.IsCancellationRequested)
			{
				return Enumerable.Empty<LongText>().ToList();
			}

			ViewFieldInfo field = GetFieldForLongTextPrecedenceDownload(artifact);

			_logger.LogVerbose("Text Precedence is stored in field {fieldName}:{fieldId}.", field.AvfColumnName, field.FieldArtifactId);

			if (_longTextHelper.IsTextTooLong(artifact, Constants.TEXT_PRECEDENCE_AWARE_AVF_COLUMN_NAME))
			{
				return CreateForTooLongText(artifact, field).InList();
			}

			return CreateForLongText(artifact, field).InList();
		}

		private LongText CreateForTooLongText(ObjectExportInfo artifact, ViewFieldInfo field)
		{
			string destinationLocation = GetDestinationLocation(artifact);
			LongTextExportRequest longTextExportRequest = CreateExportRequest(artifact, field, destinationLocation);
			Encoding sourceEncoding = _longTextHelper.GetLongTextFieldFileEncoding(field);
			if (_exportSettings.ExportFullTextAsFile)
			{
				if (CanExport(destinationLocation))
				{
					_logger.LogVerbose("LongText file missing - creating ExportRequest to destination file.");

					return LongText.CreateFromMissingFile(artifact.ArtifactID, longTextExportRequest.FieldArtifactId, longTextExportRequest, sourceEncoding, _exportSettings.TextFileEncoding);
				}

				_logger.LogVerbose("File {file} exists and won't be overwritten - updating statistics.", destinationLocation);
				_metadataProcessingStatistics.UpdateStatisticsForFile(destinationLocation);

				_logger.LogWarning("LongText file already exists and cannot overwrite - creating ExportRequest from existing file. Assuming that file encoding is the same as selected.");
				return LongText.CreateFromExistingFile(artifact.ArtifactID, longTextExportRequest.FieldArtifactId, destinationLocation, _exportSettings.TextFileEncoding);
			}

			_logger.LogVerbose("LongText file missing - creating ExportRequest to temporary file.");
			return LongText.CreateFromMissingValue(artifact.ArtifactID, longTextExportRequest.FieldArtifactId, longTextExportRequest, sourceEncoding);
		}

		private LongText CreateForLongText(ObjectExportInfo artifact, ViewFieldInfo field)
		{
			string longTextValue = _longTextHelper.GetTextFromField(artifact, Constants.TEXT_PRECEDENCE_AWARE_AVF_COLUMN_NAME);

			if (_exportSettings.ExportFullTextAsFile)
			{
				string destinationLocation = GetDestinationLocation(artifact);
				if (CanExport(destinationLocation))
				{
					_logger.LogVerbose("LongText value exists - writing it to destination file {location}.", destinationLocation);
					using (StreamWriter streamWriter = new StreamWriter(destinationLocation, false, _exportSettings.TextFileEncoding))
					{
						streamWriter.Write(longTextValue);
					}
				}
				else
				{
					_logger.LogVerbose("LongText file already exists and cannot overwrite - using existing file {location}.", destinationLocation);
				}

				_logger.LogVerbose("File {file} exists or has been created from metadata - updating statistics.", destinationLocation);
				_metadataProcessingStatistics.UpdateStatisticsForFile(destinationLocation);
				return LongText.CreateFromExistingFile(artifact.ArtifactID, field.FieldArtifactId, destinationLocation, _exportSettings.TextFileEncoding);
			}

			_logger.LogVerbose("LongText value exists - storing it into memory.");
			return LongText.CreateFromExistingValue(artifact.ArtifactID, field.FieldArtifactId, longTextValue);
		}

		private LongTextExportRequest CreateExportRequest(ObjectExportInfo artifact, ViewFieldInfo field, string destinationLocation)
		{
			if (_exportSettings.ArtifactTypeID == (int) ArtifactType.Document && field.Category == FieldCategory.FullText && !(field is CoalescedTextViewField))
			{
				return LongTextExportRequest.CreateRequestForFullText(artifact, field.FieldArtifactId, destinationLocation);
			}

			ViewFieldInfo fieldToExport = GetFieldForLongTextPrecedenceDownload(artifact, field);
			return LongTextExportRequest.CreateRequestForLongText(artifact, fieldToExport.FieldArtifactId, destinationLocation);
		}

		private ViewFieldInfo GetFieldForLongTextPrecedenceDownload(ObjectExportInfo artifact)
		{
			int fieldArtifactId = (int) artifact.Metadata[_fieldService.GetOrdinalIndex(Constants.TEXT_PRECEDENCE_AWARE_ORIGINALSOURCE_AVF_COLUMN_NAME)];
			return _exportSettings.SelectedTextFields.First(x => x.FieldArtifactId == fieldArtifactId);
		}

		private ViewFieldInfo GetFieldForLongTextPrecedenceDownload(ObjectExportInfo artifact, ViewFieldInfo field)
		{
			if (field == null || field.AvfColumnName == Constants.TEXT_PRECEDENCE_AWARE_AVF_COLUMN_NAME)
			{
				return GetFieldForLongTextPrecedenceDownload(artifact);
			}

			return field;
		}

		private string GetDestinationLocation(ObjectExportInfo artifact)
		{
			if (_exportSettings.ExportFullTextAsFile)
			{
				string fileName = _fileNameProvider.GetTextName(artifact);
				return _filePathProvider.GetPathForFile(fileName, artifact.ArtifactID);
			}

			return TempFileBuilder.GetTempFileName(TempFileConstants.LongTextFileNameSuffix);
        }

		private bool CanExport(string destinationLocation)
		{
			string warningMessageInCaseOfOverwriting = $"Overwriting text file {destinationLocation}.";
			return _exportFileValidator.CanExport(destinationLocation, warningMessageInCaseOfOverwriting);
		}
	}
}