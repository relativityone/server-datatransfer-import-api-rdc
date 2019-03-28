namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories
{
	public class LabelManager : ILabelManager
	{
		private readonly string _volumePrefix;
		private readonly string _subdirectoryImagePrefix;
		private readonly string _subdirectoryNativePrefix;
		private readonly string _subdirectoryTextPrefix;

		private readonly int _volumeLabelPaddingWidth;
		private readonly int _subdirectoryPaddingWidth;

		private readonly IVolume _volume;

		private readonly ISubdirectory _subdirectory;

		public LabelManager(ExportFile exportSettings, IVolume volume, ISubdirectory subdirectory)
		{
			_volume = volume;
			_subdirectory = subdirectory;
			_volumePrefix = exportSettings.VolumeInfo.VolumePrefix;
			_subdirectoryImagePrefix = exportSettings.VolumeInfo.get_SubdirectoryImagePrefix();
			_subdirectoryNativePrefix = exportSettings.VolumeInfo.get_SubdirectoryNativePrefix();
			_subdirectoryTextPrefix = exportSettings.VolumeInfo.get_SubdirectoryFullTextPrefix();

			_volumeLabelPaddingWidth = exportSettings.VolumeDigitPadding;
			_subdirectoryPaddingWidth = exportSettings.SubdirectoryDigitPadding;
		}

		public string GetCurrentVolumeLabel()
		{
			return FormatLabel(_volumePrefix, _volume.CurrentVolumeNumber, _volumeLabelPaddingWidth);
		}

		public string GetCurrentImageSubdirectoryLabel()
		{
			return FormatLabel(_subdirectoryImagePrefix, _subdirectory.CurrentSubdirectoryNumber, _subdirectoryPaddingWidth);
		}

		public string GetCurrentNativeSubdirectoryLabel()
		{
			return FormatLabel(_subdirectoryNativePrefix, _subdirectory.CurrentSubdirectoryNumber, _subdirectoryPaddingWidth);
		}

		public string GetCurrentTextSubdirectoryLabel()
		{
			return FormatLabel(_subdirectoryTextPrefix, _subdirectory.CurrentSubdirectoryNumber, _subdirectoryPaddingWidth);
		}

		private string FormatLabel(string prefix, int number, int padding)
		{
			return $"{prefix}{number.ToString().PadLeft(padding, '0')}";
		}
	}
}