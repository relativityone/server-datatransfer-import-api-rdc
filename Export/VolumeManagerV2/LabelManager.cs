namespace kCura.WinEDDS.Core.Export.VolumeManagerV2
{
	public class LabelManager
	{
		private readonly string _volumePrefix;
		private readonly string _subdirectoryImagePrefix;
		private readonly string _subdirectoryNativePrefix;
		private readonly string _subdirectoryTextPrefix;

		private readonly int _volumeLabelPaddingWidth;
		private readonly int _subdirectoryPaddingWidth;

		public LabelManager(ExportFile exportSettings)
		{
			_volumePrefix = exportSettings.VolumeInfo.VolumePrefix;
			_subdirectoryImagePrefix = exportSettings.VolumeInfo.get_SubdirectoryImagePrefix();
			_subdirectoryNativePrefix = exportSettings.VolumeInfo.get_SubdirectoryNativePrefix();
			_subdirectoryTextPrefix = exportSettings.VolumeInfo.get_SubdirectoryFullTextPrefix();

			_volumeLabelPaddingWidth = exportSettings.VolumeDigitPadding;
			_subdirectoryPaddingWidth = exportSettings.SubdirectoryDigitPadding;
		}

		public string GetCurrentVolumeLabel(int volumeNumber)
		{
			return FormatLabel(_volumePrefix, volumeNumber, _volumeLabelPaddingWidth);
		}

		public string GetCurrentImageSubdirectoryLabel(int subdirectoryNumber)
		{
			return FormatLabel(_subdirectoryImagePrefix, subdirectoryNumber, _subdirectoryPaddingWidth);
		}

		public string GetCurrentNativeSubdirectoryLabel(int subdirectoryNumber)
		{
			return FormatLabel(_subdirectoryNativePrefix, subdirectoryNumber, _subdirectoryPaddingWidth);
		}

		public string GetCurrentTextSubdirectoryLabel(int subdirectoryNumber)
		{
			return FormatLabel(_subdirectoryTextPrefix, subdirectoryNumber, _subdirectoryPaddingWidth);
		}

		private string FormatLabel(string prefix, int number, int padding)
		{
			return $"{prefix}{number.ToString().PadLeft(padding, '0')}";
		}
	}
}