Namespace kCura.EDDS.WinForm
	Public Class VolumeInfoForm
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
		Friend WithEvents _subDirectoryInformationGroupBox As System.Windows.Forms.GroupBox
		Friend WithEvents _subDirectoryNativePrefix As System.Windows.Forms.TextBox
		Friend WithEvents Label13 As System.Windows.Forms.Label
		Friend WithEvents _subDirectoryMaxSize As System.Windows.Forms.NumericUpDown
		Friend WithEvents _subdirectoryStartNumber As System.Windows.Forms.NumericUpDown
		Friend WithEvents Label9 As System.Windows.Forms.Label
		Friend WithEvents Label10 As System.Windows.Forms.Label
		Friend WithEvents Label11 As System.Windows.Forms.Label
		Friend WithEvents _subdirectoryImagePrefix As System.Windows.Forms.TextBox
		Friend WithEvents _volumeInformationGroupBox As System.Windows.Forms.GroupBox
		Friend WithEvents _volumeMaxSize As System.Windows.Forms.NumericUpDown
		Friend WithEvents _volumeStartNumber As System.Windows.Forms.NumericUpDown
		Friend WithEvents Label8 As System.Windows.Forms.Label
		Friend WithEvents Label7 As System.Windows.Forms.Label
		Friend WithEvents Label5 As System.Windows.Forms.Label
		Friend WithEvents _volumePrefix As System.Windows.Forms.TextBox
		Friend WithEvents _okButton As System.Windows.Forms.Button
		Friend WithEvents _cancelButton As System.Windows.Forms.Button
		Friend WithEvents _subdirectoryTextPrefix As System.Windows.Forms.TextBox
		Friend WithEvents Label1 As System.Windows.Forms.Label
		<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
			Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(VolumeInfoForm))
			Me._subDirectoryInformationGroupBox = New System.Windows.Forms.GroupBox
			Me._subdirectoryTextPrefix = New System.Windows.Forms.TextBox
			Me.Label1 = New System.Windows.Forms.Label
			Me._subDirectoryNativePrefix = New System.Windows.Forms.TextBox
			Me.Label13 = New System.Windows.Forms.Label
			Me._subDirectoryMaxSize = New System.Windows.Forms.NumericUpDown
			Me._subdirectoryStartNumber = New System.Windows.Forms.NumericUpDown
			Me.Label9 = New System.Windows.Forms.Label
			Me.Label10 = New System.Windows.Forms.Label
			Me.Label11 = New System.Windows.Forms.Label
			Me._subdirectoryImagePrefix = New System.Windows.Forms.TextBox
			Me._volumeInformationGroupBox = New System.Windows.Forms.GroupBox
			Me._volumeMaxSize = New System.Windows.Forms.NumericUpDown
			Me._volumeStartNumber = New System.Windows.Forms.NumericUpDown
			Me.Label8 = New System.Windows.Forms.Label
			Me.Label7 = New System.Windows.Forms.Label
			Me.Label5 = New System.Windows.Forms.Label
			Me._volumePrefix = New System.Windows.Forms.TextBox
			Me._okButton = New System.Windows.Forms.Button
			Me._cancelButton = New System.Windows.Forms.Button
			Me._subDirectoryInformationGroupBox.SuspendLayout()
			CType(Me._subDirectoryMaxSize, System.ComponentModel.ISupportInitialize).BeginInit()
			CType(Me._subdirectoryStartNumber, System.ComponentModel.ISupportInitialize).BeginInit()
			Me._volumeInformationGroupBox.SuspendLayout()
			CType(Me._volumeMaxSize, System.ComponentModel.ISupportInitialize).BeginInit()
			CType(Me._volumeStartNumber, System.ComponentModel.ISupportInitialize).BeginInit()
			Me.SuspendLayout()
			'
			'_subDirectoryInformationGroupBox
			'
			Me._subDirectoryInformationGroupBox.Controls.Add(Me._subdirectoryTextPrefix)
			Me._subDirectoryInformationGroupBox.Controls.Add(Me.Label1)
			Me._subDirectoryInformationGroupBox.Controls.Add(Me._subDirectoryNativePrefix)
			Me._subDirectoryInformationGroupBox.Controls.Add(Me.Label13)
			Me._subDirectoryInformationGroupBox.Controls.Add(Me._subDirectoryMaxSize)
			Me._subDirectoryInformationGroupBox.Controls.Add(Me._subdirectoryStartNumber)
			Me._subDirectoryInformationGroupBox.Controls.Add(Me.Label9)
			Me._subDirectoryInformationGroupBox.Controls.Add(Me.Label10)
			Me._subDirectoryInformationGroupBox.Controls.Add(Me.Label11)
			Me._subDirectoryInformationGroupBox.Controls.Add(Me._subdirectoryImagePrefix)
			Me._subDirectoryInformationGroupBox.Location = New System.Drawing.Point(8, 120)
			Me._subDirectoryInformationGroupBox.Name = "_subDirectoryInformationGroupBox"
			Me._subDirectoryInformationGroupBox.Size = New System.Drawing.Size(280, 160)
			Me._subDirectoryInformationGroupBox.TabIndex = 16
			Me._subDirectoryInformationGroupBox.TabStop = False
			Me._subDirectoryInformationGroupBox.Text = "Subdirectory Information"
			'
			'_subdirectoryTextPrefix
			'
			Me._subdirectoryTextPrefix.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
			Me._subdirectoryTextPrefix.Location = New System.Drawing.Point(88, 76)
			Me._subdirectoryTextPrefix.Name = "_subdirectoryTextPrefix"
			Me._subdirectoryTextPrefix.Size = New System.Drawing.Size(176, 20)
			Me._subdirectoryTextPrefix.TabIndex = 22
			Me._subdirectoryTextPrefix.Text = "TEXT"
			'
			'Label1
			'
			Me.Label1.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
			Me.Label1.Location = New System.Drawing.Point(8, 80)
			Me.Label1.Name = "Label1"
			Me.Label1.Size = New System.Drawing.Size(80, 16)
			Me.Label1.TabIndex = 21
			Me.Label1.Text = "Text Prefix: "
			Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight
			'
			'_subDirectoryNativePrefix
			'
			Me._subDirectoryNativePrefix.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
			Me._subDirectoryNativePrefix.Location = New System.Drawing.Point(88, 48)
			Me._subDirectoryNativePrefix.Name = "_subDirectoryNativePrefix"
			Me._subDirectoryNativePrefix.Size = New System.Drawing.Size(176, 20)
			Me._subDirectoryNativePrefix.TabIndex = 20
			Me._subDirectoryNativePrefix.Text = "NATIVE"
			'
			'Label13
			'
			Me.Label13.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
			Me.Label13.Location = New System.Drawing.Point(16, 52)
			Me.Label13.Name = "Label13"
			Me.Label13.Size = New System.Drawing.Size(72, 16)
			Me.Label13.TabIndex = 19
			Me.Label13.Text = "Native Prefix: "
			Me.Label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight
			'
			'_subDirectoryMaxSize
			'
			Me._subDirectoryMaxSize.Location = New System.Drawing.Point(88, 132)
			Me._subDirectoryMaxSize.Maximum = New Decimal(New Integer() {2000000, 0, 0, 0})
			Me._subDirectoryMaxSize.Name = "_subDirectoryMaxSize"
			Me._subDirectoryMaxSize.Size = New System.Drawing.Size(176, 20)
			Me._subDirectoryMaxSize.TabIndex = 17
			Me._subDirectoryMaxSize.Value = New Decimal(New Integer() {500, 0, 0, 0})
			'
			'_subdirectoryStartNumber
			'
			Me._subdirectoryStartNumber.Location = New System.Drawing.Point(88, 104)
			Me._subdirectoryStartNumber.Maximum = New Decimal(New Integer() {2000000, 0, 0, 0})
			Me._subdirectoryStartNumber.Name = "_subdirectoryStartNumber"
			Me._subdirectoryStartNumber.Size = New System.Drawing.Size(176, 20)
			Me._subdirectoryStartNumber.TabIndex = 16
			Me._subdirectoryStartNumber.Value = New Decimal(New Integer() {1, 0, 0, 0})
			'
			'Label9
			'
			Me.Label9.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
			Me.Label9.Location = New System.Drawing.Point(28, 132)
			Me.Label9.Name = "Label9"
			Me.Label9.Size = New System.Drawing.Size(60, 16)
			Me.Label9.TabIndex = 15
			Me.Label9.Text = "Max Files:"
			Me.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight
			'
			'Label10
			'
			Me.Label10.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
			Me.Label10.Location = New System.Drawing.Point(42, 112)
			Me.Label10.Name = "Label10"
			Me.Label10.Size = New System.Drawing.Size(44, 16)
			Me.Label10.TabIndex = 14
			Me.Label10.Text = "Start #:"
			Me.Label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight
			'
			'Label11
			'
			Me.Label11.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
			Me.Label11.Location = New System.Drawing.Point(16, 24)
			Me.Label11.Name = "Label11"
			Me.Label11.Size = New System.Drawing.Size(72, 16)
			Me.Label11.TabIndex = 13
			Me.Label11.Text = "Image Prefix: "
			Me.Label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight
			'
			'_subdirectoryImagePrefix
			'
			Me._subdirectoryImagePrefix.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
			Me._subdirectoryImagePrefix.Location = New System.Drawing.Point(88, 20)
			Me._subdirectoryImagePrefix.Name = "_subdirectoryImagePrefix"
			Me._subdirectoryImagePrefix.Size = New System.Drawing.Size(176, 20)
			Me._subdirectoryImagePrefix.TabIndex = 12
			Me._subdirectoryImagePrefix.Text = "IMG"
			'
			'_volumeInformationGroupBox
			'
			Me._volumeInformationGroupBox.Controls.Add(Me._volumeMaxSize)
			Me._volumeInformationGroupBox.Controls.Add(Me._volumeStartNumber)
			Me._volumeInformationGroupBox.Controls.Add(Me.Label8)
			Me._volumeInformationGroupBox.Controls.Add(Me.Label7)
			Me._volumeInformationGroupBox.Controls.Add(Me.Label5)
			Me._volumeInformationGroupBox.Controls.Add(Me._volumePrefix)
			Me._volumeInformationGroupBox.Location = New System.Drawing.Point(8, 8)
			Me._volumeInformationGroupBox.Name = "_volumeInformationGroupBox"
			Me._volumeInformationGroupBox.Size = New System.Drawing.Size(280, 104)
			Me._volumeInformationGroupBox.TabIndex = 15
			Me._volumeInformationGroupBox.TabStop = False
			Me._volumeInformationGroupBox.Text = "Volume Information"
			'
			'_volumeMaxSize
			'
			Me._volumeMaxSize.Location = New System.Drawing.Point(88, 76)
			Me._volumeMaxSize.Maximum = New Decimal(New Integer() {2000000, 0, 0, 0})
			Me._volumeMaxSize.Name = "_volumeMaxSize"
			Me._volumeMaxSize.Size = New System.Drawing.Size(180, 20)
			Me._volumeMaxSize.TabIndex = 11
			Me._volumeMaxSize.Value = New Decimal(New Integer() {650, 0, 0, 0})
			'
			'_volumeStartNumber
			'
			Me._volumeStartNumber.Location = New System.Drawing.Point(88, 48)
			Me._volumeStartNumber.Maximum = New Decimal(New Integer() {2000000, 0, 0, 0})
			Me._volumeStartNumber.Name = "_volumeStartNumber"
			Me._volumeStartNumber.Size = New System.Drawing.Size(180, 20)
			Me._volumeStartNumber.TabIndex = 10
			Me._volumeStartNumber.Value = New Decimal(New Integer() {1, 0, 0, 0})
			'
			'Label8
			'
			Me.Label8.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
			Me.Label8.Location = New System.Drawing.Point(4, 76)
			Me.Label8.Name = "Label8"
			Me.Label8.Size = New System.Drawing.Size(82, 16)
			Me.Label8.TabIndex = 9
			Me.Label8.Text = "Max Size (MB):"
			Me.Label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight
			'
			'Label7
			'
			Me.Label7.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
			Me.Label7.Location = New System.Drawing.Point(44, 48)
			Me.Label7.Name = "Label7"
			Me.Label7.Size = New System.Drawing.Size(44, 16)
			Me.Label7.TabIndex = 8
			Me.Label7.Text = "Start #:"
			Me.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight
			'
			'Label5
			'
			Me.Label5.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
			Me.Label5.Location = New System.Drawing.Point(48, 24)
			Me.Label5.Name = "Label5"
			Me.Label5.Size = New System.Drawing.Size(40, 16)
			Me.Label5.TabIndex = 7
			Me.Label5.Text = "Prefix: "
			Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight
			'
			'_volumePrefix
			'
			Me._volumePrefix.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
			Me._volumePrefix.Location = New System.Drawing.Point(88, 20)
			Me._volumePrefix.Name = "_volumePrefix"
			Me._volumePrefix.Size = New System.Drawing.Size(180, 20)
			Me._volumePrefix.TabIndex = 6
			Me._volumePrefix.Text = "VOL"
			'
			'_okButton
			'
			Me._okButton.Location = New System.Drawing.Point(132, 288)
			Me._okButton.Name = "_okButton"
			Me._okButton.TabIndex = 17
			Me._okButton.Text = "OK"
			'
			'_cancelButton
			'
			Me._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
			Me._cancelButton.Location = New System.Drawing.Point(212, 288)
			Me._cancelButton.Name = "_cancelButton"
			Me._cancelButton.TabIndex = 18
			Me._cancelButton.Text = "Cancel"
			'
			'VolumeInfoForm
			'
			Me.AcceptButton = Me._okButton
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
			Me.CancelButton = Me._cancelButton
			Me.ClientSize = New System.Drawing.Size(296, 319)
			Me.Controls.Add(Me._cancelButton)
			Me.Controls.Add(Me._okButton)
			Me.Controls.Add(Me._subDirectoryInformationGroupBox)
			Me.Controls.Add(Me._volumeInformationGroupBox)
			Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
			Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
			Me.MaximumSize = New System.Drawing.Size(302, 316)
			Me.MinimumSize = New System.Drawing.Size(302, 316)
			Me.Name = "VolumeInfoForm"
			Me.Text = "Settings"
			Me._subDirectoryInformationGroupBox.ResumeLayout(False)
			CType(Me._subDirectoryMaxSize, System.ComponentModel.ISupportInitialize).EndInit()
			CType(Me._subdirectoryStartNumber, System.ComponentModel.ISupportInitialize).EndInit()
			Me._volumeInformationGroupBox.ResumeLayout(False)
			CType(Me._volumeMaxSize, System.ComponentModel.ISupportInitialize).EndInit()
			CType(Me._volumeStartNumber, System.ComponentModel.ISupportInitialize).EndInit()
			Me.ResumeLayout(False)

		End Sub

