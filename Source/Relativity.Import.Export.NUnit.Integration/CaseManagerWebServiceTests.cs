// -----------------------------------------------------------------------------------------------------
// <copyright file="CaseManagerWebServiceTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="kCura.WinEDDS.Service.CaseManager"/> tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Import.Export.NUnit.Integration
{
	using global::NUnit.Framework;

	using Relativity.Import.Export.TestFramework;
	using Relativity.Testing.Identification;

	/// <summary>
	/// Represents <see cref="kCura.WinEDDS.Service.CaseManager"/> tests.
	/// </summary>
	[TestFixture]
	[Feature.DataTransfer.ImportApi]
	public class CaseManagerWebServiceTests : WebServiceTestsBase
	{
		[Test]
		[Category(TestCategories.Integration)]
		[Category(TestCategories.WebService)]
		public void ShouldReadTheCaseInfo()
		{
			using (kCura.WinEDDS.Service.CaseManager caseManager = new kCura.WinEDDS.Service.CaseManager(
				this.RelativityInstance.Credentials,
				this.RelativityInstance.CookieContainer,
				this.RelativityInstance.WebApiServiceUrl.ToString(),
				DefaultTimeOutMilliseconds))
			{
				Relativity.Import.Export.Services.CaseInfo caseInfo = caseManager.Read(this.TestParameters.WorkspaceId);
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