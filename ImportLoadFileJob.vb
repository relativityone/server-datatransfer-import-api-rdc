Namespace kCura.Relativity.DataReaderClient
	Public Class ImportBulkArtifactJob
		Inherits LoadFileJobBase

#Region "Private Variables"
		Private _bulkLoadFileFieldDelimiter As String
		Private _docIDFieldCollection As WinEDDS.DocumentField()
#End Region

#Region "Constructors"
		''' <summary>
		''' Creates a new job that can bulk insert a large amount of artifacts.
		''' </summary>
		Public Sub New()
			_nativeSettings = New Settings
			_nativeDataReader = New SourceIDataReader

			BulkLoadFileFieldDelimiter = Global.Relativity.Constants.DEFAULT_FIELD_DELIMITER
		End Sub
#End Region

#Region "Events"
		Public Shadows Event OnMessage(ByVal status As Status)
		Public Shadows Event OnError(ByVal row As IDictionary)
#End Region

#Region "Public Routines"
		Public Overrides Sub Execute()
			If IsSettingsValid() Then
				RaiseEvent OnMessage(New Status("Getting source data from database"))

				If Not String.IsNullOrWhiteSpace(Settings.ServiceURL) Then
					RaiseEvent OnMessage(New Status(String.Format("Using supplied ServiceURL {0}", Settings.ServiceURL)))
				ElseIf Not String.IsNullOrWhiteSpace(WinEDDS.Config.AppConfigWebServiceURL) Then
					RaiseEvent OnMessage(New Status(String.Format("Using application configuration ServiceURL {0}", WinEDDS.Config.AppConfigWebServiceURL)))
					Settings.ServiceURL = WinEDDS.Config.AppConfigWebServiceURL
				Else
					RaiseEvent OnMessage(New Status(String.Format("Using supplied ServiceURL {0}", WinEDDS.Config.WebServiceURL)))
					Settings.ServiceURL = WinEDDS.Config.WebServiceURL
				End If

				Dim process As WinEDDS.ImportExtension.DataReaderImporterProcess = New WinEDDS.ImportExtension.DataReaderImporterProcess(SourceData.SourceData, Settings.ServiceURL)

				_observer = process.ProcessObserver
				_controller = process.ProcessController
				RaiseEvent OnMessage(New Status("Updating settings"))
				process.LoadFile = Me.CreateLoadFile(Settings)
				process.BulkLoadFileFieldDelimiter = BulkLoadFileFieldDelimiter

				RaiseEvent OnMessage(New Status("Executing"))
				Try
					process.StartProcess()
				Catch ex As Exception
					RaiseEvent OnMessage(New Status(String.Format("Exception: {0}", ex.ToString)))
				End Try
			Else
				RaiseEvent OnMessage(New Status("There was an error in your settings.  Import aborted."))
			End If
		End Sub

		''' <summary>
		''' Exports the error log file from the import if any errors occurred.
		''' If no errors occurred, no file is copied.
		''' </summary>
		''' <param name="location">The location to export the error file to</param>
		Public Sub ExportErrorReport(ByVal location As String)
			_controller.ExportErrorReport(location)
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
			'TODO: RaiseEvent if we fall back to default??
			'
			tempLoadFile.SelectedIdentifierField = loadFileTemp.SelectedIdentifierField
			If Not String.IsNullOrWhiteSpace(clientSettings.SelectedIdentifierFieldName) Then
				Dim tempIDField As WinEDDS.DocumentField = GetIdentifierFieldFromName(_docIDFieldCollection, clientSettings.SelectedIdentifierFieldName)

				If Not tempIDField Is Nothing Then
					tempLoadFile.SelectedIdentifierField = tempIDField
					RaiseEvent OnMessage(New Status(String.Format("Selected identifier {0} found", clientSettings.SelectedIdentifierFieldName)))
				End If
			End If
			'
			tempLoadFile.SendEmailOnLoadCompletion = clientSettings.SendEmailOnLoadCompletion
			tempLoadFile.SourceFileEncoding = loadFileTemp.SourceFileEncoding
			tempLoadFile.StartLineNumber = loadFileTemp.StartLineNumber

			Return tempLoadFile
		End Function

		Private Function GetIdentifierFieldFromName(ByVal docIDFieldHayStack As WinEDDS.DocumentField(), ByVal docFieldNeedle As String) As WinEDDS.DocumentField
			Dim returnField As WinEDDS.DocumentField = Nothing

			For Each identifierField As WinEDDS.DocumentField In docIDFieldHayStack
				If identifierField.FieldName.Equals(docFieldNeedle, StringComparison.CurrentCultureIgnoreCase) Then
					returnField = identifierField
					Exit For
				End If
			Next

			Return returnField
		End Function

		Protected Overrides Function IsSettingsValid() As Boolean
			Try
				ValidateRelativitySettings()
				ValidateDelimiterSettings()
				ValidateOverwriteModeSettings()
				ValidateNativeFileSettings()
				ValidateExtractedTextSettings()
			Catch ex As Exception
				RaiseEvent OnMessage(New Status(ex.Message))
				Return False
			End Try
			Return True
		End Function

		Private Function MapInputToSettingsFactory(ByVal clientSettings As Settings) As WinEDDS.DynamicObjectSettingsFactory
			Dim dosf_settings As kCura.WinEDDS.DynamicObjectSettingsFactory = New kCura.WinEDDS.DynamicObjectSettingsFactory(clientSettings.RelativityUsername, clientSettings.RelativityPassword, clientSettings.CaseArtifactId, clientSettings.ArtifactTypeId, clientSettings.ServiceURL)
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
				Throw New Exception("MultiValueDelimiter not set")
			End If

			If String.IsNullOrEmpty(Settings.NestedValueDelimiter) Then
				Throw New Exception("NestedValueDelimiter not set")
			End If
		End Sub

		Private Sub ValidateExtractedTextSettings()
			If Settings.ExtractedTextFieldContainsFilePath Then
				If Settings.ExtractedTextEncoding Is Nothing Then
					Throw New Exception("ExtractedTextEncoding not set")
				End If
			End If
		End Sub

		Private Sub ValidateNativeFileSettings()
			If Settings.NativeFileCopyMode = NativeFileCopyModeEnum.DoNotImportNativeFiles Then
				If Not Settings.NativeFilePathSourceFieldName = String.Empty Then
					Throw New Exception("If NativeFileCopyMode is set to DoNotImportNativeFiles, then NativeFilePathSourceFieldName cannot be set.")
				End If
			Else
				If Settings.NativeFilePathSourceFieldName = String.Empty Then
					Throw New Exception("If NativeFileCopyMode is set, then NativeFilePathSourceFieldName must be set. Format: [Field] ([index]). Example: File (3). ")
				Else
					RaiseEvent OnMessage(New Status(String.Format("Importing native files using {0}", Settings.NativeFileCopyMode.ToString)))
				End If
			End If
		End Sub

		Private Sub ValidateOverwriteModeSettings()
			If Settings.OverwriteMode = OverwriteModeEnum.Overlay Then
				If Settings.OverlayIdentifierSourceFieldName Is Nothing OrElse Settings.OverlayIdentifierSourceFieldName.Trim = String.Empty Then
					Throw New Exception("When Overwrite Mode is set to Overlay, Overlay Identifier Field must be set.")
				End If
			End If
		End Sub

		Private Sub ValidateRelativitySettings()
			If Settings.RelativityUsername Is Nothing OrElse Settings.RelativityUsername = String.Empty Then
				Throw New Exception(String.Format("{0} must be set", "RelativityUsername"))
			End If
			If Settings.RelativityPassword Is Nothing OrElse Settings.RelativityPassword = String.Empty Then
				Throw New Exception(String.Format("{0} must be set", "RelativityPassword"))
			End If
			If Settings.CaseArtifactId = 0 Then
				Throw New Exception(String.Format("{0} must be set and cannot be 0", "CaseArtifactId"))
			End If
			If Settings.ArtifactTypeId = 0 Then
				Throw New Exception(String.Format("{0} must be set and cannot be 0", "ArtifactTypeId"))
			End If
		End Sub
