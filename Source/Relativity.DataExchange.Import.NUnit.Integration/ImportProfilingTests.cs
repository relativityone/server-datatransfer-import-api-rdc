// <copyright file="ImportProfilingTests.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.Import.NUnit.Integration
{
	using System.Collections.Generic;

	using global::NUnit.Framework;

	using kCura.Relativity.DataReaderClient;

	using Relativity.DataExchange.Import.NUnit.Integration.Dto;
	using Relativity.DataExchange.Import.NUnit.Integration.SetUp;
	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.NUnitExtensions;
	using Relativity.DataExchange.Transfer;
	using Relativity.Testing.Identification;

	[TestFixture]
	[Explicit]
	[Feature.DataTransfer.ImportApi.Operations.ImportDocuments]
	public class ImportProfilingTests : ImportJobTestBase<ImportBulkArtifactJob, Settings>
	{
		public ImportProfilingTests()
			: base(new NativeImportApiSetUp())
		{
		}

		[CollectWebApiExecutionPlans]
		[CollectWebApiSql]
		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.Integration)]
		[IdentifiedTestCase("b756e4b0-0206-43e1-8508-6b1a65759e1b", 5)]
		[IdentifiedTestCase("6b6d0cf1-e9ce-4d6a-8e32-02af853efd3b", 1000)]
		[IdentifiedTestCase("1c9e735b-922d-49f2-8536-94b041917467", 5000)]
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

			this.InitializeImportApiWithUserAndPassword(NativeImportSettingsProvider.GetNativeFilePathSourceDocumentImportSettings());

			const int NumberOfFilesToImport = 5;
			IEnumerable<DefaultImportDto> importData = DefaultImportDto.GetRandomTextFiles(this.TempDirectory.Directory, NumberOfFilesToImport);

			// ACT
			this.Execute(importData);

			// ASSERT
			this.ThenTheImportJobIsSuccessful(numberOfDocuments);
		}
	}
}
