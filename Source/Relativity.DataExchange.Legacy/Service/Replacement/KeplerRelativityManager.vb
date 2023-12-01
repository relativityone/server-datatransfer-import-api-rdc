Imports kCura.WinEDDS.Mapping
Imports Relativity.DataExchange.Service
Imports Relativity.DataTransfer.Legacy.SDK.ImportExport.V1

Namespace kCura.WinEDDS.Service.Replacement
    Public Class KeplerRelativityManager
        Inherits KeplerManager
        Implements IRelativityManager

        Public Sub New(serviceProxyFactory As IServiceProxyFactory, exceptionMapper As IServiceExceptionMapper, correlationIdFunc As Func(Of String))
            MyBase.New(serviceProxyFactory, exceptionMapper, correlationIdFunc)
        End Sub

        Public Function RetrieveCurrencySymbol() As String Implements IRelativityManager.RetrieveCurrencySymbol
            Return Execute(Async Function(s)
                               Using service As IRelativityService = s.CreateProxyInstance(Of IRelativityService)
                                   Return Await service.RetrieveCurrencySymbolAsync(CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                               End Using
                           End Function)
        End Function

        Public Function RetrieveCurrencySymbolV2() As String Implements IRelativityManager.RetrieveCurrencySymbolV2
            Return Execute(Async Function(s)
                               Using service As IRelativityService = s.CreateProxyInstance(Of IRelativityService)
                                   Return Await service.RetrieveCurrencySymbolV2Async(CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
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

        Public Function ValidateCertificate() As Boolean Implements IRelativityManager.ValidateCertificate
            ' We use here ExecuteWithoutRetries as we do not want to wait multiple retries to check if url or cert is valid
            ExecuteWithoutRetries(Async Function(s)
                                      Using service As IRelativityService = s.CreateProxyInstance(Of IRelativityService)
                                          Return Await service.RetrieveCurrencySymbolAsync(CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                                      End Using
                                  End Function)
            Return False ' The return value is not important for Kepler, we expect this call to fail with specific exception
        End Function
    End Class
End Namespace
