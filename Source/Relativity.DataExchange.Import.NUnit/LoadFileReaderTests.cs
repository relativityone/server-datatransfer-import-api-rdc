// -----------------------------------------------------------------------------------------------------
// <copyright file="LoadFileReaderTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Import.Client.NUnit
{
	using System.Globalization;

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
			this.subjectUnderTest = new LoadFileReader(this.loadFileArgs, false, ExecutionSource.Rdc);
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
			this.subjectUnderTest = new LoadFileReader(this.loadFileArgs, false, executionSource);
			CultureInfo.CurrentCulture = new CultureInfo(GermanCulture);

			// Act
			decimal? result = this.subjectUnderTest.GetNullableDecimal(DecimalGermanFormat, 1);

			// Assert
			Assert.AreNotEqual(ExpectedDecimal, result);
			Assert.AreEqual(DecimalParsedInvariantWay, result);
		}
	}
}
