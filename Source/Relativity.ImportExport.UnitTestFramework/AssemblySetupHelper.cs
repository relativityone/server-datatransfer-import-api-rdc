// ----------------------------------------------------------------------------
// <copyright file="AssemblySetupHelper.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.ImportExport.UnitTestFramework
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Diagnostics;
	using System.IO;
	using System.Net;
	using System.Reflection;

	using kCura.WinEDDS.TApi;

	using Relativity.Transfer;

	/// <summary>
	/// Represents a global assembly-wide setup routine that's guaranteed to be executed before ANY NUnit test.
	/// </summary>
	public static class AssemblySetupHelper
	{
		/// <summary>
		/// Gets the Relativity logging instance.
		/// </summary>
		/// <value>
		/// The <see cref="Relativity.Logging.ILog"/> instance.
		/// </value>
		public static Relativity.Logging.ILog Logger
		{
			get;
			private set;
		}

		/// <summary>
		/// The main setup method.
		/// </summary>
		/// <returns>
		/// The <see cref="DtxTestParameters"/> instance.
		/// </returns>
		public static DtxTestParameters Setup()
		{
			// Note: don't create the logger until all parameters have been retrieved.
			DtxTestParameters parameters = GetDtxTestParameters();
			SetupLogger(parameters);
			if (parameters.SkipIntegrationTests)
			{
				Console.WriteLine("Skipping test workspace creation.");
				return parameters;
			}

			Console.WriteLine("Creating a test workspace...");
			WorkspaceHelper.CreateTestWorkspace(parameters, Logger);
			using (ITransferLog transferLog = new RelativityTransferLog(Logger, false))
			{
				IHttpCredential credential =
					new BasicAuthenticationCredential(parameters.RelativityUserName, parameters.RelativityPassword);
				RelativityConnectionInfo connectionInfo = new RelativityConnectionInfo(
					parameters.RelativityUrl,
					credential,
					parameters.WorkspaceId);
				WorkspaceService workspaceService = new WorkspaceService(connectionInfo, transferLog);
				Workspace workspace = workspaceService.GetWorkspaceAsync().GetAwaiter().GetResult();
				parameters.FileShareUncPath = workspace.DefaultFileShareUncPath;
				Console.WriteLine($"Created {parameters.WorkspaceId} test workspace.");
				return parameters;
			}
		}

		/// <summary>
		/// The main teardown method.
		/// </summary>
		/// <param name="parameters">
		/// The data transfer test parameters used to perform the teardown.
		/// </param>
		public static void TearDown(DtxTestParameters parameters)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

			if (parameters.SkipIntegrationTests)
			{
				Console.WriteLine("Skipping test workspace teardown.");
				return;
			}

			WorkspaceHelper.DeleteTestWorkspace(parameters, Logger);
			string database = $"EDDS{parameters.WorkspaceId}";
			if (parameters.SqlDropWorkspaceDatabase && parameters.WorkspaceId > 0)
			{
				try
				{
					SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
					{
						DataSource = parameters.SqlInstanceName,
						IntegratedSecurity = false,
						UserID = parameters.SqlAdminUserName,
						Password = parameters.SqlAdminPassword,
						InitialCatalog = string.Empty
					};

					SqlConnection.ClearAllPools();
					using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
					{
						connection.Open();
						using (SqlCommand command = connection.CreateCommand())
						{
							command.CommandText = $@"
IF EXISTS(SELECT name FROM sys.databases WHERE name = '{database}')
BEGIN
	ALTER DATABASE [{database}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
	DROP DATABASE [{database}]
END";
							command.CommandType = CommandType.Text;
							command.ExecuteNonQuery();
							Logger.LogInformation("Successfully dropped the {DatabaseName} SQL workspace database.", database);
							Console.WriteLine($"Successfully dropped the {database} SQL workspace database.");
						}
					}
				}
				catch (Exception e)
				{
					Logger.LogError(e, "Failed to drop the {DatabaseName} SQL workspace database.", database);
					Console.WriteLine($"Failed to drop the {database} SQL workspace database. Exception: " + e);
				}
			}
			else
			{
				Logger.LogInformation("Skipped dropping the {DatabaseName} SQL workspace database.", database);
			}
		}

		private static DtxTestParameters GetDtxTestParameters()
		{
			Console.WriteLine("Retrieving all DTX test parameters...");
			DtxTestParameters parameters = new DtxTestParameters
				                               {
					                               RelativityUserName = GetConfigurationStringValue("RelativityUserName"),
					                               RelativityPassword = GetConfigurationStringValue("RelativityPassword"),
					                               RelativityRestUrl =
						                               new Uri(GetConfigurationStringValue("RelativityRestUrl")),
					                               RelativityServicesUrl =
						                               new Uri(GetConfigurationStringValue("RelativityServicesUrl")),
					                               RelativityUrl = new Uri(GetConfigurationStringValue("RelativityUrl")),
					                               RelativityWebApiUrl =
						                               new Uri(GetConfigurationStringValue("RelativityWebApiUrl")),
					                               SqlInstanceName = GetConfigurationStringValue("SqlInstanceName"),
					                               SqlAdminUserName = GetConfigurationStringValue("SqlAdminUserName"),
					                               SqlAdminPassword = GetConfigurationStringValue("SqlAdminPassword"),
					                               SqlDropWorkspaceDatabase =
						                               bool.Parse(GetConfigurationStringValue("SqlDropWorkspaceDatabase")),
					                               SkipAsperaModeTests =
						                               bool.Parse(GetConfigurationStringValue("SkipAsperaModeTests")),
					                               SkipDirectModeTests =
						                               bool.Parse(GetConfigurationStringValue("SkipDirectModeTests")),
					                               SkipIntegrationTests = bool.Parse(
						                               GetConfigurationStringValue("SkipIntegrationTests")),
					                               WorkspaceTemplate = GetConfigurationStringValue("WorkspaceTemplate")
				                               };
			Console.WriteLine("Retrieved all DTX test parameters.");
			Console.WriteLine("Dumping all DTX test parameters.");
			foreach (var prop in parameters.GetType().GetProperties())
			{
				DebuggerBrowsableAttribute attribute = prop.GetCustomAttribute<DebuggerBrowsableAttribute>();
				if (attribute != null && attribute.State == DebuggerBrowsableState.Never)
				{
					Console.WriteLine("{0}=[Obfuscated]", prop.Name);
				}
				else
				{
					Console.WriteLine("{0}={1}", prop.Name, prop.GetValue(parameters, null));
				}
			}

			Console.WriteLine("Dumped all DTX test parameters.");
			return parameters;
		}

		private static string GetConfigurationStringValue(string key)
		{
			string envVariable = $"IAPI_INTEGRATION_{key.ToUpperInvariant()}";

			// Note: these targets are intentionally ordered to favor process vars!
			IEnumerable<EnvironmentVariableTarget> targets = new[]
				{
				 EnvironmentVariableTarget.Process,
				 EnvironmentVariableTarget.User,
				 EnvironmentVariableTarget.Machine
				};

			foreach (EnvironmentVariableTarget target in targets)
			{
				string envValue = Environment.GetEnvironmentVariable(envVariable, target);
				if (!string.IsNullOrEmpty(envValue))
				{
					return envValue;
				}
			}

			string value = System.Configuration.ConfigurationManager.AppSettings.Get(key);
			if (!string.IsNullOrEmpty(value))
			{
				return value;
			}

			throw new InvalidOperationException($"The '{key}' app.config setting or '{envVariable}' environment variable is not specified.");
		}

		private static void SetupLogger(DtxTestParameters parameters)
		{
			Logging.LoggerOptions loggerOptions = new Logging.LoggerOptions
			{
				Application = "8A1A6418-29B3-4067-8C9E-51E296F959DE",
				ConfigurationFileLocation = Path.Combine(ResourceFileHelper.GetBasePath(), "LogConfig.xml"),
				System = "Import-API",
				SubSystem = "Samples"
			};

			// Configure the optional SEQ sink to periodically send logs to the local SEQ server for improved debugging.
			// See https://getseq.net/ for more details.
			loggerOptions.AddSinkParameter(
				Logging.Configuration.SeqSinkConfig.ServerUrlSinkParameterKey,
				new Uri("http://localhost:5341"));

			// Configure the optional HTTP sink to periodically send logs to Relativity.
			loggerOptions.AddSinkParameter(
				Logging.Configuration.RelativityHttpSinkConfig.CredentialSinkParameterKey,
				new NetworkCredential(parameters.RelativityUserName, parameters.RelativityPassword));
			loggerOptions.AddSinkParameter(
				Logging.Configuration.RelativityHttpSinkConfig.InstanceUrlSinkParameterKey,
				parameters.RelativityUrl);
			Logger = Logging.Factory.LogFactory.GetLogger(loggerOptions);

			// Until Import API supports passing a logger instance via constructor, the API
			// internally uses the Logger singleton instance if defined.
			Relativity.Logging.Log.Logger = Logger;
		}
	}
}