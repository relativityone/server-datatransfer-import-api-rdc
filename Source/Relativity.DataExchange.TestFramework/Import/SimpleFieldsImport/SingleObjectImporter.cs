// <copyright file="SingleObjectImporter.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport
{
	using System;

	using kCura.Relativity.DataReaderClient;

	using NUnit.Framework;

	using Relativity.DataExchange.TestFramework.Import.JobExecutionContext;

	public class SingleObjectImporter
	{
		private readonly IntegrationTestParameters testParameters;

		private readonly NativeImportExecutionContext executionContext;

		public SingleObjectImporter(IntegrationTestParameters testParameters, NativeImportExecutionContext executionContext)
		{
			this.testParameters = testParameters;
			this.executionContext = executionContext;
		}

		public void ImportData(ImportDataSource<object[]> dataSource, Settings importSettings)
		{
			dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));
			importSettings = importSettings ?? throw new ArgumentNullException(nameof(importSettings));

			this.executionContext.InitializeImportApiWithUserAndPassword(this.testParameters, importSettings);

			ImportTestJobResult result = this.executionContext.Execute(dataSource);

			if (result.FatalException != null)
			{
				throw result.FatalException;
			}

			Assume.That(result.JobReportErrorsCount == 0);
		}
	}
}
