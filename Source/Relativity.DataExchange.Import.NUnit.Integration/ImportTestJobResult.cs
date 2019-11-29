// ----------------------------------------------------------------------------
// <copyright file="ImportTestJobResult.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit.Integration
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	public class ImportTestJobResult
	{
		public List<string> JobMessages { get; } = new List<string>();

		public List<Exception> JobFatalExceptions { get; } = new List<Exception>();

		public List<IDictionary> ErrorRows { get; } = new List<IDictionary>();

		public List<long> ProgressCompletedRows { get; } = new List<long>();

		public List<JobReport> CompletedJobReports { get; } = new List<JobReport>();
	}
}