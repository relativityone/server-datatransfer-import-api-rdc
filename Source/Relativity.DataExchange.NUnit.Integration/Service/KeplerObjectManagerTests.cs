// ----------------------------------------------------------------------------
// <copyright file="KeplerObjectManagerTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit.Integration.Service
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using kCura.WinEDDS.Service;
	using kCura.WinEDDS.Service.Replacement;

	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.RelativityHelpers;
	using Relativity.Testing.Identification;

	[TestFixture(true)]
	[TestFixture(false)]
	public class KeplerObjectManagerTests : KeplerServiceTestBase
	{
		private const int InvalidWorkspaceId = 42;
		private const int InvalidObjectTypeId = -999;

		private ObjectType parentObjectType;
		private string parentObjectTextIdentifier;
		private int parentObjectArtifactId;

		private ObjectType childObjectType;
		private string childObjectTextIdentifier;
		private int childObjectArtifactId;

		public KeplerObjectManagerTests(bool useKepler)
			: base(useKepler)
		{
		}

		[OneTimeSetUp]
		public async Task OneTimeSetupAsync()
		{
			this.parentObjectType = await RdoHelper.CreateObjectTypeAsync(this.TestParameters, Guid.NewGuid().ToString(), this.TestParameters.WorkspaceId, RdoHelper.WorkspaceArtifactTypeId).ConfigureAwait(false);
			this.parentObjectTextIdentifier = $"{Guid.NewGuid()}-ParentObj";
			this.parentObjectArtifactId = RdoHelper.CreateObjectTypeInstance(
				this.TestParameters,
				this.parentObjectType.ArtifactTypeId,
				new Dictionary<string, object>
					{
						{ WellKnownFields.RdoIdentifier, this.parentObjectTextIdentifier },
					});

			this.childObjectType = await RdoHelper.CreateObjectTypeAsync(this.TestParameters, Guid.NewGuid().ToString(), this.TestParameters.WorkspaceId, this.parentObjectType.ArtifactTypeId).ConfigureAwait(false);
			this.childObjectTextIdentifier = $"{Guid.NewGuid()}-ChildObj";
			this.childObjectArtifactId = RdoHelper.CreateObjectTypeInstance(
				this.TestParameters,
				this.childObjectType.ArtifactTypeId,
				new Dictionary<string, object>
					{
						{ WellKnownFields.RdoIdentifier, this.childObjectTextIdentifier },
					},
				this.parentObjectArtifactId);
		}

		[OneTimeTearDown]
		public async Task OneTimeTearDownAsync()
		{
			await RdoHelper.DeleteAllObjectsByTypeAsync(this.TestParameters, this.childObjectType.ArtifactTypeId).ConfigureAwait(false);
			await RdoHelper.DeleteAllObjectsByTypeAsync(this.TestParameters, this.parentObjectType.ArtifactTypeId).ConfigureAwait(false);

			await RdoHelper.DeleteObjectTypeAsync(this.TestParameters, this.childObjectType.ArtifactId).ConfigureAwait(false);
			await RdoHelper.DeleteObjectTypeAsync(this.TestParameters, this.parentObjectType.ArtifactId).ConfigureAwait(false);
		}

		[IdentifiedTest("d81acce0-8aca-4253-b06e-371ced316fb6")]
		public void ShouldRetrieveArtifactIdOfMappedObject()
		{
			// Arrange
			const string ArtifactIdKey = "ArtifactID";
			using (IObjectManager sut = ManagerFactory.CreateObjectManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// Act
				DataSet result = sut.RetrieveArtifactIdOfMappedObject(this.TestParameters.WorkspaceId, this.childObjectTextIdentifier, this.childObjectType.ArtifactTypeId);

				// Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Tables.Count, Is.EqualTo(1));
				Assert.That(result.Tables[0].Rows.Count, Is.EqualTo(1));

				var actualObjectArtifactId = result.Tables[0].Rows[0].Field<int>(ArtifactIdKey);
				Assert.That(actualObjectArtifactId, Is.EqualTo(this.childObjectArtifactId));
			}
		}

		[IdentifiedTest("0d2703a9-d75e-4e7b-bd94-9bdccb1a6b2c")]
		public void ShouldThrowExceptionWhenCallingRetrieveArtifactIdOfMappedObjectWithInvalidWorkspaceId()
		{
			// Arrange
			using (IObjectManager sut = ManagerFactory.CreateObjectManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// Act & Assert
				Assert.That(() => sut.RetrieveArtifactIdOfMappedObject(InvalidWorkspaceId, this.childObjectTextIdentifier, this.childObjectType.ArtifactTypeId), this.GetExpectedExceptionConstraintForNonExistingWorkspace(InvalidWorkspaceId));
			}
		}

		[IdentifiedTest("950e6c33-afce-43fc-8518-c30616963421")]
		public void ShouldReturnEmptyDatasetWhenCallingRetrieveArtifactIdOfMappedObjectWithInvalidArtifactTypeId()
		{
			// Arrange
			using (IObjectManager sut = ManagerFactory.CreateObjectManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// Act
				DataSet result = sut.RetrieveArtifactIdOfMappedObject(
							this.TestParameters.WorkspaceId,
							this.childObjectTextIdentifier,
							InvalidObjectTypeId);

				// Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Tables.Count, Is.EqualTo(1));
				Assert.That(result.Tables[0].Rows.Count, Is.Zero, "Should not retrieve artifact ID when invalid object type ID was passed");
			}
		}

		[IdentifiedTest("3ff65964-58b8-49be-879e-d1150fd834bd")]
		public void ShouldReturnEmptyDatasetWhenCallingRetrieveArtifactIdOfMappedObjectWithInvalidTextIdentifier()
		{
			// Arrange
			const string InvalidTextIdentifier = "InvalidTextIdentifier";
			using (IObjectManager sut = ManagerFactory.CreateObjectManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// Act
				DataSet result = sut.RetrieveArtifactIdOfMappedObject(
							this.TestParameters.WorkspaceId,
							InvalidTextIdentifier,
							this.childObjectType.ArtifactTypeId);

				// Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Tables.Count, Is.EqualTo(1));
				Assert.That(result.Tables[0].Rows.Count, Is.Zero, "Should not retrieve artifact ID when invalid text identifier was passed");
			}
		}

		[IdentifiedTest("d8018e63-1ab5-42a4-9f80-23093832fa6b")]
		public void ShouldRetrieveTextIdentifierOfMappedObject()
		{
			// Arrange
			const string TextIdentifierKey = "TextIdentifier";
			using (IObjectManager sut = ManagerFactory.CreateObjectManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// Act
				DataSet result = sut.RetrieveTextIdentifierOfMappedObject(this.TestParameters.WorkspaceId, this.childObjectArtifactId, this.childObjectType.ArtifactTypeId);

				// Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Tables.Count, Is.EqualTo(1));
				Assert.That(result.Tables[0].Rows.Count, Is.EqualTo(1));

				var actualObjectTextIdentifier = result.Tables[0].Rows[0].Field<string>(TextIdentifierKey);
				Assert.That(actualObjectTextIdentifier, Is.EqualTo(this.childObjectTextIdentifier));
			}
		}

		[IdentifiedTest("16da1527-1ca1-4fdf-b4ec-9dbf15387795")]
		public void ShouldThrowExceptionWhenCallingRetrieveTextIdentifierOfMappedObjectWithInvalidWorkspaceId()
		{
			// Arrange
			using (IObjectManager sut = ManagerFactory.CreateObjectManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// Act & Assert
				Assert.That(() => sut.RetrieveTextIdentifierOfMappedObject(InvalidWorkspaceId, this.childObjectArtifactId, this.childObjectType.ArtifactTypeId), this.GetExpectedExceptionConstraintForNonExistingWorkspace(InvalidWorkspaceId));
			}
		}

		[IdentifiedTest("9e1f9a84-631c-43ad-9bc0-4b6e0e9ff2d6")]
		public void ShouldReturnEmptyDatasetWhenCallingRetrieveTextIdentifierOfMappedObjectWithInvalidArtifactTypeId()
		{
			// Arrange
			using (IObjectManager sut = ManagerFactory.CreateObjectManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// Act
				DataSet result = sut.RetrieveTextIdentifierOfMappedObject(
							this.TestParameters.WorkspaceId,
							this.childObjectArtifactId,
							InvalidObjectTypeId);

				// Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Tables.Count, Is.EqualTo(1));
				Assert.That(result.Tables[0].Rows.Count, Is.Zero, "Should not retrieve text identifier when invalid object type ID was passed");
			}
		}

		[IdentifiedTest("c30d2f98-3c64-4cd9-ac8c-98e73d459cda")]
		public void ShouldReturnEmptyDatasetWhenCallingRetrieveTextIdentifierOfMappedObjectWithInvalidArtifactId()
		{
			// Arrange
			const int InvalidArtifactId = -999;
			using (IObjectManager sut = ManagerFactory.CreateObjectManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// Act
				DataSet result = sut.RetrieveTextIdentifierOfMappedObject(
							  this.TestParameters.WorkspaceId,
							  InvalidArtifactId,
							  this.childObjectType.ArtifactTypeId);

				// Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Tables.Count, Is.EqualTo(1));
				Assert.That(result.Tables[0].Rows.Count, Is.Zero, "Should not retrieve text identifier when invalid artifact ID was passed");
			}
		}

		[IdentifiedTest("9fc22790-90b0-4fef-a683-9b7d98542053")]
		public void ShouldRetrieveArtifactIdOfMappedParentObject()
		{
			// Arrange
			const string ArtifactIdKey = "ArtifactID";
			using (IObjectManager sut = ManagerFactory.CreateObjectManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// Act
				DataSet result = sut.RetrieveArtifactIdOfMappedParentObject(this.TestParameters.WorkspaceId, this.parentObjectTextIdentifier, this.childObjectType.ArtifactTypeId);

				// Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Tables.Count, Is.EqualTo(1));
				Assert.That(result.Tables[0].Rows.Count, Is.EqualTo(1));

				var actualObjectArtifactId = result.Tables[0].Rows[0].Field<int>(ArtifactIdKey);
				Assert.That(actualObjectArtifactId, Is.EqualTo(this.parentObjectArtifactId));
			}
		}

		[IdentifiedTest("dca1f579-bb66-46ab-8162-6bf607d32e23")]
		public void ShouldThrowExceptionWhenCallingRetrieveArtifactIdOfMappedParentObjectWithInvalidWorkspaceId()
		{
			// Arrange
			using (IObjectManager sut = ManagerFactory.CreateObjectManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// Act & Assert
				Assert.That(() => sut.RetrieveArtifactIdOfMappedParentObject(InvalidWorkspaceId, this.parentObjectTextIdentifier, this.childObjectType.ArtifactTypeId), this.GetExpectedExceptionConstraintForNonExistingWorkspace(InvalidWorkspaceId));
			}
		}

		[IdentifiedTest("41763323-9370-4d09-b8ee-fb42b77d1695")]
		public void ShouldReturnEmptyDatasetWhenCallingRetrieveArtifactIdOfMappedParentObjectWithInvalidArtifactTypeId()
		{
			// Arrange
			using (IObjectManager sut = ManagerFactory.CreateObjectManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// Act
				DataSet result = sut.RetrieveArtifactIdOfMappedParentObject(this.TestParameters.WorkspaceId, this.parentObjectTextIdentifier, InvalidObjectTypeId);

				// Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Tables.Count, Is.EqualTo(1));
				Assert.That(result.Tables[0].Rows.Count, Is.Zero, "Should not retrieve parent artifact ID when invalid object type ID was passed");
			}
		}

		[IdentifiedTest("129b9edf-dce7-4620-8ca7-a240c84d7ffb")]
		public void ShouldReturnEmptyDatasetWhenCallingRetrieveArtifactIdOfMappedParentObjectWithInvalidTextIdentifier()
		{
			// Arrange
			const string InvalidTextIdentifier = "InvalidTextIdentifier";
			using (IObjectManager sut = ManagerFactory.CreateObjectManager(this.Credential, this.CookieContainer, this.CorrelationIdFunc))
			{
				// Act
				DataSet result = sut.RetrieveArtifactIdOfMappedParentObject(this.TestParameters.WorkspaceId, InvalidTextIdentifier, this.childObjectType.ArtifactTypeId);

				// Assert
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Tables.Count, Is.EqualTo(1));
				Assert.That(result.Tables[0].Rows.Count, Is.Zero, "Should not retrieve parent artifact ID when invalid text identifier was passed");
			}
		}
	}
}