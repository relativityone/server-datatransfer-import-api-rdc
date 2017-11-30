using System.Collections.Generic;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images
{
	public abstract class ImageLoadFile
	{
		private IList<KeyValuePair<string, string>> _lines;

		private readonly ILoadFileEntry _loadFileEntry;
		private readonly ExportFile _exportSettings;
		private readonly IFilePathProvider _filePathProvider;
		private readonly IFullTextLoadFileEntry _fullTextLoadFileEntry;

		protected ImageLoadFile(ExportFile exportSettings, IFilePathProvider filePathProvider, ILoadFileEntry loadFileEntry, IFullTextLoadFileEntry fullTextLoadFileEntry)
		{
			_exportSettings = exportSettings;
			_filePathProvider = filePathProvider;
			_loadFileEntry = loadFileEntry;
			_fullTextLoadFileEntry = fullTextLoadFileEntry;
		}

		public IList<KeyValuePair<string, string>> CreateLoadFileEntries(ObjectExportInfo[] artifacts)
		{
			_lines = new List<KeyValuePair<string, string>>();

			foreach (var artifact in artifacts)
			{
				CreateLoadFileEntry(artifact);
			}

			return _lines;
		}

		protected abstract List<ImageExportInfo> GetImagesToProcess(ObjectExportInfo artifact);

		private void CreateLoadFileEntry(ObjectExportInfo artifact)
		{
			int numberOfPages = artifact.Images.Count;
			List<ImageExportInfo> images = GetImagesToProcess(artifact);

			for (int i = 0; i < images.Count; i++)
			{
				ImageExportInfo image = images[i];
				long pageOffset;
				if (i == 0 && image.PageOffset == null || i == images.Count - 1)
				{
					pageOffset = long.MinValue;
				}
				else
				{
					ImageExportInfo nextImage = images[i + 1];
					pageOffset = nextImage.PageOffset ?? long.MinValue;
				}

				string localFilePath = GetLocalFilePath(images, i);

				KeyValuePair<string, string> fullTextEntry;
				if (_fullTextLoadFileEntry.TryCreateFullTextLine(artifact, image.BatesNumber, i, pageOffset, out fullTextEntry))
				{
					_lines.Add(fullTextEntry);
				}

				KeyValuePair<string, string> loadFileEntry = _loadFileEntry.Create(image.BatesNumber, localFilePath, i + 1, pageOffset, numberOfPages);
				_lines.Add(loadFileEntry);
			}
		}

		private string GetLocalFilePath(List<ImageExportInfo> images, int i)
		{
			int baseImageIndex = GetBaseImageIndex(i);
			string localFilePath = images[baseImageIndex].SourceLocation;
			if (_exportSettings.VolumeInfo.CopyImageFilesFromRepository)
			{
				localFilePath = _filePathProvider.GetPathForLoadFile(images[baseImageIndex].TempLocation);
			}
			return localFilePath;
		}

		protected abstract int GetBaseImageIndex(int i);
	}
}