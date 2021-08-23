// <copyright file="KeplerUserManagerTests.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.NUnit.Integration.Service
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Linq;
	using System.Threading.Tasks;
	using System.Web.Services.Protocols;

	using global::NUnit.Framework;

	using kCura.WinEDDS.Service;
	using kCura.WinEDDS.Service.Replacement;

	using Relativity.DataExchange.TestFramework.RelativityHelpers;
	using Relativity.Testing.Identification;

	[TestFixture(true)]
	[TestFixture(false)]
	[Feature.DataTransfer.ImportApi]
	public class KeplerUserManagerTests : KeplerServiceTestBase
	{
		private const string AssignableUserEmail = "assignable.user@test.com";
		private const string EveryoneOnlyGroupUserEmail = "in.everyone.group@test.com";
		private const string NoneGroupUserEmail = "without.any.group@test.com";
		private const string GroupNotInWorkspaceUserEmail = "group.not.in.workspace@test.com";

		private readonly List<int> createdUsersIds = new List<int>();
		private readonly List<int> createdGroupsIds = new List<int>();

		public KeplerUserManagerTests(bool useKepler)
			: base(useKepler)
		{
		}

		[OneTimeSetUp]
		public async Task OneTimeSetUpAsync()
		{
			await this.CreateUserAsync(NoneGroupUserEmail, Enumerable.Empty<int>()).ConfigureAwait(false);
			await this.CreateUserAsync(EveryoneOnlyGroupUserEmail, new[] { GroupHelper.EveryoneGroupId }).ConfigureAwait(false);

			int groupNotInWorkspaceUserId = await this.CreateUserAsync(GroupNotInWorkspaceUserEmail, new[] { GroupHelper.EveryoneGroupId }).ConfigureAwait(false);
			int nonWorkspaceGroupId = await this.CreateGroupAsync().ConfigureAwait(false);
			await GroupHelper.AddMemberAsync(this.TestParameters, nonWorkspaceGroupId, groupNotInWorkspaceUserId).ConfigureAwait(false);

			int assignableUserId = await this.CreateUserAsync(AssignableUserEmail, Enumerable.Empty<int>()).ConfigureAwait(false);
			int workspaceGroupId = await this.CreateGroupAsync().ConfigureAwait(false);
			await GroupHelper.AddMemberAsync(this.TestParameters, workspaceGroupId, assignableUserId).ConfigureAwait(false);
			await PermissionsHelper.AddGroupToWorkspaceAsync(this.TestParameters, workspaceGroupId).ConfigureAwait(false);
		}

		[OneTimeTearDown]
		public async Task OneTimeTearDownAsync()
		{
			foreach (var userId in this.createdUsersIds)
			{
				await UsersHelper.RemoveUserAsync(this.TestParameters, userId).ConfigureAwait(false);
			}

			foreach (var groupId in this.createdGroupsIds)
			{
				await GroupHelper.RemoveGroupAsync(this.TestParameters, groupId).ConfigureAwait(false);
			}
		}

		[IdentifiedTest("e2c396c2-46d3-42b5-962f-551c91451780")]
		public void ShouldRetrieveAllAssignableInCase()
		{
			// arrange
			using (IUserManager sut = ManagerFactory.CreateUserManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act
				DataSet actualResult = sut.RetrieveAllAssignableInCase(this.TestParameters.WorkspaceId);

				// assert
				var actualUsersEmails = new List<string>();
				foreach (DataRow dataRow in actualResult.Tables[0].Rows)
				{
					string email = dataRow.Field<string>(2);
					actualUsersEmails.Add(email);
				}

				Assert.That(actualUsersEmails, Does.Contain(AssignableUserEmail), "Should return assignable user.");
				Assert.That(
					actualUsersEmails,
					Does.Not.Contain(NoneGroupUserEmail),
					"Should not return user which is not assigned to any group.");
				Assert.That(
					actualUsersEmails,
					Does.Not.Contain(EveryoneOnlyGroupUserEmail),
					"Should not return user which is assigned only to Everyone group.");
				Assert.That(
					actualUsersEmails,
					Does.Not.Contain(GroupNotInWorkspaceUserEmail),
					"Should not return user which group is not assigned to workspace.");
			}
		}

		[IdentifiedTest("fbffb953-6e65-47dd-b5b7-ded13b5d70b2")]
		public void ShouldRetrieveAllAssignableInCaseForNonExistingWorkspace()
		{
			// arrange
			const int NonExistingWorkspaceId = 123456789;
			using (IUserManager sut = ManagerFactory.CreateUserManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act & assert
				Assert.That(
					() => sut.RetrieveAllAssignableInCase(NonExistingWorkspaceId),
					this.GetExpectedExceptionConstraintForNonExistingWorkspace(NonExistingWorkspaceId));
			}
		}

		[IdentifiedTest("02c4b0ad-244b-4920-9d7f-81b0019c9ddd")]
		public void ShouldRetrieveAllAssignableInCaseWhenUserDoesNotHaveAccessToTheWorkspace()
		{
			// arrange
			this.Credential.UserName = GroupNotInWorkspaceUserEmail;
			this.Credential.Password = "!4321tseT";

			using (IUserManager sut = ManagerFactory.CreateUserManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				if (this.UseKepler)
				{
					// act & assert
					Assert.That(
						() => sut.RetrieveAllAssignableInCase(this.TestParameters.WorkspaceId),
						Throws.Exception
							.InstanceOf<SoapException>()
							.With.Message.EqualTo("The service endpoint denied the request.")
							.Or.With.Message.EqualTo("User does not have permissions to use WebAPI Kepler replacement"));
				}
				else
				{
					// act
					DataSet result = sut.RetrieveAllAssignableInCase(this.TestParameters.WorkspaceId);

					// assert
					Assert.That(
						result.Tables[0].Rows,
						Is.Not.Empty,
						"WebAPI returns assignable users even if user does not have access to the workspace.");
				}
			}
		}

		private async Task<int> CreateUserAsync(
			string email,
			IEnumerable<int> groupArtifactIds)
		{
			int userId = await UsersHelper.CreateNewUserAsync(
							 this.TestParameters,
							 email,
							 "!4321tseT",
							 groupArtifactIds).ConfigureAwait(false);
			this.createdUsersIds.Add(userId);
			return userId;
		}

		private async Task<int> CreateGroupAsync()
		{
			int groupId = await GroupHelper.CreateNewGroupAsync(this.TestParameters, Guid.NewGuid().ToString()).ConfigureAwait(false);
			this.createdGroupsIds.Add(groupId);
			return groupId;
		}
	}
}
