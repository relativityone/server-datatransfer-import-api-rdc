// <copyright file="InvalidFileNameExportTests.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.Export.NUnit.Integration
{
	using System.Linq;
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.RelativityHelpers;
	using Relativity.Testing.Identification;

	[TestFixture(true)]
	[TestFixture(false)]
	[TestType.Error]
	[Feature.DataTransfer.DocumentExportApi.Operations.ExportFolderAndSubfolders]
	public class InvalidFileNameExportTests : ExportTestBase
	{
		private SqlQueryHelper queryHelper;

		public InvalidFileNameExportTests(bool useKepler)
			: base(useKepler)
		{
		}

		protected override IntegrationTestParameters TestParameters => AssemblySetup.TestParameters;

		[OneTimeSetUp]
		public Task OneTimeSetUpAsync()
		{
			if (IntegrationTestHelper.IsRegressionEnvironment())
			{
				Assert.Ignore("This fixture requires access to SQL.");
			}

			this.queryHelper = new SqlQueryHelper(TestParameters);
			return this.AddTrailingWhitespacesToFileNames();
		}

		[OneTimeTearDown]
		public Task OneTimeTearDownAsync()
		{
			return this.RemoveTrailingWhitespacesFromFileNames();
		}

		[IdentifiedTest("fa7a5189-4eae-46fb-b4b8-473ba136badf")]
		public async Task ShouldExportFilesWithTrailingWhitespacesInFileNameColumn()
		{
			// ARRANGE
			ExtendedExportFileSetup.SetupDocumentExport(ExtendedExportFile);
			ExtendedExportFileSetup.SetupImageExport(ExtendedExportFile);
			ExtendedExportFileSetup.SetupPaddings(ExtendedExportFile);
			ExtendedExportFileSetup.SetupDelimiters(ExtendedExportFile);

			// ACT
			this.ExecuteFolderAndSubfoldersAndVerify();

			// ASSERT
			this.ThenTheExportJobIsSuccessful(TestData.SampleDocFiles.Count());

			ExportedFilesValidator.ValidateNativesCount(this.ExtendedExportFile, TestData.SampleDocFiles.Count());
			await ExportedFilesValidator.ValidateNativeFilesAsync(this.ExtendedExportFile).ConfigureAwait(false);

			ExportedFilesValidator.ValidateImagesCount(this.ExtendedExportFile, TestData.SampleImageFiles.Count());
			await ExportedFilesValidator.ValidateImageFilesAsync(this.ExtendedExportFile).ConfigureAwait(false);

			await this.ThenTheExportedDocumentLoadFileIsAsExpectedAsync().ConfigureAwait(false);
			await this.ThenTheExportedImageLoadFileIsAsExpectedAsync().ConfigureAwait(false);
		}

		private Task AddTrailingWhitespacesToFileNames()
		{
			string query = $@"UPDATE [EDDS{TestParameters.WorkspaceId}].[EDDSDBO].[File] SET Filename = Filename + '		  '";
			return this.queryHelper.ExecuteQueryAsync(query);
		}

		private Task RemoveTrailingWhitespacesFromFileNames()
		{
			string query = $@"UPDATE [EDDS{TestParameters.WorkspaceId}].[EDDSDBO].[File] SET Filename = TRIM('	 ' FROM Filename)";
			return this.queryHelper.ExecuteQueryAsync(query);
		}
	}
}