using System;
using kCura.NUnit.Integration;
using kCura.Relativity.Client;
using NUnit.Framework;
using Platform.Keywords.Connection;
using Platform.Keywords.RSAPI;

namespace kCura.Relativity.ImportAPI.IntegrationTests.Tests
{
	public class TestBase
	{
		public int WorkspaceId { get; private set; }

		[SetUp]
		public virtual void SetUp()
		{
			CreateWorkspace();
		}

		[TearDown]
		public void TearDown()
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
