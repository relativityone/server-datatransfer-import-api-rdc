using Castle.Windsor;
using Relativity;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers
{
	public class ImageLoadFileWriterFactory
	{
		public IImageLoadFileWriter Create(ExportFile exportSettings, IWindsorContainer container)
		{
			if (exportSettings.ArtifactTypeID != (int) ArtifactType.Document)
			{
				return new EmptyImageLoadFileWriter();
			}
			return container.Resolve<ImageLoadFileWriterRetryable>();
		}
	}
}