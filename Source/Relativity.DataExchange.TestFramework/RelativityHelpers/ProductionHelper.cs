// ----------------------------------------------------------------------------
// <copyright file="ProductionHelper.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework.RelativityHelpers
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Threading.Tasks;

	using Newtonsoft.Json.Linq;

	using NUnit.Framework;

	using Relativity.DataExchange.TestFramework.RelativityVersions;
	using Relativity.Productions.Services;
	using Relativity.Productions.Services.Interfaces.DTOs;
	using Relativity.Services.Search;

	/// <summary>
	/// Defines static helper methods to manage productions.
	/// </summary>
	public static class ProductionHelper
	{
		public static async Task<Production> QueryProductionAsync(IntegrationTestParameters parameters, int productionId)
		{
			parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));

			using (IProductionManager client =
				ServiceHelper.GetServiceProxy<IProductionManager>(parameters))
			{
				Production production = await client
					.ReadSingleAsync(parameters.WorkspaceId, productionId).ConfigureAwait(false);
				if (production == null)
				{
					throw new InvalidOperationException($"The production {productionId} does not exist.");
				}

				return production;
			}
		}

		/// <summary>Deletes all productions from the workspace.</summary>
		/// <param name="parameters">Test parameters.</param>
		/// <returns>A Task which completes when all productions are added to the delete queue.</returns>
		/// <remarks>Productions are deleted asynchronously be the agent. Task returned by this method
		/// completes before productions are physically removed from the database.</remarks>>
		public static async Task DeleteAllProductionsAsync(IntegrationTestParameters parameters)
		{
			parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));

			List<Production> productions;
			using (IProductionManager client = ServiceHelper.GetServiceProxy<IProductionManager>(parameters))
			{
				productions = await client.GetAllAsync(parameters.WorkspaceId, DataSourceReadMode.None).ConfigureAwait(false);
			}

			foreach (var productionId in productions.Select(x => x.ArtifactID))
			{
				await RdoHelper.DeleteObjectAsync(parameters, productionId).ConfigureAwait(false);
			}
		}

		public static async Task DeleteProductionAsync(IntegrationTestParameters parameters, int productionId)
		{
			parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));

			using (IProductionManager client =
				ServiceHelper.GetServiceProxy<IProductionManager>(parameters))
			{
				await client.DeleteSingleAsync(parameters.WorkspaceId, productionId).ConfigureAwait(false);
			}
		}

		public static async Task<int> CreateProductionAsync(
			IntegrationTestParameters parameters,
			string productionName,
			string batesPrefix)
		{
			// Starting from Goatsbeard release we can use Kepler to create production
			return RelativityVersionChecker.VersionIsLowerThan(parameters, RelativityVersion.Goatsbeard)
					? await CreateProductionUsingHttpClientAsync(parameters, productionName, batesPrefix).ConfigureAwait(false)
					: await CreateProductionUsingKeplerAsync(parameters, productionName, batesPrefix).ConfigureAwait(false);
		}

		public static async Task AddDataSourceAsync(
			IntegrationTestParameters parameters,
			int productionId,
			int savedSearchId)
		{
			using (var client = ServiceHelper.GetServiceProxy<IProductionDataSourceManager>(parameters))
			{
				var dataSource = new ProductionDataSource
				{
					Name = "From Saved search",
					SavedSearch = new SavedSearchRef(savedSearchId),
					ProductionType = ProductionType.ImagesAndNatives,
					BurnRedactions = false,
				};
				await client.CreateSingleAsync(parameters.WorkspaceId, productionId, dataSource).ConfigureAwait(false);
			}
		}

		public static async Task<bool> StageAndRunAsync(IntegrationTestParameters parameters, int productionId)
		{
			using (var client = ServiceHelper.GetServiceProxy<IProductionManager>(parameters))
			{
				var stageResult = await client.StageProductionAsync(parameters.WorkspaceId, productionId, automaticallyRun: true).ConfigureAwait(false);
				if (!stageResult.WasJobCreated || stageResult.Errors.Any())
				{
					TestContext.WriteLine($"Staging and running production failed: {string.Join(";", stageResult.Errors)}");
					return false;
				}

				await WaitForStatus(client, parameters, productionId, ProductionStatus.Produced, timeout: TimeSpan.FromMinutes(5)).ConfigureAwait(false);
				return true;
			}
		}

		private static async Task WaitForStatus(
			IProductionManager client,
			IntegrationTestParameters parameters,
			int productionId,
			ProductionStatus expectedStatus,
			TimeSpan timeout)
		{
			async Task WaitIndefinitelyForStatus()
			{
				ProductionJobStatusResult status;
				do
				{
					status = await client.GetJobStatus(parameters.WorkspaceId, productionId).ConfigureAwait(false);
				}
				while (status.Status != expectedStatus);
			}

			Task waitIndefinitelyForStatus = WaitIndefinitelyForStatus();
			var completedTask = await Task.WhenAny(waitIndefinitelyForStatus, Task.Delay(timeout)).ConfigureAwait(false);
			if (completedTask != waitIndefinitelyForStatus)
			{
				throw new TimeoutException($"Timeout occurred while waiting for '{expectedStatus}' production status.");
			}
		}

		private static async Task<int> CreateProductionUsingKeplerAsync(
			IntegrationTestParameters parameters,
			string productionName,
			string batesPrefix)
		{
			parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));

			const int ProductionFontSize = 10;
			const int NumberOfDigits = 7;
			using (IProductionManager client = ServiceHelper.GetServiceProxy<IProductionManager>(parameters))
			{
				var production = new Production
				{
					Details = new ProductionDetails
					{
						BrandingFontSize = ProductionFontSize,
						ScaleBrandingFont = false,
					},
					Name = productionName,
					Numbering = new DocumentLevelNumbering
					{
						NumberingType = NumberingType.DocumentLevel,
						BatesPrefix = batesPrefix,
						BatesStartNumber = 0,
						NumberOfDigitsForDocumentNumbering = NumberOfDigits,
						IncludePageNumbers = false,
					},
				};

				return await client.CreateSingleAsync(parameters.WorkspaceId, production).ConfigureAwait(false);
			}
		}

		private static async Task<int> CreateProductionUsingHttpClientAsync(
			IntegrationTestParameters parameters,
			string productionName,
			string batesPrefix)
		{
			parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));

			string createProductionJson = ResourceFileHelper.GetResourceFolderPath("CreateProductionInput.json");
			JObject request = JObject.Parse(File.ReadAllText(createProductionJson));
			request["workspaceArtifactID"] = parameters.WorkspaceId;
			request["Production"]["Name"] = productionName;
			request["Production"]["Numbering"]["BatesPrefix"] = batesPrefix;

			string url = $"{parameters.RelativityRestUrl.AbsoluteUri}/Relativity.Productions.Services.IProductionModule/Production%20Manager/CreateSingleAsync";
			string result = await HttpClientHelper.PostAsync(parameters, new Uri(url), request.ToString()).ConfigureAwait(false);

			return int.Parse(result);
		}
	}
}