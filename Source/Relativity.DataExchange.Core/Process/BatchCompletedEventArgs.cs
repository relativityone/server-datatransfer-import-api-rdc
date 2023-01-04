// ----------------------------------------------------------------------------
// <copyright file="BatchCompletedEventArgs.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Process
{
	using System;

	/// <summary>
	/// Batch completed event args.
	/// </summary>
	[Serializable]
	public sealed class BatchCompletedEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BatchCompletedEventArgs"/> class.
		/// </summary>
		/// <param name="batchOrdinalNumber">Ordinal number.</param>
		/// <param name="numberOfFiles">Number of imported files.</param>
		/// <param name="numberOfRecords">Number of imported records.</param>
		/// <param name="numberOfRecordsWithErrors">Number of records with errors.</param>
		public BatchCompletedEventArgs(int batchOrdinalNumber, int numberOfFiles, int numberOfRecords, int numberOfRecordsWithErrors)
		{
			this.BatchOrdinalNumber = batchOrdinalNumber;
			this.NumberOfFiles = numberOfFiles;
			this.NumberOfRecords = numberOfRecords;
			this.NumberOfRecordsWithErrors = numberOfRecordsWithErrors;
		}

		/// <summary>
		/// Gets batch ordinal number.
		/// </summary>
		/// <returns>Batch ordinal number.</returns>
		public int BatchOrdinalNumber { get; }

		/// <summary>
		/// Gets number of files in batch.
		/// </summary>
		/// <returns>Number of files.</returns>
		public int NumberOfFiles { get; }

		/// <summary>
		/// Gets number of records in batch.
		/// </summary>
		/// <returns>Number of records.</returns>
		public int NumberOfRecords { get; }

		/// <summary>
		/// Gets number of records with errors in batch.
		/// </summary>
		/// <returns>Number of records with errors.</returns>
		public int NumberOfRecordsWithErrors { get; }
	}
}