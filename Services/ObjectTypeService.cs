using System.Linq;
using System.Threading.Tasks;
using kCura.NUnit.Integration;
using Platform.Keywords.Connection;
using Relativity.Services.Objects;
using Relativity.Services.Objects.DataContracts;

namespace kCura.Relativity.ImportAPI.IntegrationTests.Services
{
	internal class ObjectTypeService
	{
		private const string _OBJECT_TYPE_NAME = "Object Type";
		private const string _ARTIFACT_TYPE_ID_FIELD = "Artifact Type ID";
		private const string _NAME_FIELD = "Name";

		public static async Task<int> GetArtifactTypeId(int workspaceId, string objectTypeName)
		{
			using (var objectManager = ServiceFactory.GetProxy<IObjectManager>(SharedTestVariables.ADMIN_USERNAME, SharedTestVariables.DEFAULT_PASSWORD))
			{
				var queryRequest = new QueryRequest
				{
					ObjectType = new ObjectTypeRef
					{
						Name = _OBJECT_TYPE_NAME
					},
					Fields = new[]
					{
						new FieldRef
						{
							Name = _ARTIFACT_TYPE_ID_FIELD
						}
					},
					Condition = $"'{_NAME_FIELD}' == '{objectTypeName}'"
				};
				QueryResult result = await objectManager.QueryAsync(workspaceId, queryRequest, 0, 1);
				return (int)result.Objects.Single().FieldValues.Single().Value;
			}
		}
	}
}