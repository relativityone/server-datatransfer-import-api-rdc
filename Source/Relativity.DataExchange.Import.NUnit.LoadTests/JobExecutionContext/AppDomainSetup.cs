// <copyright file="AppDomainSetup.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.Import.NUnit.LoadTests.JobExecutionContext
{
	using System;
	using System.Net;

	using Relativity.DataExchange.Import.NUnit.Integration;
	using Relativity.DataExchange.TestFramework;

	public class AppDomainSetup : MarshalByRefObject
	{
		/// <summary>
		/// This method initializes AppDomain to be used to run Import tests.
		/// </summary>
		/// <param name="parameters">test parameters.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "It cannot be static because we need to create an instance of that class in a proper AppDomain.")]
		public void SetupAppDomain(IntegrationTestParameters parameters)
		{
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
			ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
			AssemblySetup.TestParameters = parameters;
			IntegrationTestHelper.IntegrationTestParameters = parameters;
			IntegrationTestHelper.SetupLogger(parameters);
		}
	}
}
