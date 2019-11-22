Imports System.ComponentModel
Imports Relativity.DataExchange
Imports Relativity.DataExchange.Process
Imports Relativity.Logging

Namespace Relativity.Desktop.Client
	Public Class ProcessForm
		Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "

		Public Sub New()
			MyBase.New()

			'This call is required by the Windows Form Designer.
			InitializeComponent()

			'Add any initialization after the InitializeComponent() call
		End Sub

		'Form overrides dispose to clean up the component list.
		Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
			If disposing Then
				If Not (components Is Nothing) Then
					components.Dispose()
				End If
			End If
			MyBase.Dispose(disposing)
		End Sub

		'Required by the Windows Form Designer
		Private components As System.ComponentModel.IContainer

		'NOTE: The following procedure is required by the Windows Form Designer
		'It can be modified using the Windows Form Designer.  
		'Do not modify it using the code editor.
		Friend WithEvents _showDetailButton As System.Windows.Forms.Button
		Friend WithEvents _progressBar As System.Windows.Forms.ProgressBar
		Friend WithEvents _currentRecordLabel As System.Windows.Forms.Label
		Friend WithEvents _overalProgressLabel As System.Windows.Forms.Label
		Friend WithEvents _stopImportButton As System.Windows.Forms.Button
		Friend WithEvents _saveOutputButton As System.Windows.Forms.Button
		Friend WithEvents _saveFileDialog As System.Windows.Forms.SaveFileDialog
		Friend WithEvents ProgressTab As System.Windows.Forms.TabPage
		Friend WithEvents SummaryTab As System.Windows.Forms.TabPage
		Friend WithEvents WarningsTab As System.Windows.Forms.TabPage
		Friend WithEvents ErrorsTab As System.Windows.Forms.TabPage
		Friend WithEvents _outputTextBox As Relativity.Desktop.Client.OutputRichTextBox
		Friend WithEvents _Tabs As System.Windows.Forms.TabControl
		Friend WithEvents _summaryOutput As System.Windows.Forms.TextBox
		Friend WithEvents _currentMessageStatus As System.Windows.Forms.Label
		Friend WithEvents _warningsOutputTextBox As Relativity.Desktop.Client.OutputRichTextBox
		Friend WithEvents _errorsOutputTextBox As Relativity.Desktop.Client.OutputRichTextBox

		Friend WithEvents _statusBar As System.Windows.Forms.Label
		Friend WithEvents ErrorReportTab As System.Windows.Forms.TabPage
		Friend WithEvents _reportDataGrid As System.Windows.Forms.DataGrid
		Friend WithEvents _exportErrorsDialog As System.Windows.Forms.SaveFileDialog
		Friend WithEvents _exportErrorReportBtn As System.Windows.Forms.Button
		Friend WithEvents _exportErrorFileButton As System.Windows.Forms.Button
		Friend WithEvents _exportErrorFileDialog As System.Windows.Forms.SaveFileDialog
		Friend WithEvents _exportErrorFilesTo As System.Windows.Forms.FolderBrowserDialog
		<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
		Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ProcessForm))
		Me._stopImportButton = New System.Windows.Forms.Button()
		Me._showDetailButton = New System.Windows.Forms.Button()
		Me._progressBar = New System.Windows.Forms.ProgressBar()
		Me._currentRecordLabel = New System.Windows.Forms.Label()
		Me._overalProgressLabel = New System.Windows.Forms.Label()
		Me._saveOutputButton = New System.Windows.Forms.Button()
		Me._saveFileDialog = New System.Windows.Forms.SaveFileDialog()
		Me._Tabs = New System.Windows.Forms.TabControl()
		Me.SummaryTab = New System.Windows.Forms.TabPage()
		Me._summaryOutput = New System.Windows.Forms.TextBox()
		Me.ErrorsTab = New System.Windows.Forms.TabPage()
		Me._errorsOutputTextBox = New Relativity.Desktop.Client.OutputRichTextBox()
		Me.ProgressTab = New System.Windows.Forms.TabPage()
		Me._outputTextBox = New Relativity.Desktop.Client.OutputRichTextBox()
		Me.WarningsTab = New System.Windows.Forms.TabPage()
		Me._warningsOutputTextBox = New Relativity.Desktop.Client.OutputRichTextBox()
		Me.ErrorReportTab = New System.Windows.Forms.TabPage()
		Me._exportErrorFileButton = New System.Windows.Forms.Button()
		Me._exportErrorReportBtn = New System.Windows.Forms.Button()
		Me._reportDataGrid = New System.Windows.Forms.DataGrid()
		Me._currentMessageStatus = New System.Windows.Forms.Label()
		Me._statusBar = New System.Windows.Forms.Label()
		Me._exportErrorsDialog = New System.Windows.Forms.SaveFileDialog()
		Me._exportErrorFileDialog = New System.Windows.Forms.SaveFileDialog()
		Me._exportErrorFilesTo = New System.Windows.Forms.FolderBrowserDialog()
		Me._Tabs.SuspendLayout
		Me.SummaryTab.SuspendLayout
		Me.ErrorsTab.SuspendLayout
		Me.ProgressTab.SuspendLayout
		Me.WarningsTab.SuspendLayout
		Me.ErrorReportTab.SuspendLayout
		CType(Me._reportDataGrid,System.ComponentModel.ISupportInitialize).BeginInit
		Me.SuspendLayout
		'
		'_stopImportButton
		'
		Me._stopImportButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
		Me._stopImportButton.Location = New System.Drawing.Point(700, 56)
		Me._stopImportButton.Name = "_stopImportButton"
		Me._stopImportButton.Size = New System.Drawing.Size(84, 23)
		Me._stopImportButton.TabIndex = 0
		Me._stopImportButton.Text = "Stop"
		'
		'_showDetailButton
		'
		Me._showDetailButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
		Me._showDetailButton.Location = New System.Drawing.Point(700, 80)
		Me._showDetailButton.Name = "_showDetailButton"
		Me._showDetailButton.Size = New System.Drawing.Size(84, 23)
		Me._showDetailButton.TabIndex = 1
		Me._showDetailButton.Text = "Hide Detail"
		'
		'_progressBar
		'
		Me._progressBar.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
		Me._progressBar.Location = New System.Drawing.Point(4, 56)
		Me._progressBar.Name = "_progressBar"
		Me._progressBar.Size = New System.Drawing.Size(688, 23)
		Me._progressBar.TabIndex = 2
		'
		'_currentRecordLabel
		'
		Me._currentRecordLabel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
		Me._currentRecordLabel.Location = New System.Drawing.Point(4, 4)
		Me._currentRecordLabel.Name = "_currentRecordLabel"
		Me._currentRecordLabel.Size = New System.Drawing.Size(776, 16)
		Me._currentRecordLabel.TabIndex = 4
		'
		'_overalProgressLabel
		'
		Me._overalProgressLabel.Location = New System.Drawing.Point(4, 84)
		Me._overalProgressLabel.Name = "_overalProgressLabel"
		Me._overalProgressLabel.Size = New System.Drawing.Size(368, 16)
		Me._overalProgressLabel.TabIndex = 5
		'
		'_saveOutputButton
		'
		Me._saveOutputButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
		Me._saveOutputButton.Enabled = false
		Me._saveOutputButton.Location = New System.Drawing.Point(676, 536)
		Me._saveOutputButton.Name = "_saveOutputButton"
		Me._saveOutputButton.Size = New System.Drawing.Size(108, 23)
		Me._saveOutputButton.TabIndex = 10
		Me._saveOutputButton.Text = "Save Progress Log"
		Me._saveOutputButton.Visible = false
		'
		'_saveFileDialog
		'
		Me._saveFileDialog.DefaultExt = "log"
		Me._saveFileDialog.Filter = "XML Log Files (*.log)|*.log"
		'
		'_Tabs
		'
		Me._Tabs.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
            Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
		Me._Tabs.Controls.Add(Me.SummaryTab)
		Me._Tabs.Controls.Add(Me.ErrorsTab)
		Me._Tabs.Controls.Add(Me.ProgressTab)
		Me._Tabs.Controls.Add(Me.WarningsTab)
		Me._Tabs.Controls.Add(Me.ErrorReportTab)
		Me._Tabs.Location = New System.Drawing.Point(4, 104)
		Me._Tabs.Name = "_Tabs"
		Me._Tabs.SelectedIndex = 0
		Me._Tabs.Size = New System.Drawing.Size(780, 432)
		Me._Tabs.TabIndex = 11
		'
		'SummaryTab
		'
		Me.SummaryTab.Controls.Add(Me._summaryOutput)
		Me.SummaryTab.Location = New System.Drawing.Point(4, 22)
		Me.SummaryTab.Name = "SummaryTab"
		Me.SummaryTab.Size = New System.Drawing.Size(772, 406)
		Me.SummaryTab.TabIndex = 1
		Me.SummaryTab.Text = "Summary"
		'
		'_summaryOutput
		'
		Me._summaryOutput.BackColor = System.Drawing.Color.White
		Me._summaryOutput.Dock = System.Windows.Forms.DockStyle.Fill
		Me._summaryOutput.Font = New System.Drawing.Font("Courier New", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
		Me._summaryOutput.Location = New System.Drawing.Point(0, 0)
		Me._summaryOutput.Multiline = true
		Me._summaryOutput.Name = "_summaryOutput"
		Me._summaryOutput.ReadOnly = true
		Me._summaryOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both
		Me._summaryOutput.Size = New System.Drawing.Size(772, 406)
		Me._summaryOutput.TabIndex = 12
		'
		'ErrorsTab
		'
		Me.ErrorsTab.Controls.Add(Me._errorsOutputTextBox)
		Me.ErrorsTab.Location = New System.Drawing.Point(4, 22)
		Me.ErrorsTab.Name = "ErrorsTab"
		Me.ErrorsTab.Size = New System.Drawing.Size(456, 202)
		Me.ErrorsTab.TabIndex = 2
		Me.ErrorsTab.Text = "Errors"
		'
		'_errorsOutputTextBox
		'
		Me._errorsOutputTextBox.AllowForSave = false
		Me._errorsOutputTextBox.Dock = System.Windows.Forms.DockStyle.Fill
		Me._errorsOutputTextBox.InSafeMode = false
		Me._errorsOutputTextBox.Location = New System.Drawing.Point(0, 0)
		Me._errorsOutputTextBox.Name = "_errorsOutputTextBox"
		Me._errorsOutputTextBox.Scrollbars = System.Windows.Forms.ScrollBars.Both
		Me._errorsOutputTextBox.Size = New System.Drawing.Size(456, 202)
		Me._errorsOutputTextBox.TabIndex = 11
		'
		'ProgressTab
		'
		Me.ProgressTab.Controls.Add(Me._outputTextBox)
		Me.ProgressTab.Location = New System.Drawing.Point(4, 22)
		Me.ProgressTab.Name = "ProgressTab"
		Me.ProgressTab.Size = New System.Drawing.Size(456, 202)
		Me.ProgressTab.TabIndex = 0
		Me.ProgressTab.Text = "Progress"
		'
		'_outputTextBox
		'
		Me._outputTextBox.AllowForSave = false
		Me._outputTextBox.Dock = System.Windows.Forms.DockStyle.Fill
		Me._outputTextBox.InSafeMode = false
		Me._outputTextBox.Location = New System.Drawing.Point(0, 0)
		Me._outputTextBox.Name = "_outputTextBox"
		Me._outputTextBox.Scrollbars = System.Windows.Forms.ScrollBars.Both
		Me._outputTextBox.Size = New System.Drawing.Size(456, 202)
		Me._outputTextBox.TabIndex = 10
		'
		'WarningsTab
		'
		Me.WarningsTab.Controls.Add(Me._warningsOutputTextBox)
		Me.WarningsTab.Location = New System.Drawing.Point(4, 22)
		Me.WarningsTab.Name = "WarningsTab"
		Me.WarningsTab.Size = New System.Drawing.Size(456, 202)
		Me.WarningsTab.TabIndex = 3
		Me.WarningsTab.Text = "Warnings"
		'
		'_warningsOutputTextBox
		'
		Me._warningsOutputTextBox.AllowForSave = false
		Me._warningsOutputTextBox.Dock = System.Windows.Forms.DockStyle.Fill
		Me._warningsOutputTextBox.InSafeMode = false
		Me._warningsOutputTextBox.Location = New System.Drawing.Point(0, 0)
		Me._warningsOutputTextBox.Name = "_warningsOutputTextBox"
		Me._warningsOutputTextBox.Scrollbars = System.Windows.Forms.ScrollBars.Both
		Me._warningsOutputTextBox.Size = New System.Drawing.Size(456, 202)
		Me._warningsOutputTextBox.TabIndex = 11
		'
		'ErrorReportTab
		'
		Me.ErrorReportTab.Controls.Add(Me._exportErrorFileButton)
		Me.ErrorReportTab.Controls.Add(Me._exportErrorReportBtn)
		Me.ErrorReportTab.Controls.Add(Me._reportDataGrid)
		Me.ErrorReportTab.Location = New System.Drawing.Point(4, 22)
		Me.ErrorReportTab.Name = "ErrorReportTab"
		Me.ErrorReportTab.Size = New System.Drawing.Size(456, 202)
		Me.ErrorReportTab.TabIndex = 4
		Me.ErrorReportTab.Text = "Report"
		'
		'_exportErrorFileButton
		'
		Me._exportErrorFileButton.Location = New System.Drawing.Point(134, 2)
		Me._exportErrorFileButton.Name = "_exportErrorFileButton"
		Me._exportErrorFileButton.Size = New System.Drawing.Size(114, 20)
		Me._exportErrorFileButton.TabIndex = 3
		Me._exportErrorFileButton.Text = "Export Error File"
		'
		'_exportErrorReportBtn
		'
		Me._exportErrorReportBtn.Location = New System.Drawing.Point(0, 2)
		Me._exportErrorReportBtn.Name = "_exportErrorReportBtn"
		Me._exportErrorReportBtn.Size = New System.Drawing.Size(132, 20)
		Me._exportErrorReportBtn.TabIndex = 2
		Me._exportErrorReportBtn.Text = "Export Error Report"
		'
		'_reportDataGrid
		'
		Me._reportDataGrid.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
            Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
		Me._reportDataGrid.DataMember = ""
		Me._reportDataGrid.HeaderForeColor = System.Drawing.SystemColors.ControlText
		Me._reportDataGrid.Location = New System.Drawing.Point(0, 24)
		Me._reportDataGrid.Name = "_reportDataGrid"
		Me._reportDataGrid.Size = New System.Drawing.Size(480, 176)
		Me._reportDataGrid.TabIndex = 0
		'
		'_currentMessageStatus
		'
		Me._currentMessageStatus.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
		Me._currentMessageStatus.Location = New System.Drawing.Point(4, 24)
		Me._currentMessageStatus.Name = "_currentMessageStatus"
		Me._currentMessageStatus.Size = New System.Drawing.Size(776, 28)
		Me._currentMessageStatus.TabIndex = 12
		'
		'_statusBar
		'
		Me._statusBar.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
		Me._statusBar.Location = New System.Drawing.Point(8, 540)
		Me._statusBar.Name = "_statusBar"
		Me._statusBar.Size = New System.Drawing.Size(352, 20)
		Me._statusBar.TabIndex = 13
		Me._statusBar.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
		'
		'_exportErrorsDialog
		'
		Me._exportErrorsDialog.Filter = "CSV Files|*.csv|All Files|*.*"
		'
		'ProgressForm
		'
		Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
		Me.ClientSize = New System.Drawing.Size(784, 561)
		Me.Controls.Add(Me._statusBar)
		Me.Controls.Add(Me._currentMessageStatus)
		Me.Controls.Add(Me._Tabs)
		Me.Controls.Add(Me._saveOutputButton)
		Me.Controls.Add(Me._overalProgressLabel)
		Me.Controls.Add(Me._currentRecordLabel)
		Me.Controls.Add(Me._progressBar)
		Me.Controls.Add(Me._showDetailButton)
		Me.Controls.Add(Me._stopImportButton)
		Me.Icon = CType(resources.GetObject("$this.Icon"),System.Drawing.Icon)
		Me.Name = "ProgressForm"
		Me.Text = "Import Progress..."
		Me._Tabs.ResumeLayout(false)
		Me.SummaryTab.ResumeLayout(false)
		Me.SummaryTab.PerformLayout
		Me.ErrorsTab.ResumeLayout(false)
		Me.ProgressTab.ResumeLayout(false)
		Me.WarningsTab.ResumeLayout(false)
		Me.ErrorReportTab.ResumeLayout(false)
		CType(Me._reportDataGrid,System.ComponentModel.ISupportInitialize).EndInit
		Me.ResumeLayout(false)

End Sub

#End Region

		Public Sub New(ByRef logger As ILog)
			Me.New()
			Me._logger = logger
		End Sub

		Protected _processId As Guid
		Protected WithEvents _processContext As ProcessContext
		Private ReadOnly _logger As ILog = New NullLogger()
		Private Property FatalException As System.Exception
		Private _inSafeMode As Boolean
		Private _hasReceivedFatalError As Boolean = False
		Private _statusBarPopupText As String = ""
		Private _hasClickedStop As Boolean = False
		Private _summaryString As System.Text.StringBuilder
		Private _cancelled As Boolean = False
		Private Shared _summaryLock As Object = New Object()

		Private Property NumberOfWarnings As Long
		Private Property ShowWarningPopup As Boolean
			Get
				Return (NumberOfWarnings <> 0)
			End Get
			Set(ByVal value As Boolean)
				If Not value Then NumberOfWarnings = 0
			End Set
		End Property

		Public Property ErrorFileExtension() As String = "CSV"
		Public Property ProcessID() As Guid
			Get
				Return _processId
			End Get
			Set(ByVal value As Guid)
				_processId = value
			End Set
		End Property

		Public Property Context As ProcessContext
			Get
				Return _processContext
			End Get
			Set(ByVal value As ProcessContext)
				_processContext = value
			End Set
		End Property

		Public Property InSafeMode() As Boolean
			Get
				Return _inSafeMode
			End Get
			Set(ByVal value As Boolean)
				_inSafeMode = value
				_outputTextBox.InSafeMode = Me.InSafeMode
				_processContext.SafeMode = Me.InSafeMode
			End Set
		End Property

		Private ReadOnly Property SummaryString() As System.Text.StringBuilder
			Get
				If _summaryString Is Nothing Then
					_summaryString = New System.Text.StringBuilder
				End If
				Return _summaryString
			End Get
		End Property

		Private Sub _stopImportButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _stopImportButton.Click
			Try
				If _stopImportButton.Text = "Stop" Then
					_processContext.PublishCancellationRequest(_processId, requestByUser := True)
					_stopImportButton.Enabled = False
					_currentRecordLabel.Text = "Process halting"
					_hasClickedStop = True
				Else
					Me.Close()
				End If
			Catch
			End Try
		End Sub

		Private Sub HideDetail()
			_showDetailButton.Text = "Show Detail"
			_outputTextBox.Visible = False
			_saveOutputButton.Visible = False
			_statusBar.Visible = False
			Me.Height = 155
		End Sub

		Private Sub ShowDetail()
			_showDetailButton.Text = "Hide Detail"
			_outputTextBox.Visible = True
			_outputTextBox.Refresh()
			If Not _inSafeMode Then
				_saveOutputButton.Visible = True
			End If
			_statusBar.Visible = True
			Me.Height = 332
		End Sub

		Public Property StopImportButtonText() As String
			Get
				Return _stopImportButton.Text
			End Get
			Set(ByVal value As String)
				_stopImportButton.Text = value
			End Set
		End Property

		Private Sub _showDetailButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _showDetailButton.Click
			Select Case _showDetailButton.Text
				Case "Hide Detail"
					Me.HideDetail()
				Case "Show Detail"
					Me.ShowDetail()
			End Select
		End Sub

		Private Sub ImportProgressForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
			CheckForIllegalCrossThreadCalls = False
			'Me.HideDetail()
		End Sub

		Private Sub _saveOutputButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _saveOutputButton.Click
			_saveFileDialog.ShowDialog()
		End Sub

		Private Sub _saveFileDialog_FileOk(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles _saveFileDialog.FileOk
			_processContext.SaveOutputFile(_saveFileDialog.FileName)
		End Sub

#Region " Process Observer"

		Private Sub _processContext_OnProcessEvent(ByVal sender As Object, ByVal e As ProcessEventArgs) Handles _processContext.ProcessEvent
			If e.EventType = ProcessEventType.Status AndAlso e.Message.ToLower = "cancel import" Then
				_cancelled = True
			End If

			Try
				If Not _hasClickedStop Then _currentRecordLabel.Text = e.RecordInfo
				_currentMessageStatus.Text = e.Message
				Select Case e.EventType
					Case ProcessEventType.Status
						_outputTextBox.WriteLine(e.Message + " " + e.RecordInfo)
					Case ProcessEventType.Error
						_errorsOutputTextBox.WriteLine(e.Message)
					Case ProcessEventType.Warning
						_warningsOutputTextBox.WriteLine(e.Message)
				End Select
			Catch ex As Win32Exception ' This exception is most likely caused by cross thread calls to UI elements Win32Exception
				'We decided to ignore that exception while working on REL-378780
				_logger.LogError(ex, "Exception occured while handling process event: {@e}", e)
			End Try
		End Sub
		
		Private Function GetTimeSpanString(ByVal ts As System.TimeSpan) As String
			Dim retval As String = ts.ToString
			If retval.IndexOf(".") <> -1 Then
				retval = retval.Substring(0, retval.LastIndexOf("."))
			End If
			Return retval
		End Function
		Private _lastEvent As ProgressEventArgs
		Public Property StatusRefreshRate As Long
		Protected ReadOnly Property DeJitter As Boolean
			Get
				Return StatusRefreshRate > 0
			End Get
		End Property
		Private _lastStatusMessageTs As Long = 0
		Private _timer As System.Windows.Forms.Timer

		Sub _processContext_OnProcessProgressEvent(ByVal sender As Object, ByVal e As ProgressEventArgs) Handles _processContext.Progress
			_lastEvent = e
			Dim redrawSummary As Boolean = True

			If DeJitter Then
				If _timer Is Nothing Then
					_timer = New Timer() With {.Interval = CInt(StatusRefreshRate / 10000)}
					AddHandler _timer.Tick, Sub() BuildOutSummary(Nothing)
				End If
				Dim now As Long = System.DateTime.Now.Ticks
				Dim hasIntervalElapsed As Boolean = (now - _lastStatusMessageTs > StatusRefreshRate)
				If hasIntervalElapsed Then
					_lastStatusMessageTs = now
				Else
					_timer.Stop()
					_timer.Start()
				End If
				redrawSummary = hasIntervalElapsed
			End If
			If redrawSummary Then BuildOutSummary(e)
		End Sub

		Private Sub BuildOutSummary(args As ProgressEventArgs)
			SyncLock (_summaryLock)
				If args Is Nothing Then args = _lastEvent
				Dim stubDate As DateTime
				Dim totalRecords, totalRecordsProcessed As Int32
				If args.TotalRecords > Int32.MaxValue Then
					totalRecordsProcessed = CType((args.TotalProcessedRecords / args.TotalRecords) * Int32.MaxValue, Int32)
					totalRecords = Int32.MaxValue
				Else
					totalRecords = CType(args.TotalRecords, Int32)
					totalRecordsProcessed = CType(args.TotalProcessedRecords, Int32)
				End If

				If totalRecordsProcessed > totalRecords Or totalRecordsProcessed < 0 Then
					Return
				End If

				_progressBar.Minimum = 0
				_progressBar.Maximum = totalRecords
				_progressBar.Value = totalRecordsProcessed
				_overalProgressLabel.Text = args.TotalProcessedRecordsDisplay + " of " + args.TotalRecordsDisplay + " processed"

				NumberOfWarnings = args.TotalProcessedWarningRecords

				'_summaryOutput.Text = ""
				WriteSummaryLine("Start Time: " + args.StartTime.ToLongTimeString)
				If args.Timestamp <> stubDate Then
					WriteSummaryLine("Finish Time: " + args.Timestamp.ToLongTimeString)
					WriteSummaryLine("Duration: " + (Me.GetTimeSpanString(args.Timestamp.Subtract(args.StartTime))))
				Else
					Dim duration As TimeSpan = DateTime.Now.Subtract(args.StartTime)
					WriteSummaryLine("Duration: " + (Me.GetTimeSpanString(duration)))
					WriteSummaryLine("")
					If args.TotalProcessedRecords > 0 Then
						Dim timeToCompletionString As String = Me.GetTimeSpanString(New TimeSpan(CType(duration.Ticks / CType(args.TotalProcessedRecords, Double) * CType((args.TotalRecords - args.TotalProcessedRecords), Double), Long)))
						WriteSummaryLine("Time Left to Completion: " + timeToCompletionString)
					End If
				End If
				WriteSummaryLine("")
				WriteSummaryLine("Total Records: " + args.TotalRecords.ToString)
				WriteSummaryLine("Total Processed: " + args.TotalProcessedRecords.ToString)
				WriteSummaryLine("Total Processed w/Warnings: " + args.TotalProcessedWarningRecords.ToString)
				If args.TotalProcessedErrorRecords > 0 Then
					_summaryOutput.ForeColor = System.Drawing.Color.Red
				End If
				WriteSummaryLine("Total Processed w/Errors: " + args.TotalProcessedErrorRecords .ToString)
				If Not args.Metadata Is Nothing Then
					WriteSummaryLine("")
					For Each title As String In args.Metadata.Keys
						WriteSummaryLine(String.Format("{0}: {1}", title, args.Metadata(title).ToString))
					Next
				End If
				UpdateSummaryText()
			End SyncLock
		End Sub

		Private Sub _processContext_OnProcessComplete(ByVal sender As Object, ByVal e As ProcessCompleteEventArgs) Handles _processContext.ProcessCompleted
			If _lastEvent IsNot Nothing Then BuildOutSummary(_lastEvent)
			If e.CloseForm Then
				Me.Close()
			Else
				If _cancelled Then Me.Close()
				_currentRecordLabel.Text = "All records have been processed"
				If _hasReceivedFatalError Then
					ShowWarningPopup = False
					_currentRecordLabel.Text = "Fatal Exception Encountered"
				End If
				If _hasClickedStop Then _currentRecordLabel.Text = "Process stopped by user"
				_currentMessageStatus.Text = ""
				_stopImportButton.Text = "Close"
				_stopImportButton.Enabled = True
				_saveOutputButton.Enabled = AppSettings.Instance.LogAllEvents
				If e.ExportFilePath <> "" Then
					ShowWarningPopup = False
					_exportErrorFileButton.Visible = True
					If MsgBox("Errors have occurred. Export error files?", MsgBoxStyle.OkCancel, "Relativity Desktop Client") = MsgBoxResult.Ok Then
						If e.ExportLog Then
							Me.ExportErrorFiles()
						Else
							_exportErrorFileDialog.ShowDialog()
						End If
					End If
					_exportErrorFileButton.Visible = True
				ElseIf ShowWarningPopup Then
					MsgBox(String.Format("All records have been processed with {0} warning{1}.", NumberOfWarnings, If(NumberOfWarnings <> 1, "s", "")), MsgBoxStyle.OkOnly, "Relativity Desktop Client")
					ShowWarningPopup = False
				End If
			End If
		End Sub

		Private Sub ExportErrorFiles()
			_exportErrorFilesTo.SelectedPath = System.IO.Directory.GetCurrentDirectory
			_exportErrorFilesTo.ShowDialog()
			Dim folderPath As String = _exportErrorFilesTo.SelectedPath
			If Not folderPath = "" Then
				folderPath = folderPath.TrimEnd("\"c) & "\"
				_processContext.PublishExportServerErrors(folderPath)
			End If
		End Sub

		Private Sub _processContext_OnProcessFatalException(ByVal sender As Object, ByVal e As FatalExceptionEventArgs) Handles _processContext.FatalException
			If _lastEvent IsNot Nothing Then BuildOutSummary(_lastEvent)
			Dim errorFriendlyMessage As String = e.FatalException.Message
			Dim errorFullMessage As String = e.FatalException.ToString
			Me.FatalException = e.FatalException

			If errorFullMessage.ToLower.IndexOf("soapexception") <> -1 Then
				errorFullMessage = System.Web.HttpUtility.HtmlDecode(errorFullMessage)
			End If
			_outputTextBox.WriteLine(errorFullMessage)

			_errorsOutputTextBox.WriteLine(errorFriendlyMessage, " ............")
			_errorsOutputTextBox.WriteErrorDetails()

			If _Tabs.SelectedTab.Equals(ErrorsTab) Then
				_errorsOutputTextBox.PositionDetailsLink()
			End If

			AddHandler _errorsOutputTextBox.DetailsLink.LinkClicked, AddressOf OpenDetailsPane


			_currentRecordLabel.Text = "Fatal Exception Encountered"
			ShowWarningPopup = False
			_hasReceivedFatalError = True
			'_stopImportButton.Text = "Stop"
			_stopImportButton.Text = "Close"
			_saveOutputButton.Enabled = AppSettings.Instance.LogAllEvents
			_summaryOutput.ForeColor = System.Drawing.Color.Red
			Me.ShowDetail()
		End Sub

#End Region

		Private Sub OpenDetailsPane(ByVal sender As Object, ByVal e As LinkLabelLinkClickedEventArgs)
			Dim x As New ErrorDialog With {.Text = "Relativity Desktop Client Error"}
			x.Initialize(Me.FatalException, True)
			x.ShowDialog()
		End Sub

		Private Sub WriteSummaryLine(ByVal what As String)
			SummaryString.Append(what + vbCrLf)
		End Sub

		Private Sub UpdateSummaryText()
			_summaryOutput.Text = _summaryString.ToString
			SummaryString.Remove(0, _summaryString.Length)
		End Sub

		Private Sub _processContext_StatusBarEvent(ByVal sender As Object, ByVal e As StatusBarEventArgs) Handles _processContext.StatusBarChanged
			_statusBar.Text = e.Message
			_statusBarPopupText = e.PopupText
			If _statusBarPopupText Is Nothing Then _statusBarPopupText = ""
			If _statusBarPopupText = "" Then
				_statusBar.ForeColor = System.Drawing.Color.Black
				_statusBar.Cursor = System.Windows.Forms.Cursors.Default
				_statusBar.Font = New System.Drawing.Font(_statusBar.Font, FontStyle.Regular)
			Else
				_statusBar.ForeColor = System.Drawing.Color.Blue
				_statusBar.Cursor = System.Windows.Forms.Cursors.Hand
				_statusBar.Font = New System.Drawing.Font(_statusBar.Font, FontStyle.Underline)
			End If
		End Sub

		'Private Sub _processContext_ShowReportEvent(ByVal datasource As System.Data.DataTable, ByVal maxlengthExceeded As Boolean) Handles _processContext.ShowReportEvent
		'	_errorsDataSource = datasource
		'End Sub

		Private Sub _exportErrorReportBtn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles _exportErrorReportBtn.Click
			_exportErrorsDialog.FileName = ""
			_exportErrorsDialog.Filter = "CSV Files|*.csv|All Files|*.*"
			_exportErrorsDialog.ShowDialog()
			If Not _exportErrorsDialog.FileName Is Nothing AndAlso Not _exportErrorsDialog.FileName = "" Then
				_processContext.PublishExportErrorReport(_exportErrorsDialog.FileName)
			End If
		End Sub

		Private Sub _exportErrorFileButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles _exportErrorFileButton.Click
			_exportErrorsDialog.FileName = ""
			Dim errorFileExtension As String = Me.ErrorFileExtension.ToLower.TrimStart("."c)
			_exportErrorsDialog.Filter = errorFileExtension.ToUpper & " Files|*." & errorFileExtension.ToLower & "|All Files|*.*"
			_exportErrorsDialog.ShowDialog()
			If Not _exportErrorsDialog.FileName Is Nothing AndAlso Not _exportErrorsDialog.FileName = "" Then
				_processContext.PublishExportErrorFile(_exportErrorsDialog.FileName)
			End If
		End Sub

		Private Sub Form_OnFormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
			Dim requestByUser = (e.CloseReason = CloseReason.UserClosing And _stopImportButton.Text = "Stop")
			_processContext.PublishCancellationRequest(_processId, requestByUser)
			_processContext.PublishParentFormClosing(_processId)
		End Sub

		Private Sub _processContext_ErrorReportEvent(ByVal sender As Object, ByVal e As ErrorReportEventArgs) Handles _processContext.ErrorReport
			If DirectCast(_reportDataGrid, System.ComponentModel.ISynchronizeInvoke).InvokeRequired Then
				Me.Invoke(New DoErrorReportFromThread(AddressOf DoErrorReport), New Object() { e.Error })
			Else
				Me.DoErrorReport(e.Error)
			End If
		End Sub

		Public Delegate Sub DoErrorReportFromThread(ByVal row As System.Collections.IDictionary)

		Private Sub DoErrorReport(ByVal row As System.Collections.IDictionary)
			If _reportDataGrid.DataSource Is Nothing Then
				Dim dt As New System.Data.DataTable
				For Each key As String In row.Keys
					dt.Columns.Add(key.ToString, row(key).GetType)
				Next
				_reportDataGrid.DataSource = dt
			End If
			Dim dataSource As System.Data.DataTable = DirectCast(_reportDataGrid.DataSource, System.Data.DataTable)
			Dim newRow As System.Data.DataRow = dataSource.NewRow
			For Each key As String In row.Keys
				If Not dataSource.Columns.Contains(key) Then dataSource.Columns.Add(key, row(key).GetType)
				newRow(key) = row(key)
			Next
			dataSource.Rows.Add(newRow)
		End Sub

		Private Sub _statusBar_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles _statusBar.Click
			If _statusBarPopupText <> "" Then
				Dim frm As New MessageBoxForm
				frm.Show()
				frm._messageBox.Text = _statusBarPopupText
			End If
		End Sub

		Private Sub _observer_ShutdownEvent(ByVal sender As Object, ByVal e As System.EventArgs) Handles _processContext.Shutdown
			Me.Close()
		End Sub

		Private Sub ErrorsTab_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles ErrorsTab.GotFocus
			_errorsOutputTextBox.PositionDetailsLink()
		End Sub
	End Class
End Namespace