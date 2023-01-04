Namespace kCura.WinEDDS.Service.Replacement
    Public interface IProductionManager
        Inherits Export.IProductionManager
        Inherits IDisposable
        Shadows Sub DoPostImportProcessing(ByVal contextArtifactID As Int32, ByVal productionArtifactID As Int32)
        Shadows Sub DoPreImportProcessing(ByVal contextArtifactID As Int32, ByVal productionArtifactID As Int32)
        Shadows Function Read(ByVal caseContextArtifactID As Int32, ByVal productionArtifactID As Int32) As kCura.EDDS.WebAPI.ProductionManagerBase.ProductionInfo
        Shadows Function RetrieveBatesByProductionAndDocument(caseContextArtifactID As Int32, productionIds As Int32(), documentIds As Int32()) As Object()()
        Shadows Function RetrieveProducedByContextArtifactID(ByVal caseContextArtifactID As Int32) As System.Data.DataSet
        Shadows Function RetrieveImportEligibleByContextArtifactID(ByVal caseContextArtifactID As Int32) As System.Data.DataSet

    end interface
End NameSpace