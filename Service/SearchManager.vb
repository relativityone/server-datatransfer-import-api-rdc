Imports kCura.WinEDDS.Service.Export

Namespace kCura.WinEDDS.Service
	Public Class SearchManager
		Inherits kCura.EDDS.WebAPI.SearchManagerBase.SearchManager
		Implements ISearchManager

		Public Sub New(ByVal credentials As Net.ICredentials, ByVal cookieContainer As System.Net.CookieContainer)
			MyBase.New()

			Me.Credentials = credentials
			Me.CookieContainer = cookieContainer
			Me.Url = String.Format("{0}SearchManager.asmx", kCura.WinEDDS.Config.WebServiceURL)
			Me.Timeout = Settings.DefaultTimeOut
		End Sub

		Protected Overrides Function GetWebRequest(ByVal uri As System.Uri) As System.Net.WebRequest
			Dim wr As System.Net.HttpWebRequest = DirectCast(MyBase.GetWebRequest(uri), System.Net.HttpWebRequest)
			wr.UnsafeAuthenticatedConnectionSharing = True
			wr.Credentials = Me.Credentials
			Return wr
		End Function

#Region " Shadow Functions "
		Public Shadows Function RetrieveNativesForProduction(ByVal caseContextArtifactID As Int32, ByVal productionArtifactID As Int32, ByVal documentArtifactIDs As String) As System.Data.DataSet Implements ISearchManager.RetrieveNativesForProduction
			Return RetryOnReLoginException(Function() MyBase.RetrieveNativesForProduction(caseContextArtifactID, productionArtifactID, documentArtifactIDs))
		End Function

		Public Shadows Function RetrieveNativesForSearch(ByVal caseContextArtifactID As Int32, ByVal documentArtifactIDs As String) As System.Data.DataSet Implements ISearchManager.RetrieveNativesForSearch
			Return RetryOnReLoginException(Function() MyBase.RetrieveNativesForSearch(caseContextArtifactID, documentArtifactIDs))
		End Function

		Public Shadows Function RetrieveFilesForDynamicObjects(ByVal caseContextArtifactID As Int32, ByVal fileFieldArtifactID As Int32, ByVal objectIds As Int32()) As System.Data.DataSet Implements ISearchManager.RetrieveFilesForDynamicObjects
			Return RetryOnReLoginException(Function() MyBase.RetrieveFilesForDynamicObjects(caseContextArtifactID, fileFieldArtifactID, objectIds))
		End Function

		Public Function RetrieveImagesForDocuments(ByVal caseContextArtifactID As Int32, ByVal documentArtifactIDs As Int32()) As System.Data.DataSet Implements ISearchManager.RetrieveImagesForDocuments
			Return RetryOnReLoginException(Function() RetrieveImagesForSearch(caseContextArtifactID, documentArtifactIDs))
		End Function

		Public Function RetrieveImagesForProductionDocuments(ByVal caseContextArtifactID As Int32, ByVal documentArtifactIDs As Int32(), ByVal productionArtifactID As Int32) As System.Data.DataSet Implements ISearchManager.RetrieveImagesForProductionDocuments
			Return RetryOnReLoginException(Function() Me.RetrieveImagesByProductionArtifactIDForProductionExportByDocumentSet(caseContextArtifactID, productionArtifactID, documentArtifactIDs))
		End Function

		Public Shadows Function RetrieveImagesForSearch(ByVal caseContextArtifactID As Int32, ByVal documentArtifactIDs As Int32()) As System.Data.DataSet
			Return RetryOnReLoginException(Function() MyBase.RetrieveImagesForSearch(caseContextArtifactID, documentArtifactIDs))
		End Function

		Public Shadows Function RetrieveImagesByProductionIDsAndDocumentIDsForExport(ByVal caseContextArtifactID As Int32, ByVal productionArtifactIDs As Int32(), ByVal documentArtifactIDs As Int32()) As System.Data.DataSet Implements ISearchManager.RetrieveImagesByProductionIDsAndDocumentIDsForExport
			Return RetryOnReLoginException(Function() MyBase.RetrieveImagesByProductionIDsAndDocumentIDsForExport(caseContextArtifactID, productionArtifactIDs, documentArtifactIDs))
		End Function

		Public Shadows Function RetrieveViewsByContextArtifactID(ByVal caseContextArtifactID As Int32, ByVal artifactTypeID As Int32, ByVal isSearch As Boolean) As System.Data.DataSet
			Return RetryOnReLoginException(Function() MyBase.RetrieveViewsByContextArtifactID(caseContextArtifactID, artifactTypeID, isSearch))
		End Function

		Public Shadows Function RetrieveSearchFields(ByVal caseContextArtifactID As Int32, ByVal viewArtifactID As Int32) As System.Data.DataSet
			Return RetryOnReLoginException(Function() MyBase.RetrieveSearchFields(caseContextArtifactID, viewArtifactID))
		End Function

		Public Shadows Function RetrieveSearchFieldsForProduction(ByVal caseContextArtifactID As Int32, ByVal productionArtifactID As Int32) As System.Data.DataSet
			Return RetryOnReLoginException(Function() MyBase.RetrieveSearchFieldsForProduction(caseContextArtifactID, productionArtifactID))
		End Function

		Public Shadows Function RetrieveDefaultViewFieldsForIdList(ByVal caseContextArtifactID As Int32, ByVal artifactTypeID As Int32, ByVal artifactIdList As Int32(), ByVal isProductionList As Boolean) As System.Collections.Specialized.HybridDictionary
			Return RetryOnReLoginException(
				Function()
					Dim dt As System.Data.DataTable = MyBase.RetrieveDefaultViewFieldsForIdList(caseContextArtifactID, artifactTypeID, artifactIdList, isProductionList).Tables(0)
					Dim retval As New System.Collections.Specialized.HybridDictionary
					For Each row As System.Data.DataRow In dt.Rows
						If Not retval.Contains(row("ArtifactID")) Then
							retval.Add(row("ArtifactID"), New ArrayList)
						End If
						DirectCast(retval(row("ArtifactID")), ArrayList).Add(row("ArtifactViewFieldID"))
					Next
					Return retval
				End Function)
		End Function

		Public Shadows Function RetrieveAllExportableViewFields(ByVal caseContextArtifactID As Int32, ByVal artifactTypeID As Int32) As WinEDDS.ViewFieldInfo() Implements ISearchManager.RetrieveAllExportableViewFields
			Return RetryOnReLoginException(
				Function()
					Dim dt As System.Data.DataTable = MyBase.RetrieveAllExportableViewFields(caseContextArtifactID, artifactTypeID).Tables(0)
					Dim retval As New System.Collections.ArrayList
					For Each row As System.Data.DataRow In dt.Rows
						retval.Add(New WinEDDS.ViewFieldInfo(row))
					Next
					Return DirectCast(retval.ToArray(GetType(WinEDDS.ViewFieldInfo)), WinEDDS.ViewFieldInfo())
				End Function)
		End Function

		Public Shadows Function RetrieveDefaultViewFieldIds(ByVal caseContextArtifactID As Int32, ByVal viewArtifactID As Int32, ByVal artifactTypeID As Int32, ByVal isProduction As Boolean) As Int32() Implements ISearchManager.RetrieveDefaultViewFieldIds
			Return RetryOnReLoginException(
				Function()
					Dim retval As New System.Collections.Generic.List(Of Int32)
					For Each id As Object In CType(RetrieveDefaultViewFieldsForIdList(caseContextArtifactID, artifactTypeID, {viewArtifactID}, isProduction)(CType(viewArtifactID, Object)), ArrayList)
						retval.Add(Int32.Parse(id.ToString()))
					Next
					Return retval.ToArray()
				End Function)
		End Function

		Public Shadows Function IsAssociatedSearchProviderAccessible(ByVal caseContextArtifactID As Int32, ByVal searchArtifactID As Int32) As Boolean()
			Return RetryOnReLoginException(Function() MyBase.IsAssociatedSearchProviderAccessible(caseContextArtifactID, searchArtifactID))
		End Function

#End Region

	End Class
End Namespace