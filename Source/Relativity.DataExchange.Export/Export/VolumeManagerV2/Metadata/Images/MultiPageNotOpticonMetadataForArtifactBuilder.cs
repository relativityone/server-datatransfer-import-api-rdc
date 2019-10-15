namespace Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Images
{
	using System.Collections.Generic;
	using System.Linq;

	using Relativity.Logging;
	using Relativity.DataExchange.Export.VolumeManagerV2.Directories;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Images.Lines;

	using kCura.WinEDDS.Exporters;
	using kCura.WinEDDS;

	public class MultiPageNotOpticonMetadataForArtifactBuilder : ImageLoadFileMetadataForArtifactBuilder
	{
		public MultiPageNotOpticonMetadataForArtifactBuilder(ExportFile exportSettings, IFilePathTransformer filePathTransformer, IImageLoadFileEntry imageLoadFileEntry,
			IFullTextLoadFileEntry fullTextLoadFileEntry, ILog logger) : base(exportSettings, filePathTransformer, imageLoadFileEntry, fullTextLoadFileEntry, logger)
		{
		}

		protected override List<ImageExportInfo> GetImagesToProcess(ObjectExportInfo artifact)
		{
			return artifact.Images;
		}

		protected override int GetBaseImageIndex(int i)
		{
			return 0;
		}
	}
}