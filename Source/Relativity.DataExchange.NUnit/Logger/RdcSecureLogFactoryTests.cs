// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RdcSecureLogFactoryTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
// Represents<see cref="SecureLogFactory"/> tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Relativity.DataExchange.NUnit.Logger
{
	using System.IO;
	using System.Reflection;

	using global::NUnit.Framework;

	using Moq;

	using Relativity.DataExchange.Logger;
	using Relativity.Logging;

	/// <summary>
	/// Represents<see cref="RdcSecureLogFactory"/> tests.
	/// </summary>
	[TestFixture]
	public class RdcSecureLogFactoryTests
	{
		private const string RdcLoggingSystem = "Relativity.Desktop.Client";
		private const string RdcLoggingSubSystem = "Relativity.DataExchange";
		private const string RdcLoggingApplication = "626BD889-2BFF-4407-9CE5-5CF3712E1BB7";

		[TestCase(true)]
		[TestCase(false)]
		public void ShouldCreateSecureLogger(bool hashingRequired)
		{
			// ARRANGE
			string configFile = hashingRequired ? "TestLogConfigSplunk.xml" : "TestLogConfigFile.xml";
			string directory = Directory.GetParent(Assembly.GetCallingAssembly().Location).FullName;
			AppSettings.Instance.LogConfigXmlFileName = Path.Combine(directory, "Logger", configFile);
			var secureLogFactory = new RdcSecureLogFactory();

			// ACT
			ILog secureLogger = secureLogFactory.CreateSecureLogger();

			// ASSERT
			Assert.That(secureLogger is AggregatingLoggerDecorator == hashingRequired);
			Assert.AreEqual(RdcLoggingApplication, secureLogger.Application);
			Assert.AreEqual(RdcLoggingSystem, secureLogger.System);
			Assert.AreEqual(RdcLoggingSubSystem, secureLogger.SubSystem);
		}

		[Test]
		public void ShouldCreateSecureLoggerUsingDefaultConfig()
		{
			// ARRANGE
			string directory = Directory.GetParent(Assembly.GetCallingAssembly().Location).FullName;
			string logConfigFilePath = Path.Combine(directory, "Logger", "LogConfig.xml");
			string defaultLogConfigFilePath = Path.Combine(directory, "LogConfig.xml");
			File.Copy(logConfigFilePath, defaultLogConfigFilePath);

			var secureLogFactory = new RdcSecureLogFactory();

			// ACT
			ILog secureLogger = secureLogFactory.CreateSecureLogger();
			File.Delete(defaultLogConfigFilePath);

			// ASSERT
			Assert.That(!(secureLogger is NullLogger));
			Assert.AreEqual(RdcLoggingApplication, secureLogger.Application);
			Assert.AreEqual(RdcLoggingSystem, secureLogger.System);
			Assert.AreEqual(RdcLoggingSubSystem, secureLogger.SubSystem);
		}

		[Test]
		public void ShouldCreateNullLogger()
		{
			// ARRANGE
			var secureLogFactory = new RdcSecureLogFactory();

			// ACT
			ILog secureLogger = secureLogFactory.CreateSecureLogger();

			// ASSERT
			Assert.That(secureLogger is NullLogger);
		}
	}
}
