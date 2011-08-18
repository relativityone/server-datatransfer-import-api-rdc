Imports kCura.Utility.NullableTypesHelper
Imports System.Runtime.Remoting.Lifetime
Namespace kCura.WinEDDS.Service
	Public Class DocumentManager
		Inherits kCura.EDDS.WebAPI.DocumentManagerBase.DocumentManager

		Private ReadOnly _serviceURLPageFormat As String

		Public Sub New(ByVal credentials As Net.ICredentials, ByVal cookieContainer As System.Net.CookieContainer)
			Me.New(credentials, cookieContainer, kCura.WinEDDS.Config.WebServiceURL)
		End Sub

		Public Sub New(ByVal credentials As Net.ICredentials, ByVal cookieContainer As System.Net.CookieContainer, ByVal webURL As String)
			MyBase.New()

			_serviceURLPageFormat = "{0}DocumentManager.asmx"
			Me.Credentials = credentials
			Me.CookieContainer = cookieContainer
			Me.ServiceURL = webURL
			Me.Timeout = Settings.DefaultTimeOut
		End Sub

		Public Overridable Property ServiceURL As String
			Get
				Return Me.Url
			End Get
			Set(ByVal value As String)
				Me.Url = String.Format(_serviceURLPageFormat, value)
			End Set
		End Property

		Protected Overrides Function GetWebRequest(ByVal uri As System.Uri) As System.Net.WebRequest
			Dim wr As System.Net.HttpWebRequest = DirectCast(MyBase.GetWebRequest(uri), System.Net.HttpWebRequest)
			wr.UnsafeAuthenticatedConnectionSharing = True
			wr.Credentials = Me.Credentials
			Return wr
		End Function

		Public Shadows Function ReadFromDocumentArtifactID(ByVal caseContextArtifactID As Int32, ByVal documentArtifactID As Int32, ByVal fieldArtifactIds As Int32()) As kCura.EDDS.WebAPI.DocumentManagerBase.Document
			Dim doc As kCura.EDDS.WebAPI.DocumentManagerBase.Document = Me.Read(caseContextArtifactID, documentArtifactID, fieldArtifactIds)
			Dim field As kCura.EDDS.WebAPI.DocumentManagerBase.Field
			For Each field In doc.Fields
				If field.FieldCategoryID = Relativity.FieldCategory.FullText Then
					field.Value = System.Text.Encoding.Default.GetString(DirectCast(field.Value, Byte()))
					Exit For
				End If
			Next
			Return doc
		End Function

#Region " Translations "

		Private Function GetWebAPIFullTextBuilder(ByVal eddsftb As Relativity.FullTextBuilder) As kCura.EDDS.WebAPI.DocumentManagerBase.FullTextBuilderDTO
			Dim wapiftb As New kCura.EDDS.WebAPI.DocumentManagerBase.FullTextBuilderDTO
			wapiftb.Pages = DirectCast(eddsftb.Pages.ToArray(GetType(Int32)), Int32())
			wapiftb.FullText = System.Text.Encoding.Unicode.GetBytes(eddsftb.FullTextString)
			wapiftb.FilePointer = eddsftb.FilePointer
			Return wapiftb
		End Function

#End Region

