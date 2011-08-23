Imports kCura.WinEDDS.ImportExtension
Imports kCura.WinEDDS

Namespace kCura.Relativity.DataReaderClient
	Public Class ImageImportBulkArtifactJob
		Inherits LoadFileJobBase

#Region " Public Events and Variables "
		Public Settings As ImageSettings
		Public SourceData As ImageSourceIDataReader

#End Region

#Region "Private Variables"
		Private ReadOnly _cookieContainer As New Net.CookieContainer
#End Region

#Region "Constructors"
		Public Sub New()
			Me.Settings = New ImageSettings
			Me.SourceData = New ImageSourceIDataReader
		End Sub
#End Region

#Region "Events"
		Public Shadows Event OnError(ByVal row As IDictionary)
		Public Shadows Event OnMessage(ByVal status As Status)
#End Region

#Region "Public Routines"
		Public Overrides Sub Execute()
			If IsSettingsValid() Then
				'TODO: Remove this? What database data???
				RaiseEvent OnMessage(New Status("Getting source data from database"))

				MapSuppliedFieldNamesToActual(Settings, SourceData.SourceData)

				SelectServiceURL()

				Dim process As ImportImageFileProcess = CreateImageImporterProcess(SourceData.SourceData, Settings.WebServiceURL)
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

		Protected Overridable Function CreateImageImporterProcess(ByVal sourceData As DataTable, ByVal webServiceUrl As String) As ImportImageFileProcess
			Return New DataReaderImageImporterProcess(sourceData, webServiceUrl)
		End Function
#End Region

