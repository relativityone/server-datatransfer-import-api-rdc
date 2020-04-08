// // ----------------------------------------------------------------------------
// <copyright file="ImportImagesLoadTests.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>
// // ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit.LoadTests
{
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using kCura.Relativity.DataReaderClient;

	using Relativity.DataExchange.Import.NUnit.Integration;
	using Relativity.DataExchange.Media;
	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.Import.JobExecutionContext;
	using Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport;
	using Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport.FieldValueSources;
	using Relativity.DataExchange.TestFramework.NUnitExtensions;
	using Relativity.DataExchange.Transfer;
	using Relativity.Testing.Identification;

	[Explicit]
	[TestFixture]
	[Feature.DataTransfer.ImportApi.Operations.ImportImages]
	public class ImportImagesLoadTests : ImportLoadTestsBase<ImageImportExecutionContext, ImageSettings>
	{
		[CollectDeadlocks]
		[Category(TestCategories.ImportImage)]
		[Category(TestCategories.TransferApi)]
		[IdentifiedTestCase("6a32530b-a6ca-4f78-9ee9-0875f90ae49e")]
		public async Task ShouldImportImagesParallelAsync(
			[Values(16, 8)] int parallelIApiClientCount,
			[Values(100_000)] int numberOfDocumentsPerIApiClient,
			[Values(1, 10)] int numberOfImagesPerDocument,
			[Values(TapiClient.Aspera, TapiClient.Direct, TapiClient.Web)] TapiClient client)
		{
			// ARRANGE
			TapiClientModeAvailabilityChecker.SkipTestIfModeNotAvailable(AssemblySetup.TestParameters, client);
			ForceClient(client);
			const ImageFormat ImageFormat = ImageFormat.Tiff;

			var settingsBuilder = new ImageSettingsBuilder()
				.WithWellKnownFieldNames()
				.WithOverlayMode(OverwriteModeEnum.AppendOverlay);

			var dataSourceBuilder = new ImportDataSourceBuilder();
			dataSourceBuilder.AddField(WellKnownFields.BatesNumber, new IdentifierValueSource());
			dataSourceBuilder.AddField(WellKnownFields.DocumentIdentifier, new IdentifierValueSource(numberOfImagesPerDocument));
			dataSourceBuilder.AddField(WellKnownFields.FileName, new FileNameValueSource(ImageFormat.ToString()));
			dataSourceBuilder.AddField(WellKnownFields.FileLocation, new ImageLocationValueSource(this.TempDirectory.Directory, ImageFormat, numberOfImagesPerDocument));

			this.InitializeImportApiWithUserAndPwd(settingsBuilder, parallelIApiClientCount);

			// ACT
			ImportTestJobResult results = await this.JobExecutionContext
											  .ExecuteAsync(dataSourceBuilder, numberOfDocumentsPerIApiClient * numberOfImagesPerDocument)
											  .ConfigureAwait(false);

			// ASSERT
			int expectedNumberOfImportedImages = numberOfImagesPerDocument * numberOfDocumentsPerIApiClient * parallelIApiClientCount;
			this.ThenTheImportJobIsSuccessful(results, expectedNumberOfImportedImages);
			Assert.That(results.NumberOfJobMessages, Is.Positive);
		}
	}
}