// -----------------------------------------------------------------------------------------------------
// <copyright file="TimekeeperTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="Timekeeper"/> tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Import.Export.NUnit
{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;

	using CsvHelper;

	using global::NUnit.Framework;

	using Moq;

	using Relativity.Import.Export;
	using Relativity.Import.Export.Io;
	using Relativity.Import.Export.TestFramework;

	/// <summary>
	/// Represents <see cref="Timekeeper"/> tests.
	/// </summary>
	[TestFixture]
	[System.Diagnostics.CodeAnalysis.SuppressMessage(
		"Microsoft.Design",
		"CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
		Justification = "The test class handles the disposal.")]
	public class TimekeeperTests
	{
		private TempDirectory tempDirectory;
		private IFileSystem fileSystem;
		private Mock<IAppSettings> mockAppSettings;
		private Timekeeper timekeeper;

		[SetUp]
		public void Setup()
		{
			this.fileSystem = new FileSystemWrap();
			this.tempDirectory = new TempDirectory();
			this.tempDirectory.Create();
			this.mockAppSettings = new Mock<IAppSettings>();
			this.timekeeper = new Timekeeper(this.fileSystem, this.mockAppSettings.Object);
		}

		[TearDown]
		public void Teardown()
		{
			this.tempDirectory.Dispose();
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		[Category(TestCategories.Framework)]
		public void ShouldOnlyTrackTimeWhenTheLogAllEventsSettingIsTrue(bool logAllEvents)
        {
	        this.mockAppSettings.SetupGet(x => x.LogAllEvents).Returns(logAllEvents);
			this.timekeeper.MarkStart("a");
			this.timekeeper.MarkEnd("a");
			Assert.That(this.timekeeper.Count, !logAllEvents ? Is.Zero : Is.EqualTo(1));
			Assert.That(this.timekeeper.GetEntry("a"), !logAllEvents ? Is.Null : Is.Not.Null);
			this.timekeeper.MarkStart("a");
			this.timekeeper.MarkEnd("a");
			Assert.That(this.timekeeper.Count, !logAllEvents ? Is.Zero : Is.EqualTo(1));
			Assert.That(this.timekeeper.GetEntry("a"), !logAllEvents ? Is.Null : Is.Not.Null);
			this.timekeeper.MarkStart("b", 1);
			this.timekeeper.MarkEnd("b", 1);
			Assert.That(this.timekeeper.Count, !logAllEvents ? Is.Zero : Is.EqualTo(2));
			Assert.That(this.timekeeper.GetEntry("b", 1), !logAllEvents ? Is.Null : Is.Not.Null);
			this.timekeeper.MarkStart("b", 1);
			this.timekeeper.MarkEnd("b", 1);
			Assert.That(this.timekeeper.Count, !logAllEvents ? Is.Zero : Is.EqualTo(2));
			Assert.That(this.timekeeper.GetEntry("b", 1), !logAllEvents ? Is.Null : Is.Not.Null);
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		[Category(TestCategories.Framework)]
		public void ShouldOnlyGenerateTheCsvReportWhenTheLogAllEventsSettingIsTrue(bool logAllEvents)
		{
			this.mockAppSettings.SetupGet(x => x.LogAllEvents).Returns(logAllEvents);
			this.timekeeper.MarkStart("a");
			this.timekeeper.MarkEnd("a");
			this.timekeeper.MarkStart("a", 1);
			this.timekeeper.MarkEnd("a", 1);
			this.timekeeper.GenerateCsvReportItemsAsRows("import-api", this.tempDirectory.Directory);
			List<string> files = System.IO.Directory.GetFiles(this.tempDirectory.Directory).ToList();
			if (logAllEvents)
			{
				Assert.That(files.Count, Is.EqualTo(1));
				string file = files[0];
				using (var reader = new StreamReader(file))
				{
					var csv = new CsvReader(reader);
					csv.Read();
					bool header = csv.ReadHeader();
					Assert.That(header, Is.True);
					csv.Read();
					string functionName = csv.GetField<string>(0);
					int numberOfCalls = csv.GetField<int>(1);
					long totalLength = csv.GetField<long>(2);
					Assert.That(functionName, Is.Not.Null.Or.Empty);
					Assert.That(numberOfCalls, Is.EqualTo(2));
					Assert.That(totalLength, Is.GreaterThan(0));
				}
			}
			else
			{
				Assert.That(files.Count, Is.Zero);
			}
		}
	}
}