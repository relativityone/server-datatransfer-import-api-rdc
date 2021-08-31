// ----------------------------------------------------------------------------
// <copyright file="KeplerBulkImportPermissionTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit.Integration.Service.BulkImport
{
	using System;
	using System.Collections.Generic;
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
	public class KeplerBulkImportPermissionTests : KeplerServiceTestBase
	{
		private const string TestUserPassword = "!4321tseT";
		private string _testUserEmail;
		private int _testUserId;
		private int _workspaceGroupId;

		public KeplerBulkImportPermissionTests(bool useKepler)
			: base(useKepler)
		{
		}

		[OneTimeSetUp]
		public async Task OneTimeSetUpAsync()
		{
			this._testUserEmail = $"test.user.{Guid.NewGuid()}@test.com";
			this._workspaceGroupId = await GroupHelper.CreateNewGroupAsync(this.TestParameters, Guid.NewGuid().ToString()).ConfigureAwait(false);
			await PermissionsHelper.AddGroupToWorkspaceAsync(this.TestParameters, this._workspaceGroupId).ConfigureAwait(false);
			await PermissionsHelper.SetWorkspaceOtherSettingsAsync(this.TestParameters, this._workspaceGroupId, new List<string>() { "Allow Export" }, true).ConfigureAwait(false);
			this._testUserId = await UsersHelper.CreateNewUserAsync(
								   this.TestParameters,
								   this._testUserEmail,
								   TestUserPassword,
								   new[] { GroupHelper.EveryoneGroupId, this._workspaceGroupId }).ConfigureAwait(false);
		}

		[OneTimeTearDown]
		public async Task OneTimeTearDownAsync()
		{
			await UsersHelper.RemoveUserAsync(this.TestParameters, this._testUserId).ConfigureAwait(false);
			await GroupHelper.RemoveGroupAsync(this.TestParameters, this._workspaceGroupId).ConfigureAwait(false);
		}

		[IdentifiedTest("315C0D85-4ABD-479A-8FA0-3CB5B0EF5855")]
		public void ShouldReturnTrueWhenHasPermissionForHasImportPermissions()
		{
			// arrange
			using (IBulkImportManager sut = ManagerFactory.CreateBulkImportManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act
				bool actualResult = sut.HasImportPermissions(this.TestParameters.WorkspaceId);

				// assert
				Assert.That(actualResult, Is.True);
			}
		}

		[IdentifiedTest("B5D87962-AB83-40DF-808D-037ADC37FFF8")]
		public void ShouldReturnFalseWhenHasNoPermissionForHasImportPermissions()
		{
			// arrange
			var user = this.Credential.UserName;
			var pass = this.Credential.Password;
			this.Credential.UserName = this._testUserEmail;
			this.Credential.Password = TestUserPassword;

			using (IBulkImportManager sut = ManagerFactory.CreateBulkImportManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act
				bool actualResult = sut.HasImportPermissions(this.TestParameters.WorkspaceId);

				// assert
				Assert.That(actualResult, Is.False);
			}

			this.Credential.UserName = user;
			this.Credential.Password = pass;
		}

		[IdentifiedTest("F630A92C-FCF9-4AAC-8479-7AECF10922FB")]
		public void ShouldThrowExceptionWhenWorkspaceDoesNotExistForHasExportPermissions()
		{
			// arrange
			using (IBulkImportManager sut = ManagerFactory.CreateBulkImportManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act && assert
				Assert.That(
					() => sut.HasImportPermissions(NonExistingWorkspaceId),
					Throws.Exception.InstanceOf<SoapException>().With.Message.Contain("Could not retrieve ApplicationID #0"));
			}
		}
	}
}