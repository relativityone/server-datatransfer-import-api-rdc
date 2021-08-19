// <copyright file="KeplerCaseManagerTests.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.NUnit.Integration.Service
{
	using System.Collections.Generic;
	using System.Data;

	using global::NUnit.Framework;

	using kCura.WinEDDS.Service;
	using kCura.WinEDDS.Service.Replacement;

	[TestFixture(true)]
	[TestFixture(false)]
	public class KeplerCaseManagerTests : KeplerServiceTestBase
	{
		public KeplerCaseManagerTests(bool useKepler)
			: base(useKepler)
		{
		}

		[Test]
		public void ShouldGetAllDocumentFolderPaths()
		{
			// arrange
			using (ICaseManager sut = ManagerFactory.CreateCaseManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act
				string[] documentFolderPaths = sut.GetAllDocumentFolderPaths();

				// assert
				Assert.That(documentFolderPaths, Does.Contain(this.TestParameters.FileShareUncPath));
			}
		}

		[Test]
		public void ShouldGetAllDocumentFolderPathsForCase()
		{
			// arrange
			using (ICaseManager sut = ManagerFactory.CreateCaseManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act
				string[] documentFolderPaths = sut.GetAllDocumentFolderPathsForCase(this.TestParameters.WorkspaceId);

				// assert
				Assert.That(documentFolderPaths, Does.Contain(this.TestParameters.FileShareUncPath));
			}
		}

		[Test]
		public void ShouldRead()
		{
			// arrange
			using (ICaseManager sut = ManagerFactory.CreateCaseManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act
				DataExchange.Service.CaseInfo caseInfo = sut.Read(this.TestParameters.WorkspaceId);

				// assert
				Assert.That(caseInfo.Name, Is.EqualTo(this.TestParameters.WorkspaceName));
				Assert.That(caseInfo.DocumentPath, Is.EqualTo(this.TestParameters.FileShareUncPath));
			}
		}

		[Test]
		public void ShouldRetrieveAll()
		{
			// arrange
			using (ICaseManager sut = ManagerFactory.CreateCaseManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act
				DataSet actualResult = sut.RetrieveAll();

				// assert
				var actualWorkspaceNames = new List<string>();
				foreach (DataRow dataRow in actualResult.Tables[0].Rows)
				{
					string workspaceName = dataRow.Field<string>(1);
					actualWorkspaceNames.Add(workspaceName);
				}

				Assert.That(actualWorkspaceNames, Does.Contain(this.TestParameters.WorkspaceName), "Should contain current workspace.");
			}
		}
	}
}
