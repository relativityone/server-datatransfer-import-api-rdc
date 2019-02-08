using System.Collections.Generic;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Natives;
using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.LoadFileEntry;
using Moq;
using NUnit.Framework;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Metadata.Natives
{
	[TestFixture]
	public class LoadFileMetadataBuilderTests
	{
		private LoadFileMetadataBuilder _instance;

		private Mock<IFieldService> _fieldLookupService;
		private Mock<ILoadFileLine> _loadFileLine;

		[SetUp]
		public void SetUp()
		{
			_fieldLookupService = new Mock<IFieldService>();
			LoadFileHeader loadFileHeader = new LoadFileHeader(_fieldLookupService.Object, new NullLogger());

			_loadFileLine = new Mock<ILoadFileLine>();

			_instance = new LoadFileMetadataBuilder(loadFileHeader, _loadFileLine.Object, new NullLogger());
		}

		[Test]
		public void ItShouldAddHeaderOnlyOnce()
		{
			const string columnHeader = "column_header";
			_fieldLookupService.Setup(x => x.GetColumnHeader()).Returns(columnHeader);

			//ACT
			IDictionary<int, ILoadFileEntry> loadFileEntriesWithHeader = _instance.AddLines(new ObjectExportInfo[0], CancellationToken.None);
			IDictionary<int, ILoadFileEntry> loadFileEntriesWithoutHeader = _instance.AddLines(new ObjectExportInfo[0], CancellationToken.None);

			//ASSERT
			Assert.That(loadFileEntriesWithHeader.ContainsKey(-1), Is.True);
			Assert.That(loadFileEntriesWithoutHeader.ContainsKey(-1), Is.False);

			CompletedLoadFileEntry header = loadFileEntriesWithHeader[-1] as CompletedLoadFileEntry;
			Assert.That(header, Is.Not.Null);
			Assert.That(header.LineBuilder.ToString(), Is.EqualTo(columnHeader));
		}

		[Test]
		public void ItShouldAddLineForEachArtifact()
		{
			const int artifactId1 = 1;
			const int artifactId2 = 2;
			ObjectExportInfo artifact1 = new ObjectExportInfo
			{
				ArtifactID = artifactId1
			};
			ObjectExportInfo artifact2 = new ObjectExportInfo
			{
				ArtifactID = artifactId2
			};

			ILoadFileEntry line1 = new CompletedLoadFileEntry("value_1");
			_loadFileLine.Setup(x => x.CreateLine(artifact1)).Returns(line1);

			ILoadFileEntry line2 = new CompletedLoadFileEntry("value_2");
			_loadFileLine.Setup(x => x.CreateLine(artifact2)).Returns(line2);

			//ACT
			IDictionary<int, ILoadFileEntry> loadFileEntries = _instance.AddLines(new[] {artifact1, artifact2}, CancellationToken.None);

			//ASSERT
			Assert.That(loadFileEntries.ContainsKey(artifactId1), Is.True);
			Assert.That(loadFileEntries[artifactId1], Is.EqualTo(line1));

			Assert.That(loadFileEntries.ContainsKey(artifactId2), Is.True);
			Assert.That(loadFileEntries[artifactId2], Is.EqualTo(line2));
		}
	}
}