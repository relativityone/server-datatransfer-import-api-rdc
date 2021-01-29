// <copyright file="NonDefaultCollationWorkspaceHelper.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.RelativityHelpers
{
	using System;
	using System.Collections.Generic;
	using System.Data.SqlClient;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.Text;
	using System.Threading;
	using System.Threading.Tasks;

	using Microsoft.SqlServer.Management.Common;
	using Microsoft.SqlServer.Management.Sdk.Sfc;
	using Microsoft.SqlServer.Management.Smo;

	/// <summary>
	/// Defines helper method to create workspace with non-default collation.
	/// Implementation is based on: https://einstein.kcura.com/x/coglD.
	/// </summary>
	internal class NonDefaultCollationWorkspaceHelper
	{
		// Tests on pipelines can be executed in parallel
		// Mutex is required to process synchronization and prevent creation of workspace template by more than one test in the same time
		private static readonly Mutex CollationTemplateMutex = new Mutex(false, "API_Tests_CollationTemplateMutex");

		public static async Task<string> GetOrCreateWorkspaceTemplateWithNonDefaultCollationAsync(IntegrationTestParameters parameters, Relativity.Logging.ILog logger)
		{
			const string NonDefaultCollationTemplateName = "Import API Collation Template";

			if (CollationTemplateMutex.WaitOne(TimeSpan.FromMinutes(30)))
			{
				try
				{
					// create workspace template if it does not exist
					await WorkspaceHelper.RetrieveWorkspaceIdAsync(parameters, logger, NonDefaultCollationTemplateName).ConfigureAwait(false);
				}
				catch (InvalidOperationException)
				{
					await CreateWorkspaceTemplateWithNonDefaultCollationAsync(parameters, logger, NonDefaultCollationTemplateName).ConfigureAwait(false);
				}
				finally
				{
					CollationTemplateMutex.ReleaseMutex();
				}
			}
			else
			{
				throw new TimeoutException("Failed on get or create workspace template with non default collation, seems like it's doing by another process longer than 30 minutes");
			}

			return NonDefaultCollationTemplateName;
		}

		private static async Task CreateWorkspaceTemplateWithNonDefaultCollationAsync(IntegrationTestParameters parameters, Relativity.Logging.ILog logger, string nonDefaultCollationTemplateName)
		{
			const string DefaultCollation = "SQL_Latin1_General_CP1_CI_AS";
			const string NonDefaultCollation = "Latin1_General_CI_AI";

			string nonDefaultCollationTemplateNameDuringCreation = $"{nonDefaultCollationTemplateName} Creation...";

			await WorkspaceHelper
				.CreateWorkspaceFromTemplateAsync(
					parameters,
					logger,
					parameters.WorkspaceTemplate,
					nonDefaultCollationTemplateNameDuringCreation)
				.ConfigureAwait(false);
			string workspaceDatabaseBackupScript = await GenerateSqlScriptOfWorkspaceDatabaseAsync(
															parameters,
															logger,
															nonDefaultCollationTemplateNameDuringCreation)
														.ConfigureAwait(false);
			IntegrationTestHelper.DropWorkspaceDatabase(parameters, parameters.WorkspaceId, logger);

			ReplaceCollationInDatabaseScript(workspaceDatabaseBackupScript, DefaultCollation, NonDefaultCollation);
			RestoreDatabaseFromScript(parameters, workspaceDatabaseBackupScript);
			ChangeColumnNameCollationInArtifactViewFieldTable(parameters);

			WorkspaceHelper.RenameTestWorkspace(parameters, parameters.WorkspaceId, nonDefaultCollationTemplateName);
		}

		private static async Task<string> GenerateSqlScriptOfWorkspaceDatabaseAsync(IntegrationTestParameters parameters, Relativity.Logging.ILog logger, string workspaceName)
		{
			int workspaceId = await WorkspaceHelper.RetrieveWorkspaceIdAsync(parameters, logger, workspaceName).ConfigureAwait(false);

			string databaseScriptFilePath = GetDatabaseScriptFilePath(workspaceName);
			Scripter scripter = CreateScripter(parameters, databaseScriptFilePath);
			Database database = GetDatabaseByName(parameters, $"EDDS{workspaceId}");

			ScriptDatabase(database, scripter);

			string[] useDatabaseStatement = { $"USE {database.Name};", "GO" };
			File.AppendAllLines(databaseScriptFilePath, useDatabaseStatement);

			ScriptSchemasAndUsers(database, scripter);

			string[] executeAsUserStatement = { "EXECUTE AS USER = 'EDDSDBO';", "GO" };
			File.AppendAllLines(databaseScriptFilePath, executeAsUserStatement);

			ScriptOtherObjects(database, scripter);

			string[] revertExecuteAsUserStatement = { "REVERT" };
			File.AppendAllLines(databaseScriptFilePath, revertExecuteAsUserStatement);

			return databaseScriptFilePath;
		}

		private static void ReplaceCollationInDatabaseScript(string sqlDatabaseFile, string originalCollation, string newCollation)
		{
			File.WriteAllText(sqlDatabaseFile, File.ReadAllText(sqlDatabaseFile).Replace(originalCollation, newCollation));
		}

		/// <summary>
		/// In 'zTemplate DLA Collation Primary' template, ArtifactViewField.ColumnName has different collation than a database.
		/// </summary>
		private static void ChangeColumnNameCollationInArtifactViewFieldTable(IntegrationTestParameters parameters)
		{
			SqlConnectionStringBuilder builder = IntegrationTestHelper.GetSqlConnectionStringBuilder(parameters);
			builder.InitialCatalog = $"EDDS{parameters.WorkspaceId}";
			using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
			{
				connection.Open();
				using (SqlCommand command = connection.CreateCommand())
				{
					command.CommandText = @"ALTER TABLE EDDSDBO.ArtifactViewField ALTER COLUMN ColumnName varchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS not null";
					command.ExecuteNonQuery();
				}
			}
		}

		private static void RestoreDatabaseFromScript(IntegrationTestParameters parameters, string filePath)
		{
			FileInfo fileInfo = new FileInfo(filePath);
			string script;
			using (var reader = fileInfo.OpenText())
			{
				script = reader.ReadToEnd();
			}

			SqlConnectionStringBuilder builder = IntegrationTestHelper.GetSqlConnectionStringBuilder(parameters);

			using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
			{
				connection.Open();
				Server server = new Server(new ServerConnection(connection));
				server.ConnectionContext.ExecuteNonQuery(script);
			}
		}

		private static Database GetDatabaseByName(IntegrationTestParameters parameters, string databaseName)
		{
			Server server = new Server(new ServerConnection(parameters.SqlInstanceName, parameters.SqlAdminUserName, parameters.SqlAdminPassword));
			DatabaseCollection databasesCollection = server.Databases;
			return databasesCollection.Cast<Database>().FirstOrDefault(x => x.Name == databaseName);
		}

		private static string GetDatabaseScriptFilePath(string workspaceName)
		{
			string parentFolder = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.Parent.Parent.Parent.FullName;
			string reportFolder = Path.Combine(parentFolder, "TestReports", "integration-tests");
			string databaseScriptFilePath = Path.Combine(reportFolder, $"{workspaceName}.sql");
			Directory.CreateDirectory(reportFolder);
			File.Delete(databaseScriptFilePath);

			return databaseScriptFilePath;
		}

		private static Scripter CreateScripter(IntegrationTestParameters parameters, string databaseScriptFilePath)
		{
			Server server = new Server(new ServerConnection(parameters.SqlInstanceName, parameters.SqlAdminUserName, parameters.SqlAdminPassword));
			Scripter scripter = new Scripter(server);
			scripter.PrefetchObjects = true;

			scripter.Options.ToFileOnly = true;
			scripter.Options.FileName = databaseScriptFilePath;

			scripter.Options.NoCollation = false;
			scripter.Options.Triggers = true;
			scripter.Options.ExtendedProperties = true;
			scripter.Options.IncludeDatabaseContext = true;
			scripter.Options.ScriptData = true;
			scripter.Options.ScriptSchema = true;
			scripter.Options.Statistics = false;
			scripter.Options.IncludeHeaders = true;
			scripter.Options.AppendToFile = true;
			scripter.Options.Encoding = Encoding.UTF8;
			scripter.Options.IncludeDatabaseRoleMemberships = true;

			scripter.Options.Indexes = true;
			scripter.Options.ClusteredIndexes = true;
			scripter.Options.NonClusteredIndexes = true;
			scripter.Options.FullTextIndexes = true;
			scripter.Options.XmlIndexes = true;
			scripter.Options.NoIndexPartitioningSchemes = true;

			scripter.Options.DriAll = true;
			scripter.Options.DriIndexes = true;
			scripter.Options.DriClustered = true;
			scripter.Options.DriNonClustered = true;

			scripter.Options.DriAllConstraints = true;

			scripter.Options.Permissions = true;

			return scripter;
		}

		private static void ScriptDatabase(Database database, Scripter scripter)
		{
			scripter.EnumScript(new[] { database.Urn });
		}

		private static void ScriptSchemasAndUsers(Database database, Scripter scripter)
		{
			var urns = new List<Urn>();
			foreach (Schema schema in database.Schemas)
			{
				if (!schema.IsSystemObject)
				{
					urns.Add(schema.Urn);
				}
			}

			scripter.EnumScript(urns.ToArray());
		}

		private static void ScriptOtherObjects(Database database, Scripter scripter)
		{
			var urns = new List<Urn>();
			foreach (Table table in database.Tables)
			{
				if (!table.IsSystemObject)
				{
					urns.Add(table.Urn);
				}

				foreach (Index index in table.Indexes)
				{
					urns.Add(index.Urn);
				}

				foreach (Column column in table.Columns)
				{
					if (column.DefaultConstraint != null)
					{
						urns.Add(column.DefaultConstraint.Urn);
					}
				}
			}

			foreach (View view in database.Views)
			{
				if (!view.IsSystemObject)
				{
					urns.Add(view.Urn);
				}
			}

			foreach (StoredProcedure storedProcedure in database.StoredProcedures)
			{
				if (!storedProcedure.IsSystemObject)
				{
					urns.Add(storedProcedure.Urn);
				}
			}

			foreach (UserDefinedTableType userDefinedTableType in database.UserDefinedTableTypes)
			{
				urns.Add(userDefinedTableType.Urn);
			}

			foreach (UserDefinedFunction userDefinedFunction in database.UserDefinedFunctions)
			{
				if (!userDefinedFunction.IsSystemObject)
				{
					urns.Add(userDefinedFunction.Urn);
				}
			}

			foreach (FullTextCatalog fullTextCatalog in database.FullTextCatalogs)
			{
				urns.Add(fullTextCatalog.Urn);
			}

			scripter.EnumScript(urns.ToArray());
		}
	}
}