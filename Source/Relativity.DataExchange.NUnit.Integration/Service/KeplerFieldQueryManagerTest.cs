// ----------------------------------------------------------------------------
// <copyright file="KeplerFieldQueryManagerTest.cs" company="Relativity ODA LLC">
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

	using kCura.WinEDDS;
	using kCura.WinEDDS.Service;
	using kCura.WinEDDS.Service.Replacement;

	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.RelativityHelpers;

	[TestFixture(true)]
	[TestFixture(false)]
	public class KeplerFieldQueryManagerTest : KeplerServiceTestBase
	{
		private const int TestFieldObjectArtifactTypeID = (int)ArtifactType.Document;
		private const int NonExistingWorkspaceID = 0;
		private const int NonExistingArtifactTypeID = -15;
		private string _testDecimalFieldName = Guid.NewGuid().ToString();
		private string _testFixLenTextFieldName = Guid.NewGuid().ToString();
		private int _testDecimalFieldArtifactID;
		private int _testFixLenTextFieldArtifactID;

		public KeplerFieldQueryManagerTest(bool useKepler)
			: base(useKepler)
		{
		}

		[OneTimeSetUp]
		public async Task KeplerFieldManagerTestsOneTimeSetup()
		{
			this._testDecimalFieldArtifactID = await FieldHelper.CreateDecimalFieldAsync(
				                                   this.TestParameters,
				                                   TestFieldObjectArtifactTypeID,
				                                   this._testDecimalFieldName).ConfigureAwait(false);

			this._testFixLenTextFieldArtifactID = await FieldHelper.CreateFixedLengthTextFieldAsync(
				                                      this.TestParameters,
				                                      TestFieldObjectArtifactTypeID,
				                                      this._testFixLenTextFieldName,
				                                      false,
				                                      255).ConfigureAwait(false);
		}

		[OneTimeTearDown]
		public async Task KeplerFieldManagerTestsOneTimeTearDownAsync()
		{
			await FieldHelper.DeleteFieldAsync(this.TestParameters, this._testDecimalFieldArtifactID).ConfigureAwait(false);
			await FieldHelper.DeleteFieldAsync(this.TestParameters, this._testFixLenTextFieldArtifactID).ConfigureAwait(false);
		}

		[Test]
		public void ShouldRetrieveAllMappable()
		{
			// arrange
			string filterString = $"ArtifactID = {this._testDecimalFieldArtifactID}";

			using (IFieldQuery sut = ManagerFactory.CreateFieldQuery(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act
				DataSet actualResult = sut.RetrieveAllMappable(this.TestParameters.WorkspaceId, (int)ArtifactType.Document);

				// assert
				Assert.That(actualResult, Is.Not.Null);
				Assert.That(actualResult.Tables[0].Rows.Count, Is.Not.Negative);
				Assert.That(actualResult.Tables[0].Select(filterString).Length, Is.EqualTo(1));
			}
		}

		[Test]
		public void ShouldThrowExceptionWhenWorkspaceDoesNotExist()
		{
			// arrange
			using (IFieldQuery sut = ManagerFactory.CreateFieldQuery(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act && assert
				Assert.That(
					() => sut.RetrieveAllMappable(NonExistingWorkspaceID, (int)ArtifactType.Document),
					this.GetExpectedExceptionConstraintForNonExistingWorkspace(NonExistingWorkspaceID));
			}
		}

		[Test]
		public void ShouldReturnEmptyTableWhenArtifactTypeIdDoesNotExist()
		{
			// arrange
			using (IFieldQuery sut = ManagerFactory.CreateFieldQuery(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act
				DataSet actualReposnse = sut.RetrieveAllMappable(this.TestParameters.WorkspaceId, NonExistingArtifactTypeID);

				// assert
				Assert.That(actualReposnse, Is.Not.Null);
				Assert.That(actualReposnse.Tables[0].Rows.Count, Is.EqualTo(0));
			}
		}

		[Test]
		public void ShouldRetreiveDataSetFromRetrievePotentialBeginBatesFields()
		{
			// arrange
			string filterString = $"ArtifactID = {this._testFixLenTextFieldArtifactID}";

			using (IFieldQuery sut = ManagerFactory.CreateFieldQuery(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act
				DataSet actualResponse = sut.RetrievePotentialBeginBatesFields(this.TestParameters.WorkspaceId);

				// assert
				Assert.That(actualResponse, Is.Not.Null);
				Assert.That(actualResponse.Tables[0].Rows.Count, Is.Not.Negative);
				Assert.That(actualResponse.Tables[0].Select(filterString).Length, Is.EqualTo(1));
			}
		}

		[Test]
		public void ShouldThrowExceptionWhenWorkspaceDoesNotExistFromRetrievePotentialBeginBatesFields()
		{
			// arrange
			using (IFieldQuery sut = ManagerFactory.CreateFieldQuery(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act && assert
				Assert.That(
					() => sut.RetrievePotentialBeginBatesFields(NonExistingWorkspaceID),
					this.GetExpectedExceptionConstraintForNonExistingWorkspace(NonExistingWorkspaceID));
			}
		}

		[Test]
		public void ShouldTReturnFalseFromIsFieldIndexed()
		{
			// arrange
			using (IFieldQuery sut = ManagerFactory.CreateFieldQuery(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act
				bool actualResult = sut.IsFieldIndexed(this.TestParameters.WorkspaceId, this._testDecimalFieldArtifactID);

				// assert
				Assert.That(actualResult, Is.False);
			}
		}

		[Test]
		public void ShouldReturnTrueForControlNumberFromIsFieldIndexed()
		{
			// arrange
			using (IFieldQuery sut = ManagerFactory.CreateFieldQuery(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act
				bool actualResult = sut.IsFieldIndexed(this.TestParameters.WorkspaceId, WellKnownFields.ControlNumberId);

				// assert
				Assert.That(actualResult, Is.True);
			}
		}

		[Test]
		public void ShouldThrowExceptionWhenWorkspaceDoesNotExistFromIsFieldIndexed()
		{
			// arrange
			using (IFieldQuery sut = ManagerFactory.CreateFieldQuery(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act && assert
				Assert.That(
					() => sut.IsFieldIndexed(NonExistingWorkspaceID, this._testDecimalFieldArtifactID),
					this.GetExpectedExceptionConstraintForNonExistingWorkspace(NonExistingWorkspaceID));
			}
		}

		[Test]
		public void ShouldReturnFalseWhenFieldDoesNotExistFromIsFieldIndexed()
		{
			// arrange
			using (IFieldQuery sut = ManagerFactory.CreateFieldQuery(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act
				bool actualResult = sut.IsFieldIndexed(this.TestParameters.WorkspaceId, NonExistingArtifactTypeID);

				// assert
				Assert.That(actualResult, Is.False);
			}
		}

		[Test]
		public void ShouldReturnCollectionFromRetrieveAllAsDocumentFieldCollection()
		{
			// arrange
			using (IFieldQuery sut = ManagerFactory.CreateFieldQuery(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act
				DocumentFieldCollection actualResult = sut.RetrieveAllAsDocumentFieldCollection(this.TestParameters.WorkspaceId, TestFieldObjectArtifactTypeID);

				// assert
				Assert.That(actualResult, Is.Not.Null);
				Assert.That(actualResult.ToList().Where(x => x.FieldName == this._testDecimalFieldName || x.FieldName == this._testFixLenTextFieldName).Count(), Is.EqualTo(2));
			}
		}

		[Test]
		public void ShouldReturnEmptyWithNonExistingArtifactTypeIdFromRetrieveAllAsDocumentFieldCollection()
		{
			// arrange
			using (IFieldQuery sut = ManagerFactory.CreateFieldQuery(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act
				DocumentFieldCollection actualResult = sut.RetrieveAllAsDocumentFieldCollection(this.TestParameters.WorkspaceId, NonExistingArtifactTypeID);

				// assert
				Assert.That(actualResult, Is.Empty);
			}
		}

		[Test]
		public void ShouldThrowExceptionFromRetrieveAllAsDocumentFieldCollection()
		{
			// arrange
			using (IFieldQuery sut = ManagerFactory.CreateFieldQuery(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act && assert
				Assert.That(
					() => sut.RetrieveAllAsDocumentFieldCollection(NonExistingWorkspaceID, TestFieldObjectArtifactTypeID),
					this.GetExpectedExceptionConstraintForNonExistingWorkspace(NonExistingWorkspaceID));
			}
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public void ShouldReturnCollectionFromRetrieveAllAsDocumentFieldCollection(bool includeUnmappable)
		{
			// arrange
			using (IFieldQuery sut = ManagerFactory.CreateFieldQuery(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act
				kCura.EDDS.WebAPI.DocumentManagerBase.Field[] actualResult = sut.RetrieveAllAsArray(this.TestParameters.WorkspaceId, TestFieldObjectArtifactTypeID, includeUnmappable);

				// assert
				Assert.That(actualResult, Is.Not.Null);
				Assert.That(actualResult.ToList().Where(x => x.DisplayName == this._testDecimalFieldName || x.DisplayName == this._testFixLenTextFieldName).Count(), Is.EqualTo(2));
			}
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public void ShouldReturnEmptyWithNonExistingArtifactTypeIdForRetrieveAllAsArray(bool includeUnmappable)
		{
			// arrange
			using (IFieldQuery sut = ManagerFactory.CreateFieldQuery(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act
				kCura.EDDS.WebAPI.DocumentManagerBase.Field[] actualResult = sut.RetrieveAllAsArray(this.TestParameters.WorkspaceId, NonExistingArtifactTypeID, includeUnmappable);

				// assert
				Assert.That(actualResult, Is.Empty);
			}
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public void ShouldThrowExceptionForRetrieveAllAsArray(bool includeUnmappable)
		{
			// arrange
			using (IFieldQuery sut = ManagerFactory.CreateFieldQuery(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act && assert
				Assert.That(
					() => sut.RetrieveAllAsArray(NonExistingWorkspaceID, TestFieldObjectArtifactTypeID, includeUnmappable),
					this.GetExpectedExceptionConstraintForNonExistingWorkspace(NonExistingWorkspaceID));
			}
		}
	}
}