Namespace kCura.WinEDDS.Service.Export
	Public Interface ISearchManager
        Inherits IDisposable
		Function RetrieveNativesForSearch(ByVal caseContextArtifactID As Int32, ByVal documentArtifactIDs As String) As System.Data.DataSet
		Function RetrieveNativesForProduction(ByVal caseContextArtifactID As Int32, ByVal productionArtifactID As Int32, ByVal documentArtifactIDs As String) As System.Data.DataSet

		Function RetrieveFilesForDynamicObjects(ByVal caseContextArtifactID As Int32, ByVal fileFieldArtifactID As Int32, ByVal objectIds As Int32()) As System.Data.DataSet
		Function RetrieveImagesForProductionDocuments(ByVal caseContextArtifactID As Int32, ByVal documentArtifactIDs As Int32(), ByVal productionArtifactID As Int32) As System.Data.DataSet
		Function RetrieveImagesForDocuments(ByVal caseContextArtifactID As Int32, ByVal documentArtifactIDs As Int32()) As System.Data.DataSet
		Function RetrieveImagesByProductionIDsAndDocumentIDsForExport(ByVal caseContextArtifactID As Int32, ByVal productionArtifactIDs As Int32(), ByVal documentArtifactIDs As Int32()) As System.Data.DataSet
		Function RetrieveAllExportableViewFields(ByVal caseContextArtifactID As Int32, ByVal artifactTypeID As Int32) As WinEDDS.ViewFieldInfo()
		Function RetrieveDefaultViewFieldIds(ByVal caseContextArtifactID As Int32, ByVal viewArtifactID As Int32, ByVal artifactTypeID As Int32, ByVal isProduction As Boolean) As Int32()
		Function RetrieveViewsByContextArtifactID(ByVal caseContextArtifactID As Int32, ByVal artifactTypeID As Int32, ByVal isSearch As Boolean) As System.Data.DataSet
	End Interface
End Namespace
