// -----------------------------------------------------------------------------------------------------
// <copyright file="ImportExportCompatibilityCheckTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Import.Export.NUnit
{
	using System;

	using global::NUnit.Framework;

	using Moq;

	using Relativity.Logging;

	public class ImportExportCompatibilityCheckTests
	{
		private Mock<IApplicationVersionService> relativityVersionServiceMock;
		private Mock<ILog> logMock;

		[SetUp]
		public void Setup()
		{
			this.relativityVersionServiceMock = new Mock<IApplicationVersionService>();
			this.logMock = new Mock<ILog>();
		}

		[Test]
		[TestCase("9.7.228.0", "0.0", false)]
		[TestCase("9.7.229.0", "0.0", true)]
		[TestCase("10.1.2.0", "0.0", true)]
		[TestCase("10.3.0.0", "1.0", true)]
		[TestCase("10.3.0.0", "1.5", true)]
		[TestCase("10.4.0.0", "2.0", false)]
		public void VerifyIApiClientAndServerVersionCompatibility(
			string relativityVersion,
			string webApiVersion,
			bool expectedResult)
		{
			// arrange
			Version relativityVer = Version.Parse(relativityVersion);
			Version webApiVer = Version.Parse(webApiVersion);

			this.relativityVersionServiceMock.Setup(x => x.RetrieveRelativityVersion()).Returns(relativityVer);
			this.relativityVersionServiceMock.Setup(x => x.RetrieveImportExportWebApiVersion()).Returns(webApiVer);

			ImportExportCompatibilityCheck subjectUnderTest = new ImportExportCompatibilityCheck(this.relativityVersionServiceMock.Object, this.logMock.Object);

			// act
			var actualResult = subjectUnderTest.ValidateCompatibility();

			// assert
			if (relativityVer < MinimalCompatibleRelativitySemanticVersion.Version)
			{// we don't expect to call web api for its version for old Relativity instance
				this.relativityVersionServiceMock.Verify(x => x.RetrieveImportExportWebApiVersion(), Times.Never);
			}
			else
			{
				this.relativityVersionServiceMock.Verify(x => x.RetrieveImportExportWebApiVersion(), Times.Once);
			}

			Assert.That(actualResult, Is.EqualTo(expectedResult));
		}
	}
}
