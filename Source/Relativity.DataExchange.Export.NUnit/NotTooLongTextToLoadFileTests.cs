// -----------------------------------------------------------------------------------------------------
// <copyright file="NotTooLongTextToLoadFileTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System.Text;

	using global::NUnit.Framework;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;
	using kCura.WinEDDS.LoadFileEntry;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Text;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Writers;
	using Relativity.DataExchange.Export.VolumeManagerV2.Repository;
	using Relativity.DataExchange.TestFramework;

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

			this._fieldService = new Mock<IFieldService>();
			LongTextRepository longTextRepository = new LongTextRepository(null, new TestNullLogger());
			ILongTextStreamFormatterFactory formatterFactory = new DelimitedFileLongTextStreamFormatterFactory(exportSettings);
			FromFieldToLoadFileWriter fileWriter = new FromFieldToLoadFileWriter(new TestNullLogger(), formatterFactory);

			this._instance = new NotTooLongTextToLoadFile(new LongTextHelper(exportSettings, this._fieldService.Object, longTextRepository), fileWriter, new TestNullLogger(), exportSettings);
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

			this._fieldService.Setup(x => x.GetOrdinalIndex(It.IsAny<string>())).Returns(0);

			DeferredEntry lineEntry = new DeferredEntry();

			// ACT
			this._instance.HandleLongText(artifact, field, lineEntry);

			// ASSERT
			Assert.That(lineEntry.GetTextFromEntry(), Is.EqualTo(expectedText));
		}
	}
}