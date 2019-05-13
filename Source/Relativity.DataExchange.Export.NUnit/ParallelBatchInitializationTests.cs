// -----------------------------------------------------------------------------------------------------
// <copyright file="ParallelBatchInitializationTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System.Linq;
	using System.Threading;

	using global::NUnit.Framework;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Batches;
	using Relativity.DataExchange.Export.VolumeManagerV2.Directories;
	using Relativity.Logging;

	[TestFixture]
	public class ParallelBatchInitializationTests : BatchInitializationTests
	{
		protected Mock<ILabelManagerForArtifact> LabelManagerForArtifact { get; private set; }

		protected override IBatchInitialization CreateBatchInitialization()
		{
			this.LabelManagerForArtifact = new Mock<ILabelManagerForArtifact>();

			return new ParallelBatchInitialization(this.RepositoryBuilderMocks.Select(x => x.Object).ToList(), this.LabelManagerForArtifact.Object, new NullLogger());
		}

		[Ignore("Not valid for parallel initialization")]
		public override void ItShouldUpdateDirectoryManagerForEachArtifact()
		{
		}

		[Test]
		public void ItShouldInitializeLabelManagerForArtifact()
		{
			// ACT
			this.Instance.PrepareBatch(this.Artifacts, this.VolumePredictions, CancellationToken.None);

			// ASSERT
			this.LabelManagerForArtifact.Verify(lm => lm.InitializeFor(this.Artifacts, this.VolumePredictions, CancellationToken.None), Times.Once);
		}
	}
}