// <copyright file="DeadlockReportRowDto.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.WebApiSqlProfiling.DeadlockReport
{
	using System.Collections.Generic;

	internal class DeadlockReportRowDto
	{
		public string DeadlockReport { get; set; }

		public List<(string Name, string Sql)> LockedObjectInfo { get; } = new List<(string Name, string Sql)>();
	}
}