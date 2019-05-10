// -----------------------------------------------------------------------------------------------------
// <copyright file="MockBulkLoadFileImporter.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Import.Client.NUnit
{
	using System;
	using System.Threading;

	using kCura.EDDS.WebAPI.BulkImportManagerBase;
	using kCura.WinEDDS;

	using Relativity.Import.Export.Io;
	using Relativity.Import.Export.Process;
	using Relativity.Logging;

	using ExecutionSource = Relativity.Import.Export.Service.ExecutionSource;

	/// <summary>
	/// Represents a mock class object for <see cref="BulkLoadFileImporter"/>.
	/// </summary>
	public class MockBulkLoadFileImporter : BulkLoadFileImporter
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
			ExecutionSource executionSource)
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
		}

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

		public kCura.EDDS.WebAPI.BulkImportManagerBase.OverlayBehavior ConvertOverlayBehavior(LoadFile.FieldOverlayBehavior? behavior)
		{
			return this.GetMassImportOverlayBehavior(behavior);
		}

		public MassImportResults TryBulkImport(NativeLoadInfo settings)
		{
			const bool IncludeExtractedTextEncoding = true;
			return this.BulkImport(settings, IncludeExtractedTextEncoding);
		}

		protected override void RaiseWarningAndPause(Exception exception, int timeoutSeconds, int retryCount, int totalRetryCount)
		{
			base.RaiseWarningAndPause(exception, timeoutSeconds, retryCount, totalRetryCount);
			this.PauseCalled++;
		}
	}
}