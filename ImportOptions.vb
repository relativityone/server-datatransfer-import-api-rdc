Imports kCura.CommandLine
Imports kCura.EDDS.WinForm
Imports System.Exception
Imports kCura.EDDS.WinForm.Exceptions

Public Class ImportOptions

	Private _runningDirectory As String = System.IO.Directory.GetCurrentDirectory
	Private _exportErrorReportFile As Boolean = False
	Private _exportErrorLoadFile As Boolean = False
	Private _hasSetLoadFileLocation As Boolean = False

#Region "Properties"

	Private _loadFilePath As String
	Public Property LoadFilePath As String
		Get
			return _loadFilePath
		End Get
	    Private Set(value As String)
			_loadFilePath = value
	    End Set
	End Property

	Private _selectedCaseInfo As Relativity.CaseInfo
	Public Property SelectedCaseInfo As Relativity.CaseInfo
		Get
			return _selectedCaseInfo
		End Get
	    Private Set(value As Relativity.CaseInfo)
			_selectedCaseInfo = value
	    End Set
	End Property

	Private _selectedNativeLoadFile As New kCura.WinEDDS.LoadFile
	Public Property SelectedNativeLoadFile As kCura.WinEDDS.LoadFile
		Get
			return _selectedNativeLoadFile
		End Get
	    Private Set(value As kCura.WinEDDS.LoadFile)
			_selectedNativeLoadFile = value
	    End Set
	End Property

	Private _selectedImageLoadFile As kCura.WinEDDS.ImageLoadFile
	Public Property SelectedImageLoadFile As kCura.WinEDDS.ImageLoadFile
		Get
			return _selectedImageLoadFile
		End Get
	    Private Set(value As kCura.WinEDDS.ImageLoadFile)
			_selectedImageLoadFile = value
	    End Set
	End Property

	Private _sourceFileEncoding As System.Text.Encoding
	Public Property SourceFileEncoding As System.Text.Encoding
		Get
			return _sourceFileEncoding
		End Get
	    Private Set(value As System.Text.Encoding)
			_sourceFileEncoding = value
	    End Set
	End Property

	Private _extractedTextFileEncoding As System.Text.Encoding
	Public Property ExtractedTextFileEncoding As System.Text.Encoding
		Get
			return _extractedTextFileEncoding
		End Get
	    Private Set(value As System.Text.Encoding)
			_extractedTextFileEncoding = value
	    End Set
	End Property

	Private _selectedCasePath As String = ""
	Public Property SelectedCasePath As String
		Get
			return _selectedCasePath
		End Get
	    Private Set(value As String)
			_selectedCasePath = value
	    End Set
	End Property

	Private _copyFilesToDocumentRepository As Boolean = True
	Public Property CopyFilesToDocumentRepository As Boolean
		Get
			return _copyFilesToDocumentRepository
		End Get
	    Private Set(value As Boolean)
			_copyFilesToDocumentRepository = value
	    End Set
	End Property

	Private _destinationFolderID As Int32
	Public Property DestinationFolderID As Int32
		Get
			return _destinationFolderID
		End Get
	    Private Set(value As Int32)
			_destinationFolderID = value
	    End Set
	End Property

	Private _errorReportFileLocation As String = ""
	Public Property ErrorReportFileLocation As String
		Get
			return _errorReportFileLocation
		End Get
	    Private Set(value As String)
			_errorReportFileLocation = value
	    End Set
	End Property

	Private _errorLoadFileLocation As String = ""
	Public Property ErrorLoadFileLocation As String
		Get
			return _errorLoadFileLocation
		End Get
	    Private Set(value As String)
			_errorLoadFileLocation = value
	    End Set
	End Property

	Private _startLineNumber As Int64
	Public Property StartLineNumber As Int64
		Get
			return _startLineNumber
		End Get
	    Private Set(value As Int64)
			_startLineNumber = value
	    End Set
	End Property

	Private _loadMode As LoadMode

	Sub New()
	End Sub

	Public Property LoadMode As LoadMode
		Get
			return _loadMode
		End Get
	    Private Set(value As LoadMode)
			_loadMode = value
	    End Set
	End Property

