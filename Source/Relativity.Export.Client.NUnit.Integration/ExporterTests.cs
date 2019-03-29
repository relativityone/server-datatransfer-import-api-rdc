// -----------------------------------------------------------------------------------------------------
// <copyright file="ExporterTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="Exporter"/> tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Export.Client.NUnit.Integration
{
	using System.Collections.Generic;
	using System.Text;
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using kCura.WinEDDS;

	using Relativity.Import.Export.TestFramework;

	using Relativity.Testing.Identification;

	/// <summary>
	/// Represents <see cref="Exporter"/> tests.
	/// </summary>
	[TestFixture]
	[Feature.DataTransfer.RelativityDesktopClient.Export]
	public class ExporterTests : ExporterTestBase
	{
		/// <summary>
		/// The sample PDF file name that's available for testing within the output directory.
		/// </summary>
		protected const string SampleDocPdfFileName = "EDRM-Sample1.pdf";

		[IdentifiedTest("5b20c6f1-1196-41ea-9326-0e875e2cabe9")]
		[Category(TestCategories.Export)]
		[Category(TestCategories.Integration)]
		public async Task ShouldExportAsync()
		{
			IReadOnlyList<string> sampleDocFileNames =
				new List<string> { ResourceFileHelper.GetDocsResourceFilePath(SampleDocPdfFileName) };

			this.GivenTheExportType(ExportFile.ExportType.ParentSearch);
			Relativity.CaseInfo caseInfo = await this.WhenGettingTheWorkspaceInfoAsync().ConfigureAwait(false);
			this.GivenTheFilesAreImported(sampleDocFileNames);
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