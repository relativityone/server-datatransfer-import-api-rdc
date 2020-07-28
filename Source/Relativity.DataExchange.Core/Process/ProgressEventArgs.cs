// ----------------------------------------------------------------------------
// <copyright file="ProgressEventArgs.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Process
{
	using System;
	using System.Collections;

	/// <summary>
	/// Represents the progress event argument data. This class cannot be inherited.
	/// </summary>
	[Serializable]
	public sealed class ProgressEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ProgressEventArgs"/> class.
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
		/// <param name="total">
		/// The total number of units to process.
		/// </param>
		/// <param name="totalDisplay">
		/// The display string for the total number of units to process.
		/// </param>
		/// <param name="processed">
		/// The number of units that have been processed.
		/// </param>
		/// <param name="processedDisplay">
		/// The display string for the number of units that have been processed.
		/// </param>
		/// <param name="processedWithWarning">
		/// The total number of units that have been processed with warnings.
		/// </param>
		/// <param name="processedWithError">
		/// The total number of units that have been processed with errors.
		/// </param>
		/// <param name="metadataThroughput">
		/// The metadata throughput in MB/sec units.
		/// </param>
		/// <param name="nativeFileThroughput">
		/// The native file throughput in MB/sec units.
		/// </param>
		/// <param name="progressUnit">
		/// The unit of measurement used to track progress.
		/// </param>
		public ProgressEventArgs(
			Guid processId,
			IDictionary metadata,
			DateTime startTime,
			DateTime timestamp,
			long total,
			string totalDisplay,
			long processed,
			string processedDisplay,
			long processedWithWarning,
			long processedWithError,
			double metadataThroughput,
			double nativeFileThroughput,
			UnitOfMeasurement progressUnit)
		{
			this.ProcessId = processId;
			this.Metadata = metadata;
			this.StartTime = startTime;
			this.Timestamp = timestamp;
			this.Total = total;
			this.TotalDisplay = !string.IsNullOrEmpty(totalDisplay)
											? totalDisplay
											: total.ToString();
			this.Processed = processed;
			this.ProcessedDisplay = !string.IsNullOrEmpty(processedDisplay)
											? processedDisplay
											: processed.ToString();
			this.ProcessedWithError = processedWithError;
			this.ProcessedWithWarning = processedWithWarning;
			this.MetadataThroughput = metadataThroughput;
			this.NativeFileThroughput = nativeFileThroughput;
			this.ProgressUnit = progressUnit;
		}

		/// <summary>
		/// Unit of measurement used to track progress.
		/// </summary>
		public enum UnitOfMeasurement
		{
			/// <summary>
			/// Progress is measured in number of processed records.
			/// </summary>
			Records,

			/// <summary>
			/// Progress is measured in number of processed bytes.
			/// </summary>
			Bytes,
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
		/// Gets the total number of units to process measured in <see cref="ProgressUnit"/>.
		/// </summary>
		/// <value>
		/// The total number of units.
		/// </value>
		public long Total { get; }

		/// <summary>
		/// Gets the display string for the total number of units to process measured in <see cref="ProgressUnit"/>.
		/// </summary>
		/// <value>
		/// The display string.
		/// </value>
		public string TotalDisplay { get; }

		/// <summary>
		/// Gets the total number of units that have already been processed measured in <see cref="ProgressUnit"/>.
		/// </summary>
		/// <value>
		/// The total number of processed units.
		/// </value>
		public long Processed { get; }

		/// <summary>
		/// Gets the display string for the total number of units that have been processed measured in <see cref="ProgressUnit"/>.
		/// </summary>
		/// <value>
		/// The display string.
		/// </value>
		public string ProcessedDisplay { get; }

		/// <summary>
		/// Gets the total number of units that have been processed with warnings measured in <see cref="ProgressUnit"/>.
		/// </summary>
		/// <value>
		/// The total number of units processed with warnings.
		/// </value>
		public long ProcessedWithWarning { get; }

		/// <summary>
		/// Gets the total number of units that have been processed with errors measured in <see cref="ProgressUnit"/>.
		/// </summary>
		/// <value>
		/// The total number of units processed with errors.
		/// </value>
		public long ProcessedWithError { get; }

		/// <summary>
		/// Gets the unit of measurement used to track progress.
		/// </summary>
		public UnitOfMeasurement ProgressUnit { get; }
	}
}