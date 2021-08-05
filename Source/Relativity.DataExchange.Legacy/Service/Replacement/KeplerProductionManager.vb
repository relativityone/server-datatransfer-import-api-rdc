Imports kCura.WinEDDS.Mapping
Imports Relativity.DataExchange.Service
Imports Relativity.DataTransfer.Legacy.SDK.ImportExport.V1
Imports Relativity.DataTransfer.Legacy.SDK.ImportExport.V1.Models

Namespace kCura.WinEDDS.Service.Replacement
    Public Class KeplerProductionManager
        Inherits KeplerManager
        Implements IProductionManager

        Public Sub New(serviceProxyFactory As IServiceProxyFactory, typeMapper As ITypeMapper, exceptionMapper As IServiceExceptionMapper, correlationIdFunc As Func(Of String))
            MyBase.New(serviceProxyFactory, typeMapper, exceptionMapper, correlationIdFunc)
        End Sub

        Public Sub DoPostImportProcessing(contextArtifactID As Integer, productionArtifactID As Integer) Implements IProductionManager.DoPostImportProcessing
            Execute(Async Function(s)
                Using service As IProductionService = s.CreateProxyInstance(Of IProductionService)
                            Await service.DoPostImportProcessingAsync(contextArtifactID, productionArtifactID, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                            Return True
                        End Using
                    End Function)
        End Sub

        Public Sub DoPreImportProcessing(contextArtifactID As Integer, productionArtifactID As Integer) Implements IProductionManager.DoPreImportProcessing
            Execute(Async Function(s)
                        Using service As IProductionService = s.CreateProxyInstance(Of IProductionService)
                            Await service.DoPreImportProcessingAsync(contextArtifactID, productionArtifactID, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                            Return True
                        End Using
                    End Function)
        End Sub

        Public Function Read(caseContextArtifactID As Integer, productionArtifactID As Integer) As kCura.EDDS.WebAPI.ProductionManagerBase.ProductionInfo Implements IProductionManager.Read, Export.IProductionManager.Read
            Return Execute(Async Function(s)
                               Using service As IProductionService = s.CreateProxyInstance(Of IProductionService)
                                   Dim result As Models.ProductionInfo = Await service.ReadAsync(caseContextArtifactID, productionArtifactID, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                                   Return Map(Of kCura.EDDS.WebAPI.ProductionManagerBase.ProductionInfo)(result)
                               End Using
                           End Function)
        End Function

        Public Function RetrieveBatesByProductionAndDocument(caseContextArtifactID As Integer, productionIds() As Integer, documentIds() As Integer) As Object()() Implements IProductionManager.RetrieveBatesByProductionAndDocument, Export.IProductionManager.RetrieveBatesByProductionAndDocument
            Return Execute(Async Function(s)
                               Using service As IProductionService = s.CreateProxyInstance(Of IProductionService)
                                   Dim wrappedResult As ExportDataWrapper = Await service.RetrieveBatesByProductionAndDocumentAsync(caseContextArtifactID, productionIds, documentIds, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                                   Dim unwrappedResult As Object()() = wrappedResult.Unwrap()
                                   ProductionDocumentBatesHelper.CleanupSerialization(unwrappedResult)
                                   Return unwrappedResult
                               End Using
                           End Function)
        End Function

        Public Function RetrieveProducedByContextArtifactID(caseContextArtifactID As Integer) As DataSet Implements IProductionManager.RetrieveProducedByContextArtifactID, Export.IProductionManager.RetrieveProducedByContextArtifactID
            Return Execute(Async Function(s)
                               Using service As IProductionService = s.CreateProxyInstance(Of IProductionService)
                                   Return Await service.RetrieveProducedByContextArtifactIDAsync(caseContextArtifactID, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                               End Using
                           End Function)
        End Function

        Public Function RetrieveImportEligibleByContextArtifactID(caseContextArtifactID As Integer) As DataSet Implements IProductionManager.RetrieveImportEligibleByContextArtifactID, Export.IProductionManager.RetrieveImportEligibleByContextArtifactID
            Return Execute(Async Function(s)
                               Using service As IProductionService = s.CreateProxyInstance(Of IProductionService)
                                   Return Await service.RetrieveImportEligibleByContextArtifactIDAsync(caseContextArtifactID, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                               End Using
            End Function)
        End Function
        
    End Class
End Namespace
