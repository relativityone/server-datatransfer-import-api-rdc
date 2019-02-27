// -----------------------------------------------------------------------------------------------------
// <copyright file="TapiBridgeTestBase.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents the <see cref="TapiBridgeBase"/> base test class.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Import.Export.NUnit.Integration
{
	using System;
	using System.Net;

	using global::NUnit.Framework;

	using Relativity.Import.Export.TestFramework;
	using Relativity.Import.Export.Transfer;
	using Relativity.Transfer;

	/// <summary>
	/// Represents the <see cref="TapiBridgeBase"/> base test class.
	/// </summary>
	public abstract class TapiBridgeTestBase
	{
		/// <summary>
		/// The minimum test file length [1KB]
		/// </summary>
		private const int MinTestFileLength = 1024;

		/// <summary>
		/// The maximum test file length [10KB]
		/// </summary>
		private const int MaxTestFileLength = 10 * MinTestFileLength;

		/// <summary>
		/// The thread synchronization root backing.
		/// </summary>
		private static readonly object SyncRoot = new object();

		/// <summary>
		/// The file count.
		/// </summary>
		private int fileCount;

		/// <summary>
		/// The number of files transferred.
		/// </summary>
		private int filesTransferred;

		/// <summary>
		/// The total number of warnings.
		/// </summary>
		private int warnings;

		/// <summary>
		/// The total number of fatal errors
		/// </summary>
		private int fatalErrors;

		/// <summary>
		/// Gets the cookie container.
		/// </summary>
		/// <value>
		/// The <see cref="CookieContainer"/> instance.
		/// </value>
		protected CookieContainer CookieContainer
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the maximum number of files per folder.
		/// </summary>
		/// <value>
		/// The file count.
		/// </value>
		protected int MaxFilesPerFolder
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the native file transfer class.
		/// </summary>
		protected abstract TapiBridgeBase NativeFileTransfer
		{
			get;
		}

		/// <summary>
		/// Gets the target path.
		/// </summary>
		/// <value>
		/// The full path.
		/// </value>
		protected string TargetPath
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the temp directory.
		/// </summary>
		/// <value>
		/// The <see cref="TempDirectory"/> instance.
		/// </value>
		protected TempDirectory TempDirectory
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the mock transfer log.
		/// </summary>
		/// <value>
		/// The <see cref="ITransferLog"/> instance.
		/// </value>
		protected ITransferLog TransferLog
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the list of source paths.
		/// </summary>
		/// <value>
		/// The full paths.
		/// </value>
		protected System.Collections.Generic.IList<string> SourcePaths
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the integration test parameters.
		/// </summary>
		/// <value>
		/// The <see cref="IntegrationTestParameters"/> value.
		/// </value>
		protected IntegrationTestParameters TestParameters
		{
			get;
			private set;
		}

		/// <summary>
		/// The test setup.
		/// </summary>
		[SetUp]
		public void Setup()
		{
			ServicePointManager.SecurityProtocol =
				SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11
				| SecurityProtocolType.Tls12;
			this.AssignTestSettings();
			Assert.That(
				this.TestParameters.WorkspaceId,
				Is.Positive,
				() => "The test workspace must be created or specified in order to run this integration test.");
			this.TempDirectory = new TempDirectory();
			this.TempDirectory.Create();
			this.SourcePaths = new global::System.Collections.Generic.List<string>();
			this.MaxFilesPerFolder = 1000;
			this.TargetPath = null;
			this.CookieContainer = new CookieContainer();
			this.fileCount = 0;
			this.filesTransferred = 0;
			this.fatalErrors = 0;
			this.warnings = 0;
			this.TransferLog = new RelativityTransferLog(IntegrationTestHelper.Logger, false);
		}

		[TearDown]
		public void Teardown()
		{
			this.NativeFileTransfer?.Dispose();
		}

		/// <summary>
		/// Assign the test parameters. This should always be called from methods with <see cref="SetUpAttribute"/> or <see cref="OneTimeSetUpAttribute"/>.
		/// </summary>
		protected void AssignTestSettings()
		{
			if (this.TestParameters == null)
			{
				this.TestParameters = AssemblySetup.TestParameters.DeepCopy();
			}
		}

		/// <summary>
		/// Given the maximum number of files per folder.
		/// </summary>
		/// <param name="value">
		/// The max value.
		/// </param>
		protected void GivenTheMaxFilesPerFolder(int value)
		{
			this.MaxFilesPerFolder = value;
		}

		/// <summary>
		/// The given the number of files to generate.
		/// </summary>
		/// <param name="value">
		/// The number of files.
		/// </param>
		protected void GivenTheNumberOfFiles(int value)
		{
			this.fileCount = value;
		}

		/// <summary>
		/// Given the dataset is auto-generated by the number of specified files.
		/// </summary>
		/// <param name="directory">
		/// The directory.
		/// </param>
		protected void GivenTheAutoGeneratedDataset(string directory)
		{
			if (!System.IO.Directory.Exists(directory))
			{
				System.IO.Directory.CreateDirectory(directory);
			}

			for (var i = 0; i < this.fileCount; i++)
			{
				var file = RandomHelper.NextTextFile(
					MinTestFileLength,
					MaxTestFileLength,
					directory,
					false);
				this.SourcePaths.Add(file);
			}
		}

		/// <summary>
		/// Given the target path.
		/// </summary>
		/// <param name="value">
		/// The value.
		/// </param>
		protected void GivenTheTargetPath(string value)
		{
			this.TargetPath = value;
		}

		/// <summary>
		/// Given the <see cref="TapiBridgeBase"/> class.
		/// </summary>
		protected void GivenTheNativeFileTransfer()
		{
			this.CreateTapiBridge();
			this.NativeFileTransfer.TapiFatalError += (sender, args) =>
			{
				lock (SyncRoot)
				{
					this.fatalErrors++;
				}
			};

			this.NativeFileTransfer.TapiWarningMessage += (sender, args) =>
			{
				lock (SyncRoot)
				{
					this.warnings++;
				}
			};

			this.NativeFileTransfer.TapiProgress += (sender, args) =>
			{
				lock (SyncRoot)
				{
					this.filesTransferred++;
				}
			};
		}

		protected abstract void CreateTapiBridge();

		/// <summary>
		/// Then all the files were transferred.
		/// </summary>
		protected void ThenTheFilesWereTransferred()
		{
			Console.WriteLine($"Files Transferred: {this.filesTransferred}");
			Assert.That(this.fatalErrors, Is.EqualTo(0));
			Assert.That(this.warnings, Is.EqualTo(0));
			Assert.That(this.filesTransferred, Is.EqualTo(this.fileCount));
		}
	}
}