// <copyright file="IExecutionContext.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.Import.JobExecutionContext
{
	using System;
	using System.Collections.Generic;
	using System.Data;

	using Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport;

	public interface IExecutionContext : IDisposable
	{
		ImportTestJobResult TestJobResult { get; }

		ImportTestJobResult Execute<T>(ImportDataSource<T> importData);
	}
}
