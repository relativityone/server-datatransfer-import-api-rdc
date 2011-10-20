Namespace kCura.EDDS.WinForm
	Public Class ImageLoad
		Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "

		Public Sub New()
			MyBase.New()

			'This call is required by the Windows Form Designer.
			InitializeComponent()

			'Add any initialization after the InitializeComponent() call
			_application = kCura.EDDS.WinForm.Application.Instance
			'_imageLoadFile = New kCura.WinEDDS.ImageLoadFile(kCura.EDDS.WinForm.Application.Instance.Identity)
			_imageLoadFile = New kCura.WinEDDS.ImageLoadFile
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
		Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
		Friend WithEvents _browseButton As System.Windows.Forms.Button
		Friend WithEvents _filePath As System.Windows.Forms.TextBox
		Friend WithEvents MainMenu As System.Windows.Forms.MainMenu
		Friend WithEvents MenuItem1 As System.Windows.Forms.MenuItem
		Friend WithEvents ImportFileMenu As System.Windows.Forms.MenuItem
		Friend WithEvents _openFileDialog As System.Windows.Forms.OpenFileDialog
		Friend WithEvents _importMenuSaveSettingsItem As System.Windows.Forms.MenuItem
		Friend WithEvents _saveImageLoadFileDialog As System.Windows.Forms.SaveFileDialog
		Friend WithEvents _importMenuLoadSettingsItem As System.Windows.Forms.MenuItem
		Friend WithEvents _loadImageLoadFileDialog As System.Windows.Forms.OpenFileDialog
		Friend WithEvents _replaceFullText As System.Windows.Forms.CheckBox
		Friend WithEvents _importMenuCheckErrorsItem As System.Windows.Forms.MenuItem
		Friend WithEvents _overwriteDropdown As System.Windows.Forms.ComboBox
		Friend WithEvents ExtractedTextGroupBox As System.Windows.Forms.GroupBox
		Friend WithEvents _productionDropdown As System.Windows.Forms.ComboBox
		Friend WithEvents _advancedButton As System.Windows.Forms.Button
		Friend WithEvents _beginBatesDropdown As System.Windows.Forms.ComboBox
		Friend WithEvents _startLineNumberLabel As System.Windows.Forms.Label
		Friend WithEvents _startLineNumber As System.Windows.Forms.NumericUpDown
		Friend WithEvents I As System.Windows.Forms.MenuItem
		Friend WithEvents MenuItem3 As System.Windows.Forms.MenuItem
		Friend WithEvents _encodingPicker As kCura.EDDS.WinForm.EncodingPicker
		Friend WithEvents GroupBox5 As System.Windows.Forms.GroupBox
		Friend WithEvents _autoNumberingOn As System.Windows.Forms.RadioButton
		Friend WithEvents _autoNumberingOff As System.Windows.Forms.RadioButton
		Friend WithEvents GroupBox6 As System.Windows.Forms.GroupBox
		Friend WithEvents Label2 As System.Windows.Forms.Label
		Friend WithEvents Label1 As System.Windows.Forms.Label
		Friend WithEvents _productionLabel As System.Windows.Forms.Label
		Friend WithEvents Label9 As System.Windows.Forms.Label
		Friend WithEvents _importMenuSendEmailNotificationItem As System.Windows.Forms.MenuItem
		<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
			Me.components = New System.ComponentModel.Container
			Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ImageLoad))
			Me.GroupBox3 = New System.Windows.Forms.GroupBox
			Me._browseButton = New System.Windows.Forms.Button
			Me._filePath = New System.Windows.Forms.TextBox
			Me._startLineNumberLabel = New System.Windows.Forms.Label
			Me._startLineNumber = New System.Windows.Forms.NumericUpDown
			Me._openFileDialog = New System.Windows.Forms.OpenFileDialog
			Me._overwriteDropdown = New System.Windows.Forms.ComboBox
			Me._replaceFullText = New System.Windows.Forms.CheckBox
			Me.MainMenu = New System.Windows.Forms.MainMenu(Me.components)
			Me.MenuItem1 = New System.Windows.Forms.MenuItem
			Me._importMenuCheckErrorsItem = New System.Windows.Forms.MenuItem
			Me.ImportFileMenu = New System.Windows.Forms.MenuItem
			Me.MenuItem3 = New System.Windows.Forms.MenuItem
			Me._importMenuSendEmailNotificationItem = New System.Windows.Forms.MenuItem
			Me.I = New System.Windows.Forms.MenuItem
			Me._importMenuSaveSettingsItem = New System.Windows.Forms.MenuItem
			Me._importMenuLoadSettingsItem = New System.Windows.Forms.MenuItem
			Me._saveImageLoadFileDialog = New System.Windows.Forms.SaveFileDialog
			Me._loadImageLoadFileDialog = New System.Windows.Forms.OpenFileDialog
			Me.ExtractedTextGroupBox = New System.Windows.Forms.GroupBox
			Me._encodingPicker = New kCura.EDDS.WinForm.EncodingPicker
			Me._productionDropdown = New System.Windows.Forms.ComboBox
			Me._advancedButton = New System.Windows.Forms.Button
			Me._beginBatesDropdown = New System.Windows.Forms.ComboBox
			Me.GroupBox5 = New System.Windows.Forms.GroupBox
			Me._autoNumberingOn = New System.Windows.Forms.RadioButton
			Me._autoNumberingOff = New System.Windows.Forms.RadioButton
			Me.GroupBox6 = New System.Windows.Forms.GroupBox
			Me._productionLabel = New System.Windows.Forms.Label
			Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
			Me.Label2 = New System.Windows.Forms.Label
			Me.Label1 = New System.Windows.Forms.Label
			Me.Label9 = New System.Windows.Forms.Label
			Me.GroupBox3.SuspendLayout()
			CType(Me._startLineNumber, System.ComponentModel.ISupportInitialize).BeginInit()
			Me.ExtractedTextGroupBox.SuspendLayout()
			Me.GroupBox5.SuspendLayout()
			Me.GroupBox6.SuspendLayout()
			Me.SuspendLayout()

			'
			'GroupBox3
			'
			Me.GroupBox3.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
			  Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me.GroupBox3.Controls.Add(Me._browseButton)
			Me.GroupBox3.Controls.Add(Me._filePath)
			Me.GroupBox3.Controls.Add(Me._startLineNumberLabel)
			Me.GroupBox3.Controls.Add(Me._startLineNumber)
			Me.GroupBox3.Location = New System.Drawing.Point(7, 4)
			Me.GroupBox3.Name = "GroupBox3"
			Me.GroupBox3.Size = New System.Drawing.Size(556, 82)
			Me.GroupBox3.TabIndex = 7
			Me.GroupBox3.TabStop = False
			Me.GroupBox3.Text = "Load File"
			'
			'_browseButton
			'
			Me._browseButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me._browseButton.Location = New System.Drawing.Point(521, 19)
			Me._browseButton.Name = "_browseButton"
			Me._browseButton.Size = New System.Drawing.Size(24, 20)
			Me._browseButton.TabIndex = 6
			Me._browseButton.Text = "..."
			'
			'_filePath
			'
			Me._filePath.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
			 Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me._filePath.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
			Me._filePath.Location = New System.Drawing.Point(8, 20)
			Me._filePath.Name = "_filePath"
			Me._filePath.Size = New System.Drawing.Size(507, 20)
			Me._filePath.TabIndex = 5
			Me._filePath.Text = "Select a file ..."
			'
			'_startLineNumberLabel
			'
			Me._startLineNumberLabel.Location = New System.Drawing.Point(6, 51)
			Me._startLineNumberLabel.Name = "_startLineNumberLabel"
			Me._startLineNumberLabel.Size = New System.Drawing.Size(56, 20)
			Me._startLineNumberLabel.TabIndex = 31
			Me._startLineNumberLabel.Text = "Start Line"
			Me._startLineNumberLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
			'
			'_startLineNumber
			'
			Me._startLineNumber.Location = New System.Drawing.Point(62, 51)
			Me._startLineNumber.Maximum = New Decimal(New Integer() {268435455, 1042612833, 542101086, 0})
			Me._startLineNumber.Name = "_startLineNumber"
			Me._startLineNumber.Size = New System.Drawing.Size(144, 20)
			Me._startLineNumber.TabIndex = 30
			'
			'_openFileDialog
			'
			Me._openFileDialog.Filter = "Opticon Files (*.opt)|*.opt|Log Files (*.log)|*.log|Text Files (*.txt)|*.txt|All " & _
			  "files (*.*)|*.*"
			'
			'_overwriteDropdown
			'
			Me._overwriteDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
			Me._overwriteDropdown.Items.AddRange(New Object() {"Append Only", "Overlay Only", "Append/Overlay"})
			Me._overwriteDropdown.Location = New System.Drawing.Point(9, 42)
			Me._overwriteDropdown.Name = "_overwriteDropdown"
			Me._overwriteDropdown.Size = New System.Drawing.Size(252, 21)
			Me._overwriteDropdown.TabIndex = 29
			'
			'_replaceFullText
			'
			Me._replaceFullText.Location = New System.Drawing.Point(12, 20)
			Me._replaceFullText.Name = "_replaceFullText"
			Me._replaceFullText.Size = New System.Drawing.Size(144, 24)
			Me._replaceFullText.TabIndex = 3
			Me._replaceFullText.Text = "Load Extracted Text"
			'
			'MainMenu
			'
			Me.MainMenu.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.MenuItem1})
			'
			'MenuItem1
			'
			Me.MenuItem1.Index = 0
			Me.MenuItem1.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me._importMenuCheckErrorsItem, Me.ImportFileMenu, Me.MenuItem3, Me._importMenuSendEmailNotificationItem, Me.I, Me._importMenuSaveSettingsItem, Me._importMenuLoadSettingsItem})
			Me.MenuItem1.Text = "&Import"
			'
			'_importMenuCheckErrorsItem
			'
			Me._importMenuCheckErrorsItem.Index = 0
			Me._importMenuCheckErrorsItem.Shortcut = System.Windows.Forms.Shortcut.F6
			Me._importMenuCheckErrorsItem.Text = "Check Errors..."
			'
			'ImportFileMenu
			'
			Me.ImportFileMenu.Index = 1
			Me.ImportFileMenu.Shortcut = System.Windows.Forms.Shortcut.F5
			Me.ImportFileMenu.Text = "&Import File..."
			'
			'MenuItem3
			'
			Me.MenuItem3.Index = 2
			Me.MenuItem3.Text = "-"
			'
			'_importMenuSendEmailNotificationItem
			'
			Me._importMenuSendEmailNotificationItem.Index = 3
			Me._importMenuSendEmailNotificationItem.Text = "Send email notification on completion"
			'
			'I
			'
			Me.I.Index = 4
			Me.I.Text = "-"
			'
			'_importMenuSaveSettingsItem
			'
			Me._importMenuSaveSettingsItem.Index = 5
			Me._importMenuSaveSettingsItem.Shortcut = System.Windows.Forms.Shortcut.CtrlS
			Me._importMenuSaveSettingsItem.Text = "Save Settings"
			'
			'_importMenuLoadSettingsItem
			'
			Me._importMenuLoadSettingsItem.Index = 6
			Me._importMenuLoadSettingsItem.Shortcut = System.Windows.Forms.Shortcut.CtrlO
			Me._importMenuLoadSettingsItem.Text = "Load Settings"
			'
			'_saveImageLoadFileDialog
			'
			Me._saveImageLoadFileDialog.DefaultExt = "kwe"
			Me._saveImageLoadFileDialog.Filter = "WinEDDS image load files (*.kwi)|*.kwi|All Files (*.*)|*.*"
			'
			'_loadImageLoadFileDialog
			'
			Me._loadImageLoadFileDialog.Filter = "WinEDDS image load files (*.kwi)|*.kwi|All Files (*.*)|*.*"
			'
			'ExtractedTextGroupBox
			'
			Me.ExtractedTextGroupBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
			 Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me.ExtractedTextGroupBox.Controls.Add(Me.Label9)
			Me.ExtractedTextGroupBox.Controls.Add(Me._encodingPicker)
			Me.ExtractedTextGroupBox.Controls.Add(Me._replaceFullText)
			Me.ExtractedTextGroupBox.Location = New System.Drawing.Point(7, 300)
			Me.ExtractedTextGroupBox.Name = "ExtractedTextGroupBox"
			Me.ExtractedTextGroupBox.Size = New System.Drawing.Size(556, 78)
			Me.ExtractedTextGroupBox.TabIndex = 9
			Me.ExtractedTextGroupBox.TabStop = False
			Me.ExtractedTextGroupBox.Text = "ExtractedText"
			'
			'_encodingPicker
			'
			Me._encodingPicker.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
			 Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me._encodingPicker.Enabled = False
			Me._encodingPicker.Location = New System.Drawing.Point(293, 39)
			Me._encodingPicker.Name = "_encodingPicker"
			Me._encodingPicker.SelectedEncoding = Nothing
			Me._encodingPicker.Size = New System.Drawing.Size(252, 21)
			Me._encodingPicker.TabIndex = 0
			'
			'_productionDropdown
			'
			Me._productionDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
			Me._productionDropdown.Location = New System.Drawing.Point(9, 87)
			Me._productionDropdown.Name = "_productionDropdown"
			Me._productionDropdown.Size = New System.Drawing.Size(252, 21)
			Me._productionDropdown.TabIndex = 29
			'
			'_advancedButton
			'
			Me._advancedButton.Location = New System.Drawing.Point(293, 87)
			Me._advancedButton.Name = "_advancedButton"
			Me._advancedButton.Size = New System.Drawing.Size(75, 21)
			Me._advancedButton.TabIndex = 28
			Me._advancedButton.Text = "Repository"
			'
			'_beginBatesDropdown
			'
			Me._beginBatesDropdown.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
			 Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me._beginBatesDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
			Me._beginBatesDropdown.Location = New System.Drawing.Point(293, 42)
			Me._beginBatesDropdown.Name = "_beginBatesDropdown"
			Me._beginBatesDropdown.Size = New System.Drawing.Size(252, 21)
			Me._beginBatesDropdown.TabIndex = 29
			'
			'GroupBox5
			'
			Me.GroupBox5.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
			 Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me.GroupBox5.Controls.Add(Me._autoNumberingOn)
			Me.GroupBox5.Controls.Add(Me._autoNumberingOff)
			Me.GroupBox5.Location = New System.Drawing.Point(7, 92)
			Me.GroupBox5.Name = "GroupBox5"
			Me.GroupBox5.Size = New System.Drawing.Size(556, 73)
			Me.GroupBox5.TabIndex = 35
			Me.GroupBox5.TabStop = False
			Me.GroupBox5.Text = "Numbering"
			'
			'_autoNumberingOn
			'
			Me._autoNumberingOn.AutoSize = True
			Me._autoNumberingOn.Location = New System.Drawing.Point(12, 43)
			Me._autoNumberingOn.Name = "_autoNumberingOn"
			Me._autoNumberingOn.Size = New System.Drawing.Size(117, 17)
			Me._autoNumberingOn.TabIndex = 1
			Me._autoNumberingOn.Text = "Auto-number pages"
			Me._autoNumberingOn.UseVisualStyleBackColor = True
			'
			'_autoNumberingOff
			'
			Me._autoNumberingOff.AutoSize = True
			Me._autoNumberingOff.Checked = True
			Me._autoNumberingOff.Location = New System.Drawing.Point(12, 20)
			Me._autoNumberingOff.Name = "_autoNumberingOff"
			Me._autoNumberingOff.Size = New System.Drawing.Size(129, 17)
			Me._autoNumberingOff.TabIndex = 0
			Me._autoNumberingOff.TabStop = True
			Me._autoNumberingOff.Text = "Use load file page IDs"
			Me._autoNumberingOff.UseVisualStyleBackColor = True
			'
			'GroupBox6
			'
			Me.GroupBox6.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
			  Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me.GroupBox6.Controls.Add(Me._productionLabel)
			Me.GroupBox6.Controls.Add(Me._productionDropdown)
			Me.GroupBox6.Controls.Add(Me.Label2)
			Me.GroupBox6.Controls.Add(Me.Label1)
			Me.GroupBox6.Controls.Add(Me._beginBatesDropdown)
			Me.GroupBox6.Controls.Add(Me._advancedButton)
			Me.GroupBox6.Controls.Add(Me._overwriteDropdown)
			Me.GroupBox6.Location = New System.Drawing.Point(7, 171)
			Me.GroupBox6.Name = "GroupBox6"
			Me.GroupBox6.Size = New System.Drawing.Size(556, 123)
			Me.GroupBox6.TabIndex = 36
			Me.GroupBox6.TabStop = False
			Me.GroupBox6.Text = "Import Mode"
			'
			'_productionLabel
			'
			Me._productionLabel.AutoSize = True
			Me._productionLabel.Location = New System.Drawing.Point(5, 70)
			Me._productionLabel.Name = "_productionLabel"
			Me._productionLabel.Size = New System.Drawing.Size(58, 13)
			Me._productionLabel.TabIndex = 32
			Me._productionLabel.Text = "Production"
			'
			'Label2
			'
			Me.Label2.AutoSize = True
			Me.Label2.Location = New System.Drawing.Point(290, 23)
			Me.Label2.Name = "Label2"
			Me.Label2.Size = New System.Drawing.Size(86, 13)
			Me.Label2.TabIndex = 31
			Me.Label2.Text = "Overlay Identifier"
			'
			'Label1
			'
			Me.Label1.AutoSize = True
			Me.Label1.Location = New System.Drawing.Point(5, 23)
			Me.Label1.Name = "Label1"
			Me.Label1.Size = New System.Drawing.Size(67, 13)
			Me.Label1.TabIndex = 30
			Me.Label1.Text = "Select Mode"
			'
			'Label9
			'
			Me.Label9.Location = New System.Drawing.Point(290, 20)
			Me.Label9.Name = "Label9"
			Me.Label9.Size = New System.Drawing.Size(204, 16)
			Me.Label9.TabIndex = 4
			Me.Label9.Text = "Encoding for undetectable files"
			'
			'ImageLoad
			'
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
			Me.ClientSize = New System.Drawing.Size(571, 405)
			Me.MinimumSize = New System.Drawing.Size(579, 455)
			Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show
			Me.Controls.Add(Me.GroupBox6)
			Me.Controls.Add(Me.GroupBox5)
			Me.Controls.Add(Me.ExtractedTextGroupBox)
			Me.Controls.Add(Me.GroupBox3)
			Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
			Me.MaximizeBox = False
			Me.Menu = Me.MainMenu
			Me.Name = "ImageLoad"
			Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
			Me.Text = "Relativity Desktop Client | Import Image Load File"
			Me.GroupBox3.ResumeLayout(False)
			Me.GroupBox3.PerformLayout()
			CType(Me._startLineNumber, System.ComponentModel.ISupportInitialize).EndInit()
			Me.ExtractedTextGroupBox.ResumeLayout(False)
			Me.GroupBox5.ResumeLayout(False)
			Me.GroupBox5.PerformLayout()
			Me.GroupBox6.ResumeLayout(False)
			Me.GroupBox6.PerformLayout()
			Me.ResumeLayout(False)

		End Sub

