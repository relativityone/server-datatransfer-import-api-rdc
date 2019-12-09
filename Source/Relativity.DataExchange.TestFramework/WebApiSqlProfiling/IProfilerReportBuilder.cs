// <copyright file="IProfilerReportBuilder.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.WebApiSqlProfiling
{
	using System.Xml.Linq;

	public interface IProfilerReportBuilder
	{
		void CreateNewRow();

		void AddDetailsToCurrentRow(XElement element);

		void CommitRow();

		ProfilerReport Build();
	}
}
