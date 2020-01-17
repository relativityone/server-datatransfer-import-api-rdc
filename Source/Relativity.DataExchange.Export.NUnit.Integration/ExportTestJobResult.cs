// <copyright file="ExportTestJobResult.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.Export.NUnit.Integration
{
	using System.Collections.Generic;

	using Relativity.DataExchange.Transfer;

	public class ExportTestJobResult
	{
		public List<string> Alerts { get; } = new List<string>();

		public List<string> AlertCriticalErrors { get; } = new List<string>();

		public List<string> AlertWarningSkippables { get; } = new List<string>();

		public List<string> StatusMessages { get; } = new List<string>();

		public List<string> ErrorMessages { get; } = new List<string>();

		public List<string> FatalErrors { get; } = new List<string>();

		public int TotalDocumentsProcessed { get; set; }

		public int TotalDocuments { get; set; }

		public bool SearchResult { get; set; }

		public List<TapiClient> TransferModes { get; } = new List<TapiClient>();
	}
}