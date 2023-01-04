// <copyright file="RelativityManagerServiceFactoryTests.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.NUnit.Service
{
	using System;
	using System.Net;

	using global::NUnit.Framework;

	using Relativity.DataExchange.Service;

	[TestFixture]
	public class RelativityManagerServiceFactoryTests
	{
		private RelativityManagerServiceFactory sut;
		private RelativityInstanceInfo instanceInfo;

		[SetUp]
		public void SetUp()
		{
			this.instanceInfo = new RelativityInstanceInfo
			{
				WebApiServiceUrl = new Uri("https://iapi.relativity.com"),
				Credentials = new NetworkCredential(),
				CookieContainer = new CookieContainer(),
			};

			this.sut = new RelativityManagerServiceFactory();
		}

		[TestCase(true)]
		[TestCase(false)]
		public void ShouldThrowArgumentNullExceptionWhenInstanceInfoIsNull(bool useLegacyWebApi)
		{
			Assert.That(() => this.sut.Create(null, useLegacyWebApi), Throws.ArgumentNullException);
		}

		[Test]
		public void ShouldCreateWebApiRelativityManagerService()
		{
			// act
			var actualService = this.sut.Create(this.instanceInfo, useLegacyWebApi: true);

			// assert
			Assert.That(actualService, Is.InstanceOf<RelativityManagerService>());
		}

		[Test]
		public void ShouldCreateKeplerRelativityManagerService()
		{
			// act
			var actualService = this.sut.Create(this.instanceInfo, useLegacyWebApi: false);

			// assert
			Assert.That(actualService, Is.InstanceOf<KeplerRelativityManagerService>());
		}
	}
}
