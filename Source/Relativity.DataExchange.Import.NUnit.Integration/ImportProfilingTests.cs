// <copyright file="ImportProfilingTests.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.Import.NUnit.Integration
{
	using System.Collections.Generic;
	using System.Linq;

	using global::NUnit.Framework;

	using kCura.Relativity.DataReaderClient;

	using Relativity.DataExchange.Import.NUnit.Integration.Dto;
	using Relativity.DataExchange.Import.NUnit.Integration.SetUp;
	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.Extensions;
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
			// ARRANGE
			const TapiClient Client = TapiClient.Direct;
			const bool DisableNativeLocationValidation = false;
			const bool DisableNativeValidation = false;

			TapiClientModeAvailabilityChecker.SkipTestIfModeNotAvailable(AssemblySetup.TestParameters, Client);

			ForceClient(Client);
			kCura.WinEDDS.Config.ConfigSettings["DisableNativeLocationValidation"] = DisableNativeLocationValidation;
			kCura.WinEDDS.Config.ConfigSettings["DisableNativeValidation"] = DisableNativeValidation;

			this.InitializeImportApiWithUserAndPassword(NativeImportSettingsProvider.GetNativeFilePathSourceDocumentImportSettings());

			IEnumerable<DefaultImportDto> importData = DefaultImportDto.GetRandomTextFiles(this.TempDirectory.Directory, numberOfDocuments);

			// ACT
			this.Execute(importData);

			// ASSERT
			this.ThenTheImportJobIsSuccessful(numberOfDocuments);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", MessageId = "2*numberOfDocuments", Justification = "It won't overflow for values used in test")]
		[CollectWebApiExecutionPlans]
		[CollectWebApiSql]
		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.Integration)]
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

			Settings settings = NativeImportSettingsProvider.GetDefaultNativeDocumentImportSettings();
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

			this.InitializeImportApiWithUserAndPassword(NativeImportSettingsProvider.GetDefaultNativeDocumentImportSettings());

			using (var dataReader = new ZipDataReader())
			{
				dataReader.Add(WellKnownFields.ControlNumber, Enumerable.Range((2 * numberOfDocuments) + 1, numberOfDocuments).Select(p => p.ToString()));

				dataReader.Add(WellKnownFields.FolderName, randomFolderGenerator.ToFolders(numberOfDocuments));
				dataReader.Add(WellKnownFields.ConfidentialDesignation, confidentialDesignation.ToEnumerable(numberOfDocuments));
				dataReader.Add(WellKnownFields.PrivilegeDesignation, privilegeDesignation.ToEnumerable(int.MaxValue, nestedValueDelimiter).RandomUniqueBatch(4, multiValueDelimiter));

				// ACT
				this.Execute(dataReader);
			}

			// ASSERT
			this.ThenTheImportJobIsSuccessful(numberOfDocuments);
		}
	}
}
