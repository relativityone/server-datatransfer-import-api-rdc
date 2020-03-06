// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HashingLoggerDecoratorTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
// Represents<see cref="HashingLoggerDecorator"/> tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Relativity.DataExchange.NUnit.Logger
{
	using System;
	using System.Linq;
	using global::NUnit.Framework;
	using Moq;
	using Relativity.DataExchange.Logger;
	using Relativity.Logging;

	/// <summary>
	/// Represents<see cref="HashingLoggerDecorator"/> tests.
	/// </summary>
	[TestFixture]
	public class HashingLoggerDecoratorTests
	{
		private Mock<ILog> loggerMock;
		private ILog decoratedLogger;

		[SetUp]
		public void SetUp()
		{
			this.loggerMock = new Mock<ILog>();
			this.decoratedLogger = new HashingLoggerDecorator(this.loggerMock.Object);
		}

		[Test]
		[TestCaseSource(typeof(HashingLoggerDecoratorTestCases), nameof(HashingLoggerDecoratorTestCases.ShouldHashSensitiveDataTestCaseData))]
		public void ShouldHashSensitiveData(string message, object[] sourcePropertyValues, object[] expectedPropertyValues, bool lastPropertyValueRemoved)
		{
			// ACT
			this.decoratedLogger.LogVerbose(message, sourcePropertyValues);
			this.decoratedLogger.LogDebug(message, sourcePropertyValues);
			this.decoratedLogger.LogInformation(message, sourcePropertyValues);
			this.decoratedLogger.LogWarning(message, sourcePropertyValues);
			this.decoratedLogger.LogError(message, sourcePropertyValues);
			this.decoratedLogger.LogFatal(message, sourcePropertyValues);

			// ASSERT
			this.loggerMock.Verify(logger => logger.LogVerbose(message, It.Is<object[]>(loggerPropertyValues => this.VerifyLoggerPropertyValuesAreChangedProperly(sourcePropertyValues, loggerPropertyValues, expectedPropertyValues, lastPropertyValueRemoved))));
			this.loggerMock.Verify(logger => logger.LogDebug(message, It.Is<object[]>(loggerPropertyValues => this.VerifyLoggerPropertyValuesAreChangedProperly(sourcePropertyValues, loggerPropertyValues, expectedPropertyValues, lastPropertyValueRemoved))));
			this.loggerMock.Verify(logger => logger.LogInformation(message, It.Is<object[]>(loggerPropertyValues => this.VerifyLoggerPropertyValuesAreChangedProperly(sourcePropertyValues, loggerPropertyValues, expectedPropertyValues, lastPropertyValueRemoved))));
			this.loggerMock.Verify(logger => logger.LogWarning(message, It.Is<object[]>(loggerPropertyValues => this.VerifyLoggerPropertyValuesAreChangedProperly(sourcePropertyValues, loggerPropertyValues, expectedPropertyValues, lastPropertyValueRemoved))));
			this.loggerMock.Verify(logger => logger.LogError(message, It.Is<object[]>(loggerPropertyValues => this.VerifyLoggerPropertyValuesAreChangedProperly(sourcePropertyValues, loggerPropertyValues, expectedPropertyValues, lastPropertyValueRemoved))));
			this.loggerMock.Verify(logger => logger.LogFatal(message, It.Is<object[]>(loggerPropertyValues => this.VerifyLoggerPropertyValuesAreChangedProperly(sourcePropertyValues, loggerPropertyValues, expectedPropertyValues, lastPropertyValueRemoved))));
		}

		[Test]
		[TestCaseSource(typeof(HashingLoggerDecoratorTestCases), nameof(HashingLoggerDecoratorTestCases.ShouldHashSensitiveDataTestCaseData))]
		public void ShouldHashSensitiveDataWithException(string message, object[] sourcePropertyValues, object[] expectedPropertyValues, bool lastPropertyValueRemoved)
		{
			// ACT
			this.decoratedLogger.LogVerbose(new Exception(), message, sourcePropertyValues);
			this.decoratedLogger.LogDebug(new Exception(), message, sourcePropertyValues);
			this.decoratedLogger.LogInformation(new Exception(), message, sourcePropertyValues);
			this.decoratedLogger.LogWarning(new Exception(), message, sourcePropertyValues);
			this.decoratedLogger.LogError(new Exception(), message, sourcePropertyValues);
			this.decoratedLogger.LogFatal(new Exception(), message, sourcePropertyValues);

			// ASSERT
			this.loggerMock.Verify(logger => logger.LogVerbose(It.IsAny<Exception>(), message, It.Is<object[]>(loggerPropertyValues => this.VerifyLoggerPropertyValuesAreChangedProperly(sourcePropertyValues, loggerPropertyValues, expectedPropertyValues, lastPropertyValueRemoved))));
			this.loggerMock.Verify(logger => logger.LogDebug(It.IsAny<Exception>(), message, It.Is<object[]>(loggerPropertyValues => this.VerifyLoggerPropertyValuesAreChangedProperly(sourcePropertyValues, loggerPropertyValues, expectedPropertyValues, lastPropertyValueRemoved))));
			this.loggerMock.Verify(logger => logger.LogInformation(It.IsAny<Exception>(), message, It.Is<object[]>(loggerPropertyValues => this.VerifyLoggerPropertyValuesAreChangedProperly(sourcePropertyValues, loggerPropertyValues, expectedPropertyValues, lastPropertyValueRemoved))));
			this.loggerMock.Verify(logger => logger.LogWarning(It.IsAny<Exception>(), message, It.Is<object[]>(loggerPropertyValues => this.VerifyLoggerPropertyValuesAreChangedProperly(sourcePropertyValues, loggerPropertyValues, expectedPropertyValues, lastPropertyValueRemoved))));
			this.loggerMock.Verify(logger => logger.LogError(It.IsAny<Exception>(), message, It.Is<object[]>(loggerPropertyValues => this.VerifyLoggerPropertyValuesAreChangedProperly(sourcePropertyValues, loggerPropertyValues, expectedPropertyValues, lastPropertyValueRemoved))));
			this.loggerMock.Verify(logger => logger.LogFatal(It.IsAny<Exception>(), message, It.Is<object[]>(loggerPropertyValues => this.VerifyLoggerPropertyValuesAreChangedProperly(sourcePropertyValues, loggerPropertyValues, expectedPropertyValues, lastPropertyValueRemoved))));
		}

		[Test]
		[TestCaseSource(typeof(HashingLoggerDecoratorTestCases), nameof(HashingLoggerDecoratorTestCases.ShouldHashSensitiveDataLogIndexErrorTestCaseData))]
		public void ShouldHashSensitiveDataLogIndexError(string message, object[] sourcePropertyValues, object[] expectedPropertyValues, bool lastPropertyValueRemoved)
		{
			// ACT
			this.decoratedLogger.LogVerbose(new Exception(), message, sourcePropertyValues);
			this.decoratedLogger.LogDebug(new Exception(), message, sourcePropertyValues);
			this.decoratedLogger.LogInformation(new Exception(), message, sourcePropertyValues);
			this.decoratedLogger.LogWarning(new Exception(), message, sourcePropertyValues);
			this.decoratedLogger.LogError(new Exception(), message, sourcePropertyValues);
			this.decoratedLogger.LogFatal(new Exception(), message, sourcePropertyValues);

			// ASSERT
			this.loggerMock.Verify(logger => logger.LogVerbose(It.IsAny<Exception>(), message, It.Is<object[]>(loggerPropertyValues => this.VerifyLoggerPropertyValuesAreChangedProperly(sourcePropertyValues, loggerPropertyValues, expectedPropertyValues, lastPropertyValueRemoved))));
			this.loggerMock.Verify(logger => logger.LogDebug(It.IsAny<Exception>(), message, It.Is<object[]>(loggerPropertyValues => this.VerifyLoggerPropertyValuesAreChangedProperly(sourcePropertyValues, loggerPropertyValues, expectedPropertyValues, lastPropertyValueRemoved))));
			this.loggerMock.Verify(logger => logger.LogInformation(It.IsAny<Exception>(), message, It.Is<object[]>(loggerPropertyValues => this.VerifyLoggerPropertyValuesAreChangedProperly(sourcePropertyValues, loggerPropertyValues, expectedPropertyValues, lastPropertyValueRemoved))));
			this.loggerMock.Verify(logger => logger.LogWarning(It.IsAny<Exception>(), message, It.Is<object[]>(loggerPropertyValues => this.VerifyLoggerPropertyValuesAreChangedProperly(sourcePropertyValues, loggerPropertyValues, expectedPropertyValues, lastPropertyValueRemoved))));
			this.loggerMock.Verify(logger => logger.LogError(It.IsAny<Exception>(), message, It.Is<object[]>(loggerPropertyValues => this.VerifyLoggerPropertyValuesAreChangedProperly(sourcePropertyValues, loggerPropertyValues, expectedPropertyValues, lastPropertyValueRemoved))));
			this.loggerMock.Verify(logger => logger.LogFatal(It.IsAny<Exception>(), message, It.Is<object[]>(loggerPropertyValues => this.VerifyLoggerPropertyValuesAreChangedProperly(sourcePropertyValues, loggerPropertyValues, expectedPropertyValues, lastPropertyValueRemoved))));

			this.loggerMock.Verify(logger => logger.LogError(It.IsAny<string>(), null), Times.Exactly(6));
		}

		[Test]
		public void ShouldForContextReturnNewHashingLoggerDecorator()
		{
			// ARRANGE
			object propertyValue = "propertyValue".Secure();
			Type type = this.GetType();
			string propertyName = "propertyName";
			bool destructureObjects = true;

			this.loggerMock
				.Setup(logger => logger.ForContext<HashingLoggerDecoratorTests>())
				.Returns(Mock.Of<ILog>());
			this.loggerMock
				.Setup(logger => logger.ForContext(It.IsAny<Type>()))
				.Returns(Mock.Of<ILog>());
			this.loggerMock
				.Setup(logger => logger.ForContext(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<bool>()))
				.Returns(Mock.Of<ILog>());

			// ACT
			var newLogger1 = this.decoratedLogger.ForContext<HashingLoggerDecoratorTests>();
			var newLogger2 = this.decoratedLogger.ForContext(type);
			var newLogger3 = this.decoratedLogger.ForContext(propertyName, propertyValue, destructureObjects);

			// ASSERT
			this.loggerMock.Verify(logger => logger.ForContext<HashingLoggerDecoratorTests>());
			this.loggerMock.Verify(logger => logger.ForContext(type));
			this.loggerMock.Verify(logger => logger.ForContext(propertyName, It.Is<object>(loggerPropertyValue => this.VerifyPropertyValueIsChangedProperly(propertyValue, loggerPropertyValue)), destructureObjects));

			Assert.That(newLogger1 is HashingLoggerDecorator);
			Assert.That(newLogger2 is HashingLoggerDecorator);
			Assert.That(newLogger3 is HashingLoggerDecorator);
		}

		private bool VerifyLoggerPropertyValuesAreChangedProperly(object[] sourcePropertyValues, object[] resultPropertyValues, object[] expectedPropertyValues, bool lastPropertyRemoved)
		{
			if (lastPropertyRemoved)
			{
				Assert.AreEqual(sourcePropertyValues.Length - 1, resultPropertyValues.Length);
			}

			if (resultPropertyValues != null)
			{
				for (int index = 0; index < resultPropertyValues.Length; index++)
				{
					Assert.AreEqual(expectedPropertyValues[index], resultPropertyValues[index]);
				}
			}

			return true;
		}

		private bool VerifyPropertyValueIsChangedProperly(object sourcePropertyValue, object propertyValue)
		{
			Assert.AreNotEqual(sourcePropertyValue, propertyValue);
			return true;
		}
	}
}