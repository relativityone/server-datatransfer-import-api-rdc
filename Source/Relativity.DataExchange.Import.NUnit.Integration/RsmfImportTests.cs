// ----------------------------------------------------------------------------
// <copyright file="RsmfImportTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit.Integration
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using global::NUnit.Framework;
	using kCura.Relativity.DataReaderClient;
	using Relativity.DataExchange.Import.NUnit.Integration.Dto;
	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.Import.JobExecutionContext;
	using Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport;
	using Relativity.DataExchange.TestFramework.RelativityHelpers;
	using Relativity.Services.Objects.DataContracts;
	using Relativity.Testing.Identification;

	[TestFixture]
	[Feature.DataTransfer.ImportApi.Operations.ImportDocuments]
	[TestType.MainFlow]
	[TestExecutionCategory.CI]
	public class RsmfImportTests : ImportJobTestBase<NativeImportExecutionContext>
	{
		[Category(TestCategories.Regression)]
		[IdentifiedTest("EDFE548C-7EC2-4A92-93DF-243B39DA3FE7")]
		[Feature.DataTransfer.ImportApi.Operations.ImportDocuments]
		public void ShouldImportDocumentsWithMetadataFileId()
		{
			const int NumberOfRecords = 10;
			Settings settings = NativeImportSettingsProvider.GetRsmfSettings();

			// ARRANGE
			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);

			IEnumerable<string> controlNumberSource = GetControlNumberEnumerable(
				OverwriteModeEnum.Append,
				NumberOfRecords,
				nameof(ShouldImportDocumentsWithMetadataFileId));
			IEnumerable<string> metadataFileIds = Enumerable.Range(1, NumberOfRecords).Select(p => Guid.NewGuid().ToString());

			ImportDataSource<object[]> importDataSource = ImportDataSourceBuilder.New()
				.AddField(WellKnownFields.ControlNumber, controlNumberSource)
				.AddField(WellKnownFields.MetadataFileId, metadataFileIds)
				.Build();

			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, settings);

			// ACT
			ImportTestJobResult results = this.JobExecutionContext.Execute(importDataSource);

			// ASSERT
			this.ThenTheImportJobIsSuccessful(results, NumberOfRecords);
			Assert.That(results.NumberOfJobMessages, Is.GreaterThan(0));

			Assert.That(results.NumberOfCompletedRows, Is.EqualTo(NumberOfRecords));
			ThenTheImportJobIsSuccessful(results, NumberOfRecords);

			IList<RelativityObject> relativityObjects = RdoHelper.QueryRelativityObjects(this.TestParameters, (int)ArtifactType.Document, fields: new[] { WellKnownFields.ControlNumber, WellKnownFields.HasNative });
			Assert.That(relativityObjects.Count, Is.EqualTo(NumberOfRecords));

			Assert.That(relativityObjects.All(item => (bool)item.FieldValues[1].Value == false));
		}
	}
}
