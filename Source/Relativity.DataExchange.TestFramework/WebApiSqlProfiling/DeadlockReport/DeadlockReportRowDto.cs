// <copyright file="DeadlockReportRowDto.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.WebApiSqlProfiling.DeadlockReport
{
	using System.Collections.Generic;

	internal class DeadlockReportRowDto
	{
		public DeadlockReportRowDto()
		{
			LockedObjectsNames = new List<string>();
			ProcessIdToClientApp = new Dictionary<string, string>();
		}

		public string DeadlockReport { get; set; }

		public List<string> LockedObjectsNames { get; }

		public Dictionary<string, string> ProcessIdToClientApp { get; }

		public string VictimProcessId { get; set; }
	}
}