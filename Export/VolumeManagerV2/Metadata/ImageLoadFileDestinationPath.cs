using System.Text;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata
{
	public class ImageLoadFileDestinationPath : DestinationPath
	{
		public ImageLoadFileDestinationPath(ExportFile exportSettings) : base(exportSettings)
		{
		}

		public override string Path => FormatPath(GetExtension());

		public override Encoding Encoding => GetEncoding();

		private string GetExtension()
		{
			string logFileExtension = "";

			switch (ExportSettings.LogFileFormat)
			{
				case LoadFileType.FileFormat.Opticon:
					logFileExtension = ".opt";
					break;
				case LoadFileType.FileFormat.IPRO:
					logFileExtension = ".lfp";
					break;
				case LoadFileType.FileFormat.IPRO_FullText:
					logFileExtension = "_FULLTEXT_.lfp";
					break;
			}
			return logFileExtension;
		}

		private Encoding GetEncoding()
		{
			Encoding encoding = ExportSettings.LoadFileEncoding;
			if (ExportSettings.ExportImages)
			{
				encoding = Encoding.Default;
				if (ExportSettings.LogFileFormat != LoadFileType.FileFormat.Opticon)
				{
					encoding = Encoding.UTF8;
				}
			}
			return encoding;
		}
	}
}