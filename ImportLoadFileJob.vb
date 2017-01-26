Imports System.Net
Imports Relativity

Namespace kCura.Relativity.DataReaderClient

	''' <summary>
	''' Provides the functionality for importing Artifacts into a workspace, setting import parameters, loading data, and retrieving messages from the OnMessage event.
	''' </summary>
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

		Private _credentials As ICredentials
		Private _cookieMonster As Net.CookieContainer

		Private _executionSource As ExecutionSource

		Private Const _DOCUMENT_ARTIFACT_TYPE_ID As Int32 = 10 'TODO: make a reference to Relativity so we don't have to do this

#End Region

#Region "Constructors"
		''' <summary>
		''' Creates a new job to import Artifacts in bulk.
		''' </summary>
		Public Sub New()
			_controlNumberFieldName = "control number"
			_nativeSettings = New Settings
			_nativeDataReader = New SourceIDataReader

			_bulkLoadFileFieldDelimiter = Global.Relativity.Constants.DEFAULT_FIELD_DELIMITER
			_hasErrors = False
		End Sub

		'Friend Sub New(ByVal relativityUserName As String, ByVal password As String)
		'	Me.New()
		'	Settings.RelativityUsername = relativityUserName
		'	Settings.RelativityPassword = password
		'End Sub

		Friend Sub New(ByVal credentials As ICredentials, ByVal cookieMonster As Net.CookieContainer, ByVal relativityUserName As String, ByVal password As String, ByVal Optional executionSource As Integer = 0)
			Me.New()
			_executionSource = CType(executionSource, ExecutionSource)
			_credentials = credentials
			_cookieMonster = cookieMonster
			Settings.RelativityUsername = relativityUserName
			Settings.RelativityPassword = password

		End Sub

#End Region

#Region "Events"
		''' <summary>
		''' Occurs when a call is made to the Execute method. This event contains a status message.
		''' </summary>
		''' <param name="status">The status message.</param>
		Public Event OnMessage(ByVal status As Status)
		''' <summary>
		''' Occurs when an error is found.
		''' </summary>
		''' <param name="row">The IDictionary containing the error.</param>
		Public Event OnError(ByVal row As IDictionary)
		''' <summary>
		''' Occurs when all the data for an import job has been processed.  Raised at the end of an import.
		''' </summary>
		''' <param name="jobReport">The JobReport describing the completed import job.</param><remarks>
		''' Does not guarantee successful or error-free completion.
		''' </remarks>
		Public Event OnComplete(ByVal jobReport As JobReport) Implements IImportNotifier.OnComplete
		''' <summary>
		''' Occurs when an import job suffers a fatal exception and aborts.  Raised at the end of an import.
		''' </summary>
		''' <param name="jobReport">The JobReport describing the failed import job.</param>
		Public Event OnFatalException(ByVal jobReport As JobReport) Implements IImportNotifier.OnFatalException
		''' <summary>
		''' Occurs when a record has been processed.
		''' </summary>
		''' <param name="completedRow">The processed record.</param>
		Public Event OnProgress(ByVal completedRow As Long) Implements IImportNotifier.OnProgress


#End Region