#Region " Shadow Functions "
		Public Shadows Function GetAllDocumentsForCase(ByVal caseID As Int32) As System.Data.DataSet
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				Try
					tries += 1
					Return MyBase.GetAllDocumentsForCase(caseID)
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso _
					 ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso _
					 tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries, ServiceURL)
					Else
						Throw
					End If
				End Try
			End While
			Return Nothing
		End Function

		Public Shadows Function CreateEmptyDocument(ByVal caseContextArtifactID As Int32, ByVal parentFolderID As Int32, ByVal identifierValue As Byte(), ByVal identifierColumn As String, ByVal fullTextBuilder As Relativity.FullTextBuilder) As Int32
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				Try
					tries += 1
					Return MyBase.CreateEmptyDocument(caseContextArtifactID, parentFolderID, identifierValue, identifierColumn, GetWebAPIFullTextBuilder(fullTextBuilder))
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries, ServiceURL)
					Else
						Throw
					End If
				End Try
			End While
			Return Nothing
		End Function

		Public Shadows Function Read(ByVal caseContextArtifactID As Int32, ByVal artifactID As Int32, ByVal fieldArtifactIds As Int32()) As kCura.EDDS.WebAPI.DocumentManagerBase.Document
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				Try
					tries += 1
					Return MyBase.Read(caseContextArtifactID, artifactID, fieldArtifactIds)
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries, ServiceURL)
					Else
						Throw
					End If
				End Try
			End While
			Return Nothing
		End Function

		Public Shadows Function Create(ByVal caseContextArtifactID As Int32, ByVal document As kCura.EDDS.WebAPI.DocumentManagerBase.Document, ByVal files As kCura.EDDS.WebAPI.DocumentManagerBase.File()) As Int32
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					Return MyBase.Create(caseContextArtifactID, document, files)
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries, ServiceURL)
					Else
						Throw
					End If
				End Try
			End While
			Return Nothing
		End Function

		Public Shadows Function Update(ByVal caseContextArtifactID As Int32, ByVal document As kCura.EDDS.WebAPI.DocumentManagerBase.Document, ByVal files As kCura.EDDS.WebAPI.DocumentManagerBase.File()) As Int32
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				Try
					tries += 1
					Return MyBase.Update(caseContextArtifactID, document, files)
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries, ServiceURL)
					Else
						Throw
					End If
				End Try
			End While
			Return Nothing
		End Function

		Public Shadows Function AddFullTextToDocument(ByVal caseContextArtifactID As Int32, ByVal documentArtifactID As Int32, ByVal fullTextBuilder As Relativity.FullTextBuilder) As Boolean
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					Return MyBase.AddFullTextToDocument(caseContextArtifactID, documentArtifactID, GetWebAPIFullTextBuilder(fullTextBuilder))
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries, ServiceURL)
					Else
						Throw
					End If
				End Try
			End While
			Return Nothing
		End Function

		Public Shadows Function GetDocumentArtifactIDFromIdentifier(ByVal caseContextArtifactID As Int32, ByVal identifier As String, ByVal fieldDisplayName As String) As Int32
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					Return MyBase.GetDocumentArtifactIDFromIdentifier(caseContextArtifactID, identifier, fieldDisplayName)
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries, ServiceURL)
					Else
						Throw
					End If
				End Try
			End While
			Return Nothing
		End Function

		Public Shadows Function ReadFromIdentifier(ByVal caseContextArtifactID As Int32, ByVal fieldDisplayName As String, ByVal identifier As String, ByVal fieldArtifactIds As Int32()) As kCura.EDDS.WebAPI.DocumentManagerBase.Document
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				Try
					tries += 1
					If kCura.WinEDDS.Config.UsesWebAPI Then
						Return MyBase.ReadFromIdentifier(caseContextArtifactID, fieldDisplayName, identifier, fieldArtifactIds)
					Else
						'Return Me.DTOToWebAPIDocument(_documentManager.ExternalReadFromIdentifier(caseID, fieldDisplayName, identifier, _identity))
					End If
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries, ServiceURL)
					ElseIf TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("ResetFieldIdCacheException") <> -1 Then

					Else
						Throw
					End If
				End Try
			End While
			Return Nothing
		End Function

		Public Shadows Sub ClearImagesFromDocument(ByVal caseContextArtifactID As Int32, ByVal artifactID As Int32)
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				Try
					tries += 1
					MyBase.ClearImagesFromDocument(caseContextArtifactID, artifactID)
					Return
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries, ServiceURL)
					Else
						Throw
					End If
				End Try
			End While
		End Sub

		Public Shadows Function RetrieveAllUnsupportedOiFileIds() As Int32()
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				Try
					tries += 1
					Return MyBase.RetrieveAllUnsupportedOiFileIds()
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries, ServiceURL)
					Else
						Throw
					End If
				End Try
			End While
			Return Nothing
		End Function

		Public Shadows Function GetDocumentDirectoryByCaseArtifactID(ByVal caseArtifactID As Int32) As String
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				Try
					tries += 1
					If kCura.WinEDDS.Config.UsesWebAPI Then
						Return MyBase.GetDocumentDirectoryByCaseArtifactID(caseArtifactID)
					Else
						'Return Relativity.Core.DocumentHelper.GetDocumentDirectoryByCaseArtifactID(caseArtifactID)
					End If
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries, ServiceURL)
					Else
						Throw
					End If
				End Try
			End While
			Return Nothing
		End Function


		Public Shadows Function GetPrintImageGuids(ByVal caseContextArtifactID As Int32, ByVal artifactID As Int32, ByVal orderedProductionIDList As Int32()) As Guid()
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				Try
					tries += 1
					If kCura.WinEDDS.Config.UsesWebAPI Then
						Return MyBase.GetPrintImageGuids(caseContextArtifactID, artifactID, orderedProductionIDList)
					Else
						'Return _documentManager.GetPrintImageGuids(artifactID, orderedProductionIDList, _identity)
					End If
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries, ServiceURL)
					Else
						Throw
					End If
				End Try
			End While
			Return Nothing
		End Function
#End Region

	End Class
End Namespace