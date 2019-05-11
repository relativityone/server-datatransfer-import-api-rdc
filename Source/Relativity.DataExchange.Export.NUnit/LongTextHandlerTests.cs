// -----------------------------------------------------------------------------------------------------
// <copyright file="LongTextHandlerTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using global::NUnit.Framework;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;
	using kCura.WinEDDS.LoadFileEntry;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Text;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Text.Delimiter;
	using Relativity.DataExchange.TestFramework;
	using Relativity.Logging;

	[TestFixture]
	public class LongTextHandlerTests
	{
		private const string _DELIMITER_START = "delimiter_start";
		private const string _DELIMITER_END = "delimiter_end";
		private LongTextHandler _instance;
		private Mock<ILongTextHandler> _textPrecedenceHandler;
		private Mock<ILongTextHandler> _textToLoadFile;

		[SetUp]
		public void SetUp()
		{
			this._textPrecedenceHandler = new Mock<ILongTextHandler>();
			this._textToLoadFile = new Mock<ILongTextHandler>();

			IDelimiter delimiter = new ConfigurableDelimiter(_DELIMITER_START, _DELIMITER_END);

			this._instance = new LongTextHandler(this._textPrecedenceHandler.Object, this._textToLoadFile.Object, delimiter, new NullLogger());
		}

		[Test]
		public void ItShouldHandleLongTextField()
		{
			kCura.WinEDDS.ViewFieldInfo field = new QueryFieldFactory().GetExtractedTextField();

			ObjectExportInfo artifact = new ObjectExportInfo();

			DeferredEntry lineEntry = new DeferredEntry();

			// ACT
			this._instance.HandleLongText(artifact, field, lineEntry);

			// ASSERT
			this._textToLoadFile.Verify(x => x.HandleLongText(artifact, field, lineEntry));
			Assert.That(lineEntry.GetTextFromEntry(), Is.EqualTo($"{_DELIMITER_START}{_DELIMITER_END}"));
		}

		[Test]
		public void ItShouldHandleTextPrecedenceField()
		{
			CoalescedTextViewField field = new CoalescedTextViewField(new QueryFieldFactory().GetExtractedTextField(), true);

			ObjectExportInfo artifact = new ObjectExportInfo();

			DeferredEntry lineEntry = new DeferredEntry();

			// ACT
			this._instance.HandleLongText(artifact, field, lineEntry);

			// ASSERT
			this._textPrecedenceHandler.Verify(x => x.HandleLongText(artifact, field, lineEntry));
			Assert.That(lineEntry.GetTextFromEntry(), Is.EqualTo($"{_DELIMITER_START}{_DELIMITER_END}"));
		}
	}
}