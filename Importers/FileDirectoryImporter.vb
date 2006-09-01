Namespace kCura.WinEDDS
	Public Class FileDirectoryImporter
		Inherits kCura.EDDS.Import.ImporterBase

		Private _importFileDirectorySettings As kCura.WinEDDS.ImportFileDirectorySettings

		Private _folders As New System.Collections.Hashtable
		Protected WithEvents _recursiveFileProcessor As New kCura.Utility.RecursiveFileProcessor

    Private _emailExtractor As New kCura.WinEDDS.PropertyExtractor.EML
    Private _fileExtractor As New kCura.WinEDDS.PropertyExtractor.File

		Private _rootFolderACLID As Int32
		Private _caseFields() As kCura.EDDS.WebAPI.DocumentManagerBase.Field

		Private _uploader As kCura.WinEDDS.FileUploader
		Private _folderManager As kCura.WinEDDS.Service.FolderManager
		Private _documentManager As kCura.WinEDDS.Service.DocumentManager
		Private _fileManager As kCura.WinEDDS.Service.FileManager
		Private _credential As Net.NetworkCredential
		Private _cookieContainer As System.Net.CookieContainer
    Private _currentBatesNumber As Integer
    Private _fileExtentionsToImport As System.Collections.ArrayList

		Public Overrides Sub Import()

      ' create webservice proxies
			_documentManager = New kCura.WinEDDS.Service.DocumentManager(_credential, _cookieContainer)
			_folderManager = New kCura.WinEDDS.Service.FolderManager(_credential, _cookieContainer)
			_fileManager = New kCura.WinEDDS.Service.FileManager(_credential, _cookieContainer)
			_uploader = New kCura.WinEDDS.FileUploader(_credential, _documentManager.GetDocumentDirectoryByCaseArtifactID(_importFileDirectorySettings.CaseInfo.ArtifactID) & "\", _cookieContainer)

			' get case fields
			Dim fieldManager As New kCura.WinEDDS.Service.FieldQuery(_credential, _cookieContainer)
			_caseFields = fieldManager.RetrieveAllAsArray(_importFileDirectorySettings.CaseInfo.RootArtifactID)

      ' get valid extentiosn
      _fileExtentionsToImport = New System.Collections.ArrayList(_importFileDirectorySettings.FileExtentionsToImport.ToUpper.Split(CType(";", Char)))

			' identify the rootfolder ACL and add it to the fodler hash map
			_rootFolderACLID = _folderManager.Read(_importFileDirectorySettings.DestinationFolderID).AccessControlListID
			' extract parent folder
      Dim dirInfo As New System.IO.DirectoryInfo(_importFileDirectorySettings.FilePath)
      If Not dirInfo.Parent Is Nothing Then
        _folders.Add(dirInfo.Parent.FullName, New Folder(_importFileDirectorySettings.DestinationFolderID, dirInfo.Parent.FullName))
      Else
        _folders.Add("", New Folder(_importFileDirectorySettings.DestinationFolderID, ""))
      End If

      ' determine the number of records to process
      Me.ReportStatus("", "Counting number of files to import...")
      Me.TotalRecords = kCura.Utility.File.CountFilesInDirectory(_importFileDirectorySettings.FilePath, True) + 1

      ' set the batesnumber seed
      _currentBatesNumber = _importFileDirectorySettings.BatesNumberSeed

      ' begin the import
      Me.StartTime = DateTime.Now
      Me.ReportProgress()
      _recursiveFileProcessor.ProcessDirectory(_importFileDirectorySettings.FilePath)

		End Sub

		Private Sub _recursiveFileProcessor_OnProcessDirectory(ByVal evt As kCura.Utility.OnProcessFileEvent) Handles _recursiveFileProcessor.OnProcessDirectory
			Me.TotalRecordsProcessed = Me.TotalRecordsProcessed + 1
			CreateFolder(evt.FileName)
			Me.ReportStatus(evt.FileName, "Folder Created")
		End Sub

		Private Sub _recursiveFileProcessor_OnProcessFile(ByVal evt As kCura.Utility.OnProcessFileEvent) Handles _recursiveFileProcessor.OnProcessFile
			Me.TotalRecordsProcessed = Me.TotalRecordsProcessed + 1
      Me.ReportStatus(evt.FileName, "Begin Import")
      CreateDocumentRecord(evt.FileName)
			Me.ReportStatus(evt.FileName, "End Import")
			Me.ReportProgress()
		End Sub

#Region "EDDS Artifact Create"

		Private Sub CreateFolder(ByVal filePath As String)

      Dim dir As New System.IO.DirectoryInfo(filePath)
      Dim parent As String
      If dir.Parent Is Nothing Then
        parent = ""
      Else
        parent = dir.Parent.FullName
      End If
      Dim parentFolderID As Int32 = CType(_folders.Item(parent), Folder).FolderID
      Dim folderId As Int32 = _folderManager.Create(parentFolderID, dir.Name)
      _folders.Add(filePath, New Folder(folderId, filePath))

		End Sub

		Private Sub CreateDocumentRecord(ByVal filePath As String)

			Me.ReportStatus(filePath, "Extracting Properties")

      Dim fileProperties As System.Collections.Hashtable
      If _importFileDirectorySettings.EnronImport Then
        fileProperties = _emailExtractor.Extract(filePath)
      Else
        fileProperties = _fileExtractor.Extract(filePath)
      End If

      Dim fileInfo As New System.IO.FileInfo(filePath)
			Dim extension As String = fileInfo.Extension.ToUpper.Substring(1)
			If Not _fileExtentionsToImport.Contains(extension) Then
				Me.ReportWarning(filePath, "File Skipped")
				Exit Sub
			End If

			Dim parentFolderID As Int32 = CType(_folders.Item(fileInfo.Directory.FullName), Folder).FolderID

			Dim documentDTO As New kCura.EDDS.WebAPI.DocumentManagerBase.Document
			documentDTO.DocumentAgentFlags = New kCura.EDDS.WebAPI.DocumentManagerBase.DocumentAgentFlags
			documentDTO.DocumentAgentFlags.UpdateFullText = _importFileDirectorySettings.ExtractFullTextFromFile
			documentDTO.Fields = _caseFields
			documentDTO.ParentArtifactID = New NullableTypes.NullableInt32(parentFolderID)
			documentDTO.ContainerID = New NullableTypes.NullableInt32(_importFileDirectorySettings.CaseInfo.ArtifactID)
			documentDTO.AccessControlListIsInherited = True
			documentDTO.AccessControlListID = _rootFolderACLID

			Me.ReportStatus(filePath, "Building Field Map")

			' set the control field
			Dim field As kCura.EDDS.WebAPI.DocumentManagerBase.Field
			For Each field In documentDTO.Fields
				If field.FieldCategoryID = kCura.EDDS.Types.FieldCategory.FullText Then
					'field.Value = (New System.Text.ASCIIEncoding).GetBytes("")
					field.Value = String.Empty
					Dim fieldMap As kCura.WinEDDS.ImportFileDirectorySettings.FieldMap
					For Each fieldMap In _importFileDirectorySettings.FieldMappings
						If field.DisplayName = fieldMap.CaseField Then
							'field.Value = (New System.Text.ASCIIEncoding).GetBytes(CType(fileProperties.Item(fieldMap.FileField.ToLower), String))
							field.Value = _uploader.UploadTextAsFile(CType(fileProperties.Item(fieldMap.FileField.ToLower), String), _importFileDirectorySettings.DestinationFolderID, System.Guid.NewGuid.ToString)
						End If
					Next
				Else
					field.Value = String.Empty
					' determin if field is part of fieldMap
					Dim fieldMap As kCura.WinEDDS.ImportFileDirectorySettings.FieldMap
					For Each fieldMap In _importFileDirectorySettings.FieldMappings
						If field.DisplayName = fieldMap.CaseField Then
							If fieldMap.FileField = "AutoGenerated Bates Number" Then
								field.Value = _importFileDirectorySettings.BatesNumberPrefix + _currentBatesNumber.ToString("00000000")
								_currentBatesNumber = _currentBatesNumber + 1
								'ElseIf fieldMap.FileField = "MD5Hash" Then
								'	field.Value = kCura.Utility.File.GenerateMD5HashForFile(filePath)
							Else
								field.Value = fileProperties.Item(fieldMap.FileField.ToLower)
							End If
						End If
					Next
				End If
			Next

			Me.ReportStatus(filePath, "Creating EDDS Record")
			Dim documentArtifactID As Int32 = _documentManager.Create(documentDTO)

			Me.ReportStatus(filePath, "Uploading Document")
			If _importFileDirectorySettings.AttachFiles Then
				Try
					Dim fileIdentifier As String
					fileIdentifier = _uploader.UploadFile(filePath, documentArtifactID)
					If fileIdentifier <> String.Empty Then
						_fileManager.CreateFile(documentArtifactID, fileInfo.Name, fileIdentifier, 0, kCura.EDDS.Types.FileType.Native)
						Me.ReportStatus(filePath, "Extracting Full Text")
						'_documentManager.UpdateFullTextWithCrackedText(documentArtifactID, fileIdentifier)
					End If
				Catch ex As Exception
					Me.ReportError(filePath, "Exception Encountered Uploading Document" + vbCrLf + ex.ToString)
				End Try
			End If

		End Sub

#End Region

		Private Class Folder
			Public LocalPath As String
			Public FolderID As Int32

			Public Sub New(ByVal id As Int32, ByVal path As String)
				FolderID = id
				LocalPath = path
			End Sub
		End Class

		Public Sub New(ByVal importSettings As kCura.WinEDDS.ImportFileDirectorySettings, ByVal credential As Net.NetworkCredential, ByVal cookieContainer As System.Net.CookieContainer)
			_importFileDirectorySettings = importSettings
			_credential = credential
			_cookieContainer = cookieContainer
		End Sub

	End Class

End Namespace