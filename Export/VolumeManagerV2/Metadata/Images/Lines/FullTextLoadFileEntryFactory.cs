using System;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images.Lines
{
	public class FullTextLoadFileEntryFactory
	{
		private readonly IFieldService _fieldService;
		private readonly LongTextHelper _longTextHelper;
		private readonly ILog _logger;

		public FullTextLoadFileEntryFactory(IFieldService fieldService, LongTextHelper longTextHelper, ILog logger)
		{
			_fieldService = fieldService;
			_longTextHelper = longTextHelper;
			_logger = logger;
		}

		public IFullTextLoadFileEntry Create(ExportFile exportSettings)
		{
			if (exportSettings.LogFileFormat == LoadFileType.FileFormat.IPRO_FullText)
			{
				if (_longTextHelper.IsTextPrecedenceSet())
				{
					_logger.LogVerbose("Created {type} for handling Full Text precedence in IPRO.", nameof(IproFullTextWithPrecedenceLoadFileEntry));
					return new IproFullTextWithPrecedenceLoadFileEntry(exportSettings, _fieldService, _longTextHelper, _logger);
				}
				_logger.LogVerbose("Created {type} for handling Full Text without precedence in IPRO.", nameof(IproFullTextWithoutPrecedenceLoadFileEntry));
				return new IproFullTextWithoutPrecedenceLoadFileEntry(exportSettings, _fieldService, _longTextHelper, _logger);
			}
			if (exportSettings.LogFileFormat == LoadFileType.FileFormat.IPRO || exportSettings.LogFileFormat == LoadFileType.FileFormat.Opticon)
			{
				_logger.LogVerbose("Created {type} for Opticon or IPRO without Full Text.", nameof(NoFullTextLoadFileEntry));
				return new NoFullTextLoadFileEntry();
			}
			_logger.LogError("Unknown image load file type {type}.", exportSettings.LogFileFormat);
			throw new ArgumentException($"Unknown image load file type {exportSettings.LogFileFormat}.");
		}
	}
}