// <copyright file="KeplerFolderManagerTests.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.NUnit.Integration.Service
{
	using System;
	using System.Linq;
	using System.Threading.Tasks;
	using System.Web.Services.Protocols;

	using global::NUnit.Framework;
	using global::NUnit.Framework.Constraints;

	using kCura.WinEDDS.Service;
	using kCura.WinEDDS.Service.Replacement;

	using Relativity.DataExchange.TestFramework.RelativityHelpers;
	using Relativity.Services;
	using Relativity.Testing.Identification;

	[TestFixture(true)]
	[TestFixture(false)]
	[Feature.DataTransfer.ImportApi]
	public class KeplerFolderManagerTests : KeplerServiceTestBase
	{
		public KeplerFolderManagerTests(bool useKepler)
			: base(useKepler)
		{
		}

		[TestType.MainFlow]
		[IdentifiedTest("08066e53-e519-4a4b-8257-946f0afcc0c5")]
		public async Task ShouldCreateNewFolderInRootAndFolderShouldExist()
		{
			// arrange
			var folderName = Guid.NewGuid().ToString();
			using (IFolderManager sut = ManagerFactory.CreateFolderManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act
				var folderId = sut.Create(this.TestParameters.WorkspaceId, WorkspaceRootFolderId, folderName);
				var exists = sut.Exists(this.TestParameters.WorkspaceId, folderId);
				var id = sut.ReadID(this.TestParameters.WorkspaceId, WorkspaceRootFolderId, folderName);

				// assert
				Assert.That(folderId, Is.GreaterThan(0));
				Assert.That(exists, Is.True);
				Assert.That(id, Is.EqualTo(folderId));

				await this.AssertFolderExists(folderName, folderId).ConfigureAwait(false);
				await FolderHelper.DeleteUnusedFoldersAsync(this.TestParameters).ConfigureAwait(false);
			}
		}

		[TestType.MainFlow]
		[IdentifiedTest("4da42e36-7c24-4790-ad22-752c398a3578")]
		public async Task ShouldNotCreateFolderWithTheSameNameButReturnTheSameFolderId()
		{
			// arrange
			var folderName = Guid.NewGuid().ToString();

			using (IFolderManager sut = ManagerFactory.CreateFolderManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act
				var firstFolderId = sut.Create(this.TestParameters.WorkspaceId, WorkspaceRootFolderId, folderName);
				var firstExists = sut.Exists(this.TestParameters.WorkspaceId, firstFolderId);

				var secondFolderId = sut.Create(this.TestParameters.WorkspaceId, WorkspaceRootFolderId, folderName);
				var secondExists = sut.Exists(this.TestParameters.WorkspaceId, secondFolderId);

				// assert
				Assert.That(firstFolderId, Is.GreaterThan(0));
				Assert.That(secondFolderId, Is.GreaterThan(0));
				Assert.That(firstExists, Is.True);
				Assert.That(secondExists, Is.True);
				Assert.That(firstFolderId, Is.EqualTo(secondFolderId), "When creating folder with name that already exist, the same ID should be returned");

				await this.AssertFolderExists(folderName, firstFolderId).ConfigureAwait(false);
				await FolderHelper.DeleteUnusedFoldersAsync(this.TestParameters).ConfigureAwait(false);
			}
		}

		[TestType.Error]
		[IdentifiedTest("c80b18b1-8011-400e-8079-7a3f67ce8402")]
		public void ShouldNotCreateFolderInNonExistingWorkspace()
		{
			// arrange
			var folderName = Guid.NewGuid().ToString();

			using (IFolderManager sut = ManagerFactory.CreateFolderManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act & assert
				Assert.That(
					() => sut.Create(NonExistingWorkspaceId, WorkspaceRootFolderId, folderName),
					this.GetExpectedExceptionConstraintForNonExistingWorkspace(NonExistingWorkspaceId));
			}
		}

		[TestType.Error]
		[IdentifiedTest("0c96a41f-e692-46cc-8c1b-2cc105473d1c")]
		public void ShouldNotCreateFolderInNonExistingRootFolder()
		{
			// arrange
			var folderName = Guid.NewGuid().ToString();

			using (IFolderManager sut = ManagerFactory.CreateFolderManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act & assert
				Assert.That(
					() => sut.Create(this.TestParameters.WorkspaceId, NonExistingFolderId, folderName),
					GetExpectedExceptionConstraintForNonExistingFolder(NonExistingFolderId, "CreateAsync"));
			}
		}

		[TestType.Error]
		[IdentifiedTest("66418b2f-d06f-45e8-82e6-074a1ccd2ec9")]
		public void ShouldNotCreateFolderForNullFolderName()
		{
			// arrange
			using (IFolderManager sut = ManagerFactory.CreateFolderManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				Assert.That(
					() => sut.Create(this.TestParameters.WorkspaceId, NonExistingFolderId, null),
					Throws.ArgumentNullException);
			}
		}

		[TestType.MainFlow]
		[IdentifiedTest("557d3000-3cf7-4db3-bd72-fe7a2d7c64d1")]
		public async Task ShouldCreateFolderForEmptyFolderName()
		{
			// arrange
			using (IFolderManager sut = ManagerFactory.CreateFolderManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act
				var folderId = sut.Create(this.TestParameters.WorkspaceId, WorkspaceRootFolderId, string.Empty);
				var exists = sut.Exists(this.TestParameters.WorkspaceId, folderId);

				// assert
				Assert.That(folderId, Is.GreaterThan(0));
				Assert.That(exists, Is.True);

				await this.AssertFolderExists(string.Empty, folderId).ConfigureAwait(false);
				await FolderHelper.DeleteUnusedFoldersAsync(this.TestParameters).ConfigureAwait(false);
			}
		}

		[TestType.MainFlow]
		[IdentifiedTest("63c4cb95-a6b9-4c93-a3ea-270a61b09fd4")]
		public void ShouldReturnMinusOneForNonExistingFolderName()
		{
			// arrange
			var folderName = Guid.NewGuid().ToString();

			using (IFolderManager sut = ManagerFactory.CreateFolderManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act
				var id = sut.ReadID(this.TestParameters.WorkspaceId, NonExistingFolderId, folderName);

				// assert
				Assert.That(id, Is.EqualTo(-1));
			}
		}

		[TestType.MainFlow]
		[IdentifiedTest("5374f8bd-3c4b-4a27-a05e-edff036b7899")]
		public void ShouldReturnFalseForNonExistingFolderId()
		{
			// arrange
			using (IFolderManager sut = ManagerFactory.CreateFolderManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act
				var id = sut.Exists(this.TestParameters.WorkspaceId, NonExistingFolderId);

				// assert
				Assert.That(id, Is.False);
			}
		}

		[TestType.Error]
		[IdentifiedTest("512d8915-5dc1-4c91-9777-ea50a09123a7")]
		public void ShouldReturnErrorForNonExistingWorkspaceId()
		{
			// arrange
			using (IFolderManager sut = ManagerFactory.CreateFolderManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act & assert
				Assert.That(
					() => sut.Exists(NonExistingWorkspaceId, NonExistingFolderId),
					this.GetExpectedExceptionConstraintForNonExistingWorkspace(NonExistingWorkspaceId));
			}
		}

		[TestType.MainFlow]
		[IdentifiedTest("d60642c4-c6e4-4c64-ab33-e296c419517c")]
		public async Task ShouldReadFolderInfoForCreatedFolder()
		{
			// arrange
			using (IFolderManager sut = ManagerFactory.CreateFolderManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act
				var folderName = Guid.NewGuid().ToString();
				var folderId = sut.Create(this.TestParameters.WorkspaceId, WorkspaceRootFolderId, folderName);

				// assert
				var folder = sut.Read(this.TestParameters.WorkspaceId, folderId);
				Assert.That(folder, Is.Not.Null);
				Assert.That(folder.Name, Is.EqualTo(folderName));
				Assert.That(folder.TextIdentifier, Is.EqualTo(folderName));
				Assert.That(folder.ParentArtifactID, Is.EqualTo(WorkspaceRootFolderId));

				await this.AssertFolderExists(folderName, folderId).ConfigureAwait(false);
				await FolderHelper.DeleteUnusedFoldersAsync(this.TestParameters).ConfigureAwait(false);
			}
		}

		[TestType.Error]
		[IdentifiedTest("8d5ab368-abdc-455a-a40a-9bcb337be2e6")]
		public void ShouldThrowErrorWhenReadNonExistingFolder()
		{
			// arrange
			using (IFolderManager sut = ManagerFactory.CreateFolderManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act & assert
				Assert.That(
					() => sut.Read(this.TestParameters.WorkspaceId, NonExistingFolderId),
					GetExpectedExceptionConstraintForNonExistingFolder(NonExistingFolderId, "ReadAsync"));
			}
		}

		[TestType.Error]
		[IdentifiedTest("d45a306c-9626-437f-a8dc-7a8824aa94bc")]
		public void ShouldThrowErrorWhenReadNonExistingWorkspace()
		{
			// arrange
			using (IFolderManager sut = ManagerFactory.CreateFolderManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act & assert
				Assert.That(
					() => sut.Read(NonExistingWorkspaceId, NonExistingFolderId),
					this.GetExpectedExceptionConstraintForNonExistingWorkspace(NonExistingWorkspaceId));
			}
		}

		[TestType.MainFlow]
		[IdentifiedTest("78e12d60-431f-42aa-989b-ce21a58d8c28")]
		public async Task ShouldRetrieveFolderInfoForCreatedFolders()
		{
			// arrange
			var folderName = Guid.NewGuid().ToString();

			using (IFolderManager sut = ManagerFactory.CreateFolderManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				var folderId = sut.Create(this.TestParameters.WorkspaceId, WorkspaceRootFolderId, folderName);

				// act
				var folder = sut.RetrieveIntitialChunk(this.TestParameters.WorkspaceId);

				// assert
				Assert.That(folder, Is.Not.Null);
				var table = folder.Tables[0];
				Assert.That(table.Columns.Count, Is.EqualTo(3));
				Assert.That(table.Columns.Contains("Name"), Is.True);
				Assert.That(table.Columns.Contains("ArtifactID"), Is.True);
				Assert.That(table.Columns.Contains("ParentArtifactID"), Is.True);

				Assert.That(table.Rows.Count, Is.GreaterThan(0));
				Assert.That(table.Rows[0]["ArtifactID"], Is.EqualTo(WorkspaceRootFolderId));
				Assert.That(table.Rows[0]["ParentArtifactID"], Is.EqualTo(DBNull.Value));
				Assert.That(table.Rows[0]["Name"], Is.EqualTo(this.TestParameters.WorkspaceName));

				var row = table.Select($"ArtifactID = {folderId}");
				Assert.That(row[0]["ArtifactID"], Is.EqualTo(folderId));
				Assert.That(row[0]["ParentArtifactID"], Is.EqualTo(WorkspaceRootFolderId));
				Assert.That(row[0]["Name"], Is.EqualTo(folderName));

				await this.AssertFolderExists(folderName, folderId).ConfigureAwait(false);
				await FolderHelper.DeleteUnusedFoldersAsync(this.TestParameters).ConfigureAwait(false);
			}
		}

		private static ContainsConstraint GetExpectedExceptionConstraintForNonExistingFolder(int folderId, string callName)
		{
			return Throws.Exception.InstanceOf<SoapException>()
				.With.Message.Contains($"ArtifactID {folderId} does not exist.")
				.And.Message.Contains(callName);
		}

		private async Task AssertFolderExists(string folderName, int folderId)
		{
			var result = await FolderHelper.QueryAsync(this.TestParameters, new Query()).ConfigureAwait(false);
			var folderRef = result.Results.SingleOrDefault(x => x.Artifact.Name == folderName);
			Assert.That(folderRef, Is.Not.Null);
			Assert.That(folderRef.Artifact.ArtifactID, Is.EqualTo(folderId));
		}
	}
}
