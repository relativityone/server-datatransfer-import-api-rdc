// ----------------------------------------------------------------------------
// <copyright file="ImportLoadTestsBase.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit.LoadTests
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using kCura.Relativity.DataReaderClient;

	using Relativity.DataExchange.Import.NUnit.Integration;
	using Relativity.DataExchange.Import.NUnit.LoadTests.JobExecutionContext;
	using Relativity.DataExchange.TestFramework.Import.JobExecutionContext;
	using Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport;

	public abstract class ImportLoadTestsBase<TExecutionContext, TSettings> : ImportJobTestBase<ParallelImportExecutionContext<TExecutionContext, TSettings>>
		where TSettings : ImportSettingsBase
		where TExecutionContext : IExecutionContext, IImportApiSetup<TSettings>, new()
	{
		[TearDown]
		public Task TearDownAsync()
		{
			return this.ResetContextAsync();
		}

		protected override void ValidateTotalRowsCount(ImportTestJobResult testJobResult, int expectedTotalRows)
		{
			int totalRowsCount = this.JobExecutionContext.CompletedTotalRowsCountFromReport;

			Assert.That(totalRowsCount, Is.EqualTo(expectedTotalRows));
		}

		protected override void ValidateFatalExceptionsNotExist(ImportTestJobResult testJobResult)
		{
			IEnumerable<Exception> fatalExceptions =
				this.JobExecutionContext.FatalExceptionsFromReport;

			Assert.That(fatalExceptions.All(item => item == null));
			Assert.That(this.JobExecutionContext.TestJobResult.JobFatalExceptions, Has.Count.Zero);
		}

		protected override void ValidateErrorRowsCount(ImportTestJobResult testJobResult, int expectedErrorRows)
		{
			testJobResult = testJobResult ?? throw new ArgumentNullException(nameof(testJobResult));
			int numberOfErrorRows = this.JobExecutionContext.ErrorRowsCountFromReport;

			Assert.That(numberOfErrorRows, Is.EqualTo(expectedErrorRows));
			Assert.That(testJobResult.ErrorRows.Count, Is.EqualTo(expectedErrorRows));
		}

		protected void InitializeImportApiWithUserAndPwd(ISettingsBuilder<TSettings> settingsBuilder, int instanceCount)
		{
			this.JobExecutionContext
				.ConfigureImportApiInstanceCount(instanceCount)
				.InitializeImportApiWithUserAndPassword(this.TestParameters, settingsBuilder);
		}
	}
}