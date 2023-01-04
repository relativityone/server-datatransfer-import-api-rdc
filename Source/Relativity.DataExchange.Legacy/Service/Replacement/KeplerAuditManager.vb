Imports kCura.EDDS.WebAPI.AuditManagerBase
Imports kCura.WinEDDS.Mapping
Imports Relativity.DataExchange.Service
Imports Relativity.DataTransfer.Legacy.SDK.ImportExport.V1

Namespace kCura.WinEDDS.Service.Replacement
    Public Class KeplerAuditManager
        Inherits KeplerManager
        Implements IAuditManager

        Public Sub New(serviceProxyFactory As IServiceProxyFactory, exceptionMapper As IServiceExceptionMapper, correlationIdFunc As Func(Of String))
            MyBase.New(serviceProxyFactory, exceptionMapper, correlationIdFunc)
        End Sub

        Public Sub DeleteAuditToken(token As String) Implements IAuditManager.DeleteAuditToken
            Execute(Async Function(s)
                        Using auditService As IAuditService = s.CreateProxyInstance(Of IAuditService)
                            Await auditService.DeleteAuditTokenAsync(token, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                            Return True
                        End Using
                    End Function)
        End Sub

        Public Function AuditImageImport(appID As Integer, runId As String, isFatalError As Boolean, importStats As ImageImportStatistics) As Boolean Implements IAuditManager.AuditImageImport
            Return Execute(Async Function(s)
                               Using auditService As IAuditService = s.CreateProxyInstance(Of IAuditService)
                                   Return Await auditService.AuditImageImportAsync(appID, runId, isFatalError, KeplerTypeMapper.Map(importStats), CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                               End Using
                           End Function)
        End Function

        Public Function AuditObjectImport(appID As Integer, runId As String, isFatalError As Boolean, importStats As ObjectImportStatistics) As Boolean Implements IAuditManager.AuditObjectImport
            Return Execute(Async Function(s)
                               Using auditService As IAuditService = s.CreateProxyInstance(Of IAuditService)
                                   Return Await auditService.AuditObjectImportAsync(appID, runId, isFatalError, KeplerTypeMapper.Map(importStats), CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                               End Using
                           End Function)
        End Function

        Public Function AuditExport(appID As Integer, isFatalError As Boolean, exportStats As EDDS.WebAPI.AuditManagerBase.ExportStatistics) As Boolean Implements IAuditManager.AuditExport, Export.IAuditManager.AuditExport
            Return Execute(Async Function(s)
                               Using auditService As IAuditService = s.CreateProxyInstance(Of IAuditService)
                                   Return Await auditService.AuditExportAsync(appID, isFatalError, KeplerTypeMapper.Map(exportStats), CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                               End Using
                           End Function)
        End Function
    End Class
End Namespace
