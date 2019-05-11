namespace Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Images
{
	using System.Collections.Generic;

	using Relativity.Logging;
	using Relativity.DataExchange.Export.VolumeManagerV2.Directories;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Images.Lines;

	using kCura.WinEDDS.Exporters;
	using kCura.WinEDDS;

	public class MultiPageOpticonMetadataForArtifactBuilder : ImageLoadFileMetadataForArtifactBuilder
	{
		public MultiPageOpticonMetadataForArtifactBuilder(ExportFile exportSettings, IFilePathTransformer filePathTransformer, IImageLoadFileEntry imageLoadFileEntry,
			IFullTextLoadFileEntry fullTextLoadFileEntry, ILog logger) : base(exportSettings, filePathTransformer, imageLoadFileEntry, fullTextLoadFileEntry, logger)
		{
		}

		protected override List<ImageExportInfo> GetImagesToProcess(ObjectExportInfo artifact)
		{
			if (artifact.Images == null || artifact.Images.Count == 0)
			{
				return new List<ImageExportInfo>();
			}

			//Opticon file should have only one entry for all pages
			return new List<ImageExportInfo> {(ImageExportInfo) artifact.Images[0]};
		}

		protected override int GetBaseImageIndex(int i)
		{
			return i;
		}
	}
}