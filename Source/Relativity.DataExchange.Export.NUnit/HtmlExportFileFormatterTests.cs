// -----------------------------------------------------------------------------------------------------
// <copyright file="HtmlExportFileFormatterTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Export.NUnit
{
    using System;
    using System.Linq;
    using System.Text;

    using global::NUnit.Framework;

    using Relativity.DataExchange.Export;
    using Relativity.DataExchange.Service;

	public class HtmlExportFileFormatterTests : ExportFileFormatterSetUp<HtmlExportFileFormatter>
	{
		private const string _WKSP_NAME = "WkspName";
		private const string _HTML_TH = "<th>";
		private const string _HTML_TH_END = "</th>";
		private const string _HTML_TR_END = "</tr>";
		private const string _IMAGE_COL_NAME = "Image Files";
		private const string _NATIVE_COL_NAME = "Native Files";
		private readonly string _headerSuffix = $"{Environment.NewLine}{_HTML_TR_END}{Environment.NewLine}";
		private string _headerPrefix;

		protected override void InitTestCase()
		{
			base.InitTestCase();

			StringBuilder retString = new StringBuilder();

			retString.Append("<html><head><title>" + System.Web.HttpUtility.HtmlEncode(_WKSP_NAME) + "</title>");
			retString.Append("<style type='text/css'>" + Environment.NewLine);
			retString.Append("td {vertical-align: top;background-color:#EEEEEE;}" + Environment.NewLine);
			retString.Append("th {color:#DDDDDD;text-align:left;}" + Environment.NewLine);
			retString.Append("table {background-color:#000000;}" + Environment.NewLine);
			retString.Append("</style>" + Environment.NewLine);
			retString.Append("</head><body>" + Environment.NewLine);
			retString.Append("<table width='100%'><tr>" + Environment.NewLine);

			_headerPrefix = retString.ToString();

			ExpFile.CaseInfo = new CaseInfo
			{
				Name = _WKSP_NAME
			};
		}

		[Test]
		public void ItShouldReturnHeaderStringWithoutNativeAndWithoutImage()
		{
			// Arrange
			ExpFile.ExportNative = false;
			ExpFile.ExportImages = false;
			SubjectUnderTest = new HtmlExportFileFormatter(ExpFile, FieldNameProviderMock.Object);

			string expectedHeaderRow = $"{_HTML_TH}{FileName1}{_HTML_TH_END}{_HTML_TH}{FieldName2}{_HTML_TH_END}";
			string expectedHeader = GetExpectedHeaderText(expectedHeaderRow);

			// Act
			string header = SubjectUnderTest.GetHeader(Fields.ToList());

			// Assert
			Assert.That(header, Is.EqualTo(expectedHeader));
		}

		[Test]
		public void ItShouldReturnHeaderStringWithoutNative()
		{
			// Arrange
			ExpFile.ExportNative = false;
			ExpFile.ExportImages = true;
			SubjectUnderTest = new HtmlExportFileFormatter(ExpFile, FieldNameProviderMock.Object);

			string expectedHeaderRow = $"{_HTML_TH}{FileName1}{_HTML_TH_END}{_HTML_TH}{FieldName2}{_HTML_TH_END}" +
									   $"{_HTML_TH}{_IMAGE_COL_NAME}{_HTML_TH_END}";

			string expectedHeader = GetExpectedHeaderText(expectedHeaderRow);

			// Act
			string header = SubjectUnderTest.GetHeader(Fields.ToList());

			// Assert
			Assert.That(header, Is.EqualTo(expectedHeader));
		}

		[Test]
		public void ItShouldReturnHeaderStringWithoutImage()
		{
			// Arrange
			ExpFile.ExportNative = true;
			ExpFile.ExportImages = false;
			SubjectUnderTest = new HtmlExportFileFormatter(ExpFile, FieldNameProviderMock.Object);

			string expectedHeaderRow = $"{_HTML_TH}{FileName1}{_HTML_TH_END}{_HTML_TH}{FieldName2}{_HTML_TH_END}" +
									   $"{_HTML_TH}{_NATIVE_COL_NAME}{_HTML_TH_END}";

			string expectedHeader = GetExpectedHeaderText(expectedHeaderRow);

			// Act
			string header = SubjectUnderTest.GetHeader(Fields.ToList());

			// Assert
			Assert.That(header, Is.EqualTo(expectedHeader));
		}

		private string GetExpectedHeaderText(string expectedHeaderRow)
		{
			return $"{_headerPrefix}{expectedHeaderRow}{_headerSuffix}{Environment.NewLine}";
		}
	}
}