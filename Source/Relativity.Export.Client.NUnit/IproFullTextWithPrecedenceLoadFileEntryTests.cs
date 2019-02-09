// -----------------------------------------------------------------------------------------------------
// <copyright file="IproFullTextWithPrecedenceLoadFileEntryTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Export.Client.NUnit
{
    using System.Text;

    using kCura.WinEDDS;
    using kCura.WinEDDS.Core.Export.VolumeManagerV2;
    using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images.Lines;
    using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text;
    using kCura.WinEDDS.Exporters;

    using global::NUnit.Framework;
    
    using Relativity.Logging;
    using RelativityConstants = Relativity.Export.Constants;

    [TestFixture]
	public class IproFullTextWithPrecedenceLoadFileEntryTests : IproFullTextLoadFileEntryTests
	{
		protected override IproFullTextLoadFileEntry CreateInstance(IFieldService fieldService, LongTextHelper longTextHelper, IFullTextLineWriter fullTextLineWriter)
		{
			return new IproFullTextWithPrecedenceLoadFileEntry(fieldService, longTextHelper, new NullLogger(), fullTextLineWriter);
		}

		protected override void PrepareDataSet(ObjectExportInfo artifact, string textToWrite)
		{
			FieldService.Setup(x => x.GetOrdinalIndex(RelativityConstants.TEXT_PRECEDENCE_AWARE_AVF_COLUMN_NAME)).Returns(0);
			artifact.Metadata = new object[] {textToWrite};
		}

		protected override void PrepareDataSetForTooLongText(ObjectExportInfo artifact, string textToWrite, string fileLocation)
		{
			const int fieldArtifactId = 998687;
			const int artifactId = 817225;

			FieldService.Setup(x => x.GetOrdinalIndex(RelativityConstants.TEXT_PRECEDENCE_AWARE_AVF_COLUMN_NAME)).Returns(0);

			FieldService.Setup(x => x.GetOrdinalIndex(RelativityConstants.TEXT_PRECEDENCE_AWARE_ORIGINALSOURCE_AVF_COLUMN_NAME)).Returns(1);

			artifact.ArtifactID = artifactId;
			artifact.Metadata = new object[] {textToWrite, fieldArtifactId};

			LongText longText = LongText.CreateFromExistingFile(artifactId, fieldArtifactId, fileLocation, Encoding.Default);
			LongTextRepository.Add(longText.InList());
		}
	}
}