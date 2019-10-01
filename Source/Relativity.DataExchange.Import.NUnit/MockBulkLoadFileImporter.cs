// -----------------------------------------------------------------------------------------------------
// <copyright file="MockBulkLoadFileImporter.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit
{
	using System;
	using System.Net;
	using System.Text;
	using System.Threading;
	using kCura.EDDS.WebAPI.BulkImportManagerBase;
	using kCura.WinEDDS;
	using Relativity.DataExchange.Io;
	using Relativity.DataExchange.Process;
	using Relativity.DataExchange.Transfer;
	using Relativity.Logging;

	/// <summary>
	/// Represents a mock class object for <see cref="BulkLoadFileImporter"/>.
	/// </summary>
	public sealed class MockBulkLoadFileImporter : BulkLoadFileImporter
	{
		public MockBulkLoadFileImporter(
			LoadFile args,
			ProcessContext context,
			IIoReporter reporter,
			ILog logger,
			int timeZoneOffset,
			bool autoDetect,
			bool initializeUploaders,
			Guid processID,
			bool doRetryLogic,
			string bulkLoadFileFieldDelimiter,
			kCura.WinEDDS.Service.IBulkImportManager manager,
			CancellationTokenSource tokenSource,
			Relativity.DataExchange.Service.ExecutionSource executionSource)
			: base(
				args,
				context,
				reporter,
				logger,
				timeZoneOffset,
				autoDetect,
				initializeUploaders,
				processID,
				doRetryLogic,
				bulkLoadFileFieldDelimiter,
				false,
				tokenSource,
				executionSource)
		{
			this._bulkImportManager = manager;
			this.ImportBatchSize = 500;
			this.ImportBatchVolume = 1000000;
			this.OutputFileWriter = new OutputFileWriter();
			this.OutputFileWriter.Open(true);
		}

		public int GetMetadataFilesCount => this.MetadataFilesCount;

		public int BatchSize
		{
			get => this.ImportBatchSize;
			set => this.ImportBatchSize = value;
		}

		public LoadFileFieldMap FieldMap
		{
			get => this._fieldMap;
			set => this._fieldMap = value;
		}

		public int MinimumBatch
		{
			get => this.MinimumBatchSize;
			set => this.MinimumBatchSize = value;
		}

		public int PauseCalled
		{
			get;
			private set;
		}

		protected override bool BatchResizeEnabled => true;

		protected override int ParentArtifactTypeID => 4000;

		protected override int NumberOfRetries => 3;

		protected override int WaitTimeBetweenRetryAttempts => 0;

		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Performance",
			"CA1822:MarkMembersAsStatic",
			Justification = "To be consistent with existing VB.NET tests.")]
		public string CleanFolderPath(string path)
		{
			return CleanDestinationFolderPath(path);
		}

		public OverlayBehavior ConvertOverlayBehavior(LoadFile.FieldOverlayBehavior? behavior)
		{
			return this.GetMassImportOverlayBehavior(behavior);
		}

		public MassImportResults TryBulkImport(NativeLoadInfo settings)
		{
			const bool IncludeExtractedTextEncoding = true;
			return this.BulkImport(settings, IncludeExtractedTextEncoding);
		}

		public void SetTapiBridges()
		{
			UploadTapiBridgeParameters2 parameters = new UploadTapiBridgeParameters2
			{
				Credentials = new NetworkCredential(),
				WebServiceUrl = "https://relativity.one.com",
				WorkspaceId = 1337,
				TargetPath = "./",
				FileShare = "./somepath/",
				TimeoutSeconds = 0,
			};
			this.CreateTapiBridges(parameters, parameters.ShallowCopy());
		}

		public void SetBatchCounter(int numberToSet)
		{
			this._batchCounter = numberToSet;
		}

		public void PushNativeBatchInvoker(string outputNativePath, bool shouldCompleteJob, bool lastRun)
		{
			this.PushNativeBatch(outputNativePath, shouldCompleteJob, lastRun);
		}

		public override void Dispose()
		{
			this.OutputFileWriter?.Dispose();
			base.Dispose();
		}

		protected override void RaiseWarningAndPause(Exception exception, int timeoutSeconds, int retryCount, int totalRetryCount)
		{
			base.RaiseWarningAndPause(exception, timeoutSeconds, retryCount, totalRetryCount);
			this.PauseCalled++;
		}
	}
}