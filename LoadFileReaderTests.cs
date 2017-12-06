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

		[SetUp]
		public void Setup()
		{
			_loadFileArgs = new LoadFile() {CaseInfo = new CaseInfo()};
		}

		[Test]
		public void GetNullableDecimal_UsesCurrentCultureOnRdc()
		{
			_subjectUnderTest = new LoadFileReader( _loadFileArgs , false, ExecutionSource.Rdc);

			CultureInfo.CurrentCulture = new CultureInfo("de-DE");
			decimal? result = _subjectUnderTest.GetNullableDecimal("10,05", 1);

			Assert.AreEqual( 10.05d, result);
		}


		[TearDown]
		public void TearDown()
		{

		}
	}
}
