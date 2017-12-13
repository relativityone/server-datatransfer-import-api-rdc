using System.Text;
using kCura.WinEDDS.Exceptions;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Paths
{
	public class ErrorFileDestinationPath : IDestinationPath, IErrorFile
	{
		private string _errorFilePath;
		private readonly ExportFile _exportSettings;

		public ErrorFileDestinationPath(ExportFile exportSettings)
		{
			_exportSettings = exportSettings;
		}

		public string Path
		{
			get
			{
				if (string.IsNullOrEmpty(_errorFilePath))
				{
					_errorFilePath = System.IO.Path.GetTempFileName();
				}
				return _errorFilePath;
			}
		}

		public Encoding Encoding => _exportSettings.LoadFileEncoding;
		public FileWriteException.DestinationFile DestinationFileType => FileWriteException.DestinationFile.Errors;

		public bool IsErrorFileCreated()
		{
			return !string.IsNullOrEmpty(_errorFilePath);
		}

		string IErrorFile.Path()
		{
			return Path;
		}
	}
}