// ----------------------------------------------------------------------------
// <copyright file="ObjectNegativeImportTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Samples.NUnit.Import
{
	using System;
	using System.Linq;

	using global::NUnit.Framework;

	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.NUnitExtensions;
	using Relativity.DataExchange.TestFramework.RelativityVersions;
	using Relativity.Testing.Identification;

	/// <summary>
	/// Represents tests that fails to import objects and validates the results.
	/// </summary>
	[TestFixture]
	[Feature.DataTransfer.ImportApi.Operations.ImportRDOs]
	[TestType.Error]
	public class ObjectNegativeImportTests : ObjectImportTestsBase
	{
		[IdentifiedTest("00c1b76f-d6d7-407f-b0c0-c84effaffeb2")]
		public void ShouldReportItemLevelErrorWhenImportingDuplicatedSingleObjectFields()
		{
			// Arrange
			const string Name = "Negative-Transfer-1";
			const string DetailName = "Negative-Detail-1";
			const string DataSourceName = "Negative-DataSourceName-1";

			this.CreateAssociatedDetailInstance(DetailName);
			this.CreateAssociatedDetailInstance(DetailName);
			this.CreateAssociatedDataSourceInstance(DataSourceName);
			kCura.Relativity.DataReaderClient.ImportBulkArtifactJob job = this.CreateImportBulkArtifactJob();
			string description = RandomHelper.NextString(50, 450);
			decimal requestBytes = RandomHelper.NextDecimal(10, 1000000);
			DateTime requestDate = DateTime.Now;
			decimal requestFiles = RandomHelper.NextDecimal(1000, 10000);
			this.DataSource.Rows.Add(
				Name,
				description,
				requestBytes,
				requestFiles,
				requestDate,
				DetailName,
				DataSourceName);
			job.SourceData.SourceData = this.DataSource.CreateDataReader();

			// Act
			job.Execute();

			// Assert - the job completed and the report matches the expected values.
			Assert.That(this.PublishedJobReport, Is.Not.Null);
			Assert.That(this.PublishedJobReport.EndTime, Is.GreaterThan(this.PublishedJobReport.StartTime));

			// Assert - duplicate single-object field yields a job-level error.
			Assert.That(this.PublishedJobReport.FatalException, Is.Null);
			Assert.That(this.PublishedJobReport.ErrorRowCount, Is.Positive);
			Assert.That(this.PublishedJobReport.TotalRows, Is.EqualTo(1));

			// Assert - the events match the expected values.
			Assert.That(this.PublishedErrors.Count, Is.Positive);
			Assert.That(this.PublishedFatalException, Is.Null);
			Assert.That(this.PublishedMessages.Count, Is.Positive);
			Assert.That(this.PublishedProcessProgress.Count, Is.Positive);
			Assert.That(this.PublishedProgressRows.Count, Is.Positive);
		}

		[IdentifiedTest("994ac3ba-dd7e-4130-8191-89bae32361c4")]
		[IgnoreIfVersionGreaterOrEqual(RelativityVersion.Foxglove)]
		public void ShouldReportFatalExceptionWithGenericErrorWhenImportingDuplicatedMultiObjectFieldsBeforeFoxglove()
		{
			// Arrange & Act
			this.ImportDuplicatedMultiObjectFields();

			// Assert
			const string ExpectedExceptionMessage = "Subquery returned more than 1 value";
			this.AssertFatalExceptionOccuredDuringImport(ExpectedExceptionMessage);
		}

		[IdentifiedTest("1e1266ad-7e1c-4bf3-8ed3-cf3f8b8ee2d5")]
		[IgnoreIfVersionLowerThan(RelativityVersion.Foxglove)]
		[IgnoreIfVersionGreaterOrEqual(RelativityVersion.Mayapple)]
		public void ShouldReportFatalExceptionWithMeaningfulErrorWhenImportingDuplicatedMultiObjectFieldsSinceFoxgloveBeforeMayapple()
		{
			// Arrange & Act
			this.ImportDuplicatedMultiObjectFields();

			// Assert
			const string ExpectedExceptionMessage = "Failed to create the associated multi-object artifact";
			this.AssertFatalExceptionOccuredDuringImport(ExpectedExceptionMessage);
		}

		[IdentifiedTest("73d8f3b3-feef-4cc1-9fa1-17ba4b0ac64a")]
		[IgnoreIfMassImportImprovementsToggleHasValue(isEnabled: true)]
		[IgnoreIfVersionLowerThan(RelativityVersion.Mayapple)]
		public void ShouldReportFatalExceptionWithMeaningfulErrorWhenImportingDuplicatedMultiObjectFieldsSinceMayappleForToggleOff()
		{
			// Arrange & Act
			this.ImportDuplicatedMultiObjectFields();

			// Assert
			const string ExpectedExceptionMessage = "Failed to create the associated multi-object artifact";
			this.AssertFatalExceptionOccuredDuringImport(ExpectedExceptionMessage);
		}

		[IdentifiedTest("1199da43-19bc-4f6d-862b-272e00bc1bae")]
		[IgnoreIfMassImportImprovementsToggleHasValue(isEnabled: false)]
		[IgnoreIfVersionLowerThan(RelativityVersion.Mayapple)]
		public void ShouldReportItemLevelErrorWhenImportingDuplicatedMultiObjectFieldsSinceMayappleForToggleOn()
		{
			// Arrange & Act
			string identifierValue = this.ImportDuplicatedMultiObjectFields();

			// Assert - the job completed and the report matches the expected values.
			Assert.That(this.PublishedJobReport, Is.Not.Null);
			Assert.That(this.PublishedJobReport.EndTime, Is.GreaterThan(this.PublishedJobReport.StartTime));

			// Assert - duplicate multi-object field currently yields a item-level error.
			const string ExpectedItemLevelErrorMessage = " - A non unique associated object is specified for this new object";

			Assert.That(this.PublishedJobReport.FatalException, Is.Null);
			Assert.That(this.PublishedJobReport.ErrorRowCount, Is.EqualTo(1));
			Assert.That(this.PublishedJobReport.ErrorRows.Single().Message, Is.EqualTo(ExpectedItemLevelErrorMessage));
			Assert.That(this.PublishedJobReport.ErrorRows.Single().Identifier, Is.EqualTo(identifierValue));
			Assert.That(this.PublishedJobReport.TotalRows, Is.EqualTo(1));

			// Assert - the events match the expected values.
			Assert.That(this.PublishedErrors.Count, Is.EqualTo(1));
			Assert.That(this.PublishedFatalException, Is.Null);
			Assert.That(this.PublishedMessages.Count, Is.Positive);
			Assert.That(this.PublishedProcessProgress.Count, Is.Positive);
			Assert.That(this.PublishedProgressRows.Count, Is.Positive);
		}

		private string ImportDuplicatedMultiObjectFields()
		{
			// Arrange
			const string IdentifierValue = "Negative-Transfer-2";
			const string DetailName = "Negative-Detail-2";
			const string DataSourceName = "Negative-DataSourceName-2";

			this.CreateAssociatedDetailInstance(DetailName);
			this.CreateAssociatedDataSourceInstance(DataSourceName);
			this.CreateAssociatedDataSourceInstance(DataSourceName);
			kCura.Relativity.DataReaderClient.ImportBulkArtifactJob job = this.CreateImportBulkArtifactJob();
			string description = RandomHelper.NextString(50, 450);
			decimal requestBytes = RandomHelper.NextDecimal(10, 1000000);
			DateTime requestDate = DateTime.Now;
			decimal requestFiles = RandomHelper.NextDecimal(1000, 10000);
			this.DataSource.Rows.Add(
				IdentifierValue,
				description,
				requestBytes,
				requestFiles,
				requestDate,
				DetailName,
				DataSourceName);
			job.SourceData.SourceData = this.DataSource.CreateDataReader();

			// Act
			job.Execute();

			return IdentifierValue;
		}

		private void AssertFatalExceptionOccuredDuringImport(string expectedExceptionMessage)
		{
			// Assert - the job completed and the report matches the expected values.
			Assert.That(this.PublishedJobReport, Is.Not.Null);
			Assert.That(this.PublishedJobReport.EndTime, Is.GreaterThan(this.PublishedJobReport.StartTime));

			// Assert - fatal exception occured, no item level errors
			Assert.That(this.PublishedJobReport.FatalException, Is.Not.Null);
			Assert.That(this.PublishedJobReport.FatalException.Message, Contains.Substring(expectedExceptionMessage));
			Assert.That(this.PublishedJobReport.ErrorRowCount, Is.Zero);
			Assert.That(this.PublishedJobReport.TotalRows, Is.EqualTo(1));

			// Assert - the events match the expected values.
			Assert.That(this.PublishedErrors.Count, Is.Zero);
			Assert.That(this.PublishedFatalException, Is.Not.Null);
			Assert.That(this.PublishedMessages.Count, Is.Positive);
			Assert.That(this.PublishedProcessProgress.Count, Is.Positive);
			Assert.That(this.PublishedProgressRows.Count, Is.Positive);
		}
	}
}