#End Region

#Region "Event Handlers"
		Private Sub _observer_ErrorReportEvent(ByVal row As System.Collections.IDictionary) Handles _observer.ErrorReportEvent
			RaiseEvent OnError(row)
		End Sub

		Private Sub _observer_OnProcessComplete(ByVal closeForm As Boolean, ByVal exportFilePath As String, ByVal exportLogs As Boolean) Handles _observer.OnProcessComplete
			RaiseEvent OnMessage(New Status(String.Format("Completed!")))
		End Sub

		Private Sub _observer_OnProcessEvent(ByVal evt As kCura.Windows.Process.ProcessEvent) Handles _observer.OnProcessEvent
			If evt.Type = kCura.Windows.Process.ProcessEventTypeEnum.Error OrElse evt.Type = kCura.Windows.Process.ProcessEventTypeEnum.Warning OrElse evt.Type = Windows.Process.ProcessEventTypeEnum.Status Then
				RaiseEvent OnMessage(New Status(String.Format("[Timestamp: {0}] [Record Info: {2}] {3} - {1}", evt.DateTime, evt.Message, evt.RecordInfo, evt.Type)))
			End If
		End Sub

		Private Sub _observer_OnProcessFatalException(ByVal ex As System.Exception) Handles _observer.OnProcessFatalException
			RaiseEvent OnMessage(New Status(String.Format("FatalException: {0}", ex.ToString)))
		End Sub

		Private Sub _observer_OnProcessProgressEvent(ByVal evt As kCura.Windows.Process.ProcessProgressEvent) Handles _observer.OnProcessProgressEvent
			RaiseEvent OnMessage(New Status(String.Format("[Timestamp: {0}] [Progress Info: {1} of {2}]", System.DateTime.Now, evt.TotalRecordsProcessedDisplay, Settings.RowCount)))
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
			Set(value As Settings)
				_nativeSettings = value
			End Set
		End Property

		Public Property SourceData() As SourceIDataReader
			Get
				Return _nativeDataReader
			End Get
			Set(value As SourceIDataReader)
				_nativeDataReader = value
			End Set
		End Property
#End Region

	End Class
End Namespace