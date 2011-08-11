Namespace kCura.Relativity.DataReaderClient
	Public Class ImportBulkArtifactJob

#Region " Public Events and Variables "

		Public Event OnMessage(ByVal status As Status)
		Public Event OnError(ByVal row As IDictionary)

		Public Settings As Settings
		Public SourceData As SourceIDataReader

		Public _bulkLoadFileFieldDelimiter As String

#End Region

#Region " Private variables "
		Private WithEvents _observer As kCura.Windows.Process.ProcessObserver
		Private _controller As kCura.Windows.Process.Controller
#End Region

#Region " Public Properties "
		''' <summary>
		''' Gets or sets the field delimiter to use when writing
		''' out the bulk load file. Line delimiters will be this value plus a line feed.
		''' </summary>
		''' <exception cref="ArgumentNullException">Thrown if <paramref name="bulkLoadFileFieldDelimiter"/>
		''' is <c>null</c> or <c>String.Empty</c>.</exception>
		Public Property BulkLoadFileFieldDelimiter As String
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
#End Region

#Region " Public Methods "

		''' <summary>
		''' Creates a new job that can bulk insert a large amount of artifacts.
		''' </summary>
		Public Sub New()
			Me.Settings = New Settings
			Me.SourceData = New SourceIDataReader

			Me.BulkLoadFileFieldDelimiter = Global.Relativity.Constants.DEFAULT_FIELD_DELIMITER
		End Sub

		Public Sub Execute()
			If IsSettingsValid() Then
				RaiseEvent OnMessage(New Status("Getting source data from database"))

				If String.IsNullOrWhiteSpace(Settings.ServiceURL) Then
					Settings.ServiceURL = kCura.WinEDDS.Config.WebServiceURL
					RaiseEvent OnMessage(New Status("Setting default ServiceURL, none was found"))
				End If

				Dim process As WinEDDS.ImportExtension.DataReaderImporterProcess = New WinEDDS.ImportExtension.DataReaderImporterProcess(SourceData.SourceData, Settings.ServiceURL)
				RaiseEvent OnMessage(New Status(String.Format("Using value {0} for ServiceURL", Settings.ServiceURL)))

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

#Region " Private Methods "

		Private Function CreateLoadFile(ByVal sqlClientSettings As Settings) As kCura.WinEDDS.ImportExtension.DataReaderLoadFile
			'A SQLLoadFile contains everything that a regular load file contains plus sql connection string properties and a query.
			Dim loadFileTemp As kCura.WinEDDS.LoadFile = MapToDynamicObjectSettingsFactory(Settings).ToLoadFile

			Dim tempLoadFile As New kCura.WinEDDS.ImportExtension.DataReaderLoadFile
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
			tempLoadFile.SelectedIdentifierField = loadFileTemp.SelectedIdentifierField
			tempLoadFile.SendEmailOnLoadCompletion = loadFileTemp.SendEmailOnLoadCompletion
			tempLoadFile.SourceFileEncoding = loadFileTemp.SourceFileEncoding
			tempLoadFile.StartLineNumber = loadFileTemp.StartLineNumber

			Return tempLoadFile

		End Function

		Private Function MapToDynamicObjectSettingsFactory(ByVal sqlClientSettings As Settings) As kCura.WinEDDS.DynamicObjectSettingsFactory
			Dim dosf_settings As kCura.WinEDDS.DynamicObjectSettingsFactory = New kCura.WinEDDS.DynamicObjectSettingsFactory(sqlClientSettings.RelativityUsername, sqlClientSettings.RelativityPassword, sqlClientSettings.CaseArtifactId, sqlClientSettings.ArtifactTypeId, sqlClientSettings.ServiceURL)

			With dosf_settings
				.FirstLineContainsHeaders = False

				Select Case sqlClientSettings.OverwriteMode
					Case OverwriteModeEnum.Append
						.OverwriteDestination = WinEDDS.SettingsFactoryBase.OverwriteType.Append
					Case OverwriteModeEnum.AppendOverlay
						.OverwriteDestination = WinEDDS.SettingsFactoryBase.OverwriteType.AppendOverlay
					Case OverwriteModeEnum.Overlay
						.OverwriteDestination = WinEDDS.SettingsFactoryBase.OverwriteType.Overlay
				End Select

				.MultiRecordDelimiter = CType(sqlClientSettings.MultiValueDelimiter, Char)
				.HierarchicalValueDelimiter = CType(sqlClientSettings.NestedValueDelimiter, Char)

				If Not sqlClientSettings.NativeFilePathSourceFieldName = String.Empty Then
					.NativeFilePathColumn = sqlClientSettings.NativeFilePathSourceFieldName
					.LoadNativeFiles = True
				Else
					.LoadNativeFiles = False
				End If

				If Not sqlClientSettings.ParentObjectIdSourceFieldName = String.Empty AndAlso Not sqlClientSettings.FolderPathSourceFieldName = String.Empty Then
					If Not sqlClientSettings.ParentObjectIdSourceFieldName = sqlClientSettings.FolderPathSourceFieldName Then
						Throw New Exception("Only set one of ParentObjectIdSourceFieldName and FolderPathSourceFieldName")
					End If
				End If

				If Not sqlClientSettings.ParentObjectIdSourceFieldName = String.Empty Then
					.ParentInfoContainedInColumn = sqlClientSettings.ParentObjectIdSourceFieldName
					.CreateFolderStructure = True
				Else
					.CreateFolderStructure = False
				End If

				If Not sqlClientSettings.FolderPathSourceFieldName = String.Empty Then
					.ParentInfoContainedInColumn = sqlClientSettings.FolderPathSourceFieldName
					.CreateFolderStructure = True
				Else
					.CreateFolderStructure = False
				End If

				Select Case sqlClientSettings.NativeFileCopyMode
					Case NativeFileCopyModeEnum.CopyFiles
						.CopyFilesToDocumentRepository = True
					Case NativeFileCopyModeEnum.SetFileLinks, Nothing
						.CopyFilesToDocumentRepository = False
					Case Else
						Throw New Exception("ERROR with  NativeFileCopyMode")
				End Select

				.FullTextColumnContainsFileLocation = sqlClientSettings.ExtractedTextFieldContainsFilePath
				If Not sqlClientSettings.ExtractedTextEncoding Is Nothing Then
					.ExtractedTextFileEncoding = sqlClientSettings.ExtractedTextEncoding
				Else
					.ExtractedTextFileEncoding = System.Text.Encoding.Default
				End If

				If sqlClientSettings.DestinationFolderArtifactID > 0 Then
					'This is automatically set from the caseArtifactID. optionally, it can be changed to point to a different folder
					.DestinationFolderID = sqlClientSettings.DestinationFolderArtifactID
				End If


				'				sqlClientSettings.OverlayIdentifierSourceFieldName()

			End With
			Return dosf_settings
		End Function


#End Region

#Region " Validation "

		Private Function IsSettingsValid() As Boolean
			Try
				'	ValidateSqlCommandSettings()
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

		'Private Sub ValidateSqlCommandSettings()
		'	RaiseEvent OnMessage(New Status("Validating SqlCommand"))

		'	If SQLCommand.ServerName Is Nothing OrElse SQLCommand.ServerName = String.Empty Then
		'		Throw New Exception("SqlCommand.ServerName is not set")
		'	End If

		'	If SQLCommand.DatabaseName Is Nothing OrElse SQLCommand.DatabaseName = String.Empty Then
		'		Throw New Exception("SqlCommand.DatabaseName is not set")
		'	End If

		'	If SQLCommand.LoginName Is Nothing OrElse SQLCommand.LoginName = String.Empty Then
		'		Throw New Exception("SqlCommand.LoginName is not set")
		'	End If

		'	If SQLCommand.LoginPassword Is Nothing OrElse SQLCommand.LoginPassword = String.Empty Then
		'		Throw New Exception("SqlCommand.LoginPassword is not set")
		'	End If

		'	If SQLCommand.Query Is Nothing OrElse SQLCommand.Query = String.Empty Then
		'		Throw New Exception("SqlCommand.Query is not set")
		'	End If

		'	Try
		'		'TODO: Find a better way to verify the SQL connection and that the query runs
		'		Dim process As New kCura.WinEDDS.ImportExtension.QueryImporterProcess(SQLCommand.ServerName, SQLCommand.DatabaseName, SQLCommand.LoginName, SQLCommand.LoginPassword, SQLCommand.Query)
		'	Catch ex As Exception
		'		RaiseEvent OnMessage(New Status(String.Format("Error in SQL Command. There is a problem connecting to the database or executing the query ")))
		'		Throw
		'	End Try

		'	RaiseEvent OnMessage(New Status("SQLCommand is valid"))

		'End Sub

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


		Private Sub ValidateDelimiterSettings()
			If Settings.MultiValueDelimiter = String.Empty Then
				Throw New Exception("MultiValueDelimiter not set")
			End If

			If Settings.NestedValueDelimiter = String.Empty Then
				Throw New Exception("NestedValueDelimiter not set")
			End If
		End Sub

		Private Sub ValidateOverwriteModeSettings()
			If Settings.OverwriteMode = OverwriteModeEnum.Overlay Then
				If Settings.OverlayIdentifierSourceFieldName Is Nothing OrElse Settings.OverlayIdentifierSourceFieldName.Trim = String.Empty Then
					Throw New Exception("When Overwrite Mode is set to Overlay, Overlay Identifier Field must be set.")
				End If
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

#End Region

#Region " Event Handling "

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

		Private Sub _observer_ErrorReportEvent(ByVal row As System.Collections.IDictionary) Handles _observer.ErrorReportEvent
			RaiseEvent OnError(row)
		End Sub

#End Region

		Protected Overrides Sub Finalize()
			MyBase.Finalize()
		End Sub
	End Class
End Namespace