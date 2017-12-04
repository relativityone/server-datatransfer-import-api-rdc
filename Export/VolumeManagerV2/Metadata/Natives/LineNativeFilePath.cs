using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.LoadFileEntry;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Natives
{
	public class LineNativeFilePath
	{
		private readonly ILoadFileCellFormatter _loadFileCellFormatter;
		private readonly ExportFile _exportSettings;
		private readonly IFilePathTransformer _filePathTransformer;

		public LineNativeFilePath(ILoadFileCellFormatter loadFileCellFormatter, ExportFile exportSettings, IFilePathTransformer filePathTransformer)
		{
			_loadFileCellFormatter = loadFileCellFormatter;
			_exportSettings = exportSettings;
			_filePathTransformer = filePathTransformer;
		}

		public void AddNativeFilePath(DeferredEntry loadFileEntry, ObjectExportInfo artifact)
		{
			if (_exportSettings.ExportNative)
			{
				string nativeLocationCell;
				if (_exportSettings.VolumeInfo.CopyNativeFilesFromRepository)
				{
					string nativeLocation = _filePathTransformer.TransformPath(artifact.NativeTempLocation);
					nativeLocationCell = _loadFileCellFormatter.CreateNativeCell(nativeLocation, artifact);
				}
				else
				{
					nativeLocationCell = _loadFileCellFormatter.CreateNativeCell(artifact.NativeSourceLocation, artifact);
				}
				loadFileEntry.AddStringEntry(nativeLocationCell);
			}
		}
	}
}