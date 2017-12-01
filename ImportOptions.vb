Imports kCura.CommandLine
Imports kCura.EDDS.WinForm
Imports System.Exception
Imports System.Threading.Tasks
Imports kCura.EDDS.WinForm.Exceptions
Imports System.Xml.Linq
Imports System.Linq
Imports System.Collections.Generic

Public Class ImportOptions

	Private _runningDirectory As String = System.IO.Directory.GetCurrentDirectory
	Private _exportErrorReportFile As Boolean = False
	Private _exportErrorLoadFile As Boolean = False
	Private _hasSetLoadFileLocation As Boolean = False
	Private _exportSettings As ExportFile

#Region "Properties"

	Private _loadFilePath As String
	Public Property LoadFilePath As String
		Get
			Return _loadFilePath
		End Get
		Private Set(value As String)
			_loadFilePath = value
		End Set
	End Property

	Private _selectedCaseInfo As Relativity.CaseInfo
	Public Property SelectedCaseInfo As Relativity.CaseInfo
		Get
			Return _selectedCaseInfo
		End Get
		Private Set(value As Relativity.CaseInfo)
			_selectedCaseInfo = value
		End Set
	End Property

	Private _selectedNativeLoadFile As New kCura.WinEDDS.LoadFile

	Public Property SelectedExportSettings As ExportFile

	Public Property SelectedNativeLoadFile As kCura.WinEDDS.LoadFile
		Get
			Return _selectedNativeLoadFile
		End Get
		Private Set(value As kCura.WinEDDS.LoadFile)
			_selectedNativeLoadFile = value
		End Set
	End Property

	Private _selectedImageLoadFile As kCura.WinEDDS.ImageLoadFile
	Public Property SelectedImageLoadFile As kCura.WinEDDS.ImageLoadFile
		Get
			Return _selectedImageLoadFile
		End Get
		Private Set(value As kCura.WinEDDS.ImageLoadFile)
			_selectedImageLoadFile = value
		End Set
	End Property

	Private _sourceFileEncoding As System.Text.Encoding
	Public Property SourceFileEncoding As System.Text.Encoding
		Get
			Return _sourceFileEncoding
		End Get
		Private Set(value As System.Text.Encoding)
			_sourceFileEncoding = value
		End Set
	End Property

	Private _extractedTextFileEncoding As System.Text.Encoding
	Public Property ExtractedTextFileEncoding As System.Text.Encoding
		Get
			Return _extractedTextFileEncoding
		End Get
		Private Set(value As System.Text.Encoding)
			_extractedTextFileEncoding = value
		End Set
	End Property

	Private _selectedCasePath As String = ""
	Public Property SelectedCasePath As String
		Get
			Return _selectedCasePath
		End Get
		Private Set(value As String)
			_selectedCasePath = value
		End Set
	End Property

	Private _copyFilesToDocumentRepository As Boolean = True
	Public Property CopyFilesToDocumentRepository As Boolean
		Get
			Return _copyFilesToDocumentRepository
		End Get
		Private Set(value As Boolean)
			_copyFilesToDocumentRepository = value
		End Set
	End Property

	Private _destinationFolderID As Int32
	Public Property DestinationFolderID As Int32
		Get
			Return _destinationFolderID
		End Get
		Private Set(value As Int32)
			_destinationFolderID = value
		End Set
	End Property

	Private _errorReportFileLocation As String = ""
	Public Property ErrorReportFileLocation As String
		Get
			Return _errorReportFileLocation
		End Get
		Private Set(value As String)
			_errorReportFileLocation = value
		End Set
	End Property

	Private _errorLoadFileLocation As String = ""
	Public Property ErrorLoadFileLocation As String
		Get
			Return _errorLoadFileLocation
		End Get
		Private Set(value As String)
			_errorLoadFileLocation = value
		End Set
	End Property

	Private _startLineNumber As Int64
	Public Property StartLineNumber As Int64
		Get
			Return _startLineNumber
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
			Return _loadMode
		End Get
		Private Set(value As LoadMode)
			_loadMode = value
		End Set
	End Property

