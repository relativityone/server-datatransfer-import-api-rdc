using System.Collections;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images.Lines;
using kCura.WinEDDS.Exporters;
using Moq;
using NUnit.Framework;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Metadata.Images
{
	[TestFixture]
	public class MultiPageMetadataForArtifactBuilderTests : ImageLoadFileMetadataForArtifactBuilderTests
	{
		protected override ImageLoadFileMetadataForArtifactBuilder CreateInstance(ExportFile exportSettings, IFilePathTransformer filePathTransformer,
			IImageLoadFileEntry imageLoadFileEntry,
			IFullTextLoadFileEntry fullTextLoadFileEntry)
		{
			return new MultiPageOpticonMetadataForArtifactBuilder(exportSettings, filePathTransformer, imageLoadFileEntry, fullTextLoadFileEntry, new NullLogger());
		}

		[Test]
		public void ItShouldCalculatePageOffset()
		{
			ImageExportInfo image1 = new ImageExportInfo
			{
				BatesNumber = "image1",
				PageOffset = 100
			};
			ImageExportInfo image2 = new ImageExportInfo
			{
				BatesNumber = "image2",
				PageOffset = 200
			};
			ImageExportInfo image3 = new ImageExportInfo
			{
				BatesNumber = "image3",
				PageOffset = 300
			};
			ObjectExportInfo artifact = new ObjectExportInfo
			{
				Images = new ArrayList
				{
					image1,
					image2,
					image3
				}
			};

			//ACT
			Instance.WriteLoadFileEntry(artifact, Writer.Object, CancellationToken.None);

			//ASSERT
			FullTextLoadFileEntry.Verify(x => x.WriteFullTextLine(artifact, image1.BatesNumber, 0, long.MinValue, Writer.Object, CancellationToken.None), Times.Once);
			FullTextLoadFileEntry.Verify(x => x.WriteFullTextLine(artifact, image2.BatesNumber, 1, (long) image3.PageOffset, Writer.Object, CancellationToken.None), Times.Never());
			FullTextLoadFileEntry.Verify(x => x.WriteFullTextLine(artifact, image3.BatesNumber, 2, long.MinValue, Writer.Object, CancellationToken.None), Times.Never());
		}

		[Test]
		public void ItShouldHandleEmptyPageOffset()
		{
			ImageExportInfo image1 = new ImageExportInfo
			{
				BatesNumber = "image1",
				PageOffset = null
			};
			ImageExportInfo image2 = new ImageExportInfo
			{
				BatesNumber = "image2",
				PageOffset = null
			};
			ObjectExportInfo artifact = new ObjectExportInfo
			{
				Images = new ArrayList {image1, image2}
			};

			//ACT
			Instance.WriteLoadFileEntry(artifact, Writer.Object, CancellationToken.None);

			//ASSERT
			FullTextLoadFileEntry.Verify(x => x.WriteFullTextLine(artifact, image1.BatesNumber, 0, long.MinValue, Writer.Object, CancellationToken.None), Times.Once);
			FullTextLoadFileEntry.Verify(x => x.WriteFullTextLine(artifact, image2.BatesNumber, 1, long.MinValue, Writer.Object, CancellationToken.None), Times.Never());
		}
	}
}