using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text;
using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.LoadFileEntry;
using kCura.WinEDDS.NUnit.TestObjectFactories;
using Moq;
using NUnit.Framework;
using Relativity.Logging;
using Constants = Relativity.Constants;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Metadata.Text
{
	[TestFixture]
	public class LongTextToLoadFileTests
	{
		private LongTextToLoadFile _instance;

		private Mock<IFieldService> _fieldService;
		private Mock<ILongTextHandler> _tooLongTextToLoadFile;
		private Mock<ILongTextHandler> _notTooLongTextToLoadFile;

		[SetUp]
		public void SetUp()
		{
			ExportFile exportSettings = new ExportFile(1);

			_fieldService = new Mock<IFieldService>();

			LongTextHelper longTextHelper = new LongTextHelper(exportSettings, _fieldService.Object, null);

			_tooLongTextToLoadFile = new Mock<ILongTextHandler>();
			_notTooLongTextToLoadFile = new Mock<ILongTextHandler>();

			_instance = new LongTextToLoadFile(longTextHelper, _tooLongTextToLoadFile.Object, _notTooLongTextToLoadFile.Object, new NullLogger());
		}

		[Test]
		public void ItShouldHandleLongText()
		{
			ObjectExportInfo artifact = new ObjectExportInfo
			{
				Metadata = new object[] {"not too long text"}
			};

			ViewFieldInfo field = new QueryFieldFactory().GetArtifactIdField();

			DeferredEntry lineEntry = new DeferredEntry();

			_fieldService.Setup(x => x.GetOrdinalIndex(It.IsAny<string>())).Returns(0);

			//ACT
			_instance.HandleLongText(artifact, field, lineEntry);

			//ASSERT
			_notTooLongTextToLoadFile.Verify(x => x.HandleLongText(artifact, field, lineEntry));
		}

		[Test]
		public void ItShouldHandleTooLongText()
		{
			ObjectExportInfo artifact = new ObjectExportInfo
			{
				Metadata = new object[] {Constants.LONG_TEXT_EXCEEDS_MAX_LENGTH_FOR_LIST_TOKEN}
			};

			ViewFieldInfo field = new QueryFieldFactory().GetArtifactIdField();

			DeferredEntry lineEntry = new DeferredEntry();

			_fieldService.Setup(x => x.GetOrdinalIndex(It.IsAny<string>())).Returns(0);

			//ACT
			_instance.HandleLongText(artifact, field, lineEntry);

			//ASSERT
			_tooLongTextToLoadFile.Verify(x => x.HandleLongText(artifact, field, lineEntry));
		}
	}
}