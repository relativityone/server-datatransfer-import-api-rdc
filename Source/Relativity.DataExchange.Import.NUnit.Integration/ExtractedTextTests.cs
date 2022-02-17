// -----------------------------------------------------------------------------------------------------
// <copyright file="ExtractedTextTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents positive import job tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit.Integration
{
	using System.Collections.Generic;
	using System.IO;
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using kCura.Relativity.DataReaderClient;

	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.Import.JobExecutionContext;
	using Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport;
	using Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport.FieldValueSources;
	using Relativity.DataExchange.TestFramework.NUnitExtensions;
	using Relativity.DataExchange.TestFramework.RelativityHelpers;
	using Relativity.DataExchange.TestFramework.RelativityVersions;
	using Relativity.DataExchange.Transfer;
	using Relativity.Services.Objects.DataContracts;
	using Relativity.Testing.Identification;

	[TestFixture]
	[Feature.DataTransfer.ImportApi.Operations.ImportDocuments]
	[TestType.MainFlow]
	[TestExecutionCategory.CI]
	public class ExtractedTextTests : ImportJobTestBase<NativeImportExecutionContext>
	{
		private const RelativityVersion MinSupportedVersion = RelativityVersion.Foxglove;

		private bool testsSkipped = false;
		private int originalImportBatchMaxVolume;

		[OneTimeSetUp]
		public async Task OneTimeSetUpAsync()
		{
			TapiClientModeAvailabilityChecker.InitializeTapiClient(TapiClient.Direct, TestParameters);

			testsSkipped = RelativityVersionChecker.VersionIsLowerThan(this.TestParameters, MinSupportedVersion);
			if (!testsSkipped)
			{
				await RdoHelper.DeleteAllObjectsByTypeAsync(this.TestParameters, (int)ArtifactType.Document).ConfigureAwait(false); // Remove all Documents imported in AssemblySetup
				this.originalImportBatchMaxVolume = AppSettings.Instance.ImportBatchMaxVolume;
				AppSettings.Instance.ImportBatchMaxVolume = 10000;
			}
		}

		[TearDown]
		public void TearDown()
		{
			if (!testsSkipped)
			{
				AppSettings.Instance.ImportBatchMaxVolume = this.originalImportBatchMaxVolume;
			}
		}

		[IgnoreIfVersionLowerThan(MinSupportedVersion)]
		[IdentifiedTest("E2D6FFF9-CD48-42BD-AB54-BA2270CF16CE")]
		[Feature.DataTransfer.ImportApi.Operations.ImportDocuments]
		public async Task ShouldImportTheFiles()
		{
			// ARRANGE
			var numberOfDocuments = 5;

			string documentFolder = await WorkspaceHelper.GetDefaultFileRepositoryAsync(this.TestParameters).ConfigureAwait(false);
			string documentWorkspaceFolder = Path.Combine(documentFolder, $"EDDS{this.TestParameters.WorkspaceId}");
			List<string> extractedTextFiles = new List<string>();
			for (int i = 0; i < numberOfDocuments; i++)
			{
				var file = await BcpFileHelper.CreateAsync(this.TestParameters, RandomHelper.NextString(3000, 4000), documentWorkspaceFolder).ConfigureAwait(false);
				extractedTextFiles.Add(Path.Combine(documentWorkspaceFolder, file));
			}

			Settings settings = NativeImportSettingsProvider.GetDefaultSettings();
			settings.LoadImportedFullTextFromServer = true;
			settings.LongTextColumnThatContainsPathToFullText = WellKnownFields.ExtractedText;

			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);

			IEnumerable<string> controlNumberSource = GetControlNumberEnumerable(
				OverwriteModeEnum.Append,
				numberOfDocuments,
				"ExtractedTextTests");

			ImportDataSource<object[]> importDataSource = ImportDataSourceBuilder.New()
				.AddField(WellKnownFields.ControlNumber, controlNumberSource)
				.AddField(WellKnownFields.ExtractedText, new MultipleValueSource(extractedTextFiles))
				.Build();

			// ACT
			ImportTestJobResult result = this.JobExecutionContext.Execute(importDataSource);

			// ASSERT
			this.ThenTheImportJobIsSuccessful(result, numberOfDocuments);
			Assert.That(result.NumberOfJobMessages, Is.Positive);
			Assert.That(result.NumberOfCompletedRows, Is.EqualTo(numberOfDocuments));
			Assert.That(result.BatchReports.Count, Is.GreaterThan(1));

			string[] fieldsToValidate = new[] { WellKnownFields.ControlNumber, WellKnownFields.ExtractedText };
			IList<RelativityObject> relativityObjects = await RdoHelper.QueryRelativityObjectsAsync(this.TestParameters, (int)ArtifactType.Document, fieldsToValidate).ConfigureAwait(false);
			Assert.That(relativityObjects.Count, Is.EqualTo(numberOfDocuments));
			ObjectsValidator.ThenObjectsFieldsAreImported(relativityObjects, fieldsToValidate);
		}
	}
}
