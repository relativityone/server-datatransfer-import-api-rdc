// <copyright file="ParallelImportTestJobResult.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>
namespace Relativity.DataExchange.Import.NUnit.LoadTests.JobExecutionContext
{
	using System.Collections.Generic;
	using System.Linq;

	using Relativity.DataExchange.TestFramework.Import.JobExecutionContext;

	public class ParallelImportTestJobResult : ImportTestJobResult
	{
		private readonly List<double> aggregatedSqlProcRate = new List<double>();

		public override double SqlProcessRate => this.aggregatedSqlProcRate.Sum() / this.aggregatedSqlProcRate.Count;

		public void AddSqlProcessRate(double sqlProcessRate)
		{
			this.aggregatedSqlProcRate.Add(sqlProcessRate);
		}
	}
}
