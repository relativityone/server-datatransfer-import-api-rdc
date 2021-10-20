Imports System.Collections.Concurrent
Imports System.Net
Imports Relativity.DataExchange
Imports Relativity.DataExchange.Service
Imports Relativity.DataExchange.Service.WebApiVsKeplerSwitch
Imports Relativity.Logging

Namespace kCura.WinEDDS.Service.Kepler
	Public Class WebApiVsKeplerFactory
		Implements IWebApiVsKeplerFactory

		Private ReadOnly Shared Cache As ConcurrentDictionary(Of String, IWebApiVsKepler) = New ConcurrentDictionary(Of String,IWebApiVsKepler)()

		Private ReadOnly _logger As ILog

		Public Sub New(logger As ILog)
			Me._logger = logger
		End Sub

		Public Function Create(webApiUrl As Uri, credentials As NetworkCredential, getCorrelationId As Func(Of String)) As IWebApiVsKepler _
			Implements IWebApiVsKeplerFactory.Create
			
			Dim cacheKey As String = $"WebApiVsKepler." + webApiUrl.ToString()
			Dim instance As IWebApiVsKepler = Cache.GetOrAdd(
				cacheKey, 
				function(x) New WebApiVsKeplerInternal(function() UseKepler(webApiUrl, credentials, getCorrelationId)))
			Return Instance
		End Function

		Public Shared Sub InvalidateCache()
			Cache.Clear()
		End Sub

		Private Function UseKepler(webApiUrl As Uri, credentials As NetworkCredential, getCorrelationId As Func(Of String)) As Boolean
			Dim connectionInfo As IServiceConnectionInfo = New KeplerServiceConnectionInfo(webApiUrl, credentials)

			Using serviceProxyFactory As New KeplerServiceProxyFactory(connectionInfo, Me._logger)
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

