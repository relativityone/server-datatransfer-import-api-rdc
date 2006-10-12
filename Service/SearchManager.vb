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
		Public Shadows Function CountSearchByArtifactID(ByVal searchArtifactID As Int32) As Int32
			If kCura.WinEDDS.Config.UsesWebAPI Then
				Return MyBase.CountSearchByArtifactID(searchArtifactID)
			Else
				'Return _searchManager.SearchByArtifactIDAsDataSet(_identity, searchArtifactID)
			End If
		End Function

		Public Shadows Function SearchBySearchArtifactID(ByVal searchArtifactID As Int32, ByVal start As Int32, ByVal finish As Int32) As System.Data.DataSet
			If kCura.WinEDDS.Config.UsesWebAPI Then
				Return MyBase.SearchBySearchArtifactID(searchArtifactID, start, finish)
			Else
				'Return _searchManager.SearchByArtifactIDAsDataSet(_identity, searchArtifactID)
			End If
		End Function

		Public Shadows Function RetrieveNativesForSearch(ByVal artifactID As Int32, ByVal documentArtifactIDs As String) As System.Data.DataSet
			If kCura.WinEDDS.Config.UsesWebAPI Then
				Return MyBase.RetrieveNativesForSearch(artifactID, documentArtifactIDs)
			Else
				'Return kCura.EDDS.Service.FileQuery.RetrieveNativesForDocuments(_identity, artifactID, documentArtifactIDs).ToDataSet()
			End If
		End Function

		Public Shadows Function RetrieveFullTextFilesForSearch(ByVal artifactID As Int32, ByVal documentArtifactIDs As String) As System.Data.DataSet
			If kCura.WinEDDS.Config.UsesWebAPI Then
				Return MyBase.RetrieveFullTextFilesForSearch(artifactID, documentArtifactIDs)
			Else
				'Return kCura.EDDS.Service.FileQuery.RetrieveFullTextFilesForDocuments(_identity, artifactID, documentArtifactIDs).ToDataSet()
			End If
		End Function

		Public Shadows Function RetrieveViewsByContextArtifactID(ByVal contextArtifactID As Int32, ByVal isSearch As Boolean) As System.Data.DataSet
			If kCura.WinEDDS.Config.UsesWebAPI Then
				Return MyBase.RetrieveViewsByContextArtifactID(contextArtifactID, isSearch)
			Else
				'Return _viewManager.ExternalRetrieveViewsByContextArtifactID(contextArtifactID, isSearch)
			End If
		End Function

		Public Shadows Function RetrieveSearchFields(ByVal viewArtifactID As Int32) As System.Data.DataSet
			If kCura.WinEDDS.Config.UsesWebAPI Then
				Return MyBase.RetrieveSearchFields(viewArtifactID)
			Else
				'Return _viewManager.ExternalRetrieveSearchFields(viewArtifactID, _identity)
			End If
		End Function

		Public Shadows Function CountSearchByParentArtifactID(ByVal parentArtifactID As Int32, ByVal searchSubFolders As Boolean) As Int32
			If kCura.WinEDDS.Config.UsesWebAPI Then
				Return MyBase.CountSearchByParentArtifactID(parentArtifactID, searchSubFolders)
			Else
				'Return _searchManager.SearchByParentArtifactIDAsDataSet(_identity, parentArtifactID, searchSubFolders)
			End If
		End Function

		Public Shadows Function SearchByParentArtifactID(ByVal parentArtifactID As Int32, ByVal searchSubFolders As Boolean, ByVal start As Int32, ByVal finish As Int32) As System.Data.DataSet
			If kCura.WinEDDS.Config.UsesWebAPI Then
				Return MyBase.SearchByParentArtifactID(parentArtifactID, searchSubFolders, start, finish)
			Else
				'Return _searchManager.SearchByParentArtifactIDAsDataSet(_identity, parentArtifactID, searchSubFolders)
			End If
		End Function
#End Region

	End Class
End Namespace