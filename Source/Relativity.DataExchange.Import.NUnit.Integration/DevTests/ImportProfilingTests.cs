// <copyright file="ImportProfilingTests.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.Import.NUnit.Integration.DevTests
{
	using System.Collections.Generic;
	using System.Linq;

	using global::NUnit.Framework;

	using kCura.Relativity.DataReaderClient;

	using Relativity.DataExchange.Import.NUnit.Integration.Dto;
	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.Extensions;
	using Relativity.DataExchange.TestFramework.Import.JobExecutionContext;
	using Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport;
	using Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport.FieldValueSources;
	using Relativity.DataExchange.TestFramework.NUnitExtensions;
	using Relativity.DataExchange.Transfer;
	using Relativity.Testing.Identification;

	[TestFixture]
	[Explicit]
	[Feature.DataTransfer.ImportApi.Operations.ImportDocuments]
	public class ImportProfilingTests : ImportJobTestBase<NativeImportExecutionContext>
	{
		[CollectWebApiExecutionPlans]
		[CollectWebApiSql]
		[Category(TestCategories.ImportDoc)]
		[IdentifiedTestCase("b756e4b0-0206-43e1-8508-6b1a65759e1b", 5)]
		[IdentifiedTestCase("6b6d0cf1-e9ce-4d6a-8e32-02af853efd3b", 1000)]
		[IdentifiedTestCase("1c9e735b-922d-49f2-8536-94b041917467", 5000)]
		public void SimpleImportTestCase(int numberOfDocuments)
		{
			// ARRANGE
			const TapiClient Client = TapiClient.Direct;
			const bool DisableNativeLocationValidation = false;
			const bool DisableNativeValidation = false;

			TapiClientModeAvailabilityChecker.SkipTestIfModeNotAvailable(AssemblySetup.TestParameters, Client);

			ForceClient(Client);
			kCura.WinEDDS.Config.ConfigSettings["DisableNativeLocationValidation"] = DisableNativeLocationValidation;
			kCura.WinEDDS.Config.ConfigSettings["DisableNativeValidation"] = DisableNativeValidation;

			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, NativeImportSettingsProvider.NativeFilePathSourceDocumentImportSettings);

			IEnumerable<DefaultImportDto> importData = DefaultImportDto.GetRandomTextFiles(this.TempDirectory.Directory, numberOfDocuments);

			// ACT
			ImportTestJobResult result = this.JobExecutionContext.Execute(importData);

			// ASSERT
			this.ThenTheImportJobIsSuccessful(result, numberOfDocuments);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", MessageId = "2*numberOfDocuments", Justification = "It won't overflow for values used in test")]
		[CollectWebApiExecutionPlans]
		[CollectWebApiSql]
		[Category(TestCategories.ImportDoc)]
		[IdentifiedTestCase("1c9e735b-922d-49f2-8536-94b041917467", 5000)]
		public void FoldersAndChoicesTestCase(int numberOfDocuments)
		{
			// ARRANGE
			const TapiClient Client = TapiClient.Direct;
			const bool DisableNativeLocationValidation = false;
			const bool DisableNativeValidation = false;

			TapiClientModeAvailabilityChecker.SkipTestIfModeNotAvailable(AssemblySetup.TestParameters, Client);

			ForceClient(Client);
			kCura.WinEDDS.Config.ConfigSettings["DisableNativeLocationValidation"] = DisableNativeLocationValidation;
			kCura.WinEDDS.Config.ConfigSettings["DisableNativeValidation"] = DisableNativeValidation;

			Settings settings = NativeImportSettingsProvider.DefaultNativeDocumentImportSettings;
			char multiValueDelimiter = settings.MultiValueDelimiter;
			char nestedValueDelimiter = settings.NestedValueDelimiter;

			settings.FolderPathSourceFieldName = WellKnownFields.FolderName;
			RandomPathGenerator randomFolderGenerator = RandomPathGenerator.GetFolderGenerator(
				maxPathDepth: 4,
				numOfDifferentElements: 100,
				numOfDifferentPaths: 1000,
				maxElementLength: 50);

			RandomPathGenerator confidentialDesignation = RandomPathGenerator.GetChoiceGenerator(
				maxPathDepth: 1,
				numOfDifferentElements: 100,
				numOfDifferentPaths: 100,
				maxElementLength: 200,
				multiValueDelimiter: multiValueDelimiter,
				nestedValueDelimiter: nestedValueDelimiter);

			RandomPathGenerator privilegeDesignation = RandomPathGenerator.GetChoiceGenerator(
				maxPathDepth: 2,
				numOfDifferentElements: 250,
				numOfDifferentPaths: 100,
				maxElementLength: 200,
				multiValueDelimiter: multiValueDelimiter,
				nestedValueDelimiter: nestedValueDelimiter);

			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, NativeImportSettingsProvider.DefaultNativeDocumentImportSettings);

			ImportDataSource<object[]> dataSource = ImportDataSourceBuilder.New()
				.AddField(WellKnownFields.ControlNumber, Enumerable.Range((2 * numberOfDocuments) + 1, numberOfDocuments).Select(p => p.ToString()))
				.AddField(WellKnownFields.FolderName, randomFolderGenerator.ToFolders())
				.AddField(WellKnownFields.ConfidentialDesignation, confidentialDesignation.ToEnumerable())
				.AddField(WellKnownFields.PrivilegeDesignation, privilegeDesignation.ToEnumerable(nestedValueDelimiter).RandomUniqueBatch(4, multiValueDelimiter))
				.Build();

			// ACT
			ImportTestJobResult result = this.JobExecutionContext.Execute(dataSource);

			// ASSERT
			this.ThenTheImportJobIsSuccessful(result, numberOfDocuments);
		}
	}
}
