Namespace kCura.Windows.Process
	Public Class ProgressForm
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
		Friend WithEvents _outputTextBox As kCura.Windows.Forms.OutputRichTextBox
		Friend WithEvents _Tabs As System.Windows.Forms.TabControl
		Friend WithEvents _summaryOutput As System.Windows.Forms.TextBox
		Friend WithEvents _currentMessageStatus As System.Windows.Forms.Label
		Friend WithEvents _warningsOutputTextBox As kCura.Windows.Forms.OutputRichTextBox
		Friend WithEvents _errorsOutputTextBox As kCura.Windows.Forms.OutputRichTextBox
		Friend WithEvents _statusBar As System.Windows.Forms.Label
		Friend WithEvents ErrorReportTab As System.Windows.Forms.TabPage
		Friend WithEvents _reportDataGrid As System.Windows.Forms.DataGrid
		Friend WithEvents _exportErrorsDialog As System.Windows.Forms.SaveFileDialog
		Friend WithEvents _exportErrorReportBtn As System.Windows.Forms.Button
		Friend WithEvents _exportErrorFileButton As System.Windows.Forms.Button
		Friend WithEvents _exportErrorFileDialog As System.Windows.Forms.SaveFileDialog
		Friend WithEvents _exportErrorFilesTo As System.Windows.Forms.FolderBrowserDialog
		<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
			Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(ProgressForm))
			Me._stopImportButton = New System.Windows.Forms.Button
			Me._showDetailButton = New System.Windows.Forms.Button
			Me._progressBar = New System.Windows.Forms.ProgressBar
			Me._currentRecordLabel = New System.Windows.Forms.Label
			Me._overalProgressLabel = New System.Windows.Forms.Label
			Me._saveOutputButton = New System.Windows.Forms.Button
			Me._saveFileDialog = New System.Windows.Forms.SaveFileDialog
			Me._Tabs = New System.Windows.Forms.TabControl
			Me.SummaryTab = New System.Windows.Forms.TabPage
			Me._summaryOutput = New System.Windows.Forms.TextBox
			Me.ErrorsTab = New System.Windows.Forms.TabPage
			Me._errorsOutputTextBox = New kCura.Windows.Forms.OutputRichTextBox
			Me.ProgressTab = New System.Windows.Forms.TabPage
			Me._outputTextBox = New kCura.Windows.Forms.OutputRichTextBox
			Me.WarningsTab = New System.Windows.Forms.TabPage
			Me._warningsOutputTextBox = New kCura.Windows.Forms.OutputRichTextBox
			Me.ErrorReportTab = New System.Windows.Forms.TabPage
			Me._exportErrorFileButton = New System.Windows.Forms.Button
			Me._exportErrorReportBtn = New System.Windows.Forms.Button
			Me._reportDataGrid = New System.Windows.Forms.DataGrid
			Me._currentMessageStatus = New System.Windows.Forms.Label
			Me._statusBar = New System.Windows.Forms.Label
			Me._exportErrorsDialog = New System.Windows.Forms.SaveFileDialog
			Me._exportErrorFileDialog = New System.Windows.Forms.SaveFileDialog
			Me._exportErrorFilesTo = New System.Windows.Forms.FolderBrowserDialog
			Me._Tabs.SuspendLayout()
			Me.SummaryTab.SuspendLayout()
			Me.ErrorsTab.SuspendLayout()
			Me.ProgressTab.SuspendLayout()
			Me.WarningsTab.SuspendLayout()
			Me.ErrorReportTab.SuspendLayout()
			CType(Me._reportDataGrid, System.ComponentModel.ISupportInitialize).BeginInit()
			Me.SuspendLayout()
			'
			'_stopImportButton
			'
			Me._stopImportButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me._stopImportButton.Location = New System.Drawing.Point(384, 56)
			Me._stopImportButton.Name = "_stopImportButton"
			Me._stopImportButton.Size = New System.Drawing.Size(84, 23)
			Me._stopImportButton.TabIndex = 0
			Me._stopImportButton.Text = "Stop"
			'
			'_showDetailButton
			'
			Me._showDetailButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me._showDetailButton.Location = New System.Drawing.Point(384, 80)
			Me._showDetailButton.Name = "_showDetailButton"
			Me._showDetailButton.Size = New System.Drawing.Size(84, 23)
			Me._showDetailButton.TabIndex = 1
			Me._showDetailButton.Text = "Hide Detail"
			'
			'_progressBar
			'
			Me._progressBar.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
									Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me._progressBar.Location = New System.Drawing.Point(4, 56)
			Me._progressBar.Name = "_progressBar"
			Me._progressBar.Size = New System.Drawing.Size(372, 23)
			Me._progressBar.TabIndex = 2
			'
			'_currentRecordLabel
			'
			Me._currentRecordLabel.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
									Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me._currentRecordLabel.Location = New System.Drawing.Point(4, 4)
			Me._currentRecordLabel.Name = "_currentRecordLabel"
			Me._currentRecordLabel.Size = New System.Drawing.Size(460, 16)
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
			Me._saveOutputButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me._saveOutputButton.Enabled = False
			Me._saveOutputButton.Location = New System.Drawing.Point(360, 332)
			Me._saveOutputButton.Name = "_saveOutputButton"
			Me._saveOutputButton.Size = New System.Drawing.Size(108, 23)
			Me._saveOutputButton.TabIndex = 10
			Me._saveOutputButton.Text = "Save Progress Log"
			Me._saveOutputButton.Visible = False
			'
			'_saveFileDialog
			'
			Me._saveFileDialog.DefaultExt = "log"
			Me._saveFileDialog.Filter = "XML Log Files (*.log)|*.log"
			'
			'_Tabs
			'
			Me._Tabs.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
									Or System.Windows.Forms.AnchorStyles.Left) _
									Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me._Tabs.Controls.Add(Me.SummaryTab)
			Me._Tabs.Controls.Add(Me.ErrorsTab)
			Me._Tabs.Controls.Add(Me.ProgressTab)
			Me._Tabs.Controls.Add(Me.WarningsTab)
			Me._Tabs.Controls.Add(Me.ErrorReportTab)
			Me._Tabs.Location = New System.Drawing.Point(4, 104)
			Me._Tabs.Name = "_Tabs"
			Me._Tabs.SelectedIndex = 0
			Me._Tabs.Size = New System.Drawing.Size(464, 228)
			Me._Tabs.TabIndex = 11
			'
			'SummaryTab
			'
			Me.SummaryTab.Controls.Add(Me._summaryOutput)
			Me.SummaryTab.Location = New System.Drawing.Point(4, 22)
			Me.SummaryTab.Name = "SummaryTab"
			Me.SummaryTab.Size = New System.Drawing.Size(456, 202)
			Me.SummaryTab.TabIndex = 1
			Me.SummaryTab.Text = "Summary"
			'
			'_summaryOutput
			'
			Me._summaryOutput.Dock = System.Windows.Forms.DockStyle.Fill
			Me._summaryOutput.Font = New System.Drawing.Font("Courier New", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
			Me._summaryOutput.Location = New System.Drawing.Point(0, 0)
			Me._summaryOutput.Multiline = True
			Me._summaryOutput.Name = "_summaryOutput"
			Me._summaryOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both
			Me._summaryOutput.Size = New System.Drawing.Size(456, 202)
			Me._summaryOutput.TabIndex = 12
			Me._summaryOutput.Text = ""
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
			Me._errorsOutputTextBox.Dock = System.Windows.Forms.DockStyle.Fill
			Me._errorsOutputTextBox.InSafeMode = False
			Me._errorsOutputTextBox.Location = New System.Drawing.Point(0, 0)
			Me._errorsOutputTextBox.Name = "_errorsOutputTextBox"
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
			Me._outputTextBox.Dock = System.Windows.Forms.DockStyle.Fill
			Me._outputTextBox.InSafeMode = False
			Me._outputTextBox.Location = New System.Drawing.Point(0, 0)
			Me._outputTextBox.Name = "_outputTextBox"
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
			Me._warningsOutputTextBox.Dock = System.Windows.Forms.DockStyle.Fill
			Me._warningsOutputTextBox.InSafeMode = False
			Me._warningsOutputTextBox.Location = New System.Drawing.Point(0, 0)
			Me._warningsOutputTextBox.Name = "_warningsOutputTextBox"
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
			Me._reportDataGrid.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
									Or System.Windows.Forms.AnchorStyles.Left) _
									Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me._reportDataGrid.DataMember = ""
			Me._reportDataGrid.HeaderForeColor = System.Drawing.SystemColors.ControlText
			Me._reportDataGrid.Location = New System.Drawing.Point(0, 24)
			Me._reportDataGrid.Name = "_reportDataGrid"
			Me._reportDataGrid.Size = New System.Drawing.Size(480, 176)
			Me._reportDataGrid.TabIndex = 0
			'
			'_currentMessageStatus
			'
			Me._currentMessageStatus.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
									Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me._currentMessageStatus.Location = New System.Drawing.Point(4, 24)
			Me._currentMessageStatus.Name = "_currentMessageStatus"
			Me._currentMessageStatus.Size = New System.Drawing.Size(460, 28)
			Me._currentMessageStatus.TabIndex = 12
			'
			'_statusBar
			'
			Me._statusBar.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
			Me._statusBar.Location = New System.Drawing.Point(8, 336)
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
			Me.ClientSize = New System.Drawing.Size(468, 357)
			Me.Controls.Add(Me._statusBar)
			Me.Controls.Add(Me._currentMessageStatus)
			Me.Controls.Add(Me._Tabs)
			Me.Controls.Add(Me._saveOutputButton)
			Me.Controls.Add(Me._overalProgressLabel)
			Me.Controls.Add(Me._currentRecordLabel)
			Me.Controls.Add(Me._progressBar)
			Me.Controls.Add(Me._showDetailButton)
			Me.Controls.Add(Me._stopImportButton)
			Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
			Me.Name = "ProgressForm"
			Me.Text = "Import Progress..."
			Me._Tabs.ResumeLayout(False)
			Me.SummaryTab.ResumeLayout(False)
			Me.ErrorsTab.ResumeLayout(False)
			Me.ProgressTab.ResumeLayout(False)
			Me.WarningsTab.ResumeLayout(False)
			Me.ErrorReportTab.ResumeLayout(False)
			CType(Me._reportDataGrid, System.ComponentModel.ISupportInitialize).EndInit()
			Me.ResumeLayout(False)

		End Sub

