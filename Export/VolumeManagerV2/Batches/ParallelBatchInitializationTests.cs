using System.Linq;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Batches;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using Moq;
using NUnit.Framework;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Batches
{
	[TestFixture]
	public class ParallelBatchInitializationTests: BatchInitializationTests
	{
		protected Mock<ILabelManagerForArtifact> LabelManagerForArtifact { get; private set; }

		protected override IBatchInitialization CreateBatchInitialization()
		{
			LabelManagerForArtifact = new Mock<ILabelManagerForArtifact>();

			return new ParallelBatchInitialization(RepositoryBuilderMocks.Select(x => x.Object).ToList(), LabelManagerForArtifact.Object, new NullLogger());
		}

		[Ignore("Not valid for parallel initialization")]
		public override void ItShouldUpdateDirectoryManagerForEachArtifact()
		{
		}

		[Test]
		public void ItShouldInitializeLabelManagerForArtifact()
		{
			//ACT
			Instance.PrepareBatch(Artifacts, VolumePredictions, CancellationToken.None);

			//ASSERT
			LabelManagerForArtifact.Verify(lm => lm.InitializeFor(Artifacts, VolumePredictions, CancellationToken.None), Times.Once);
		}
	}
}