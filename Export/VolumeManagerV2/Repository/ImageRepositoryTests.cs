using System.Collections.Generic;
using kCura.Utility.Extensions;
using kCura.WinEDDS.Core.Export.VolumeManagerV2;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Repository;
using kCura.WinEDDS.Exporters;
using NUnit.Framework;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Repository
{
	[TestFixture]
	public class ImageRepositoryTests
	{
		private ImageRepository _instance;

		private IList<Image> _images;

		[SetUp]
		public void SetUp()
		{
			_images = CreateDataSet();

			_instance = new ImageRepository();
			_instance.Add(_images);
		}

		[Test]
		public void ItShouldSearchImageByArtifactIdAndBatesNumber()
		{
			//ACT
			Image image1 = _instance.GetImage(1, "bates_1");
			Image image2 = _instance.GetImage(1, "bates_2");
			Image image3 = _instance.GetImage(2, "bates_3");

			//ASSERT
			Assert.That(image1, Is.EqualTo(_images[0]));
			Assert.That(image2, Is.EqualTo(_images[1]));
			Assert.That(image3, Is.EqualTo(_images[2]));
		}

		[Test]
		public void ItShouldGetImagesForArtifact()
		{
			//ACT
			IList<Image> images1 = _instance.GetArtifactImages(1);
			IList<Image> images2 = _instance.GetArtifactImages(2);

			//ASSERT
			CollectionAssert.AreEquivalent(_images.GetRange(0, 2), images1);
			CollectionAssert.AreEquivalent(_images[2].InList(), images2);
		}

		[Test]
		public void ItShouldGetAllImages()
		{
			//ACT
			IList<Image> images = _instance.GetImages();

			//ASSERT
			CollectionAssert.AreEquivalent(_images, images);
		}

		[Test]
		public void ItShouldGetExportRequests()
		{
			var expectedExportRequests = new List<ExportRequest>
			{
				_images[0].ExportRequest,
				_images[2].ExportRequest
			};

			//ACT
			IList<ExportRequest> exportRequests = _instance.GetExportRequests();

			//ASSERT
			CollectionAssert.AreEquivalent(expectedExportRequests, exportRequests);
		}

		[Test]
		public void ItShouldGetImageByUniqueId()
		{
			//ACT
			Image image1 = _instance.GetByUniqueId("unique_1");
			Image image2 = _instance.GetByUniqueId("unique_2");
			Image image3 = _instance.GetByUniqueId("unique_3");

			//ASSERT
			Assert.That(image1, Is.EqualTo(_images[0]));
			Assert.That(image2, Is.Null);
			Assert.That(image3, Is.EqualTo(_images[2]));
		}

		[Test]
		public void ItShouldClearRepository()
		{
			//ACT
			_instance.Clear();
			IList<Image> images = _instance.GetImages();

			//ASSERT
			CollectionAssert.IsEmpty(images);
		}

		#region DataSet

		private List<Image> CreateDataSet()
		{
			ImageExportInfo artifact1 = new ImageExportInfo
			{
				ArtifactID = 1,
				BatesNumber = "bates_1"
			};
			Image image1 = new Image(artifact1)
			{
				HasBeenDownloaded = false,
				ExportRequest = new NativeFileExportRequest(artifact1, "")
				{
					UniqueId = "unique_1"
				}
			};

			ImageExportInfo artifact2 = new ImageExportInfo
			{
				ArtifactID = 1,
				BatesNumber = "bates_2"
			};
			Image image2 = new Image(artifact2)
			{
				HasBeenDownloaded = true
			};

			ImageExportInfo artifact3 = new ImageExportInfo
			{
				ArtifactID = 2,
				BatesNumber = "bates_3"
			};
			Image image3 = new Image(artifact3)
			{
				HasBeenDownloaded = false,
				ExportRequest = new NativeFileExportRequest(artifact3, "")
				{
					UniqueId = "unique_3"
				}
			};

			return new List<Image> {image1, image2, image3};
		}

		#endregion
	}
}