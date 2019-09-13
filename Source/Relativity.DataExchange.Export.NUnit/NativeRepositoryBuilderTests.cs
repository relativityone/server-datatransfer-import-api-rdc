// -----------------------------------------------------------------------------------------------------
// <copyright file="NativeRepositoryBuilderTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;

	using global::NUnit.Framework;

	using kCura.WinEDDS.Exporters;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Directories;
	using Relativity.DataExchange.Export.VolumeManagerV2.Download;
	using Relativity.DataExchange.Export.VolumeManagerV2.Repository;
	using Relativity.Logging;

	[TestFixture]
	public class NativeRepositoryBuilderTests
	{
		private NativeRepositoryBuilder _instance;
		private NativeRepository _nativeRepository;
		private Mock<ILabelManagerForArtifact> _labelManager;
		private Mock<IExportRequestBuilder> _exportRequestBuilder;

		[SetUp]
		public void SetUp()
		{
			this._nativeRepository = new NativeRepository();

			this._labelManager = new Mock<ILabelManagerForArtifact>();
			this._exportRequestBuilder = new Mock<IExportRequestBuilder>();

			this._instance = new NativeRepositoryBuilder(this._nativeRepository, this._labelManager.Object, this._exportRequestBuilder.Object, new NullLogger());
		}

		[Test]
		public void ItShouldSetDestinationVolume()
		{
			const string volumeLabel = "volume_label";

			this._labelManager.Setup(x => x.GetVolumeLabel(It.IsAny<int>())).Returns(volumeLabel);
			this._exportRequestBuilder.Setup(x => x.Create(It.IsAny<ObjectExportInfo>(), CancellationToken.None)).Returns(new List<ExportRequest>());

			ObjectExportInfo artifact = new ObjectExportInfo();

			// ACT
			this._instance.AddToRepository(artifact, CancellationToken.None);

			// ASSERT
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

			this._exportRequestBuilder.Setup(x => x.Create(artifact, CancellationToken.None)).Returns(new List<ExportRequest>());

			// ACT
			this._instance.AddToRepository(artifact, CancellationToken.None);

			// ASSERT
			Assert.That(this._nativeRepository.GetNative(artifactId1).TransferCompleted, Is.True);
		}

		[Test]
		public void ItShouldMarkArtifactAsNotDownloadedWhenExportRequestsExists()
		{
			const int artifactId1 = 1;
			ObjectExportInfo artifact = new ObjectExportInfo
			{
				ArtifactID = artifactId1
			};

			this._exportRequestBuilder.Setup(x => x.Create(artifact, CancellationToken.None)).Returns(new List<ExportRequest>
			{
				new PhysicalFileExportRequest(artifact, string.Empty)
			});

			// ACT
			this._instance.AddToRepository(artifact, CancellationToken.None);

			// ASSERT
			Assert.That(this._nativeRepository.GetNative(artifactId1).TransferCompleted, Is.False);
			CollectionAssert.IsNotEmpty(this._nativeRepository.GetExportRequests());
			Assert.That(this._nativeRepository.GetExportRequests().ToList()[0].ArtifactId, Is.EqualTo(artifact.ArtifactID));
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

			this._exportRequestBuilder.Setup(x => x.Create(artifact1, CancellationToken.None)).Returns(new List<ExportRequest>
			{
				new PhysicalFileExportRequest(artifact1, string.Empty)
			});
			this._exportRequestBuilder.Setup(x => x.Create(artifact2, CancellationToken.None)).Returns(new List<ExportRequest>());

			// ACT
			this._instance.AddToRepository(artifact1, CancellationToken.None);
			this._instance.AddToRepository(artifact2, CancellationToken.None);

			// ASSERT
			Native native1 = this._nativeRepository.GetNative(artifactId1);
			Native native2 = this._nativeRepository.GetNative(artifactId2);

			Assert.That(native1.Artifact.ArtifactID, Is.EqualTo(artifactId1));
			Assert.That(native1.ExportRequest, Is.Not.Null);

			Assert.That(native2.Artifact.ArtifactID, Is.EqualTo(artifactId2));
			Assert.That(native2.ExportRequest, Is.Null);
		}
	}
}