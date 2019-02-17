// -----------------------------------------------------------------------------------------------------
// <copyright file="ExporterTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="Exporter"/> tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Import.Client.NUnit.Integration
{
	using System.Text;
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using kCura.WinEDDS;

	using Relativity.ImportExport.UnitTestFramework;

	/// <summary>
	/// Represents <see cref="Exporter"/> tests.
	/// </summary>
	[TestFixture]
	public class ExporterTests : ExporterTestBase
	{
		[Test]
		[Category(TestCategories.Export)]
		[Category(TestCategories.Integration)]
		public async Task ShouldExportAsync()
		{
			this.GivenTheExportType(ExportFile.ExportType.ParentSearch);
			Relativity.CaseInfo caseInfo = await this.WhenGettingTheWorkspaceInfoAsync().ConfigureAwait(false);
			this.GivenTheSelectedFolderId(caseInfo.RootFolderID);
			this.GivenTheIdentifierColumnName(WellKnownFields.ControlNumber);
			this.GivenTheEncoding(Encoding.Unicode);
			await this.WhenCreatingTheExportFileAsync(caseInfo).ConfigureAwait(false);
			this.WhenExecutingTheExportSearch();
			this.ThenTheSearchResultShouldEqual(true);
			this.ThenTheAlertCriticalErrorsCountShouldEqual(0);
			this.ThenTheAlertsCountShouldEqual(0);
			this.ThenTheAlertWarningSkippablesCountShouldEqual(0);
			this.ThenTheFatalErrorsCountShouldEqual(0);
			this.ThenTheStatusMessagesCountShouldBeNonZero();
		}
	}
}