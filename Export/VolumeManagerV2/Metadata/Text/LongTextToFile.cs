using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text.Repository;
using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.LoadFileEntry;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text
{
	public class LongTextToFile : ILongTextHandler
	{
		private readonly ExportFile _exportFile;
		private readonly LongTextHelper _longTextHelper;
		private readonly IFilePathProvider _filePathProvider;
		private readonly LongTextRepository _longTextRepository;

		public LongTextToFile(ExportFile exportFile, IFilePathProvider filePathProvider, LongTextRepository longTextRepository, LongTextHelper longTextHelper)
		{
			_exportFile = exportFile;
			_filePathProvider = filePathProvider;
			_longTextRepository = longTextRepository;
			_longTextHelper = longTextHelper;
		}

		public void HandleLongText(ObjectExportInfo artifact, ViewFieldInfo field, DeferredEntry lineEntry)
		{
			ViewFieldInfo fieldForPrecedence = _longTextHelper.GetTextPrecedenceTrueField(artifact, field);
			string destinationLocation = _longTextRepository.GetTextFileLocation(artifact.ArtifactID, fieldForPrecedence.FieldArtifactId);
			string textLocation = _filePathProvider.GetPathForLoadFile(destinationLocation);
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