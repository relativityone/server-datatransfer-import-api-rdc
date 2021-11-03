// <copyright file="KeplerBulkImportExceptionMapperTests.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.NUnit.Integration.Mapping
{
	using System;
	using System.Net;
	using System.Web.Services.Protocols;

	using global::NUnit.Framework;

	using kCura.EDDS.WebAPI.BulkImportManagerBase;
	using kCura.WinEDDS.Mapping;
	using kCura.WinEDDS.Service.Replacement;

	using Relativity.DataExchange.Service;
	using Relativity.Testing.Identification;

	using BulkImportManager = kCura.WinEDDS.Service.BulkImportManager;

	[TestFixture]
	[Feature.DeveloperPlatform.ExtensibilityPoints.Api.Kepler]
	[TestType.MainFlow]
	public class KeplerBulkImportExceptionMapperTests : WebServiceTestsBase
	{
		[IdentifiedTest("7D3013C2-2A36-428E-A6D4-ED81CA1142FB")]
		public void BulkImportImageShouldGiveTheSameExceptionForWebApiAndKepler()
		{
			var appId = this.TestParameters.WorkspaceId;
			var settings = new ImageLoadInfo();
			var inRepository = false;

			BulkImportManager.BulkImportSqlException webApiException = null;
			BulkImportManager.BulkImportSqlException keplerException = null;

			// act - call WebApi
			this.CallWebApiExportManager(webApiBulkImportManager =>
				{
					webApiException = Assert.Throws<BulkImportManager.BulkImportSqlException>(() => webApiBulkImportManager.BulkImportImage(appId, settings, inRepository));
				});

			// act - call Kepler
			this.CallKeplerExportManager(keplerBulkImportManager =>
				{
					keplerException = Assert.Throws<BulkImportManager.BulkImportSqlException>(() => keplerBulkImportManager.BulkImportImage(appId, settings, inRepository));
				});

			// Assert
			ExceptionAssertHelper.EnsureWebApiAndKeplerExceptionsAreTheSame(webApiException, keplerException);
		}

		[IdentifiedTest("232013CA-DC74-4B95-BEA9-2B20C165AA05")]
		public void GenerateImageErrorFilesShouldGiveTheSameExceptionForWebApiAndKepler()
		{
			var appId = this.TestParameters.WorkspaceId;
			var keyFieldId = this.TestParameters.WorkspaceId;
			var importKey = "TestImportKey";
			var writeHeader = false;

			SoapException webApiException = null;
			SoapException keplerException = null;

			// act - call WebApi
			this.CallWebApiExportManager(webApiBulkImportManager =>
				{
					webApiException = Assert.Throws<SoapException>(() => webApiBulkImportManager.GenerateImageErrorFiles(appId, importKey, writeHeader, keyFieldId));
				});

			// act - call Kepler
			this.CallKeplerExportManager(keplerBulkImportManager =>
				{
					keplerException = Assert.Throws<SoapException>(() => keplerBulkImportManager.GenerateImageErrorFiles(appId, importKey, writeHeader, keyFieldId));
				});

			// Assert
			ExceptionAssertHelper.EnsureWebApiAndKeplerExceptionsAreTheSame(webApiException, keplerException);
		}

		private void CallWebApiExportManager(Action<BulkImportManager> action)
		{
			using (var webApiBulkImportManager = new BulkImportManager(this.RelativityInstance.Credentials, this.RelativityInstance.CookieContainer))
			{
				action?.Invoke(webApiBulkImportManager);
			}
		}

		private void CallKeplerExportManager(Action<KeplerBulkImportManager> action)
		{
			var keplerServiceConnectionInfo = new KeplerServiceConnectionInfo(this.RelativityInstance.WebApiServiceUrl, this.RelativityInstance.Credentials as NetworkCredential);
			using (var keplerServiceProxyFactory = new KeplerServiceProxyFactory(keplerServiceConnectionInfo))
			{
				using (var keplerBulkImportManager = new KeplerBulkImportManager(keplerServiceProxyFactory, new KeplerExceptionMapper(), this.RelativityInstance.Credentials as NetworkCredential, () => nameof(KeplerBulkImportExceptionMapperTests)))
				{
					action?.Invoke(keplerBulkImportManager);
				}
			}
		}
	}
}
