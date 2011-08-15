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

				Dim updatedSourceData As DataTable = MapSuppliedFieldNamesToActual(Settings, SourceData.SourceData)
				SourceData.SourceData = updatedSourceData

				If String.IsNullOrWhiteSpace(Settings.ServiceURL) Then
					Settings.ServiceURL = kCura.WinEDDS.Config.WebServiceURL
					RaiseEvent OnMessage(New Status("Setting default ServiceURL, none was found"))
				End If

				Dim process As WinEDDS.ImportExtension.DataReaderImageImporterProcess = New WinEDDS.ImportExtension.DataReaderImageImporterProcess(SourceData.SourceData, Settings.ServiceURL)
				RaiseEvent OnMessage(New Status(String.Format("Using value {0} for ServiceURL", Settings.ServiceURL)))

				_observer = process.ProcessObserver

				RaiseEvent OnMessage(New Status("Updating settings"))
				process.ImageLoadFile = Me.CreateLoadFile(Settings)

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

		Private Function CreateLoadFile(ByVal imgSettings As ImageSettings) As WinEDDS.ImageLoadFile
			Dim credential As System.Net.NetworkCredential = DirectCast(GetCredentials(imgSettings), Net.NetworkCredential)
			Dim casemanager As kCura.WinEDDS.Service.CaseManager = GetCaseManager(credential)

			Dim tempImgSettings As WinEDDS.ImageSettingsFactory = MapToImageSettingsFactory(credential, imgSettings)
			Dim tempLoadFile As WinEDDS.ImageLoadFile = tempImgSettings.ToLoadFile()

			'These are ALL of the image file settings
			tempLoadFile.AutoNumberImages = imgSettings.AutoNumberImages
			'These 3 will get set as a result of ImageSettingsFactory.CaseArtifactID
			'tempLoadFile.CaseInfo
			'tempLoadFile.CaseDefaultPath
			'tempLoadFile.DestinationFolderID
			tempLoadFile.Credential = credential
			tempLoadFile.CookieContainer = cookieMonster
			tempLoadFile.DestinationFolderID = tempLoadFile.CaseInfo.RootFolderID
			tempLoadFile.ForProduction = imgSettings.ForProduction
			tempLoadFile.FullTextEncoding = imgSettings.ExtractedTextEncoding
			tempLoadFile.Overwrite = imgSettings.OverwriteMode.ToString
			tempLoadFile.ReplaceFullText = imgSettings.ExtractedTextFieldContainsFilePath
			If tempLoadFile.Overwrite = OverwriteModeEnum.Overlay.ToString Then
				tempLoadFile.IdentityFieldId = GetDefaultIdentifierFieldID(credential, imgSettings.CaseArtifactId)
			Else
				tempLoadFile.IdentityFieldId = imgSettings.IdentityFieldId	'e.x Control Number
			End If

			tempLoadFile.SelectedCasePath = tempLoadFile.CaseInfo.DocumentPath
			tempLoadFile.SendEmailOnLoadCompletion = False
			tempLoadFile.StartLineNumber = 0
			'tempLoadFile.BeginBatesFieldArtifactID = 1035276

			Return tempLoadFile

		End Function

		'TODO: Allow user to specify "Just overwrite the original column names to avoid a copy operation"?
		Private Function MapSuppliedFieldNamesToActual(ByVal imageSettings As ImageSettings, ByVal srcDataTable As DataTable) As DataTable
			'kCura.WinEDDS.ImportExtension.ImageDataTableReader contains the real field names
			Dim returnDataTbl As DataTable = srcDataTable.Copy()
			returnDataTbl.Columns(imageSettings.BatesNumberField).ColumnName = "BatesNumber"
			returnDataTbl.Columns(imageSettings.DocumentIdentifierField).ColumnName = "DocumentIdentifier"
			returnDataTbl.Columns(imageSettings.FileLocationField).ColumnName = "FileLocation"

			Return returnDataTbl
		End Function

		Private Function MapToImageSettingsFactory(ByVal credential As Net.NetworkCredential, ByVal imgSettings As ImageSettings) As WinEDDS.ImageSettingsFactory
			Dim tempImgSettings As WinEDDS.ImageSettingsFactory = New WinEDDS.ImageSettingsFactory(credential, imgSettings.CaseArtifactId, imgSettings.ServiceURL)

			With tempImgSettings
				If imgSettings.BeginBatesFieldArtifactID > 0 Then
					.BeginBatesFieldArtifactID = imgSettings.BeginBatesFieldArtifactID
				End If

				If imgSettings.DestinationFolderArtifactID > 0 Then
					.DestinationFolderID = imgSettings.DestinationFolderArtifactID
				End If

				Select Case imgSettings.NativeFileCopyMode
					Case NativeFileCopyModeEnum.CopyFiles
						.CopyFilesToDocumentRepository = True
					Case NativeFileCopyModeEnum.SetFileLinks, Nothing
						.CopyFilesToDocumentRepository = False
					Case Else
						Throw New Exception("ERROR with  NativeFileCopyMode")
				End Select

				Select Case imgSettings.OverwriteMode
					Case OverwriteModeEnum.Append
						.OverwriteDestination = WinEDDS.SettingsFactoryBase.OverwriteType.Append
					Case OverwriteModeEnum.AppendOverlay
						.OverwriteDestination = WinEDDS.SettingsFactoryBase.OverwriteType.AppendOverlay
					Case OverwriteModeEnum.Overlay
						.OverwriteDestination = WinEDDS.SettingsFactoryBase.OverwriteType.Overlay
				End Select

				If imgSettings.ProductionArtifactID = 0 Then
					.ProductionArtifactID = 0
					'.ProductionTable = Nothing	'Don't know what this should be
				Else
					.ProductionArtifactID = imgSettings.ProductionArtifactID
				End If
			End With

			Return tempImgSettings
		End Function

		Private Function GetDefaultIdentifierFieldID(ByVal credential As System.Net.NetworkCredential, ByVal caseArtifactID As Int32) As Int32
			Dim retval As Int32
			Dim tempFieldQuery As kCura.WinEDDS.Service.FieldQuery = New kCura.WinEDDS.Service.FieldQuery(credential, cookieMonster)
			tempFieldQuery.ServiceURL = Settings.ServiceURL

			Dim dt As System.Data.DataTable = tempFieldQuery.RetrievePotentialBeginBatesFields(caseArtifactID).Tables(0)
			For Each identifierRow As System.Data.DataRow In dt.Rows
				If CType(identifierRow("FieldCategoryID"), Global.Relativity.FieldCategory) = Global.Relativity.FieldCategory.Identifier Then
					retval = CType(identifierRow("ArtifactID"), Int32)
				End If
			Next
			Return retval
		End Function

		Private Function GetCaseManager(ByVal credentials As Net.ICredentials) As kCura.WinEDDS.Service.CaseManager
			Dim returnCaseMan As kCura.WinEDDS.Service.CaseManager = New kCura.WinEDDS.Service.CaseManager(credentials, cookieMonster)
			returnCaseMan.ServiceURL = Settings.ServiceURL

			Return returnCaseMan
		End Function

		Private Function GetCredentials(ByVal settings As ImageSettings) As System.Net.ICredentials
			Dim credential As System.Net.ICredentials = Nothing
			If credential Is Nothing Then
				Try
					credential = kCura.WinEDDS.Api.LoginHelper.LoginWindowsAuthWithServiceURL(cookieMonster, settings.ServiceURL)
				Catch
				End Try
			End If

			While credential Is Nothing
				credential = kCura.WinEDDS.Api.LoginHelper.LoginUsernamePasswordWithServiceURL(settings.RelativityUsername, settings.RelativityPassword, cookieMonster, settings.ServiceURL)
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
				ValidateDelimiterSettings()
				ValidateDataSourceSettings()
				'ValidateOverwriteModeSettings()
				'ValidateExtractedTextSettings()
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

		Private Sub ValidateDataSourceSettings()
			'This expects the DataTable in SourceData to have already been set

			If String.IsNullOrEmpty(Settings.BatesNumberField) Then
				Throw New Exception("No field name specified for BatesNumber")
			Else
				If Not SourceData.SourceData.Columns.Contains(Settings.BatesNumberField) Then
					Throw New Exception(String.Format("No field named {0} found in the DataTable for BatesNumber", Settings.BatesNumberField))
				End If
			End If

			If String.IsNullOrEmpty(Settings.DocumentIdentifierField) Then
				Throw New Exception("No field name specified for DocumentIdentifier")
			Else
				If Not SourceData.SourceData.Columns.Contains(Settings.DocumentIdentifierField) Then
					Throw New Exception(String.Format("No field named {0} found in the DataTable for DocumentIdentifier", Settings.DocumentIdentifierField))
				End If
			End If

			If String.IsNullOrEmpty(Settings.FileLocationField) Then
				Throw New Exception("No field name specified for FileLocation")
			Else
				If Not SourceData.SourceData.Columns.Contains(Settings.FileLocationField) Then
					Throw New Exception(String.Format("No field named {0} found in the DataTable for FileLocation", Settings.FileLocationField))
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