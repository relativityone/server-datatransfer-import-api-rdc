// -----------------------------------------------------------------------------------------------------
// <copyright file="TapiBridgeTestBase.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents the <see cref="TapiBridgeBase2"/> base test class.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit.Integration
{
	using System;
	using System.IO;
	using System.Net;

	using global::NUnit.Framework;

	using Relativity.DataExchange;
	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.Transfer;
	using Relativity.Transfer;

	/// <summary>
	/// Represents the <see cref="TapiBridgeBase2"/> base test class.
	/// </summary>
	public abstract class TapiBridgeTestBase
	{
		/// <summary>
		/// The minimum test file length [1KB].
		/// </summary>
		private const int MinTestFileLength = 1024;

		/// <summary>
		/// The maximum test file length [10KB].
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
		/// The total number of fatal errors.
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
		/// Gets the file creation time.
		/// </summary>
		/// <value>
		/// The <see cref="DateTime"/> value.
		/// </value>
		protected DateTime FileCreationTime
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the file last access time.
		/// </summary>
		/// <value>
		/// The <see cref="DateTime"/> value.
		/// </value>
		protected DateTime FileLastAccessTime
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the file last write time.
		/// </summary>
		/// <value>
		/// The <see cref="DateTime"/> value.
		/// </value>
		protected DateTime FileLastWriteTime
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
		/// Gets the Transfer API bridge object.
		/// </summary>
		/// <value>
		/// The <see cref="TapiBridgeBase2"/> instance.
		/// </value>
		protected abstract TapiBridgeBase2 TapiBridge
		{
			get;
		}

		/// <summary>
		/// Gets a value indicating whether to preserve file timestamps.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to preserve file timestamps; otherwise, <see langword="false" />.
		/// </value>
		protected bool PreserveFileTimestamps
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the TAPI client.
		/// </summary>
		/// <value>
		/// The <see cref="TapiClient"/> instance.
		/// </value>
		protected TapiClient TapiClient
		{
			get;
			private set;
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
		protected TempDirectory2 TempDirectory
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
			this.TempDirectory = new TempDirectory2();
			this.TempDirectory.Create();
			this.SourcePaths = new global::System.Collections.Generic.List<string>();
			this.MaxFilesPerFolder = 1000;
			this.PreserveFileTimestamps = false;
			this.TapiClient = TapiClient.None;
			this.TargetPath = null;
			this.CookieContainer = new CookieContainer();
			this.fileCount = 0;
			this.filesTransferred = 0;
			this.fatalErrors = 0;
			this.warnings = 0;
			this.TransferLog = new RelativityTransferLog(IntegrationTestHelper.Logger);
			this.FileCreationTime = new DateTime(2001, 6, 1, 18, 0, 0);
			this.FileLastAccessTime = new DateTime(2002, 6, 1, 18, 0, 0);
			this.FileLastWriteTime = new DateTime(2003, 6, 1, 18, 0, 0);
		}

		[TearDown]
		public void Teardown()
		{
			this.TapiBridge?.Dispose();
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
		/// Given the preserve file timestamps setting.
		/// </summary>
		/// <param name="value">
		/// The TAPI client.
		/// </param>
		protected void GivenThePreserveFileTimestampsSetting(bool value)
		{
			this.PreserveFileTimestamps = value;
		}

		/// <summary>
		/// Given the TAPI client setting.
		/// </summary>
		/// <param name="value">
		/// The TAPI client.
		/// </param>
		protected void GivenTheTapiClientSetting(TapiClient value)
		{
			this.TapiClient = value;
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
			Directory.CreateDirectory(directory);

			for (var i = 0; i < this.fileCount; i++)
			{
				var file = RandomHelper.NextTextFile(
					MinTestFileLength,
					MaxTestFileLength,
					directory,
					false);
				if (this.PreserveFileTimestamps)
				{
					System.IO.File.SetCreationTime(file, this.FileCreationTime);
					System.IO.File.SetLastAccessTime(file, this.FileLastAccessTime);
					System.IO.File.SetLastWriteTime(file, this.FileLastWriteTime);
				}

				this.SourcePaths.Add(file);
			}

			this.TransferLog.LogInformation("Autogenerated {FileCount} source files.", this.fileCount);
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
		/// Given the <see cref="TapiBridgeBase2"/> class.
		/// </summary>
		protected void GivenTheNativeFileTransfer()
		{
			this.CreateTapiBridge();
			this.TapiBridge.TapiFatalError += (sender, args) =>
			{
				lock (SyncRoot)
				{
					this.fatalErrors++;
					Console.WriteLine($"[TapiBridge Fatal Error]: {args.Message} ({args.LineNumber})");
				}
			};

			this.TapiBridge.TapiWarningMessage += (sender, args) =>
			{
				lock (SyncRoot)
				{
					this.warnings++;
					Console.WriteLine($"[TapiBridge Warning]: {args.Message} ({args.LineNumber})");
				}
			};

			this.TapiBridge.TapiProgress += (sender, args) =>
			{
				lock (SyncRoot)
				{
					if (args.Successful)
					{
						this.filesTransferred++;
					}

					Console.WriteLine($"[TapiBridge Progress]: {args.FileName} ({args.LineNumber})");
				}
			};
		}

		protected void SetupTapiBridgeParameters(TapiBridgeParameters2 parameters)
		{
			ITapiObjectService objectService = new TapiObjectService();
			objectService.SetTapiClient(parameters, this.TapiClient);
		}

		protected abstract void CreateTapiBridge();

		/// <summary>
		/// Then all the files were transferred.
		/// </summary>
		protected void ThenTheFilesWereTransferred()
		{
			Console.WriteLine($"Files Transferred: {this.filesTransferred}");
			Assert.That(this.fatalErrors, Is.EqualTo(0));
			Assert.That(this.warnings, Is.GreaterThanOrEqualTo(0));
			Assert.That(this.filesTransferred, Is.EqualTo(this.fileCount));
		}

		protected void ThenTheTapiClientShouldEqualTheRequestedType()
		{
			if (this.TapiClient == TapiClient.None)
			{
				return;
			}

			Assert.That(this.TapiBridge.Client, Is.EqualTo(this.TapiClient));
		}

		protected void ThenTheFileTimestampsArePreserved()
		{
			if (!this.PreserveFileTimestamps)
			{
				return;
			}

			if (this.TapiBridge.Client == TapiClient.Web)
			{
				Assert.Fail("A logic error has occurred because the web client doesn't preserve timestamps.");
			}

			string[] targetFiles = Directory.GetFiles(this.TargetPath, "*", SearchOption.AllDirectories);
			foreach (string targetFile in targetFiles)
			{
				DateTime creationTime = System.IO.File.GetCreationTime(targetFile);
				Assert.That(creationTime, Is.EqualTo(this.FileCreationTime));
				DateTime lastAccessTime = System.IO.File.GetLastAccessTime(targetFile);
				Assert.That(lastAccessTime, Is.EqualTo(this.FileLastAccessTime));
				DateTime lastWriteTime = System.IO.File.GetLastWriteTime(targetFile);
				Assert.That(lastWriteTime, Is.EqualTo(this.FileLastWriteTime));
			}
		}

		protected virtual void CheckSkipTest(TapiClient client)
		{
			if ((client == TapiClient.Aspera && this.TestParameters.SkipAsperaModeTests) ||
				(client == TapiClient.Direct && this.TestParameters.SkipDirectModeTests))
			{
				Assert.Ignore(TestStrings.SkipTestMessage, $"{client}");
			}
		}
	}
}