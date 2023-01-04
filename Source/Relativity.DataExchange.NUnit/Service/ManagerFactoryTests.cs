// <copyright file="ManagerFactoryTests.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.NUnit.Service
{
	using System;
	using System.Net;

	using global::NUnit.Framework;

	using kCura.WinEDDS.Service;

	[TestFixture]
	public class ManagerFactoryTests
	{
		private const string LocalhostWebApiUrl = "https://localhost/RelativityWebApi";
		private const string LocalhostBaseUrl = "https://localhost/";
		private const string RemoteWebApiUrl = "https://remote/RelativityWebApi";
		private const string RemoteBaseUrl = "https://remote/";

		private readonly NetworkCredential emptyCredentials = new NetworkCredential();

		[SetUp]
		public void SetUp()
		{
			ManagerFactory.InvalidateCache();
		}

		[Test]
		public void ShouldUseWebServiceUrlFromAppSettingsWhenValueNotProvided()
		{
			// arrange
			AppSettings.Instance.ProgrammaticWebApiServiceUrl = LocalhostWebApiUrl;

			// act
			ManagerFactory.Initialize(this.emptyCredentials, null);

			// assert
			var expectedWebServiceUrl = new Uri(LocalhostBaseUrl);
			Assert.That(ManagerFactory._connectionInfo, Is.Not.Null);
			Assert.That(ManagerFactory._connectionInfo.WebServiceBaseUrl, Is.EqualTo(expectedWebServiceUrl));
		}

		[Test]
		public void ShouldUseProvidedWebServiceUrl()
		{
			// arrange
			AppSettings.Instance.ProgrammaticWebApiServiceUrl = LocalhostWebApiUrl;

			// act
			ManagerFactory.Initialize(this.emptyCredentials, null, RemoteWebApiUrl);

			// assert
			var expectedWebServiceUrl = new Uri(RemoteBaseUrl);
			Assert.That(ManagerFactory._connectionInfo, Is.Not.Null);
			Assert.That(ManagerFactory._connectionInfo.WebServiceBaseUrl, Is.EqualTo(expectedWebServiceUrl));
		}

		[Test]
		public void ShouldReinitializeWhenDifferentWebServiceUrlWasProvided()
		{
			// arrange
			AppSettings.Instance.ProgrammaticWebApiServiceUrl = LocalhostWebApiUrl;
			ManagerFactory.Initialize(this.emptyCredentials, null);

			// act
			ManagerFactory.Initialize(this.emptyCredentials, null, RemoteWebApiUrl);

			// assert
			var expectedWebServiceUrl = new Uri(RemoteBaseUrl);
			Assert.That(ManagerFactory._connectionInfo, Is.Not.Null);
			Assert.That(ManagerFactory._connectionInfo.WebServiceBaseUrl, Is.EqualTo(expectedWebServiceUrl));
		}

		[Test]
		public void ShouldReinitializeWhenWebServiceUrlInAppConfigHasChanged()
		{
			// arrange
			AppSettings.Instance.ProgrammaticWebApiServiceUrl = LocalhostWebApiUrl;
			ManagerFactory.Initialize(this.emptyCredentials, null);
			AppSettings.Instance.ProgrammaticWebApiServiceUrl = RemoteWebApiUrl;

			// act
			ManagerFactory.Initialize(this.emptyCredentials, null);

			// assert
			var expectedWebServiceUrl = new Uri(RemoteBaseUrl);
			Assert.That(ManagerFactory._connectionInfo, Is.Not.Null);
			Assert.That(ManagerFactory._connectionInfo.WebServiceBaseUrl, Is.EqualTo(expectedWebServiceUrl));
		}

		[Test]
		public void ShouldReinitializeWhenDifferentCredentialsWereProvided()
		{
			// arrange
			AppSettings.Instance.ProgrammaticWebApiServiceUrl = LocalhostWebApiUrl;
			ManagerFactory.Initialize(this.emptyCredentials, null);

			var usernamePasswordCredentials = new NetworkCredential("admin", "1234");

			// act
			ManagerFactory.Initialize(usernamePasswordCredentials, null);

			// assert
			Assert.That(ManagerFactory._connectionInfo, Is.Not.Null);
			Assert.That(ManagerFactory._currentCredentials, Is.SameAs(usernamePasswordCredentials));
		}
	}
}
