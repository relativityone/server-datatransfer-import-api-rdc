// <copyright file="SqlStatementRowDto.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.WebApiSqlProfiling.SqlStatement
{
	internal class SqlStatementRowDto
	{
		public string Sql { get; set; }

		public string BatchText { get; set; }

		public string Statement { get; set; }

		public ulong Duration { get; set; }

		public ulong CpuTime { get; set; }

		public ulong PhysicalReads { get; set; }

		public ulong LogicalReads { get; set; }

		public ulong Writes { get; set; }

		public ulong RowCount { get; set; }

		public ulong QueryHash { get; set; }

		public ulong SessionID { get; set; }

		public ulong RequestID { get; set; }
	}
}