#End Region

		Private WithEvents _application As kCura.EDDS.WinForm.Application
		Private WithEvents _advancedFileForm As AdvancedFileLocation

		Private _imageLoadFile As kCura.WinEDDS.ImageLoadFile
		Private _identifierFieldArtifactID As Int32

		Friend Property ImageLoadFile() As kCura.WinEDDS.ImageLoadFile
			Get
				Return _imageLoadFile
			End Get
			Set(ByVal value As kCura.WinEDDS.ImageLoadFile)
				_imageLoadFile = value
			End Set
		End Property

		Private Function PopulateImageLoadFile(ByVal doFormValidation As Boolean) As Boolean
			Me.Cursor = Cursors.WaitCursor
			If Not Me.EnsureConnection() Then
				Me.Cursor = Cursors.Default
				Return False
			End If
			Dim rtr As Boolean = False
			If doFormValidation Then
				Dim msg As New System.Text.StringBuilder
				_imageLoadFile.FileName = _filePath.Text
				If _imageLoadFile.FileName Is Nothing OrElse _imageLoadFile.FileName.Trim = "" OrElse _imageLoadFile.FileName.Trim.ToLower = "select a file ..." Then
					msg.Append(" - No opticon file selected").Append(vbNewLine)
				ElseIf Not (System.IO.File.Exists(_imageLoadFile.FileName)) Then
					msg.Append(" - Selected opticon file does not exist").Append(vbNewLine)
				End If
				If _encodingPicker.Enabled Then
					Me.ImageLoadFile.FullTextEncoding = _encodingPicker.SelectedEncoding
					If Me.ImageLoadFile.FullTextEncoding Is Nothing Then
						msg.Append(" - No Encoding selected")
					End If
				End If
				If ImageLoadFile.ForProduction Then
					If CType(_productionDropdown.SelectedValue, Int32) = 0 Then msg.Append(" - No destination production selected").Append(vbNewLine)
					If CType(_beginBatesDropdown.SelectedValue, Int32) = 0 Then msg.Append(" - No import key field selected").Append(vbNewLine)
				End If
				If msg.ToString.Trim <> String.Empty Then
					msg.Insert(0, "The following issues need to be addressed before continuing:" & vbNewLine & vbNewLine)
					MsgBox(msg.ToString, MsgBoxStyle.Exclamation, "Warning")
					Me.Cursor = Cursors.Default
					Return False
				End If
			End If
			ImageLoadFile.FullTextEncoding = _encodingPicker.SelectedEncoding
			ImageLoadFile.Overwrite = Me.GetOverwrite
			ImageLoadFile.DestinationFolderID = _imageLoadFile.DestinationFolderID
			ImageLoadFile.ControlKeyField = _application.GetCaseIdentifierFields(10)(0)
			ImageLoadFile.AutoNumberImages = _autoNumberingOn.Checked
			If ImageLoadFile.ForProduction Then
				ImageLoadFile.ProductionArtifactID = CType(_productionDropdown.SelectedValue, Int32)
				Me.ImageLoadFile.ReplaceFullText = False
				Me.ImageLoadFile.BeginBatesFieldArtifactID = CType(_beginBatesDropdown.SelectedValue, Int32)
			Else
				If Me.GetOverwrite = "Strict" Then
					Me.ImageLoadFile.IdentityFieldId = CType(_beginBatesDropdown.SelectedValue, Int32)
				Else
					Me.ImageLoadFile.IdentityFieldId = -1
				End If
				Me.ImageLoadFile.ReplaceFullText = _replaceFullText.Checked
			End If
			If Me.ImageLoadFile.IdentityFieldId = -1 Then Me.ImageLoadFile.IdentityFieldId = _application.CurrentFields(10).IdentifierFields(0).FieldID
			Me.ImageLoadFile.CaseDefaultPath = _application.SelectedCaseInfo.DocumentPath
			Me.ImageLoadFile.SendEmailOnLoadCompletion = _importMenuSendEmailNotificationItem.Checked
			ImageLoadFile.StartLineNumber = CType(_startLineNumber.Value, Int64)
			Me.Cursor = Cursors.Default
			Return True
		End Function

		Private Function GetOverwrite() As String
			Select Case _overwriteDropdown.SelectedItem.ToString.ToLower
				Case "append only"
					Return "None"
				Case "overlay only"
					Return "Strict"
				Case "append/overlay"
					Return "Append"
				Case Else
					Throw New IndexOutOfRangeException("'" & _overwriteDropdown.SelectedItem.ToString.ToLower & "' isn't a valid option.")
			End Select
		End Function

		Private Function GetOverwriteDropdownItem(ByVal input As String) As String
			Select Case input.ToLower
				Case "none"
					Return "Append Only"
				Case "strict"
					Return "Overlay Only"
				Case "append"
					Return "Append/Overlay"
				Case Else
					Throw New IndexOutOfRangeException("'" & input.ToLower & "' isn't a valid option.")
			End Select
		End Function

		Private Sub ImageLoad_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
			Me.Cursor = Cursors.WaitCursor
			If Not Me.EnsureConnection() Then
				Me.Cursor = Cursors.Default
				Exit Sub
			End If
			Dim dt As System.Data.DataTable = New kCura.WinEDDS.Service.FieldQuery(_application.Credential, _application.CookieContainer).RetrievePotentialBeginBatesFields(ImageLoadFile.CaseInfo.ArtifactID).Tables(0)
			For Each identifierRow As System.Data.DataRow In dt.Rows
				If CType(identifierRow("FieldCategoryID"), Relativity.FieldCategory) = Relativity.FieldCategory.Identifier Then
					_identifierFieldArtifactID = CType(identifierRow("ArtifactID"), Int32)
				End If
			Next
			'Dim row As System.Data.DataRow = dt.NewRow
			'row("ArtifactID") = -1
			'row("DisplayName") = "Select..."
			'dt.Rows.InsertAt(row, 0)
			_beginBatesDropdown.DataSource = dt
			_beginBatesDropdown.DisplayMember = "DisplayName"
			_beginBatesDropdown.ValueMember = "ArtifactID"
			_beginBatesDropdown.SelectedValue = _identifierFieldArtifactID

			If Not ImageLoadFile.ForProduction Then
				_productionDropdown.Visible = False
				_productionLabel.Visible = False
			Else
				_replaceFullText.Checked = False
				_replaceFullText.Enabled = False
				_productionDropdown.DataSource = ImageLoadFile.ProductionTable
				_productionDropdown.DisplayMember = "Name"
				_productionDropdown.ValueMember = "ArtifactID"
				_overwriteDropdown.SelectedIndex = 1
				Me.Text = "Relativity Desktop Client | Import Production Load File"
			End If
			If Not _application.SendLoadNotificationEmailEnabled Then
				_importMenuSendEmailNotificationItem.Visible = False
				_MenuItem3.Visible = False
			Else
				_importMenuSendEmailNotificationItem.Checked = ImageLoadFile.SendEmailOnLoadCompletion
			End If
			_overwriteDropdown.SelectedItem = Me.GetOverwriteDropdownItem(ImageLoadFile.Overwrite)
			Me.Cursor = Cursors.Default
		End Sub

		Private Sub ImportFileMenu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ImportFileMenu.Click
			Me.Cursor = Cursors.WaitCursor
			If PopulateImageLoadFile(True) Then
				If _application.ReadyToLoad(Me.ImageLoadFile, False) Then _application.ImportImageFile(_imageLoadFile)
			End If
			Me.Cursor = Cursors.Default
		End Sub

		Private Sub _browseButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _browseButton.Click
			Me.Cursor = Cursors.WaitCursor
			_openFileDialog.ShowDialog()
			Me.Cursor = Cursors.Default
		End Sub

		Private Sub _openFileDialog_FileOk(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles _openFileDialog.FileOk
			Me.Cursor = Cursors.WaitCursor
			If Not Me.EnsureConnection() Then
				Me.Cursor = Cursors.Default
				Exit Sub
			End If
			_imageLoadFile.FileName = _openFileDialog.FileName
			_filePath.Text = _openFileDialog.FileName
			Me.Cursor = Cursors.Default
		End Sub

		Private Sub _overWrite_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _overwriteDropdown.SelectedIndexChanged
			_imageLoadFile.Overwrite = Me.GetOverwrite
			If _overwriteDropdown.SelectedIndex = 1 Then
				_beginBatesDropdown.Enabled = True
			Else
				_beginBatesDropdown.SelectedValue = _identifierFieldArtifactID
				_beginBatesDropdown.Enabled = False
			End If
		End Sub

		Private Sub _importMenuSaveSettingsItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles _importMenuSaveSettingsItem.Click
			If Not Me.EnsureConnection() Then
				Me.Cursor = Cursors.Default
				Exit Sub
			End If
			PopulateImageLoadFile(False)
			_saveImageLoadFileDialog.ShowDialog()
		End Sub

		Private Sub _saveImageLoadFileDialog_FileOk(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles _saveImageLoadFileDialog.FileOk
			Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
			If Not Me.EnsureConnection() Then
				Me.Cursor = Cursors.Default
				Exit Sub
			End If
			If Not System.IO.File.Exists(_saveImageLoadFileDialog.FileName) Then
				System.IO.File.Create(_saveImageLoadFileDialog.FileName).Close()
			End If
			_application.SaveImageLoadFile(Me.ImageLoadFile, _saveImageLoadFileDialog.FileName)
			Me.Cursor = System.Windows.Forms.Cursors.Default
		End Sub

		Private Sub _importMenuLoadSettingsItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles _importMenuLoadSettingsItem.Click
			_loadImageLoadFileDialog.ShowDialog()
		End Sub

		Private Sub _loadImageLoadFileDialog_FileOk(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles _loadImageLoadFileDialog.FileOk
			Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
			If Not Me.EnsureConnection() Then
				Me.Cursor = Cursors.Default
				Exit Sub
			End If
			If System.IO.File.Exists(_loadImageLoadFileDialog.FileName) Then
				Dim currentFolder As Int32 = Me.ImageLoadFile.DestinationFolderID
				Dim copyFilesToRepository As Boolean = Me.ImageLoadFile.CopyFilesToDocumentRepository
				Me.ImageLoadFile = _application.ReadImageLoadFile(_loadImageLoadFileDialog.FileName)
				Me.ImageLoadFile.CopyFilesToDocumentRepository = copyFilesToRepository
				_encodingPicker.SelectedEncoding = Me.ImageLoadFile.FullTextEncoding
				_overwriteDropdown.SelectedItem = Me.GetOverwriteDropdownItem(ImageLoadFile.Overwrite)
				_filePath.Text = ImageLoadFile.FileName
				_replaceFullText.Checked = ImageLoadFile.ReplaceFullText
				_autoNumberingOn.Checked = ImageLoadFile.AutoNumberImages
				_autoNumberingOff.Checked = Not _autoNumberingOn.Checked
				_importMenuSendEmailNotificationItem.Checked = Me.ImageLoadFile.SendEmailOnLoadCompletion
				Me.ImageLoadFile.DestinationFolderID = currentFolder
				_startLineNumber.Value = CType(ImageLoadFile.StartLineNumber, Decimal)
			End If
			Me.Cursor = System.Windows.Forms.Cursors.Default
		End Sub

		Private Function EnsureConnection() As Boolean
			If Not _imageLoadFile Is Nothing AndAlso Not _imageLoadFile.CaseInfo Is Nothing Then
				Dim casefields As String() = Nothing
				Dim [continue] As Boolean = True
				Try
					casefields = _application.GetCaseFields(_imageLoadFile.CaseInfo.ArtifactID, 10, True)
					Return Not casefields Is Nothing
				Catch ex As System.Exception
					If ex.Message.IndexOf("Need To Re Login") <> -1 Then
						Return False
					Else
						Throw
					End If
				End Try
			Else
				Return True
			End If
		End Function

		Private Sub _importMenuCheckErrorsItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _importMenuCheckErrorsItem.Click
			If Me.PopulateImageLoadFile(True) Then If _application.ReadyToLoad(Me.ImageLoadFile, True) Then _application.PreviewImageFile(Me.ImageLoadFile)
		End Sub

		Private Sub _advancedButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _advancedButton.Click
			_advancedFileForm = New AdvancedFileLocation
			_advancedFileForm._copyNativeFiles.Checked = Me.ImageLoadFile.CopyFilesToDocumentRepository
			_advancedFileForm._keepNativeFiles.Checked = Not Me.ImageLoadFile.CopyFilesToDocumentRepository
			If Not Me.ImageLoadFile.SelectedCasePath Is Nothing AndAlso Not Me.ImageLoadFile.SelectedCasePath = "" Then
				_advancedFileForm.SelectPath(Me.ImageLoadFile.SelectedCasePath)
				_advancedFileForm.SelectDefaultPath = False
			End If
			_advancedFileForm.ShowDialog()
		End Sub
		Private Sub _advancedFileForm_FileLocationOK(ByVal copyFiles As Boolean, ByVal selectedRepository As String) Handles _advancedFileForm.FileLocationOK
			Me.ImageLoadFile.CopyFilesToDocumentRepository = copyFiles
			Me.ImageLoadFile.SelectedCasePath = selectedRepository
		End Sub
		Private Sub _importMenuSendEmailNotificationItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _importMenuSendEmailNotificationItem.Click
			_importMenuSendEmailNotificationItem.Checked = Not _importMenuSendEmailNotificationItem.Checked
		End Sub

		Private Sub _replaceFullText_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _replaceFullText.CheckedChanged
			Dim replaceFullText As CheckBox = DirectCast(sender, CheckBox)
			_encodingPicker.Enabled = replaceFullText.Checked
		End Sub

		Private Sub _autoNumberingOn_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _autoNumberingOn.CheckedChanged
			_autoNumberingOff.Checked = Not _autoNumberingOn.Checked
		End Sub

		Private Sub _autoNumberingOff_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _autoNumberingOff.CheckedChanged
			_autoNumberingOn.Checked = Not _autoNumberingOff.Checked
		End Sub
	End Class
End Namespace