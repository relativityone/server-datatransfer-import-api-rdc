// ----------------------------------------------------------------------------
// <copyright file="HttpClientHelperTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Relativity.Import.Export.NUnit.Integration
{
	using System;
	using System.Globalization;
	using System.Net;
	using System.Security.Policy;
	using System.Text;

	using global::NUnit.Framework;

	using Relativity.Import.Export.TestFramework;
	using Relativity.Testing.Identification;

	/// <summary>
	/// Represents Integration Tests for <see cref="HttpClientHelper"/> tests.
	/// </summary>
	[TestFixture]
	[Feature.DataTransfer.ImportApi]
	public class HttpClientHelperTests : WebServiceTestsBase
	{
		private static readonly Uri InstanceDetailsServiceRelPath = new Uri(
			"api/Relativity.Services.InstanceDetails.IInstanceDetailsModule/InstanceDetailsService/GetRelativityVersionAsync", UriKind.Relative);

		[Test]
		[Category(TestCategories.Integration)]
		[Category(TestCategories.WebService)]
		public void ShouldReadReadRelativityVersionFromKepler()
		{
			var subjectUnderTests = new HttpClientHelper();

			Version resultToTest = new Version();

			var value = string.Format(CultureInfo.InvariantCulture, "{0}:{1}", this.TestParameters.RelativityUserName, this.TestParameters.RelativityPassword);
			string basicHeader = string.Format(CultureInfo.InvariantCulture, "Basic {0}", Convert.ToBase64String(Encoding.ASCII.GetBytes(value)));
			var queryUrl = new Uri(this.TestParameters.RelativityRestUrl, InstanceDetailsServiceRelPath);

			var response = subjectUnderTests.DoPost(queryUrl, basicHeader, string.Empty);

			// Act
			string version = response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult()
				.TrimStart('"')
				.TrimEnd('"');

			// Assert
			Assert.That(response, Is.Not.Null);
			Assert.That(Version.TryParse(version, out resultToTest));
			Assert.That(resultToTest, Is.GreaterThanOrEqualTo(Version.Parse("10.3")));
		}
	}
}
