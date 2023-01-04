Imports kCura.WinEDDS.Mapping
Imports Relativity.DataExchange.Service
Imports Relativity.DataTransfer.Legacy.SDK.ImportExport.V1

Namespace kCura.WinEDDS.Service.Replacement
    Public Class KeplerObjectManager
        Inherits KeplerManager
        Implements IObjectManager

        Public Sub New(serviceProxyFactory As IServiceProxyFactory, exceptionMapper As IServiceExceptionMapper, correlationIdFunc As Func(Of String))
            MyBase.New(serviceProxyFactory, exceptionMapper, correlationIdFunc)
        End Sub

        Public Function RetrieveArtifactIdOfMappedObject(caseContextArtifactID As Integer, textIdentifier As String, artifactTypeID As Integer) As DataSet Implements IObjectManager.RetrieveArtifactIdOfMappedObject
            Return Execute(Async Function(s)
                Using service As IObjectService = s.CreateProxyInstance(Of IObjectService)
                                   Return Await service.RetrieveArtifactIdOfMappedObjectAsync(caseContextArtifactID, textIdentifier, artifactTypeID, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                               End Using
                           End Function)
        End Function

        Public Function RetrieveTextIdentifierOfMappedObject(caseContextArtifactID As Integer, artifactId As Integer, artifactTypeID As Integer) As DataSet Implements IObjectManager.RetrieveTextIdentifierOfMappedObject
            Return Execute(Async Function(s)
                               Using service As IObjectService = s.CreateProxyInstance(Of IObjectService)
                                   Return Await service.RetrieveTextIdentifierOfMappedObjectAsync(caseContextArtifactID, artifactId, artifactTypeID, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                               End Using
                           End Function)
        End Function

        Public Function RetrieveArtifactIdOfMappedParentObject(caseContextArtifactID As Integer, textIdentifier As String, artifactTypeID As Integer) As DataSet Implements IObjectManager.RetrieveArtifactIdOfMappedParentObject
            Return Execute(Async Function(s)
                               Using service As IObjectService = s.CreateProxyInstance(Of IObjectService)
                                   Return Await service.RetrieveArtifactIdOfMappedParentObjectAsync(caseContextArtifactID, textIdentifier, artifactTypeID, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                               End Using
            End Function)
        End Function
    End Class
End Namespace