#Region "Public Routines"

		''' <summary>
		''' Executes the DataReaderClient, which operates as an iterator over a data source.
		''' </summary>
		Public Sub Execute()
			_jobReport = New JobReport()
			_jobReport.StartTime = DateTime.Now()

			' authenticate here
			If _credentials Is Nothing Then
				ImportCredentialManager.WebServiceURL = Settings.WebServiceURL
				Dim creds As ImportCredentialManager.SessionCredentials = ImportCredentialManager.GetCredentials(Settings.RelativityUsername, Settings.RelativityPassword)
				_credentials = creds.Credentials
				_cookieMonster = creds.CookieMonster
			End If

			If IsSettingsValid() Then

				RaiseEvent OnMessage(New Status("Getting source data from database"))

				Dim process As WinEDDS.ImportExtension.DataReaderImporterProcess = New WinEDDS.ImportExtension.DataReaderImporterProcess(SourceData.SourceData) With {.OnBehalfOfUserToken = Settings.OnBehalfOfUserToken}
				process.ExecutionSource = _executionSource
				_observer = process.ProcessObserver
				_controller = process.ProcessController

				If Settings.DisableNativeValidation.HasValue Then process.DisableNativeValidation = Settings.DisableNativeValidation.Value
				If Settings.DisableNativeLocationValidation.HasValue Then process.DisableNativeLocationValidation = Settings.DisableNativeLocationValidation.Value
				process.MaximumErrorCount = Settings.MaximumErrorCount
				process.DisableUserSecurityCheck = Settings.DisableUserSecurityCheck
				process.AuditLevel = Settings.AuditLevel
				process.SkipExtractedTextEncodingCheck = Settings.DisableExtractedTextEncodingCheck
				process.LoadImportedFullTextFromServer = Settings.LoadImportedFullTextFromServer
				process.DisableExtractedTextFileLocationValidation = Settings.DisableExtractedTextFileLocationValidation
				process.OIFileIdColumnName = Settings.OIFileIdColumnName
				If (Not String.IsNullOrEmpty(Settings.BulkLoadFileFieldDelimiter)) Then
					process.BulkLoadFileFieldDelimiter = Settings.BulkLoadFileFieldDelimiter
				Else
					process.BulkLoadFileFieldDelimiter = _bulkLoadFileFieldDelimiter
				End If
				process.OIFileIdMapped = Settings.OIFileIdMapped
				process.OIFileTypeColumnName = Settings.OIFileTypeColumnName
				process.FileSizeMapped = Settings.FileSizeMapped
				process.FileSizeColumn = Settings.FileSizeColumn
				process.FileNameColumn = Settings.FileNameColumn
				RaiseEvent OnMessage(New Status("Updating settings"))
				process.LoadFile = CreateLoadFile(Settings)

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

		'The 'OnComplete' and 'OnFatalException' events are alternatives to OnMessage, OnError, and
		' the OnProgress event. The latter 3 are difficult to parse, so the JobReport class was
		' created to centralize all of the import information. If 'real-time' progress is desired,
		' then OnComplete & OnFatalException are *not* the events to listen to--these are only raised
		' at the end of an import. Furthermore, OnComplete does not indicate 100% success, but that
		' all of the data was processed. OnFatalException is raised when the import 'bombed out'.
		' -Phil S. 07/10/2012

		Private Sub RaiseFatalException()
			_jobReport.EndTime = DateTime.Now
			RaiseEvent OnFatalException(_jobReport)
		End Sub

		Private Sub RaiseComplete()
			_jobReport.EndTime = DateTime.Now
			RaiseEvent OnComplete(_jobReport)
		End Sub


		''' <summary>
		''' Exports the error log file for an import job. This file is written only when errors occur.
		''' </summary>
		''' <param name="filePathAndName">The folder path and file name to export the error file</param>
		Public Sub ExportErrorReport(ByVal filePathAndName As String)
			_controller.ExportErrorReport(filePathAndName)
		End Sub

		''' <summary>
		''' Exports the error file for an import job. This file is written only when errors occur.
		''' </summary>
		''' <param name="filePathAndName">The folder path and file name to export the error file</param>
		Public Sub ExportErrorFile(ByVal filePathAndName As String)
			_controller.ExportErrorFile(filePathAndName)
		End Sub

		''' <summary>
		''' Unknown; development testing
		''' </summary>
		''' <param name="exportLocation">A directory to place error files in</param>
		Public Sub ExportServerErrors(ByVal exportLocation As String)
			_controller.ExportServerErrors(exportLocation)
		End Sub
#End Region

#Region "Private Functions"
		Private Function CreateLoadFile(ByVal clientSettings As Settings) As kCura.WinEDDS.ImportExtension.DataReaderLoadFile
			Dim loadFileTemp As WinEDDS.LoadFile = MapInputToSettingsFactory(clientSettings).ToLoadFile

			' If IdentityFieldId is greater than zero, it has been explicitly set, 
			' otherwise allow LoadFile its defaulting as it already does
			If AllowNonIdentifierKeyField(clientSettings) Then
				loadFileTemp.IdentityFieldId = clientSettings.IdentityFieldId
			End If

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
			tempLoadFile.FieldMap = loadFileTemp.FieldMap
			tempLoadFile.FilePath = loadFileTemp.FilePath
			tempLoadFile.FirstLineContainsHeaders = loadFileTemp.FirstLineContainsHeaders
			tempLoadFile.FolderStructureContainedInColumn = loadFileTemp.FolderStructureContainedInColumn
            tempLoadFile.FullTextColumnContainsFileLocation = loadFileTemp.FullTextColumnContainsFileLocation
            tempLoadFile.LongTextColumnThatContainsPathToFullText = loadFileTemp.LongTextColumnThatContainsPathToFullText
            tempLoadFile.GroupIdentifierColumn = loadFileTemp.GroupIdentifierColumn
			tempLoadFile.DataGridIDColumn = loadFileTemp.DataGridIDColumn
			tempLoadFile.HierarchicalValueDelimiter = loadFileTemp.HierarchicalValueDelimiter
			tempLoadFile.IdentityFieldId = loadFileTemp.IdentityFieldId
			tempLoadFile.LoadNativeFiles = loadFileTemp.LoadNativeFiles
			tempLoadFile.MultiRecordDelimiter = loadFileTemp.MultiRecordDelimiter
			tempLoadFile.NativeFilePathColumn = loadFileTemp.NativeFilePathColumn
			tempLoadFile.NewlineDelimiter = loadFileTemp.NewlineDelimiter
			tempLoadFile.OverwriteDestination = loadFileTemp.OverwriteDestination
			tempLoadFile.MoveDocumentsInAppendOverlayMode = loadFileTemp.MoveDocumentsInAppendOverlayMode
			tempLoadFile.PreviewCodeCount = loadFileTemp.PreviewCodeCount
			tempLoadFile.QuoteDelimiter = loadFileTemp.QuoteDelimiter
			tempLoadFile.RecordDelimiter = loadFileTemp.RecordDelimiter
			tempLoadFile.SelectedCasePath = loadFileTemp.SelectedCasePath
			tempLoadFile.OIFileIdColumnName = loadFileTemp.OIFileIdColumnName
			tempLoadFile.OIFileIdMapped = loadFileTemp.OIFileIdMapped
			tempLoadFile.OIFileTypeColumnName = loadFileTemp.OIFileTypeColumnName
			tempLoadFile.FileSizeColumn = loadFileTemp.FileSizeColumn
			tempLoadFile.FileSizeMapped = loadFileTemp.FileSizeMapped
			tempLoadFile.FileNameColumn = loadFileTemp.FileNameColumn
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
			tempLoadFile.ObjectFieldIdListContainsArtifactId = loadFileTemp.ObjectFieldIdListContainsArtifactId
			tempLoadFile.OverlayBehavior = loadFileTemp.OverlayBehavior

			Return tempLoadFile
		End Function

		Private Function AllowNonIdentifierKeyField(clientSettings As Settings) As Boolean
			Dim retval As Boolean = (clientSettings.IdentityFieldId > 0)
			If clientSettings.ArtifactTypeId = _DOCUMENT_ARTIFACT_TYPE_ID Then
				retval = retval AndAlso (clientSettings.OverwriteMode = OverwriteModeEnum.Overlay)
			End If
			Return retval
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

		''' <summary>
		''' Validates Relativity, delimiter, native file, and extracted text settings.
		''' </summary>
		''' <returns></returns>
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
			Dim dosf_settings As kCura.WinEDDS.DynamicObjectSettingsFactory = Nothing
			'If clientSettings.Credential Is Nothing Then
			'dosf_settings = New kCura.WinEDDS.DynamicObjectSettingsFactory(clientSettings.RelativityUsername, clientSettings.RelativityPassword, clientSettings.CaseArtifactId, clientSettings.ArtifactTypeId)
			'Else
			dosf_settings = New kCura.WinEDDS.DynamicObjectSettingsFactory(_credentials, _cookieMonster, clientSettings.CaseArtifactId, clientSettings.ArtifactTypeId)
			'End If
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

				.OverlayBehavior = clientSettings.OverlayBehavior

				.MoveDocumentsInAppendOverlayMode = clientSettings.MoveDocumentsInAppendOverlayMode

				.MultiRecordDelimiter = CType(clientSettings.MultiValueDelimiter, Char)
				.HierarchicalValueDelimiter = CType(clientSettings.NestedValueDelimiter, Char)

				If Not clientSettings.NativeFilePathSourceFieldName = String.Empty Then
					.NativeFilePathColumn = clientSettings.NativeFilePathSourceFieldName
					.LoadNativeFiles = True
				Else
					.LoadNativeFiles = False
				End If

				If Not String.IsNullOrWhiteSpace(clientSettings.ParentObjectIdSourceFieldName) AndAlso Not String.IsNullOrWhiteSpace(clientSettings.FolderPathSourceFieldName) _
				AndAlso Not clientSettings.ParentObjectIdSourceFieldName = clientSettings.FolderPathSourceFieldName Then
					Throw New Exception("Only set one of ParentObjectIdSourceFieldName and FolderPathSourceFieldName")
				End If

				If Not String.IsNullOrWhiteSpace(clientSettings.ParentObjectIdSourceFieldName) Then
					.ParentInfoContainedInColumn = clientSettings.ParentObjectIdSourceFieldName
					.CreateFolderStructure = True
				ElseIf Not String.IsNullOrWhiteSpace(clientSettings.FolderPathSourceFieldName) Then
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
						Throw New Exception("ERROR with NativeFileCopyMode")
				End Select

                .LongTextColumnThatContainsPathToFullText = clientSettings.LongTextColumnThatContainsPathToFullText
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
				.ObjectFieldIdListContainsArtifactId = clientSettings.ObjectFieldIdListContainsArtifactId

				.DataGridIDColumn = clientSettings.DataGridIDColumnName
			End With

			Return dosf_settings
		End Function
#End Region

#Region "Private Routines"
		''' <summary>
		''' Cleans up and frees resources.
		''' </summary>
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
				If Settings.DisableExtractedTextEncodingCheck.HasValue AndAlso Settings.DisableExtractedTextEncodingCheck AndAlso Settings.ExtractedTextEncoding Is Nothing Then
					Throw New ImportSettingsConflictException("DisableExtractedTextEncodingCheck", "ExtractedTextEncoding", "ExtractedTextEncoding must be set if DisableExtractedTextEncodingCheck is enabled.")
				End If
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
					Throw New ImportSettingsException("NativeFilePathSourceFieldName", "If NativeFileCopyMode is set, then NativeFilePathSourceFieldName must be set.")
				Else
					RaiseEvent OnMessage(New Status(String.Format("Importing native files using {0}", Settings.NativeFileCopyMode.ToString)))
				End If
			End If
		End Sub


		Private Sub ValidateRelativitySettings()
			If Settings.CaseArtifactId <= 0 Then
				Throw New ImportSettingsException("CaseArtifactId", "This must be the ID of an existing case.")
			End If
			If Settings.ArtifactTypeId <= 0 Then
				Throw New ImportSettingsException("ArtifactTypeId", "This must be the ID of an existing artifact type.")
			End If
			If Settings.MaximumErrorCount.HasValue AndAlso (Settings.MaximumErrorCount.Value < 1 OrElse Settings.MaximumErrorCount.Value = Int32.MaxValue) Then
				Throw New ImportSettingsException("MaximumErrorCount", "This must be greater than 0 and less than Int32.MaxValue.")
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
			If Not ex.InnerException Is Nothing Then
				RaiseFatalInnerException(ex)
			End If
			_jobReport.FatalException = ex
			RaiseFatalException()
		End Sub
		Private Sub RaiseFatalInnerException(ByVal ex As Exception)
			RaiseEvent OnMessage(New Status(String.Format("Inner Exception: {0}", ex.ToString())))
			If Not ex.InnerException Is Nothing Then
				RaiseFatalInnerException(ex.InnerException)
			End If
		End Sub

		Private Sub _observer_OnProcessProgressEvent(ByVal evt As kCura.Windows.Process.ProcessProgressEvent) Handles _observer.OnProcessProgressEvent
			RaiseEvent OnMessage(New Status(String.Format("[Timestamp: {0}] [Progress Info: {1} ]", System.DateTime.Now, evt.TotalRecordsProcessedDisplay)))
		End Sub

		Private Sub _observer_RecordProcessedEvent(ByVal recordNumber As Long) Handles _observer.RecordProcessed
			RaiseEvent OnProgress(recordNumber)
		End Sub

		Private Sub _observer_IncrementRecordCount() Handles _observer.IncrementRecordCount
			_jobReport.TotalRows += 1
		End Sub
#End Region

#Region "Properties"
		''' <summary>
		''' Gets or sets the field delimiter to use when writing
		''' out the bulk load file. Line delimiters will be this value plus a line feed.
		''' </summary>
		''' <exception cref="ArgumentNullException">Thrown if value
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

		'TODO: Because these were public fields before (vs properties), no exception was thrown if value = Nothing;
		' for compatibility, that is still the case here
		''' <summary>
		''' Gets or sets the current settings for the import job.
		''' </summary>
		Public Property Settings() As Settings
			Get
				Return CType(_nativeSettings, Settings)
			End Get
			Friend Set(value As Settings)
				_nativeSettings = value
			End Set
		End Property

		''' <summary>
		''' Represents an instance of the SourceIDataReader, which contains data for import. This property is required.
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks>For standard imports, the SourceIDataReader requires a generic IDataReader object and operates as an iterator over a DataTable instance that contains the data source.</remarks>
		Public Property SourceData() As SourceIDataReader
			Get
				Return _nativeDataReader
			End Get
			Friend Set(value As SourceIDataReader)
				_nativeDataReader = value
			End Set
		End Property
#End Region

	End Class

End Namespace