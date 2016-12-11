Namespace kCura.WinEDDS.Service.Export
	Public Interface IProductionManager
		Function Read(ByVal caseContextArtifactID As Int32, ByVal productionArtifactID As Int32) As kCura.EDDS.WebAPI.ProductionManagerBase.ProductionInfo
		Function RetrieveProducedByContextArtifactID(ByVal caseContextArtifactID As Int32) As System.Data.DataSet
		Function RetrieveImportEligibleByContextArtifactID(ByVal caseContextArtifactID As Int32) As System.Data.DataSet
		Function RetrieveBatesByProductionAndDocument(ByVal caseContextArtifactID As Int32, productionIds As Int32(), documentIds As Int32()) As Object()()
	End Interface
End Namespace