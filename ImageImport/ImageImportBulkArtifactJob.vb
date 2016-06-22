Imports System.Net
Imports kCura.WinEDDS
Imports Relativity

Namespace kCura.Relativity.DataReaderClient

	''' <summary>
	''' Provides the functionality required to load data for an import job and to retrieve messages from the OnMessage event.
	''' </summary>
	Public Class ImageImportBulkArtifactJob
		Implements IImportNotifier

#Region " Public Events and Variables "

		''' <summary>
		''' Occurs when a status message needs to be presented to the user.
		''' </summary>
		''' <param name="status">The message.</param>
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

		''' <summary>
		''' Gets or sets the current options for imaging files.
		''' </summary>
		''' <value>
		''' The settings.
		''' </value>
		Public Property Settings As ImageSettings
			Get
				Return _settings
			End Get
			Friend Set(value As ImageSettings)
				_settings = value
			End Set
		End Property

		''' <summary>
		''' Represents an instance of the SourceIDataReader, which contains data for import. This property is required.
		''' </summary>
		''' <value>
		''' The source data.
		''' </value>
		''' <returns></returns><remarks>
		''' For standard imports, the SourceIDataReader requires a generic IDataReader object and operates as an iterator over a DataTable instance that contains the data source.
		''' </remarks>
		Public Property SourceData As ImageSourceIDataReader
			Get
				Return _sourceData
			End Get
			Friend Set(value As ImageSourceIDataReader)
				_sourceData = value
			End Set
		End Property
#End Region

#Region " Private variables "
		Private WithEvents _observer As kCura.Windows.Process.ProcessObserver
		Private _controller As kCura.Windows.Process.Controller
		Private _settings As ImageSettings
		Private _sourceData As ImageSourceIDataReader
		Private _credentials As ICredentials
		Private _cookieMonster As Net.CookieContainer
		Private _jobReport As JobReport
		Private _executionSource As ExecutionSource
#End Region

#Region " Public Methods "

		''' <summary>
		''' Creates new job to import images in bulk.
		''' </summary>
		Public Sub New()
			_settings = New ImageSettings
			_sourceData = New ImageSourceIDataReader
			_cookieMonster = New Net.CookieContainer()
		End Sub

		''' <summary>
		''' Initializes a new instance of the <see cref="ImageImportBulkArtifactJob"/> class.
		''' </summary>
		''' <param name="credentials">The credentials.</param>
		''' <param name="cookieMonster">The cookie monster.</param>
		''' <param name="relativityUserName">Name of the relativity user.</param>
		''' <param name="password">The password.</param>
		''' <param name="executionSource">Optional parameter that states what process the import is coming from.</param>
		Friend Sub New(ByVal credentials As ICredentials, ByVal cookieMonster As Net.CookieContainer, ByVal relativityUserName As String, ByVal password As String, ByVal Optional executionSource As Integer = 0)
			Me.New()
			_executionSource = CType(executionSource, ExecutionSource)
			_credentials = credentials
			_cookieMonster = cookieMonster
			Settings.RelativityUsername = relativityUserName
			Settings.RelativityPassword = password

		End Sub

		''' <summary>
		''' Executes the DataReaderClient, which operates as an iterator over a data source.
		''' </summary>
		Public Sub Execute()
			_jobReport = New JobReport()
			_jobReport.StartTime = DateTime.Now()

			' Authenticate here instead of in CreateLoadFile
			If _credentials Is Nothing Then
				ImportCredentialManager.WebServiceURL = Settings.WebServiceURL
				Dim creds As ImportCredentialManager.SessionCredentials = ImportCredentialManager.GetCredentials(Settings.RelativityUsername, Settings.RelativityPassword)
				_credentials = creds.Credentials
				_cookieMonster = creds.CookieMonster
			End If

			If IsSettingsValid() Then
				RaiseEvent OnMessage(New Status("Getting source data from database"))

				MapSuppliedFieldNamesToActual(Settings, SourceData.SourceData)

				Dim process As New kCura.WinEDDS.ImportExtension.DataReaderImageImporterProcess(SourceData.SourceData)
				process.ExecutionSource = _executionSource
				_observer = process.ProcessObserver
				_controller = process.ProcessController

				If Settings.DisableImageTypeValidation.HasValue Then process.DisableImageTypeValidation = Settings.DisableImageTypeValidation.Value
				If Settings.DisableImageLocationValidation.HasValue Then process.DisableImageLocationValidation = Settings.DisableImageLocationValidation.Value
				process.MaximumErrorCount = Settings.MaximumErrorCount
				process.DisableUserSecurityCheck = Settings.DisableUserSecurityCheck
				process.AuditLevel = Settings.AuditLevel
				process.SkipExtractedTextEncodingCheck = Settings.DisableExtractedTextEncodingCheck
				RaiseEvent OnMessage(New Status("Updating settings"))
				process.ImageLoadFile = Me.CreateLoadFile()

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
				RaiseFatalException()

			End If
		End Sub

