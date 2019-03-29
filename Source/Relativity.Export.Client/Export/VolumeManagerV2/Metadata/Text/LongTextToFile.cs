namespace Relativity.Export.VolumeManagerV2.Metadata.Text
{
	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;
	using kCura.WinEDDS.LoadFileEntry;

	using Relativity.Export.VolumeManagerV2.Directories;
	using Relativity.Export.VolumeManagerV2.Repository;
	using Relativity.Logging;

	public class LongTextToFile : ILongTextHandler
	{
		private readonly ExportFile _exportSettings;
		private readonly LongTextHelper _longTextHelper;
		private readonly IFilePathTransformer _filePathTransformer;
		private readonly LongTextRepository _longTextRepository;
		private readonly ILog _logger;

		public LongTextToFile(ExportFile exportSettings, IFilePathTransformer filePathTransformer, LongTextRepository longTextRepository, LongTextHelper longTextHelper, ILog logger)
		{
			_exportSettings = exportSettings;
			_filePathTransformer = filePathTransformer;
			_longTextRepository = longTextRepository;
			_longTextHelper = longTextHelper;
			_logger = logger;
		}

		public void HandleLongText(ObjectExportInfo artifact, kCura.WinEDDS.ViewFieldInfo field, DeferredEntry lineEntry)
		{
			_logger.LogVerbose("Handling long text to file.");
			ViewFieldInfo fieldForPrecedence = _longTextHelper.GetTextPrecedenceTrueField(artifact, field);
			string destinationLocation = _longTextRepository.GetTextFileLocation(artifact.ArtifactID, fieldForPrecedence.FieldArtifactId);
			string textLocation = _filePathTransformer.TransformPath(destinationLocation);
			if (_exportSettings.LoadFileIsHtml)
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