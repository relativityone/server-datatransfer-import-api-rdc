// -----------------------------------------------------------------------------------------------------
// <copyright file="BatchInitializationTests.cs" company="Relativity ODA LLC">
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

	using Relativity.DataExchange.Export.VolumeManagerV2.Batches;
	using Relativity.DataExchange.Export.VolumeManagerV2.Directories;
	using Relativity.DataExchange.Export.VolumeManagerV2.Repository;
	using Relativity.Logging;

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
			this.Artifacts = new[] { new ObjectExportInfo(), new ObjectExportInfo(), new ObjectExportInfo() };
			this.VolumePredictions = new[] { new VolumePredictions(), new VolumePredictions(), new VolumePredictions() };

			this.RepositoryBuilderMocks = new List<Mock<IRepositoryBuilder>>
			{
				new Mock<IRepositoryBuilder>(),
				new Mock<IRepositoryBuilder>(),
				new Mock<IRepositoryBuilder>()
			};
			this.DirectoryManager = new Mock<IDirectoryManager>();

			this.Instance = this.CreateBatchInitialization();
		}

		protected virtual IBatchInitialization CreateBatchInitialization()
		{
			return new BatchInitialization(this.RepositoryBuilderMocks.Select(x => x.Object).ToList(), this.DirectoryManager.Object, new NullLogger());
		}

		[Test]
		public virtual void ItShouldUpdateDirectoryManagerForEachArtifact()
		{
			// ACT
			this.Instance.PrepareBatch(this.Artifacts, this.VolumePredictions, CancellationToken.None);

			// ASSERT
			this.VolumePredictions.ToList().ForEach(x => this.DirectoryManager.Verify(dm => dm.MoveNext(x), Times.Once));
		}

		[Test]
		public void ItShouldAddAllArtifactsToRepositories()
		{
			// ACT
			this.Instance.PrepareBatch(this.Artifacts, this.VolumePredictions, CancellationToken.None);

			// ASSERT
			foreach (var artifact in this.Artifacts)
			{
				foreach (var repositoryBuilderMock in this.RepositoryBuilderMocks)
				{
					repositoryBuilderMock.Verify(x => x.AddToRepository(artifact, CancellationToken.None), Times.Once);
				}
			}
		}
	}
}