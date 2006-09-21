Imports kCura.Utility.NullableTypesHelper
Imports System.Runtime.Remoting.Lifetime
Namespace kCura.WinEDDS.Service
	Public Class DocumentManager
		Inherits kCura.EDDS.WebAPI.DocumentManagerBase.DocumentManager

		Private _identity As kCura.EDDS.EDDSIdentity
		Private _documentManager As New kCura.EDDS.Service.DocumentManager

		Public Sub New(ByVal credentials As Net.NetworkCredential, ByVal cookieContainer As System.Net.CookieContainer, ByVal identity As kCura.EDDS.EDDSIdentity)
			MyBase.New()
			Me.Credentials = credentials
			Me.CookieContainer = cookieContainer
			Me.Url = String.Format("{0}DocumentManager.asmx", kCura.WinEDDS.Config.WebServiceURL)
			Me.Timeout = Settings.DefaultTimeOut
			_identity = identity
		End Sub

		Protected Overrides Function GetWebRequest(ByVal uri As System.Uri) As System.Net.WebRequest
			Dim wr As System.Net.HttpWebRequest = DirectCast(MyBase.GetWebRequest(uri), System.Net.HttpWebRequest)
			wr.UnsafeAuthenticatedConnectionSharing = True
			wr.Credentials = Me.Credentials
			Return wr
		End Function

		Public Shadows Function ReadFromDocumentArtifactID(ByVal documentArtifactID As Int32) As kCura.EDDS.WebAPI.DocumentManagerBase.Document
			Dim doc As kCura.EDDS.WebAPI.DocumentManagerBase.Document = Me.Read(documentArtifactID)
			Dim field As kCura.EDDS.WebAPI.DocumentManagerBase.Field
			For Each field In doc.Fields
				If field.FieldCategoryID = kCura.EDDS.Types.FieldCategory.FullText Then
					field.Value = New NullableTypes.NullableString(System.Text.Encoding.Default.GetString(DirectCast(field.Value, Byte())))
					Exit For
				End If
			Next
			Return doc
		End Function

#Region " Translations "
		Public Shared Function DTOtoDocumentInfo(ByVal dto As kCura.EDDS.WebAPI.DocumentManagerBase.Document) As DocumentInfo
			Dim doc As New DocumentInfo
			doc.AccessControlListID = dto.AccessControlListID
			doc.AccessControlListIsInherited = dto.AccessControlListIsInherited
			doc.ArtifactID = dto.ArtifactID
			doc.ArtifactTypeID = dto.ArtifactTypeID
			doc.ContainerID = dto.ContainerID
			doc.CreatedBy = dto.CreatedBy
			doc.CreatedOn = dto.CreatedOn
			doc.DeleteFlag = dto.DeleteFlag
			doc.Fields = FieldManager.DTOsToDocumentField(dto.Fields)
			doc.Files = FileManager.DTOsToFileInfo(dto.Files)
			doc.Keywords = dto.Keywords
			doc.LastModifiedBy = dto.LastModifiedBy
			doc.LastModifiedOn = dto.LastModifiedOn
			doc.Notes = dto.Notes
			doc.ParentArtifactID = dto.ParentArtifactID
			doc.TextIdentifier = dto.TextIdentifier
			Return doc
		End Function

		Public Shared Function DTOToWebAPIDocument(ByVal docDTO As kCura.EDDS.DTO.Document) As kCura.EDDS.WebAPI.DocumentManagerBase.Document
			If docDTO Is Nothing Then Return Nothing

			Dim doc As New kCura.EDDS.WebAPI.DocumentManagerBase.Document
			doc.AccessControlListID = docDTO.AccessControlListID
			doc.AccessControlListIsInherited = docDTO.AccessControlListIsInherited
			doc.ArtifactID = docDTO.ArtifactID
			doc.ArtifactTypeID = docDTO.ArtifactTypeID
			doc.ContainerID = docDTO.ContainerID
			doc.CreatedBy = docDTO.CreatedBy
			doc.CreatedOn = docDTO.CreatedOn
			doc.DeleteFlag = docDTO.DeleteFlag
			doc.Fields = FieldManager.DTOsToWebAPIFields(docDTO.Fields)
			doc.Files = FileManager.DTOstoWebAPIFiles(docDTO.Files)
			doc.Keywords = docDTO.Keywords
			doc.LastModifiedBy = docDTO.LastModifiedBy
			doc.LastModifiedOn = docDTO.LastModifiedOn
			doc.Notes = docDTO.Notes
			doc.ParentArtifactID = docDTO.ParentArtifactID
			doc.TextIdentifier = docDTO.TextIdentifier
			doc.DocumentAgentFlags = New kCura.EDDS.WebAPI.DocumentManagerBase.DocumentAgentFlags
			doc.DocumentAgentFlags.IndexStatus = docDTO.DocumentAgentFlags.IndexStatus
			doc.DocumentAgentFlags.UpdateFullText = docDTO.DocumentAgentFlags.UpdateFullText
			Return doc
		End Function

		Public Shared Function WebAPIDocumentToDTO(ByVal doc As kCura.EDDS.WebAPI.DocumentManagerBase.Document) As kCura.EDDS.DTO.Document
			Dim docDTO As New kCura.EDDS.DTO.Document
			docDTO.AccessControlListID = doc.AccessControlListID
			docDTO.AccessControlListIsInherited = doc.AccessControlListIsInherited
			docDTO.ArtifactID = doc.ArtifactID
			docDTO.ArtifactTypeID = doc.ArtifactTypeID
			docDTO.ContainerID = doc.ContainerID
			docDTO.CreatedBy = doc.CreatedBy
			docDTO.CreatedOn = doc.CreatedOn
			docDTO.DeleteFlag = doc.DeleteFlag
			docDTO.Fields = FieldManager.WebAPIFieldsToDTOs(doc.Fields)
			docDTO.Files = FileManager.WebAPIFilestoDTOs(doc.Files)
			If doc.Keywords Is Nothing Then
				docDTO.Keywords = String.Empty
			Else
				docDTO.Keywords = doc.Keywords
			End If
			docDTO.LastModifiedBy = doc.LastModifiedBy
			docDTO.LastModifiedOn = doc.LastModifiedOn
			If doc.Notes Is Nothing Then
				docDTO.Notes = String.Empty
			Else
				docDTO.Notes = doc.Notes
			End If
			docDTO.ParentArtifactID = doc.ParentArtifactID
			docDTO.TextIdentifier = doc.TextIdentifier

			docDTO.DocumentAgentFlags = New kCura.EDDS.DTO.DocumentAgentFlags
			docDTO.DocumentAgentFlags.IndexStatus = doc.DocumentAgentFlags.IndexStatus
			docDTO.DocumentAgentFlags.UpdateFullText = doc.DocumentAgentFlags.UpdateFullText
			Return docDTO
		End Function

		Public Shared Function WebAPIDocumentsToDTOs(ByVal docs() As kCura.EDDS.WebAPI.DocumentManagerBase.Document) As kCura.EDDS.DTO.Document()
			Dim docDTOs(docs.Length - 1) As kCura.EDDS.DTO.Document

			Dim i As Int32
			For i = 0 To docDTOs.Length - 1
				docDTOs(i) = WebAPIDocumentToDTO(docs(i))
			Next
			Return docDTOs
		End Function