#End Region

	Public Sub SetOptions(ByVal commandLine As kCura.CommandLine.CommandList, ByRef application As kCura.EDDS.WinForm.Application)
		SetLoadType(GetValueFromCommandListByFlag(commandLine, "m"))
		SetCaseInfo(GetValueFromCommandListByFlag(commandLine, "c"), application)
		SetFileLocation(GetValueFromCommandListByFlag(commandLine, "f"))
		SetSavedMapLocation(GetValueFromCommandListByFlag(commandLine, "k"), LoadMode, application)
		EnsureLoadFileLocation()
		SetSourceFileEncoding(GetValueFromCommandListByFlag(commandLine, "e"))
		SetFullTextFileEncoding(GetValueFromCommandListByFlag(commandLine, "x"))
		SetSelectedCasePath(GetValueFromCommandListByFlag(commandLine, "r"))
		SetCopyFilesToDocumentRepository(GetValueFromCommandListByFlag(commandLine, "l"))
		SetDestinationFolderID(GetValueFromCommandListByFlag(commandLine, "d"), application)
		SetStartLineNumber(GetValueFromCommandListByFlag(commandLine, "s"))
		SetExportErrorReportLocation(commandLine)
		SetExportErrorFileLocation(commandLine)
	End Sub

#Region " Input Validation "

	Private Sub EnsureLoadFileLocation()
		If String.IsNullOrEmpty(LoadFilePath) Then Throw New LoadFilePathException
		If Not System.IO.File.Exists(LoadFilePath) Then Throw New LoadFilePathException(LoadFilePath)
	End Sub

	Private Function EnsureEncoding() As Boolean
		Dim determinedEncoding As System.Text.Encoding = kCura.WinEDDS.Utility.DetectEncoding(LoadFilePath, True).DeterminedEncoding
		If determinedEncoding Is Nothing Then Return True
		Return (determinedEncoding.Equals(SourceFileEncoding))
	End Function

	Private Sub SetFileLocation(ByVal path As String)
		LoadFilePath = path
		_hasSetLoadFileLocation = Not String.IsNullOrEmpty(LoadFilePath)
	End Sub

	Private Sub SetCaseInfo(ByVal caseID As String, ByRef application As kCura.EDDS.WinForm.Application)
		Try
			Dim caseManager As New kCura.WinEDDS.Service.CaseManager(application.Credential, application.CookieContainer)
			SelectedCaseInfo = caseManager.Read(Int32.Parse(caseID))
		Catch ex As System.Exception
			Throw New CaseArtifactIdException(caseID, ex)
		End Try
		If SelectedCaseInfo Is Nothing Then Throw New CaseArtifactIdException(caseID)
		application.RefreshSelectedCaseInfo(SelectedCaseInfo)
	End Sub

	Private Sub SetExportErrorReportLocation(ByVal commandLine As kCura.CommandLine.CommandList)
		For Each command As kCura.CommandLine.Command In commandLine
			If command.Directive.ToLower.Replace("-", "").Replace("/", "") = "er" Then
				_exportErrorReportFile = True
				If command.Value Is Nothing OrElse command.Value = "" Then
					ErrorReportFileLocation = _runningDirectory.TrimEnd("\".ToCharArray) & "\ErrorReport_" & System.DateTime.Now.Ticks.ToString & ".csv"
				Else
					ErrorReportFileLocation = String.Copy(command.Value)
				End If
				Try
					If Not System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(ErrorReportFileLocation)) Then System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(ErrorReportFileLocation))
					System.IO.File.Create(ErrorReportFileLocation).Close()
					System.IO.File.Delete(ErrorReportFileLocation)
				Catch ex As System.Exception
					Throw New InvalidPathLocationException(ErrorReportFileLocation, "error report", ex)
				End Try
				'Return command.Value
			End If
		Next
		If Not _exportErrorReportFile Then ErrorReportFileLocation = ""
	End Sub

	Private Sub SetExportErrorFileLocation(ByVal commandLine As kCura.CommandLine.CommandList)
		For Each command As kCura.CommandLine.Command In commandLine
			If command.Directive.ToLower.Replace("-", "").Replace("/", "") = "ef" Then
				_exportErrorLoadFile = True
				If command.Value Is Nothing OrElse command.Value = "" Then
					Dim extension As String = System.IO.Path.GetExtension(LoadFilePath)
					If extension Is Nothing Then extension = "txt"
					extension = extension.TrimStart(".".ToCharArray)
					ErrorLoadFileLocation = _runningDirectory.TrimEnd("\".ToCharArray) & "\ErrorLines_" & System.DateTime.Now.Ticks.ToString & "." & extension
				Else
					ErrorLoadFileLocation = String.Copy(command.Value)
				End If
				Try
					If Not System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(ErrorLoadFileLocation)) Then System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(ErrorLoadFileLocation))
					System.IO.File.Create(ErrorLoadFileLocation).Close()
					System.IO.File.Delete(ErrorLoadFileLocation)
				Catch ex As System.Exception
					Throw New InvalidPathLocationException(ErrorReportFileLocation, "error file", ex)
				End Try
				'Return command.Value
			End If
		Next
		If Not _exportErrorLoadFile Then ErrorLoadFileLocation = ""
	End Sub

	Private Sub SetSavedMapLocation(ByVal path As String, ByVal currentLoadMode As LoadMode, ByRef application As kCura.EDDS.WinForm.Application)
		Try
			Select Case currentLoadMode
				Case LoadMode.Image
					If Not System.IO.File.Exists(path) Then Throw New SavedSettingsFilePathException(path)
					SelectedImageLoadFile = application.ReadImageLoadFile(path)
					If _hasSetLoadFileLocation Then SelectedImageLoadFile.FileName = LoadFilePath
				Case LoadMode.Native, LoadMode.DynamicObject
					If Not System.IO.File.Exists(path) Then Throw New SavedSettingsFilePathException(path)
					Dim sr As New System.IO.StreamReader(path)
					Dim doc As String = sr.ReadToEnd
					sr.Close()
					Dim xmlDoc As New System.Xml.XmlDocument
					xmlDoc.LoadXml(doc)

					sr = New System.IO.StreamReader(path)
					Dim stringr As New System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(application.CleanLoadFile(xmlDoc)))
					Dim tempLoadFile As LoadFile
					Dim deserializer As New System.Runtime.Serialization.Formatters.Soap.SoapFormatter
					tempLoadFile = DirectCast(deserializer.Deserialize(stringr), LoadFile)
					sr.Close()
					If Not String.IsNullOrEmpty(LoadFilePath) Then
						tempLoadFile.FilePath = LoadFilePath
					Else
						LoadFilePath = tempLoadFile.FilePath
					End If
					tempLoadFile.OverwriteDestination = Utility.ConvertLegacyOverwriteDestinationValueToEnum(tempLoadFile.OverwriteDestination)
					tempLoadFile.ForceFolderPreview = False
					tempLoadFile.CaseInfo = SelectedCaseInfo
					tempLoadFile.CopyFilesToDocumentRepository = True           'LoadFile.CopyFilesToDocumentRepository
					tempLoadFile.SelectedCasePath = SelectedCaseInfo.DocumentPath           ''''''''''
					tempLoadFile.CaseDefaultPath = SelectedCaseInfo.DocumentPath
					tempLoadFile.Credentials = application.Credential
					tempLoadFile.DestinationFolderID = 35
					tempLoadFile.ExtractedTextFileEncoding = System.Text.Encoding.Unicode
					tempLoadFile.SourceFileEncoding = System.Text.Encoding.Default
					Dim artifactTypeID As Int32
					If currentLoadMode = LoadMode.Native Then
						artifactTypeID = Relativity.ArtifactType.Document
					Else
						artifactTypeID = tempLoadFile.ArtifactTypeID
					End If
					tempLoadFile.SelectedIdentifierField = application.CurrentFields(artifactTypeID, True).IdentifierFields(0)
					Dim mapItemToRemove As LoadFileFieldMap.LoadFileFieldMapItem = Nothing
					If tempLoadFile.GroupIdentifierColumn = "" AndAlso System.IO.File.Exists(tempLoadFile.FilePath) Then
						Dim fieldMapItem As kCura.WinEDDS.LoadFileFieldMap.LoadFileFieldMapItem
						For Each fieldMapItem In tempLoadFile.FieldMap
							If Not fieldMapItem.DocumentField Is Nothing AndAlso
							 fieldMapItem.NativeFileColumnIndex >= 0 AndAlso
							 fieldMapItem.DocumentField.FieldName.ToLower = "group identifier" Then
								tempLoadFile.GroupIdentifierColumn = application.GetColumnHeadersFromLoadFile(tempLoadFile, tempLoadFile.FirstLineContainsHeaders)(fieldMapItem.NativeFileColumnIndex)
							End If
						Next
					End If
					If Not mapItemToRemove Is Nothing Then tempLoadFile.FieldMap.Remove(mapItemToRemove)

					For Each fieldMapItem As kCura.WinEDDS.LoadFileFieldMap.LoadFileFieldMapItem In tempLoadFile.FieldMap
						If Not fieldMapItem.DocumentField Is Nothing Then
							Try
								Dim thisField As DocumentField = application.CurrentFields(tempLoadFile.ArtifactTypeID).Item(fieldMapItem.DocumentField.FieldID)
								fieldMapItem.DocumentField.AssociatedObjectTypeID = thisField.AssociatedObjectTypeID
								fieldMapItem.DocumentField.UseUnicode = thisField.UseUnicode
								fieldMapItem.DocumentField.CodeTypeID = thisField.CodeTypeID
								fieldMapItem.DocumentField.FieldLength = thisField.FieldLength
								fieldMapItem.DocumentField.ImportBehavior = thisField.ImportBehavior
							Catch
							End Try
						End If
					Next
					SelectedNativeLoadFile = tempLoadFile
					SelectedNativeLoadFile.SelectedCasePath = SelectedCaseInfo.DocumentPath
					SelectedNativeLoadFile.Credentials = application.Credential
					SelectedNativeLoadFile.CookieContainer = application.CookieContainer
				Case LoadMode.Application
					If System.IO.File.Exists(path) Then Throw New InvalidOperationException("Load file is not supported for application imports")
			End Select
		Catch ex As System.Exception
			Throw New SavedSettingsRehydrationException(path, ex)
		End Try
	End Sub

	Private Sub SetSourceFileEncoding(ByVal value As String)
		If value Is Nothing OrElse value = "" Then
			SourceFileEncoding = System.Text.Encoding.Default
		Else
			Try
				SourceFileEncoding = System.Text.Encoding.GetEncoding(Int32.Parse(value))
			Catch ex As System.Exception
				Throw New EncodingException(value, "source file", ex)
			End Try
		End If
	End Sub
	Private Sub SetFullTextFileEncoding(ByVal value As String)
		If value Is Nothing OrElse value = "" Then
			ExtractedTextFileEncoding = System.Text.Encoding.Default
		Else
			Try
				ExtractedTextFileEncoding = System.Text.Encoding.GetEncoding(Int32.Parse(value))
			Catch ex As System.Exception
				Throw New EncodingException(value, "extracted text files", ex)
			End Try
		End If
	End Sub
	Private Sub SetSelectedCasePath(ByVal value As String)
		If value Is Nothing OrElse value = "" Then
			SelectedCasePath = SelectedCaseInfo.DocumentPath
		Else
			SelectedCasePath = value.TrimEnd("\"c) & "\"
			If Config.CloudInstance AndAlso Not SelectedCaseInfo.DocumentPath.Equals(SelectedCasePath) Then
				Throw New MustCopyFilesToRepositoryException()
			End If
		End If
		
	End Sub
	Private Sub SetCopyFilesToDocumentRepository(ByVal value As String)
		If value Is Nothing OrElse value = "" Then
			CopyFilesToDocumentRepository = True
		Else
			CopyFilesToDocumentRepository = True
			Try
				CopyFilesToDocumentRepository = Boolean.Parse(value)
			Catch ex As System.Exception
				Select Case value.ToLower.Substring(0, 1)
					Case "t", "y", "1"
						CopyFilesToDocumentRepository = True
					Case "f", "n", "0"
						CopyFilesToDocumentRepository = False
				End Select
				If value.ToLower = "on" Then
					CopyFilesToDocumentRepository = True
				ElseIf value.ToLower = "off" Then
					CopyFilesToDocumentRepository = False
				End If
			End Try
		End If
		If Config.CloudInstance AndAlso Not CopyFilesToDocumentRepository Then
			Throw New MustCopyFilesToRepositoryException()
		End If
	End Sub

	Private Sub SetDestinationFolderID(ByVal value As String, ByRef application As kCura.EDDS.WinForm.Application)
		If value Is Nothing OrElse value = "" Then
			DestinationFolderID = SelectedCaseInfo.RootFolderID
		Else
			Dim folderManager As New kCura.WinEDDS.Service.FolderManager(application.Credential, application.CookieContainer)
			Dim folderExists As Boolean = False
			Try
				folderExists = folderManager.Exists(SelectedCaseInfo.ArtifactID, Int32.Parse(value))
			Catch
			End Try
			If folderExists Then
				DestinationFolderID = Int32.Parse(value)
			Else
				Throw New FolderIdException(value)
			End If
		End If

	End Sub

	Private Sub SetLoadType(ByVal loadTypeString As String)
		Select Case loadTypeString.ToLower.Trim
			Case "i", "image"
				LoadMode = LoadMode.Image
			Case "n", "native"
				LoadMode = LoadMode.Native
			Case "o", "object"
				LoadMode = LoadMode.DynamicObject
			Case "a", "application"
				LoadMode = LoadMode.Application
			Case Else
				Throw New NoLoadTypeModeSetException
		End Select
	End Sub

	Private Sub SetStartLineNumber(ByVal value As String)
		If value Is Nothing OrElse value = "" Then
			StartLineNumber = 0
		Else
			Try
				StartLineNumber = Int64.Parse(value)
				If StartLineNumber < 0 Then
					Throw New StartLineNumberException(value)
				End If
			Catch ex As System.Exception
				Throw New StartLineNumberException(value, ex)
			End Try
		End If
	End Sub

	Public Shared Function GetValueFromCommandListByFlag(ByVal commandList As kCura.CommandLine.CommandList, ByVal flag As String) As String
			For Each command As kCura.CommandLine.Command In commandList
				If command.Directive.ToLower.Replace("-", "").Replace("/", "") = flag.ToLower Then
					If command.Value Is Nothing Then Return ""
					Return command.Value
				End If
			Next
			Return ""
	End Function

#End Region

End Class

