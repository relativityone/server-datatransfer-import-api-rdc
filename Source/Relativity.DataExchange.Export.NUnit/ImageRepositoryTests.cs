// -----------------------------------------------------------------------------------------------------
// <copyright file="ImageRepositoryTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System.Collections.Generic;

	using global::NUnit.Framework;

	using kCura.WinEDDS.Exporters;

	using Relativity.DataExchange;
	using Relativity.DataExchange.Export.VolumeManagerV2;
	using Relativity.DataExchange.Export.VolumeManagerV2.Download;
	using Relativity.DataExchange.Export.VolumeManagerV2.Repository;

	[TestFixture]
	public class ImageRepositoryTests
	{
		private ImageRepository _instance;

		private IList<Image> _images;

		[SetUp]
		public void SetUp()
		{
			this._images = this.CreateDataSet();

			this._instance = new ImageRepository();
			this._instance.Add(this._images);
		}

		[Test]
		public void ItShouldSearchImageByArtifactIdAndBatesNumber()
		{
			// ACT
			Image image1 = this._instance.GetImage(1, "bates_1");
			Image image1DiffCase = this._instance.GetImage(1, "BATES_1");
			Image image2 = this._instance.GetImage(1, "bates_2");
			Image image2DiffCase = this._instance.GetImage(1, "BATES_2");
			Image image3 = this._instance.GetImage(2, "bates_3");
			Image image3DiffCase = this._instance.GetImage(2, "BATES_3");
			Image image4DoesNotExist = this._instance.GetImage(99, "bates_99");

			// ASSERT
			Assert.That(image1, Is.EqualTo(this._images[0]));
			Assert.That(image1DiffCase, Is.EqualTo(this._images[0]));
			Assert.That(image2, Is.EqualTo(this._images[1]));
			Assert.That(image2DiffCase, Is.EqualTo(this._images[1]));
			Assert.That(image3, Is.EqualTo(this._images[2]));
			Assert.That(image3DiffCase, Is.EqualTo(this._images[2]));
			Assert.That(image4DoesNotExist, Is.Null);
		}

		[Test]
		public void ItShouldGetImagesForArtifact()
		{
			// ACT
			IList<Image> images1 = this._instance.GetArtifactImages(1);
			IList<Image> images2 = this._instance.GetArtifactImages(2);

			// ASSERT
			CollectionAssert.AreEquivalent(this._images.GetRange(0, 2), images1);
			CollectionAssert.AreEquivalent(this._images[2].InList(), images2);
		}

		[Test]
		public void ItShouldGetAllImages()
		{
			// ACT
			IList<Image> images = this._instance.GetImages();

			// ASSERT
			CollectionAssert.AreEquivalent(this._images, images);
		}

		[Test]
		public void ItShouldGetExportRequests()
		{
			var expectedExportRequests = new List<ExportRequest>
			{
				this._images[0].ExportRequest,
				this._images[2].ExportRequest
			};

			// ACT
			IEnumerable<ExportRequest> exportRequests = this._instance.GetExportRequests();

			// ASSERT
			CollectionAssert.AreEquivalent(expectedExportRequests, exportRequests);
		}

		[Test]
		public void ItShouldGetImageByTargetFile()
		{
			// ACT
			IList<Image> image1 = this._instance.GetImagesByTargetFile("a.txt");
			IList<Image> image2 = this._instance.GetImagesByTargetFile("b.txt");
			IList<Image> image3 = this._instance.GetImagesByTargetFile("c.txt");
			IList<Image> image4 = this._instance.GetImagesByTargetFile(null);
			IList<Image> image5 = this._instance.GetImagesByTargetFile(string.Empty);

			// ASSERT
			Assert.That(image1.Count, Is.EqualTo(1));
			Assert.That(image1[0], Is.EqualTo(this._images[0]));
			Assert.That(image2.Count, Is.Zero);
			Assert.That(image3.Count, Is.EqualTo(1));
			Assert.That(image3[0], Is.EqualTo(this._images[2]));
			Assert.That(image4.Count, Is.Zero);
			Assert.That(image5.Count, Is.Zero);
		}

		[Test]
		public void ItShouldClearRepository()
		{
			// ACT
			this._instance.Clear();
			IList<Image> images = this._instance.GetImages();

			// ASSERT
			CollectionAssert.IsEmpty(images);
		}

		private List<Image> CreateDataSet()
		{
			var artifact1 = new ImageExportInfo
			{
				ArtifactID = 1,
				BatesNumber = "bates_1"
			};
			var image1 = new Image(artifact1)
			{
				TransferCompleted = false,
				ExportRequest = new PhysicalFileExportRequest(artifact1, "a.txt")
				{
					FileName = "filename_1",
					Order = 1
				}
			};
			image1.ExportRequest.CreateTransferPath(1);

			var artifact2 = new ImageExportInfo
			{
				ArtifactID = 1,
				BatesNumber = "bates_2"
			};
			var image2 = new Image(artifact2)
			{
				TransferCompleted = true,
			};

			var artifact3 = new ImageExportInfo
			{
				ArtifactID = 2,
				BatesNumber = "bates_3"
			};
			var image3 = new Image(artifact3)
			{
				TransferCompleted = false,
				ExportRequest = new PhysicalFileExportRequest(artifact3, "c.txt")
				{
					FileName = "filename_3",
					Order = 3
				}
			};
			image3.ExportRequest.CreateTransferPath(3);

			return new List<Image> { image1, image2, image3 };
		}
	}
}