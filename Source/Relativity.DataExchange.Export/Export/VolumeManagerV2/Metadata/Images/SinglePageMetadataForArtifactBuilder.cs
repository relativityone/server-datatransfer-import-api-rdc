﻿namespace Relativity.Export.VolumeManagerV2.Metadata.Images
{
	using System.Collections.Generic;
	using System.Linq;

	using Relativity.Logging;
	using Relativity.Export.VolumeManagerV2.Directories;
	using Relativity.Export.VolumeManagerV2.Metadata.Images.Lines;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;

	public class SinglePageMetadataForArtifactBuilder : ImageLoadFileMetadataForArtifactBuilder
	{
		public SinglePageMetadataForArtifactBuilder(ExportFile exportSettings, IFilePathTransformer filePathTransformer, IImageLoadFileEntry imageLoadFileEntry,
			IFullTextLoadFileEntry fullTextLoadFileEntry, ILog logger) : base(exportSettings, filePathTransformer, imageLoadFileEntry, fullTextLoadFileEntry, logger)
		{
		}

		protected override List<ImageExportInfo> GetImagesToProcess(ObjectExportInfo artifact)
		{
			return artifact.Images.Cast<ImageExportInfo>().ToList();
		}

		protected override int GetBaseImageIndex(int i)
		{
			return i;
		}
	}
}