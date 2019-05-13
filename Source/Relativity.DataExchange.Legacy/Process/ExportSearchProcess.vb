Imports System.Threading
Imports kCura.WinEDDS.Exporters
Imports kCura.WinEDDS.Service.Export
Imports Relativity.DataExchange.Process
Imports Relativity.DataExchange.Transfer
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
			MyBase.New(New MessageService())
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
			SendTransferJobCompletedMessage()
			Me.Context.PublishStatusEvent("", "Export completed")
			Me.Context.PublishProcessCompleted()
		End Sub

		Protected Overrides Sub OnFatalError()
			MyBase.OnFatalError()
			SendTransferJobFailedMessage()
		End Sub

		Protected Overrides Sub OnHasErrors()
			MyBase.OnHasErrors()
			SendTransferJobCompletedMessage()
			Me.Context.PublishProcessCompleted(False, _searchExporter.ErrorLogFileName, True)
		End Sub

		Protected Overrides Function HasErrors() As Boolean
			Return _hasErrors
		End Function

		Private Function GetExporter() As Exporter
			If (ExportFile.UseCustomFileNaming) Then
				Return _
					New ExtendedExporter(TryCast(Me.ExportFile, ExtendedExportFile), Me.Context,
										 New WebApiServiceFactory(Me.ExportFile),
										 _loadFileHeaderFormatterFactory, _exportConfig) With {.InteractionManager = UserNotification}
			Else
				Return _
					New Exporter(Me.ExportFile, Me.Context,
										 New WebApiServiceFactory(Me.ExportFile),
										 _loadFileHeaderFormatterFactory, _exportConfig) With {.InteractionManager = UserNotification}

			End If
		End Function

		Protected Overrides Sub Initialize()
			MyBase.Initialize()
			_warningCount = 0
			_errorCount = 0
			_searchExporter = GetExporter()
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
				Dim tapiObjectService As ITapiObjectService = New TapiObjectService
				_uploadModeText = tapiObjectService.BuildFileTransferModeDocText(False)
			End If
			_tapiClientName = mode
			SendTransferJobStartedMessage()
			Me.Context.PublishStatusBarChanged("File Transfer Mode: " & mode, _uploadModeText)
		End Sub

		Private Sub _productionExporter_StatusMessage(ByVal e As ExportEventArgs) Handles _searchExporter.StatusMessage
			Select Case e.EventType
				Case EventType2.End
					SendJobStatistics(e.Statistics)
				Case EventType2.Error
					Interlocked.Increment(_errorCount)
					Me.Context.PublishErrorEvent(e.DocumentsExported.ToString, e.Message)
				Case EventType2.Progress
					SendThroughputStatistics(e.Statistics.MetadataThroughput, e.Statistics.FileThroughput)
					Me.Context.PublishStatusEvent("", e.Message)
				Case EventType2.Statistics
					SendThroughputStatistics(e.Statistics.MetadataThroughput, e.Statistics.FileThroughput)
				Case EventType2.Status
					Me.Context.PublishStatusEvent(e.DocumentsExported.ToString, e.Message)
				Case EventType2.Warning
					Interlocked.Increment(_warningCount)
					Me.Context.PublishWarningEvent(e.DocumentsExported.ToString, e.Message)
				Case EventType2.ResetStartTime
					SetStartTime()
			End Select
			TotalRecords = e.TotalDocuments
			CompletedRecordsCount = e.DocumentsExported
			Dim statDict As IDictionary = Nothing
			If Not e.AdditionalInfo Is Nothing AndAlso TypeOf e.AdditionalInfo Is IDictionary Then
				statDict = DirectCast(e.AdditionalInfo, IDictionary)
			End If

			Me.Context.PublishProgress(e.TotalDocuments, e.DocumentsExported, _warningCount, _errorCount, StartTime, New DateTime, e.Statistics.MetadataThroughput, e.Statistics.FileThroughput, Me.ProcessID, Nothing, Nothing, statDict)
		End Sub

		Private Sub _productionExporter_FatalErrorEvent(ByVal message As String, ByVal ex As System.Exception) Handles _searchExporter.FatalErrorEvent
			Me.Context.PublishFatalException(ex)
			_hasFatalErrorOccured = True
		End Sub

		Private Sub _searchExporter_ShutdownEvent() Handles _searchExporter.ShutdownEvent
			Me.Context.PublishShutdown()
		End Sub
	End Class
End Namespace