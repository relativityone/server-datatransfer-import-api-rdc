// <copyright file="SearchHelper.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.RelativityHelpers
{
	using System.Collections.Generic;
	using System.Threading.Tasks;

	using Relativity.Services.Field;
	using Relativity.Services.Search;

	public static class SearchHelper
	{
		public static async Task<int> CreateSavedSearchWithSingleDocument(IntegrationTestParameters parameters, string searchName, string documentIdentifier)
		{
			using (var kepler = ServiceHelper.GetServiceProxy<IKeywordSearchManager>(parameters))
			{
				var search = new KeywordSearch
				{
					Name = searchName,
					ArtifactTypeID = (int)ArtifactType.Document,
					Fields = new List<FieldRef> { new FieldRef { Name = WellKnownFields.ControlNumber } },
					SearchCriteria = new CriteriaCollection
					{
						Conditions = new List<CriteriaBase>
						{
							new Criteria
							{
							 Condition =
							  new CriteriaCondition
							   {
								   FieldIdentifier = new Services.Field.FieldRef(WellKnownFields.ControlNumber),
								   NotOperator = false,
								   Operator = CriteriaConditionEnum.Is,
								   Value = documentIdentifier,
							   },
							 BooleanOperator = BooleanOperatorEnum.And,
							 HasPermission = true,
							},
						},
					},
				};
				return await kepler.CreateSingleAsync(parameters.WorkspaceId, search).ConfigureAwait(false);
			}
		}
	}
}
