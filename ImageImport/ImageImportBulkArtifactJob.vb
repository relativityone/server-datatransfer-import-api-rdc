Namespace kCura.Relativity.DataReaderClient
	Public Class ImageImportBulkArtifactJob

#Region " Public Evens and Variables "

		Public Event OnMessage(ByVal status As Status)
		Public Settings As ImageSettings
		Public SourceData As SourceIDataReader

#End Region

#Region " Private variables "
		Private WithEvents _observer As kCura.Windows.Process.ProcessObserver
		Private cookieMonster As New Net.CookieContainer
#End Region

#Region " Public Methods "

		Public Sub New()
			Me.Settings = New ImageSettings
			Me.SourceData = New SourceIDataReader
		End Sub

		Public Sub Execute()
			If IsSettingsValid() Then
				RaiseEvent OnMessage(New Status("Getting source data from database"))

				Dim process As New kCura.WinEDDS.ImportExtension.DataReaderImageImporterProcess(SourceData.SourceData)
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
				RaiseEvent OnMessage(New Status("There was an error in your settings.  Im	port aborted."))
			End If
		End Sub

#End Region

#Region " Private Methods "

		Private Function CreateLoadFile(ByVal sqlClientSettings As ImageSettings) As kCura.WinEDDS.ImportExtension.DataReaderImageFile
			Dim tempLoadFile As New kCura.WinEDDS.ImportExtension.DataReaderImageFile
			tempLoadFile.DataReader = SourceData.SourceData

			'These are ALL of the load file settings
			tempLoadFile.AutoNumberImages = False	' NOT IN GENERAL SETTINGS
			tempLoadFile.CaseInfo = New kCura.EDDS.Types.CaseInfo
			tempLoadFile.CaseInfo.ArtifactID = 1028042 '1015022
			tempLoadFile.CaseInfo.DocumentPath = "\\localhost\fileRepo\"
			tempLoadFile.CaseInfo.DownloadHandlerURL = "EDDS.Distributed"
			tempLoadFile.CaseInfo.MatterArtifactID = 1000002 '1015021
			tempLoadFile.CaseInfo.Name = "_MAIN"
			tempLoadFile.CaseInfo.RootArtifactID = 1003663
			tempLoadFile.CaseInfo.RootFolderID = 1003697
			tempLoadFile.ControlKeyField = "Identifier"
			tempLoadFile.Credential = DirectCast(GetCredentials(Settings, cookieMonster), Net.NetworkCredential)
			tempLoadFile.CookieContainer = cookiemonster
			tempLoadFile.CopyFilesToDocumentRepository = True
			tempLoadFile.DestinationFolderID = tempLoadFile.CaseInfo.RootFolderID
			tempLoadFile.FileName = "Dicks"
			tempLoadFile.ForProduction = False
			tempLoadFile.FullTextEncoding = Nothing
			tempLoadFile.IdentityFieldId = 1003667 'e.x COntrol Number
			tempLoadFile.Overwrite = "Append"
			'tempLoadFile.ProductionArtifactID = 1038014
			tempLoadFile.ProductionArtifactID = 0
			tempLoadFile.ProductionTable = Nothing
			tempLoadFile.SelectedCasePath = tempLoadFile.CaseInfo.DocumentPath
			tempLoadFile.SendEmailOnLoadCompletion = False
			tempLoadFile.StartLineNumber = 0
			'tempLoadFile.BeginBatesFieldArtifactID = 1035276

			Return tempLoadFile

		End Function

		Private Function GetCredentials(ByVal settings As ImageSettings, ByVal cookieMonster As Net.CookieContainer) As System.Net.ICredentials
			Dim credential As System.Net.ICredentials
			If credential Is Nothing Then
				credential = kCura.WinEDDS.Api.LoginHelper.LoginWindowsAuth(cookieMonster)
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
				ValidateDelimiterSettings()
				ValidateOverwriteModeSettings()
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
			RaiseEvent OnMessage(New Status(String.Format("[Timestamp: {0}] [Progress Info: {1} of {2}]", System.DateTime.Now, evt.TotalRecordsProcessedDisplay, evt.TotalRecordsDisplay)))
		End Sub

#End Region

	End Class
End Namespace