// -----------------------------------------------------------------------------------------------------
// <copyright file="ExportFileFormatterTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System;
	using System.Linq;

	using global::NUnit.Framework;

	using Relativity.DataExchange.Export;

	public class ExportFileFormatterTests : ExportFileFormatterSetUp<ExportFileFormatter>
	{
		[Test]
		public void ItShouldReturnHeaderStringWithoutFilePathCol()
		{
			// Arrange
			this.ExpFile.ExportNative = false;
			this.ExpFile.ExportPdf = false;
			this.SubjectUnderTest = new ExportFileFormatter(this.ExpFile, this.FieldNameProviderMock.Object);

			string expectedHeader = $"{QuoteDelimiter}{FileName1}{QuoteDelimiter}{RecordDelimiter}{QuoteDelimiter}{FieldName2}{QuoteDelimiter}{Environment.NewLine}";

			// Act
			string header = this.SubjectUnderTest.GetHeader(this.Fields.ToList());

			// Assert
			Assert.That(header, Is.EqualTo(expectedHeader));
		}

		[Test]
		public void ItShouldReturnHeaderStringWithFilePathCol()
		{
			// Arrange
			this.ExpFile.ExportNative = true;
			this.ExpFile.ExportPdf = false;
			this.SubjectUnderTest = new ExportFileFormatter(this.ExpFile, this.FieldNameProviderMock.Object);

			string filePathCol = "FILE_PATH";
			string expectedHeader = $"{QuoteDelimiter}{FileName1}{QuoteDelimiter}{RecordDelimiter}{QuoteDelimiter}{FieldName2}{QuoteDelimiter}" +
			                        $"{RecordDelimiter}{QuoteDelimiter}{filePathCol}{QuoteDelimiter}{Environment.NewLine}";

			// Act
			string header = this.SubjectUnderTest.GetHeader(this.Fields.ToList());

			// Assert
			Assert.That(header, Is.EqualTo(expectedHeader));
		}

		[Test]
		public void ItShouldReturnHeaderStringWithPdfPathCol()
		{
			// Arrange
			const string PdfPathColumnName = "PDF_PATH";

			this.ExpFile.ExportNative = false;
			this.ExpFile.ExportPdf = true;
			this.SubjectUnderTest = new ExportFileFormatter(this.ExpFile, this.FieldNameProviderMock.Object);

			string expectedHeader = $"{QuoteDelimiter}{FileName1}{QuoteDelimiter}{RecordDelimiter}{QuoteDelimiter}{FieldName2}{QuoteDelimiter}" +
									$"{RecordDelimiter}{QuoteDelimiter}{PdfPathColumnName}{QuoteDelimiter}{Environment.NewLine}";

			// Act
			string header = this.SubjectUnderTest.GetHeader(this.Fields.ToList());

			// Assert
			Assert.That(header, Is.EqualTo(expectedHeader));
		}

		[Test]
		public void ItShouldReturnHeaderStringWithPdfAndFilePathCol()
		{
			// Arrange
			const string PdfPathColumnName = "PDF_PATH";
			const string FilePathColumnName = "FILE_PATH";

			this.ExpFile.ExportNative = true;
			this.ExpFile.ExportPdf = true;
			this.SubjectUnderTest = new ExportFileFormatter(this.ExpFile, this.FieldNameProviderMock.Object);

			string expectedHeader = $"{QuoteDelimiter}{FileName1}{QuoteDelimiter}{RecordDelimiter}{QuoteDelimiter}{FieldName2}{QuoteDelimiter}" +
									$"{RecordDelimiter}{QuoteDelimiter}{FilePathColumnName}{QuoteDelimiter}" +
									$"{RecordDelimiter}{QuoteDelimiter}{PdfPathColumnName}{QuoteDelimiter}{Environment.NewLine}";

			// Act
			string header = this.SubjectUnderTest.GetHeader(this.Fields.ToList());

			// Assert
			Assert.That(header, Is.EqualTo(expectedHeader));
		}

		[Test]
		public void ItShouldReturnEmptyHeader()
		{
			// Arrange
			this.ExpFile.ExportNative = true;
			this.ExpFile.ExportPdf = true;
			this.SubjectUnderTest = new ExportFileFormatter(this.ExpFile, this.FieldNameProviderMock.Object);

			// Act
			string header = this.SubjectUnderTest.GetHeader(null);

			// Assert
			Assert.That(header, Is.Empty);
		}
	}
}