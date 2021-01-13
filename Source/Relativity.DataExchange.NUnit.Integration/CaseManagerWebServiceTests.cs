// -----------------------------------------------------------------------------------------------------
// <copyright file="CaseManagerWebServiceTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="kCura.WinEDDS.Service.CaseManager"/> tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit.Integration
{
	using global::NUnit.Framework;

	using Relativity.DataExchange.Service;
	using Relativity.Testing.Identification;

	/// <summary>
	/// Represents <see cref="kCura.WinEDDS.Service.CaseManager"/> tests.
	/// </summary>
	[TestFixture]
	[Feature.DeveloperPlatform.ExtensibilityPoints.Api.WebApi]
	[TestType.MainFlow]
	public class CaseManagerWebServiceTests : WebServiceTestsBase
	{
		[IdentifiedTest("010641be-b75c-4eac-b871-eab2cf153790")]
		public void ShouldReadTheCaseInfo()
		{
			using (kCura.WinEDDS.Service.CaseManager caseManager = new kCura.WinEDDS.Service.CaseManager(
				this.RelativityInstance.Credentials,
				this.RelativityInstance.CookieContainer,
				this.RelativityInstance.WebApiServiceUrl.ToString(),
				DefaultTimeOutMilliseconds))
			{
				CaseInfo caseInfo = caseManager.Read(this.TestParameters.WorkspaceId);
				Assert.That(caseInfo, Is.Not.Null);
				Assert.That(caseInfo.ArtifactID, Is.Positive);
				Assert.That(caseInfo.DocumentPath, Is.Not.Empty.Or.Null);
				Assert.That(caseInfo.DownloadHandlerURL, Is.Not.Empty.Or.Null);
				Assert.That(caseInfo.MatterArtifactID, Is.Positive);
				Assert.That(caseInfo.Name, Is.Not.Empty.Or.Null);
			}
		}
	}
}