using System;
using kCura.Relativity.ImportAPI.IntegrationTests.Helpers;
using Newtonsoft.Json;

namespace kCura.Relativity.ImportAPI.IntegrationTests.Services
{
	internal class ProductionService
	{
		private const string _PRODUCTION_SERVICE_URL_BASE =
			"api/Relativity.Productions.Services.IProductionModule/Production%20Manager/";

		private const string _CREATE_PRODUCTION_SERVICE =
			_PRODUCTION_SERVICE_URL_BASE + "CreateSingleAsync";

		private const string _READ_PRODUCTION_SERVICE =
			_PRODUCTION_SERVICE_URL_BASE + "/ReadSingleAsync";

		public static int Create(int workspaceId, string productionName)
		{
#pragma warning disable RG2007 // Explicit Type Declaration
			var json =
				$@"
				{{
					""workspaceArtifactID"": {workspaceId},
					""Production"": {{
						""Details"": {{
							""BrandingFontSize"": 10,
							""ScaleBrandingFont"": false
						}},
						""Numbering"": {{
								""NumberingType"": ""DocumentField"",
								""NumberingField"":{{  
										 ""ArtifactID"":1003667,
										 ""ViewFieldID"":0,
										 ""Name"":""Control Number""
								}},
								""AttachmentRelationalField"": {{
										""ArtifactID"": 0,
										""ViewFieldID"": 0,
										""Name"": """"
								}},							  
								""BatesPrefix"": """",
								""BatesSuffix"": """",								
								""BatesStartNumber"": 1,
								""IncludePageNumbers"":false,
								""DocumentNumberPageNumberSeparator"":"""",
								""NumberOfDigitsForPageNumbering"":0,
								""NumberOfDigitsForDocumentNumbering"": 7,
								""StartNumberingOnSecondPage"":false
						}},								
						""ShouldCopyInstanceOnWorkspaceCreate"": false,
						""Name"": ""{productionName}""
					}}
				}}";
#pragma warning restore RG2007 // Explicit Type Declaration

			string output = Rest.PostRequestAsJson(_CREATE_PRODUCTION_SERVICE, json);
			return int.Parse(output);
		}

		/// <summary>
		/// Retrieves bates numbers form production kepler service
		/// </summary>
		/// <param name="workspaceId"></param>
		/// <param name="productionId"></param>
		/// <returns>First item is FirstBatesValue, second item is LastBatesValue</returns>
		public static Tuple<string, string> GetProductionBatesNumbers(int workspaceId, int productionId)
		{
#pragma warning disable RG2007 // Explicit Type Declaration
			var json =
				$@"
				{{
					""workspaceArtifactID"": {workspaceId},
					""productionArtifactID"": {productionId},
					""dataSourceReadMode"": ""OnlyDataSources""
				}}";
#pragma warning restore RG2007 // Explicit Type Declaration

			string productionDtoJson = Rest.PostRequestAsJson(_READ_PRODUCTION_SERVICE, json);
			string firstBates = ExtractValueFromProductionDetails(productionDtoJson, "FirstBatesValue");
			string lastBates = ExtractValueFromProductionDetails(productionDtoJson, "LastBatesValue");
			return new Tuple<string, string>(firstBates, lastBates);
		}

		private static string ExtractValueFromProductionDetails(string productionDtoJson, string key)
		{
			dynamic productionObject = JsonConvert.DeserializeObject<dynamic>(productionDtoJson);
			dynamic productionDetails = productionObject["Details"];
			dynamic value = productionDetails[key];
			return value.ToString();
		}
	}
}
