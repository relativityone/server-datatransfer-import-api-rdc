using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Repository;
using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.LoadFileEntry;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text
{
	public class LongTextToFile : ILongTextHandler
	{
		private readonly ExportFile _exportFile;
		private readonly LongTextHelper _longTextHelper;
		private readonly IFilePathTransformer _filePathTransformer;
		private readonly LongTextRepository _longTextRepository;
		private readonly ILog _logger;

		public LongTextToFile(ExportFile exportFile, IFilePathTransformer filePathTransformer, LongTextRepository longTextRepository, LongTextHelper longTextHelper, ILog logger)
		{
			_exportFile = exportFile;
			_filePathTransformer = filePathTransformer;
			_longTextRepository = longTextRepository;
			_longTextHelper = longTextHelper;
			_logger = logger;
		}

		public void HandleLongText(ObjectExportInfo artifact, ViewFieldInfo field, DeferredEntry lineEntry)
		{
			_logger.LogVerbose("Handling long text to file.");
			ViewFieldInfo fieldForPrecedence = _longTextHelper.GetTextPrecedenceTrueField(artifact, field);
			string destinationLocation = _longTextRepository.GetTextFileLocation(artifact.ArtifactID, fieldForPrecedence.FieldArtifactId);
			string textLocation = _filePathTransformer.TransformPath(destinationLocation);
			if (_exportFile.LoadFileIsHtml)
			{
				lineEntry.AddStringEntry($"<a href='{textLocation}' target='_textwindow'>{textLocation}</a>");
			}
			else
			{
				lineEntry.AddStringEntry(textLocation);
			}
		}
	}
}