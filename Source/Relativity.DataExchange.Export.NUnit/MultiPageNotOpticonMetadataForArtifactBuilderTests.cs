// -----------------------------------------------------------------------------------------------------
// <copyright file="MultiPageNotOpticonMetadataForArtifactBuilderTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System.Collections;
	using System.Threading;

	using global::NUnit.Framework;

	using kCura.WinEDDS.Exporters;

	using Moq;

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

			this.FilePathTransformer.Setup(x => x.TransformPath(It.IsAny<string>())).Returns((string s) => $"{s}_transformed");
			this.ImageLoadFileEntry.Setup(x => x.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).Returns(loadFileEntry);

			// ACT
			this.Instance.WriteLoadFileEntry(artifact, this.Writer.Object, CancellationToken.None);

			// ASSERT
			this.FullTextLoadFileEntry.Verify(x => x.WriteFullTextLine(artifact, image1.BatesNumber, 0, It.IsAny<long>(), this.Writer.Object, CancellationToken.None), Times.Once);
			this.ImageLoadFileEntry.Verify(x => x.Create(image1.BatesNumber, $"{image1.TempLocation}_transformed", artifact.DestinationVolume, 1, numberOfImages), Times.Once);

			this.FullTextLoadFileEntry.Verify(x => x.WriteFullTextLine(artifact, image2.BatesNumber, 1, It.IsAny<long>(), this.Writer.Object, CancellationToken.None), Times.Never);
			this.ImageLoadFileEntry.Verify(x => x.Create(image2.BatesNumber, $"{image1.TempLocation}_transformed", artifact.DestinationVolume, 2, numberOfImages), Times.Never);

			this.FullTextLoadFileEntry.Verify(x => x.WriteFullTextLine(artifact, image3.BatesNumber, 2, It.IsAny<long>(), this.Writer.Object, CancellationToken.None), Times.Never);
			this.ImageLoadFileEntry.Verify(x => x.Create(image3.BatesNumber, $"{image1.TempLocation}_transformed", artifact.DestinationVolume, 3, numberOfImages), Times.Never);

			this.Writer.Verify(x => x.WriteEntry(loadFileEntry, CancellationToken.None), Times.Once);
		}
	}
}