using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Castle.Core.Internal;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Batches;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Repository;
using kCura.WinEDDS.Exporters;
using Moq;
using NUnit.Framework;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Batches
{
	[TestFixture]
	public class BatchInitializationTests
	{
		private BatchInitialization _instance;

		private Mock<IDirectoryManager> _directoryManager;
		private IList<Mock<IRepositoryBuilder>> _repositoryBuilderMocks;
		private VolumePredictions[] _volumePredictions;
		private ObjectExportInfo[] _artifacts;

		[SetUp]
		public void SetUp()
		{
			_artifacts = new[] {new ObjectExportInfo(), new ObjectExportInfo(), new ObjectExportInfo()};
			_volumePredictions = new[] {new VolumePredictions(), new VolumePredictions(), new VolumePredictions()};

			_repositoryBuilderMocks = new List<Mock<IRepositoryBuilder>>
			{
				new Mock<IRepositoryBuilder>(),
				new Mock<IRepositoryBuilder>(),
				new Mock<IRepositoryBuilder>()
			};
			_directoryManager = new Mock<IDirectoryManager>();
			_instance = new BatchInitialization(_repositoryBuilderMocks.Select(x => x.Object).ToList(), _directoryManager.Object, new NullLogger());
		}

		[Test]
		public void ItShouldUpdateDirectoryManagerForEachArtifact()
		{
			//ACT
			_instance.PrepareBatch(_artifacts, _volumePredictions, CancellationToken.None);

			//ASSERT
			_volumePredictions.ForEach(x => _directoryManager.Verify(dm => dm.MoveNext(x), Times.Once));
		}

		[Test]
		public void ItShouldAddAllArtifactsToRepositories()
		{
			//ACT
			_instance.PrepareBatch(_artifacts, _volumePredictions, CancellationToken.None);

			//ASSERT
			foreach (var artifact in _artifacts)
			{
				foreach (var repositoryBuilderMock in _repositoryBuilderMocks)
				{
					repositoryBuilderMock.Verify(x => x.AddToRepository(artifact, CancellationToken.None), Times.Once);
				}
			}
		}
	}
}