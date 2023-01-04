// -----------------------------------------------------------------------------------------------------
// <copyright file="HtmlExportFileFormatterTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System;
	using System.Linq;
	using System.Text;

	using global::NUnit.Framework;

	using kCura.WinEDDS;

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
		private const string _PDF_COL_NAME = "PDF Files";
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

			this._headerPrefix = retString.ToString();

			this.ExpFile.CaseInfo = new CaseInfo
			{
				Name = _WKSP_NAME
			};
		}

		[TestCase(false, false, false)]
		[TestCase(true, true, true)]
		[TestCase(false, true, true)]
		[TestCase(true, false, true)]
		[TestCase(true, true, false)]
		[TestCase(true, false, false)]
		[TestCase(false, true, false)]
		[TestCase(false, false, true)]
		public void ItShouldReturnCorrectHeaderStringForDocument(bool exportImages, bool exportNatives, bool exportPdfs)
		{
			// Arrange
			this.ExpFile.ExportImages = exportImages;
			this.ExpFile.ExportNative = exportNatives;
			this.ExpFile.ExportPdf = exportPdfs;
			this.SubjectUnderTest = new HtmlExportFileFormatter(this.ExpFile, this.FieldNameProviderMock.Object);

			string expectedHeader = this.GetExpectedHeader(exportImages, exportNatives, exportPdfs);

			// Act
			string header = this.SubjectUnderTest.GetHeader(this.Fields.ToList());

			// Assert
			Assert.That(header, Is.EqualTo(expectedHeader));
		}

		[Test]
		public void ItShouldNotIncludeImagesWhenArtifactIsNotDocument()
		{
			// Arrange
			this.ExpFile = new ExportFile((int)ArtifactType.Field)
			{
				ExportImages = true,
				ExportNative = true,
				ExportPdf = true,
				QuoteDelimiter = QuoteDelimiter,
				RecordDelimiter = RecordDelimiter,
				CaseInfo = new CaseInfo
				{
					Name = _WKSP_NAME
				}
			};

			this.SubjectUnderTest = new HtmlExportFileFormatter(this.ExpFile, this.FieldNameProviderMock.Object);
			string expectedHeader = this.GetExpectedHeader(false, true, true);

			// Act
			string header = this.SubjectUnderTest.GetHeader(this.Fields.ToList());

			// Assert
			Assert.That(header, Is.EqualTo(expectedHeader));
		}

		private string GetExpectedHeader(bool addImages, bool addNatives, bool addPdfs)
		{
			StringBuilder builder = new StringBuilder($"{_HTML_TH}{FileName1}{_HTML_TH_END}{_HTML_TH}{FieldName2}{_HTML_TH_END}");
			if (addImages)
			{
				builder.Append($"{_HTML_TH}{_IMAGE_COL_NAME}{_HTML_TH_END}");
			}

			if (addNatives)
			{
				builder.Append($"{_HTML_TH}{_NATIVE_COL_NAME}{_HTML_TH_END}");
			}

			if (addPdfs)
			{
				builder.Append($"{_HTML_TH}{_PDF_COL_NAME}{_HTML_TH_END}");
			}

			return this.GetExpectedHeaderText(builder.ToString());
		}

		private string GetExpectedHeaderText(string expectedHeaderRow)
		{
			return $"{this._headerPrefix}{expectedHeaderRow}{this._headerSuffix}{Environment.NewLine}";
		}
	}
}