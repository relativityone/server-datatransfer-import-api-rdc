// -----------------------------------------------------------------------------------------------------
// <copyright file="SinglePageMetadataForArtifactBuilderTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System.Collections;
	using System.Threading;

	using global::NUnit.Framework;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Directories;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Images;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Images.Lines;
	using Relativity.Logging;

	[TestFixture]
	public class SinglePageMetadataForArtifactBuilderTests : ImageLoadFileMetadataForArtifactBuilderTests
	{
		protected override ImageLoadFileMetadataForArtifactBuilder CreateInstance(
			ExportFile exportSettings,
			IFilePathTransformer filePathTransformer,
			IImageLoadFileEntry imageLoadFileEntry,
			IFullTextLoadFileEntry fullTextLoadFileEntry)
		{
			return new SinglePageMetadataForArtifactBuilder(exportSettings, filePathTransformer, imageLoadFileEntry, fullTextLoadFileEntry, new NullLogger());
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

			// ACT
			this.Instance.WriteLoadFileEntry(artifact, this.Writer.Object, CancellationToken.None);

			// ASSERT
			this.FullTextLoadFileEntry.Verify(x => x.WriteFullTextLine(artifact, image1.BatesNumber, 0, (long)image2.PageOffset, this.Writer.Object, CancellationToken.None), Times.Once);
			this.FullTextLoadFileEntry.Verify(x => x.WriteFullTextLine(artifact, image2.BatesNumber, 1, (long)image3.PageOffset, this.Writer.Object, CancellationToken.None), Times.Once());
			this.FullTextLoadFileEntry.Verify(x => x.WriteFullTextLine(artifact, image3.BatesNumber, 2, long.MinValue, this.Writer.Object, CancellationToken.None), Times.Once());
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
			ObjectExportInfo artifact = new ObjectExportInfo { Images = new ArrayList { image1, image2 } };

			// ACT
			this.Instance.WriteLoadFileEntry(artifact, this.Writer.Object, CancellationToken.None);

			// ASSERT
			this.FullTextLoadFileEntry.Verify(x => x.WriteFullTextLine(artifact, image1.BatesNumber, 0, long.MinValue, this.Writer.Object, CancellationToken.None), Times.Once);
			this.FullTextLoadFileEntry.Verify(x => x.WriteFullTextLine(artifact, image2.BatesNumber, 1, long.MinValue, this.Writer.Object, CancellationToken.None), Times.Once());
		}

		[Test]
		public void ItShouldCreateEntriesForAllImages()
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

			this.FullTextLoadFileEntry.Verify(x => x.WriteFullTextLine(artifact, image2.BatesNumber, 1, It.IsAny<long>(), this.Writer.Object, CancellationToken.None), Times.Once);
			this.ImageLoadFileEntry.Verify(x => x.Create(image2.BatesNumber, null, artifact.DestinationVolume, 2, numberOfImages), Times.Once);

			this.FullTextLoadFileEntry.Verify(x => x.WriteFullTextLine(artifact, image3.BatesNumber, 2, It.IsAny<long>(), this.Writer.Object, CancellationToken.None), Times.Once);
			this.ImageLoadFileEntry.Verify(x => x.Create(image3.BatesNumber, $"{image3.TempLocation}_transformed", artifact.DestinationVolume, 3, numberOfImages), Times.Once);

			this.Writer.Verify(x => x.WriteEntry(loadFileEntry, CancellationToken.None), Times.Exactly(numberOfImages));
		}
	}
}