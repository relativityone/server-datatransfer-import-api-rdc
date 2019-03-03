// ----------------------------------------------------------------------------
// <copyright file="ProcessProgressEventArgs.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Process
{
	using System;
	using System.Collections;

	/// <summary>
	/// Represents the process progress event argument data.
	/// </summary>
	[Serializable]
	public sealed class ProcessProgressEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ProcessProgressEventArgs"/> class.
		/// </summary>
		/// <param name="processId">
		/// The process unique identifier.
		/// </param>
		/// <param name="metadata">
		/// The progress metadata.
		/// </param>
		/// <param name="startTime">
		/// The process start time.
		/// </param>
		/// <param name="timestamp">
		/// The timestamp when the event occurred.
		/// </param>
		/// <param name="totalRecords">
		/// The total number of records to process.
		/// </param>
		/// <param name="totalRecordsDisplay">
		/// The display string for the total number of records to process.
		/// </param>
		/// <param name="totalProcessedRecords">
		/// The total number of records that have been processed.
		/// </param>
		/// <param name="totalProcessedRecordsDisplay">
		/// The display string for the total number of records that have been processed.
		/// </param>
		/// <param name="totalProcessedWarningRecords">
		/// The total number of records that have been processed with warnings.
		/// </param>
		/// <param name="totalProcessedErrorRecords">
		/// The total number of records that have been processed with errors.
		/// </param>
		/// <param name="metadataThroughput">
		/// The metadata throughput in MB/sec units.
		/// </param>
		/// <param name="nativeFileThroughput">
		/// The native file throughput in MB/sec units.
		/// </param>
		public ProcessProgressEventArgs(
			Guid processId,
			IDictionary metadata,
			DateTime startTime,
			DateTime timestamp,
			long totalRecords,
			string totalRecordsDisplay,
			long totalProcessedRecords,
			string totalProcessedRecordsDisplay,
			long totalProcessedWarningRecords,
			long totalProcessedErrorRecords,
			double metadataThroughput,
			double nativeFileThroughput)
		{
			this.ProcessId = processId;
			this.Metadata = metadata;
			this.StartTime = startTime;
			this.Timestamp = timestamp;
			this.TotalRecords = totalRecords;
			this.TotalRecordsDisplay = !string.IsNullOrEmpty(totalRecordsDisplay)
				                           ? totalRecordsDisplay
				                           : totalRecords.ToString();
			this.TotalProcessedRecords = totalProcessedRecords;
			this.TotalProcessedRecordsDisplay = !string.IsNullOrEmpty(totalProcessedRecordsDisplay)
				                           ? totalProcessedRecordsDisplay
										   : totalProcessedRecords.ToString();
			this.TotalProcessedErrorRecords = totalProcessedErrorRecords;
			this.TotalProcessedWarningRecords = totalProcessedWarningRecords;
			this.MetadataThroughput = metadataThroughput;
			this.NativeFileThroughput = nativeFileThroughput;
		}

		/// <summary>
		/// Gets the progress metadata.
		/// </summary>
		/// <value>
		/// The <see cref="IDictionary"/> instance.
		/// </value>
		public IDictionary Metadata { get; }

		/// <summary>
		/// Gets the metadata throughput in MB/sec units.
		/// </summary>
		/// <value>
		/// The throughput.
		/// </value>
		public double MetadataThroughput { get; }

		/// <summary>
		/// Gets the native file throughput in MB/sec units.
		/// </summary>
		/// <value>
		/// The throughput.
		/// </value>
		public double NativeFileThroughput { get; }

		/// <summary>
		/// Gets the process unique identifier.
		/// </summary>
		/// <value>
		/// The <see cref="Guid"/> value.
		/// </value>
		public Guid ProcessId { get; }

		/// <summary>
		/// Gets the timestamp when the process was started.
		/// </summary>
		/// <value>
		/// The <see cref="DateTime"/> value.
		/// </value>
		public DateTime StartTime { get; }

		/// <summary>
		/// Gets the timestamp when the event occurred.
		/// </summary>
		/// <value>
		/// The <see cref="DateTime"/> value.
		/// </value>
		public DateTime Timestamp { get; }

		/// <summary>
		/// Gets the total number of records to process.
		/// </summary>
		/// <value>
		/// The total number of records.
		/// </value>
		public long TotalRecords { get; }

		/// <summary>
		/// Gets the display string for the total number of records to process.
		/// </summary>
		/// <value>
		/// The display string.
		/// </value>
		public string TotalRecordsDisplay { get; }

		/// <summary>
		/// Gets the total number of records that have been processed.
		/// </summary>
		/// <value>
		/// The total number of records.
		/// </value>
		public long TotalProcessedRecords { get; }

		/// <summary>
		/// Gets the display string for the total number of records that have been processed.
		/// </summary>
		/// <value>
		/// The display string.
		/// </value>
		public string TotalProcessedRecordsDisplay { get; }

		/// <summary>
		/// Gets the total number of records that have been processed with warnings.
		/// </summary>
		/// <value>
		/// The total number of records.
		/// </value>
		public long TotalProcessedWarningRecords { get; }

		/// <summary>
		/// Gets the total number of records that have been processed with errors.
		/// </summary>
		/// <value>
		/// The total number of records.
		/// </value>
		public long TotalProcessedErrorRecords { get; }
	}
}