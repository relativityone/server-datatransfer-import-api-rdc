// ----------------------------------------------------------------------------
// <copyright file="ImportTestJobResult.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework.Import.JobExecutionContext
{
	using System;
	using System.Collections;
	using System.Collections.Concurrent;
	using System.Collections.Generic;

	/// <summary>
	/// This class represents results of import test.
	/// </summary>
	/// <remarks>It can be accessed across App Domain boundaries.</remarks>
	public class ImportTestJobResult : MarshalByRefObject
	{
		private const int JobMessagesLimit = 10000; // that value is referenced in doc comments

		private readonly ConcurrentQueue<string> jobMessages = new ConcurrentQueue<string>();

		public List<Exception> JobFatalExceptions { get; } = new List<Exception>();

		public List<IDictionary> ErrorRows { get; } = new List<IDictionary>();

		public long NumberOfJobMessages { get; set; }

		/// <summary>
		/// Gets 10 000 most recent job messages.
		/// </summary>
		public IEnumerable<string> JobMessages => this.jobMessages;

		public long NumberOfCompletedRows { get; set; }

		public DateTime StartTime => this.CompletedJobReport.StartTime;

		public DateTime EndTime => this.CompletedJobReport.EndTime;

		public int JobReportTotalRows => this.CompletedJobReport.TotalRows;

		public int JobReportErrorsCount => this.CompletedJobReport.ErrorRows.Count;

		public Exception FatalException => this.CompletedJobReport.FatalException;

		/// <summary>
		/// Sets completed job report.
		/// </summary>
		/// <remarks>It is not safe to use that property across App Domains.</remarks>
		internal JobReport CompletedJobReport { private get; set; }

		/// <summary>
		/// It adds job message to test result object.
		/// When there is more than 10 000 messages,
		/// then the oldest entries will be deleted.
		/// That limit was introduced to improve memory usage in tests.
		/// </summary>
		/// <param name="message">message to add to list.</param>
		public void AddMessage(string message)
		{
			this.NumberOfJobMessages++;
			while (this.jobMessages.Count > JobMessagesLimit)
			{
				this.jobMessages.TryDequeue(out string _);
			}

			// When messages are added concurrently, we can end up with slightly more messages than we should, but it is acceptable in case of tests
			// The real limit is: '10k + number of threads adding messages'
			this.jobMessages.Enqueue(message);
		}
	}
}