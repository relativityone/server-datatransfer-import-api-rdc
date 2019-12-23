// -----------------------------------------------------------------------------------------------------
// <copyright file="IproFullTextWithoutPrecedenceLoadFileEntryTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System.Text;

	using global::NUnit.Framework;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;

	using Relativity.DataExchange.Export.VolumeManagerV2;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Images.Lines;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Text;
	using Relativity.Logging;

	[TestFixture]
	public class IproFullTextWithoutPrecedenceLoadFileEntryTests : IproFullTextLoadFileEntryTests
	{
		protected override IproFullTextLoadFileEntry CreateInstance(IFieldService fieldService, LongTextHelper longTextHelper, IFullTextLineWriter fullTextLineWriter)
		{
			return new IproFullTextWithoutPrecedenceLoadFileEntry(fieldService, longTextHelper, new NullLogger(), fullTextLineWriter);
		}

		protected override void PrepareDataSet(ObjectExportInfo artifact, string textToWrite)
		{
			this.FieldService.Setup(x => x.GetOrdinalIndex(LongTextHelper.EXTRACTED_TEXT_COLUMN_NAME)).Returns(0);
			artifact.Metadata = new object[] { textToWrite };
		}

		protected override void PrepareDataSetForTooLongText(ObjectExportInfo artifact, string textToWrite, string fileLocation)
		{
			const int artifactId = 817225;

			this.FieldService.Setup(x => x.GetOrdinalIndex(LongTextHelper.EXTRACTED_TEXT_COLUMN_NAME)).Returns(1);
			this.FieldService.Setup(x => x.GetColumns()).Returns(new ViewFieldInfo[0]);

			artifact.ArtifactID = artifactId;
			artifact.Metadata = new object[] { string.Empty, textToWrite, fileLocation };

			LongText longText = LongText.CreateFromExistingFile(artifactId, -1, fileLocation, Encoding.Default, artifact.LongTextLength);
			this.LongTextRepository.Add(longText.InList());
		}
	}
}