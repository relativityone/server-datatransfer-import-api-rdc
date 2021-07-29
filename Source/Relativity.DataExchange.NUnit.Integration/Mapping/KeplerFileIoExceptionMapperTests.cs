// <copyright file="KeplerFileIoExceptionMapperTests.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.NUnit.Integration.Mapping
{
	using System;
	using System.Net;
	using System.Web.Services.Protocols;

	using global::NUnit.Framework;

	using kCura.WinEDDS.Mapping;
	using kCura.WinEDDS.Service;

	using Relativity.DataExchange.Service;
	using Relativity.Testing.Identification;

	[TestFixture]
	[Feature.DeveloperPlatform.ExtensibilityPoints.Api.Kepler]
	[TestType.MainFlow]
	public class KeplerFileIoExceptionMapperTests : WebServiceTestsBase
	{
		[IdentifiedTest("836BD458-670C-409C-ACA4-19ED30AA77B5")]
		public void ValidateBcpShareShouldGiveTheSameExceptionForWebApiAndKepler()
		{
			var appId = 1111;

			SoapException webApiException;
			SoapException keplerException;

			// act - call WebApi
			using (var webApiFileIo = new FileIO(this.RelativityInstance.Credentials, this.RelativityInstance.CookieContainer))
			{
				webApiException = Assert.Throws<SoapException>(() => webApiFileIo.ValidateBcpShare(appId));
			}

			// act - call Kepler
			var keplerServiceConnectionInfo = new KeplerServiceConnectionInfo(this.RelativityInstance.WebApiServiceUrl, this.RelativityInstance.Credentials as NetworkCredential);
			using (var keplerServiceProxyFactory = new KeplerServiceProxyFactory(keplerServiceConnectionInfo))
			{
				using (var keplerFileIo = new KeplerFileIO(keplerServiceProxyFactory, new KeplerTypeMapper(), new KeplerExceptionMapper(), () => nameof(KeplerExportExceptionMapperTests)))
				{
					keplerException = Assert.Throws<SoapException>(() => keplerFileIo.ValidateBcpShare(appId));
				}
			}

			// Assert
			ExceptionAssertHelper.EnsureWebApiAndKeplerExceptionsAreTheSame(webApiException, keplerException);
		}
	}
}
