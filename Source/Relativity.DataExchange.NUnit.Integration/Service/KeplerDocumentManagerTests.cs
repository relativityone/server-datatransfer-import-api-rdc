// ----------------------------------------------------------------------------
// <copyright file="KeplerDocumentManagerTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit.Integration.Service
{
	using global::NUnit.Framework;
	using kCura.WinEDDS.Service;

	/// <summary>
	/// Additional tests for IDocumentManger in DataTransfer.Legacy
	/// <see href="https://git.kcura.com/projects/DTX/repos/relativity-data-transfer-legacy-sdk/browse/Source/DataTransfer.Legacy.FunctionalTests/CI/WebApiCompatibility"/>.
	/// </summary>
	[TestFixture(true)]
	[TestFixture(false)]
	public class KeplerDocumentManagerTests : KeplerServiceTestBase
	{
		public KeplerDocumentManagerTests(bool useKepler)
			: base(useKepler)
		{
		}

		[Test]
		public void ShouldRetrieveAllUnsupportedOiFileIds()
		{
			// arrange
			using (kCura.WinEDDS.Service.Replacement.IDocumentManager sut = ManagerFactory.CreateDocumentManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act
				int[] actualResult = sut.RetrieveAllUnsupportedOiFileIds();

				// assert
				Assert.That(actualResult, Is.Not.Null);
				Assert.That(actualResult, Is.Not.Empty);
			}
		}
	}
}