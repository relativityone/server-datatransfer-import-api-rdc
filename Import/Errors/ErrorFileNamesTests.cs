using System;
using kCura.WinEDDS.Core.Import.Errors;
using Moq;
using NUnit.Framework;

namespace kCura.WinEDDS.Core.NUnit.Import.Errors
{
	[TestFixture]
	public class ErrorFileNamesTests
	{
		private const string _FILE_SUFFIX = "123456";

		private ErrorFileNames _instance;

		[SetUp]
		public void SetUp()
		{
			var dateTimeHelper = new Mock<IDateTimeHelper>();
			dateTimeHelper.Setup(x => x.Now()).Returns(DateTime.MinValue + TimeSpan.FromTicks(int.Parse(_FILE_SUFFIX)));

			_instance = new ErrorFileNames(dateTimeHelper.Object);
		}

		[Test]
		[TestCase("a.dat", "a_ErrorLines_123456.dat")]
		[TestCase("a", "a_ErrorLines_123456.txt")]
		[TestCase("c:\\temp\\a.dat", "a_ErrorLines_123456.dat")]
		[TestCase("qwerty.asd.dat", "qwerty.asd_ErrorLines_123456.dat")]
		public void ItShouldReturnValidErrorLinesFileName(string loadFilePath, string expectedFileName)
		{
			// ACT
			var actualFileName = _instance.GetErrorLinesFileName(loadFilePath);

			// ASSERT
			Assert.That(actualFileName, Is.EqualTo(expectedFileName));
		}

		[Test]
		[TestCase("a.dat", "a_ErrorReport_123456.csv")]
		[TestCase("a", "a_ErrorReport_123456.csv")]
		[TestCase("c:\\temp\\a.dat", "a_ErrorReport_123456.csv")]
		[TestCase("qwerty.asd.dat", "qwerty.asd_ErrorReport_123456.csv")]
		public void ItShouldReturnValidErrorReportFileName(string loadFilePath, string expectedFileName)
		{
			// ACT
			var actualFileName = _instance.GetErrorReportFileName(loadFilePath);

			// ASSERT
			Assert.That(actualFileName, Is.EqualTo(expectedFileName));
		}
	}
}