Imports System.Collections.Specialized
Imports kCura.WinEDDS.Mapping
Imports Relativity.DataExchange.Service
Imports Relativity.DataTransfer.Legacy.SDK.ImportExport.V1

Namespace kCura.WinEDDS.Service.Replacement
    Public Class KeplerSearchManager
        Inherits KeplerManager
        Implements ISearchManager

        Public Sub New(serviceProxyFactory As IServiceProxyFactory, typeMapper As ITypeMapper, exceptionMapper As IServiceExceptionMapper, correlationIdFunc As Func(Of String))
            MyBase.New(serviceProxyFactory, typeMapper, exceptionMapper, correlationIdFunc)
        End Sub

        Public Function IsAssociatedSearchProviderAccessible(caseContextArtifactID As Integer, searchArtifactID As Integer) As Boolean() Implements ISearchManager.IsAssociatedSearchProviderAccessible
            Return Execute(Async Function(s)
                Using service As ISearchService = s.CreateProxyInstance(Of ISearchService)
                                   Return Await service.IsAssociatedSearchProviderAccessibleAsync(caseContextArtifactID, searchArtifactID, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                               End Using
                           End Function)
        End Function

        Public Function RetrieveNativesForSearch(caseContextArtifactID As Integer, documentArtifactIDs As String) As DataSet Implements ISearchManager.RetrieveNativesForSearch, Export.ISearchManager.RetrieveNativesForSearch
            Return Execute(Async Function(s)
                               Using service As ISearchService = s.CreateProxyInstance(Of ISearchService)
                                   Return Await service.RetrieveNativesForSearchAsync(caseContextArtifactID, documentArtifactIDs, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                               End Using
                           End Function)
        End Function

        Public Function RetrievePdfForSearch(caseContextArtifactID As Integer, documentArtifactIDs As String) As DataSet Implements ISearchManager.RetrievePdfForSearch, Export.ISearchManager.RetrievePdfForSearch
            Return Execute(Async Function(s)
                               Using service As ISearchService = s.CreateProxyInstance(Of ISearchService)
                                   Return Await service.RetrievePdfForSearchAsync(caseContextArtifactID, documentArtifactIDs, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                               End Using
                           End Function)
        End Function

        Public Function RetrieveFilesForDynamicObjects(caseContextArtifactID As Integer, fileFieldArtifactID As Integer, objectIds() As Integer) As DataSet Implements ISearchManager.RetrieveFilesForDynamicObjects, Export.ISearchManager.RetrieveFilesForDynamicObjects
            Return Execute(Async Function(s)
                               Using service As ISearchService = s.CreateProxyInstance(Of ISearchService)
                                   Return Await service.RetrieveFilesForDynamicObjectsAsync(caseContextArtifactID, fileFieldArtifactID, objectIds, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                               End Using
                           End Function)
        End Function

        Public Function RetrieveNativesForProduction(caseContextArtifactID As Integer, productionArtifactID As Integer, documentArtifactIDs As String) As DataSet Implements ISearchManager.RetrieveNativesForProduction, Export.ISearchManager.RetrieveNativesForProduction
            Return Execute(Async Function(s)
                               Using service As ISearchService = s.CreateProxyInstance(Of ISearchService)
                                   Return Await service.RetrieveNativesForProductionAsync(caseContextArtifactID, productionArtifactID, documentArtifactIDs, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                               End Using
                           End Function)
        End Function

        Public Function RetrieveImagesForSearch(caseContextArtifactID As Integer, documentArtifactIDs() As Integer) As DataSet Implements ISearchManager.RetrieveImagesForSearch, Export.ISearchManager.RetrieveImagesForDocuments
            Return Execute(Async Function(s)
                               Using service As ISearchService = s.CreateProxyInstance(Of ISearchService)
                                   Return Await service.RetrieveImagesForSearchAsync(caseContextArtifactID, documentArtifactIDs, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                               End Using
                           End Function)
        End Function

        Public Function RetrieveProducedImagesForDocument(caseContextArtifactID As Integer, documentArtifactID As Integer) As DataSet Implements ISearchManager.RetrieveProducedImagesForDocument, Export.ISearchManager.RetrieveProducedImagesForDocument
            Return Execute(Async Function(s)
                               Using service As ISearchService = s.CreateProxyInstance(Of ISearchService)
                                   Return Await service.RetrieveProducedImagesForDocumentAsync(caseContextArtifactID, documentArtifactID, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                               End Using
                           End Function)
        End Function

        Public Function RetrieveImagesForProductionDocuments(caseContextArtifactID As Integer, documentArtifactIDs() As Integer, productionArtifactID As Integer) As DataSet Implements ISearchManager.RetrieveImagesForProductionDocuments, Export.ISearchManager.RetrieveImagesForProductionDocuments
            Return Execute(Async Function(s)
                               Using service As ISearchService = s.CreateProxyInstance(Of ISearchService)
                                   Return Await service.RetrieveImagesByProductionArtifactIDForProductionExportByDocumentSetAsync(caseContextArtifactID, productionArtifactID, documentArtifactIDs, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                               End Using
                           End Function)
        End Function

        Public Function RetrieveImagesByProductionIDsAndDocumentIDsForExport(caseContextArtifactID As Integer, productionArtifactIDs() As Integer, documentArtifactIDs() As Integer) As DataSet Implements ISearchManager.RetrieveImagesByProductionIDsAndDocumentIDsForExport, Export.ISearchManager.RetrieveImagesByProductionIDsAndDocumentIDsForExport
            Return Execute(Async Function(s)
                               Using service As ISearchService = s.CreateProxyInstance(Of ISearchService)
                                   Return Await service.RetrieveImagesByProductionIDsAndDocumentIDsForExportAsync(caseContextArtifactID, productionArtifactIDs, documentArtifactIDs, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                               End Using
                           End Function)
        End Function

        Public Function RetrieveViewsByContextArtifactID(caseContextArtifactID As Integer, artifactTypeID As Integer, isSearch As Boolean) As DataSet Implements ISearchManager.RetrieveViewsByContextArtifactID, Export.ISearchManager.RetrieveViewsByContextArtifactID
            Return Execute(Async Function(s)
                               Using service As ISearchService = s.CreateProxyInstance(Of ISearchService)
                                   Return Await service.RetrieveViewsByContextArtifactIDAsync(caseContextArtifactID, artifactTypeID, isSearch, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                               End Using
                           End Function)
        End Function

        Public Function RetrieveDefaultViewFieldIds(caseContextArtifactID As Integer, viewArtifactID As Integer, artifactTypeID As Integer, isProduction As Boolean) As Integer() Implements ISearchManager.RetrieveDefaultViewFieldIds, Export.ISearchManager.RetrieveDefaultViewFieldIds
            Dim dic As HybridDictionary = RetrieveDefaultViewFieldsForIdList(caseContextArtifactID, artifactTypeID, {viewArtifactID}, isProduction)
            Dim retval As New System.Collections.Generic.List(Of Int32)
            For Each id As Object In CType(dic(CType(viewArtifactID, Object)), ArrayList)
                retval.Add(Int32.Parse(id.ToString()))
            Next
            Return retval.ToArray()
        End Function

        Public Function RetrieveAllExportableViewFields(caseContextArtifactID As Integer, artifactTypeID As Integer) As ViewFieldInfo() Implements ISearchManager.RetrieveAllExportableViewFields, Export.ISearchManager.RetrieveAllExportableViewFields
            Dim dataSet As DataSet = Execute(Async Function(s)
                                                 Using service As ISearchService = s.CreateProxyInstance(Of ISearchService)
                                                     Return Await service.RetrieveAllExportableViewFieldsAsync(caseContextArtifactID, artifactTypeID, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                                                 End Using
                                             End Function)
            Dim retval As New System.Collections.ArrayList
            For Each row As System.Data.DataRow In dataSet.Tables(0).Rows
                retval.Add(New WinEDDS.ViewFieldInfo(row))
            Next
            Return DirectCast(retval.ToArray(GetType(WinEDDS.ViewFieldInfo)), WinEDDS.ViewFieldInfo())
        End Function

        Public Function RetrieveDefaultViewFieldsForIdList(caseContextArtifactID As Integer, artifactTypeID As Integer, artifactIdList() As Integer, isProductionList As Boolean) As HybridDictionary Implements ISearchManager.RetrieveDefaultViewFieldsForIdList
            Dim dataSet As DataSet = Execute(Async Function(s)
                                                 Using service As ISearchService = s.CreateProxyInstance(Of ISearchService)
                                                     Return Await service.RetrieveDefaultViewFieldsForIdListAsync(caseContextArtifactID, artifactTypeID, artifactIdList, isProductionList, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                                                 End Using
            End Function)
            Dim retval As New System.Collections.Specialized.HybridDictionary
            For Each row As System.Data.DataRow In dataSet.Tables(0).Rows
                If Not retval.Contains(row("ArtifactID")) Then
                    retval.Add(row("ArtifactID"), New ArrayList)
                End If
                DirectCast(retval(row("ArtifactID")), ArrayList).Add(row("ArtifactViewFieldID"))
            Next
            Return retval
        End Function
    End Class
End Namespace

