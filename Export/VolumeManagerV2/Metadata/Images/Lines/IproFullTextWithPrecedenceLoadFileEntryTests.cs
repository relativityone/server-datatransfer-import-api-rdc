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
	public class IproFullTextWithPrecedenceLoadFileEntryTests : IproFullTextLoadFileEntryTests
	{
		protected override IproFullTextLoadFileEntry CreateInstance(IFieldService fieldService, LongTextHelper longTextHelper, IFullTextLineWriter fullTextLineWriter)
		{
			return new IproFullTextWithPrecedenceLoadFileEntry(fieldService, longTextHelper, new NullLogger(), fullTextLineWriter);
		}

		protected override void PrepareDataSet(ObjectExportInfo artifact, string textToWrite)
		{
			FieldService.Setup(x => x.GetOrdinalIndex(Relativity.Export.Constants.TEXT_PRECEDENCE_AWARE_AVF_COLUMN_NAME)).Returns(0);
			artifact.Metadata = new object[] { textToWrite };
		}

		protected override void PrepareDataSetForTooLongText(ObjectExportInfo artifact, string textToWrite, string fileLocation)
		{
			const int fieldArtifactId = 998687;
			const int artifactId = 817225;

			FieldService.Setup(x => x.GetOrdinalIndex(Relativity.Export.Constants.TEXT_PRECEDENCE_AWARE_AVF_COLUMN_NAME)).Returns(0);

			FieldService.Setup(x => x.GetOrdinalIndex(Relativity.Export.Constants.TEXT_PRECEDENCE_AWARE_ORIGINALSOURCE_AVF_COLUMN_NAME)).Returns(1);

			artifact.ArtifactID = artifactId;
			artifact.Metadata = new object[] { textToWrite, fieldArtifactId };

			LongText longText = LongText.CreateFromExistingFile(artifactId, fieldArtifactId, fileLocation, Encoding.Default);
			LongTextRepository.Add(longText.InList());
		}
	}
}