Imports Relativity.Import.Export
Imports Relativity.Import.Export.Io
Imports Relativity.Import.Export.Process

Namespace kCura.WinEDDS

	Public Class PreviewLoadFileProcess
		Inherits ProcessBase

		Public LoadFile As LoadFile
		Protected WithEvents _loadFilePreviewer As LoadFilePreviewer
		Protected WithEvents _IoReporterContext As IoReporterContext
		Private _startTime As System.DateTime
		Private _errorCount As Int32
		Private _errorsOnly As Boolean
		Private _warningCount As Int32
		Private _timeZoneOffset As Int32
		Private _formType As Int32
		Private StartTime As System.DateTime
		Private WithEvents _valueThrower As ValueThrower
		Public ReturnValueCollection As Hashtable
		Public ReturnValueKey As Guid

		Public Sub New(ByVal formType As Int32)
			_formType = formType
		End Sub

		Public Property TimeZoneOffset() As Int32
			Get
				Return _timeZoneOffset
			End Get
			Set(ByVal value As Int32)
				_timeZoneOffset = value
			End Set
		End Property

		Public Property ErrorsOnly() As Boolean
			Get
				Return _errorsOnly
			End Get
			Set(ByVal value As Boolean)
				_errorsOnly = value
			End Set
		End Property

		Public Property Thrower() As ValueThrower
			Get
				Return _valueThrower
			End Get
			Set(ByVal value As ValueThrower)
				_valueThrower = value
			End Set
		End Property

		Protected Overrides Sub OnExecute()
			_startTime = DateTime.Now
			_warningCount = 0
			_errorCount = 0
			_IoReporterContext = New IoReporterContext(Me.FileSystem, Me.AppSettings, New WaitAndRetryPolicy(Me.AppSettings))
			Dim reporter As IIoReporter = Me.CreateIoReporter(_IoReporterContext)
			_loadFilePreviewer = New kCura.WinEDDS.LoadFilePreviewer(
				LoadFile, _
				reporter, _
				logger, _
				_timeZoneOffset, _
				_errorsOnly, _
				True,
				Me.CancellationTokenSource, _
				Me.Context)
			_valueThrower.ThrowValue(New Object() {_loadFilePreviewer.ReadFile(LoadFile.FilePath, _formType), _errorsOnly})
			Me.Context.PublishProcessCompleted(True)
		End Sub

		Private Sub _loadFilePreviewer_OnEvent(ByVal e As LoadFilePreviewer.EventArgs) Handles _loadFilePreviewer.OnEvent
			SyncLock Me.Context
				Dim totaldisplay As String
				Dim processeddisplay As String
				If e.TotalBytes >= 1048576 Then
					totaldisplay = (e.TotalBytes / 1048576).ToString("N0") & " MB"
					processeddisplay = (e.BytesRead / 1048576).ToString("N0") & " MB"
				ElseIf e.TotalBytes < 1048576 AndAlso e.TotalBytes >= 102400 Then
					totaldisplay = (e.TotalBytes / 1024).ToString("N0") & " KB"
					processeddisplay = (e.BytesRead / 1024).ToString("N0") & " KB"
				Else
					totaldisplay = e.TotalBytes.ToString & " B"
					processeddisplay = e.BytesRead.ToString & " B"
				End If
				Select Case e.Type
					Case LoadFilePreviewer.EventType.Begin
						Me.StartTime = System.DateTime.Now
					Case LoadFilePreviewer.EventType.Complete
						If e.TotalBytes = -1 Then
							Me.Context.PublishProgress(e.TotalBytes, e.TotalBytes, 0, 0, Me.StartTime, System.DateTime.Now, 0, 0, Me.ProcessID, "First " & Me.AppSettings.PreviewThreshold & " records", totaldisplay)
						Else
							Me.Context.PublishProgress(e.TotalBytes, e.TotalBytes, 0, 0, Me.StartTime, System.DateTime.Now, 0, 0, Me.ProcessID, totaldisplay, processeddisplay)
						End If
					Case LoadFilePreviewer.EventType.Progress
						Me.Context.PublishProgress(e.TotalBytes, e.BytesRead, 0, 0, Me.StartTime, System.DateTime.Now, 0, 0, Me.ProcessID, totaldisplay, processeddisplay)
						Me.Context.PublishStatusEvent("", "Preparing file for preview")
				End Select
			End SyncLock
		End Sub

		Private Sub _loadFileImporter_IoErrorEvent(ByVal sender As Object, ByVal e As IoWarningEventArgs) Handles _IoReporterContext.IoWarningEvent
			SyncLock Me.Context
				Me.Context.PublishWarningEvent((e.CurrentLineNumber + 1).ToString, e.Message)
			End SyncLock
		End Sub
	End Class
End Namespace