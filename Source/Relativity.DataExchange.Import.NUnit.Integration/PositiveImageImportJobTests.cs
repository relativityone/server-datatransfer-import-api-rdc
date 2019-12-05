// -----------------------------------------------------------------------------------------------------
// <copyright file="PositiveImageImportJobTests.cs" company="Relativity ODA LLC">
//   � Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents positive import job tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit.Integration
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Globalization;
	using System.Linq;

	using global::NUnit.Framework;

	using Relativity.DataExchange.Import.NUnit.Integration.Dto;
	using Relativity.DataExchange.Media;
	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.Transfer;
	using Relativity.Testing.Identification;

	[TestFixture]
	[Feature.DataTransfer.ImportApi.Operations.ImportDocuments]
	public class PositiveImageImportJobTests : ImageImportJobTestBase
	{
		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.Integration)]
		[Category(TestCategories.TransferApi)]
		[IdentifiedTest("9db2e7f4-0bc8-46a8-9e95-621ca9bcc5c1")]
		[Test]
		[Pairwise]
		public void ShouldImportTheFiles(
			[Values(TapiClient.Aspera, TapiClient.Direct, TapiClient.Web)] TapiClient client,
			[Values(true, false)] bool disableImageLocationValidation,
			[Values(true, false)] bool disableImageTypeValidation,
			[Values(ImageFormat.Jpeg, ImageFormat.Tiff)] ImageFormat imageFormat)
		{
			// ARRANGE
			TapiClientModeAvailabilityChecker.SkipTestIfModeNotAvailable(AssemblySetup.TestParameters, client);

			ForceClient(client);
			AppSettings.Instance.DisableImageLocationValidation = disableImageLocationValidation;
			AppSettings.Instance.DisableImageTypeValidation = disableImageTypeValidation;

			this.GivenTheImportJob();
			this.GiveImageFilePathSourceDocumentImportJob();

			const int NumberOfDocumentsToImport = 5;
			const int NumberOfImagesPerDocument = 3;

			using (var dataTable = new DataTable())
			{
				dataTable.Locale = CultureInfo.InvariantCulture;
				GenerateRandomDataTable(dataTable, this.TempDirectory.Directory, NumberOfDocumentsToImport, NumberOfImagesPerDocument, imageFormat);

				// ACT
				this.WhenExecutingTheJob(dataTable);
			}

			// ASSERT
			const int TotalNumberOfImagesToImport = NumberOfDocumentsToImport * NumberOfImagesPerDocument;
			this.ThenTheImportJobIsSuccessful(TotalNumberOfImagesToImport);
			Assert.That(this.TestJobResult.JobMessages, Has.Count.Positive);

			// test result job returns ProgressCompletedRows larger by 1 than real files count
			Assert.That(this.TestJobResult.ProgressCompletedRows, Has.Count.EqualTo(TotalNumberOfImagesToImport + 1));
		}

		private static void GenerateRandomDataTable(DataTable dataTable, string directory, int numberOfDocumentsToImport, int numberOfImagesPerDocument, ImageFormat imageFormat)
		{
			dataTable.Columns.Add("BatesNumber", typeof(string));
			dataTable.Columns.Add("DocumentIdentifier", typeof(string));
			dataTable.Columns.Add("FileLocation", typeof(string));

			for (int i = 1; i <= numberOfDocumentsToImport; i++)
			{
				string documentIdentifier = $"{Guid.NewGuid():D}";
				for (int j = 1; j <= numberOfImagesPerDocument; j++)
				{
					dataTable.Rows.Add($"{documentIdentifier}_{j}", $"{documentIdentifier}", RandomHelper.NextImageFile(imageFormat, directory));
				}
			}
		}
	}
}