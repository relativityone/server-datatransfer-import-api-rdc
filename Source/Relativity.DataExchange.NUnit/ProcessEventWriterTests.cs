// -----------------------------------------------------------------------------------------------------
// <copyright file="ProcessEventWriterTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="ProcessEventWriter"/> tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Import.Export.NUnit
{
	using System;

	using global::NUnit.Framework;

	using Relativity.Import.Export.Io;
	using Relativity.Import.Export.Process;
	using Relativity.Import.Export.TestFramework;

	/// <summary>
	/// Represents <see cref="ProcessEventWriter"/> tests.
	/// </summary>
	[TestFixture]
	[System.Diagnostics.CodeAnalysis.SuppressMessage(
		"Microsoft.Design",
		"CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
		Justification = "The test class handles the disposal.")]
	public class ProcessEventWriterTests
	{
		private IFileSystem fileSystem;
		private ProcessEventWriter writer;

		[SetUp]
		public void Setup()
		{
			this.fileSystem = new FileSystemWrap();
			this.writer = new ProcessEventWriter(this.fileSystem);
		}

		[TearDown]
		public void Teardown()
		{
			if (this.writer != null)
			{
				this.writer.Close();
				this.writer.Dispose();
			}
		}

		[Test]
		[Category(TestCategories.Framework)]
		public void ShouldThrowWhenTheConstructorArgsAreInvalid()
		{
			Assert.Throws<ArgumentNullException>(
				() =>
					{
						IFileSystem fileSystemCopy = this.fileSystem;
						fileSystemCopy = null;
						using (new ProcessEventWriter(fileSystemCopy))
						{
						}
					});
		}

		[Test]
		[Category(TestCategories.Framework)]
		public void ShouldThrowWhenTheProcessEventArgsIsNull()
		{
			Assert.Throws<ArgumentNullException>(() => { this.writer.Write(null); });
		}

		[Test]
		[Category(TestCategories.Framework)]
		public void ShouldNotCreateTheFileWhenNoEventsWereWritten()
		{
			Assert.That(this.writer.File, Is.Null.Or.Empty);
		}

		[Test]
		[Category(TestCategories.Framework)]
		public void ShouldCloseTheStream()
		{
			this.writer.Write(
				new ProcessEventDto(ProcessEventType.Error, "error-info-1", "Error message.", DateTime.Now));
			this.writer.Close();
		}

		[Test]
		[Category(TestCategories.Framework)]
		public void ShouldWriteTheEvents()
		{
			Assert.That(this.writer.File, Is.Null.Or.Empty);
			Assert.That(this.writer.HasEvents, Is.False);
			this.writer.Write(
				new ProcessEventDto(ProcessEventType.Error, "error-1", "Error message.", DateTime.Now.Subtract(TimeSpan.FromMinutes(5))));
			Assert.That(this.writer.File, Does.Exist);
			Assert.That(this.writer.HasEvents, Is.True);
			this.writer.Write(
				new ProcessEventDto(ProcessEventType.Status, "status-1", "Status message.", DateTime.Now.Subtract(TimeSpan.FromMinutes(4))));
			this.writer.Write(
				new ProcessEventDto(ProcessEventType.Warning, "warning-1", "Warning message.", DateTime.Now.Subtract(TimeSpan.FromMinutes(3))));
		}
	}
}