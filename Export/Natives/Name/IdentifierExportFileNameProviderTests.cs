using kCura.WinEDDS.Exporters;
using Moq;
using NUnit.Framework;

namespace kCura.WinEDDS.Core.NUnit.Export.Natives.Name
{
	public class IdentifierExportFileNameProviderTest
	{
		private const int _ARTIFACT_ID = 10;
		private ExportFile _exportFileMock;
		private Mock<ObjectExportInfo> _objectExportInfoMock;
		private IdentifierExportFileNameProvider _sut;

		[SetUp]
		public void SetUp()
		{
			_exportFileMock = new ExportFile(_ARTIFACT_ID);
			_sut = new IdentifierExportFileNameProvider(_exportFileMock);
			_objectExportInfoMock = new Mock<ObjectExportInfo>();
		}

		[TestCase(false)]
		[TestCase(true)]
		[Test]
		public void GetNameShouldCallNativeFileNameMethodWithAppendOriginalFileNameProperty(bool appendOriginalFileName)
		{
			string expected = appendOriginalFileName.ToString();
			_exportFileMock.AppendOriginalFileName = appendOriginalFileName;
			_objectExportInfoMock.Setup(x => x.NativeFileName(It.IsAny<bool>())).Returns((bool b) => b.ToString());

			string actual = _sut.GetName(_objectExportInfoMock.Object);

			Assert.AreEqual(expected, actual, "GetName should call NativeFileName method with AppendOriginalFileName value and return proper result.");
		}

		[TestCase(false)]
		[TestCase(true)]
		[Test]
		public void GetTextNameShouldCallFullTextFileNameMethodWithAppendOriginalFileNameProperty(bool appendOriginalFileName)
		{
			string expected = $"{true}{false}{appendOriginalFileName}";
			_exportFileMock.AppendOriginalFileName = appendOriginalFileName;
			_objectExportInfoMock.Setup(x => x.FullTextFileName(It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns((bool b1, bool b2, bool b3) => $"{b1}{b2}{b3}");

			string actual = _sut.GetTextName(_objectExportInfoMock.Object);

			Assert.AreEqual(expected, actual, "GetTextName should call FullTextFileName method with AppendOriginalFileName value and return proper result.");
		}


	}

}
