// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AggregatingLoggerDecoratorTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
// Represents<see cref="AggregatingLoggerDecorator"/> tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Relativity.DataExchange.NUnit.Logger
{
	using System;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reactive.Disposables;

	using global::NUnit.Framework;
	using Moq;
	using Relativity.DataExchange.Logger;
	using Relativity.Logging;

	/// <summary>
	/// Represents<see cref="AggregatingLoggerDecorator"/> tests.
	/// </summary>
	[TestFixture]
	public class AggregatingLoggerDecoratorTests
	{
		private Mock<ILog> loggerMock;
		private Mock<ILog> additionalLoggerMock;
		private ILog decoratedLogger;

		[SetUp]
		public void SetUp()
		{
			this.loggerMock = new Mock<ILog>();
			this.additionalLoggerMock = new Mock<ILog>();
		}

		[TestCaseSource(typeof(AggregatingLoggerDecoratorTestCases), nameof(AggregatingLoggerDecoratorTestCases.ShouldLogToBothLoggersTestCaseData))]
		public void ShouldLogToBothLoggers(string message, object[] propertyValues, bool additionalLoggerIsNotNull)
		{
			// ARRANGE
			this.decoratedLogger = new AggregatingLoggerDecorator(
				this.loggerMock.Object,
				additionalLoggerIsNotNull ? this.additionalLoggerMock.Object : null);

			var exception = new Exception();

			Expression<Action<ILog>>[] methodsToTest =
				{
					x => x.LogVerbose(message, propertyValues),
					x => x.LogVerbose(exception, message, propertyValues),
					x => x.LogDebug(message, propertyValues),
					x => x.LogDebug(exception, message, propertyValues),
					x => x.LogInformation(message, propertyValues),
					x => x.LogInformation(exception, message, propertyValues),
					x => x.LogWarning(message, propertyValues),
					x => x.LogWarning(exception, message, propertyValues),
					x => x.LogError(message, propertyValues),
					x => x.LogError(exception, message, propertyValues),
					x => x.LogFatal(message, propertyValues),
					x => x.LogError(exception, message, propertyValues),
				};
			var compiled = methodsToTest.Select(x => x.Compile());

			// ACT
			foreach (Action<ILog> action in compiled)
			{
				action(this.decoratedLogger);
			}

			// ASSERT
			foreach (var methodToTest in methodsToTest)
			{
				this.loggerMock.Verify(methodToTest);
				if (additionalLoggerIsNotNull)
				{
					this.additionalLoggerMock.Verify(methodToTest);
				}
			}
		}

		[TestCase(true)]
		[TestCase(false)]
		public void ShouldForContextReturnNewAggregatingLoggerDecorator(bool additionalLoggerIsNotNull)
		{
			// ARRANGE
			this.decoratedLogger = new AggregatingLoggerDecorator(
				this.loggerMock.Object,
				additionalLoggerIsNotNull ? this.additionalLoggerMock.Object : null);

			object value = new object();
			Type type = this.GetType();
			const string propertyValue = "propertyValue";
			const bool destructureObjects = true;

			this.loggerMock
				.Setup(logger => logger.ForContext<AggregatingLoggerDecoratorTests>())
				.Returns(Mock.Of<ILog>());
			this.loggerMock
				.Setup(logger => logger.ForContext(It.IsAny<Type>()))
				.Returns(Mock.Of<ILog>());
			this.loggerMock
				.Setup(logger => logger.ForContext(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<bool>()))
				.Returns(Mock.Of<ILog>());

			this.additionalLoggerMock
				.Setup(logger => logger.ForContext<AggregatingLoggerDecoratorTests>())
				.Returns(Mock.Of<ILog>());
			this.additionalLoggerMock
				.Setup(logger => logger.ForContext(It.IsAny<Type>()))
				.Returns(Mock.Of<ILog>());
			this.additionalLoggerMock
				.Setup(logger => logger.ForContext(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<bool>()))
				.Returns(Mock.Of<ILog>());

			// ACT
			var newLogger1 = this.decoratedLogger.ForContext<AggregatingLoggerDecoratorTests>();
			var newLogger2 = this.decoratedLogger.ForContext(type);
			var newLogger3 = this.decoratedLogger.ForContext(propertyValue, value, destructureObjects);

			// ASSERT
			this.loggerMock.Verify(logger => logger.ForContext<AggregatingLoggerDecoratorTests>());
			this.loggerMock.Verify(logger => logger.ForContext(type));
			this.loggerMock.Verify(logger => logger.ForContext(propertyValue, value, destructureObjects));

			if (additionalLoggerIsNotNull)
			{
				this.additionalLoggerMock.Verify(logger => logger.ForContext<AggregatingLoggerDecoratorTests>());
				this.additionalLoggerMock.Verify(logger => logger.ForContext(type));
				this.additionalLoggerMock.Verify(logger => logger.ForContext(propertyValue, value, destructureObjects));
			}

			Assert.That(newLogger1 is AggregatingLoggerDecorator);
			Assert.That(newLogger2 is AggregatingLoggerDecorator);
			Assert.That(newLogger3 is AggregatingLoggerDecorator);
		}

		[TestCase(true)]
		[TestCase(false)]
		public void LogContextPushProperty(bool additionalLoggerIsNotNull)
		{
			// ARRANGE
			this.decoratedLogger = new AggregatingLoggerDecorator(
				this.loggerMock.Object,
				additionalLoggerIsNotNull ? this.additionalLoggerMock.Object : null);

			object value = new object();
			string propertyName = "propertyValue";

			this.loggerMock.Setup(logger => logger.LogContextPushProperty(It.IsAny<string>(), It.IsAny<object>()))
				.Returns(Mock.Of<IDisposable>());

			this.additionalLoggerMock
				.Setup(logger => logger.LogContextPushProperty(It.IsAny<string>(), It.IsAny<object>()))
				.Returns(Mock.Of<IDisposable>());

			// ACT
			var disposable = this.decoratedLogger.LogContextPushProperty(propertyName, value);

			// ASERT
			this.loggerMock.Verify(logger => logger.LogContextPushProperty(propertyName, value));
			bool resultIsCompositeDisposableType = disposable is CompositeDisposable;

			if (additionalLoggerIsNotNull)
			{
				this.additionalLoggerMock.Verify(logger => logger.LogContextPushProperty(propertyName, value));
				Assert.True(resultIsCompositeDisposableType);
			}
			else
			{
				Assert.False(resultIsCompositeDisposableType);
			}
		}
	}
}