Imports kCura.Utility.NullableTypesHelper
Imports System.Runtime.Remoting.Lifetime
Namespace kCura.WinEDDS.Service
	Public Class DocumentManager
		Inherits kCura.EDDS.WebAPI.DocumentManagerBase.DocumentManager

		Public Sub New(ByVal credentials As Net.NetworkCredential, ByVal cookieContainer As System.Net.CookieContainer)
			MyBase.New()
			Me.Credentials = credentials
			Me.CookieContainer = cookieContainer
			Me.Url = String.Format("{0}DocumentManager.asmx", kCura.WinEDDS.Config.WebServiceURL)
			Me.Timeout = Settings.DefaultTimeOut
		End Sub

		Protected Overrides Function GetWebRequest(ByVal uri As System.Uri) As System.Net.WebRequest
			Dim wr As System.Net.HttpWebRequest = DirectCast(MyBase.GetWebRequest(uri), System.Net.HttpWebRequest)
			wr.UnsafeAuthenticatedConnectionSharing = True
			wr.Credentials = Me.Credentials
			Return wr
		End Function

		Public Shadows Function ReadFromDocumentArtifactID(ByVal caseContextArtifactID As Int32, ByVal documentArtifactID As Int32, ByVal fieldArtifactIds As Int32()) As kCura.EDDS.WebAPI.DocumentManagerBase.Document
			Dim doc As kCura.EDDS.WebAPI.DocumentManagerBase.Document = Me.Read(caseContextArtifactID, documentArtifactID, fieldArtifactIDs)
			Dim field As kCura.EDDS.WebAPI.DocumentManagerBase.Field
			For Each field In doc.Fields
				If field.FieldCategoryID = kCura.DynamicFields.Types.FieldCategory.FullText Then
					field.Value = System.Text.Encoding.Default.GetString(DirectCast(field.Value, Byte()))
					Exit For
				End If
			Next
			Return doc
		End Function

