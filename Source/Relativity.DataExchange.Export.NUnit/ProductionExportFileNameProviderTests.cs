// -----------------------------------------------------------------------------------------------------
// <copyright file="ProductionExportFileNameProviderTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System;

	using global::NUnit.Framework;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;

	[TestFixture]
	public class ProductionExportFileNameProviderTests
	{
		private const string _BEGIN_BATES = "beg001";
		private const string _CONTROL_NUMBER = "ABC0001";
		private const string _ORIGINAL_FILE_NAME = "originalFileName.pdf";
		private const int _ARTIFACT_ID = 10;
		private const string _DEFAULT_SEPARATOR = "_";
		private const string _NATIVE_EXTENSION = "htm";
		private const string _ORIGINAL_FILE_NAME_PART = _DEFAULT_SEPARATOR + _ORIGINAL_FILE_NAME;
		private ExportFile _exportFile;
		private ObjectExportInfo _exportedObjectInfo;
		private ProductionExportFileNameProvider _sut;

		[SetUp]
		public void SetUp()
		{
			this._exportFile = new ExportFile(_ARTIFACT_ID)
			{
				TypeOfExport = ExportFile.ExportType.Production
			};
			this._sut = new ProductionExportFileNameProvider(this._exportFile, false);
			this._exportedObjectInfo = new ObjectExportInfo
			{
				IdentifierValue = _CONTROL_NUMBER,
				ProductionBeginBates = _BEGIN_BATES,
				OriginalFileName = _ORIGINAL_FILE_NAME,
				NativeExtension = _NATIVE_EXTENSION
			};
		}

		[Test]
		public void GetNameShouldReturnNameWithBeginBates()
		{
			// arrange
			string expected = $"{_BEGIN_BATES}.{_NATIVE_EXTENSION}";
			this._exportFile.AppendOriginalFileName = false;

			// act
			string actual = this._sut.GetName(this._exportedObjectInfo);

			// assert
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void GetNameShouldReturnNameWithBeginBatesAndAppendOriginalFileNameWithoutNativeExtension()
		{
			// arrange
			string expected = $"{_BEGIN_BATES}{_ORIGINAL_FILE_NAME_PART}";
			this._exportFile.AppendOriginalFileName = true;

			// act
			string actual = this._sut.GetName(this._exportedObjectInfo);

			// assert
			Assert.AreEqual(expected, actual);
		}

		[TestCase(false, "")]
		[TestCase(true, _ORIGINAL_FILE_NAME_PART)]
		public void GetTextNameShouldReturnNameWithBatesForProductionExportWithProductionNaming(bool appendOriginalFileName, string originalFileNamePart)
		{
			// arrange
			string expected = $"{_BEGIN_BATES}";
			expected = AppendOriginalFileNameAndExtension(expected, "txt", originalFileNamePart);
			this._exportFile.AppendOriginalFileName = appendOriginalFileName;
			this._exportFile.ExportNativesToFileNamedFrom = ExportNativeWithFilenameFrom.Production;

			// act
			string actual = this._sut.GetTextName(this._exportedObjectInfo);

			// assert
			Assert.AreEqual(expected, actual);
		}

		[TestCase(false, "")]
		[TestCase(true, _ORIGINAL_FILE_NAME_PART)]
		public void GetTextNameShouldReturnNameWithIdentifierForProductionExportWithIdentifierNaming(bool appendOriginalFileName, string originalFileNamePart)
		{
			// arrange
			string expected = $"{_CONTROL_NUMBER}";
			expected = AppendOriginalFileNameAndExtension(expected, "txt", originalFileNamePart);
			this._exportFile.AppendOriginalFileName = appendOriginalFileName;
			this._exportFile.ExportNativesToFileNamedFrom = ExportNativeWithFilenameFrom.Identifier;

			// act
			string actual = this._sut.GetTextName(this._exportedObjectInfo);

			// assert
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void GetTextNameShouldThrowArgumentNullExceptionIfParameterPassedIsNull()
		{
			Assert.Throws<ArgumentNullException>(() => this._sut.GetTextName(null));
		}

		[Test]
		public void GetNameShouldThrowArgumentNullExceptionIfParameterPassedIsNull()
		{
			Assert.Throws<ArgumentNullException>(() => this._sut.GetName(null));
		}

		private static string AppendOriginalFileNameAndExtension(string filename, string extension, string originalFileNamePart = "")
		{
			return $"{filename}{originalFileNamePart}.{extension}";
		}
	}
}