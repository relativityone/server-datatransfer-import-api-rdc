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
	using System.Net;

	using global::NUnit.Framework;

	using Relativity.DataExchange.Service;
	using Relativity.DataExchange.TestFramework;

	/// <summary>
	/// Represents <see cref="KeplerServiceProxyFactory"/> tests.
	/// </summary>
	[TestFixture]
	public static class KeplerServiceProxyFactoryTests
	{
		[Test]
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
	}
}