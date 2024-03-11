// -----------------------------------------------------------------------------------------------------
// <copyright file="LoadFileReaderTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit
{
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;

	using global::NUnit.Framework;

	using kCura.WinEDDS;

	using Relativity.DataExchange.Service;

	[TestFixture]
	public class LoadFileReaderTests
	{
		private const decimal ExpectedDecimal = 10.05m;
		private const decimal DecimalParsedInvariantWay = 1005m;
		private const string DecimalGermanFormat = "10,05";
		private const string GermanCulture = "de-DE";
		private LoadFileReader subjectUnderTest;
		private LoadFile loadFileArgs;

		[SetUp]
		public void Setup()
		{
			CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
			this.loadFileArgs = new LoadFile { CaseInfo = new Relativity.DataExchange.Service.CaseInfo() };
		}

		[Test]
		public void GetNullableDecimalUsesCurrentCultureOnRdc()
		{
			// Arrange
			this.subjectUnderTest = new LoadFileReader(this.loadFileArgs, false, () => this.GetType().Name, ExecutionSource.Rdc);
			CultureInfo.CurrentCulture = new CultureInfo(GermanCulture);

			// Act
			decimal? result = this.subjectUnderTest.GetNullableDecimal(DecimalGermanFormat, 1);

			// Assert
			Assert.AreEqual(ExpectedDecimal, result);
		}

		[Test]
		[TestCase(ExecutionSource.Unknown)]
		[TestCase(ExecutionSource.ImportAPI)]
		[TestCase(ExecutionSource.Processing)]
		[TestCase(ExecutionSource.RIP)]
		public void GetNullableDecimalNeglectsCurrentCultureOnOtherExecutionSources(ExecutionSource executionSource)
		{
			// Arrange
			this.subjectUnderTest = new LoadFileReader(this.loadFileArgs, false, () => this.GetType().Name, executionSource);
			CultureInfo.CurrentCulture = new CultureInfo(GermanCulture);

			// Act
			decimal? result = this.subjectUnderTest.GetNullableDecimal(DecimalGermanFormat, 1);

			// Assert
			Assert.AreNotEqual(ExpectedDecimal, result);
			Assert.AreEqual(DecimalParsedInvariantWay, result);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "It is a unit test")]
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

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "It is a unit test")]
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

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "It is a unit test")]
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
