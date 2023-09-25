// <copyright file="FieldValueHelperTests.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>
namespace Relativity.DataExchange.Export.NUnit
{
    using System.Linq;
    using global::NUnit.Framework;
    using kCura.WinEDDS.Helpers;
    using Relativity.DataExchange.Service;

    [TestFixture]
	public class FieldValueHelperTests
	{
		private const FieldType TextFieldType = FieldType.Text;
		private const FieldType DateFieldType = FieldType.Date;
		private string fieldFormatStringEmpty = string.Empty;
		private string fieldFormatStringDate = "d";

		[Test]
		[TestCase("<object>Value1</object><object>Value2</object><object>Value3</object>",	';', "Value1;Value2;Value3")]
		[TestCase("<object/> <object>Value2</object> <object>Value3</object>", ';', ";Value2;Value3")]
		[TestCase("<object>Value1</object><object>Value2</object><object/>", ';', "Value1;Value2;")]
		[TestCase("<object>Value1</object><object/><object>Value3</object>", ';', "Value1;;Value3")]
		[TestCase("<object/><object>Value2</object><object/>", ';', ";Value2;")]
		[TestCase("<object/><object/><object/>", ';', ";;")]
		[TestCase("<object/>", ';', "")]
		[TestCase("", ';', "")]
		[TestCase("nonXmlValue", ';', "nonXmlValue")]
		[TestCase("&><'\"", ';', "&><'\"")]
		[TestCase("<object>&amp;</object><object>&gt;</object><object>&lt;</object>", ':', "&:>:<")]
		[TestCase("<object/><object>Value2</object><object/>", ':', ":Value2:")]
		[TestCase("<object/><object/><object/>", ':', "::")]
		[TestCase("<object/>", ':', "")]
		public void ItShouldReturnCorrectMultiValueStringForText(string input, char delimiter, string expected)
		{
			// Act
			string fieldValue = FieldValueHelper.GetMultiValueString(input, TextFieldType, fieldFormatStringEmpty, delimiter);

			// Assert
			Assert.AreEqual(expected, fieldValue);
		}

		[Test]
		[TestCase("<object>Sep 24 2020 12:0:00AM</object>", ';', "2020-09-24 12:00:00")]
		[TestCase("<object>Sep 24 2020 12:0:00AM</object>", ';', "2020-09-24")]
		[TestCase("<object>2023-12-31</object><object>2019-12-31</object><object>2023-01-01</object>", ';', "2023-12-31;2019-12-31;2023-01-01")]
		[TestCase("<object/> <object>Sep 24 2020 12:0:00AM</object> <object>Sep 1 2020 12:0:00AM</object>", ';', ";2020-09-24 12:00:00;2020-09-01 12:00:00")]
		[TestCase("<object/><object/><object/>", ';', ";;")]
		[TestCase("<object/>", ';', "")]
		[TestCase("", ';', "")]
		public void ItShouldReturnCorrectMultiValueStringForDates(string input, char delimiter, string expected)
        {
			// Act
			string fieldValue = FieldValueHelper.GetMultiValueString(input, DateFieldType, fieldFormatStringDate, delimiter);

			// Assert
			string expectedLocal = null;
			if (expected != null)
            {
				expectedLocal = string.Join(
					delimiter.ToString(),
					expected.Split(delimiter).Select(
						d => FieldValueHelper.ToExportableDateString(d, fieldFormatStringDate)).ToArray<string>());
			}

			Assert.AreEqual(expectedLocal, fieldValue);
		}
	}
}