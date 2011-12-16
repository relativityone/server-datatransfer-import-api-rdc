Imports System.Runtime.CompilerServices
Imports System.Net


Namespace kCura.Relativity.DataReaderClient

	Public Class ImageImportBulkArtifactJob
		Implements IImportNotifier


#Region " Public Events and Variables "

		Public Event OnMessage(ByVal status As Status)
		Public Event OnError(ByVal row As IDictionary)
		Public Event OnComplete(ByVal jobReport As JobReport) Implements IImportNotifier.OnComplete
		Public Event OnFatalException(ByVal jobReport As JobReport) Implements IImportNotifier.OnFatalException
		Public Event OnProgress(ByVal completedRow As Long) Implements IImportNotifier.OnProgress


		Public Property Settings As ImageSettings
			Get
				Return _settings
			End Get
			<Obsolete("Assigning a value to the 'Settings' property is being phased out.  Please use the existing value of the property.")>
			Set(value As ImageSettings)
				_settings = value
			End Set
		End Property



		Public Property SourceData As ImageSourceIDataReader
			Get
				Return _sourceData
			End Get
			<Obsolete("Assigning a value to the 'SourceData' property is being phased out.  Please use the existing value of the property.")>
			Set(value As ImageSourceIDataReader)
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

#End Region

#Region " Public Methods "

		Public Sub New()
			_settings = New ImageSettings
			_sourceData = New ImageSourceIDataReader
			_cookieMonster = New Net.CookieContainer()
		End Sub

		Friend Sub New(ByVal credentials As ICredentials, ByVal cookieMonster As Net.CookieContainer, ByVal relativityUserName As String, ByVal password As String)
			Me.New()
			_credentials = credentials
			_cookieMonster = cookieMonster
			Settings.RelativityUsername = relativityUserName
			Settings.RelativityPassword = password

		End Sub




		Public Sub Execute()
			_jobReport = New JobReport()
			_jobReport.StartTime = DateTime.Now()

			If IsSettingsValid() Then
				RaiseEvent OnMessage(New Status("Getting source data from database"))

				MapSuppliedFieldNamesToActual(Settings, SourceData.SourceData)

				Dim process As New kCura.WinEDDS.ImportExtension.DataReaderImageImporterProcess(SourceData.SourceData)
				_observer = process.ProcessObserver
				_controller = process.ProcessController

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
			If credential Is Nothing Then
				credential = DirectCast(GetCredentials(Settings), Net.NetworkCredential)
			End If

			Dim casemanager As kCura.WinEDDS.Service.CaseManager = GetCaseManager(credential)
			Dim tempLoadFile As New kCura.WinEDDS.ImageLoadFile

			'These are ALL of the image file settings
			tempLoadFile.AutoNumberImages = Settings.AutoNumberImages
			tempLoadFile.CaseInfo = casemanager.Read(Settings.CaseArtifactId)
			'tempLoadFile.ControlKeyField = "Identifier"
			tempLoadFile.Credential = credential
			tempLoadFile.CookieContainer = _cookieMonster
			tempLoadFile.CopyFilesToDocumentRepository = Settings.CopyFilesToDocumentRepository
			tempLoadFile.DestinationFolderID = tempLoadFile.CaseInfo.RootFolderID
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

		Private Function GetCredentials(ByVal settings As ImageSettings) As System.Net.ICredentials
			Dim credential As System.Net.ICredentials = Nothing
			If credential Is Nothing Then
				Try
					credential = kCura.WinEDDS.Api.LoginHelper.LoginWindowsAuth(_cookieMonster)
				Catch
				End Try
			End If

			While credential Is Nothing
				credential = kCura.WinEDDS.Api.LoginHelper.LoginUsernamePassword(settings.RelativityUsername, settings.RelativityPassword, _cookieMonster)
				Exit While
			End While
			Return credential
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
			If Not _credentials Is Nothing Then
				' using the new ImportAPI class, credentials will be passed in automatically.  In that case, the username and password will be ignored and the original credentials will be used.
				If Settings.RelativityUsername Is Nothing OrElse Settings.RelativityUsername = String.Empty Then
					Throw New ImportSettingsException("RelativityUserName")
				End If
				If Settings.RelativityPassword Is Nothing OrElse Settings.RelativityPassword = String.Empty Then
					Throw New ImportSettingsException("RelativityPassword")
				End If
			End If

			If Settings.CaseArtifactId <= 0 Then
				Throw New ImportSettingsException("CaseArtifactId", "This must be the ID of an existing case.")
			End If
		End Sub

		Private Sub ValidateOverwriteModeSettings()
			If Settings.OverwriteMode = OverwriteModeEnum.Overlay AndAlso Settings.IdentityFieldId < 1 Then
				Throw New ImportSettingsException("IdentityFieldId", "When OverwriteMode is set to Overlay, then the IdentityFieldId must be set.")
			End If
		End Sub

		Private Sub ValidateExtractedTextSettings()
			If Settings.ExtractedTextFieldContainsFilePath AndAlso Settings.ExtractedTextEncoding Is Nothing Then
				Throw New ImportSettingsException("ExtractedTextEncoding")
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

			Dim msg As String = row.Item("Messages").ToString
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
#End Region

		''' <summary>
		''' ExportErrorReport
		''' Export the CSV file containing any errors from the import
		''' </summary>
		''' <param name="filePathAndName">Specify a full path and filename which will contain the output.</param>
		''' <remarks></remarks>
		Public Sub ExportErrorReport(ByVal filePathAndName As String)
			_controller.ExportErrorReport(filePathAndName)
		End Sub


	End Class
End Namespace