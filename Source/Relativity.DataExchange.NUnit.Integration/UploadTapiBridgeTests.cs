// -----------------------------------------------------------------------------------------------------
// <copyright file="UploadTapiBridgeTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="UploadTapiBridge2"/> tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit.Integration
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;

	using global::NUnit.Framework;

	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.Transfer;
	using Relativity.Testing.Identification;

	/// <summary>
	/// Represents <see cref="UploadTapiBridge2"/> tests.
	/// </summary>
	[TestFixture]
	[Feature.DataTransfer.ImportApi.Operations.ImportDocuments]
	[System.Diagnostics.CodeAnalysis.SuppressMessage(
		"Microsoft.Design",
		"CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
		Justification = "The test class handles the disposal.")]
	public class UploadTapiBridgeTests : TapiBridgeTestBase
	{
		private UploadTapiBridge2 tapiBridge;

		protected override TapiBridgeBase2 TapiBridge => this.tapiBridge;

		[IdentifiedTestCase("3b1d585f-a2ab-4a30-84ce-d0816e088333", TapiClient.None, false)]
		[IdentifiedTestCase("49c40e14-0fe6-429b-9690-63438d1c6a2e", TapiClient.Aspera, true)]
		[IdentifiedTestCase("f8cb64d1-4fe8-425c-8922-83a1646a5c9d", TapiClient.Aspera, false)]
		[IdentifiedTestCase("f4e8bc35-bf4c-47d2-9166-0603f4f34008", TapiClient.Direct, true)]
		[IdentifiedTestCase("c344b1c8-3c39-4aa1-97a1-6a32f1f1e18c", TapiClient.Direct, false)]
		[IdentifiedTestCase("50f47046-f2d0-4ffb-ba29-3b986233330b", TapiClient.Web, false)]
		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.Integration)]
		[Category(TestCategories.TransferApi)]
		public void ShouldUploadTheFiles(TapiClient client, bool preserveTimestamps)
		{
			TapiClientModeAvailabilityChecker.SkipTestIfModeNotAvailable(this.TestParameters, client);

			this.GivenTheTapiClientSetting(client);
			this.GivenThePreserveFileTimestampsSetting(preserveTimestamps);
			this.GivenTheMaxFilesPerFolder(10);
			this.GivenTheNumberOfFiles(100);
			this.GivenTheDataset(this.GenerateTestFilesInDirectory(Path.Combine(this.TempDirectory.Directory, "Expected")));
			this.GivenTheTargetPath(
				System.IO.Path.Combine(
					System.IO.Path.Combine(this.TestParameters.FileShareUncPath, "NativeFileUploadTest"),
					$"{typeof(UploadTapiBridgeTests).Name}-{Guid.NewGuid()}"));
			this.GivenTheNativeFileTransfer();
			this.WhenExecutingTheJob();
			this.ThenTheTapiClientShouldEqualTheRequestedType();
			this.ThenTheFilesWereTransferred();
			this.ThenTheFileTimestampsArePreserved();
		}

		protected override void CreateTapiBridge()
		{
			this.tapiBridge = this.CreateUploadTapiBridge(this.TargetPath);
		}

		protected override List<string> GetFilesToCheckTimestamps()
		{
			string targetPath = Path.Combine(this.TempDirectory.Directory, "Actual");
			Directory.CreateDirectory(targetPath);

			using (var downloadTapiBridge = this.CreateDownloadTapiBridge(targetPath))
			{
				RunDownload(downloadTapiBridge, this.TransferredFilesPaths);
			}

			return Directory.GetFiles(targetPath, "*", SearchOption.AllDirectories).ToList();
		}

		private void WhenExecutingTheJob()
		{
			RunUpload(this.tapiBridge, this.SourcePaths);
		}
	}
}