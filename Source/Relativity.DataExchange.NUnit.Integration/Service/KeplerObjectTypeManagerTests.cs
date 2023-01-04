// ----------------------------------------------------------------------------
// <copyright file="KeplerObjectTypeManagerTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit.Integration.Service
{
	using System;
	using System.Data;
	using System.Linq;
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using kCura.WinEDDS.Service;
	using kCura.WinEDDS.Service.Replacement;

	using Relativity.DataExchange.TestFramework.RelativityHelpers;
	using Relativity.Testing.Identification;

	[TestFixture(true)]
	[TestFixture(false)]
	public class KeplerObjectTypeManagerTests : KeplerServiceTestBase
	{
		private const int InvalidWorkspaceId = 42;

		private ObjectType parentObjectType;
		private ObjectType childObjectType;

		public KeplerObjectTypeManagerTests(bool useKepler)
			: base(useKepler)
		{
		}

		[OneTimeSetUp]
		public async Task OneTimeSetupAsync()
		{
			this.parentObjectType = await RdoHelper.CreateObjectTypeAsync(this.TestParameters, $"{Guid.NewGuid()}-Object", this.TestParameters.WorkspaceId, RdoHelper.WorkspaceArtifactTypeId)
										.ConfigureAwait(false);
			this.childObjectType = await RdoHelper.CreateObjectTypeAsync(this.TestParameters, $"{Guid.NewGuid()}-ChildObject", this.TestParameters.WorkspaceId, this.parentObjectType.ArtifactTypeId)
										.ConfigureAwait(false);
		}

		[OneTimeTearDown]
		public async Task OneTimeTearDownAsync()
		{
			await RdoHelper.DeleteObjectTypeAsync(this.TestParameters, this.childObjectType.ArtifactId).ConfigureAwait(false);
			await RdoHelper.DeleteObjectTypeAsync(this.TestParameters, this.parentObjectType.ArtifactId).ConfigureAwait(false);
		}

		[IdentifiedTest("B39C54D0-3232-47DD-897E-1098FFE7F263")]
		public void ShouldRetrieveAllUploadable()
		{
			// Arrange
			string[] expectedColumns = { "DescriptorArtifactTypeID", "Name", "IsSystem", "ArtifactID", "ParentArtifactTypeID", "ParentArtifactType", "IsChild", "HasAddPermission" };
			using (IObjectTypeManager sut = ManagerFactory.CreateObjectTypeManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// Act
				DataSet result = sut.RetrieveAllUploadable(this.TestParameters.WorkspaceId);

				// Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Tables.Count, Is.EqualTo(1));
				string[] actualColumns = (from DataColumn column in result.Tables[0].Columns select column.ColumnName).ToArray();
				Assert.That(actualColumns, Is.EquivalentTo(expectedColumns));
			}
		}

		[IdentifiedTest("C12AD340-97A9-4DD6-8D81-16A572DBC267")]
		public void ShouldThrowExceptionWhenCallingRetrieveAllUploadableWithInvalidWorkspaceId()
		{
			// Arrange
			using (IObjectTypeManager sut = ManagerFactory.CreateObjectTypeManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// Act & Assert
				Assert.That(() => sut.RetrieveAllUploadable(InvalidWorkspaceId), this.GetExpectedExceptionConstraintForNonExistingWorkspace(InvalidWorkspaceId));
			}
		}

		[IdentifiedTest("2F17AA76-CC31-46F2-B909-D88E6ED27E37")]
		public void ShouldRetrieveParentArtifactTypeID()
		{
			// Arrange
			const string ParentArtifactTypeIdKey = "ParentArtifactTypeID";
			using (IObjectTypeManager sut = ManagerFactory.CreateObjectTypeManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// Act
				DataSet result = sut.RetrieveParentArtifactTypeID(this.TestParameters.WorkspaceId, this.childObjectType.ArtifactTypeId);

				// Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Tables.Count, Is.EqualTo(1));
				Assert.That(result.Tables[0].Rows.Count, Is.EqualTo(1));

				var actualParentArtifactTypeId = result.Tables[0].Rows[0].Field<int>(ParentArtifactTypeIdKey);
				Assert.That(actualParentArtifactTypeId, Is.EqualTo(this.parentObjectType.ArtifactTypeId));
			}
		}

		[IdentifiedTest("8DCADDA7-B661-4EFC-8EF9-3C28FAA1B545")]
		public void ShouldThrowExceptionWhenCallingRetrieveParentArtifactTypeIDWithInvalidWorkspaceId()
		{
			// Arrange
			using (IObjectTypeManager sut = ManagerFactory.CreateObjectTypeManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// Act & Assert
				Assert.That(() => sut.RetrieveParentArtifactTypeID(InvalidWorkspaceId, this.childObjectType.ArtifactTypeId), this.GetExpectedExceptionConstraintForNonExistingWorkspace(InvalidWorkspaceId));
			}
		}

		[IdentifiedTest("ABC3B3A7-651E-459E-A4E1-2E1B3A4A59E4")]
		public void ShouldNotThrowExceptionWhenCallingRetrieveParentArtifactTypeIDWithInvalidArtifactTypeId()
		{
			// Arrange
			const int InvalidArtifactTypeId = -999;
			DataSet result = null;
			using (IObjectTypeManager sut = ManagerFactory.CreateObjectTypeManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// Act & Assert
				Assert.DoesNotThrow(() => result = sut.RetrieveParentArtifactTypeID(this.TestParameters.WorkspaceId, InvalidArtifactTypeId));

				Assert.That(result, Is.Not.Null);
				Assert.That(result.Tables.Count, Is.EqualTo(1));
				Assert.That(result.Tables[0].Rows.Count, Is.Zero, "Should not retrieve parent artifact type identifier when invalid artifact type id passed.");
			}
		}
	}
}