Imports kCura.WinEDDS.Mapping
Imports Relativity.DataExchange.Service
Imports Relativity.DataTransfer.Legacy.SDK.ImportExport.V1

Namespace kCura.WinEDDS.Service.Replacement
    Public Class KeplerDocumentManager
        Inherits KeplerManager
        Implements IDocumentManager

        Public Sub New(serviceProxyFactory As IServiceProxyFactory, typeMapper As ITypeMapper, exceptionMapper As IServiceExceptionMapper, correlationIdFunc As Func(Of String))
            MyBase.New(serviceProxyFactory, typeMapper, exceptionMapper, correlationIdFunc)
        End Sub

        Public Function RetrieveAllUnsupportedOiFileIds() As Integer() Implements IDocumentManager.RetrieveAllUnsupportedOiFileIds
            Return Execute(Async Function(s)
                Using service As IDocumentService = s.CreateProxyInstance(Of IDocumentService)
                                   Return Await service.RetrieveAllUnsupportedOiFileIdsAsync(CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                               End Using
            End Function)
        End Function
    End Class
End Namespace
