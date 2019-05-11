// -----------------------------------------------------------------------------------------------------
// <copyright file="IdentifierExportFileNameProviderTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using global::NUnit.Framework;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;

	using Moq;

	public class IdentifierExportFileNameProviderTests
	{
		private const int _ARTIFACT_ID = 10;
		private ExportFile _exportFileMock;
		private Mock<ObjectExportInfo> _objectExportInfoMock;
		private IdentifierExportFileNameProvider _sut;

		[SetUp]
		public void SetUp()
		{
			this._exportFileMock = new ExportFile(_ARTIFACT_ID);
			this._sut = new IdentifierExportFileNameProvider(this._exportFileMock);
			this._objectExportInfoMock = new Mock<ObjectExportInfo>();
		}

		[TestCase(false)]
		[TestCase(true)]
		[Test]
		public void GetNameShouldCallNativeFileNameMethodWithAppendOriginalFileNameProperty(bool appendOriginalFileName)
		{
			string expected = appendOriginalFileName.ToString();
			this._exportFileMock.AppendOriginalFileName = appendOriginalFileName;
			this._objectExportInfoMock.Setup(x => x.NativeFileName(It.IsAny<bool>())).Returns((bool b) => b.ToString());

			string actual = this._sut.GetName(this._objectExportInfoMock.Object);

			Assert.AreEqual(expected, actual, "GetName should call NativeFileName method with AppendOriginalFileName value and return proper result.");
		}

		[TestCase(false)]
		[TestCase(true)]
		[Test]
		public void GetTextNameShouldCallFullTextFileNameMethodWithAppendOriginalFileNameProperty(bool appendOriginalFileName)
		{
			string expected = $"{true}{false}{appendOriginalFileName}";
			this._exportFileMock.AppendOriginalFileName = appendOriginalFileName;
			this._objectExportInfoMock.Setup(x => x.FullTextFileName(It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns((bool b1, bool b2, bool b3) => $"{b1}{b2}{b3}");

			string actual = this._sut.GetTextName(this._objectExportInfoMock.Object);

			Assert.AreEqual(expected, actual, "GetTextName should call FullTextFileName method with AppendOriginalFileName value and return proper result.");
		}
	}
}