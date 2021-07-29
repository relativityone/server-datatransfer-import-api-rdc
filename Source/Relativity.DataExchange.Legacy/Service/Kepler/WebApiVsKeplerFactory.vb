Imports System.Net
Imports Relativity.DataExchange
Imports Relativity.DataExchange.Service
Imports Relativity.DataExchange.Service.WebApiVsKeplerSwitch
Imports Relativity.Logging

Namespace kCura.WinEDDS.Service.Kepler
	Public Class WebApiVsKeplerFactory
		Implements IWebApiVsKeplerFactory

		Private ReadOnly _logger As ILog

		Public Sub New(logger As ILog)
			Me._logger = logger
		End Sub

		Public Function Create(webApiUrl As Uri, credentials As NetworkCredential, getCorrelationId As Func(Of String)) As IWebApiVsKepler _
			Implements IWebApiVsKeplerFactory.Create

			Return New WebApiVsKeplerInternal(function() UseKepler(webApiUrl, credentials, getCorrelationId))
		End Function

		Private Function UseKepler(webApiUrl As Uri, credentials As NetworkCredential, getCorrelationId As Func(Of String)) As Boolean
			Dim connectionInfo As IServiceConnectionInfo = New KeplerServiceConnectionInfo(webApiUrl, credentials)

			Using serviceProxyFactory As New KeplerServiceProxyFactory(connectionInfo)
				Dim keplerProxy As IKeplerProxy = New KeplerProxy(serviceProxyFactory, Me._logger)
				Dim iApiCommunicationModeManager As IIAPICommunicationModeManager = New IAPICommunicationModeManager(keplerProxy, getCorrelationId)
				Dim serviceAvailabilityChecker As IServiceAvailabilityChecker = New ServiceAvailabilityChecker(iApiCommunicationModeManager)
				Dim webApiVsKepler As WebApiVsKepler = New WebApiVsKepler(serviceAvailabilityChecker)
				Return webApiVsKepler.UseKepler()
			End Using
		End Function

		Private Class WebApiVsKeplerInternal
			Implements IWebApiVsKepler

			Private ReadOnly _useKeplerLazy As Lazy(Of Boolean)

			Public Sub New(useKeplerValueFactory As Func(Of Boolean))
				Me._useKeplerLazy = New Lazy(Of Boolean)(useKeplerValueFactory)
			End Sub

			Private Function UseKepler() As Boolean Implements IWebApiVsKepler.UseKepler
				Return Me._useKeplerLazy.Value
			End Function
		End Class
	End Class
End Namespace

