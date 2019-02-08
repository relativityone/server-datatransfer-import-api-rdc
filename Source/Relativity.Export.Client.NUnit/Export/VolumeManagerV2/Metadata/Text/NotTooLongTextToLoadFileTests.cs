// ----------------------------------------------------------------------------
// <copyright file="NotTooLongTextToLoadFileTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Export.Client.NUnit.Export.VolumeManagerV2.Metadata.Text
{
    using System.Text;

    using kCura.WinEDDS;
    using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text;
    using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers;
    using kCura.WinEDDS.Core.Export.VolumeManagerV2.Repository;
    using kCura.WinEDDS.Exporters;
    using kCura.WinEDDS.LoadFileEntry;

    using Moq;

    using global::NUnit.Framework;

    using Relativity.Import.Client.NUnit;
    using Relativity.Logging;

    [TestFixture]
	public class NotTooLongTextToLoadFileTests
	{
		private NotTooLongTextToLoadFile _instance;
		private Mock<IFieldService> _fieldService;

		[SetUp]
		public void SetUp()
		{
			char quoteDelimiter = 'Q';
			char newlineDelimiter = 'N';

			ExportFile exportSettings = new ExportFile(1)
			{
				QuoteDelimiter = quoteDelimiter,
				NewlineDelimiter = newlineDelimiter,
				LoadFileEncoding = Encoding.Default
			};

			_fieldService = new Mock<IFieldService>();
			LongTextRepository longTextRepository = new LongTextRepository(null, new NullLogger());
			ILongTextStreamFormatterFactory formatterFactory = new DelimitedFileLongTextStreamFormatterFactory(exportSettings);
			FromFieldToLoadFileWriter fileWriter = new FromFieldToLoadFileWriter(new NullLogger(), formatterFactory);

			_instance = new NotTooLongTextToLoadFile(new LongTextHelper(exportSettings, _fieldService.Object, longTextRepository), fileWriter, new NullLogger(), exportSettings);
		}

		[Test]
		public void ItShouldAddEntryWithLongText()
		{
			const string expectedText = "expected_text";

			ViewFieldInfo field = new QueryFieldFactory().GetArtifactIdField();

			ObjectExportInfo artifact = new ObjectExportInfo
			{
				Metadata = new object[] { expectedText }
			};

			_fieldService.Setup(x => x.GetOrdinalIndex(It.IsAny<string>())).Returns(0);

			DeferredEntry lineEntry = new DeferredEntry();

			//ACT
			_instance.HandleLongText(artifact, field, lineEntry);

			//ASSERT
			Assert.That(lineEntry.GetTextFromEntry(), Is.EqualTo(expectedText));
		}
	}
}