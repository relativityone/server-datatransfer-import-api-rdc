// -----------------------------------------------------------------------------------------------------
// <copyright file="DownloadTapiBridgeTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="DownloadTapiBridge2"/> tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit.Integration
{
	using System;
	using System.Collections.Generic;
	using System.IO;

	using global::NUnit.Framework;

	using Relativity.DataExchange.Transfer;
	using Relativity.Testing.Identification;

	/// <summary>
	/// Represents <see cref="DownloadTapiBridge2"/> tests.
	/// </summary>
	[TestFixture]
	[Feature.DataTransfer.TransferApi]
	[TestType.MainFlow]
	[System.Diagnostics.CodeAnalysis.SuppressMessage(
		"Microsoft.Design",
		"CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
		Justification = "The test class handles the disposal.")]
	public class DownloadTapiBridgeTests : TapiBridgeTestBase
	{
		private DownloadTapiBridge2 tapiBridge;

		protected override TapiBridgeBase2 TapiBridge => this.tapiBridge;

		[IdentifiedTestCase("8c7a5c0c-77eb-40c5-beb7-0a3f009a2472", TapiClient.Aspera, true)]
		[IdentifiedTestCase("74777b4f-24dc-4772-9ae9-4ef4b442da89", TapiClient.Aspera, false)]
		[IdentifiedTestCase("1616a14a-f239-42d4-b90d-c7c25d67d149", TapiClient.Direct, true)]
		[IdentifiedTestCase("87d185f9-15c6-4146-b306-0549774a2e0d", TapiClient.Direct, false)]
		public void ShouldDownloadTheFiles(TapiClient client, bool preserveTimestamps)
		{
			// Note: TapiClient.Web is skipped for now because it requires a significant amount
			//       of setup and configuration.
			this.GivenTheTapiClientSetting(client);
			this.GivenThePreserveFileTimestampsSetting(preserveTimestamps);
			this.GivenTheMaxFilesPerFolder(10);
			this.GivenTheNumberOfFiles(100);
			this.GivenTheDataset(this.GetTestFiles());
			this.GivenTheTargetPath(Path.Combine(this.TempDirectory.Directory, "Actual"));
			this.GivenTheNativeFileTransfer();
			this.WhenExecutingTheJob();
			this.ThenTheTapiClientShouldEqualTheRequestedType();
			this.ThenTheFilesWereTransferred();
			this.ThenTheFileTimestampsArePreserved();
		}

		protected override void CreateTapiBridge()
		{
			this.tapiBridge = this.CreateDownloadTapiBridge(this.TargetPath);
		}

		protected override List<string> GetFilesToCheckTimestamps()
		{
			return this.TransferredFilesPaths;
		}

		private void WhenExecutingTheJob()
		{
			RunDownload(this.tapiBridge, this.SourcePaths);
		}

		private List<string> GetTestFiles()
		{
			List<string> filesToUpload = this.GenerateTestFilesInDirectory(Path.Combine(this.TempDirectory.Directory, "Expected"));
			List<string> uploadedFiles = new List<string>();

			string targetPath = Path.Combine(
				this.TestParameters.FileShareUncPath,
				$"{typeof(DownloadTapiBridgeTests).Name}-{Guid.NewGuid()}");

			using (var uploadTapiBridge = this.CreateUploadTapiBridge(targetPath))
			{
				uploadTapiBridge.TapiProgress += (sender, args) =>
					{
						if (args.Successful)
						{
							uploadedFiles.Add(args.TargetFile);
						}
					};

				RunUpload(uploadTapiBridge, filesToUpload);
			}

			return uploadedFiles;
		}
	}
}