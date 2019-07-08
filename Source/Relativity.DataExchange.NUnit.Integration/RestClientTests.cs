// ----------------------------------------------------------------------------
// <copyright file="RestClientTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit.Integration
{
	using System;
	using System.Threading;
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using Relativity.DataExchange;
	using Relativity.DataExchange.TestFramework;
	using Relativity.Testing.Identification;

	/// <summary>
	/// Represents Integration Tests for <see cref="RestClient"/> tests.
	/// </summary>
	[TestFixture]
	[Feature.DataTransfer.ImportApi]
	public class RestClientTests : WebServiceTestsBase
	{
		private const string InstanceDetailsServiceRelPath =
			"/Relativity.Rest/api/Relativity.Services.InstanceDetails.IInstanceDetailsModule/InstanceDetailsService/GetRelativityVersionAsync";

		[IdentifiedTest("38dc313f-a8d4-48ae-b1c5-6cde45305608")]
		[Category(TestCategories.Integration)]
		[Category(TestCategories.WebService)]
		public async Task ShouldGetTheRelativityVersionFromKeplerAsync()
		{
			var subjectUnderTests = new RestClient(this.RelativityInstance, this.Logger.Object, 30, 3);
			var response = await subjectUnderTests.RequestPostStringAsync(
				               InstanceDetailsServiceRelPath,
				               string.Empty,
				               i => { return TimeSpan.FromSeconds(15); },
				               (exception, span, arg3) => { this.Logger.Object.LogError("Failed"); },
				               code => { return "Failed"; },
				               code => { return "Failed"; },
				               CancellationToken.None).ConfigureAwait(false);

			// Act
			string version = response.TrimStart('"').TrimEnd('"');

			// Assert
			Assert.That(response, Is.Not.Null);
			Assert.That(Version.TryParse(version, out var resultToTest));
			Assert.That(resultToTest, Is.GreaterThanOrEqualTo(Version.Parse("10.3")));
		}
	}
}