// -----------------------------------------------------------------------------------------------------
// <copyright file="DataReaderReaderTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Import.Client.NUnit
{
	using System.Collections.Generic;
	using System.Linq;

	using global::NUnit.Framework;

	using kCura.WinEDDS;

	[TestFixture]
	public class DataReaderReaderTests
	{
		[Test]
		public void GetListOfItemsFromStringDefaultCodePath()
		{
			var itemsUnderTest = new List<string> { "test1", "test2" };

			// Arrange
			var items = string.Join(";", itemsUnderTest);

			// Act
			var retVal = LoadFileReader.GetStringArrayFromDelimitedFieldValue(items, ';').ToList();

			// Assert
			Assert.AreEqual(2, itemsUnderTest.Intersect(retVal).Count());
			Assert.AreEqual(2, retVal.Count);
		}

		[Test]
		public void GetListOfItemsFromStringInvalidXml()
		{
			var itemsUnderTest = new List<string> { "weatherford.com??S\"Findley, Kari\" <kari.findley", "test2" };

			// Arrange
			var items = string.Join(";", itemsUnderTest);

			// Act
			var retVal = LoadFileReader.GetStringArrayFromDelimitedFieldValue(items, ';').ToList();

			// Assert
			Assert.AreEqual(2, itemsUnderTest.Intersect(retVal).Count());
			Assert.AreEqual(2, retVal.Count);
		}

		[Test]
		public void GetListOfItemsFromStringBlankString()
		{
			// Arrange
			var items = string.Empty;

			// Act
			var retVal = LoadFileReader.GetStringArrayFromDelimitedFieldValue(items, ';').ToList();

			// Assert
			Assert.AreEqual(0, retVal.Count);
		}
	}
}