﻿// ----------------------------------------------------------------------------
// <copyright file="IntegrationTestHelper.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Data;
	using System.Data.SqlClient;
	using System.IO;
	using System.Linq;
	using System.Net;
	using System.Reflection;

	using kCura.Relativity.ImportAPI;

	using Newtonsoft.Json;

	using Relativity.DataExchange.TestFramework.RelativityHelpers;
	using Relativity.Logging;

	/// <summary>
	/// Defines static methods to setup and teardown integration tests.
	/// </summary>
	public static class IntegrationTestHelper
	{
		private static IntegrationTestParameters integrationTestParameters;

		public static IntegrationTestParameters IntegrationTestParameters
		{
			get => integrationTestParameters ?? (integrationTestParameters = ReadAndLogIntegrationTestParameters());
			set => integrationTestParameters = value;
		}

		/// <summary>
		/// Gets or sets a value indicating whether environment variables are enabled.
		/// </summary>
		/// <value>
		/// <see langword="true" /> when environment variables are enabled; otherwise, <see langword="false" />.
		/// </value>
		public static bool EnvironmentVariablesEnabled { get; set; } = true;

		/// <summary>
		/// Gets the Relativity logging instance.
		/// </summary>
		/// <value>
		/// The <see cref="Relativity.Logging.ILog"/> instance.
		/// </value>
		public static Relativity.Logging.ILog Logger { get; private set; }

		/// <summary>
		/// Create the integration test environment with a new test workspace and returns the test parameters.
		/// </summary>
		/// <returns>
		/// The <see cref="TestFramework.IntegrationTestParameters"/> instance.
		/// </returns>
		public static IntegrationTestParameters Create()
		{
			// Note: don't create the logger until all parameters have been retrieved.
			IntegrationTestParameters parameters = IntegrationTestParameters;
			SetupLogger(parameters);
			SetupServerCertificateValidation(parameters);
			if (parameters.SkipIntegrationTests)
			{
				Logger.LogInformation("Skipping test workspace creation.");
				return parameters;
			}

			Logger.LogInformation("Creating a test workspace...");
			WorkspaceHelper.CreateTestWorkspace(parameters, Logger);

			var importApi = new ImportAPI(
				parameters.RelativityUserName,
				parameters.RelativityPassword,
				parameters.RelativityWebApiUrl.ToString());
			IEnumerable<kCura.Relativity.ImportAPI.Data.Workspace> workspaces = importApi.Workspaces();
			kCura.Relativity.ImportAPI.Data.Workspace workspace =
				workspaces.FirstOrDefault(x => x.ArtifactID == parameters.WorkspaceId);
			if (workspace == null)
			{
				throw new InvalidOperationException(
					$"This operation cannot be performed because the workspace {parameters.WorkspaceId} that was just created doesn't exist.");
			}

			parameters.FileShareUncPath = workspace.DocumentPath;
			Logger.LogInformation("Created {workspaceId} test workspace.", parameters.WorkspaceId);
			return parameters;
		}

		/// <summary>
		/// Get connection string.
		/// </summary>
		/// <param name="parameters">
		/// The integration test parameters.
		/// </param>
		/// <returns>The builder with populated values.</returns>
		public static SqlConnectionStringBuilder GetSqlConnectionStringBuilder(IntegrationTestParameters parameters)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

			var builder = new SqlConnectionStringBuilder
			{
				DataSource = parameters.SqlInstanceName,
				IntegratedSecurity = false,
				UserID = parameters.SqlAdminUserName,
				Password = parameters.SqlAdminPassword,
				InitialCatalog = string.Empty,
			};

			return builder;
		}

		public static SqlConnectionStringBuilder GetSqlConnectionStringBuilder()
		{
			return GetSqlConnectionStringBuilder(IntegrationTestParameters);
		}

		/// <summary>
		/// Destroy the integration test environment that was previously created.
		/// </summary>
		/// <param name="parameters">
		/// The integration test parameters.
		/// </param>
		public static void Destroy(IntegrationTestParameters parameters)
		{
			// This can be null when the Setup fails.
			if (parameters == null)
			{
				return;
			}

			if (parameters.SkipIntegrationTests)
			{
				Logger.LogInformation("Skipping test workspace teardown.");
				return;
			}

			if (parameters.DeleteWorkspaceAfterTest)
			{
				DeleteTestWorkspace(parameters);
			}
			else
			{
				Logger.LogInformation("Skipped deleting the workspace {workspaceId}.", parameters.WorkspaceId);
			}
		}

		public static void SetupLogger(IntegrationTestParameters parameters)
		{
			parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));

			var loggerOptions = new LoggerOptions
			{
				Application = "8A1A6418-29B3-4067-8C9E-51E296F959DE",
				ConfigurationFileLocation =
					Path.Combine(ResourceFileHelper.GetBasePath(), "LogConfig.xml"),
				System = "Import-API",
				SubSystem = "Samples",
			};

			// Configure the optional SEQ sink to periodically send logs to the local SEQ server for improved debugging.
			// See https://getseq.net/ for more details.
			loggerOptions.AddSinkParameter(
				Relativity.Logging.Configuration.SeqSinkConfig.ServerUrlSinkParameterKey,
				new Uri("http://localhost:5341"));

			// Configure the optional HTTP sink to periodically send logs to Relativity.
			loggerOptions.AddSinkParameter(
				Relativity.Logging.Configuration.RelativityHttpSinkConfig.CredentialSinkParameterKey,
				new NetworkCredential(parameters.RelativityUserName, parameters.RelativityPassword));
			loggerOptions.AddSinkParameter(
				Relativity.Logging.Configuration.RelativityHttpSinkConfig.InstanceUrlSinkParameterKey,
				parameters.RelativityUrl);
			ILog logger = Relativity.Logging.Factory.LogFactory.GetLogger(loggerOptions);

			if (parameters.WriteLogsToConsole)
			{
				// Note: Wrapping the ILog instance to ensure all logs written via tests are dumped to the console.
				logger = new RelativityTestLogger(logger);
			}

			// Until Import API supports passing a logger instance via constructor, the API
			// internally uses the Logger singleton instance if defined.
			Log.Logger = logger;

			Logger = logger;
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Design",
			"CA1031:DoNotCatchGeneralExceptionTypes",
			Justification = "We can swallow exception in that case.")]
		public static void DropWorkspaceDatabase(IntegrationTestParameters parameters, int workspaceToRemoveId, Relativity.Logging.ILog logger)
		{
			parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
			logger = logger ?? throw new ArgumentNullException(nameof(logger));

			if (workspaceToRemoveId <= 0)
			{
				Logger.LogWarning("Skipped dropping the SQL workspace database, because Id in not valid: {workspaceId}.", workspaceToRemoveId);
				return;
			}

			string databaseName = $"EDDS{workspaceToRemoveId}";

			try
			{
				SqlConnectionStringBuilder builder = GetSqlConnectionStringBuilder(parameters);
				SqlConnection.ClearAllPools();
				using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
				{
					connection.Open();
					using (SqlCommand command = connection.CreateCommand())
					{
						command.CommandText = $@"
IF EXISTS(SELECT name FROM sys.databases WHERE name = '{databaseName}')
BEGIN
	ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
	DROP DATABASE [{databaseName}]
END";
						command.CommandType = CommandType.Text;
						command.ExecuteNonQuery();
						logger.LogInformation(
							"Successfully dropped the {DatabaseName} SQL workspace database.",
							databaseName);
					}
				}
			}
			catch (Exception e)
			{
				logger.LogError(e, "Failed to drop the {DatabaseName} SQL workspace database.", databaseName);
			}
		}

		public static void DeleteTestWorkspace(IntegrationTestParameters parameters, int workspaceToRemoveId)
		{
			parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));

			WorkspaceHelper.DeleteTestWorkspace(parameters, workspaceToRemoveId, Logger);

			if (parameters.SqlDropWorkspaceDatabase)
			{
				DropWorkspaceDatabase(parameters, workspaceToRemoveId, Logger);
			}
			else
			{
				Logger.LogInformation("Skipped dropping the SQL workspace database for workspace {workspaceId}.", parameters.WorkspaceId);
			}
		}

		private static void DeleteTestWorkspace(IntegrationTestParameters parameters)
		{
			DeleteTestWorkspace(parameters, parameters.WorkspaceId);
		}

		private static string GetConfigurationStringValue(string key)
		{
			string envVariable = $"IAPI_INTEGRATION_{key.ToUpperInvariant()}";
			string value = GetEnvironmentVariable(envVariable);
			if (!string.IsNullOrWhiteSpace(value))
			{
				return value;
			}

			value = System.Configuration.ConfigurationManager.AppSettings.Get(key);
			if (!string.IsNullOrEmpty(value))
			{
				return value;
			}

			throw new InvalidOperationException(
				$"The '{key}' app.config setting or '{envVariable}' environment variable is not specified.");
		}

		private static string GetEnvironmentVariable(string envVariable)
		{
			if (EnvironmentVariablesEnabled)
			{
				// Note: these targets are intentionally ordered to favor process vars!
				IEnumerable<EnvironmentVariableTarget> targets = new[]
				{
					EnvironmentVariableTarget.Process,
					EnvironmentVariableTarget.User,
					EnvironmentVariableTarget.Machine,
				};
				foreach (EnvironmentVariableTarget target in targets)
				{
					string envValue = Environment.GetEnvironmentVariable(envVariable, target);
					if (!string.IsNullOrEmpty(envValue))
					{
						return envValue;
					}
				}
			}

			return string.Empty;
		}

		private static void SetupServerCertificateValidation(IntegrationTestParameters parameters)
		{
			if (!parameters.ServerCertificateValidation)
			{
				ServicePointManager.ServerCertificateValidationCallback +=
					(sender, certificate, chain, sslPolicyErrors) => true;
			}
		}

		private static IntegrationTestParameters ReadAndLogIntegrationTestParameters()
		{
			Console.WriteLine("Retrieving and dumping all integration test parameters...");

			IntegrationTestParameters parameters = ReadIntegrationTestParameters();
			LogIntegrationTestParameters(parameters);

			Console.WriteLine("Retrieved and dumped all integration test parameters.");
			return parameters;
		}

		private static IntegrationTestParameters ReadIntegrationTestParameters()
		{
			string testEnvironment = GetEnvironmentVariable("IAPI_INTEGRATION_TEST_ENV");
			if (!string.IsNullOrWhiteSpace(testEnvironment))
			{
				return ReadIntegrationTestParametersFromEnvironmentConfiguration(testEnvironment);
			}

			string jsonFile = GetEnvironmentVariable("IAPI_INTEGRATION_TEST_JSON_FILE");
			if (!string.IsNullOrWhiteSpace(jsonFile))
			{
				return JsonConvert.DeserializeObject<IntegrationTestParameters>(File.ReadAllText(jsonFile));
			}

			return ReadIntegrationTestParametersFromRegistryAndAppConfig();
		}

		private static IntegrationTestParameters ReadIntegrationTestParametersFromEnvironmentConfiguration(string testEnvironment)
		{
			string resourceFile;
			switch (testEnvironment.ToUpperInvariant())
			{
				case "HOPPER":
					resourceFile = "test-parameters-hopper.json";
					break;

				default:
					throw new InvalidOperationException(
						$"The test environment '{testEnvironment}' is not recognized or supported.");
			}

			using (Stream stream = ResourceFileHelper.ExtractToStream(
				Assembly.GetExecutingAssembly(),
				$"Relativity.DataExchange.TestFramework.Resources.{resourceFile}"))
			{
				StreamReader reader = new StreamReader(stream);
				JsonSerializer serializer = new JsonSerializer();
				return serializer.Deserialize<IntegrationTestParameters>(new JsonTextReader(reader));
			}
		}

		private static IntegrationTestParameters ReadIntegrationTestParametersFromRegistryAndAppConfig()
		{
			IntegrationTestParameters parameters = new IntegrationTestParameters();
			foreach (var prop in parameters.GetType().GetProperties())
			{
				IntegrationTestParameterAttribute attribute =
					prop.GetCustomAttribute<IntegrationTestParameterAttribute>();
				if (attribute == null || !attribute.IsMapped)
				{
					continue;
				}

				string value = GetConfigurationStringValue(prop.Name);
				if (prop.PropertyType == typeof(string))
				{
					prop.SetValue(parameters, value);
				}
				else if (prop.PropertyType == typeof(bool))
				{
					prop.SetValue(parameters, bool.Parse(value));
				}
				else if (prop.PropertyType == typeof(Uri))
				{
					prop.SetValue(parameters, new Uri(value));
				}
				else
				{
					string message =
						$"The integration test parameter '{prop.Name}' of type '{prop.PropertyType}' isn't supported by the integration test helper.";
					throw new ConfigurationErrorsException(message);
				}
			}

			return parameters;
		}

		private static void LogIntegrationTestParameters(IntegrationTestParameters parameters)
		{
			foreach (var prop in parameters.GetType().GetProperties())
			{
				IntegrationTestParameterAttribute attribute = prop.GetCustomAttribute<IntegrationTestParameterAttribute>();
				if (attribute == null || !attribute.IsMapped)
				{
					continue;
				}

				Console.WriteLine("{0}={1}", prop.Name, prop.GetValue(parameters, null));
			}
		}
	}
}