// <copyright file="MassImportImprovementsToggleChecker.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework
{
	using System;
	using System.Data.SqlClient;
	using System.Threading.Tasks;
	using NUnit.Framework;

	public static class MassImportImprovementsToggleChecker
	{
		public static void SkipTestIfMassImportImprovementToggleOff(IntegrationTestParameters parameter)
		{
			bool toggleValue;

			try
			{
				toggleValue = GetMassImportToggleValueFromDatabase(parameter);
			}
			catch (SqlException)
			{
				toggleValue = GetMassImportToggleValueAsync(parameter).ConfigureAwait(false).GetAwaiter().GetResult();
			}

			if (!toggleValue)
			{
				Assert.Ignore(TestStrings.SkipTestMessage, $"MassImportImprovementToggle {toggleValue} ");
			}
		}

		public static bool GetMassImportToggleValueFromDatabase(IntegrationTestParameters parameters)
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
	}
}
