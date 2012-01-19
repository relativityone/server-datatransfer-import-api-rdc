Imports System.Net

Namespace kCura.Relativity.DataReaderClient
	Public Class ImportBulkArtifactJob
		Implements IImportNotifier



#Region "Private Variables"

		Private _jobReport As JobReport
		Private _bulkLoadFileFieldDelimiter As String
		Private ReadOnly _controlNumberFieldName As String
		Private _docIDFieldCollection As WinEDDS.DocumentField()

		Private _hasErrors As Boolean
		Private _controller As Windows.Process.Controller
		Private _nativeDataReader As SourceIDataReader
		Private _nativeSettings As ImportSettingsBase
		Private WithEvents _observer As Windows.Process.ProcessObserver
		
#End Region

#Region "Constructors"
		''' <summary>
		''' Creates a new job that can bulk insert a large amount of artifacts.
		''' </summary>
		Public Sub New()
			_controlNumberFieldName = "control number"
			_nativeSettings = New Settings
			_nativeDataReader = New SourceIDataReader

			_bulkLoadFileFieldDelimiter = Global.Relativity.Constants.DEFAULT_FIELD_DELIMITER
			_hasErrors = False
		End Sub

		Friend Sub New(ByVal relativityUserName As String, ByVal password As String)
			Me.New()
			Settings.RelativityUsername = relativityUserName
			Settings.RelativityPassword = password
		End Sub

#End Region

#Region "Events"
		Public Event OnMessage(ByVal status As Status)
		Public Event OnError(ByVal row As IDictionary)
		Public Event OnComplete(ByVal jobReport As JobReport) Implements IImportNotifier.OnComplete
		Public Event OnFatalException(ByVal jobReport As JobReport) Implements IImportNotifier.OnFatalException
		Public Event OnProgress(ByVal completedRow As Long) Implements IImportNotifier.OnProgress


#End Region

#Region "Public Routines"
		Public Sub Execute()
			_jobReport = New JobReport()
			_jobReport.StartTime = DateTime.Now()

			If IsSettingsValid() Then

				RaiseEvent OnMessage(New Status("Getting source data from database"))

				Dim process As WinEDDS.ImportExtension.DataReaderImporterProcess = New WinEDDS.ImportExtension.DataReaderImporterProcess(SourceData.SourceData)
				_observer = process.ProcessObserver
				_controller = process.ProcessController

				If DisableNativeValidation.HasValue Then process.DisableNativeValidation = DisableNativeValidation.Value
				If DisableNativeLocationValidation.HasValue Then process.DisableNativeLocationValidation = DisableNativeLocationValidation.Value
				process.DisableUserSecurityCheck = Me.DisableUserSecurityCheck
				process.AuditLevel = Me.AuditLevel

				RaiseEvent OnMessage(New Status("Updating settings"))
				process.LoadFile = CreateLoadFile(Settings)
				process.BulkLoadFileFieldDelimiter = _bulkLoadFileFieldDelimiter

				RaiseEvent OnMessage(New Status("Executing"))
				Try
					process.StartProcess()
				Catch ex As Exception
					RaiseEvent OnMessage(New Status(String.Format("Exception: {0}", ex.ToString)))
					_jobReport.FatalException = ex
					RaiseFatalException()
				End Try
			Else
				RaiseEvent OnMessage(New Status("There was an error in your settings.  Import aborted."))
				' exception was set in the IsSettingsValid function
				RaiseFatalException()
			End If
		End Sub

		Private Sub RaiseFatalException()
			_jobReport.EndTime = DateTime.Now
			RaiseEvent OnFatalException(_jobReport)
		End Sub

		Private Sub RaiseComplete()
			_jobReport.EndTime = DateTime.Now
			RaiseEvent OnComplete(_jobReport)
		End Sub


		''' <summary>
		''' Exports the error log file from the import if any errors occurred.
		''' If no errors occurred, no file is copied.
		''' </summary>
		''' <param name="filePathAndName">The folder path and file name to export the error file</param>
		Public Sub ExportErrorReport(ByVal filePathAndName As String)
			_controller.ExportErrorReport(filePathAndName)
		End Sub
