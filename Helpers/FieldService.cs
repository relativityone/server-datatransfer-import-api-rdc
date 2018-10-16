using System.Collections.Generic;
using FluentAssertions;
using kCura.NUnit.Integration;
using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using Platform.Keywords.Connection;
using Relativity.Services.Objects;
using Relativity.Services.Objects.DataContracts;
using QueryResult = Relativity.Services.Objects.DataContracts.QueryResult;

namespace kCura.Relativity.ImportAPI.IntegrationTests.Helpers
{
	internal class FieldService
	{
		public static string GetIdentifierFieldName(int workspaceId)
		{
			using (var client = ServiceFactory.GetProxy<IObjectManager>(SharedTestVariables.ADMIN_USERNAME,
				SharedTestVariables.DEFAULT_PASSWORD))
			{
				var queryRequest = new QueryRequest
				{
					Condition = $"'{ArtifactTypeNames.ObjectType}' == '{ArtifactTypeNames.Document}' AND '{FieldFieldNames.IsIdentifier}' == true",
					Fields = new List<FieldRef>
					{
						new FieldRef { Name = FieldFieldNames.Name },
					},
					ObjectType = new ObjectTypeRef { ArtifactTypeID = (int)ArtifactType.Field }
				};
				const int maxItemsToFetch = 2;
				QueryResult result = client.QueryAsync(workspaceId, queryRequest, 1, maxItemsToFetch).GetAwaiter().GetResult();
				result.TotalCount.Should().Be(1);
				return (string)result.Objects[0].FieldValues[0].Value;
			}
		}
	}
}
