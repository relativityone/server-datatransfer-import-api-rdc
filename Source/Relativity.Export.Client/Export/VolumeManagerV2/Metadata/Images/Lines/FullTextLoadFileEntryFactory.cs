using System;
using Castle.Windsor;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text;
using Relativity;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images.Lines
{
	public class FullTextLoadFileEntryFactory
	{
		private readonly LongTextHelper _longTextHelper;
		private readonly ILog _logger;

		public FullTextLoadFileEntryFactory(LongTextHelper longTextHelper, ILog logger)
		{
			_longTextHelper = longTextHelper;
			_logger = logger;
		}

		public IFullTextLoadFileEntry Create(ExportFile exportSettings, IWindsorContainer container)
		{
			if (exportSettings.ArtifactTypeID != (int) ArtifactType.Document)
			{
				_logger.LogVerbose("Created {type} for handling artifact type different than Document - {type}.", nameof(NoFullTextLoadFileEntry), exportSettings.ArtifactTypeID);
				return container.Resolve<NoFullTextLoadFileEntry>();
			}

			if (exportSettings.LogFileFormat == LoadFileType.FileFormat.IPRO_FullText)
			{
				if (_longTextHelper.IsTextPrecedenceSet())
				{
					_logger.LogVerbose("Created {type} for handling Full Text precedence in IPRO.", nameof(IproFullTextWithPrecedenceLoadFileEntry));
					return container.Resolve<IproFullTextWithPrecedenceLoadFileEntry>();
				}

				_logger.LogVerbose("Created {type} for handling Full Text without precedence in IPRO.", nameof(IproFullTextWithoutPrecedenceLoadFileEntry));
				return container.Resolve<IproFullTextWithoutPrecedenceLoadFileEntry>();
			}

			if (exportSettings.LogFileFormat == LoadFileType.FileFormat.IPRO || exportSettings.LogFileFormat == LoadFileType.FileFormat.Opticon)
			{
				_logger.LogVerbose("Created {type} for Opticon or IPRO without Full Text.", nameof(NoFullTextLoadFileEntry));
				return container.Resolve<NoFullTextLoadFileEntry>();
			}

			_logger.LogError("Unknown image load file type {type}.", exportSettings.LogFileFormat);
			throw new ArgumentException($"Unknown image load file type {exportSettings.LogFileFormat}.");
		}
	}
}