#End Region

#Region "Private Functions"
		Private Function CreateLoadFile(ByVal clientSettings As Settings) As kCura.WinEDDS.ImportExtension.DataReaderLoadFile
			Dim loadFileTemp As WinEDDS.LoadFile = MapInputToSettingsFactory(clientSettings).ToLoadFile

			Dim tempLoadFile As New WinEDDS.ImportExtension.DataReaderLoadFile
			tempLoadFile.DataReader = SourceData.SourceData

			'These are ALL of the load file settings
			tempLoadFile.ArtifactTypeID = loadFileTemp.ArtifactTypeID
			tempLoadFile.CaseDefaultPath = loadFileTemp.CaseDefaultPath
			tempLoadFile.CaseInfo = loadFileTemp.CaseInfo
			tempLoadFile.CookieContainer = loadFileTemp.CookieContainer
			tempLoadFile.CopyFilesToDocumentRepository = loadFileTemp.CopyFilesToDocumentRepository
			tempLoadFile.CreateFolderStructure = loadFileTemp.CreateFolderStructure
			tempLoadFile.Credentials = loadFileTemp.Credentials
			tempLoadFile.DestinationFolderID = loadFileTemp.DestinationFolderID
			tempLoadFile.ExtractedTextFileEncoding = loadFileTemp.ExtractedTextFileEncoding
			tempLoadFile.ExtractedTextFileEncodingName = loadFileTemp.ExtractedTextFileEncodingName
			tempLoadFile.ExtractFullTextFromNativeFile = loadFileTemp.ExtractFullTextFromNativeFile
			tempLoadFile.FieldMap = loadFileTemp.FieldMap
			tempLoadFile.FilePath = loadFileTemp.FilePath
			tempLoadFile.FirstLineContainsHeaders = loadFileTemp.FirstLineContainsHeaders
			tempLoadFile.FolderStructureContainedInColumn = loadFileTemp.FolderStructureContainedInColumn
			tempLoadFile.FullTextColumnContainsFileLocation = loadFileTemp.FullTextColumnContainsFileLocation
			tempLoadFile.GroupIdentifierColumn = loadFileTemp.GroupIdentifierColumn
			tempLoadFile.HierarchicalValueDelimiter = loadFileTemp.HierarchicalValueDelimiter
			tempLoadFile.IdentityFieldId = loadFileTemp.IdentityFieldId
			tempLoadFile.LoadNativeFiles = loadFileTemp.LoadNativeFiles
			tempLoadFile.MultiRecordDelimiter = loadFileTemp.MultiRecordDelimiter
			tempLoadFile.NativeFilePathColumn = loadFileTemp.NativeFilePathColumn
			tempLoadFile.NewlineDelimiter = loadFileTemp.NewlineDelimiter
			tempLoadFile.OverwriteDestination = loadFileTemp.OverwriteDestination
			tempLoadFile.PreviewCodeCount = loadFileTemp.PreviewCodeCount
			tempLoadFile.QuoteDelimiter = loadFileTemp.QuoteDelimiter
			tempLoadFile.RecordDelimiter = loadFileTemp.RecordDelimiter
			tempLoadFile.SelectedCasePath = loadFileTemp.SelectedCasePath
			'
			'
			'
			Dim tempIDField As WinEDDS.DocumentField = SelectIdentifier(_docIDFieldCollection, Not clientSettings.DisableControlNumberCompatibilityMode, _controlNumberFieldName, clientSettings.SelectedIdentifierFieldName)
			If tempIDField Is Nothing Then
				RaiseEvent OnMessage(New Status(String.Format("Using default identifier field {0}", loadFileTemp.SelectedIdentifierField.FieldName)))
				tempIDField = loadFileTemp.SelectedIdentifierField
			End If
			'
			'
			'
			tempLoadFile.SelectedIdentifierField = tempIDField
			'
			tempLoadFile.SendEmailOnLoadCompletion = clientSettings.SendEmailOnLoadCompletion
			tempLoadFile.SourceFileEncoding = loadFileTemp.SourceFileEncoding
			tempLoadFile.StartLineNumber = loadFileTemp.StartLineNumber

			Return tempLoadFile
		End Function

		''' <summary>
		''' Using the specified field name(s), search for the matching DocumentField in the given collection.
		''' </summary>
		''' <param name="docFieldCollection">An array of DocumentField objects, all of which should be Field Identifiers.</param>
		''' <param name="controlNumCompatEnabled">If True, will first try to find <paramref name="compatFieldName"></paramref>, otherwise
		''' just try to find <paramref name="desiredNonCompatField"></paramref>.</param>
		''' <param name="compatFieldName">The field name to find if <paramref name="controlNumCompatEnabled"></paramref> is True.</param>
		''' <param name="desiredNonCompatField">The field name to find if <paramref name="compatFieldName"></paramref> can't be
		''' found when <paramref name="controlNumCompatEnabled"></paramref> is True, otherwise is the only field name to look for.</param>
		''' <returns>The DocumentField if found, otherwise Nothing.</returns>
		''' <remarks>This function raises OnMessage events that contain further information.</remarks>
		Private Function SelectIdentifier(ByVal docFieldCollection As WinEDDS.DocumentField(), ByVal controlNumCompatEnabled As Boolean, ByVal compatFieldName As String, ByVal desiredNonCompatField As String) As WinEDDS.DocumentField
			Dim tempIDField As WinEDDS.DocumentField = Nothing

			If controlNumCompatEnabled = True Then
				tempIDField = GetIdentifierFieldFromName(docFieldCollection, compatFieldName)
			End If

			If tempIDField Is Nothing Then
				If controlNumCompatEnabled = True Then
					RaiseEvent OnMessage(New Status(String.Format("Unable to find compatibility identifier field {0}", compatFieldName)))
				End If

				If Not desiredNonCompatField Is Nothing Then
					tempIDField = GetIdentifierFieldFromName(docFieldCollection, desiredNonCompatField)

					If Not tempIDField Is Nothing Then
						RaiseEvent OnMessage(New Status(String.Format("Using selected identifier field {0}", desiredNonCompatField)))
					Else
						RaiseEvent OnMessage(New Status(String.Format("Unable to find selected identifier field {0}", desiredNonCompatField)))
					End If
				End If
			Else
				RaiseEvent OnMessage(New Status(String.Format("Using compatibility identifier {0}", compatFieldName)))
			End If

			Return tempIDField
		End Function

		Private Function GetIdentifierFieldFromName(ByVal docIDFieldHayStack As WinEDDS.DocumentField(), ByVal docFieldNeedle As String) As WinEDDS.DocumentField
			Dim returnField As WinEDDS.DocumentField = Nothing

			If Not docFieldNeedle Is Nothing Then
				For Each identifierField As WinEDDS.DocumentField In docIDFieldHayStack
					If identifierField.FieldName.Equals(docFieldNeedle, StringComparison.CurrentCultureIgnoreCase) Then
						returnField = identifierField
						Exit For
					End If
				Next
			End If

			Return returnField
		End Function

		Protected Function IsSettingsValid() As Boolean


			Try
				ValidateRelativitySettings()
				ValidateDelimiterSettings()
				'ValidateOverwriteModeSettings()
				ValidateNativeFileSettings()
				ValidateExtractedTextSettings()
			Catch ex As Exception

				_jobReport.FatalException = ex
				RaiseEvent OnMessage(New Status(ex.Message))

				Return False
			End Try
			Return True
		End Function



		Private Function MapInputToSettingsFactory(ByVal clientSettings As Settings) As WinEDDS.DynamicObjectSettingsFactory
			Dim dosf_settings As kCura.WinEDDS.DynamicObjectSettingsFactory = New kCura.WinEDDS.DynamicObjectSettingsFactory(clientSettings.RelativityUsername, clientSettings.RelativityPassword, clientSettings.CaseArtifactId, clientSettings.ArtifactTypeId)
			_docIDFieldCollection = dosf_settings.DocumentIdentifierFields

			With dosf_settings
				.FirstLineContainsHeaders = False

				Select Case clientSettings.OverwriteMode
					Case OverwriteModeEnum.Append
						.OverwriteDestination = WinEDDS.SettingsFactoryBase.OverwriteType.Append
					Case OverwriteModeEnum.AppendOverlay
						.OverwriteDestination = WinEDDS.SettingsFactoryBase.OverwriteType.AppendOverlay
					Case OverwriteModeEnum.Overlay
						.OverwriteDestination = WinEDDS.SettingsFactoryBase.OverwriteType.Overlay
				End Select

				.MultiRecordDelimiter = CType(clientSettings.MultiValueDelimiter, Char)
				.HierarchicalValueDelimiter = CType(clientSettings.NestedValueDelimiter, Char)

				If Not clientSettings.NativeFilePathSourceFieldName = String.Empty Then
					.NativeFilePathColumn = clientSettings.NativeFilePathSourceFieldName
					.LoadNativeFiles = True
				Else
					.LoadNativeFiles = False
				End If

				If Not clientSettings.ParentObjectIdSourceFieldName = String.Empty AndAlso Not clientSettings.FolderPathSourceFieldName = String.Empty Then
					If Not clientSettings.ParentObjectIdSourceFieldName = clientSettings.FolderPathSourceFieldName Then
						Throw New Exception("Only set one of ParentObjectIdSourceFieldName and FolderPathSourceFieldName")
					End If
				End If

				'TODO: EVIL!!!!!!! These following 2 If-Else-EndIf blocks are identical.
				' -Phil S. 10/04/11
				If Not clientSettings.ParentObjectIdSourceFieldName = String.Empty Then
					.ParentInfoContainedInColumn = clientSettings.ParentObjectIdSourceFieldName
					.CreateFolderStructure = True
				Else
					.CreateFolderStructure = False
				End If

				If Not clientSettings.FolderPathSourceFieldName = String.Empty Then
					.ParentInfoContainedInColumn = clientSettings.FolderPathSourceFieldName
					.CreateFolderStructure = True
				Else
					.CreateFolderStructure = False
				End If

				Select Case clientSettings.NativeFileCopyMode
					Case NativeFileCopyModeEnum.CopyFiles
						.CopyFilesToDocumentRepository = True
					Case NativeFileCopyModeEnum.SetFileLinks, Nothing
						.CopyFilesToDocumentRepository = False
					Case Else
						Throw New Exception("ERROR with  NativeFileCopyMode")
				End Select

				.FullTextColumnContainsFileLocation = clientSettings.ExtractedTextFieldContainsFilePath
				If Not clientSettings.ExtractedTextEncoding Is Nothing Then
					.ExtractedTextFileEncoding = clientSettings.ExtractedTextEncoding
				Else
					.ExtractedTextFileEncoding = System.Text.Encoding.Default
				End If

				If clientSettings.DestinationFolderArtifactID > 0 Then
					'This is automatically set from the caseArtifactID. optionally, it can be changed to point to a different folder
					.DestinationFolderID = clientSettings.DestinationFolderArtifactID
				End If

				If clientSettings.StartRecordNumber > 0 Then
					.StartLineNumber = clientSettings.StartRecordNumber
				End If
			End With

			Return dosf_settings
		End Function
#End Region

#Region "Private Routines"
		Protected Overrides Sub Finalize()
			MyBase.Finalize()
		End Sub

		Private Sub ValidateDelimiterSettings()
			If String.IsNullOrEmpty(Settings.MultiValueDelimiter) Then
				Throw New ImportSettingsException("MultiValueDelimiter", String.Empty)
			End If

			If String.IsNullOrEmpty(Settings.NestedValueDelimiter) Then
				Throw New ImportSettingsException("NestedValueDelimiter", String.Empty)
			End If
		End Sub

		Private Sub ValidateExtractedTextSettings()
			If Settings.ExtractedTextFieldContainsFilePath Then
				If Settings.ExtractedTextEncoding Is Nothing Then
					Throw New ImportSettingsException("ExtractedTextEncoding", String.Empty)
				End If
			End If
		End Sub

		Private Sub ValidateNativeFileSettings()
			If Settings.NativeFileCopyMode = NativeFileCopyModeEnum.DoNotImportNativeFiles Then
				If Not Settings.NativeFilePathSourceFieldName = String.Empty Then
					Throw New ImportSettingsConflictException("NativeFileCopyMode", "NativeFilePathSourceFieldName", "If NativeFileCopyMode is set to DoNotImportNativeFiles, then NativeFilePathSourceFieldName cannot be set.")
				End If
			Else
				If Settings.NativeFilePathSourceFieldName = String.Empty Then
					Throw New ImportSettingsException("NativeFilePathSourceFieldName", "If NativeFileCopyMode is set, then NativeFilePathSourceFieldName must be set. Format: [Field] ([index]). Example: File (3). ")
				Else
					RaiseEvent OnMessage(New Status(String.Format("Importing native files using {0}", Settings.NativeFileCopyMode.ToString)))
				End If
			End If
		End Sub

		'Private Sub ValidateOverwriteModeSettings()
		'	If Settings.OverwriteMode = OverwriteModeEnum.Overlay Then
		'		If Settings.OverlayIdentifierSourceFieldName Is Nothing OrElse Settings.OverlayIdentifierSourceFieldName.Trim = String.Empty Then
		'			Throw New Exception("When Overwrite Mode is set to Overlay, Overlay Identifier Field must be set.")
		'		End If
		'	End If
		'End Sub

		Private Sub ValidateRelativitySettings()
			If Settings.RelativityUsername Is Nothing OrElse Settings.RelativityUsername = String.Empty Then
				Throw New ImportSettingsException("RelativityUserName")
			End If
			If Settings.RelativityPassword Is Nothing OrElse Settings.RelativityPassword = String.Empty Then
				Throw New ImportSettingsException("RelativityPassword")
			End If
			If Settings.CaseArtifactId <= 0 Then
				Throw New ImportSettingsException("CaseArtifactId", "This must be the ID of an existing case.")
			End If
			If Settings.ArtifactTypeId <= 0 Then
				Throw New ImportSettingsException("ArtifactTypeId", "This must be the ID of an existing artifact type.")
			End If
		End Sub
#End Region

#Region "Event Handlers"
		Private Sub _observer_ErrorReportEvent(ByVal row As System.Collections.IDictionary) Handles _observer.ErrorReportEvent
			_hasErrors = True
			RaiseEvent OnError(row)
			Dim msg As String = row.Item("Message").ToString
			Dim lineNumbObj As Object = row.Item("Line Number")	' or is it "Line Number" ????
			Dim lineNum As Long = 0
			If Not lineNumbObj Is Nothing Then
				lineNum = DirectCast(lineNumbObj, Int32)
			End If

			Dim idobj As Object
			idobj = row.Item("Identifier")
			Dim id As String = String.Empty
			If Not idobj Is Nothing Then
				id = idobj.ToString()
			End If
			_jobReport.ErrorRows.Add(New JobReport.RowError(lineNum, msg, id))
		End Sub

		Private Sub _observer_FieldMapped(ByVal sourceField As String, ByVal workspaceField As String) Handles _observer.FieldMapped
			_jobReport.FieldMap.Add(New JobReport.FieldMapEntry(sourceField, workspaceField))
		End Sub

		Private Sub _observer_OnProcessComplete(ByVal closeForm As Boolean, ByVal exportFilePath As String, ByVal exportLogs As Boolean) Handles _observer.OnProcessComplete
			RaiseEvent OnMessage(New Status(String.Format("Completed!")))
			RaiseComplete()
		End Sub

		Private Sub _observer_OnProcessEvent(ByVal evt As kCura.Windows.Process.ProcessEvent) Handles _observer.OnProcessEvent
			If evt.Type = kCura.Windows.Process.ProcessEventTypeEnum.Error OrElse evt.Type = kCura.Windows.Process.ProcessEventTypeEnum.Warning OrElse evt.Type = Windows.Process.ProcessEventTypeEnum.Status Then
				RaiseEvent OnMessage(New Status(String.Format("[Timestamp: {0}] [Record Info: {2}] {3} - {1}", evt.DateTime, evt.Message, evt.RecordInfo, evt.Type)))
			End If
		End Sub

		Private Sub _observer_OnProcessFatalException(ByVal ex As Exception) Handles _observer.OnProcessFatalException
			RaiseEvent OnMessage(New Status(String.Format("FatalException: {0}", ex.ToString)))
			_jobReport.FatalException = ex
			RaiseFatalException()
		End Sub

		Private Sub _observer_OnProcessProgressEvent(ByVal evt As kCura.Windows.Process.ProcessProgressEvent) Handles _observer.OnProcessProgressEvent
			RaiseEvent OnMessage(New Status(String.Format("[Timestamp: {0}] [Progress Info: {1} ]", System.DateTime.Now, evt.TotalRecordsProcessedDisplay)))
		End Sub

		Private Sub _observer_RecordProcessedEvent(ByVal recordNumber As Long) Handles _observer.RecordProcessed
			RaiseEvent OnProgress(recordNumber)
		End Sub
#End Region

#Region "Properties"
		''' <summary>
		''' Gets or sets the field delimiter to use when writing
		''' out the bulk load file. Line delimiters will be this value plus a line feed.
		''' </summary>
		''' <exception cref="ArgumentNullException">Thrown if <paramref name="bulkLoadFileFieldDelimiter"/>
		''' is <c>null</c> or <c>String.Empty</c>.</exception>
		<Obsolete("TODO: Use the Settings class version instead")>
		Public Property BulkLoadFileFieldDelimiter() As String
			Get
				Return _bulkLoadFileFieldDelimiter
			End Get
			Set(ByVal value As String)
				If String.IsNullOrEmpty(value) Then
					Throw New ArgumentNullException("bulkLoadFileFieldDelimiter")
				End If

				_bulkLoadFileFieldDelimiter = value
			End Set
		End Property

		'TODO: Because these were public fields before (vs properties), no exception was thrown
		' if value = Nothing; for compatibility that is the case here
		Public Property Settings() As Settings
			Get
				Return CType(_nativeSettings, Settings)
			End Get
			<Obsolete("Assigning a value to the 'Settings' property is being phased out.  Please use the existing value of the property.")>
			Set(value As Settings)
				_nativeSettings = value
			End Set
		End Property

		Public Property SourceData() As SourceIDataReader
			Get
				Return _nativeDataReader
			End Get
			<Obsolete("Assigning a value to the 'SourceData' property is being phased out.  Please use the existing value of the property.")>
			Set(value As SourceIDataReader)
				_nativeDataReader = value
			End Set
		End Property

		''' <summary>
		''' Enables or disables native validation for the current job
		''' </summary>
		''' <value>True: validation is disabled
		''' False: validation is enabled
		''' Nothing: validation will use the pre-configured value</value>
		Public Property DisableNativeValidation As Boolean?

		''' <summary>
		''' Enables or disables native location validation for the current job
		''' </summary>
		''' <value>True: validation is disabled
		''' False: validation is enabled
		''' Nothing: validation will use the pre-configured value</value>
		Public Property DisableNativeLocationValidation As Boolean?

		''' <summary>
		''' Enables or disables user permission checks per document or object
		''' </summary>
		''' <value>True: security checks are disabled
		''' False: security checks are enabled</value>
		Public Property DisableUserSecurityCheck As Boolean

		''' <summary>
		''' Allows to set AuditLevel used during import
		''' </summary>
		''' <value>		FullAudit - default
		'''           NoSnapshot - audit is recored but not auditDetails
		'''           NoAudit - no audit at all</value>
		Public Property AuditLevel As kCura.EDDS.WebAPI.BulkImportManagerBase.ImportAuditLevel = kCura.EDDS.WebAPI.BulkImportManagerBase.ImportAuditLevel.FullAudit


#End Region

	End Class

End Namespace