#Region "Private Functions"
		Private Function CreateLoadFile(ByVal imgSettings As ImageSettings) As WinEDDS.ImageLoadFile
			Dim tempImgSettings As WinEDDS.ImageSettingsFactory = MapInputToSettingsFactory(imgSettings)
			Dim tempLoadFile As WinEDDS.ImageLoadFile = tempImgSettings.ToLoadFile()

			'These are ALL of the image file settings
			tempLoadFile.AutoNumberImages = imgSettings.AutoNumberImages
			'These 3 will get set as a result of ImageSettingsFactory.CaseArtifactID
			'tempLoadFile.CaseInfo
			'tempLoadFile.CaseDefaultPath
			'tempLoadFile.DestinationFolderID
			'These 2 will get set in ImageSettingsFactory
			'tempLoadFile.Credential = CType(GetCredentials(Settings), Net.NetworkCredential)
			'tempLoadFile.CookieContainer = _cookieContainer
			tempLoadFile.ForProduction = imgSettings.ForProduction
			tempLoadFile.FullTextEncoding = imgSettings.ExtractedTextEncoding
			tempLoadFile.ReplaceFullText = imgSettings.ExtractedTextFieldContainsFilePath
			tempLoadFile.SendEmailOnLoadCompletion = imgSettings.SendEmailOnLoadCompletion

			Return tempLoadFile
		End Function

		Private Function GetCredentials(ByVal imgSettings As ImageSettings) As Net.ICredentials
			Dim credential As System.Net.ICredentials = Nothing
			If credential Is Nothing Then
				Try
					credential = kCura.WinEDDS.Api.LoginHelper.LoginWindowsAuthWithServiceURL(_cookieContainer, imgSettings.WebServiceURL)
				Catch
				End Try
			End If

			While credential Is Nothing
				credential = kCura.WinEDDS.Api.LoginHelper.LoginUsernamePasswordWithServiceURL(imgSettings.RelativityUsername, imgSettings.RelativityPassword, _cookieContainer, imgSettings.WebServiceURL)
				Exit While
			End While
			Return credential
		End Function

		Private Function GetDefaultIdentifierFieldID(ByVal credential As Net.NetworkCredential, ByVal caseArtifactID As Int32) As Int32
			Dim retval As Int32
			Dim tempFieldQuery As WinEDDS.Service.FieldQuery = New WinEDDS.Service.FieldQuery(credential, _cookieContainer, Settings.WebServiceURL)

			Dim dt As System.Data.DataTable = tempFieldQuery.RetrievePotentialBeginBatesFields(caseArtifactID).Tables(0)
			For Each identifierRow As System.Data.DataRow In dt.Rows
				If CType(identifierRow("FieldCategoryID"), Global.Relativity.FieldCategory) = Global.Relativity.FieldCategory.Identifier Then
					retval = CType(identifierRow("ArtifactID"), Int32)
				End If
			Next
			Return retval
		End Function

		Protected Overrides Function IsSettingsValid() As Boolean
			Try
				ValidateRelativitySettings()
				ValidateDataSourceSettings()
			Catch ex As Exception
				RaiseEvent OnMessage(New Status(ex.Message))
				Return False
			End Try
			Return True
		End Function

		Private Sub MapSuppliedFieldNamesToActual(ByVal imageSettings As ImageSettings, ByRef srcDataTable As DataTable)
			'kCura.WinEDDS.ImportExtension.ImageDataTableReader contains the 'real' field names
			srcDataTable.Columns(imageSettings.BatesNumberField).ColumnName = "BatesNumber"
			srcDataTable.Columns(imageSettings.DocumentIdentifierField).ColumnName = "DocumentIdentifier"
			srcDataTable.Columns(imageSettings.FileLocationField).ColumnName = "FileLocation"
		End Sub

		Private Function MapInputToSettingsFactory(ByVal imgSettings As ImageSettings) As WinEDDS.ImageSettingsFactory
			Dim tempCredential As Net.NetworkCredential = CType(GetCredentials(imgSettings), Net.NetworkCredential)
			Dim tempImgSettings As WinEDDS.ImageSettingsFactory = New WinEDDS.ImageSettingsFactory(tempCredential, imgSettings.CaseArtifactId, imgSettings.WebServiceURL)

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

				If Not imgSettings.OverwriteMode = OverwriteModeEnum.Overlay AndAlso imgSettings.IdentityFieldId > 0 Then
					.IdentityFieldId = imgSettings.IdentityFieldId
				Else
					.IdentityFieldId = GetDefaultIdentifierFieldID(tempCredential, imgSettings.CaseArtifactId)
				End If

				If imgSettings.ProductionArtifactID = 0 Then
					.ProductionArtifactID = 0
					'.ProductionTable = Nothing	'Don't know what this should be
				Else
					.ProductionArtifactID = imgSettings.ProductionArtifactID
				End If

				If imgSettings.StartRecordNumber > 0 Then
					.StartLineNumber = imgSettings.StartRecordNumber
				End If
			End With

			Return tempImgSettings
		End Function
#End Region

#Region "Private Routines"
		Protected Overrides Sub SelectServiceURL()
			If Not String.IsNullOrWhiteSpace(Settings.WebServiceURL) Then
				RaiseEvent OnMessage(New Status(String.Format("Using supplied ServiceURL {0}", Settings.WebServiceURL)))
			ElseIf Not String.IsNullOrWhiteSpace(WinEDDS.Config.AppConfigWebServiceURL) Then
				RaiseEvent OnMessage(New Status(String.Format("Using application configuration ServiceURL {0}", WinEDDS.Config.AppConfigWebServiceURL)))
				Settings.WebServiceURL = WinEDDS.Config.AppConfigWebServiceURL
			Else
				RaiseEvent OnMessage(New Status(String.Format("Using default ServiceURL {0}", WinEDDS.Config.WebServiceURL)))
				Settings.WebServiceURL = WinEDDS.Config.WebServiceURL
			End If
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
#End Region

#Region " Event Handling "
		Private Sub _observer_ErrorReportEvent(ByVal row As IDictionary) Handles _observer.ErrorReportEvent
			RaiseEvent OnError(row)

			Dim retval As New Text.StringBuilder
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