#End Region

#Region " Private Methods "
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

		Private Sub ValidateDataSourceSettings()
			'This expects the DataTable in SourceData to have already been set
			EnsureFieldNameIsValid("BatesNumber", Settings.BatesNumberField)
			EnsureFieldNameIsValid("DocumentIdentifier", Settings.DocumentIdentifierField)
			EnsureFieldNameIsValid("FileLocation", Settings.FileLocationField)
		End Sub

		Private Sub EnsureFieldNameIsValid(ByVal imageSettingsField As String, ByVal forFieldName As String)
			If String.IsNullOrEmpty(forFieldName) Then
				Throw New Exception("No field name specified for " & imageSettingsField)
			End If

			If Not SourceData.SourceData.Columns.Contains(forFieldName) Then
				Throw New Exception(String.Format("No field named {0} found in the DataTable for " & imageSettingsField, forFieldName))
			End If
		End Sub

		Private Function CreateLoadFile() As WinEDDS.ImageLoadFile

			Dim credential As System.Net.NetworkCredential = DirectCast(_credentials, Net.NetworkCredential)

			Dim casemanager As kCura.WinEDDS.Service.CaseManager = GetCaseManager(credential)
			Dim tempLoadFile As New kCura.WinEDDS.ImageLoadFile

			'These are ALL of the image file settings
			tempLoadFile.AutoNumberImages = Settings.AutoNumberImages
			tempLoadFile.CaseInfo = casemanager.Read(Settings.CaseArtifactId)
			'tempLoadFile.ControlKeyField = "Identifier"
			tempLoadFile.Credential = credential
			tempLoadFile.CookieContainer = _cookieMonster
			tempLoadFile.CopyFilesToDocumentRepository = Settings.CopyFilesToDocumentRepository
			If Settings.DestinationFolderArtifactID > 0 Then
				tempLoadFile.DestinationFolderID = Settings.DestinationFolderArtifactID
			Else
				tempLoadFile.DestinationFolderID = tempLoadFile.CaseInfo.RootFolderID
			End If
			tempLoadFile.ForProduction = Settings.ForProduction
			tempLoadFile.FullTextEncoding = Settings.ExtractedTextEncoding
			tempLoadFile.Overwrite = Settings.OverwriteMode.ToString
			tempLoadFile.ReplaceFullText = Settings.ExtractedTextFieldContainsFilePath
			If tempLoadFile.Overwrite = OverwriteModeEnum.Overlay.ToString Then
				tempLoadFile.IdentityFieldId = GetDefaultIdentifierFieldID(credential, Settings.CaseArtifactId)
			Else
				tempLoadFile.IdentityFieldId = Settings.IdentityFieldId	'e.x Control Number
			End If

			tempLoadFile.ProductionArtifactID = Settings.ProductionArtifactID
			tempLoadFile.SelectedCasePath = tempLoadFile.CaseInfo.DocumentPath
			tempLoadFile.SendEmailOnLoadCompletion = False
			tempLoadFile.StartLineNumber = 0
			tempLoadFile.BeginBatesFieldArtifactID = GetDefaultIdentifierFieldID(credential, Settings.CaseArtifactId)
			Return tempLoadFile
		End Function

		Private Sub MapSuppliedFieldNamesToActual(ByVal imageSettings As ImageSettings, ByRef srcDataTable As DataTable)
			'kCura.WinEDDS.ImportExtension.ImageDataTableReader contains the 'real' field names
			srcDataTable.Columns(imageSettings.BatesNumberField).ColumnName = "BatesNumber"
			srcDataTable.Columns(imageSettings.DocumentIdentifierField).ColumnName = "DocumentIdentifier"
			srcDataTable.Columns(imageSettings.FileLocationField).ColumnName = "FileLocation"
		End Sub

		Private Function GetDefaultIdentifierFieldID(ByVal credential As System.Net.NetworkCredential, ByVal caseArtifactID As Int32) As Int32
			Dim retval As Int32
			Dim dt As System.Data.DataTable = New kCura.WinEDDS.Service.FieldQuery(credential, _cookieMonster).RetrievePotentialBeginBatesFields(caseArtifactID).Tables(0)
			For Each identifierRow As System.Data.DataRow In dt.Rows
				If CType(identifierRow("FieldCategoryID"), Global.Relativity.FieldCategory) = Global.Relativity.FieldCategory.Identifier Then
					retval = CType(identifierRow("ArtifactID"), Int32)
				End If
			Next
			Return retval
		End Function

		Private Function GetCaseManager(ByVal credentials As Net.ICredentials) As kCura.WinEDDS.Service.CaseManager
			Return New kCura.WinEDDS.Service.CaseManager(credentials, _cookieMonster)
		End Function


