using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Relativity;

namespace kCura.WinEDDS.ImportExtension.NUnit
{
	[TestFixture]
	public class LoadFileReaderTests
	{
		private LoadFileReader _subjectUnderTest;
		private LoadFile _loadFileArgs;
		private const decimal _EXPECTED_DECIMAL = 10.05m;
		private const decimal _DECIMAL_PARSED_INVARIANT_WAY = 1005m;
		private const string _DECIMAL_GERMAN_FORMAT = "10,05";
		private const string _GERMAN_CULTURE = "de-DE";

		[SetUp]
		public void Setup()
		{
			CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
			_loadFileArgs = new LoadFile() {CaseInfo = new CaseInfo()};
		}

		[Test]
		public void GetNullableDecimal_UsesCurrentCultureOnRdc()
		{
			//Arrange
			_subjectUnderTest = new LoadFileReader( _loadFileArgs , false, ExecutionSource.Rdc);
			CultureInfo.CurrentCulture = new CultureInfo(_GERMAN_CULTURE);

			//Act
			decimal? result = _subjectUnderTest.GetNullableDecimal(_DECIMAL_GERMAN_FORMAT, 1);

			//Assert
			Assert.AreEqual( _EXPECTED_DECIMAL , result);
		}

		[Test]
		[TestCase(ExecutionSource.Unknown)]
		[TestCase(ExecutionSource.ImportAPI)]
		[TestCase(ExecutionSource.Processing)]
		[TestCase(ExecutionSource.RIP)]
		public void GetNullableDecimal_NeglectsCurrentCultureOnOtherExecutionSources( ExecutionSource executionSource)
		{
			//Arrange
			_subjectUnderTest = new LoadFileReader(_loadFileArgs, false, executionSource);
			CultureInfo.CurrentCulture = new CultureInfo(_GERMAN_CULTURE);

			//Act
			decimal? result = _subjectUnderTest.GetNullableDecimal(_DECIMAL_GERMAN_FORMAT, 1);

			//Assert
			Assert.AreNotEqual(_EXPECTED_DECIMAL, result);
			Assert.AreEqual(_DECIMAL_PARSED_INVARIANT_WAY, result);
		}
	}
}
