// <copyright file="ImportProfilingTests.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.Import.NUnit.Integration
{
	using System.Collections.Generic;

	using global::NUnit.Framework;

	using Relativity.DataExchange.Import.NUnit.Integration.Dto;
	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.NUnitExtensions;
	using Relativity.DataExchange.Transfer;
	using Relativity.Testing.Identification;

	[TestFixture]
	[Explicit]
	[Feature.DataTransfer.ImportApi.Operations.ImportDocuments]
	public class ImportProfilingTests : NativeImportJobTestBase
	{
		[CollectWebApiExecutionPlans]
		[CollectWebApiSql]
		[TestCase(5)]
		[TestCase(1000)]
		[TestCase(5000)]
		public void SimpleImportTestCase(int numberOfDocuments)
		{
			const TapiClient Client = TapiClient.Direct;
			const bool DisableNativeLocationValidation = false;
			const bool DisableNativeValidation = false;

			// ARRANGE
			TapiClientModeAvailabilityChecker.SkipTestIfModeNotAvailable(AssemblySetup.TestParameters, Client);

			ForceClient(Client);
			kCura.WinEDDS.Config.ConfigSettings["DisableNativeLocationValidation"] = DisableNativeLocationValidation;
			kCura.WinEDDS.Config.ConfigSettings["DisableNativeValidation"] = DisableNativeValidation;

			this.GivenTheImportJob();
			this.GiveNativeFilePathSourceDocumentImportJob();

			IEnumerable<DefaultImportDto> importData = DefaultImportDto.GetRandomTextFiles(this.TempDirectory.Directory, numberOfDocuments);

			// ACT
			this.WhenExecutingTheJob(importData);

			// ASSERT
			this.ThenTheImportJobIsSuccessful(numberOfDocuments);
		}
	}
}