#End Region

#Region " Shadow Functions "
		Public Shadows Function GetAllDocumentsForCase(ByVal caseID As Int32) As System.Data.DataSet
			If kCura.WinEDDS.Config.UsesWebAPI Then
				Return MyBase.GetAllDocumentsForCase(caseID)
			Else
				Return _documentManager.GetAllDocumentsForCase(caseID, _identity).ToDataSet
			End If
		End Function

		Public Shadows Function NewFileDTO() As kCura.EDDS.DTO.File
			Return New kCura.EDDS.DTO.File
		End Function

		Public Shadows Function CreateEmptyDocument(ByVal parentFolderID As Int32, ByVal identifierValue As Byte(), ByVal fullTextFileName As String, ByVal identifierColumn As String, ByVal fullTextBuilder As kCura.EDDS.Types.FullTextBuilder) As Int32
			If kCura.WinEDDS.Config.UsesWebAPI Then
				Return MyBase.CreateEmptyDocument(parentFolderID, identifierValue, fullTextFileName, identifierColumn, fullTextBuilder)
			Else
				Return _documentManager.CreateEmptyDocument(_identity, parentFolderID, System.Text.Encoding.ASCII.GetString(identifierValue), fullTextFileName, kCura.EDDS.FieldHelper.GetColumnName(identifierColumn), fullTextBuilder)
			End If
		End Function

		Public Shadows Function Read(ByVal artifactID As Int32) As kCura.EDDS.WebAPI.DocumentManagerBase.Document
			If kCura.WinEDDS.Config.UsesWebAPI Then
				Return MyBase.Read(artifactID)
			Else
				Dim docDTO As kCura.EDDS.DTO.Document = _documentManager.ExternalRead(artifactID, _identity)
				Return DTOToWebAPIDocument(docDTO)
			End If
		End Function

		Public Shadows Function DeleteNative(ByVal document As kCura.EDDS.WebAPI.DocumentManagerBase.Document) As Boolean
			If kCura.WinEDDS.Config.UsesWebAPI Then
				Return MyBase.DeleteNative(document)
			Else
				Return _documentManager.DeleteNative(Me.WebAPIDocumentToDTO(document), _identity)
			End If
		End Function

		Public Shadows Function Create(ByVal document As kCura.EDDS.WebAPI.DocumentManagerBase.Document) As Int32
			If kCura.WinEDDS.Config.UsesWebAPI Then
				Return MyBase.Create(document)
			Else
				Return _documentManager.ExternalCreate(Me.WebAPIDocumentToDTO(document), _identity)
			End If
		End Function

		Public Shadows Function Update(ByVal document As kCura.EDDS.WebAPI.DocumentManagerBase.Document) As Int32
			If kCura.WinEDDS.Config.UsesWebAPI Then
				Return MyBase.Update(document)
			Else
				Return _documentManager.ExternalUpdate(Me.WebAPIDocumentToDTO(document), _identity)
			End If
		End Function

		Public Shadows Sub CreateRange(ByVal documents As kCura.EDDS.WebAPI.DocumentManagerBase.Document())
			If kCura.WinEDDS.Config.UsesWebAPI Then
				MyBase.CreateRange(documents)
			Else
				_documentManager.ExternalCreateRange(WebAPIDocumentsToDTOs(documents), _identity)
			End If
		End Sub

		Public Shadows Sub UpdateRange(ByVal documents As kCura.EDDS.WebAPI.DocumentManagerBase.Document())
			If kCura.WinEDDS.Config.UsesWebAPI Then
				MyBase.UpdateRange(documents)
			Else
				_documentManager.ExternalUpdateRange(WebAPIDocumentsToDTOs(documents), _identity)
			End If
		End Sub

		Public Shadows Function AddFullTextToDocumentFromFile(ByVal documentArtifactID As Int32, ByVal fullTextFileName As String, ByVal fullTextBuilder As kCura.EDDS.Types.FullTextBuilder) As Boolean
			If kCura.WinEDDS.Config.UsesWebAPI Then
				Return MyBase.AddFullTextToDocumentFromFile(documentArtifactID, fullTextFileName, fullTextBuilder)
			Else
				Return _documentManager.AddFullTextToDocumentFromFile(documentArtifactID, fullTextFileName, _identity, fullTextBuilder)
			End If
		End Function

		Public Shadows Function GetDocumentArtifactIDFromIdentifier(ByVal identifier As String, ByVal fieldDisplayName As String, ByVal caseID As Int32) As Int32
			If kCura.WinEDDS.Config.UsesWebAPI Then
				Return MyBase.GetDocumentArtifactIDFromIdentifier(identifier, fieldDisplayName, caseID)
			Else
				Return _documentManager.ExternalGetDocumentArtifactIDFromIdentifier(identifier, fieldDisplayName, caseID)
			End If
		End Function

		Public Shadows Function ReadFromIdentifier(ByVal caseID As Int32, ByVal fieldDisplayName As String, ByVal identifier As String) As kCura.EDDS.WebAPI.DocumentManagerBase.Document
			If kCura.WinEDDS.Config.UsesWebAPI Then
				Return MyBase.ReadFromIdentifier(caseID, fieldDisplayName, identifier)
			Else
				Return Me.DTOToWebAPIDocument(_documentManager.ExternalReadFromIdentifier(caseID, fieldDisplayName, identifier, _identity))
			End If
		End Function

		Public Shadows Sub UpdateFullTextWithCrackedText(ByVal documentArtifactID As Int32, ByVal fileGuid As String)
			If kCura.WinEDDS.Config.UsesWebAPI Then
				MyBase.UpdateFullTextWithCrackedText(documentArtifactID, fileGuid)
			Else
				_documentManager.UpdateFullTextWithCrackedText(documentArtifactID, fileGuid, _identity)
			End If
		End Sub

		Public Shadows Sub ClearImagesFromDocument(ByVal artifactID As Int32)
			If kCura.WinEDDS.Config.UsesWebAPI Then
				MyBase.ClearImagesFromDocument(artifactID)
			Else
				_documentManager.ClearImagesFromDocument(artifactID, _identity)
			End If
		End Sub

		Public Shadows Function GetDocumentDirectoryByContextArtifactID(ByVal contextArtifactID As Int32) As String
			If kCura.WinEDDS.Config.UsesWebAPI Then
				Return MyBase.GetDocumentDirectoryByContextArtifactID(contextArtifactID)
			Else
				Return kCura.EDDS.DocumentHelper.GetDocumentDirectoryByContextArtifactID(contextArtifactID)
			End If
		End Function

		Public Shadows Function GetDocumentDirectoryByCaseArtifactID(ByVal caseArtifactID As Int32) As String
			If kCura.WinEDDS.Config.UsesWebAPI Then
				Return MyBase.GetDocumentDirectoryByCaseArtifactID(caseArtifactID)
			Else
				Return kCura.EDDS.DocumentHelper.GetDocumentDirectoryByCaseArtifactID(caseArtifactID)
			End If
		End Function

		Public Shadows Function GetPrintImageGuids(ByVal artifactID As Int32, ByVal orderedProductionIDList As Int32()) As Guid()
			If kCura.WinEDDS.Config.UsesWebAPI Then
				Return MyBase.GetPrintImageGuids(artifactID, orderedProductionIDList)
			Else
				Return _documentManager.GetPrintImageGuids(artifactID, orderedProductionIDList, _identity)
			End If
		End Function
#End Region

	End Class
End Namespace