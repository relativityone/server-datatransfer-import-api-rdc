using System.Collections.Generic;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Repository;
using kCura.WinEDDS.Exporters;
using Moq;
using NUnit.Framework;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Repository
{
	[TestFixture]
	public class NativeRepositoryBuilderTests
	{
		private NativeRepositoryBuilder _instance;

		private NativeRepository _nativeRepository;

		private Mock<ILabelManager> _labelManager;
		private Mock<IFileExportRequestBuilder> _fileExportRequestBuilder;

		[SetUp]
		public void SetUp()
		{
			_nativeRepository = new NativeRepository();

			_labelManager = new Mock<ILabelManager>();
			_fileExportRequestBuilder = new Mock<IFileExportRequestBuilder>();

			_instance = new NativeRepositoryBuilder(_nativeRepository, _labelManager.Object, _fileExportRequestBuilder.Object, new NullLogger());
		}

		[Test]
		public void ItShouldSetDestinationVolume()
		{
			const string volumeLabel = "volume_label";

			_labelManager.Setup(x => x.GetCurrentVolumeLabel()).Returns(volumeLabel);
			_fileExportRequestBuilder.Setup(x => x.Create(It.IsAny<ObjectExportInfo>(), CancellationToken.None)).Returns(new List<FileExportRequest>());

			ObjectExportInfo artifact = new ObjectExportInfo();

			//ACT
			_instance.AddToRepository(artifact, CancellationToken.None);

			//ASSERT
			Assert.That(artifact.DestinationVolume, Is.EqualTo(volumeLabel));
		}

		[Test]
		public void ItShouldMarkArtifactAsDownloadedWhenNoExportRequestsExists()
		{
			const int artifactId1 = 1;
			ObjectExportInfo artifact = new ObjectExportInfo
			{
				ArtifactID = artifactId1
			};

			_fileExportRequestBuilder.Setup(x => x.Create(artifact, CancellationToken.None)).Returns(new List<FileExportRequest>());

			//ACT
			_instance.AddToRepository(artifact, CancellationToken.None);

			//ASSERT
			Assert.That(_nativeRepository.GetNative(artifactId1).HasBeenDownloaded, Is.True);
		}

		[Test]
		public void ItShouldMarkArtifactAsNotDownloadedWhenExportRequestsExists()
		{
			const int artifactId1 = 1;
			ObjectExportInfo artifact = new ObjectExportInfo
			{
				ArtifactID = artifactId1
			};

			_fileExportRequestBuilder.Setup(x => x.Create(artifact, CancellationToken.None)).Returns(new List<FileExportRequest>
			{
				new NativeFileExportRequest(artifact, "")
			});

			//ACT
			_instance.AddToRepository(artifact, CancellationToken.None);

			//ASSERT
			Assert.That(_nativeRepository.GetNative(artifactId1).HasBeenDownloaded, Is.False);
			CollectionAssert.IsNotEmpty(_nativeRepository.GetExportRequests());
			Assert.That(_nativeRepository.GetExportRequests()[0].ArtifactId, Is.EqualTo(artifact.ArtifactID));
		}

		[Test]
		public void ItShouldAddNativeToRepository()
		{
			const int artifactId1 = 1;
			const int artifactId2 = 2;
			ObjectExportInfo artifact1 = new ObjectExportInfo
			{
				ArtifactID = artifactId1
			};
			ObjectExportInfo artifact2 = new ObjectExportInfo
			{
				ArtifactID = artifactId2
			};

			_fileExportRequestBuilder.Setup(x => x.Create(artifact1, CancellationToken.None)).Returns(new List<FileExportRequest>
			{
				new NativeFileExportRequest(artifact1, "")
			});
			_fileExportRequestBuilder.Setup(x => x.Create(artifact2, CancellationToken.None)).Returns(new List<FileExportRequest>());

			//ACT
			_instance.AddToRepository(artifact1, CancellationToken.None);
			_instance.AddToRepository(artifact2, CancellationToken.None);

			//ASSERT
			Native native1 = _nativeRepository.GetNative(artifactId1);
			Native native2 = _nativeRepository.GetNative(artifactId2);

			Assert.That(native1.Artifact.ArtifactID, Is.EqualTo(artifactId1));
			Assert.That(native1.ExportRequest, Is.Not.Null);

			Assert.That(native2.Artifact.ArtifactID, Is.EqualTo(artifactId2));
			Assert.That(native2.ExportRequest, Is.Null);
		}
	}
}