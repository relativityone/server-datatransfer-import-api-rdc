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
		protected IBatchInitialization Instance { get; private set; }

		protected Mock<IDirectoryManager> DirectoryManager { get; private set; }
		protected IList<Mock<IRepositoryBuilder>> RepositoryBuilderMocks { get; private set; }
		protected VolumePredictions[] VolumePredictions { get; private set; }
		protected ObjectExportInfo[] Artifacts { get; private set; }

		[SetUp]
		public void SetUp()
		{
			Artifacts = new[] {new ObjectExportInfo(), new ObjectExportInfo(), new ObjectExportInfo()};
			VolumePredictions = new[] {new VolumePredictions(), new VolumePredictions(), new VolumePredictions()};

			RepositoryBuilderMocks = new List<Mock<IRepositoryBuilder>>
			{
				new Mock<IRepositoryBuilder>(),
				new Mock<IRepositoryBuilder>(),
				new Mock<IRepositoryBuilder>()
			};
			DirectoryManager = new Mock<IDirectoryManager>();

			Instance = CreateBatchInitialization();
		}

		protected virtual IBatchInitialization CreateBatchInitialization()
		{
			return new BatchInitialization(RepositoryBuilderMocks.Select(x => x.Object).ToList(), DirectoryManager.Object, new NullLogger());
		}

		[Test]
		public virtual void ItShouldUpdateDirectoryManagerForEachArtifact()
		{
			//ACT
			Instance.PrepareBatch(Artifacts, VolumePredictions, CancellationToken.None);

			//ASSERT
			VolumePredictions.ForEach(x => DirectoryManager.Verify(dm => dm.MoveNext(x), Times.Once));
		}

		[Test]
		public void ItShouldAddAllArtifactsToRepositories()
		{
			//ACT
			Instance.PrepareBatch(Artifacts, VolumePredictions, CancellationToken.None);

			//ASSERT
			foreach (var artifact in Artifacts)
			{
				foreach (var repositoryBuilderMock in RepositoryBuilderMocks)
				{
					repositoryBuilderMock.Verify(x => x.AddToRepository(artifact, CancellationToken.None), Times.Once);
				}
			}
		}
	}
}