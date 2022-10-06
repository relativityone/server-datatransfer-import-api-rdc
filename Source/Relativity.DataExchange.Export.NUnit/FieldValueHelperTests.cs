// <copyright file="FieldValueHelperTests.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>
namespace Relativity.DataExchange.Export.NUnit
{
	using global::NUnit.Framework;
	using kCura.WinEDDS.Helpers;
	using Relativity.DataExchange.Service;

	[TestFixture]
	public class FieldValueHelperTests
	{
		private const char MultiValueDelimiter = ';';

		[TestCase("<object>Value1</object><object>Value2</object><object>Value3</object>", "Value1;Value2;Value3")]
		[TestCase("<object/> <object>Value2</object> <object>Value3</object>", ";Value2;Value3")]
		[TestCase("<object>Value1</object><object>Value2</object><object/>", "Value1;Value2;")]
		[TestCase("<object>Value1</object><object/><object>Value3</object>", "Value1;;Value3")]
		[TestCase("<object/><object>Value2</object><object/>", ";Value2;")]
		[TestCase("<object/><object/><object/>", ";;")]
		[TestCase("<object/>", "")]
		[TestCase("", "")]
		[TestCase("nonXmlValue", "nonXmlValue")]
		public void ItShouldReturnCorrectMultiValueString(string input, string expected)
		{
			// Act
			string fieldValue = FieldValueHelper.GetMultiValueString(input, FieldType.Text, string.Empty, MultiValueDelimiter);

			// Assert
			Assert.AreEqual(expected, fieldValue);
		}
	}
}
