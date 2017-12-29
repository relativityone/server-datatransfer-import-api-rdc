using System.Text;
using kCura.WinEDDS.Core.Export.VolumeManagerV2;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images.Lines;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text;
using kCura.WinEDDS.Exporters;
using NUnit.Framework;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Metadata.Images.Lines
{
	[TestFixture]
	public class IproFullTextWithoutPrecedenceLoadFileEntryTests : IproFullTextLoadFileEntryTests
	{
		protected override IproFullTextLoadFileEntry CreateInstance(IFieldService fieldService, LongTextHelper longTextHelper, IFullTextLineWriter fullTextLineWriter)
		{
			return new IproFullTextWithoutPrecedenceLoadFileEntry(fieldService, longTextHelper, new NullLogger(), fullTextLineWriter);
		}

		protected override void PrepareDataSet(ObjectExportInfo artifact, string textToWrite)
		{
			FieldService.Setup(x => x.GetOrdinalIndex(LongTextHelper.EXTRACTED_TEXT_COLUMN_NAME)).Returns(0);
			artifact.Metadata = new object[] {textToWrite};
		}

		protected override void PrepareDataSetForTooLongText(ObjectExportInfo artifact, string textToWrite, string fileLocation)
		{
			const int artifactId = 817225;

			FieldService.Setup(x => x.GetOrdinalIndex(LongTextHelper.EXTRACTED_TEXT_COLUMN_NAME)).Returns(1);
			FieldService.Setup(x => x.GetColumns()).Returns(new ViewFieldInfo[0]);

			artifact.ArtifactID = artifactId;
			artifact.Metadata = new object[] {"", textToWrite, fileLocation};

			LongText longText = LongText.CreateFromExistingFile(artifactId, -1, fileLocation, Encoding.Default);
			LongTextRepository.Add(longText.InList());
		}
	}
}