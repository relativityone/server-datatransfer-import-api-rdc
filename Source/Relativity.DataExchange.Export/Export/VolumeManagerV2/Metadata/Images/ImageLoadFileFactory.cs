﻿namespace Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Images
{
	using Castle.Windsor;

	using kCura.WinEDDS;

	using Relativity.DataExchange.Service;

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