// <copyright file="ImageImportProfilingTests.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.Import.NUnit.Integration.DevTests
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using kCura.Relativity.DataReaderClient;

	using Relativity.DataExchange.Import.NUnit.Integration.Dto;
	using Relativity.DataExchange.Media;
	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.Import.JobExecutionContext;
	using Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport;
	using Relativity.DataExchange.TestFramework.NUnitExtensions;
	using Relativity.Testing.Identification;

	[TestFixture]
	[Explicit]
	[Feature.DataTransfer.ImportApi.Operations.ImportImages]
	[Feature.DataTransfer.TransferApi]
	public class ImageImportProfilingTests : ImportJobTestBase<ImageImportExecutionContext>
	{
		[CollectWebApiExecutionPlans]
		[CollectWebApiSql]
		[IdentifiedTestCase("9dd4459f-6c28-4162-b0ed-e7b19176f4b0", 40, 1000)]
		[IdentifiedTestCase("57f26236-a9a2-4a08-85df-c65a3c604d0c", 400, 100)]
		[IdentifiedTestCase("de509fc5-f5fc-45ff-8b4e-0ca9bf4a4874", 4000, 10)]
		[TestType.MainFlow]
		[Pairwise]
		public async Task ShouldImportManyImages(int numberOfDocumentsToImport, int numberOfImagesPerDocument)
		{
			// ARRANGE
			await this.ResetContextAsync().ConfigureAwait(false);

			IEnumerable<ImageImportWithFileNameDto> importData = new ImageImportWithFileNameDtoBuilder(
				this.TempDirectory.Directory,
				numberOfDocumentsToImport,
				numberOfImagesPerDocument,
				ImageFormat.Jpeg).Build();

			var imageSettingsBuilder = new ImageSettingsBuilder()
				.WithDefaultFieldNames()
				.WithOverlayMode(OverwriteModeEnum.AppendOverlay);

			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, imageSettingsBuilder);
			this.JobExecutionContext.UseFileNames = true;

			// ACT
			ImportTestJobResult results = this.JobExecutionContext.Execute(importData);

			// ASSERT
			int expectedNumberOfImportedImages = numberOfDocumentsToImport * numberOfImagesPerDocument;
			this.ThenTheImportJobIsSuccessful(results, expectedNumberOfImportedImages);
		}

		[CollectWebApiExecutionPlans]
		[CollectWebApiSql]
		[IdentifiedTestCase("4e30d60e-b160-4bb6-b62a-86b334d1b23a", 400, 100)]
		[TestType.Error]
		[Pairwise]
		public async Task ShouldImportManyImagesWithAppendErrors(int numberOfDocumentsToImport, int numberOfImagesPerDocument)
		{
			// ARRANGE
			await this.ResetContextAsync().ConfigureAwait(false);

			IEnumerable<ImageImportWithFileNameDto> importData = new ImageImportWithFileNameDtoBuilder(
				this.TempDirectory.Directory,
				numberOfDocumentsToImport,
				numberOfImagesPerDocument,
				ImageFormat.Jpeg).Build();

			var imageSettingsBuilder = new ImageSettingsBuilder()
				.WithDefaultFieldNames()
				.WithOverlayMode(OverwriteModeEnum.Append);

			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, imageSettingsBuilder);
			this.JobExecutionContext.UseFileNames = true;

			this.JobExecutionContext.Execute(importData);

			// ACT
			ImportTestJobResult results = this.JobExecutionContext.Execute(importData);

			// ASSERT
			int expectedNumberOfImportedImages = numberOfDocumentsToImport * numberOfImagesPerDocument;
			int expectedNumberOfErrors = Math.Min(expectedNumberOfImportedImages, WellKnownFields.MaximumErrorCount + 1);  // extra one error includes information that error limit was reached
			this.ThenTheImportJobCompletedWithErrors(results, expectedNumberOfErrors, expectedNumberOfImportedImages);
		}
	}
}