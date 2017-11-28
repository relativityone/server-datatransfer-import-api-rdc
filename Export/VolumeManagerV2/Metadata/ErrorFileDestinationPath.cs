using System.Text;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata
{
	public class ErrorFileDestinationPath : IDestinationPath
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
	}
}