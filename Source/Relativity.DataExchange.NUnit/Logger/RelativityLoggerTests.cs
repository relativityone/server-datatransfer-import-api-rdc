// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RelativityLoggerTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
// Represents<see cref="RelativityLogger"/> tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Relativity.DataExchange.NUnit.Logger
{
	using global::NUnit.Framework;

	using Moq;

	using Relativity.DataExchange.Logger;
	using Relativity.Logging;

	[TestFixture]
	public class RelativityLoggerTests
	{
		[SetUp]
		public void SetUp()
		{
			RelativityLogger.Instance = null;
		}

		[Test]
		public void ShouldGetNullLoggerIfNotSet()
		{
			// ACT
			var logger = RelativityLogger.Instance;

			// ASSERT
			Assert.That(logger is NullLogger);
		}

		[Test]
		public void ShouldGetLogger()
		{
			// ARRANGE
			Mock<ILog> loggerMock = new Mock<ILog>();
			RelativityLogger.Instance = loggerMock.Object;

			// ACT
			var logger = RelativityLogger.Instance;

			// ASSERT
			Assert.AreEqual(loggerMock.Object, logger);
		}
	}
}
