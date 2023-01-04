// -----------------------------------------------------------------------------------------------------
// <copyright file="BatchTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System;
	using System.Threading;
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using kCura.WinEDDS.Exporters;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2;
	using Relativity.DataExchange.Export.VolumeManagerV2.Batches;
	using Relativity.DataExchange.Export.VolumeManagerV2.Statistics;
	using Relativity.DataExchange.TestFramework;

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
			this._batchExporter = new Mock<IBatchExporter>();
			this._batchInitialization = new Mock<IBatchInitialization>();
			this._batchCleanUp = new Mock<IBatchCleanUp>();
			this._batchValidator = new Mock<IBatchValidator>();
			this._batchState = new Mock<IBatchState>();

			Mock<IMessenger> messenger = new Mock<IMessenger>();

			this._instance = new Batch(this._batchExporter.Object, this._batchInitialization.Object, this._batchCleanUp.Object, this._batchValidator.Object, this._batchState.Object, messenger.Object, new TestNullLogger());
		}

		[Test]
		public async Task GoldWorkflow()
		{
			ObjectExportInfo[] artifacts = new ObjectExportInfo[1];
			VolumePredictions[] volumePredictions = new VolumePredictions[1];

			// ACT
			await this._instance.ExportAsync(artifacts, volumePredictions, CancellationToken.None).ConfigureAwait(false);

			// ASSERT
			this._batchInitialization.Verify(x => x.PrepareBatch(artifacts, volumePredictions, CancellationToken.None), Times.Once);
			this._batchExporter.Verify(x => x.ExportAsync(artifacts, CancellationToken.None), Times.Once);
			this._batchValidator.Verify(x => x.ValidateExportedBatch(artifacts, CancellationToken.None), Times.Once);
			this._batchState.Verify(x => x.SaveState(), Times.Once);
			this._batchCleanUp.Verify(x => x.CleanUp(), Times.Once);

			this._batchState.Verify(x => x.RestoreState(), Times.Never);
		}

		[Test]
		public void ItShouldAlwaysExecuteCleanUp()
		{
			ObjectExportInfo[] artifacts = new ObjectExportInfo[1];
			VolumePredictions[] volumePredictions = new VolumePredictions[1];

			this._batchExporter.Setup(x => x.ExportAsync(artifacts, CancellationToken.None)).Throws<Exception>();

			// ACT & ASSERT
			Assert.Throws<Exception>(() => this._instance.ExportAsync(artifacts, volumePredictions, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult());

			this._batchCleanUp.Verify(x => x.CleanUp(), Times.Once);
		}

		[Test]
		public async Task ItShouldRestoreStateAfterCancel()
		{
			ObjectExportInfo[] artifacts = new ObjectExportInfo[1];
			VolumePredictions[] volumePredictions = new VolumePredictions[1];

			CancellationTokenSource tokenSource = new CancellationTokenSource();

			this._batchExporter.Setup(x => x.ExportAsync(artifacts, tokenSource.Token)).Callback(() => tokenSource.Cancel()).Returns(Task.CompletedTask);

			// ACT
			await this._instance.ExportAsync(artifacts, volumePredictions, tokenSource.Token).ConfigureAwait(false);

			// ASSERT
			this._batchState.Verify(x => x.RestoreState(), Times.Once);
		}
	}
}