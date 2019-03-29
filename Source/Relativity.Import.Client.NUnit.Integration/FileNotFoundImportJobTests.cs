﻿// -----------------------------------------------------------------------------------------------------
// <copyright file="FileNotFoundImportJobTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents file not found related import tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Import.Client.NUnit.Integration
{
	using global::NUnit.Framework;

	using kCura.WinEDDS.TApi;

	using Relativity.Import.Export.TestFramework;

	using Relativity.Testing.Identification;

	/// <summary>
	/// Represents file not found related import tests.
	/// </summary>
	[TestFixture]
	[Feature.DataTransfer.ImportApi.Operations.ImportDocuments]
	public class FileNotFoundImportJobTests : ImportJobTestBase
	{
		/// <summary>
		/// Should import the files.
		/// </summary>
		/// <param name="client">
		/// The transfer client to force.
		/// </param>
		/// <param name="disableNativeLocationValidation">
		/// Specify whether to disable native location validation.
		/// </param>
		/// <param name="disableNativeValidation">
		/// Specify whether to disable native validation.
		/// </param>
		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.Integration)]
		[Category(TestCategories.TransferApi)]
		[IdentifiedTestCase("9361b3aa-1333-4606-a05c-9b89ef43a2dd", TapiClient.Direct, false, true)]
		[IdentifiedTestCase("b4f90f5b-cf18-4d8a-bc42-259906bd27fd", TapiClient.Direct, true, true)]
		[IdentifiedTestCase("bbee1077-fc95-42aa-a625-2caad809952a", TapiClient.Web, false, true)]
		[IdentifiedTestCase("d1873516-42ad-476f-820d-b790ee3d6c3d", TapiClient.Web, true, true)]
		[IdentifiedTestCase("2d61508b-2622-4b9b-a824-61387796b09b", TapiClient.Aspera, false, true)]
		[IdentifiedTestCase("6d2220f5-4b59-4989-8a21-cca0559894d3", TapiClient.Aspera, true, true)]
		public void ShouldFailWhenTheFileIsNotFound(
			TapiClient client,
			bool disableNativeLocationValidation,
			bool disableNativeValidation)
		{
			const int AutoGeneratedSourceFiles = 5;
			if ((client == TapiClient.Aspera && this.TestParameters.SkipAsperaModeTests) ||
				(client == TapiClient.Direct && this.TestParameters.SkipDirectModeTests))
			{
				Assert.Ignore(TestStrings.SkipTestMessage, $"{client}");
			}

			// Intentionally provide an invalid file before adding valid ones.
			this.GivenTheDatasetPathToImport(@"C:\abcdefghijklmnop\out.txt");
			this.GivenTheAutoGeneratedDatasetToImport(AutoGeneratedSourceFiles, true);
			this.GivenTheImportJob();
			GivenTheStandardConfigSettings(client, disableNativeLocationValidation, disableNativeValidation);
			this.WhenExecutingTheJob();
			this.ThenTheImportJobIsNotSuccessful(disableNativeLocationValidation ? 0 : 1, AutoGeneratedSourceFiles + 1, false);
			this.ThenTheImportProgressEventsCountShouldEqual(AutoGeneratedSourceFiles + 1);
			this.ThenTheImportMessageCountIsNonZero();
			if (disableNativeLocationValidation)
			{
				this.ThenTheImportMessagesContains("does not exist");
			}
		}
	}
}