#Region " Translations "

		Private Function GetWebAPIFullTextBuilder(ByVal eddsftb As kCura.EDDS.Types.FullTextBuilder) As kCura.EDDS.WebAPI.DocumentManagerBase.FullTextBuilderDTO
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
					If kCura.WinEDDS.Config.UsesWebAPI Then
						Return MyBase.GetAllDocumentsForCase(caseID)
					Else
						'Return _documentManager.GetAllDocumentsForCase(caseID, _identity).ToDataSet
					End If
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso _
					 ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso _
					 tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries)
					Else
						Throw
					End If
				End Try
			End While
		End Function

		Public Shadows Function CreateEmptyDocument(ByVal caseContextArtifactID As Int32, ByVal parentFolderID As Int32, ByVal identifierValue As Byte(), ByVal identifierColumn As String, ByVal fullTextBuilder As kCura.EDDS.Types.FullTextBuilder) As Int32
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				Try
					tries += 1
					If kCura.WinEDDS.Config.UsesWebAPI Then
						Return MyBase.CreateEmptyDocument(caseContextArtifactID, parentFolderID, identifierValue, identifierColumn, GetWebAPIFullTextBuilder(fullTextBuilder))
					Else
						'Return _documentManager.CreateEmptyDocument(_identity, parentFolderID, System.Text.Encoding.ASCII.GetString(identifierValue), fullTextFileName, kCura.EDDS.FieldHelper.GetColumnName(identifierColumn), fullTextBuilder)
					End If
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries)
					Else
						Throw
					End If
				End Try
			End While
		End Function

		Public Shadows Function Read(ByVal caseContextArtifactID As Int32, ByVal artifactID As Int32, ByVal fieldArtifactIds As Int32()) As kCura.EDDS.WebAPI.DocumentManagerBase.Document
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				Try
					tries += 1
					If kCura.WinEDDS.Config.UsesWebAPI Then
						Return MyBase.Read(caseContextArtifactID, artifactID, fieldArtifactIds)
					Else
						'Dim docDTO As kCura.EDDS.DTO.Document = _documentManager.ExternalRead(artifactID, _identity)
						'Return DTOToWebAPIDocument(docDTO)
					End If
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries)
					Else
						Throw
					End If
				End Try
			End While
		End Function

		Public Shadows Function Create(ByVal caseContextArtifactID As Int32, ByVal document As kCura.EDDS.WebAPI.DocumentManagerBase.Document, ByVal files As kCura.EDDS.WebAPI.DocumentManagerBase.File()) As Int32
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					If kCura.WinEDDS.Config.UsesWebAPI Then
						Return MyBase.Create(caseContextArtifactID, document, files)
					Else
						'Return _documentManager.ExternalCreate(Me.WebAPIDocumentToDTO(document), _identity)
					End If
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries)
					Else
						Throw
					End If
				End Try
			End While
		End Function

		Public Shadows Function Update(ByVal caseContextArtifactID As Int32, ByVal document As kCura.EDDS.WebAPI.DocumentManagerBase.Document, ByVal files As kCura.EDDS.WebAPI.DocumentManagerBase.File()) As Int32
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				Try
					tries += 1
					If kCura.WinEDDS.Config.UsesWebAPI Then
						Return MyBase.Update(caseContextArtifactID, document, files)
					Else
						'Return _documentManager.ExternalCreate(Me.WebAPIDocumentToDTO(document), _identity)
					End If
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries)
					Else
						Throw
					End If
				End Try
			End While
		End Function

		'Public Shadows Sub CreateRange(ByVal caseContextArtifactID As Int32, ByVal documents As kCura.EDDS.WebAPI.DocumentManagerBase.Document())
		'	Dim tries As Int32 = 0
		'	While tries < Config.MaxReloginTries
		'		Try
		'			tries += 1
		'			If kCura.WinEDDS.Config.UsesWebAPI Then
		'				MyBase.CreateRange(caseContextArtifactID, documents)
		'				Exit Sub
		'			Else
		'				'_documentManager.ExternalCreateRange(WebAPIDocumentsToDTOs(documents), _identity)
		'			End If
		'		Catch ex As System.Exception
		'			If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
		'				Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer)
		'			Else
		'				Throw
		'			End If
		'		End Try
		'	End While
		'End Sub

		'Public Shadows Sub UpdateRange(ByVal caseContextArtifactID As Int32, ByVal documents As kCura.EDDS.WebAPI.DocumentManagerBase.Document())
		'	Dim tries As Int32 = 0
		'	While tries < Config.MaxReloginTries
		'		Try
		'			tries += 1
		'			If kCura.WinEDDS.Config.UsesWebAPI Then
		'				MyBase.UpdateRange(caseContextArtifactID, documents)
		'				Exit Sub
		'			Else
		'				'_documentManager.ExternalUpdateRange(WebAPIDocumentsToDTOs(documents), _identity)
		'			End If
		'		Catch ex As System.Exception
		'			If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
		'				Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer)
		'			Else
		'				Throw
		'			End If
		'		End Try
		'	End While
		'End Sub

		Public Shadows Function AddFullTextToDocument(ByVal caseContextArtifactID As Int32, ByVal documentArtifactID As Int32, ByVal fullTextBuilder As kCura.EDDS.Types.FullTextBuilder) As Boolean
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					If kCura.WinEDDS.Config.UsesWebAPI Then
						Return MyBase.AddFullTextToDocument(caseContextArtifactID, documentArtifactID, GetWebAPIFullTextBuilder(fullTextBuilder))
					Else
						'Return _documentManager.AddFullTextToDocumentFromFile(documentArtifactID, fullTextFileName, _identity, fullTextBuilder)
					End If
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries)
					Else
						Throw
					End If
				End Try
			End While
		End Function

		Public Shadows Function GetDocumentArtifactIDFromIdentifier(ByVal caseContextArtifactID As Int32, ByVal identifier As String, ByVal fieldDisplayName As String) As Int32
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					If kCura.WinEDDS.Config.UsesWebAPI Then
						Return MyBase.GetDocumentArtifactIDFromIdentifier(caseContextArtifactID, identifier, fieldDisplayName)
					Else
						'Return _documentManager.ExternalGetDocumentArtifactIDFromIdentifier(identifier, fieldDisplayName, caseID)
					End If
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries)
					Else
						Throw
					End If
				End Try
			End While
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
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries)
					ElseIf TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("ResetFieldIdCacheException") <> -1 Then

					Else
						Throw
					End If
				End Try
			End While
		End Function

		'Public Shadows Sub UpdateFullTextWithCrackedText(ByVal caseContextArtifactID As Int32, ByVal documentArtifactID As Int32, ByVal fileGuid As String)
		'	Dim tries As Int32 = 0
		'	While tries < Config.MaxReloginTries
		'		Try
		'			tries += 1
		'			If kCura.WinEDDS.Config.UsesWebAPI Then
		'				MyBase.UpdateFullTextWithCrackedText(caseContextArtifactID, documentArtifactID, fileGuid)
		'				Exit Sub
		'			Else
		'				'_documentManager.UpdateFullTextWithCrackedText(documentArtifactID, fileGuid, _identity)
		'			End If
		'		Catch ex As System.Exception
		'			If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
		'				Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer)
		'			Else
		'				Throw
		'			End If
		'		End Try
		'	End While
		'End Sub

		Public Shadows Sub ClearImagesFromDocument(ByVal caseContextArtifactID As Int32, ByVal artifactID As Int32)
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				Try
					tries += 1
					If kCura.WinEDDS.Config.UsesWebAPI Then
						MyBase.ClearImagesFromDocument(caseContextArtifactID, artifactID)
						Exit Sub
					Else
						'_documentManager.ClearImagesFromDocument(artifactID, _identity)
					End If
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries)
					Else
						Throw
					End If
				End Try
			End While
		End Sub

		Public Shadows Function GetDocumentDirectoryByCaseArtifactID(ByVal caseArtifactID As Int32) As String
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				Try
					tries += 1
					If kCura.WinEDDS.Config.UsesWebAPI Then
						Return MyBase.GetDocumentDirectoryByCaseArtifactID(caseArtifactID)
					Else
						'Return kCura.EDDS.DocumentHelper.GetDocumentDirectoryByCaseArtifactID(caseArtifactID)
					End If
				Catch ex As System.Exception
					If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries)
					Else
						Throw
					End If
				End Try
			End While
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
						Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries)
					Else
						Throw
					End If
				End Try
			End While
		End Function
#End Region

	End Class
End Namespace