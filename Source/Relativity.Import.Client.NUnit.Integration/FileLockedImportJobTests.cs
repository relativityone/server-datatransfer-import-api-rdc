﻿// -----------------------------------------------------------------------------------------------------
// <copyright file="FileLockedImportJobTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents file lock related import tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Import.Client.NUnit.Integration
{
	using global::NUnit.Framework;

	using kCura.WinEDDS.TApi;

	using Relativity.Import.Export.TestFramework;

    using Relativity.Testing.Identification;

    /// <summary>
    /// Represents file lock related import tests.
    /// </summary>
    [TestFixture]
    [Feature.DataTransfer.ImportApi.Operations.ImportDocuments]
	public class FileLockedImportJobTests : ImportJobTestBase
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
		[IdentifiedTestCase("e41a8da9-c562-467e-a638-5afa05b59224", TapiClient.Direct, false, true)]
		[IdentifiedTestCase("4bfa1999-7ba2-4c66-bd07-cfb9dbe56b23", TapiClient.Direct, true, true)]
		[IdentifiedTestCase("1b7e9bbc-eb2e-4eae-9d07-11a03778fa7d", TapiClient.Web, false, true)]
		[IdentifiedTestCase("267f10e9-418e-4ec7-83ca-19dadf604531", TapiClient.Web, true, true)]
		[IdentifiedTestCase("269aca91-7acd-42bd-96f2-2422bf9f4c4b", TapiClient.Aspera, false, true)]
		[IdentifiedTestCase("f3661eb4-ca4a-4b36-b0c5-0042780c184a", TapiClient.Aspera, true, true)]
		public void ShouldFailWhenTheFileIsLocked(TapiClient client, bool disableNativeLocationValidation, bool disableNativeValidation)
		{
			const int AutoGeneratedSourceFiles = 5;
			if ((client == TapiClient.Aspera && this.TestParameters.SkipAsperaModeTests) ||
			    (client == TapiClient.Direct && this.TestParameters.SkipDirectModeTests))
			{
				Assert.Ignore(TestStrings.SkipTestMessage, $"{client}");
			}

			// Intentionally provide an invalid file before adding valid ones.
			this.GivenTheAutoGeneratedDatasetToImport(AutoGeneratedSourceFiles, false);
			this.GivenTheSourceFileIsLocked(2);
			this.GivenTheImportJob();
			GivenTheStandardConfigSettings(client, disableNativeLocationValidation, disableNativeValidation);
			this.WhenExecutingTheJob();
			this.ThenTheImportJobIsNotSuccessful(0, AutoGeneratedSourceFiles, true);

			// The exact value is impossible to predict.
			this.ThenTheImportProgressEventsCountIsNonZero();
			this.ThenTheImportMessageCountIsNonZero();
		}
	}
}