using System;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Batches;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics;
using kCura.WinEDDS.Exporters;
using Moq;
using NUnit.Framework;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2
{
	[TestFixture]
	public class BatchTests
	{
		private Batch _instance;
		private Mock<IBatchExporter> _batchExporter;
		private Mock<IBatchInitialization> _batchInitialization;
		private Mock<IBatchCleanUp> _batchCleanUp;
		private Mock<IBatchValidator> _batchValidator;
		private Mock<IBatchState> _batchState;

		[SetUp]
		public void SetUp()
		{
			_batchExporter = new Mock<IBatchExporter>();
			_batchInitialization = new Mock<IBatchInitialization>();
			_batchCleanUp = new Mock<IBatchCleanUp>();
			_batchValidator = new Mock<IBatchValidator>();
			_batchState = new Mock<IBatchState>();

			Mock<IMessenger> messenger = new Mock<IMessenger>();

			_instance = new Batch(_batchExporter.Object, _batchInitialization.Object, _batchCleanUp.Object, _batchValidator.Object, _batchState.Object, messenger.Object, new NullLogger());
		}

		[Test]
		public void GoldWorkflow()
		{
			ObjectExportInfo[] artifacts = new ObjectExportInfo[1];
			VolumePredictions[] volumePredictions = new VolumePredictions[1];

			//ACT
			_instance.Export(artifacts, volumePredictions, CancellationToken.None);

			//ASSERT
			_batchInitialization.Verify(x => x.PrepareBatch(artifacts, volumePredictions, CancellationToken.None), Times.Once);
			_batchExporter.Verify(x => x.Export(artifacts, volumePredictions, CancellationToken.None), Times.Once);
			_batchValidator.Verify(x => x.ValidateExportedBatch(artifacts, volumePredictions, CancellationToken.None), Times.Once);
			_batchState.Verify(x => x.SaveState(), Times.Once);
			_batchCleanUp.Verify(x => x.CleanUp(), Times.Once);

			_batchState.Verify(x => x.RestoreState(), Times.Never);
		}

		[Test]
		public void ItShouldAlwaysExecuteCleanUp()
		{
			ObjectExportInfo[] artifacts = new ObjectExportInfo[1];
			VolumePredictions[] volumePredictions = new VolumePredictions[1];

			_batchExporter.Setup(x => x.Export(artifacts, volumePredictions, CancellationToken.None)).Throws<Exception>();

			//ACT & ASSERT
			Assert.Throws<Exception>(() => _instance.Export(artifacts, volumePredictions, CancellationToken.None));

			_batchCleanUp.Verify(x => x.CleanUp(), Times.Once);
		}

		[Test]
		public void ItShouldRestoreStateAfterCancel()
		{
			ObjectExportInfo[] artifacts = new ObjectExportInfo[1];
			VolumePredictions[] volumePredictions = new VolumePredictions[1];

			CancellationTokenSource tokenSource = new CancellationTokenSource();

			_batchExporter.Setup(x => x.Export(artifacts, volumePredictions, tokenSource.Token)).Callback(() => tokenSource.Cancel());

			//ACT
			_instance.Export(artifacts, volumePredictions, tokenSource.Token);

			//ASSERT
			_batchState.Verify(x => x.RestoreState(), Times.Once);
		}
	}
}