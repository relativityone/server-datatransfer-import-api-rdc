Imports kCura.WinEDDS.Exporters
Imports kCura.WinEDDS.TApi
Imports Relativity.DataTransfer.MessageService

Namespace kCura.WinEDDS
	Public Class ExportSearchProcess
		Inherits MonitoredProcessBase

		Private ReadOnly _loadFileHeaderFormatterFactory As ILoadFileHeaderFormatterFactory
		Private ReadOnly _exportConfig As IExportConfig
		Public ExportFile As ExportFile
		Private WithEvents _searchExporter As kCura.WinEDDS.Exporter
		Private _errorCount As Int32
		Private _warningCount As Int32
		Private _uploadModeText As String = Nothing
		Private _hasErrors As Boolean
		Protected Overrides ReadOnly Property JobType As String = "Export"

		Protected Overrides ReadOnly Property TapiClientName As String
			Get
				Return _tapiClientName
			End Get
		End Property

		Public Property UserNotification As Exporters.IUserNotification
		Public Property UserNotificationFactory As Func(Of Exporter, IUserNotification)

		Public Sub New(loadFileHeaderFormatterFactory As ILoadFileHeaderFormatterFactory, exportConfig As IExportConfig)
			MyBase.New(new MessageService())
			_loadFileHeaderFormatterFactory = loadFileHeaderFormatterFactory
			_exportConfig = exportConfig
		End Sub

		Public Sub New(loadFileHeaderFormatterFactory As ILoadFileHeaderFormatterFactory, exportConfig As IExportConfig, messageService As IMessageService)
			MyBase.New(messageService)
			_loadFileHeaderFormatterFactory = loadFileHeaderFormatterFactory
			_exportConfig = exportConfig
		End Sub

		Protected Overrides Sub OnSuccess()
			MyBase.OnSuccess()
			SendJobStatistics()
			SendTransferJobCompletedMessage()
			Me.ProcessObserver.RaiseStatusEvent("", "Export completed")
			Me.ProcessObserver.RaiseProcessCompleteEvent()
		End Sub

		Protected Overrides Sub OnFatalError()
			MyBase.OnFatalError()
			SendJobStatistics()
			SendTransferJobFailedMessage()
		End Sub

		Protected Overrides Sub OnHasErrors()
			MyBase.OnHasErrors()
			SendJobStatistics()
			SendTransferJobCompletedMessage()
			Me.ProcessObserver.RaiseProcessCompleteEvent(False, _searchExporter.ErrorLogFileName, True)
		End Sub

		Protected Overrides Function HasErrors() As Boolean
			Return _hasErrors
		End Function

		Protected Overrides Sub Initialize()
			MyBase.Initialize()
			_warningCount = 0
			_errorCount = 0
			_searchExporter = New Exporter(Me.ExportFile, Me.ProcessController, New Service.Export.WebApiServiceFactory(Me.ExportFile), 
											_loadFileHeaderFormatterFactory, _exportConfig) With {.InteractionManager = UserNotification}
			If Not UserNotificationFactory Is Nothing Then
				Dim un As IUserNotification = UserNotificationFactory(_searchExporter)
				If Not un Is Nothing Then
					_searchExporter.InteractionManager = un
					UserNotification = un
				End If
			End If
		End Sub

		Protected Overrides Function Run() As Boolean
			_hasErrors = Not _searchExporter.ExportSearch() 
			Return Not _hasFatalErrorOccured
		End Function

		Private Sub _searchExporter_FileTransferModeChangeEvent(ByVal mode As String) Handles _searchExporter.FileTransferModeChangeEvent
			If _uploadModeText Is Nothing Then
				_uploadModeText = Config.FileTransferModeExplanationText(False)
			End If
			_tapiClientName = mode
			SendTransferJobStartedMessage()
			Me.ProcessObserver.RaiseStatusBarEvent("File Transfer Mode: " & mode, _uploadModeText)
		End Sub

		Private Sub _productionExporter_StatusMessage(ByVal e As ExportEventArgs) Handles _searchExporter.StatusMessage
			Select Case e.EventType
				Case kCura.Windows.Process.EventType.Error
					_errorCount += 1
					Me.ProcessObserver.RaiseErrorEvent(e.DocumentsExported.ToString, e.Message)
				Case kCura.Windows.Process.EventType.Progress
					SendThroughputStatistics(e.Statistics.MetadataThroughput, e.Statistics.FileThroughput)
					Me.ProcessObserver.RaiseStatusEvent("", e.Message)
				Case kCura.Windows.Process.EventType.Status
					Me.ProcessObserver.RaiseStatusEvent(e.DocumentsExported.ToString, e.Message)
				Case kCura.Windows.Process.EventType.Warning
					_warningCount += 1
					Me.ProcessObserver.RaiseWarningEvent(e.DocumentsExported.ToString, e.Message)
				Case kCura.Windows.Process.EventType.ResetStartTime
					SetStartTime()
			End Select
			TotalRecords = e.TotalDocuments
			CompletedRecordsCount = e.DocumentsExported
			Dim statDict As IDictionary = Nothing
			If Not e.AdditionalInfo Is Nothing AndAlso TypeOf e.AdditionalInfo Is IDictionary Then
				statDict = DirectCast(e.AdditionalInfo, IDictionary)
			End If

			Me.ProcessObserver.RaiseProgressEvent(e.TotalDocuments, e.DocumentsExported, _warningCount, _errorCount, StartTime, New DateTime, e.Statistics.MetadataThroughput, e.Statistics.FileThroughput, Me.ProcessID, Nothing, Nothing, statDict)
		End Sub

		Private Sub _productionExporter_FatalErrorEvent(ByVal message As String, ByVal ex As System.Exception) Handles _searchExporter.FatalErrorEvent
			Me.ProcessObserver.RaiseFatalExceptionEvent(ex)
			_hasFatalErrorOccured = True
		End Sub

		Private Sub _searchExporter_ShutdownEvent() Handles _searchExporter.ShutdownEvent
			Me.ProcessObserver.Shutdown()
		End Sub
	End Class
End Namespace