using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.DataSize
{
	public class ObjectExportableSize : IObjectExportableSize
	{
		private readonly NativeExportableSize _nativeExportableSize;
		private readonly ImageExportableSize _imageExportableSize;
		private readonly TextExportableSize _textExportableSize;

		public ObjectExportableSize(NativeExportableSize nativeExportableSize, ImageExportableSize imageExportableSize, TextExportableSize textExportableSize)
		{
			_nativeExportableSize = nativeExportableSize;
			_imageExportableSize = imageExportableSize;
			_textExportableSize = textExportableSize;
		}

		public void FinalizeSizeCalculations(ObjectExportInfo artifact, VolumePredictions volumeSize)
		{
			_nativeExportableSize.CalculateNativesSize(volumeSize);

			_imageExportableSize.CalculateImagesSize(volumeSize);

			_textExportableSize.CalculateTextSize(volumeSize, artifact);
		}
	}
}