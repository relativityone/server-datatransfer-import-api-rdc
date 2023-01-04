Imports System.Net
Imports kCura.WinEDDS.Service.Kepler
Imports Relativity.DataExchange
Imports Relativity.DataExchange.Logger
Imports Relativity.DataExchange.Service

Namespace kCura.WinEDDS

	Public Class KeplerProxyFactory
		Public Shared Function CreateKeplerProxy(webServiceUri As Uri, credentials As NetworkCredential) As IKeplerProxy
			Dim serviceProxyFactory As IServiceProxyFactory = Create(webServiceUri, credentials)
			Return New KeplerProxy(serviceProxyFactory, RelativityLogger.Instance)
		End Function

		Public Shared Function CreateKeplerProxy(credentials As NetworkCredential) As IKeplerProxy
			Dim webServiceUri As New Uri(AppSettings.Instance.WebApiServiceUrl)
			Dim serviceProxyFactory As IServiceProxyFactory = Create(webServiceUri, credentials)
			Return New KeplerProxy(serviceProxyFactory, RelativityLogger.Instance)
		End Function

		Private Shared Function Create(webServiceUri As Uri, credentials As NetworkCredential) As IServiceProxyFactory
			Dim connectionInfo As IServiceConnectionInfo = New KeplerServiceConnectionInfo(webServiceUri, credentials)
			Return New KeplerServiceProxyFactory(connectionInfo)
		End Function
	End Class

End Namespace
