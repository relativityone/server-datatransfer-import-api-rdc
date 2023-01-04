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

	using Relativity.Testing.Identification;

	[TestFixture(true)]
	[TestFixture(false)]
	[Feature.DataTransfer.ImportApi]
	public class KeplerCaseManagerTests : KeplerServiceTestBase
	{
		public KeplerCaseManagerTests(bool useKepler)
			: base(useKepler)
		{
		}

		[IdentifiedTest("44cbeb2f-6648-4492-abe7-9e31dae413c5")]
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

		[IdentifiedTest("dac8979a-5f39-47f3-a99b-d09669a02ebd")]
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

		[IdentifiedTest("660577a1-ef8d-4943-bbcb-e5331ef96896")]
		public void ShouldNotGetAllDocumentFolderPathsForCaseWhenParametersAreInvalid()
		{
			// arrange
			using (ICaseManager sut = ManagerFactory.CreateCaseManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act && assert
				// TODO: remove inconsistency between Kepler and WebAPI REL-586780
				if (this.UseKepler)
				{
					Assert.That(() => sut.GetAllDocumentFolderPathsForCase(NonExistingWorkspaceId), this.GetExpectedExceptionConstraintForNonExistingWorkspace(NonExistingWorkspaceId));
				}
				else
				{
					string[] documentFolderPaths = sut.GetAllDocumentFolderPathsForCase(NonExistingWorkspaceId);
					Assert.That(documentFolderPaths, Does.Contain(this.TestParameters.FileShareUncPath));
				}
			}
		}

		[IdentifiedTest("52d461ae-0908-4011-ad61-c7160976ef5b")]
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

		[IdentifiedTest("61b8460a-3391-410b-93b1-42f386a0e25e")]
		public void ShouldNotReadWhenParametersAreInvalid()
		{
			// arrange
			using (ICaseManager sut = ManagerFactory.CreateCaseManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act && assert
				Assert.That(() => sut.Read(NonExistingWorkspaceId), this.GetExpectedExceptionConstraintForNonExistingWorkspace(NonExistingWorkspaceId));
			}
		}

		[IdentifiedTest("341a9156-a756-475e-891c-05fb9069d0f3")]
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

				Assert.That(actualWorkspaceNames, Does.Contain(this.TestParameters.WorkspaceName));
			}
		}
	}
}