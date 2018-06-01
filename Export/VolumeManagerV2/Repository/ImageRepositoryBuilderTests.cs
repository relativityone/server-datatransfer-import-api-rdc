using System.Collections;
using System.Collections.Generic;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Repository;
using kCura.WinEDDS.Exporters;
using Moq;
using NUnit.Framework;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Repository
{
	[TestFixture]
	public class ImageRepositoryBuilderTests
	{
		private ImageRepositoryBuilder _instance;

		private ImageRepository _imageRepository;

		private Mock<IFileExportRequestBuilder> _imageExportRequestBuilder;

		[SetUp]
		public void SetUp()
		{
			_imageRepository = new ImageRepository();

			_imageExportRequestBuilder = new Mock<IFileExportRequestBuilder>();

			_instance = new ImageRepositoryBuilder(_imageRepository, _imageExportRequestBuilder.Object, new NullLogger());
		}

		[Test]
		public void ItShouldMarkArtifactAsDownloadedWhenNoExportRequestsExists()
		{
			const string batesNumber = "bates_1";
			const int artifactId1 = 1;

			ObjectExportInfo artifact = new ObjectExportInfo
			{
				ArtifactID = artifactId1,
				Images = new ArrayList
				{
					new ImageExportInfo
					{
						ArtifactID = artifactId1,
						BatesNumber = batesNumber
					}
				}
			};

			_imageExportRequestBuilder.Setup(x => x.Create(artifact, CancellationToken.None)).Returns(new List<FileExportRequest>());

			//ACT
			_instance.AddToRepository(artifact, CancellationToken.None);

			//ASSERT
			Assert.That(_imageRepository.GetImage(artifactId1, batesNumber).HasBeenDownloaded, Is.True);
		}

		[Test]
		public void ItShouldMarkArtifactAsNotDownloadedWhenExportRequestsExists()
		{
			const string batesNumber = "bates_1";
			const int artifactId1 = 1;
			const string tempLocation = "temp_location_1";

			ObjectExportInfo artifact = new ObjectExportInfo
			{
				ArtifactID = artifactId1,
				Images = new ArrayList
				{
					new ImageExportInfo
					{
						ArtifactID = artifactId1,
						BatesNumber = batesNumber,
						TempLocation = tempLocation
					}
				}
			};

			_imageExportRequestBuilder.Setup(x => x.Create(artifact, CancellationToken.None)).Returns(new List<FileExportRequest>
			{
				new NativeFileExportRequest(artifact, tempLocation)
			});

			//ACT
			_instance.AddToRepository(artifact, CancellationToken.None);

			//ASSERT
			Assert.That(_imageRepository.GetImage(artifactId1, batesNumber).HasBeenDownloaded, Is.False);
			CollectionAssert.IsNotEmpty(_imageRepository.GetExportRequests());
			Assert.That(_imageRepository.GetExportRequests()[0].ArtifactId, Is.EqualTo(artifactId1));
		}

		[Test]
		public void ItShouldAddNativeToRepository()
		{
			const int artifactId1 = 1;
			const int artifactId2 = 2;

			const string bates1 = "bates_1";
			const string bates2 = "bates_2";
			const string bates3 = "bates_3";

			const string tempLocation1 = "temp_location_1";
			const string tempLocation2 = "temp_location_2";
			const string tempLocation3 = "temp_location_3";

			ObjectExportInfo artifact1 = new ObjectExportInfo
			{
				ArtifactID = artifactId1,
				Images = new ArrayList
				{
					new ImageExportInfo
					{
						ArtifactID = artifactId1,
						BatesNumber = bates1,
						TempLocation = tempLocation1
					},
					new ImageExportInfo
					{
						ArtifactID = artifactId1,
						BatesNumber = bates2,
						TempLocation = tempLocation2
					}
				}
			};
			ObjectExportInfo artifact2 = new ObjectExportInfo
			{
				ArtifactID = artifactId2,
				Images = new ArrayList
				{
					new ImageExportInfo
					{
						ArtifactID = artifactId2,
						BatesNumber = bates3,
						TempLocation = tempLocation3
					}
				}
			};

			IList<FileExportRequest> exportRequests1 = new List<FileExportRequest>
			{
				new NativeFileExportRequest(artifact1, "invalid_path"),
				new NativeFileExportRequest(artifact1, tempLocation2)
			};
			_imageExportRequestBuilder.Setup(x => x.Create(artifact1, CancellationToken.None)).Returns(exportRequests1);
			IList<FileExportRequest> exportRequests2 = new List<FileExportRequest>
			{
				new NativeFileExportRequest(artifact2, tempLocation3)
			};
			_imageExportRequestBuilder.Setup(x => x.Create(artifact2, CancellationToken.None)).Returns(exportRequests2);

			//ACT
			_instance.AddToRepository(artifact1, CancellationToken.None);
			_instance.AddToRepository(artifact2, CancellationToken.None);

			//ASSERT
			Image image1 = _imageRepository.GetImage(artifactId1, bates1);
			Image image2 = _imageRepository.GetImage(artifactId1, bates2);
			Image image3 = _imageRepository.GetImage(artifactId2, bates3);

			Assert.That(image1.ExportRequest, Is.Null);

			Assert.That(image2.ExportRequest, Is.Not.Null);

			Assert.That(image3.ExportRequest, Is.Not.Null);
		}
	}
}