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
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					If kCura.WinEDDS.Config.UsesWebAPI Then
						Return MyBase.CountSearchByArtifactID(caseContextArtifactID, searchArtifactID)
					Else
						'Return _searchManager.SearchByArtifactIDAsDataSet(_identity, searchArtifactID)
					End If
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer)
					Else
						Throw
					End If
				End Try
			End While
		End Function

		Public Shadows Function SearchBySearchArtifactID(ByVal caseContextArtifactID As Int32, ByVal searchArtifactID As Int32, ByVal start As Int32, ByVal finish As Int32) As System.Data.DataSet
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					If kCura.WinEDDS.Config.UsesWebAPI Then
						Return MyBase.SearchBySearchArtifactID(caseContextArtifactID, searchArtifactID, start, finish)
					Else
						'Return _searchManager.SearchByArtifactIDAsDataSet(_identity, searchArtifactID)
					End If
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer)
					Else
						Throw
					End If
				End Try
			End While
		End Function

		Public Shadows Function RetrieveNativesForSearch(ByVal caseContextArtifactID As Int32, ByVal documentArtifactIDs As String) As System.Data.DataSet
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					If kCura.WinEDDS.Config.UsesWebAPI Then
						Return MyBase.RetrieveNativesForSearch(caseContextArtifactID, documentArtifactIDs)
					Else
						'Return kCura.EDDS.Service.FileQuery.RetrieveNativesForDocuments(_identity, artifactID, documentArtifactIDs).ToDataSet()
					End If
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer)
					Else
						Throw
					End If
				End Try
			End While
		End Function

		Public Function RetrieveImagesForDocuments(ByVal caseContextArtifactID As Int32, ByVal documentArtifactIDs As Int32()) As System.Data.DataSet
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					If kCura.WinEDDS.Config.UsesWebAPI Then
						Return RetrieveImagesForSearch(caseContextArtifactID, documentArtifactIDs)
					Else
						'TODO: Implement non-webservice-based calls
					End If
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer)
					Else
						Throw
					End If
				End Try
			End While
		End Function

		Public Function RetrieveImagesForProductionDocuments(ByVal caseContextArtifactID As Int32, ByVal documentArtifactIDs As Int32(), ByVal productionArtifactID As Int32) As System.Data.DataSet
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					If kCura.WinEDDS.Config.UsesWebAPI Then
						Return Me.RetrieveImagesByProductionArtifactIDForProductionByDocumentSet(caseContextArtifactID, productionArtifactID, documentArtifactIDs)
					Else
						'TODO: Implement non-webservice-based calls
					End If
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer)
					Else
						Throw
					End If
				End Try
			End While
		End Function

		Public Shadows Function RetrieveFullTextExistenceForSearch(ByVal caseContextArtifactID As Int32, ByVal documentArtifactIDs As Int32()) As System.Data.DataSet
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					If kCura.WinEDDS.Config.UsesWebAPI Then
						Return MyBase.RetrieveFullTextExistenceForSearch(caseContextArtifactID, documentArtifactIDs)
					Else
						'Return kCura.EDDS.Service.FileQuery.RetrieveFullTextFilesForDocuments(_identity, artifactID, documentArtifactIDs).ToDataSet()
					End If
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer)
					Else
						Throw
					End If
				End Try
			End While
		End Function

		Public Shadows Function RetrieveImagesForSearch(ByVal caseContextArtifactID As Int32, ByVal documentArtifactIDs As Int32()) As System.Data.DataSet
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					If kCura.WinEDDS.Config.UsesWebAPI Then
						Return MyBase.RetrieveImagesForSearch(caseContextArtifactID, documentArtifactIDs)
					Else
						'Return kCura.EDDS.Service.FileQuery.RetrieveFullTextFilesForDocuments(_identity, artifactID, documentArtifactIDs).ToDataSet()
					End If
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer)
					Else
						Throw
					End If
				End Try
			End While
		End Function

		Public Shadows Function RetrieveByProductionIDsAndDocumentIDs(ByVal caseContextArtifactID As Int32, ByVal productionArtifactIDs As Int32(), ByVal documentArtifactIDs As Int32()) As System.Data.DataSet
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					If kCura.WinEDDS.Config.UsesWebAPI Then
						Return MyBase.RetrieveByProductionIDsAndDocumentIDs(caseContextArtifactID, productionArtifactIDs, documentArtifactIDs)
					Else
						'Return kCura.EDDS.Service.FileQuery.RetrieveFullTextFilesForDocuments(_identity, artifactID, documentArtifactIDs).ToDataSet()
					End If
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer)
					Else
						Throw
					End If
				End Try
			End While
		End Function

		Public Shadows Function RetrieveViewsByContextArtifactID(ByVal caseContextArtifactID As Int32, ByVal isSearch As Boolean) As System.Data.DataSet
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					If kCura.WinEDDS.Config.UsesWebAPI Then
						Return MyBase.RetrieveViewsByContextArtifactID(caseContextArtifactID, isSearch)
					Else
						'Return _viewManager.ExternalRetrieveViewsByContextArtifactID(contextArtifactID, isSearch)
					End If
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer)
					Else
						Throw
					End If
				End Try
			End While
		End Function

		Public Shadows Function RetrieveSearchFields(ByVal caseContextArtifactID As Int32, ByVal viewArtifactID As Int32) As System.Data.DataSet
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					If kCura.WinEDDS.Config.UsesWebAPI Then
						Return MyBase.RetrieveSearchFields(caseContextArtifactID, viewArtifactID)
					Else
						'Return _viewManager.ExternalRetrieveSearchFields(viewArtifactID, _identity)
					End If
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer)
					Else
						Throw
					End If
				End Try
			End While
		End Function

		Public Shadows Function RetrieveSearchFieldsForProduction(ByVal caseContextArtifactID As Int32, ByVal productionArtifactID As Int32) As System.Data.DataSet
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					If kCura.WinEDDS.Config.UsesWebAPI Then
						Return MyBase.RetrieveSearchFieldsForProduction(caseContextArtifactID, productionArtifactID)
					Else
						'TODO: BIZARRO! IMPLEMENT ME!  BIZARRO!
					End If
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer)
					Else
						Throw
					End If
				End Try
			End While
		End Function

		Public Shadows Function CountSearchByParentArtifactID(ByVal caseContextArtifactID As Int32, ByVal parentArtifactID As Int32, ByVal searchSubFolders As Boolean, ByVal viewArtifactID As Int32) As Int32
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					If kCura.WinEDDS.Config.UsesWebAPI Then
						Return MyBase.CountSearchByParentArtifactID(caseContextArtifactID, parentArtifactID, searchSubFolders, viewArtifactID)
					Else
						'Return _searchManager.SearchByParentArtifactIDAsDataSet(_identity, parentArtifactID, searchSubFolders)
					End If
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer)
					Else
						Throw
					End If
				End Try
			End While
		End Function

		Public Shadows Function SearchByParentArtifactID(ByVal caseContextArtifactID As Int32, ByVal parentArtifactID As Int32, ByVal searchSubFolders As Boolean, ByVal start As Int32, ByVal finish As Int32, ByVal viewArtifactID As Int32) As System.Data.DataSet
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					If kCura.WinEDDS.Config.UsesWebAPI Then
						Return MyBase.SearchByParentArtifactID(caseContextArtifactID, parentArtifactID, searchSubFolders, start, finish, viewArtifactID)
					Else
						'Return _searchManager.SearchByParentArtifactIDAsDataSet(_identity, parentArtifactID, searchSubFolders)
					End If
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer)
					Else
						Throw
					End If
				End Try
			End While
		End Function

		Public Shadows Function SearchByProductionArtifactID(ByVal caseContextArtifactID As Int32, ByVal productionArtifactID As Int32, ByVal start As Int32, ByVal finish As Int32) As System.data.DataSet
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					If kCura.WinEDDS.Config.UsesWebAPI Then
						Return MyBase.SearchByProductionArtifactID(caseContextArtifactID, productionArtifactID, start, finish)
					Else
						'Fix this
					End If
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer)
					Else
						Throw
					End If
				End Try
			End While
		End Function

		Public Shadows Function IsExtractedTextUnicode(ByVal caseArtifactID As Int32) As Boolean
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					If kCura.WinEDDS.Config.UsesWebAPI Then
						Return MyBase.IsExtractedTextUnicode(caseArtifactID)
					Else
						'Fix this
					End If
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer)
					Else
						Throw
					End If
				End Try
			End While
		End Function
#End Region

	End Class
End Namespace