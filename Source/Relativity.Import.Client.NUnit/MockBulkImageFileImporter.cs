// -----------------------------------------------------------------------------------------------------
// <copyright file="MockBulkImageFileImporter.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Import.Client.NUnit
{
	using System;
	using System.IO;
	using System.Text;
	using System.Threading;

	using kCura.WinEDDS;

	using Relativity.Import.Export.Io;
	using Relativity.Import.Export.Process;
	using Relativity.Logging;

	using ExecutionSource = Relativity.Import.Export.Services.ExecutionSource;

	/// <summary>
	/// Represents a mock class object for <see cref="BulkImageFileImporter"/>.
	/// </summary>
	public class MockBulkImageFileImporter : BulkImageFileImporter
	{
		public MockBulkImageFileImporter(
			ImageLoadFile args,
			ProcessContext context,
			IIoReporter reporter,
			ILog logger,
			Guid processID,
			bool doRetryLogic,
			kCura.WinEDDS.Service.IBulkImportManager manager,
			kCura.WinEDDS.Api.IImageReader reader,
			CancellationTokenSource tokenSource,
			ExecutionSource executionSource)
			: base(
				0,
				args,
				context,
				reporter,
				logger,
				processID,
				doRetryLogic,
				false,
				tokenSource,
				executionSource)
		{
			this._bulkImportManager = manager;
			this._imageReader = reader;

			this.ImportBatchVolume = 1000000;
			this.OutputFromStringWriter = new StringBuilder();
		}

		public int BatchSize
		{
			get => this.ImportBatchSize;
			set => this.ImportBatchSize = value;
		}

		public string InputForStringWriter
		{
			get;
			set;
		}

		public int MinimumBatch
		{
			get => this.MinimumBatchSize;
			set => this.MinimumBatchSize = value;
		}

		public StringBuilder OutputFromStringWriter
		{
			get;
		}

		public int PauseCalled
		{
			get;
			private set;
		}

		protected override bool BatchResizeEnabled => true;

		protected override int NumberOfRetries => 3;

		protected override int WaitTimeBetweenRetryAttempts => 0;

		public void SetImportBatchSize(int size)
		{
			this.ImportBatchSize = size;
		}

		public void MockLowerBatchSizeAndRetry(int numberOfRecords)
		{
			this.LowerBatchSizeAndRetry(string.Empty, string.Empty, numberOfRecords);
		}

		public kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults TryBulkImport(kCura.EDDS.WebAPI.BulkImportManagerBase.OverwriteType overwrite)
		{
			const bool UseBulk = false;
			return this.RunBulkImport(overwrite, UseBulk);
		}

		protected override TextWriter CreateStreamWriter(string tmpLocation)
		{
			return new StringWriter(this.OutputFromStringWriter);
		}

		protected override TextReader CreateStreamReader(string outputPath)
		{
			return new StringReader(this.InputForStringWriter);
		}

		protected override void InitializeDTOs(ImageLoadFile args)
		{
			// Do NOT call the base class!
			this._keyFieldDto = new kCura.EDDS.WebAPI.FieldManagerBase.Field();
			this._keyFieldDto.ArtifactID = 0;
		}

		protected override void InitializeUploaders(ImageLoadFile args)
		{
			// Do NOT call the base class!
		}

		protected override void RaiseWarningAndPause(Exception exception, int timeoutSeconds, int retryCount, int totalRetryCount)
		{
			// Do NOT call the base class!
			this.PauseCalled++;
		}

		protected override int DoLogicAndPushImageBatch(
			int totalRecords,
			int recordsProcessed,
			string bulkLocation,
			string dataGridLocation,
			ref long charactersSuccessfullyProcessed,
			int i,
			long charactersProcessed)
		{
			return 100;
		}
	}
}