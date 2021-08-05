Imports System.Net
Imports kCura.WinEDDS
Imports kCura.WinEDDS.Service.Kepler
Imports Relativity.DataExchange.Io
Imports Relativity.DataExchange.Service
Imports Relativity.DataExchange.Service.RelativityDistributed
Imports Relativity.DataExchange.Service.WebApiVsKeplerSwitch
Imports Relativity.Logging

Namespace Relativity.DataExchange.Legacy.Service.RelativityDistributed
	Friend Class RelativityDistributedFacadeFactory

		Private ReadOnly reLoginService As IReLoginService

		Private ReadOnly logger As ILog

		Private ReadOnly settings As IAppSettings

		Private ReadOnly fileHelper As IFile

		Private ReadOnly authenticationTokenProvider As Func(Of String)

		Public Sub New(logger As ILog,
					   settings As IAppSettings,
					   reLoginService As IReLoginService,
					   fileHelper As IFile,
					   authenticationTokenProvider As Func(Of String))
			Me.reLoginService = reLoginService
			Me.logger = logger
			Me.settings = settings
			Me.fileHelper = fileHelper
			Me.authenticationTokenProvider = authenticationTokenProvider
		End Sub

		Public Function Create(downloadHandlerUrl As String,
							   credentials As NetworkCredential,
							   cookieContainer As CookieContainer,
							   correlationIdFunc As Func(Of String)) As IRelativityDistributedFacade
			Dim webApiServiceUrl As Uri = New Uri(Me.settings.WebApiServiceUrl)
			Dim webApiVsKeplerFactory As New WebApiVsKeplerFactory(logger)
			Dim webApiVsKepler As IWebApiVsKepler = webApiVsKeplerFactory.Create(webApiServiceUrl, credentials, correlationIdFunc)
			If webApiVsKepler.UseKepler() Then
				Return CreateKeplerDistributedFacade(webApiServiceUrl, credentials, correlationIdFunc)
			End If
			Return CreateRelativityDistributedFacade(credentials, downloadHandlerUrl, cookieContainer)
		End Function

		Private Function CreateKeplerDistributedFacade(webApiServiceUrl As Uri,
		                                               credentials As NetworkCredential,
													   correlationIdFunc As Func(Of String)) As IRelativityDistributedFacade
			Dim keplerProxy As IKeplerProxy = KeplerProxyFactory.CreateKeplerProxy(webApiServiceUrl, credentials)
			Dim facade As IRelativityDistributedFacade = New KeplerDistributedReplacement(
				keplerProxy,
				Me.fileHelper,
				Me.logger,
				correlationIdFunc)
			facade = New RelativityDistributedFacadeRetriesDecorator(
				Me.logger,
				Me.settings,
				facade)
			Return facade
		End Function

		Private Function CreateRelativityDistributedFacade(credentials As NetworkCredential,
														   downloadHandlerUrl As String,
														   cookieContainer As CookieContainer) As IRelativityDistributedFacade
			Dim facade As IRelativityDistributedFacade = New RelativityDistributedFacade(
				Me.logger,
				Me.settings,
				Me.fileHelper,
				downloadHandlerUrl,
				credentials,
				cookieContainer,
				Me.authenticationTokenProvider)

			facade = New RelativityDistributedFacadeAuthenticationDecorator(
				Me.logger,
				Me.settings,
				Me.reLoginService,
				facade)

			facade = New RelativityDistributedFacadeRetriesDecorator(
				Me.logger,
				Me.settings,
				facade)

			Return facade
		End Function
	End Class
End Namespace
