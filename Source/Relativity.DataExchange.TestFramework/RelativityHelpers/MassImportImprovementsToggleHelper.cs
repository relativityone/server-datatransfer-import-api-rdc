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
		public static void SkipTestIfMassImportImprovementsToggleHasValue(IntegrationTestParameters parameters, bool isEnabled)
		{
			if (!TryGetMassImportImprovementsToggle(parameters, out var toggle))
			{
				Assert.Ignore(TestStrings.SkipTestMessage, "Could not determine toggle value for this instance");
			}
			else if (toggle == isEnabled)
			{
				string toggleValue = isEnabled ? "enabled" : "disabled";
				Assert.Ignore(TestStrings.SkipTestMessage, $"MassImportImprovementToggle is {toggleValue}");
			}
		}

		public static bool TryGetMassImportImprovementsToggle(IntegrationTestParameters parameters, out bool toggleValue)
		{
			toggleValue = false;

			try
			{
				// Hoppers and TestVMs - toggle set in database
				toggleValue = GetMassImportToggleValueFromDatabase(parameters);
				return true;
			}
			catch (SqlException)
			{
				// Regression environments - no access to SQL
				toggleValue = GetMassImportToggleValueAsync(parameters).GetAwaiter().GetResult();
				return true;
			}
			catch (NullReferenceException)
			{
				// Hoppers with default toggle value
				return TryGetMassImportImprovementsToggleFromRelativityVersion(parameters, out toggleValue);
			}
		}

		public static string GetDisplayableMassImportImprovementsToggle(IntegrationTestParameters parameters)
		{
			return TryGetMassImportImprovementsToggle(
				parameters, out var toggle)
				? toggle.ToString()
				: "Unknown";
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

		private static bool TryGetMassImportImprovementsToggleFromRelativityVersion(
			IntegrationTestParameters parameters,
			out bool toggleValue)
		{
			toggleValue = false;

			if (RelativityVersions.RelativityVersionChecker.VersionIsLowerThan(
				parameters,
				RelativityVersions.RelativityVersion.Lanceleaf))
			{
				toggleValue = false;
				return true;
			}

			if (RelativityVersions.RelativityVersionChecker.VersionIsLowerThan(
				parameters,
				RelativityVersions.RelativityVersion.Mayapple))
			{
				// for Lanceleaf release, we have been changing value of an toggle many times, so it is hard to determine its value.
				return false;
			}

			if (RelativityVersions.RelativityVersionChecker.VersionIsLowerThan(
				parameters,
				RelativityVersions.RelativityVersion.MayappleToggleOff))
			{
				toggleValue = true; // toggle is enabled in [RelativityVersion.Mayapple, RelativityVersion.MayappleToggleOff)
				return true;
			}

			if (RelativityVersions.RelativityVersionChecker.VersionIsLowerThan(
				parameters,
				RelativityVersions.RelativityVersion.Ninebark))
			{
				toggleValue = false; // toggle is disabled since RelativityVersion.MayappleToggleOff on the mayapple branch.
				return true;
			}

			toggleValue = true; // toggle is enabled since Ninebark release
			return true;
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
