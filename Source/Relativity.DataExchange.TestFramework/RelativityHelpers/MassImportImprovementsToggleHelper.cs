// <copyright file="MassImportImprovementsToggleHelper.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.RelativityHelpers
{
	using System;
	using System.Data.SqlClient;
	using System.Threading.Tasks;
	using NUnit.Framework;

	public static class MassImportImprovementsToggleHelper
	{
		public static void SkipTestIfMassImportImprovementsToggleOff(IntegrationTestParameters parameters)
		{
			if (!GetMassImportImprovementsToggle(parameters))
			{
				Assert.Ignore(TestStrings.SkipTestMessage, $"MassImportImprovementToggle Off");
			}
		}

		public static bool GetMassImportImprovementsToggle(IntegrationTestParameters parameters)
		{
			bool massImportToggle;

			try
			{
				massImportToggle = GetMassImportToggleValueFromDatabase(parameters);
			}
			catch (SqlException)
			{
				massImportToggle = GetMassImportToggleValueAsync(parameters).GetAwaiter().GetResult();
			}

			return massImportToggle;
		}

		public static void SetMassImportImprovementsToggle(IntegrationTestParameters parameters, bool toggleValue)
		{
			AddMassImportImprovementsToggle(parameters);

			SqlConnectionStringBuilder builder = IntegrationTestHelper.GetSqlConnectionStringBuilder(parameters);

			using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
			{
				connection.Open();
				using (SqlCommand command = connection.CreateCommand())
				{
					command.CommandText = $@"Update [EDDS].[eddsdbo].[Toggle]
							set [EDDS].[eddsdbo].[Toggle].[IsEnabled]= '{toggleValue}'
							where [EDDS].[eddsdbo].[Toggle].[Name] = 'Relativity.Core.Toggle.MassImportImprovementsToggle'";
					command.ExecuteScalar();
				}
			}
		}

		private static bool GetMassImportToggleValueFromDatabase(IntegrationTestParameters parameters)
		{
			// SQL query is used instead of call to Kepler service because of missing testmode enabled on hopper images used on pipeline
			// Relativity.Rest and ServiceHost need to have enabled TestMode
			// When it's not enable using call Kepler finish with error: Relativity.Kepler.Exceptions.ServiceNotFoundException : The service endpoint could not be found
			SqlConnectionStringBuilder builder = IntegrationTestHelper.GetSqlConnectionStringBuilder(parameters);

			using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
			{
				connection.Open();
				using (SqlCommand command = connection.CreateCommand())
				{
					command.CommandText = @"SELECT IsEnabled FROM [EDDS].[eddsdbo].[Toggle] WHERE [Name] = 'Relativity.Core.Toggle.MassImportImprovementsToggle'";
					var result = (bool)command.ExecuteScalar();
					Console.WriteLine($"Mass import improvements toggle value {result.ToString()}");
					return result;
				}
			}
		}

		private static async Task<bool> GetMassImportToggleValueAsync(IntegrationTestParameters parameters)
		{
			using (var toggleService = ServiceHelper.GetServiceProxy<Services.Environmental.IToggleService>(parameters))
			{
				return await toggleService.IsEnabledAsync("Relativity.Core.Toggle.MassImportImprovementsToggle, Relativity.Data").ConfigureAwait(false);
			}
		}

		private static void AddMassImportImprovementsToggle(IntegrationTestParameters parameters)
		{
			SqlConnectionStringBuilder builder = IntegrationTestHelper.GetSqlConnectionStringBuilder(parameters);

			using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
			{
				connection.Open();
				using (SqlCommand command = connection.CreateCommand())
				{
					command.CommandText = @"INSERT INTO [EDDS].[eddsdbo].[Toggle]([Name], [IsEnabled])
								Select 'Relativity.Core.Toggle.MassImportImprovementsToggle', 'False'
							WHERE
							NOT EXISTS (SELECT * FROM [EDDS].[eddsdbo].[Toggle]
									  WHERE [Name] = 'Relativity.Core.Toggle.MassImportImprovementsToggle')";
					command.ExecuteScalar();
				}
			}
		}
	}
}
