// <copyright file="KeplerExportExceptionMapperTests.cs" company="Relativity ODA LLC">
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
	using kCura.WinEDDS.Service.Replacement;

	using Relativity.DataExchange.Service;
	using Relativity.Testing.Identification;

	[TestFixture]
	[Feature.DeveloperPlatform.ExtensibilityPoints.Api.Kepler]
	[TestType.MainFlow]
	public class KeplerExportExceptionMapperTests : WebServiceTestsBase
	{
		[IdentifiedTest("C90661C1-A231-4A68-A47C-B96E0E290B71")]
		public void InitializeProductionExportShouldGiveTheSameExceptionForWebApiAndKepler()
		{
			var appId = this.TestParameters.WorkspaceId;
			var productionArtifactId = this.TestParameters.WorkspaceId;
			var avfIds = new int[0];
			var startRecord = 1;

			SoapException webApiException = null;
			SoapException keplerException = null;

			// act - call WebApi
			this.CallWebApiExportManager(webApiExportManager =>
				{
					webApiException = Assert.Throws<SoapException>(() => webApiExportManager.InitializeProductionExport(appId, productionArtifactId, avfIds, startRecord));
				});

			// act - call Kepler
			this.CallKeplerExportManager(keplerExportManager =>
				{
					keplerException = Assert.Throws<SoapException>(() => keplerExportManager.InitializeProductionExport(appId, productionArtifactId, avfIds, startRecord));
				});

			// Assert
			ExceptionAssertHelper.EnsureWebApiAndKeplerExceptionsAreTheSame(webApiException, keplerException);
		}

		[IdentifiedTest("4904F68B-2CF6-4EA1-A6C4-D064611ABDF0")]
		public void InitializeFolderExportShouldGiveTheSameExceptionForWebApiAndKepler()
		{
			var appId = this.TestParameters.WorkspaceId;
			var viewArtifactID = this.TestParameters.WorkspaceId;
			var parentArtifactID = this.TestParameters.WorkspaceId;
			var artifactTypeID = this.TestParameters.WorkspaceId;
			var includeSubFolders = false;
			var avfIds = new int[0];
			var startRecord = 1;

			SoapException webApiException = null;
			SoapException keplerException = null;

			// act - call WebApi
			this.CallWebApiExportManager(webApiExportManager =>
				{
					webApiException = Assert.Throws<SoapException>(() => webApiExportManager.InitializeFolderExport(appId, viewArtifactID, parentArtifactID, includeSubFolders, avfIds, startRecord, artifactTypeID));
				});

			// act - call Kepler
			this.CallKeplerExportManager(keplerExportManager =>
				{
					keplerException = Assert.Throws<SoapException>(() => keplerExportManager.InitializeFolderExport(appId, viewArtifactID, parentArtifactID, includeSubFolders, avfIds, startRecord, artifactTypeID));
				});

			// Assert
			ExceptionAssertHelper.EnsureWebApiAndKeplerExceptionsAreTheSame(webApiException, keplerException);
		}

		[IdentifiedTest("5C00DDED-6D89-4649-8D0E-678E4F90C7FA")]
		public void InitializeSearchExportShouldGiveTheSameExceptionForWebApiAndKepler()
		{
			var appId = this.TestParameters.WorkspaceId;
			var searchArtifactID = this.TestParameters.WorkspaceId;
			var avfIds = new int[0];
			var startRecord = 1;

			SoapException webApiException = null;
			SoapException keplerException = null;

			// act - call WebApi
			this.CallWebApiExportManager(webApiExportManager =>
				{
					webApiException = Assert.Throws<SoapException>(() => webApiExportManager.InitializeSearchExport(appId, searchArtifactID, avfIds, startRecord));
				});

			// act - call Kepler
			this.CallKeplerExportManager(keplerExportManager =>
				{
					keplerException = Assert.Throws<SoapException>(() => keplerExportManager.InitializeSearchExport(appId, searchArtifactID, avfIds, startRecord));
				});

			// Assert
			ExceptionAssertHelper.EnsureWebApiAndKeplerExceptionsAreTheSame(webApiException, keplerException);
		}

		private void CallWebApiExportManager(Action<ExportManager> action)
		{
			using (var webApiExportManager = new ExportManager(this.RelativityInstance.Credentials, this.RelativityInstance.CookieContainer))
			{
				action?.Invoke(webApiExportManager);
			}
		}

		private void CallKeplerExportManager(Action<KeplerExportManager> action)
		{
			var keplerServiceConnectionInfo = new KeplerServiceConnectionInfo(this.RelativityInstance.WebApiServiceUrl, this.RelativityInstance.Credentials as NetworkCredential);
			using (var keplerServiceProxyFactory = new KeplerServiceProxyFactory(keplerServiceConnectionInfo))
			{
				using (var keplerExportManager = new KeplerExportManager(keplerServiceProxyFactory, new KeplerTypeMapper(), new KeplerExceptionMapper(), () => nameof(KeplerExportExceptionMapperTests)))
				{
					action?.Invoke(keplerExportManager);
				}
			}
		}
	}
}
