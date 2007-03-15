Namespace kCura.WinEDDS.Service
	Public Class SearchManager
		Inherits kCura.EDDS.WebAPI.SearchManagerBase.SearchManager

		'Private _searchManager As New kCura.EDDS.Service.SearchManager
		'Private _viewManager As New kCura.EDDS.Service.ViewManager
		'Private _identity As kCura.EDDS.EDDSIdentity

		'Public Sub New(ByVal credentials As Net.NetworkCredential, ByVal cookieContainer As System.Net.CookieContainer, ByVal identity As kCura.EDDS.EDDSIdentity)
		Public Sub New(ByVal credentials As Net.NetworkCredential, ByVal cookieContainer As System.Net.CookieContainer)
			MyBase.New()
			Me.Credentials = credentials
			Me.CookieContainer = cookieContainer
			Me.Url = String.Format("{0}SearchManager.asmx", kCura.WinEDDS.Config.WebServiceURL)
			Me.Timeout = Settings.DefaultTimeOut
			'_identity = identity
		End Sub

		Protected Overrides Function GetWebRequest(ByVal uri As System.Uri) As System.Net.WebRequest
			Dim wr As System.Net.HttpWebRequest = DirectCast(MyBase.GetWebRequest(uri), System.Net.HttpWebRequest)
			wr.UnsafeAuthenticatedConnectionSharing = True
			wr.Credentials = Me.Credentials
			Return wr
		End Function

