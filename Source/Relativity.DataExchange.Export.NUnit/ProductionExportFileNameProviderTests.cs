// -----------------------------------------------------------------------------------------------------
// <copyright file="ProductionExportFileNameProviderTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Export.NUnit
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
			_exportFile = new ExportFile(_ARTIFACT_ID)
			{
				TypeOfExport = ExportFile.ExportType.Production
			};
			_sut = new ProductionExportFileNameProvider(_exportFile, false);
			_exportedObjectInfo = new ObjectExportInfo
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
			_exportFile.AppendOriginalFileName = false;

			// act
			string actual = _sut.GetName(_exportedObjectInfo);

			// assert
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void GetNameShouldReturnNameWithBeginBatesAndAppendOriginalFileNameWithoutNativeExtension()
		{
			// arrange
			string expected = $"{_BEGIN_BATES}{_ORIGINAL_FILE_NAME_PART}";
			_exportFile.AppendOriginalFileName = true;

			// act
			string actual = _sut.GetName(_exportedObjectInfo);

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
			_exportFile.AppendOriginalFileName = appendOriginalFileName;
			_exportFile.ExportNativesToFileNamedFrom = ExportNativeWithFilenameFrom.Production;

			// act
			string actual = _sut.GetTextName(_exportedObjectInfo);

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
			_exportFile.AppendOriginalFileName = appendOriginalFileName;
			_exportFile.ExportNativesToFileNamedFrom = ExportNativeWithFilenameFrom.Identifier;

			// act
			string actual = _sut.GetTextName(_exportedObjectInfo);

			// assert
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void GetTextNameShouldThrowArgumentNullExceptionIfParameterPassedIsNull()
		{
			Assert.Throws<ArgumentNullException>(() => _sut.GetTextName(null));
		}

		[Test]
		public void GetNameShouldThrowArgumentNullExceptionIfParameterPassedIsNull()
		{
			Assert.Throws<ArgumentNullException>(() => _sut.GetName(null));
		}

		private static string AppendOriginalFileNameAndExtension(string filename, string extension, string originalFileNamePart = "")
		{
			return $"{filename}{originalFileNamePart}.{extension}";
		}
	}
}