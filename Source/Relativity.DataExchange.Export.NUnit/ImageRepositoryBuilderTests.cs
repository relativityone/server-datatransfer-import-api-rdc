﻿// -----------------------------------------------------------------------------------------------------
// <copyright file="ImageRepositoryBuilderTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;

	using global::NUnit.Framework;

	using kCura.WinEDDS.Exporters;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Download;
	using Relativity.DataExchange.Export.VolumeManagerV2.Repository;
	using Relativity.Logging;

	[TestFixture]
	public class ImageRepositoryBuilderTests
	{
		private ImageRepositoryBuilder _instance;

		private ImageRepository _imageRepository;

		private Mock<IExportRequestBuilder> _imageExportRequestBuilder;

		[SetUp]
		public void SetUp()
		{
			this._imageRepository = new ImageRepository();

			this._imageExportRequestBuilder = new Mock<IExportRequestBuilder>();

			this._instance = new ImageRepositoryBuilder(this._imageRepository, this._imageExportRequestBuilder.Object, new NullLogger());
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

			this._imageExportRequestBuilder.Setup(x => x.Create(artifact, CancellationToken.None)).Returns(new List<ExportRequest>());

			// ACT
			this._instance.AddToRepository(artifact, CancellationToken.None);

			// ASSERT
			Assert.That(this._imageRepository.GetImage(artifactId1, batesNumber).HasBeenDownloaded, Is.True);
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

			this._imageExportRequestBuilder.Setup(x => x.Create(artifact, CancellationToken.None)).Returns(new List<ExportRequest>
			{
				new PhysicalFileExportRequest(artifact, tempLocation)
			});

			// ACT
			this._instance.AddToRepository(artifact, CancellationToken.None);

			// ASSERT
			Assert.That(this._imageRepository.GetImage(artifactId1, batesNumber).HasBeenDownloaded, Is.False);
			CollectionAssert.IsNotEmpty(this._imageRepository.GetExportRequests());
			Assert.That(this._imageRepository.GetExportRequests().ToList()[0].ArtifactId, Is.EqualTo(artifactId1));
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

			IList<ExportRequest> exportRequests1 = new List<ExportRequest>
			{
				new PhysicalFileExportRequest(artifact1, "invalid_path"),
				new PhysicalFileExportRequest(artifact1, tempLocation2)
			};
			this._imageExportRequestBuilder.Setup(x => x.Create(artifact1, CancellationToken.None)).Returns(exportRequests1);
			IList<ExportRequest> exportRequests2 = new List<ExportRequest>
			{
				new PhysicalFileExportRequest(artifact2, tempLocation3)
			};
			this._imageExportRequestBuilder.Setup(x => x.Create(artifact2, CancellationToken.None)).Returns(exportRequests2);

			// ACT
			this._instance.AddToRepository(artifact1, CancellationToken.None);
			this._instance.AddToRepository(artifact2, CancellationToken.None);

			// ASSERT
			Image image1 = this._imageRepository.GetImage(artifactId1, bates1);
			Image image2 = this._imageRepository.GetImage(artifactId1, bates2);
			Image image3 = this._imageRepository.GetImage(artifactId2, bates3);

			Assert.That(image1.ExportRequest, Is.Null);

			Assert.That(image2.ExportRequest, Is.Not.Null);

			Assert.That(image3.ExportRequest, Is.Not.Null);
		}
	}
}