#End Region

		Protected _processId As Guid
		Protected WithEvents _processObserver As kCura.Windows.Process.ProcessObserver
		Protected WithEvents _controller As kCura.Windows.Process.Controller
		Private _inSafeMode As Boolean
		Private _errorsDataSource As System.Data.DataTable
		Private _exportErrorFileLocation As String = ""
		Private _hasReceivedFatalError As Boolean = False
		Private _hasExportedErrors As Boolean = False
		Private _statusBarPopupText As String = ""
		Private _errorFilesExtension As String = "CSV"
		Private _hasClickedStop As Boolean = False
		Private _summaryString As System.Text.StringBuilder
		Private _cancelled As Boolean = False

		Public Property ErrorFileExtension() As String
			Get
				Return _errorFilesExtension
			End Get
			Set(ByVal value As String)
				_errorFilesExtension = value
			End Set
		End Property
		Public Property ProcessID() As Guid
			Get
				Return _processId
			End Get
			Set(ByVal value As Guid)
				_processId = value
			End Set
		End Property

		Public Property ProcessObserver() As kCura.Windows.Process.ProcessObserver
			Get
				Return _processObserver
			End Get
			Set(ByVal value As kCura.Windows.Process.ProcessObserver)
				_processObserver = value
			End Set
		End Property

		Public Property ProcessController() As kCura.Windows.Process.Controller
			Get
				Return _controller
			End Get
			Set(ByVal value As kCura.Windows.Process.Controller)
				_controller = value
			End Set
		End Property

		Public Property InSafeMode() As Boolean
			Get
				Return _inSafeMode
			End Get
			Set(ByVal value As Boolean)
				_inSafeMode = value
				_outputTextBox.InSafeMode = Me.InSafeMode
				_processObserver.InSafeMode = Me.InSafeMode
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
					ProcessController.HaltProcess(_processId)
					_stopImportButton.Enabled = False
					_currentRecordLabel.Text = "Process halting"
					_hasClickedStop = True
					'_application.CancelImport(_processId)
				Else
					'_application.deletethread(processID)
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
			Me.Height = 135
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
			_processObserver.SaveOutputFile(_saveFileDialog.FileName)
		End Sub

