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

	using kCura.WinEDDS;

	using global::NUnit.Framework;

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
			Relativity.CaseInfo caseInfo = await this.WhenGettingTheWorkspaceInfoAsync();
			this.GivenTheSelectedFolderId(caseInfo.RootFolderID);
			this.GivenTheIdentifierColumnName(WellKnownFields.ControlNumber);
			this.GivenTheEncoding(Encoding.Unicode);
			await this.WhenCreatingTheExportFileAsync(caseInfo);
			this.WhenExecutingTheExportSearch();
			this.ThenTheSearchResultShouldEqual(true);
			this.ThenTheAlertCriticalErrorsCountShouldEqual(0);
		}
	}
}