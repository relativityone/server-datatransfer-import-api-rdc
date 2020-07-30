// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RdcSecureLogFactory.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
// Provides fabric methods to create Relativity.Logging.ILog instances having in mind security requirements.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Logger
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Net;
	using System.Reflection;

	using Relativity.Logging;
	using Relativity.Logging.Configuration;
	using Relativity.Logging.Configuration.Serilog;
	using Relativity.Logging.Factory;

	/// <summary>
	/// Provides factory methods for RDC to create Relativity.Logging.ILog instances having in mind security requirements.
	/// </summary>
	internal class RdcSecureLogFactory : ISecureLogFactory
	{
		private const string RdcLoggingSystem = "Relativity.Desktop.Client";
		private const string RdcLoggingSubSystem = "Relativity.DataExchange";
		private const string RdcLoggingApplication = "626BD889-2BFF-4407-9CE5-5CF3712E1BB7";

		private readonly NetworkCredential credential;

		/// <summary>
		/// Initializes a new instance of the <see cref="RdcSecureLogFactory"/> class.
		/// </summary>
		public RdcSecureLogFactory()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RdcSecureLogFactory"/> class.
		/// </summary>
		/// <param name="credential">
		/// Credentials with Relativity username and password to configure HTTP sink.
		/// </param>
		public RdcSecureLogFactory(NetworkCredential credential)
		{
			this.credential = credential;
		}

		/// <inheritdoc/>
		public ILog CreateSecureLogger()
		{
			LoggerOptions options = CreateLoggerOptions(this.credential);
			ILog secureLogger;
			if (options != null)
			{
				secureLogger = LogFactory.GetLogger(options);

				var configurationReaderFactory = new ConfigurationReaderFactory();
				LogConfiguration relativityLogConfiguration =
					configurationReaderFactory.GetReader(options).ReadConfiguation();

				if (HashingRequired(relativityLogConfiguration))
				{
					LoggingLevel minimumLoggingLevel = MinimumLoggingLevel(relativityLogConfiguration);
					LogConfiguration localLogConfiguration = CreateLocalLogConfiguration(minimumLoggingLevel);
					ILog localLogger = LogFactory.GetLogger(options, localLogConfiguration);

					secureLogger = new AggregatingLoggerDecorator(
						new HashingLoggerDecorator(secureLogger),
						localLogger);
				}
			}
			else
			{
				secureLogger = LogFactory.GetNullLogger();
			}

			return secureLogger;
		}

		private static LoggingLevel MinimumLoggingLevel(LogConfiguration configuration)
		{
			return configuration.Rules.Select(rule => rule.LoggingLevel).Min();
		}

		private static bool HashingRequired(LogConfiguration configuration)
		{
			return configuration.Sinks.Any(sink => !(sink is FileSinkConfig));
		}

		private static LogConfiguration CreateLocalLogConfiguration(LoggingLevel minimumLoggingLevel)
		{
			string logFileLocation = Path.GetTempPath();

			var configuration = new LogConfiguration()
			{
				Rules = new List<Rule>()
					{
						new Rule()
							{
								System = "*",
								LoggingLevel = minimumLoggingLevel,
								Sinks = new List<string> { "File1" },
							},
					},
				Sinks = new List<Sink>()
					{
						new FileSinkConfig("File1", logFileLocation),
					},
				LoggingEnabled = true,
			};

			return configuration;
		}

		private static LoggerOptions CreateLoggerOptions(NetworkCredential credential)
		{
			string configFileName = AppSettings.Instance.LogConfigXmlFileName;
			if (string.IsNullOrWhiteSpace(configFileName) || !Path.IsPathRooted(configFileName) || !File.Exists(configFileName))
			{
				configFileName = null;
				Assembly assembly = Assembly.GetCallingAssembly();
				if (assembly != null)
				{
					string directory = Directory.GetParent(assembly.Location).FullName;
					string path = Path.Combine(directory, "LogConfig.xml");
					if (!File.Exists(path))
					{
						return null;
					}

					configFileName = path;
				}
			}

			var options = new LoggerOptions()
			{
				Application = RdcLoggingApplication,
				System = RdcLoggingSystem,
				SubSystem = RdcLoggingSubSystem,
				ConfigurationFileLocation = configFileName,
			};

			if (credential != null && !credential.Password.IsNullOrEmpty() && !credential.UserName.IsNullOrEmpty())
			{
				Uri webServiceUri = new Uri(AppSettings.Instance.WebApiServiceUrl);
				Uri host = new Uri(webServiceUri.GetLeftPart(UriPartial.Authority));

				options.AddSinkParameter(RelativityHttpSinkConfig.InstanceUrlSinkParameterKey, host);
				options.AddSinkParameter(
					RelativityHttpSinkConfig.CredentialSinkParameterKey,
					new NetworkCredential(credential.UserName, credential.Password));
			}

			return options;
		}
	}
}