#End Region

	Public Async Function SetOptions(ByVal commandLine As kCura.CommandLine.CommandList) As Task
		SetLoadType(GetValueFromCommandListByFlag(commandLine, "m"))
		Await SetCaseInfo(GetValueFromCommandListByFlag(commandLine, "c"))
		If LoadMode = LoadMode.Export Then
			Await SetSavedMapLocation(GetValueFromCommandListByFlag(commandLine, "k"), LoadMode)
		Else
			SetFileLocation(GetValueFromCommandListByFlag(commandLine, "f"))
			Await SetSavedMapLocation(GetValueFromCommandListByFlag(commandLine, "k"), LoadMode)
			EnsureLoadFileLocation()
			SetSourceFileEncoding(GetValueFromCommandListByFlag(commandLine, "e"))
			SetFullTextFileEncoding(GetValueFromCommandListByFlag(commandLine, "x"))
			SetSelectedCasePath(GetValueFromCommandListByFlag(commandLine, "r"))
			SetCopyFilesToDocumentRepository(GetValueFromCommandListByFlag(commandLine, "l"))
			Await SetDestinationFolderID(GetValueFromCommandListByFlag(commandLine, "d"))
			SetStartLineNumber(GetValueFromCommandListByFlag(commandLine, "s"))
			SetExportErrorReportLocation(commandLine)
			SetExportErrorFileLocation(commandLine)
		End If
	End Function

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

	Private Async Function SetCaseInfo(ByVal caseID As String) As Task
		Try
			Dim caseManager As New kCura.WinEDDS.Service.CaseManager(Await Application.Instance.GetCredentialsAsync(), Application.Instance.CookieContainer)
			SelectedCaseInfo = caseManager.Read(Int32.Parse(caseID))
		Catch ex As System.Exception
			Throw New CaseArtifactIdException(caseID, ex)
		End Try
		If SelectedCaseInfo Is Nothing Then Throw New CaseArtifactIdException(caseID)
		Await Application.Instance.RefreshSelectedCaseInfo(SelectedCaseInfo)
	End Function

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

	Private Async Function SetSavedMapLocation(ByVal path As String, ByVal currentLoadMode As LoadMode) As Task
		Try
			Select Case currentLoadMode
				Case LoadMode.Image
					If Not System.IO.File.Exists(path) Then Throw New SavedSettingsFilePathException(path)
					SelectedImageLoadFile = Await Application.Instance.ReadImageLoadFile(path)
					If _hasSetLoadFileLocation Then SelectedImageLoadFile.FileName = LoadFilePath
				Case LoadMode.Native, LoadMode.DynamicObject
					If Not System.IO.File.Exists(path) Then Throw New SavedSettingsFilePathException(path)
					Dim sr As New System.IO.StreamReader(path)
					Dim doc As String = sr.ReadToEnd
					sr.Close()
					Dim xmlDoc As New System.Xml.XmlDocument
					xmlDoc.LoadXml(doc)

					sr = New System.IO.StreamReader(path)
					Dim stringr As New System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(Application.Instance.CleanLoadFile(xmlDoc)))
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
					tempLoadFile.Credentials = Await Application.Instance.GetCredentialsAsync()
					tempLoadFile.DestinationFolderID = 35
					tempLoadFile.ExtractedTextFileEncoding = System.Text.Encoding.Unicode
					tempLoadFile.SourceFileEncoding = System.Text.Encoding.Default
					Dim artifactTypeID As Int32
					If currentLoadMode = LoadMode.Native Then
						artifactTypeID = Relativity.ArtifactType.Document
					Else
						artifactTypeID = tempLoadFile.ArtifactTypeID
					End If
					tempLoadFile.SelectedIdentifierField = (Await Application.Instance.CurrentFields(artifactTypeID, True)).IdentifierFields(0)
					Dim mapItemToRemove As LoadFileFieldMap.LoadFileFieldMapItem = Nothing
					If tempLoadFile.GroupIdentifierColumn = "" AndAlso System.IO.File.Exists(tempLoadFile.FilePath) Then
						Dim fieldMapItem As kCura.WinEDDS.LoadFileFieldMap.LoadFileFieldMapItem
						For Each fieldMapItem In tempLoadFile.FieldMap
							If Not fieldMapItem.DocumentField Is Nothing AndAlso
							 fieldMapItem.NativeFileColumnIndex >= 0 AndAlso
							 fieldMapItem.DocumentField.FieldName.ToLower = "group identifier" Then
								tempLoadFile.GroupIdentifierColumn = Application.Instance.GetColumnHeadersFromLoadFile(tempLoadFile, tempLoadFile.FirstLineContainsHeaders)(fieldMapItem.NativeFileColumnIndex)
							End If
						Next
					End If
					If Not mapItemToRemove Is Nothing Then tempLoadFile.FieldMap.Remove(mapItemToRemove)

					For Each fieldMapItem As kCura.WinEDDS.LoadFileFieldMap.LoadFileFieldMapItem In tempLoadFile.FieldMap
						If Not fieldMapItem.DocumentField Is Nothing Then
							Try
								Dim thisField As DocumentField = (Await Application.Instance.CurrentFields(tempLoadFile.ArtifactTypeID)).Item(fieldMapItem.DocumentField.FieldID)
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
					SelectedNativeLoadFile.Credentials = Await Application.Instance.GetCredentialsAsync()
					SelectedNativeLoadFile.CookieContainer = Application.Instance.CookieContainer
				Case LoadMode.Application
					If System.IO.File.Exists(path) Then Throw New InvalidOperationException("Load file is not supported for application imports")
				Case LoadMode.Export
					If Not System.IO.File.Exists(path) Then Throw New SavedSettingsFilePathException(path)

					Dim settings As String
					Using sr As New System.IO.StreamReader(path)
						settings = sr.ReadToEnd()
					End Using
					Dim settingsHelper As New ExportFileSerializer()
					Dim xml As XDocument = XDocument.Parse(settings)
					Dim deserializedSettings As ExportFile = settingsHelper.DeserializeExportFile(xml)
					Dim artifactTypeID As Int32 = 0
					If Not String.IsNullOrWhiteSpace(deserializedSettings.ObjectTypeName) Then
						Dim objectTypeManager As New kCura.WinEDDS.Service.ObjectTypeManager(Await _application.GetCredentialsAsync(), _application.CookieContainer)
						Dim uploadableObjectTypes As DataRow() = objectTypeManager.RetrieveAllUploadable(_application.SelectedCaseInfo.ArtifactID).Tables(0).Select()
						Dim selectedObjectType As DataRow = uploadableObjectTypes.FirstOrDefault(Function(row) deserializedSettings.ObjectTypeName.Equals(row("Name").ToString(), StringComparison.InvariantCultureIgnoreCase))
						If selectedObjectType IsNot Nothing Then
							artifactTypeID = CInt(selectedObjectType("DescriptorArtifactTypeID"))
						End If
					End If
					If artifactTypeID = 0 Then
						Throw New ArgumentOutOfRangeException($"The object type '{deserializedSettings.ObjectTypeName}' specified in the saved map does not exist in the selected workspace.")
					End If
					Dim activeSettings As ExportFile = Await Application.Instance.GetNewExportFileSettingsObject(deserializedSettings.ArtifactID, Me.SelectedCaseInfo, deserializedSettings.TypeOfExport, artifactTypeID)
					activeSettings = settingsHelper.PopulateDeserializedExportFile(activeSettings, deserializedSettings)
					activeSettings.LoadFileEncoding = If(deserializedSettings.LoadFileEncoding, System.Text.Encoding.UTF8)
					activeSettings.TextFileEncoding = If(deserializedSettings.TextFileEncoding, System.Text.Encoding.UTF8)
					Dim allFields As DocumentFieldCollection = Await Application.Instance.CurrentFields(artifactTypeID, True)
					activeSettings.FileField = deserializedSettings.FileField
					If activeSettings.FileField IsNot Nothing Then
						activeSettings.FileField = allFields.FirstOrDefault(Function(f) f.FieldName.Equals(activeSettings.FileField.FieldName, StringComparison.InvariantCultureIgnoreCase))
						If activeSettings.FileField Is Nothing Then Throw New System.Exception($"Mapped file field {activeSettings.FileField.FieldName} doesn't exist in the selected workspace.")
					End If

					activeSettings.SelectedTextFields = EnsureSelectedFields(deserializedSettings.SelectedTextFields, activeSettings.AllExportableFields, "Long Text fields")
					activeSettings.SelectedViewFields = EnsureSelectedFields(deserializedSettings.SelectedViewFields, activeSettings.AllExportableFields, "fields")
					activeSettings.ImagePrecedence = deserializedSettings.ImagePrecedence
					If Not activeSettings.ImagePrecedence Is Nothing AndAlso activeSettings.ImagePrecedence.Any() Then
						Dim dt As DataTable = Await _application.GetProductionPrecendenceList(Me.SelectedCaseInfo)
						Dim availablePrecendenceItems As Pair() = dt.Select().Select(Function(row) New Pair(row("Value").ToString, row("Display").ToString)).ToArray()
						activeSettings.ImagePrecedence = EnsureImagePrecedence(availablePrecendenceItems, activeSettings.ImagePrecedence)
					End If
					activeSettings.CookieContainer = _application.CookieContainer
					Dim potentialErrors As String = New Exporters.Validator.ExportInitializationValidator().IsValid(activeSettings)
					If Not String.IsNullOrEmpty(potentialErrors) Then
						Throw New InvalidOperationException(potentialErrors)
					End If
					SelectedExportSettings = activeSettings

			End Select
		Catch ex As System.Exception
			Throw New SavedSettingsRehydrationException(path, ex)
		End Try
	End Function

	Private Function EnsureSelectedFields(selectedFields As ViewFieldInfo(), allFields As ViewFieldInfo(), collectionName As String) As ViewFieldInfo()
		Dim nameProvider As Func(Of ViewFieldInfo, String) = Function(f) f.DisplayName
		Return EnsureSelectedFields(selectedFields, allFields, nameProvider, collectionName)
	End Function

	Private Function EnsureSelectedFields(Of T)(selectedFields As T(), allFields As T(), nameProvider As Func(Of T, String), collectionName As String) As T()
		If selectedFields Is Nothing OrElse Not selectedFields.Any() Then
			Return New T() {}
		End If
		Dim misses As New List(Of T)()
		Dim retval As T() = selectedFields.Select(
		Function(f)
			Dim workspaceField As T = allFields.FirstOrDefault(Function(af) nameProvider(af).Equals(nameProvider(f), StringComparison.InvariantCultureIgnoreCase))
			If workspaceField Is Nothing Then misses.Add(f)
			Return workspaceField
		End Function).
		ToArray()
		If misses.Any() Then
			Dim missedFieldMessage As String = String.Join(", ", misses.Select(Function(f) $"'{nameProvider(f)}'"))
			Throw New System.Exception($"These {collectionName} in the saved map don't exist in the selected workspace: {missedFieldMessage}")
		End If
		Return retval
	End Function

	Private Function EnsureImagePrecedence(selectedPrecedenceItems As Pair(), availablePrecendenceItems As Pair()) As Pair()
		Dim nameProvider As Func(Of Pair, String) = Function(pair) pair.Display
		Return EnsureSelectedFields(selectedPrecedenceItems, availablePrecendenceItems, nameProvider, "image precedence productions")
	End Function

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
			If Config.CloudInstance AndAlso Not SelectedCasePath.Equals(SelectedCaseInfo?.DocumentPath) Then
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

	Private Async Function SetDestinationFolderID(ByVal value As String) As Task
		If value Is Nothing OrElse value = "" Then
			DestinationFolderID = SelectedCaseInfo.RootFolderID
		Else
			Dim folderManager As New kCura.WinEDDS.Service.FolderManager(Await Application.Instance.GetCredentialsAsync(), Application.Instance.CookieContainer)
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

	End Function

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
			Case "e", "export"
				LoadMode = LoadMode.Export
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

