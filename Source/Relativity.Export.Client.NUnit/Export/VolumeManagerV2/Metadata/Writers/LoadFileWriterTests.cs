using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Natives;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers;
using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.LoadFileEntry;
using NUnit.Framework;
using Polly;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Metadata.Writers
{
	[TestFixture]
	public class LoadFileWriterTests
	{
		private LoadFileWriter _instance;

		private MemoryStream _memoryStream;
		private StreamWriter _streamWriter;

		[SetUp]
		public void SetUp()
		{
			_memoryStream = new MemoryStream(1);
			_streamWriter = new StreamWriter(_memoryStream, Encoding.Default);

			_instance = new LoadFileWriter(new NullLogger());
		}

		[Test]
		public void ItShouldWriteHeader()
		{
			const string header = "header";
			IDictionary<int, ILoadFileEntry> linesToWrite = new Dictionary<int, ILoadFileEntry>();
			linesToWrite.Add(LoadFileHeader.HEADER_KEY, new CompletedLoadFileEntry(header));

			//ACT
			_instance.Write(_streamWriter, linesToWrite, new ArtifactEnumerator(new ObjectExportInfo[0], new Context("")), CancellationToken.None);

			//ASSERT
			string actualText = GetWrittenText();
			Assert.That(actualText, Is.EqualTo(header));
		}

		[Test]
		public void ItShouldSkipHeaderWhenDoesNotExists()
		{
			IDictionary<int, ILoadFileEntry> linesToWrite = new Dictionary<int, ILoadFileEntry>();

			//ACT
			_instance.Write(_streamWriter, linesToWrite, new ArtifactEnumerator(new ObjectExportInfo[0], new Context("")), CancellationToken.None);

			//ASSERT
			string actualText = GetWrittenText();
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

			IDictionary<int, ILoadFileEntry> linesToWrite = new Dictionary<int, ILoadFileEntry>();
			linesToWrite.Add(artifact1.ArtifactID, new CompletedLoadFileEntry(artifact1Entry));
			linesToWrite.Add(artifact2.ArtifactID, new CompletedLoadFileEntry(artifact2Entry));
			linesToWrite.Add(artifact3.ArtifactID, new CompletedLoadFileEntry(artifact3Entry));

			//ACT
			_instance.Write(_streamWriter, linesToWrite, new ArtifactEnumerator(new[] {artifact3, artifact1, artifact2}, new Context("")), CancellationToken.None);

			//ASSERT
			string expectedText = $"{artifact3Entry}{artifact1Entry}{artifact2Entry}";
			string actualText = GetWrittenText();
			Assert.That(actualText, Is.EqualTo(expectedText));
		}

		private string GetWrittenText()
		{
			_streamWriter.Flush();
			return Encoding.Default.GetString(_memoryStream.ToArray());
		}
	}
}