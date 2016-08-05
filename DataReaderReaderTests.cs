using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace kCura.WinEDDS.ImportExtension.NUnit
{
	[TestFixture]
	public class DataReaderReaderTests
	{
		[SetUp]
		public void Setup()
		{

		}

		[Test]
		public void GetListOfItemsFromString_DefaultCodePath()
		{
			var itemsUnderTest = new List<string>(){"test1", "test2"};
			//Arrange
			var items = String.Join(";", itemsUnderTest);
			//Act
			var retVal = LoadFileReader.GetStringArrayFromDelimitedFieldValue(items, ';').ToList();
			//Assert
			Assert.AreEqual(2, itemsUnderTest.Intersect(retVal).Count());
			Assert.AreEqual(2, retVal.Count);
		}

		[Test]
		public void GetListOfItemsFromString_InvalidXML()
		{
			var itemsUnderTest = new List<string>() { "weatherford.com??S\"Findley, Kari\" <kari.findley", "test2" };
			//Arrange
			var items = String.Join(";", itemsUnderTest);
			//Act
			var retVal = LoadFileReader.GetStringArrayFromDelimitedFieldValue(items, ';').ToList();
			//Assert
			Assert.AreEqual(2, itemsUnderTest.Intersect(retVal).Count());
			Assert.AreEqual(2, retVal.Count);
		}

		[Test]
		public void GetListOfItemsFromString_BlankString()
		{
			//Arrange
			var items = String.Empty;
			//Act
			var retVal = LoadFileReader.GetStringArrayFromDelimitedFieldValue(items, ';').ToList();
			//Assert
			Assert.AreEqual(0, retVal.Count);
		}


		[TearDown]
		public void TearDown()
		{

		}
	}
}
