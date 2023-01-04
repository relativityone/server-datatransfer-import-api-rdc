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
	using Relativity.Testing.Identification;

	[TestFixture(true)]
	[TestFixture(false)]
	[Feature.DataTransfer.ImportApi]
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

		[IdentifiedTest("7807e119-5f78-44ec-a737-68e8f4a33078")]
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

		[IdentifiedTest("c8e2560a-0a6c-4c8c-8413-6f452f9aa753")]
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

		[IdentifiedTest("6107e5e2-002f-4c5b-adaa-1e7d83f1d15b")]
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

		[IdentifiedTest("57f84294-c213-4f50-ac96-df6dbe276c9c")]
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

		[IdentifiedTest("2061ebdf-65d1-44b8-ad6b-8424362dbeb9")]
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

		[IdentifiedTest("eed6fb78-ba39-410e-acdb-119ce7cfcfaa")]
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

		[IdentifiedTest("3883e0fc-c4a8-43d1-a725-046beca6e6b0")]
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

		[IdentifiedTest("0733468a-b400-44d1-83fc-eb32d9d9d733")]
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

		[IdentifiedTest("0744dac1-a6a1-452a-b866-06772279be82")]
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

		[IdentifiedTest("c3b1bc63-3ea9-46c4-a8f2-389d21ca6671")]
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

		[IdentifiedTest("9365196a-14c3-4afa-b8c6-6b4acc3856e6")]
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

		[IdentifiedTest("1011b113-814d-4461-b8d1-45b19b433b07")]
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

		[IdentifiedTest("1070ccac-481e-4f6e-b230-209f403670c8")]
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

		[IdentifiedTest("43c83022-716e-4379-92a0-ca7e1b400989")]
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

		[IdentifiedTest("ab44eae2-9f15-4a5c-b150-e817b3dc3f55")]
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