using System.Linq;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images
{
	public class FullTextLoadFileEntryFactory
	{
		private readonly IFieldService _fieldService;
		private readonly DownloadedTextFilesRepository _downloadedTextFilesRepository;

		public FullTextLoadFileEntryFactory(IFieldService fieldService, DownloadedTextFilesRepository downloadedTextFilesRepository)
		{
			_fieldService = fieldService;
			_downloadedTextFilesRepository = downloadedTextFilesRepository;
		}

		public IFullTextLoadFileEntry Create(ExportFile exportSettings)
		{
			//TODO validate arguments
			if (exportSettings.LogFileFormat == LoadFileType.FileFormat.IPRO_FullText)
			{
				if (exportSettings.IsTextPrecedenceSet())
				{
					return new IproFullTextWithPrecedenceLoadFileEntry(exportSettings, _fieldService, _downloadedTextFilesRepository);
				}
				return new IproFullTextWithoutPrecedenceLoadFileEntry(exportSettings, _fieldService, _downloadedTextFilesRepository);
			}
			return new NoFullTextLoadFileEntry();
		}
	}
}