#End Region

#Region " Validation "
		Private Function IsSettingsValid() As Boolean
			Try
				'	ValidateSqlCommandSettings()
				ValidateRelativitySettings()
				ValidateDataSourceSettings()
				ValidateOverwriteModeSettings()
				ValidateExtractedTextSettings()
				ValidateProductionSettings()
			Catch ex As Exception
				_jobReport.FatalException = ex
				RaiseEvent OnMessage(New Status(ex.Message))
				Return False
			End Try
			Return True
		End Function

		Private Sub ValidateRelativitySettings()
			If Settings.CaseArtifactId <= 0 Then
				Throw New ImportSettingsException("CaseArtifactId", "This must be the ID of an existing case.")
			End If
			If Settings.MaximumErrorCount.HasValue AndAlso (Settings.MaximumErrorCount.Value < 1 OrElse Settings.MaximumErrorCount.Value = Int32.MaxValue) Then
				Throw New ImportSettingsException("MaximumErrorCount", "This must be greater than 0 and less than Int32.MaxValue.")
			End If
		End Sub

		Private Sub ValidateOverwriteModeSettings()
			If Settings.OverwriteMode = OverwriteModeEnum.Overlay AndAlso Settings.IdentityFieldId < 1 Then
				Throw New ImportSettingsException("IdentityFieldId", "When OverwriteMode is set to Overlay, then the IdentityFieldId must be set.")
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

		Private Sub ValidateProductionSettings()
			If Settings.ForProduction AndAlso Settings.ProductionArtifactID < 1 Then
				Throw New ImportSettingsException("ProductionArtifactID", "When specifying a production import, the ProductionArtifactID must be set.")
			End If
		End Sub
#End Region

#Region " Event Handling "
		Private Sub _observer_ErrorReportEvent(ByVal row As System.Collections.IDictionary) Handles _observer.ErrorReportEvent
			RaiseEvent OnError(row)

			Dim retval As New System.Text.StringBuilder
			retval.AppendFormat("[error] ")
			For Each key As String In row.Keys
				retval.Append(key)
				retval.Append(": ")
				retval.Append(row(key))
				retval.Append(vbNewLine)
			Next
			RaiseEvent OnMessage(New Status(retval.ToString))

			' slm 10/10/2011 - really?  these are called "Messages" and "Line Number"?  These are different names than for the native document import.
			' cjh 2/28/2012 - turning Messages all to Message.  Use only "Message"

			Dim msg As String = row.Item("Message").ToString
			Dim lineNumbObj As Object = row.Item("Line Number")
			Dim lineNum As Long = 0
			If Not lineNumbObj Is Nothing Then
				lineNum = DirectCast(lineNumbObj, Int32)
			End If

			Dim idobj As Object
			idobj = row.Item("DocumentID")
			Dim id As String = String.Empty
			If Not idobj Is Nothing Then
				id = idobj.ToString()
			End If

			_jobReport.ErrorRows.Add(New JobReport.RowError(lineNum, msg, id))

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

		Private Sub _observer_OnProcessFatalException(ByVal ex As System.Exception) Handles _observer.OnProcessFatalException
			RaiseEvent OnMessage(New Status(String.Format("FatalException: {0}", ex.ToString)))
			_jobReport.FatalException = ex
			RaiseEvent OnFatalException(_jobReport)
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

		''' <summary>
		''' Exports the error log file for an import job. This file is written only when errors occur.
		''' </summary>
		''' <param name="filePathAndName">Specifies a full path and a filename to contain the output.</param>
		Public Sub ExportErrorReport(ByVal filePathAndName As String)
			_controller.ExportErrorReport(filePathAndName)
		End Sub


	End Class
End Namespace