#Region " Shadow Functions "
		Public Shadows Function CountSearchByArtifactID(ByVal caseContextArtifactID As Int32, ByVal searchArtifactID As Int32) As Int32
			If kCura.WinEDDS.Config.UsesWebAPI Then
				Return MyBase.CountSearchByArtifactID(caseContextArtifactID, searchArtifactID)
			Else
				'Return _searchManager.SearchByArtifactIDAsDataSet(_identity, searchArtifactID)
			End If
		End Function

		Public Shadows Function SearchBySearchArtifactID(ByVal caseContextArtifactID As Int32, ByVal searchArtifactID As Int32, ByVal start As Int32, ByVal finish As Int32) As System.Data.DataSet
			If kCura.WinEDDS.Config.UsesWebAPI Then
				Return MyBase.SearchBySearchArtifactID(caseContextArtifactID, searchArtifactID, start, finish)
			Else
				'Return _searchManager.SearchByArtifactIDAsDataSet(_identity, searchArtifactID)
			End If
		End Function

		Public Shadows Function RetrieveNativesForSearch(ByVal caseContextArtifactID As Int32, ByVal documentArtifactIDs As String) As System.Data.DataSet
			If kCura.WinEDDS.Config.UsesWebAPI Then
				Return MyBase.RetrieveNativesForSearch(caseContextArtifactID, documentArtifactIDs)
			Else
				'Return kCura.EDDS.Service.FileQuery.RetrieveNativesForDocuments(_identity, artifactID, documentArtifactIDs).ToDataSet()
			End If
		End Function

		Public Function RetrieveImagesForDocuments(ByVal caseContextArtifactID As Int32, ByVal documentArtifactIDs As Int32(), ByVal productionArtifactID As Int32) As System.Data.DataSet
			If kCura.WinEDDS.Config.UsesWebAPI Then
				Return RetrieveImagesForSearch(caseContextArtifactID, documentArtifactIDs)
			Else
				'TODO: Implement non-webservice-based calls
			End If
		End Function

		Public Function RetrieveImagesForProductionDocuments(ByVal caseContextArtifactID As Int32, ByVal documentArtifactIDs As Int32(), ByVal productionArtifactID As Int32) As System.Data.DataSet
			If kCura.WinEDDS.Config.UsesWebAPI Then
				Return Me.RetrieveImagesByProductionArtifactIDForProductionByDocumentSet(caseContextArtifactID, productionArtifactID, documentArtifactIDs)
			Else
				'TODO: Implement non-webservice-based calls
			End If
		End Function

		Public Shadows Function RetrieveFullTextFilesForSearch(ByVal caseContextArtifactID As Int32, ByVal documentArtifactIDs As String) As System.Data.DataSet
			If kCura.WinEDDS.Config.UsesWebAPI Then
				Return MyBase.RetrieveFullTextFilesForSearch(caseContextArtifactID, documentArtifactIDs)
			Else
				'Return kCura.EDDS.Service.FileQuery.RetrieveFullTextFilesForDocuments(_identity, artifactID, documentArtifactIDs).ToDataSet()
			End If
		End Function

		Public Shadows Function RetrieveImagesForSearch(ByVal caseContextArtifactID As Int32, ByVal documentArtifactIDs As Int32()) As System.Data.DataSet
			If kCura.WinEDDS.Config.UsesWebAPI Then
				Return MyBase.RetrieveImagesForSearch(caseContextArtifactID, documentArtifactIDs)
			Else
				'Return kCura.EDDS.Service.FileQuery.RetrieveFullTextFilesForDocuments(_identity, artifactID, documentArtifactIDs).ToDataSet()
			End If
		End Function

		Public Shadows Function RetrieveViewsByContextArtifactID(ByVal caseContextArtifactID As Int32, ByVal isSearch As Boolean) As System.Data.DataSet
			If kCura.WinEDDS.Config.UsesWebAPI Then
				Return MyBase.RetrieveViewsByContextArtifactID(caseContextArtifactID, isSearch)
			Else
				'Return _viewManager.ExternalRetrieveViewsByContextArtifactID(contextArtifactID, isSearch)
			End If
		End Function

		Public Shadows Function RetrieveSearchFields(ByVal caseContextArtifactID As Int32, ByVal viewArtifactID As Int32) As System.Data.DataSet
			If kCura.WinEDDS.Config.UsesWebAPI Then
				Return MyBase.RetrieveSearchFields(caseContextArtifactID, viewArtifactID)
			Else
				'Return _viewManager.ExternalRetrieveSearchFields(viewArtifactID, _identity)
			End If
		End Function

		Public Shadows Function RetrieveSearchFieldsForProduction(ByVal caseContextArtifactID As Int32, ByVal productionArtifactID As Int32) As System.Data.DataSet
			If kCura.WinEDDS.Config.UsesWebAPI Then
				Return MyBase.RetrieveSearchFieldsForProduction(caseContextArtifactID, productionArtifactID)
			Else
				'TODO: BIZARRO! IMPLEMENT ME!  BIZARRO!
			End If
		End Function

		Public Shadows Function CountSearchByParentArtifactID(ByVal caseContextArtifactID As Int32, ByVal parentArtifactID As Int32, ByVal searchSubFolders As Boolean, ByVal viewArtifactID As Int32) As Int32
			If kCura.WinEDDS.Config.UsesWebAPI Then
				Return MyBase.CountSearchByParentArtifactID(caseContextArtifactID, parentArtifactID, searchSubFolders, viewArtifactID)
			Else
				'Return _searchManager.SearchByParentArtifactIDAsDataSet(_identity, parentArtifactID, searchSubFolders)
			End If
		End Function

		Public Shadows Function SearchByParentArtifactID(ByVal caseContextArtifactID As Int32, ByVal parentArtifactID As Int32, ByVal searchSubFolders As Boolean, ByVal start As Int32, ByVal finish As Int32, ByVal viewArtifactID As Int32) As System.Data.DataSet
			If kCura.WinEDDS.Config.UsesWebAPI Then
				Return MyBase.SearchByParentArtifactID(caseContextArtifactID, parentArtifactID, searchSubFolders, start, finish, viewArtifactID)
			Else
				'Return _searchManager.SearchByParentArtifactIDAsDataSet(_identity, parentArtifactID, searchSubFolders)
			End If
		End Function

		Public Shadows Function SearchByProductionArtifactID(ByVal caseContextArtifactID As Int32, ByVal productionArtifactID As Int32, ByVal start As Int32, ByVal finish As Int32) As System.data.DataSet
			If kCura.WinEDDS.Config.UsesWebAPI Then
				Return MyBase.SearchByProductionArtifactID(caseContextArtifactID, productionArtifactID, start, finish)
			Else
				'Fix this
			End If
		End Function
#End Region

	End Class
End Namespace