Imports kCura.WinEDDS.Mapping
Imports Relativity.DataExchange.Service
Imports Relativity.DataTransfer.Legacy.SDK.ImportExport.V1

Namespace kCura.WinEDDS.Service.Replacement
    Public Class KeplerRelativityManager
        Inherits KeplerManager
        Implements IRelativityManager

        Public Sub New(serviceProxyFactory As IServiceProxyFactory, typeMapper As ITypeMapper, exceptionMapper As IServiceExceptionMapper, correlationIdFunc As Func(Of String))
            MyBase.New(serviceProxyFactory, typeMapper, exceptionMapper, correlationIdFunc)
        End Sub

        Public Function RetrieveCurrencySymbol() As String Implements IRelativityManager.RetrieveCurrencySymbol
            Return Execute(Async Function(s)
                Using service As IRelativityService = s.CreateProxyInstance(Of IRelativityService)
                                   Return Await service.RetrieveCurrencySymbolAsync(CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                               End Using
                           End Function)
        End Function

        Public Function IsImportEmailNotificationEnabled() As Boolean Implements IRelativityManager.IsImportEmailNotificationEnabled
            Return Execute(Async Function(s)
                               Using service As IRelativityService = s.CreateProxyInstance(Of IRelativityService)
                                   Return Await service.IsImportEmailNotificationEnabledAsync(CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                               End Using
                           End Function)
        End Function

        Public Function RetrieveRdcConfiguration() As DataSet Implements IRelativityManager.RetrieveRdcConfiguration
            Return Execute(Async Function(s)
                               Using service As IRelativityService = s.CreateProxyInstance(Of IRelativityService)
                                   Return Await service.RetrieveRdcConfigurationAsync(CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                               End Using
            End Function)
        End Function

        Public Function ValidateSuccessfulLogin() As Boolean Implements IRelativityManager.ValidateSuccessfulLogin
            Return False
        End Function
    End Class
End Namespace
