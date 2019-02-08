using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text.Delimiter;
using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.LoadFileEntry;
using kCura.WinEDDS.NUnit.TestObjectFactories;
using Moq;
using NUnit.Framework;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Metadata.Text
{
	[TestFixture]
	public class LongTextHandlerTests
	{
		private LongTextHandler _instance;

		private Mock<ILongTextHandler> _textPrecedenceHandler;
		private Mock<ILongTextHandler> _textToLoadFile;

		private const string _DELIMITER_START = "delimiter_start";
		private const string _DELIMITER_END = "delimiter_end";

		[SetUp]
		public void SetUp()
		{
			_textPrecedenceHandler = new Mock<ILongTextHandler>();
			_textToLoadFile = new Mock<ILongTextHandler>();

			IDelimiter delimiter = new ConfigurableDelimiter(_DELIMITER_START, _DELIMITER_END);

			_instance = new LongTextHandler(_textPrecedenceHandler.Object, _textToLoadFile.Object, delimiter, new NullLogger());
		}

		[Test]
		public void ItShouldHandleLongTextField()
		{
			ViewFieldInfo field = new QueryFieldFactory().GetExtractedTextField();

			ObjectExportInfo artifact = new ObjectExportInfo();

			DeferredEntry lineEntry = new DeferredEntry();

			//ACT
			_instance.HandleLongText(artifact, field, lineEntry);

			//ASSERT
			_textToLoadFile.Verify(x => x.HandleLongText(artifact, field, lineEntry));
			Assert.That(lineEntry.GetTextFromEntry(), Is.EqualTo($"{_DELIMITER_START}{_DELIMITER_END}"));
		}

		[Test]
		public void ItShouldHandleTextPrecedenceField()
		{
			CoalescedTextViewField field = new CoalescedTextViewField(new QueryFieldFactory().GetExtractedTextField(), true);

			ObjectExportInfo artifact = new ObjectExportInfo();

			DeferredEntry lineEntry = new DeferredEntry();

			//ACT
			_instance.HandleLongText(artifact, field, lineEntry);

			//ASSERT
			_textPrecedenceHandler.Verify(x => x.HandleLongText(artifact, field, lineEntry));
			Assert.That(lineEntry.GetTextFromEntry(), Is.EqualTo($"{_DELIMITER_START}{_DELIMITER_END}"));
		}
	}
}