#End Region

		Private _volumeInfo As Exporters.VolumeInfo

		Public Event VolumeOK(ByVal e As Exporters.VolumeInfo)

		Private Sub VolumeInfoForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
			_subdirectoryImagePrefix.Text = _volumeInfo.SubdirectoryImagePrefix
			_subDirectoryMaxSize.Value = _volumeInfo.SubdirectoryMaxSize
			_subDirectoryNativePrefix.Text = _volumeInfo.SubdirectoryNativePrefix
			_subdirectoryStartNumber.Value = _volumeInfo.SubdirectoryStartNumber
			_volumeMaxSize.Value = _volumeInfo.VolumeMaxSize
			_volumePrefix.Text = _volumeInfo.VolumePrefix
			_volumeStartNumber.Value = _volumeInfo.VolumeStartNumber
		End Sub

		Private Sub _okButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _okButton.Click
			Dim retval As New Exporters.VolumeInfo
			If _subdirectoryImagePrefix.Text.Trim <> "" Then
				retval.SubdirectoryImagePrefix = _subdirectoryImagePrefix.Text
			Else
				MsgBox("Subdirectory Image Prefix cannot be blank.", MsgBoxStyle.Exclamation)
				Exit Sub
			End If
			If _subdirectoryTextPrefix.Text.Trim <> "" Then
				retval.SubdirectoryFullTextPrefix = _subdirectoryTextPrefix.Text
			Else
				MsgBox("Subdirectory Text Prefix cannot be blank.", MsgBoxStyle.Exclamation)
				Exit Sub
			End If
			retval.SubdirectoryMaxSize = CType(_subDirectoryMaxSize.Value, Int64)
			If retval.SubdirectoryMaxSize < 1 OrElse _subDirectoryMaxSize.Text.Trim = "" Then
				MsgBox("Subdirectory Max Size must be greater than zero.", MsgBoxStyle.Exclamation)
				Exit Sub
			End If
			If _subDirectoryNativePrefix.Text.Trim <> "" Then
				retval.SubdirectoryNativePrefix = _subDirectoryNativePrefix.Text
			Else
				MsgBox("Subdirectory Native Prefix cannot be blank.", MsgBoxStyle.Exclamation)
				Exit Sub
			End If
			retval.SubdirectoryStartNumber = CType(_subdirectoryStartNumber.Value, Int32)
			If retval.SubdirectoryStartNumber < 1 OrElse _subdirectoryStartNumber.Text.Trim = "" Then
				MsgBox("Subdirectory Start Number must be greater than zero.", MsgBoxStyle.Exclamation)
				Exit Sub
			End If
			retval.VolumeMaxSize = CType(_volumeMaxSize.Value, Int64)
			If retval.VolumeMaxSize < 1 OrElse _volumeMaxSize.Text.Trim = "" Then
				MsgBox("Volume Max Size must be greater than zero.", MsgBoxStyle.Exclamation)
				Exit Sub
			End If
			If _volumePrefix.Text.Trim <> "" Then
				retval.VolumePrefix = _volumePrefix.Text
			Else
				MsgBox("Volume Prefix cannot be blank.", MsgBoxStyle.Exclamation)
				Exit Sub
			End If
			retval.VolumeStartNumber = CType(_volumeStartNumber.Value, Int32)
			If retval.VolumeStartNumber < 1 Then
				MsgBox("Volume Start Number must be greater than zero.", MsgBoxStyle.Exclamation)
				Exit Sub
			End If
			Me.Close()
			RaiseEvent VolumeOK(retval)
		End Sub

		Private Sub _cancelButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _cancelButton.Click
			Me.Close()
		End Sub
		Private Sub _volumePrefix_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _volumePrefix.TextChanged
			_volumePrefix.Text = Me.CleanPath(_volumePrefix.Text)
			_volumePrefix.SelectionStart = _volumePrefix.Text.Length
		End Sub

		Private Sub _subdirectoryImagePrefix_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _subdirectoryImagePrefix.TextChanged
			_subdirectoryImagePrefix.Text = Me.CleanPath(_subdirectoryImagePrefix.Text)
			_subdirectoryImagePrefix.SelectionStart = _subdirectoryImagePrefix.Text.Length
		End Sub

		Private Sub _subDirectoryNativePrefix_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _subDirectoryNativePrefix.TextChanged
			_subDirectoryNativePrefix.Text = Me.CleanPath(_subDirectoryNativePrefix.Text)
			_subDirectoryNativePrefix.SelectionStart = _subDirectoryNativePrefix.Text.Length
		End Sub

		Private Function CleanPath(ByRef path As String) As String
			Dim retval As String = path
			retval = retval.Replace("\", "")
			retval = retval.Replace("/", "")
			retval = retval.Replace("*", "")
			retval = retval.Replace(":", "")
			retval = retval.Replace("?", "")
			retval = retval.Replace("""", "")
			retval = retval.Replace("<", "")
			retval = retval.Replace(">", "")
			retval = retval.Replace("|", "")
			Return retval
		End Function

		Public Sub New(ByVal info As Exporters.VolumeInfo)
			Me.New()
			_volumeInfo = info
		End Sub

		Private Sub _volumeMaxSize_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles _volumeMaxSize.LostFocus
			If _volumeMaxSize.Text.Trim = "" Then _volumeMaxSize.Text = "0"
		End Sub

		Private Sub _volumeStartNumber_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles _volumeStartNumber.LostFocus
			If _volumeStartNumber.Text.Trim = "" Then _volumeStartNumber.Text = "0"
		End Sub

		Private Sub _subdirectoryMaxSize_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles _subDirectoryMaxSize.LostFocus
			If _subDirectoryMaxSize.Text.Trim = "" Then _subDirectoryMaxSize.Text = "0"
		End Sub

		Private Sub _subdirectoryStartNumber_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles _subdirectoryStartNumber.LostFocus
			If _subdirectoryStartNumber.Text.Trim = "" Then _subdirectoryStartNumber.Text = "0"
		End Sub

	End Class
End Namespace
