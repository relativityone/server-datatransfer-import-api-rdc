Imports kCura.EDDS.WebAPI.FieldManagerBase
Imports kCura.WinEDDS.Mapping
Imports Relativity.DataExchange.Service

Namespace kCura.WinEDDS.Service.Replacement
    Public Class KeplerFieldManager
        Inherits KeplerManager
        Implements IFieldManager

        Public Sub New(serviceProxyFactory As IServiceProxyFactory, typeMapper As ITypeMapper, exceptionMapper As IServiceExceptionMapper, correlationIdFunc As Func(Of String))
            MyBase.New(serviceProxyFactory, typeMapper, exceptionMapper, correlationIdFunc)
        End Sub

        Public Function Read(caseContextArtifactID As Integer, fieldArtifactID As Integer) As Field Implements IFieldManager.Read, Export.IFieldManager.Read
            Return Execute(Async Function(s)
                               Using service As Global.Relativity.DataTransfer.Legacy.SDK.ImportExport.V1.IFieldService = s.CreateProxyInstance(Of Global.Relativity.DataTransfer.Legacy.SDK.ImportExport.V1.IFieldService)
                                   Dim result As Global.Relativity.DataTransfer.Legacy.SDK.ImportExport.V1.Models.Field = Await service.ReadAsync(caseContextArtifactID, fieldArtifactID, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                                   Return Map(Of Field)(result)
                               End Using
                           End Function)
        End Function
    End Class
End Namespace
