Namespace kCura.EDDS.WinForm
  Public Class WinEDDSGateway
    Inherits kCura.EDDS.Import.HostGatewayBase

    Private _documentManager As kCura.WinEDDS.Service.DocumentManager
    Private _folderManager As kCura.WinEDDS.Service.FolderManager
    Private _uploader As kCura.WinEDDS.FileUploader
    Private _fileManager As kCura.WinEDDS.Service.FileManager

    Private _currentCaseID As Int32
    Private _currentDocumentCaseFields As kCura.EDDS.WebAPI.DocumentManagerBase.Field()

    Private _rootFolderACLID As Int32

    Private _application As kCura.EDDS.WinForm.Application

    Public Overrides Function GetCaseFields(ByVal caseRootArtifactID As Int32) As String()
			Return _application.GetCaseFields(caseRootArtifactID, 10)
    End Function

    Public Overrides Function GetCaseFolderPath(ByVal folderId As Int32) As String
      Return _application.GetCaseFolderPath(folderId)
    End Function

    Public Overrides Sub InvokeImport(ByVal settings As Object)
      _application.ImportGeneric(settings)
    End Sub

    Private Function GetRootFolderACLID(ByVal destinationFolderID As Int32) As Int32
      If _rootFolderACLID = 0 Then
				_rootFolderACLID = _folderManager.Read(_currentCaseID, destinationFolderID).AccessControlListID
      End If
      Return _rootFolderACLID
    End Function

		Private Function GetDocumentFields(ByVal caseID As Int32) As kCura.EDDS.WebAPI.DocumentManagerBase.Field()
			If _currentCaseID <> caseID Then
				'Dim fieldManager As New kCura.WinEDDS.Service.FieldQuery(_application.Credential, _application.CookieContainer, _application.Identity)
				Dim fieldManager As New kCura.WinEDDS.Service.FieldQuery(_application.Credential, _application.CookieContainer)
        'TODO: WINFLEX - ArtifactTypeID
        'TODO: WINFLEX - Should reference an enum or constant instead of literal 10
				_currentDocumentCaseFields = fieldManager.RetrieveAllAsArray(caseID, 10)
			End If
			Return _currentDocumentCaseFields
		End Function

		Public Overrides Function CreateDocumentRecord(ByVal recordInfo As String, ByVal fieldValues() As kCura.EDDS.Import.FieldValue, ByVal caseInfo As kCura.EDDS.Import.CaseInfo, ByVal destinationFolderID As Integer, ByVal importer As kCura.EDDS.Import.ImporterBase, ByVal extractFullText As Boolean) As Int32

			importer.ReportStatus(recordInfo, "Creating EDDS Record")

			Dim documentDTO As New kCura.EDDS.WebAPI.DocumentManagerBase.Document
			documentDTO.Fields = Me.GetDocumentFields(caseInfo.RootArtifactID)
			documentDTO.ParentArtifactID = New NullableTypes.NullableInt32(destinationFolderID)
			documentDTO.ContainerID = New NullableTypes.NullableInt32(caseInfo.ArtifactID)
			documentDTO.AccessControlListIsInherited = True
			documentDTO.AccessControlListID = Me.GetRootFolderACLID(destinationFolderID)
			documentDTO.DocumentAgentFlags = New kCura.EDDS.WebAPI.DocumentManagerBase.DocumentAgentFlags
			documentDTO.DocumentAgentFlags.UpdateFullText = extractFullText
			documentDTO.DocumentAgentFlags.IndexStatus = 1
			importer.ReportStatus(recordInfo, "Building Field Map")

			' set the control field
			Dim field As kCura.EDDS.WebAPI.DocumentManagerBase.Field
			For Each field In documentDTO.Fields
				' determin if field is part of fieldMap
				Dim fieldValue As kCura.EDDS.Import.FieldValue
				'Select Case CType(field.FieldCategoryID, kCura.EDDS.Types.FieldCategory)
				'  Case Types.FieldCategory.FullText
				'    field.Value = New Byte() {}
				'  Case Else
				field.Value = String.Empty
				'End Select
				For Each fieldValue In fieldValues
					If field.DisplayName = fieldValue.Name Then
						Select Case CType(field.FieldCategoryID, kCura.DynamicFields.Types.FieldCategory)
							Case kCura.DynamicFields.Types.FieldCategory.FullText
								field.Value = _uploader.UploadTextAsFile(fieldValue.Value, _currentCaseID, System.Guid.NewGuid.ToString)
								'field.Value = (New System.Text.ASCIIEncoding).GetBytes(fieldValue.Value)
							Case Else
								field.Value = fieldValue.Value
						End Select
					End If
				Next
			Next

			importer.ReportStatus(recordInfo, "Finished Creating EDDS Record")
			Dim documentArtifactID As Int32 = _documentManager.Create(_currentCaseID, documentDTO, Nothing)

			Return documentArtifactID

		End Function

		Public Overrides Sub UploadDocument(ByVal recordInfo As String, ByVal fileName As String, ByVal documentArtifactID As Integer, ByVal extractFullText As Boolean, ByVal caseInfo As kCura.EDDS.Import.CaseInfo, ByVal destinationFolderID As Integer, ByVal importer As kCura.EDDS.Import.ImporterBase)
			Try
				importer.ReportStatus(recordInfo, "Begin Uploading Document")
				Dim fileInfo As System.IO.FileInfo = New System.IO.FileInfo(fileName)
				Dim fileIdentifier As String
				_uploader.UploaderType = FileUploader.Type.Web
				fileIdentifier = _uploader.UploadFile(fileName, documentArtifactID)
				If fileIdentifier <> String.Empty Then
					_fileManager.CreateFile(_currentCaseID, documentArtifactID, fileInfo.Name, fileIdentifier, 0, kCura.EDDS.Types.FileType.Native)
				End If
				importer.ReportStatus(recordInfo, "Finished Uploading Document")
			Catch ex As System.Exception
				importer.ReportError(recordInfo, "Exception Encountered Uploading Document" + vbCrLf + ex.ToString)
				importer.TotalRecordsProcessedWithErrors = importer.TotalRecordsProcessedWithErrors + 1
			End Try
		End Sub

		Public Sub New()
			_application = Application.Instance
			'_folderManager = New kCura.WinEDDS.Service.FolderManager(_application.Credential, _application.CookieContainer, _application.Identity)
			'_documentManager = New kCura.WinEDDS.Service.DocumentManager(_application.Credential, _application.CookieContainer, _application.Identity)
			'_fileManager = New kCura.WinEDDS.Service.FileManager(_application.Credential, _application.CookieContainer, _application.Identity)
			_folderManager = New kCura.WinEDDS.Service.FolderManager(_application.Credential, _application.CookieContainer)
			_documentManager = New kCura.WinEDDS.Service.DocumentManager(_application.Credential, _application.CookieContainer)
			_fileManager = New kCura.WinEDDS.Service.FileManager(_application.Credential, _application.CookieContainer)
			_uploader = New kCura.WinEDDS.FileUploader(_application.Credential, _currentCaseID, "", _application.CookieContainer)
		End Sub

		Public Overrides Function CreateFolder(ByVal folderName As String, ByVal parentFolderArtifactID As Integer, ByVal importer As kCura.EDDS.Import.ImporterBase) As Integer

			Dim folderId As Int32 = _folderManager.Create(_currentCaseID, parentFolderArtifactID, folderName)
			Return folderId
			importer.ReportStatus("Folder:" + folderName, "Created")

		End Function
	End Class
End Namespace