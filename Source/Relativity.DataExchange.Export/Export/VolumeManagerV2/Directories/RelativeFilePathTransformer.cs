namespace Relativity.DataExchange.Export.VolumeManagerV2.Directories
{
	using System.IO;

	using kCura.WinEDDS;

	public class RelativeFilePathTransformer : IFilePathTransformer
	{
		private readonly ExportFile _exportSettings;
		private readonly FilePathHelper _filePathHelper;

		public RelativeFilePathTransformer(ExportFile exportSettings, FilePathHelper filePathHelper)
		{
			_exportSettings = exportSettings;
			_filePathHelper = filePathHelper;
		}

		public string TransformPath(string filePath)
		{
			string relativePath = _filePathHelper.MakeRelativePath(_exportSettings.FolderPath, filePath);
			return Path.Combine(".", relativePath);
		}
	}
}