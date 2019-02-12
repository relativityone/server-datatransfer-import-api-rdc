﻿// -----------------------------------------------------------------------------------------------------
// <copyright file="UploadTapiBridgeTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="UploadTapiBridge"/> tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Import.Client.NUnit.Integration
{
	using System.Net;
	using System.Threading;

	using global::NUnit.Framework;

	using kCura.WinEDDS.TApi;

	using Relativity.ImportExport.UnitTestFramework;

	/// <summary>
	/// Represents <see cref="UploadTapiBridge"/> tests.
	/// </summary>
	[TestFixture]
	[System.Diagnostics.CodeAnalysis.SuppressMessage(
		"Microsoft.Design",
		"CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
		Justification = "The test class handles the disposal.")]
	public class UploadTapiBridgeTests : TapiBridgeTestBase
	{
		private UploadTapiBridge tapiBridge;

		protected override TapiBridgeBase NativeFileTransfer => tapiBridge;

		/// <summary>
		/// Tests the upload bridge.
		/// </summary>
		[Test]
		public void ShouldUploadTheFiles()
		{
			this.GivenTheMaxFilesPerFolder(10);
			this.GivenTheNumberOfFiles(100);
			this.GivenTheAutoGeneratedDataset(this.TempDirectory.Directory);
			this.GivenTheTargetPath(TestSettings.FileShareUncPath + "/NativeFileUploadTest");
			GivenTheStandardConfigSettings();
			this.GivenTheNativeFileTransfer();
			this.WhenExecutingTheJob();
			this.ThenTheFilesWereTransferred();
		}

		protected override void CreateTapiBridge()
		{
			var parameters = new UploadTapiBridgeParameters
			{
				Credentials = new NetworkCredential(TestSettings.RelativityUserName, TestSettings.RelativityPassword),
				MaxFilesPerFolder = this.MaxFilesPerFolder,
				TargetPath = this.TargetPath,
				WebCookieContainer = this.CookieContainer,
				WebServiceUrl = TestSettings.RelativityWebApiUrl.ToString(),
				WorkspaceId = TestSettings.WorkspaceId
			};

			this.tapiBridge = new UploadTapiBridge(parameters, this.TransferLog?.Object, CancellationToken.None);
		}

		private void WhenExecutingTheJob()
		{
			var order = 0;
			foreach (var sourcePath in this.SourcePaths)
			{
				this.tapiBridge.AddPath(sourcePath, null, order++);
			}

			this.NativeFileTransfer.WaitForTransferJob();
		}
	}
}