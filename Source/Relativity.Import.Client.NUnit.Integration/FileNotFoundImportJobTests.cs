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

	using Relativity.Import.Export.TestFramework;
	using Relativity.Import.Export.Transfer;

	/// <summary>
	/// Represents file not found related import tests.
	/// </summary>
	[TestFixture]
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
		[Test]
		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.Integration)]
		[Category(TestCategories.TransferApi)]
		[TestCase(TapiClient.Direct, false, true)]
		[TestCase(TapiClient.Direct, true, true)]
		[TestCase(TapiClient.Web, false, true)]
		[TestCase(TapiClient.Web, true, true)]
		[TestCase(TapiClient.Aspera, false, true)]
		[TestCase(TapiClient.Aspera, true, true)]
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