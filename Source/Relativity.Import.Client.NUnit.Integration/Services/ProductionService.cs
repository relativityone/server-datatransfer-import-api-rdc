using System;
using kCura.NUnit.Integration;
using Platform.Keywords.Connection;
using Relativity.Productions.Services;

namespace kCura.Relativity.ImportAPI.IntegrationTests.Services
{
	internal class ProductionService
	{
		public static int Create(int workspaceId, string productionName)
		{
			const int productionFontSize = 10;
			const int numberOfDigits = 7;

			using (var client = ServiceFactory.GetProxy<IProductionManager>(SharedTestVariables.ADMIN_USERNAME, SharedTestVariables.DEFAULT_PASSWORD))
			{
				var production = new Production
				{
					Details = new ProductionDetails
					{
						BrandingFontSize = productionFontSize,
						ScaleBrandingFont = false
					},
					Name = productionName,
					Numbering = new DocumentLevelNumbering
					{
						NumberingType = NumberingType.DocumentLevel,
						BatesPrefix = "PREFIX",
						BatesStartNumber = 0,
						NumberOfDigitsForDocumentNumbering = numberOfDigits,
						IncludePageNumbers = false
					}
				};
				return client.CreateSingleAsync(workspaceId, production).ConfigureAwait(false).GetAwaiter().GetResult();
			}
		}

		/// <summary>
		/// Retrieves bates numbers form production kepler service
		/// </summary>
		/// <param name="workspaceId"></param>
		/// <param name="productionId"></param>
		/// <returns>First item is FirstBatesValue, second item is LastBatesValue</returns>
		public static Tuple<string, string> GetProductionBatesNumbers(int workspaceId, int productionId)
		{
			using (var client = ServiceFactory.GetProxy<IProductionManager>(SharedTestVariables.ADMIN_USERNAME,
				SharedTestVariables.DEFAULT_PASSWORD))
			{
				Production production = client.ReadSingleAsync(workspaceId, productionId)
					.ConfigureAwait(false).GetAwaiter().GetResult();
				return new Tuple<string, string>(production.Details.FirstBatesValue, production.Details.LastBatesValue);
			}
		}
	}
}
