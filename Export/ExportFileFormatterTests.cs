using System;
using System.Linq;
using kCura.WinEDDS.Core.Export;
using NUnit.Framework;

namespace kCura.WinEDDS.Core.NUnit.Export
{
	public class ExportFileFormatterTests : ExportFileFormatterSetUp<ExportFileFormatter>
	{
		
		[Test]
		public void ItShouldReturnHeaderStringWithoutFilePathCol()
		{
			// Arrange
			ExpFile.ExportNative = false;
			SubjectUnderTest = new ExportFileFormatter(ExpFile, FieldNameProviderMock.Object);

			string expectedHeader = $"{QUOTE_DELIMITER}{FIELD_NAME_1}{QUOTE_DELIMITER}{RECORD_DELIMITER}{QUOTE_DELIMITER}{FIELD_NAME_2}{QUOTE_DELIMITER}{Environment.NewLine}";
			// Act
			string header = SubjectUnderTest.GetHeader(Fields.ToList());

			// Assert
			Assert.That(header, Is.EqualTo(expectedHeader));
		}

		[Test]
		public void ItShouldReturnHeaderStringWithFilePathCol()
		{
			// Arrange
			ExpFile.ExportNative = true;
			SubjectUnderTest = new ExportFileFormatter(ExpFile, FieldNameProviderMock.Object);

			string filePathCol = "FILE_PATH";
			string expectedHeader = $"{QUOTE_DELIMITER}{FIELD_NAME_1}{QUOTE_DELIMITER}{RECORD_DELIMITER}{QUOTE_DELIMITER}{FIELD_NAME_2}{QUOTE_DELIMITER}" +
			                        $"{RECORD_DELIMITER}{QUOTE_DELIMITER}{filePathCol}{QUOTE_DELIMITER}{Environment.NewLine}";
			// Act
			string header = SubjectUnderTest.GetHeader(Fields.ToList());
			// Assert
			Assert.That(header, Is.EqualTo(expectedHeader));
		}

		[Test]
		public void ItShouldReturnEmptyHeader()
		{
			// Arrange
			ExpFile.ExportNative = true;
			SubjectUnderTest = new ExportFileFormatter(ExpFile, FieldNameProviderMock.Object);

			// Act
			string header = SubjectUnderTest.GetHeader(null);

			// Assert
			Assert.That(header, Is.Empty);
		}
	}
}
