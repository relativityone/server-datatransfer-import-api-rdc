// ----------------------------------------------------------------------------
// <copyright file="KeplerFieldManagerTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit.Integration.Service
{
	using System;
	using System.Threading.Tasks;
	using System.Web.Services.Protocols;
	using global::NUnit.Framework;
	using kCura.WinEDDS.Service;
	using kCura.WinEDDS.Service.Replacement;
	using Relativity.DataExchange.TestFramework.RelativityHelpers;

	[TestFixture(true)]
	[TestFixture(false)]
	public class KeplerFieldManagerTests : KeplerServiceTestBase
	{
		private const int TestFieldObjectArtifactTypeID = 10;
		private string _testFieldName = Guid.NewGuid().ToString();
		private int _testFieldArtifactID;

		public KeplerFieldManagerTests(bool useKepler)
			: base(useKepler)
		{
		}

		[OneTimeSetUp]
		public async Task KeplerFieldManagerTestsOneTimeSetup() =>
			this._testFieldArtifactID = await FieldHelper.CreateDecimalFieldAsync(this.TestParameters, TestFieldObjectArtifactTypeID, this._testFieldName).ConfigureAwait(false);

		[OneTimeTearDown]
		public async Task KeplerFieldManagerTestsOneTimeTearDownAsync() =>
			await FieldHelper.DeleteFieldAsync(this.TestParameters, this._testFieldArtifactID).ConfigureAwait(false);

		[Test]
		public void ShouldRetrieveControlNumberField()
		{
			// arrange
			using (IFieldManager sut = ManagerFactory.CreateFieldManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act
				kCura.EDDS.WebAPI.FieldManagerBase.Field retreivedField = sut.Read(this.TestParameters.WorkspaceId, this._testFieldArtifactID);

				// assert
				Assert.That(retreivedField, Is.Not.Null);
				Assert.That(retreivedField.DisplayName, Is.EqualTo(this._testFieldName));
				Assert.That(retreivedField.FieldArtifactTypeID, Is.EqualTo(TestFieldObjectArtifactTypeID));
				Assert.That(retreivedField.IsRequired, Is.False);
			}
		}

		[Test]
		public void ShouldCatchExceptionWhenFieldDoesNotExist()
		{
			// arrange
			using (IFieldManager sut = ManagerFactory.CreateFieldManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act & assert
				Assert.Catch<SoapException>(() => sut.Read(this.TestParameters.WorkspaceId, -1));
			}
		}

		[Test]
		public void ShouldCatchExceptionWhenWorkspaceDoesNotExist()
		{
			// arrange
			using (IFieldManager sut = ManagerFactory.CreateFieldManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act & assert
				Assert.Catch<SoapException>(() => sut.Read(0, this._testFieldArtifactID));
			}
		}
	}
}