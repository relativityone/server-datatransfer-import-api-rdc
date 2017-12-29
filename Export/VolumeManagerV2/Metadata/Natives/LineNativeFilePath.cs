using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.LoadFileEntry;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Natives
{
	public class LineNativeFilePath : ILineNativeFilePath
	{
		private readonly ILoadFileCellFormatter _loadFileCellFormatter;
		private readonly ExportFile _exportSettings;
		private readonly IFilePathTransformer _filePathTransformer;
		private readonly ILog _logger;

		public LineNativeFilePath(ILoadFileCellFormatter loadFileCellFormatter, ExportFile exportSettings, IFilePathTransformer filePathTransformer, ILog logger)
		{
			_loadFileCellFormatter = loadFileCellFormatter;
			_exportSettings = exportSettings;
			_filePathTransformer = filePathTransformer;
			_logger = logger;
		}

		public void AddNativeFilePath(DeferredEntry loadFileEntry, ObjectExportInfo artifact)
		{
			if (_exportSettings.ExportNative)
			{
				_logger.LogVerbose("Exporting natives, so adding path to load file entry.");
				string nativeLocationCell;
				if (_exportSettings.VolumeInfo.CopyNativeFilesFromRepository)
				{
					string nativeLocation = string.IsNullOrWhiteSpace(artifact.NativeTempLocation) ? artifact.NativeTempLocation : _filePathTransformer.TransformPath(artifact.NativeTempLocation);
					nativeLocationCell = _loadFileCellFormatter.CreateNativeCell(nativeLocation, artifact);
					_logger.LogVerbose("Copying natives, so path is local {path}.", nativeLocation);
				}
				else
				{
					_logger.LogVerbose("Not copying natives, so path is remote {path}.", artifact.NativeSourceLocation);
					nativeLocationCell = _loadFileCellFormatter.CreateNativeCell(artifact.NativeSourceLocation, artifact);
				}

				loadFileEntry.AddStringEntry(nativeLocationCell);
			}
		}
	}
}