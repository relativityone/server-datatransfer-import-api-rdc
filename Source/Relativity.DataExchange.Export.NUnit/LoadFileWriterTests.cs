// -----------------------------------------------------------------------------------------------------
// <copyright file="LoadFileWriterTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System.Collections.Generic;
	using System.IO;
	using System.Text;
	using System.Threading;

	using global::NUnit.Framework;

	using kCura.WinEDDS.Exporters;
	using kCura.WinEDDS.LoadFileEntry;

	using Polly;

	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Natives;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Writers;
	using Relativity.DataExchange.TestFramework;

	[TestFixture]
	public class LoadFileWriterTests
	{
		private LoadFileWriter _instance;

		private MemoryStream _memoryStream;
		private StreamWriter _streamWriter;

		[SetUp]
		public void SetUp()
		{
			this._memoryStream = new MemoryStream(1);
			this._streamWriter = new StreamWriter(this._memoryStream, Encoding.Default);

			this._instance = new LoadFileWriter(new TestNullLogger());
		}

		[Test]
		public void ItShouldWriteHeader()
		{
			const string header = "header";
			IDictionary<int, ILoadFileEntry> linesToWrite = new Dictionary<int, ILoadFileEntry>
				                                                {
					                                                {
						                                                LoadFileHeader.HEADER_KEY,
						                                                new CompletedLoadFileEntry(header)
					                                                },
				                                                };

			// ACT
			this._instance.Write(this._streamWriter, linesToWrite, new ArtifactEnumerator(new ObjectExportInfo[0], new Context(string.Empty)), CancellationToken.None);

			// ASSERT
			string actualText = this.GetWrittenText();
			Assert.That(actualText, Is.EqualTo(header));
		}

		[Test]
		public void ItShouldSkipHeaderWhenDoesNotExists()
		{
			IDictionary<int, ILoadFileEntry> linesToWrite = new Dictionary<int, ILoadFileEntry>();

			// ACT
			this._instance.Write(this._streamWriter, linesToWrite, new ArtifactEnumerator(new ObjectExportInfo[0], new Context(string.Empty)), CancellationToken.None);

			// ASSERT
			string actualText = this.GetWrittenText();
			Assert.That(actualText, Is.Empty);
		}

		[Test]
		public void ItShouldWriteEntriesForAllArtifactsInOrder()
		{
			const string artifact1Entry = "artifact1entry";
			const string artifact2Entry = "artifact2entry";
			const string artifact3Entry = "artifact3entry";

			ObjectExportInfo artifact1 = new ObjectExportInfo
			{
				ArtifactID = 1
			};

			ObjectExportInfo artifact2 = new ObjectExportInfo
			{
				ArtifactID = 2
			};

			ObjectExportInfo artifact3 = new ObjectExportInfo
			{
				ArtifactID = 3
			};

			IDictionary<int, ILoadFileEntry> linesToWrite = new Dictionary<int, ILoadFileEntry>
				                                                {
					                                                {
						                                                artifact1.ArtifactID,
						                                                new CompletedLoadFileEntry(artifact1Entry)
					                                                },
					                                                {
						                                                artifact2.ArtifactID,
						                                                new CompletedLoadFileEntry(artifact2Entry)
					                                                },
					                                                {
						                                                artifact3.ArtifactID,
						                                                new CompletedLoadFileEntry(artifact3Entry)
					                                                }
				                                                };

			// ACT
			this._instance.Write(
				this._streamWriter,
				linesToWrite,
				new ArtifactEnumerator(new[] { artifact3, artifact1, artifact2 }, new Context(string.Empty)),
				CancellationToken.None);

			// ASSERT
			string expectedText = $"{artifact3Entry}{artifact1Entry}{artifact2Entry}";
			string actualText = this.GetWrittenText();
			Assert.That(actualText, Is.EqualTo(expectedText));
		}

		private string GetWrittenText()
		{
			this._streamWriter.Flush();
			return Encoding.Default.GetString(this._memoryStream.ToArray());
		}
	}
}