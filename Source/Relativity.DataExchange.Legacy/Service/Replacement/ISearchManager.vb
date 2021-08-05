Namespace kCura.WinEDDS.Service.Replacement
    Public Interface ISearchManager
        Inherits Export.ISearchManager
        Inherits IDisposable
        Shadows Function IsAssociatedSearchProviderAccessible(ByVal caseContextArtifactID As Int32, ByVal searchArtifactID As Int32) As Boolean()
        Shadows Function RetrieveNativesForSearch(ByVal caseContextArtifactID As Int32, ByVal documentArtifactIDs As String) As System.Data.DataSet
        Shadows Function RetrievePdfForSearch(ByVal caseContextArtifactID As Int32, ByVal documentArtifactIDs As String) As System.Data.DataSet
        Shadows Function RetrieveFilesForDynamicObjects(ByVal caseContextArtifactID As Int32, ByVal fileFieldArtifactID As Int32, ByVal objectIds As Int32()) As System.Data.DataSet
        Shadows Function RetrieveNativesForProduction(ByVal caseContextArtifactID As Int32, ByVal productionArtifactID As Int32, ByVal documentArtifactIDs As String) As System.Data.DataSet
        Shadows Function RetrieveImagesForSearch(ByVal caseContextArtifactID As Int32, ByVal documentArtifactIDs As Int32()) As System.Data.DataSet
        Shadows Function RetrieveProducedImagesForDocument(ByVal caseContextArtifactID As Int32, ByVal documentArtifactID As Int32) As System.Data.DataSet
        ''' <summary>
        ''' RetrieveImagesByProductionArtifactIDForProductionExportByDocumentSetAsync
        ''' </summary>
        Shadows Function RetrieveImagesForProductionDocuments(ByVal caseContextArtifactID As Int32, ByVal documentArtifactIDs As Int32(), ByVal productionArtifactID As Int32) As System.Data.DataSet
        Shadows Function RetrieveImagesByProductionIDsAndDocumentIDsForExport(ByVal caseContextArtifactID As Int32, ByVal productionArtifactIDs As Int32(), ByVal documentArtifactIDs As Int32()) As System.Data.DataSet
        Shadows Function RetrieveViewsByContextArtifactID(ByVal caseContextArtifactID As Int32, ByVal artifactTypeID As Int32, ByVal isSearch As Boolean) As System.Data.DataSet
        Shadows Function RetrieveDefaultViewFieldIds(ByVal caseContextArtifactID As Int32, ByVal viewArtifactID As Int32, ByVal artifactTypeID As Int32, ByVal isProduction As Boolean) As Int32()
        Shadows Function RetrieveAllExportableViewFields(ByVal caseContextArtifactID As Int32, ByVal artifactTypeID As Int32) As WinEDDS.ViewFieldInfo()
        Shadows Function RetrieveDefaultViewFieldsForIdList(ByVal caseContextArtifactID As Int32, ByVal artifactTypeID As Int32, ByVal artifactIdList As Int32(), ByVal isProductionList As Boolean) As System.Collections.Specialized.HybridDictionary
    End Interface
End Namespace
