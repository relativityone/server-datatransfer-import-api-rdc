using System.Collections;
using System.Threading;
using kCura.WinEDDS.Exporters;
using Moq;
using NUnit.Framework;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Metadata.Images
{
	[TestFixture]
	public class MultiPageNotOpticonMetadataForArtifactBuilderTests : MultiPageMetadataForArtifactBuilderTests
	{
		[Test]
		public void ItShouldCreateEntriesForFirstImageOnly()
		{
			const int numberOfImages = 3;
			const string loadFileEntry = "fileEntry";

			ImageExportInfo image1 = new ImageExportInfo
			{
				BatesNumber = "image1",
				TempLocation = "temp_location_1"
			};
			ImageExportInfo image2 = new ImageExportInfo
			{
				BatesNumber = "image2",
				TempLocation = null
			};
			ImageExportInfo image3 = new ImageExportInfo
			{
				BatesNumber = "image3",
				TempLocation = "temp_location_3"
			};
			ObjectExportInfo artifact = new ObjectExportInfo
			{
				Images = new ArrayList
				{
					image1,
					image2,
					image3
				},
				DestinationVolume = "VOL0001"
			};

			FilePathTransformer.Setup(x => x.TransformPath(It.IsAny<string>())).Returns((string s) => $"{s}_transformed");
			ImageLoadFileEntry.Setup(x => x.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).Returns(loadFileEntry);

			//ACT
			Instance.WriteLoadFileEntry(artifact, Writer.Object, CancellationToken.None);

			//ASSERT
			FullTextLoadFileEntry.Verify(x => x.WriteFullTextLine(artifact, image1.BatesNumber, 0, It.IsAny<long>(), Writer.Object, CancellationToken.None), Times.Once);
			ImageLoadFileEntry.Verify(x => x.Create(image1.BatesNumber, $"{image1.TempLocation}_transformed", artifact.DestinationVolume, 1, numberOfImages), Times.Once);

			FullTextLoadFileEntry.Verify(x => x.WriteFullTextLine(artifact, image2.BatesNumber, 1, It.IsAny<long>(), Writer.Object, CancellationToken.None), Times.Never);
			ImageLoadFileEntry.Verify(x => x.Create(image2.BatesNumber, $"{image1.TempLocation}_transformed", artifact.DestinationVolume, 2, numberOfImages), Times.Never);

			FullTextLoadFileEntry.Verify(x => x.WriteFullTextLine(artifact, image3.BatesNumber, 2, It.IsAny<long>(), Writer.Object, CancellationToken.None), Times.Never);
			ImageLoadFileEntry.Verify(x => x.Create(image3.BatesNumber, $"{image1.TempLocation}_transformed", artifact.DestinationVolume, 3, numberOfImages), Times.Never);

			Writer.Verify(x => x.WriteEntry(loadFileEntry, CancellationToken.None), Times.Once);
		}
	}
}