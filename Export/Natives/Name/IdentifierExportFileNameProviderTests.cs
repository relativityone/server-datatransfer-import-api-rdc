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
		public void GetNameShouldCallNativeFileNameMethodWithAppendOriginalFileNameProperty(bool input)
		{
			string expected = input.ToString();
			_exportFileMock.AppendOriginalFileName = input;
			_objectExportInfoMock.Setup(x => x.NativeFileName(It.IsAny<bool>())).Returns((bool b) => b.ToString());

			string actual = _sut.GetName(_objectExportInfoMock.Object);

			Assert.AreEqual(expected, actual, "GetName should call NativeFileName method with AppendOriginalFileName value and return proper result.");
		}

	}

}