#Region " Process Observer"

		Private Sub _processObserver_OnProcessEvent(ByVal evt As kCura.Windows.Process.ProcessEvent) Handles _processObserver.OnProcessEvent
			If Not _hasClickedStop Then _currentRecordLabel.Text = evt.RecordInfo
			_currentMessageStatus.Text = evt.Message
			Select Case evt.Type
				Case kCura.Windows.Process.ProcessEventTypeEnum.Status
					_outputTextBox.WriteLine(evt.Message + " " + evt.RecordInfo)
					If evt.Message.ToLower = "cancel import" Then _cancelled = True
				Case kCura.Windows.Process.ProcessEventTypeEnum.Error
					_errorsOutputTextBox.WriteLine(evt.Message)
				Case kCura.Windows.Process.ProcessEventTypeEnum.Warning
					_warningsOutputTextBox.WriteLine(evt.Message)
			End Select
		End Sub

		Private Function GetTimeSpanString(ByVal ts As System.TimeSpan) As String
			Dim retval As String = ts.ToString
			If retval.IndexOf(".") <> -1 Then
				retval = retval.Substring(0, retval.LastIndexOf("."))
			End If
			Return retval
		End Function
		Private Sub _processObserver_OnProcessProgressEvent(ByVal evt As kCura.Windows.Process.ProcessProgressEvent) Handles _processObserver.OnProcessProgressEvent

			Dim stubDate As DateTime
			Dim totalRecords, totalRecordsProcessed As Int32
			If evt.TotalRecords > Int32.MaxValue Then
				totalRecordsProcessed = CType((evt.TotalRecordsProcessed / evt.TotalRecords) * Int32.MaxValue, Int32)
				totalRecords = Int32.MaxValue
			Else
				totalRecords = CType(evt.TotalRecords, Int32)
				totalRecordsProcessed = CType(evt.TotalRecordsProcessed, Int32)
			End If
			_progressBar.Minimum = 0
			_progressBar.Maximum = totalRecords
			_progressBar.Value = totalRecordsProcessed
			_overalProgressLabel.Text = evt.TotalRecordsProcessedDisplay + " of " + evt.TotalRecordsDisplay + " processed"

			'_summaryOutput.Text = ""
			WriteSummaryLine("Start Time: " + evt.StartTime.ToLongTimeString)
			If evt.EndTime <> stubDate Then
				WriteSummaryLine("Finish Time: " + evt.EndTime.ToLongTimeString)
				WriteSummaryLine("Duration: " + (Me.GetTimeSpanString(evt.EndTime.Subtract(evt.StartTime))))
			Else
				Dim duration As TimeSpan = DateTime.Now.Subtract(evt.StartTime)
				WriteSummaryLine("Duration: " + (Me.GetTimeSpanString(duration)))
				WriteSummaryLine("")
				If evt.TotalRecordsProcessed > 0 Then
					Dim timeToCompletionString As String = Me.GetTimeSpanString(New TimeSpan(CType(duration.Ticks / CType(evt.TotalRecordsProcessed, Double) * CType((evt.TotalRecords - evt.TotalRecordsProcessed), Double), Long)))
					WriteSummaryLine("Time Left to Completion: " + timeToCompletionString)
				End If
			End If
			WriteSummaryLine("")
			WriteSummaryLine("Total Records: " + evt.TotalRecords.ToString)
			WriteSummaryLine("Total Processed: " + evt.TotalRecordsProcessed.ToString)
			WriteSummaryLine("Total Processed w/Warnings: " + evt.TotalRecordsProcessedWithWarnings.ToString)
			If evt.TotalRecordsProcessedWithErrors > 0 Then
				_summaryOutput.ForeColor = System.Drawing.Color.Red
			End If
			WriteSummaryLine("Total Processed w/Errors: " + evt.TotalRecordsProcessedWithErrors.ToString)
			If Not evt.StatusSuffixEntries Is Nothing Then
				WriteSummaryLine("")
				For Each title As String In evt.StatusSuffixEntries.Keys
					WriteSummaryLine(String.Format("{0}: {1}", title, evt.StatusSuffixEntries(title).ToString))
				Next
			End If
			UpdateSummaryText()
			'_summaryOutput.Refresh()

		End Sub

		Private Sub _processObserver_OnProcessComplete(ByVal closeForm As Boolean, ByVal exportFilePath As String, ByVal exportLog As Boolean) Handles _processObserver.OnProcessComplete
			If closeForm Then
				Me.Close()
			Else
				If _cancelled Then Me.Close()
				_currentRecordLabel.Text = "All records have been processed"
				If _hasReceivedFatalError Then _currentRecordLabel.Text = "Fatal Exception Encountered"
				_currentMessageStatus.Text = ""
				_stopImportButton.Text = "Close"
				_stopImportButton.Enabled = True
				_saveOutputButton.Enabled = Config.LogAllEvents
				If exportFilePath <> "" Then
					_exportErrorFileButton.Visible = True
					Try
						Dim x As New System.Guid(exportFilePath)
						If System.IO.File.Exists(exportFilePath) Then _exportErrorFileLocation = exportFilePath
					Catch
						_exportErrorFileLocation = exportFilePath
					End Try
					If MsgBox("Errors have occurred. Export error files?", MsgBoxStyle.OKCancel, "") = MsgBoxResult.OK Then
						If exportLog Then
							Me.ExportErrorFiles()
						Else
							_exportErrorFileDialog.ShowDialog()
						End If
					End If
					_exportErrorFileButton.Visible = True
				End If
			End If
		End Sub

		Private Sub ExportErrorFiles()
			'_hasExportedErrors = True
			_exportErrorFilesTo.SelectedPath = System.IO.Directory.GetCurrentDirectory
			_exportErrorFilesTo.ShowDialog()
			Dim folderPath As String = _exportErrorFilesTo.SelectedPath
			If Not folderPath = "" Then
				folderPath = folderPath.TrimEnd("\"c) & "\"
				_controller.ExportServerErrors(folderPath)
			End If
			'Dim rootFileName As String = DirectCast(_processObserver.InputArgs, String)
			'Dim defaultExtension As String
			'If Not rootFileName.IndexOf(".") = -1 Then
			'	defaultExtension = rootFileName.Substring(rootFileName.LastIndexOf("."))
			'	rootFileName = rootFileName.Substring(0, rootFileName.LastIndexOf("."))
			'Else
			'	defaultExtension = ".dat"
			'End If
			'rootFileName.Trim("\"c)
			'If rootFileName.IndexOf("\") <> -1 Then
			'	rootFileName = rootFileName.Substring(rootFileName.LastIndexOf("\") + 1)
			'End If

			'Dim rootFilePath As String = folderPath & rootFileName
			'Dim datetimeNow As System.DateTime = System.DateTime.Now
			'Dim datetimenowstring As String = datetimeNow.Year & datetimeNow.Month.ToString.PadLeft(2, "0"c) & datetimeNow.Day.ToString.PadLeft(2, "0"c) & datetimeNow.Hour.ToString.PadLeft(2, "0"c) & datetimeNow.Minute.ToString.PadLeft(2, "0"c) & datetimeNow.Second.ToString.PadLeft(2, "0"c)

			'Dim errorFilePath As String = rootFilePath & "_ErrorLines_" & datetimenowstring & defaultExtension
			'Dim errorReportPath As String = rootFilePath & "_ErrorReport_" & datetimenowstring & ".csv"
			'Try
			'	If System.IO.File.Exists(_exportErrorFileLocation) Then
			'		System.IO.File.Copy(_exportErrorFileLocation, errorFilePath)
			'	End If
			'	_processObserver.ExportErrorReport(errorReportPath)
			'Catch ex As Exception
			'End Try
		End Sub

		Private Sub _processObserver_OnProcessFatalException(ByVal ex As System.Exception) Handles _processObserver.OnProcessFatalException
			Dim errorString As String = ex.ToString
			If errorString.ToLower.IndexOf("soapexception") <> -1 Then
				errorString = System.Web.HttpUtility.HtmlDecode(errorString)
			End If
			_outputTextBox.WriteLine(errorString)
			_errorsOutputTextBox.WriteLine(errorString)
			_currentRecordLabel.Text = "Fatal Exception Encountered"
			_hasReceivedFatalError = True
			'_stopImportButton.Text = "Stop"
			_stopImportButton.Text = "Close"
			_saveOutputButton.Enabled = Config.LogAllEvents
			_summaryOutput.ForeColor = System.Drawing.Color.Red
			Me.ShowDetail()
		End Sub

#End Region

		Private Sub WriteSummaryLine(ByVal what As String)
			SummaryString.Append(what + vbCrLf)
		End Sub

		Private Sub UpdateSummaryText()
			_summaryOutput.Text = _summaryString.ToString
			SummaryString.Remove(0, _summaryString.Length)
		End Sub

		Private Sub _processObserver_StatusBarEvent(ByVal message As String, ByVal popupText As String) Handles _processObserver.StatusBarEvent
			_statusBar.Text = message
			_statusBarPopupText = popupText
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

		Private Sub _processObserver_ShowReportEvent(ByVal datasource As System.Data.DataTable, ByVal maxlengthExceeded As Boolean) Handles _processObserver.ShowReportEvent
			'If maxlengthExceeded AndAlso Not _hasExportedErrors Then
			'	MsgBox("Maximum number of errors has been exceeded." & System.Environment.NewLine & "Export errors to view in entirety.", MsgBoxStyle.Exclamation)
			'End If
			_errorsDataSource = datasource
			Me.Invoke(New HandleDataSourceDelegate(AddressOf HandleDataSource))
		End Sub

		Delegate Sub HandleDataSourceDelegate()
		Private Sub HandleDataSource()
		End Sub

		Private Sub _exportErrorReportBtn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles _exportErrorReportBtn.Click
			_exportErrorsDialog.FileName = ""
			_exportErrorsDialog.Filter = "CSV Files|*.csv|All Files|*.*"
			_exportErrorsDialog.ShowDialog()
			If Not _exportErrorsDialog.FileName Is Nothing AndAlso Not _exportErrorsDialog.FileName = "" Then
				_controller.ExportErrorReport(_exportErrorsDialog.FileName)
			End If
		End Sub

		Private Sub _exportErrorFileButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles _exportErrorFileButton.Click
			_exportErrorsDialog.FileName = ""
			Dim errorFileExtension As String = _errorFilesExtension.ToLower.TrimStart("."c)
			_exportErrorsDialog.Filter = errorFileExtension.ToUpper & " Files|*." & errorFileExtension.ToLower & "|All Files|*.*"
			_exportErrorsDialog.ShowDialog()
			If Not _exportErrorsDialog.FileName Is Nothing AndAlso Not _exportErrorsDialog.FileName = "" Then
				_controller.ExportErrorFile(_exportErrorsDialog.FileName)
			End If
		End Sub

		'Private Sub _exportErrorsDialog_FileOk(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles _exportErrorsDialog.FileOk
		'	_processObserver.ExportErrorReport(_exportErrorsDialog.FileName)
		'End Sub


		'Private Sub _exportErrorFileDialog_FileOk(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles _exportErrorFileDialog.FileOk
		'	If System.IO.File.Exists(_exportErrorFileLocation) Then
		'		System.IO.File.Copy(_exportErrorFileLocation, _exportErrorFileDialog.FileName)
		'	End If
		'End Sub

		Private Sub ProgressForm_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing
			If Not Me.ProcessController Is Nothing Then
				ProcessController.HaltProcess(_processId)
				ProcessController.ParentFormClosing(_processId)
			End If
		End Sub

		Private Sub _processObserver_ErrorReportEvent(ByVal row As System.Collections.IDictionary) Handles _processObserver.ErrorReportEvent
			If DirectCast(_reportDataGrid, System.ComponentModel.ISynchronizeInvoke).InvokeRequired Then
				Me.Invoke(New DoErrorReportFromThread(AddressOf DoErrorReport), New Object() {row})
			Else
				Me.DoErrorReport(row)
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

		Private Sub _observer_ShutdownEvent() Handles _processObserver.ShutdownEvent
			Me.Close()
		End Sub
	End Class


End Namespace