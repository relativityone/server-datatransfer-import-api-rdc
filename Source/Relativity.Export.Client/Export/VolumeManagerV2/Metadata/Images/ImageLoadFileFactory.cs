using Castle.Windsor;
using Relativity;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images
{
	public class ImageLoadFileFactory
	{
		public IImageLoadFile Create(ExportFile exportSettings, IWindsorContainer container)
		{
			if (exportSettings.ArtifactTypeID != (int) ArtifactType.Document || !exportSettings.ExportImages)
			{
				return new EmptyImageLoadFile();
			}

			return container.Resolve<ImageLoadFile>();
		}
	}
}