Namespace kCura.Relativity.DataReaderClient
	Public Class ImageImportBulkArtifactJob

#Region " Public Events and Variables "

		Public Event OnMessage(ByVal status As Status)
		Public Event OnError(ByVal row As IDictionary)

		Public Settings As ImageSettings
		Public SourceData As ImageSourceIDataReader
		Private _caseManager As kCura.WinEDDS.Service.CaseManager

#End Region

#Region " Private variables "
		Private WithEvents _observer As kCura.Windows.Process.ProcessObserver
		Private cookieMonster As New Net.CookieContainer
#End Region

#Region " Public Methods "

		Public Sub New()
			Me.Settings = New ImageSettings
			Me.SourceData = New ImageSourceIDataReader
		End Sub

		Public Sub Execute()
			If IsSettingsValid() Then
				RaiseEvent OnMessage(New Status("Getting source data from database"))

				WinEDDS.Config.ProgrammaticServiceURL = Settings.WebServiceURL

				MapSuppliedFieldNamesToActual(Settings, SourceData.SourceData)

				Dim process As New kCura.WinEDDS.ImportExtension.DataReaderImageImporterProcess(SourceData.SourceData)
				_observer = process.ProcessObserver

				RaiseEvent OnMessage(New Status("Updating settings"))
				process.ImageLoadFile = Me.CreateLoadFile()

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

#End Region

#Region " Private Methods "

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
			Dim credential As System.Net.NetworkCredential = DirectCast(GetCredentials(Settings), Net.NetworkCredential)
			Dim casemanager As kCura.WinEDDS.Service.CaseManager = GetCaseManager(credential)
			Dim tempLoadFile As New kCura.WinEDDS.ImageLoadFile
			'tempLoadFile.DataTable = SourceData.SourceData

			'These are ALL of the image file settings
			tempLoadFile.AutoNumberImages = Settings.AutoNumberImages
			tempLoadFile.CaseInfo = casemanager.Read(Settings.CaseArtifactId)
			'tempLoadFile.ControlKeyField = "Identifier"
			tempLoadFile.Credential = credential
			tempLoadFile.CookieContainer = cookieMonster
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

			If Settings.ProductionArtifactID = 0 Then
				tempLoadFile.ProductionArtifactID = 0
				'tempLoadFile.ProductionTable = Nothing	'Don't know what this should be
			Else
				tempLoadFile.ProductionArtifactID = Settings.ProductionArtifactID
			End If

			tempLoadFile.SelectedCasePath = tempLoadFile.CaseInfo.DocumentPath
			tempLoadFile.SendEmailOnLoadCompletion = False
			tempLoadFile.StartLineNumber = 0
			'tempLoadFile.BeginBatesFieldArtifactID = 1035276

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
			Dim dt As System.Data.DataTable = New kCura.WinEDDS.Service.FieldQuery(credential, cookieMonster).RetrievePotentialBeginBatesFields(caseArtifactID).Tables(0)
			For Each identifierRow As System.Data.DataRow In dt.Rows
				If CType(identifierRow("FieldCategoryID"), Global.Relativity.FieldCategory) = Global.Relativity.FieldCategory.Identifier Then
					retval = CType(identifierRow("ArtifactID"), Int32)
				End If
			Next
			Return retval
		End Function

		Private Function GetCaseManager(ByVal credentials As Net.ICredentials) As kCura.WinEDDS.Service.CaseManager
			Return New kCura.WinEDDS.Service.CaseManager(credentials, cookieMonster)
		End Function

		Private Function GetCredentials(ByVal settings As ImageSettings) As System.Net.ICredentials
			Dim credential As System.Net.ICredentials = Nothing
			If credential Is Nothing Then
				Try
					credential = kCura.WinEDDS.Api.LoginHelper.LoginWindowsAuth(cookieMonster)
				Catch
				End Try
			End If

			While credential Is Nothing
				credential = kCura.WinEDDS.Api.LoginHelper.LoginUsernamePassword(settings.RelativityUsername, settings.RelativityPassword, cookieMonster)
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
				'ValidateOverwriteModeSettings()
				'ValidateExtractedTextSettings()
			Catch ex As Exception
				RaiseEvent OnMessage(New Status(ex.Message))
				Return False
			End Try
			Return True
		End Function

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
			RaiseEvent OnMessage(New Status(String.Format("[Timestamp: {0}] [Progress Info: {1} of {2}]", System.DateTime.Now, evt.TotalRecordsProcessedDisplay, evt.TotalRecordsDisplay)))
		End Sub

#End Region

	End Class
End Namespace