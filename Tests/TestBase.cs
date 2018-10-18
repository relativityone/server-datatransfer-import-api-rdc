using System;
using kCura.NUnit.Integration;
using kCura.Relativity.Client;
using kCura.Relativity.ImportAPI.IntegrationTests.Services;
using NUnit.Framework;
using Platform.Keywords.Connection;
using Platform.Keywords.RSAPI;

namespace kCura.Relativity.ImportAPI.IntegrationTests.Tests
{
	public class TestBase
	{
		protected int WorkspaceId { get; private set; }
		protected string DocumentIdentifierColumnName { get; private set; }


		[OneTimeSetUp]
		public virtual void OneTimeSetUp()
		{
			CreateWorkspace();
			DocumentIdentifierColumnName = FieldService.GetDocumentIdentifierFieldName(WorkspaceId);
		}

		[OneTimeTearDown]
		public void OneTimeTearDown()
		{
			DeleteWorkspace();
		}

		private void CreateWorkspace()
		{
			using (IRSAPIClient rsapiClient = ServiceFactory.GetProxy<IRSAPIClient>(SharedTestVariables.ADMIN_USERNAME,
				SharedTestVariables.DEFAULT_PASSWORD))
			{
				string now = DateTime.Now.ToString("MM-dd HH.mm.ss.fff");
				WorkspaceId =
					WorkspaceHelpers.CreateWorkspace(rsapiClient, $"Import API test workspace ({now})", "Relativity Starter Template");
				WorkspaceHelpers.MarkTestWorkspaceAsUsed(WorkspaceId);
			}
		}

		private void DeleteWorkspace()
		{
			if (WorkspaceId > 0)
			{
				WorkspaceHelpers.DeleteTestWorkspace(WorkspaceId);
			}
		}
	}
}
