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

	using Relativity.DataExchange.TestFramework.RelativityVersions;
	using Relativity.Productions.Services;

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

		public static async Task DeleteAllProductionsAsync(IntegrationTestParameters parameters)
		{
			parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));

			using (IProductionManager client =
				ServiceHelper.GetServiceProxy<IProductionManager>(parameters))
			{
				List<Production> productions = await client.GetAllAsync(parameters.WorkspaceId, DataSourceReadMode.None).ConfigureAwait(false);
				var deleteProductionsTasks = productions.Select(production => client.DeleteSingleAsync(parameters.WorkspaceId, production.ArtifactID));
				await Task.WhenAll(deleteProductionsTasks).ConfigureAwait(false);
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