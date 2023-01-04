Imports kCura.WinEDDS.Mapping
Imports Relativity.DataExchange.Service
Imports Relativity.DataTransfer.Legacy.SDK.ImportExport.V1

Namespace kCura.WinEDDS.Service.Replacement
    Public Class KeplerObjectTypeManager
        Inherits KeplerManager
        Implements IObjectTypeManager

        Public Sub New(serviceProxyFactory As IServiceProxyFactory, exceptionMapper As IServiceExceptionMapper, correlationIdFunc As Func(Of String))
            MyBase.New(serviceProxyFactory, exceptionMapper, correlationIdFunc)
        End Sub

        Public Function RetrieveAllUploadable(caseContextArtifactID As Integer) As DataSet Implements IObjectTypeManager.RetrieveAllUploadable
            Return Execute(Async Function(s)
                Using service As IObjectTypeService = s.CreateProxyInstance(Of IObjectTypeService)
                                   Return Await service.RetrieveAllUploadableAsync(caseContextArtifactID, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                               End Using
                           End Function)
        End Function

        Public Function RetrieveParentArtifactTypeID(caseContextArtifactID As Integer, artifactTypeID As Integer) As DataSet Implements IObjectTypeManager.RetrieveParentArtifactTypeID
            Return Execute(Async Function(s)
                               Using service As IObjectTypeService = s.CreateProxyInstance(Of IObjectTypeService)
                                   Return Await service.RetrieveParentArtifactTypeIDAsync(caseContextArtifactID, artifactTypeID, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                               End Using
            End Function)
        End Function
    End Class
End Namespace
