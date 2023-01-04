// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KeplerServiceProxyFactoryTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="KeplerServiceProxyFactory"/> tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit.Integration
{
	using System;
	using System.Net;
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using Relativity.DataExchange.Service;
	using Relativity.DataExchange.TestFramework;
	using Relativity.Services;
	using Relativity.Testing.Identification;

	/// <summary>
	/// Represents <see cref="KeplerServiceProxyFactory"/> tests.
	/// </summary>
	[TestFixture]
	[Feature.DeveloperPlatform.ExtensibilityPoints.Api.Kepler]
	[TestType.MainFlow]
	public static class KeplerServiceProxyFactoryTests
	{
		[IdentifiedTest("e31a2690-f6cc-43cc-b2c4-f65f64f4bd89")]
		public static void ShouldCreateAndDisposeTheServiceProxy()
		{
			// ARRANGE
			IntegrationTestParameters testParameters = AssemblySetup.TestParameters.DeepCopy();
			using (KeplerServiceProxyFactory serviceProxyFactory = new KeplerServiceProxyFactory(
				new KeplerServiceConnectionInfo(
					testParameters.RelativityWebApiUrl,
					new NetworkCredential(testParameters.RelativityUserName, testParameters.RelativityPassword))))
			{
				// ACT
				using (Relativity.Services.Client.IClientManager proxy =
					serviceProxyFactory.CreateProxyInstance<Relativity.Services.Client.IClientManager>())
				{
					// ASSERT
					Assert.That(proxy, Is.Not.Null);
				}
			}
		}

		[IdentifiedTest("e302f5a5-fc6b-4ff2-aa38-0a1971cc4a51")]
		public static async Task ShouldRefreshCredentialsBeforeCreatingProxyAsync()
		{
			// arrange
			IntegrationTestParameters testParameters = AssemblySetup.TestParameters.DeepCopy();
			var credentials = new NetworkCredential(
				testParameters.RelativityUserName,
				"WRONG_PASSWORD");
			var connectionInfo = new KeplerServiceConnectionInfo(testParameters.RelativityWebApiUrl, credentials);
			using (KeplerServiceProxyFactory serviceProxyFactory = new KeplerServiceProxyFactory(connectionInfo))
			{
				credentials.Password = testParameters.RelativityPassword;

				// act
				using (Relativity.Services.Client.IClientManager proxy = serviceProxyFactory.CreateProxyInstance<Relativity.Services.Client.IClientManager>())
				{
					// assert
					await proxy.QueryAsync(new Query()).ConfigureAwait(false);
					Assert.Pass("Valid credentials were used.");
				}
			}
		}

		[IdentifiedTest("ddd49a89-bfb0-425f-9e86-004fdb3b042a")]
		public static async Task ShouldUpdateCredentialsAsync()
		{
			// arrange
			IntegrationTestParameters testParameters = AssemblySetup.TestParameters.DeepCopy();
			var invalidCredentials = new NetworkCredential(
				testParameters.RelativityUserName,
				"WRONG_PASSWORD");
			var connectionInfo = new KeplerServiceConnectionInfo(testParameters.RelativityWebApiUrl, invalidCredentials);
			using (KeplerServiceProxyFactory serviceProxyFactory = new KeplerServiceProxyFactory(connectionInfo))
			{
				var validCredentials = new NetworkCredential(
					testParameters.RelativityUserName,
					testParameters.RelativityPassword);

				// act
				serviceProxyFactory.UpdateCredentials(validCredentials);

				// assert
				using (Relativity.Services.Client.IClientManager proxy = serviceProxyFactory.CreateProxyInstance<Relativity.Services.Client.IClientManager>())
				{
					await proxy.QueryAsync(new Query()).ConfigureAwait(false);
					Assert.Pass("Valid credentials were used.");
				}
			}
		}

		[IdentifiedTest("6C4BBEB5-28A7-4EB0-9305-C42E7252E951")]
		public static async Task ShouldExecutePostAsync()
		{
			// arrange
			IntegrationTestParameters testParameters = AssemblySetup.TestParameters.DeepCopy();
			var validCredentials = new NetworkCredential(
				testParameters.RelativityUserName,
				testParameters.RelativityPassword);
			var connectionInfo = new KeplerServiceConnectionInfo(testParameters.RelativityWebApiUrl, validCredentials);
			using (KeplerServiceProxyFactory serviceProxyFactory = new KeplerServiceProxyFactory(connectionInfo))
			{
				// act
				string relativityVersionString = await serviceProxyFactory.ExecutePostAsync(
					        @"/Relativity.Rest/api/Relativity.Services.InstanceDetails.IInstanceDetailsModule/InstanceDetailsService/GetRelativityVersionAsync",
					        string.Empty).ConfigureAwait(false);

				// assert
				Assert.That(relativityVersionString, Is.Not.Null);
				Assert.That(relativityVersionString, Is.Not.Empty);
			}
		}
	}
}