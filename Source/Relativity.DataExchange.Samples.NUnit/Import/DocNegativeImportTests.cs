﻿// ----------------------------------------------------------------------------
// <copyright file="DocNegativeImportTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Samples.NUnit.Import
{
	using global::NUnit.Framework;

	using Relativity.DataExchange.TestFramework;
	using Relativity.Testing.Identification;

	/// <summary>
	/// Represents tests that fails to import documents and validates the results.
	/// </summary>
	[TestFixture]
	[Feature.DataTransfer.ImportApi.Operations.ImportDocuments]
	[TestType.Error]
	public class DocNegativeImportTests : DocImportTestsBase
	{
		[IdentifiedTest("9be67303-3afd-4009-9558-b9b73a6c5ed3")]
		public void ShouldNotImportWhenTheFolderExceedsTheMaxLength()
		{
			// Arrange
			string controlNumber = GenerateControlNumber();
			string folder = "\\" + new string('x', IntegrationTestParameters.MaxFolderLength + 1);
			kCura.Relativity.DataReaderClient.ImportBulkArtifactJob job =
				this.ArrangeImportJob(controlNumber, folder, SampleDocPdfFileName);

			// Act
			job.Execute();

			// Assert - the job failed with a single non-fatal exception.
			this.AssertImportFailed(1);

			// Assert - exceeding the max folder length yields a doc-level error.
			this.AssertError(0, 1, controlNumber, IntegrationTestParameters.MaxFolderLength.ToString());
		}
	}
}