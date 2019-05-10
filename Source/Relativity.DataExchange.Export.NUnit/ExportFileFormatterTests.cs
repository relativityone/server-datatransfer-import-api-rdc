// -----------------------------------------------------------------------------------------------------
// <copyright file="ExportFileFormatterTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Export.NUnit
{
    using System;
    using System.Linq;

    using global::NUnit.Framework;

    public class ExportFileFormatterTests : ExportFileFormatterSetUp<ExportFileFormatter>
	{
		[Test]
		public void ItShouldReturnHeaderStringWithoutFilePathCol()
		{
			// Arrange
			ExpFile.ExportNative = false;
			SubjectUnderTest = new ExportFileFormatter(ExpFile, FieldNameProviderMock.Object);

			string expectedHeader = $"{QuoteDelimiter}{FileName1}{QuoteDelimiter}{RecordDelimiter}{QuoteDelimiter}{FieldName2}{QuoteDelimiter}{Environment.NewLine}";

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
			string expectedHeader = $"{QuoteDelimiter}{FileName1}{QuoteDelimiter}{RecordDelimiter}{QuoteDelimiter}{FieldName2}{QuoteDelimiter}" +
			                        $"{RecordDelimiter}{QuoteDelimiter}{filePathCol}{QuoteDelimiter}{Environment.NewLine}";

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