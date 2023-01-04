// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImportApiSecureLogFactoryTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
// Represents<see cref="SecureLogFactory"/> tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Relativity.DataExchange.NUnit.Logger
{
	using global::NUnit.Framework;

	using Moq;

	using Relativity.DataExchange.Logger;
	using Relativity.Logging;

	[TestFixture]
	public class ImportApiSecureLogFactoryTests
	{
		[TestCase(true)]
		[TestCase(false)]
		public void ShouldSecureCurrentLogger(bool logHashingEnabled)
		{
			// ARRANGE
			Mock<ILog> currentLoggerMock = new Mock<ILog>();
			Log.Logger = currentLoggerMock.Object;
			AppSettings.Instance.LogHashingEnabled = logHashingEnabled;
			var secureLogFactory = new ImportApiSecureLogFactory();

			// ACT
			ILog decoratedLogger = secureLogFactory.CreateSecureLogger();

			// ASSERT
			Assert.That(decoratedLogger is HashingLoggerDecorator == logHashingEnabled);
		}
	}
}
