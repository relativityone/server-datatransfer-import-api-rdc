Namespace kCura.EDDS.WinForm
	Public Class ImageImportSettingsForm
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
		Friend WithEvents _okButton As System.Windows.Forms.Button
		Friend WithEvents _cancelButton As System.Windows.Forms.Button
		Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
		Friend WithEvents _supportImageAutoNumbering As System.Windows.Forms.CheckBox
		Friend WithEvents BroooopBox As System.Windows.Forms.GroupBox
		Friend WithEvents _encodingPicker As kCura.EDDS.WinForm.EncodingPicker
		<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
			Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(ImageImportSettingsForm))
			Me._okButton = New System.Windows.Forms.Button
			Me._cancelButton = New System.Windows.Forms.Button
			Me.GroupBox1 = New System.Windows.Forms.GroupBox
			Me._supportImageAutoNumbering = New System.Windows.Forms.CheckBox
			Me.BroooopBox = New System.Windows.Forms.GroupBox
			Me._encodingPicker = New kCura.EDDS.WinForm.EncodingPicker
			Me.GroupBox1.SuspendLayout()
			Me.BroooopBox.SuspendLayout()
			Me.SuspendLayout()
			'
			'_okButton
			'
			Me._okButton.Location = New System.Drawing.Point(296, 100)
			Me._okButton.Name = "_okButton"
			Me._okButton.Size = New System.Drawing.Size(75, 24)
			Me._okButton.TabIndex = 0
			Me._okButton.Text = "OK"
			'
			'_cancelButton
			'
			Me._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
			Me._cancelButton.Location = New System.Drawing.Point(376, 100)
			Me._cancelButton.Name = "_cancelButton"
			Me._cancelButton.Size = New System.Drawing.Size(75, 24)
			Me._cancelButton.TabIndex = 1
			Me._cancelButton.Text = "Cancel"
			'
			'GroupBox1
			'
			Me.GroupBox1.Controls.Add(Me._supportImageAutoNumbering)
			Me.GroupBox1.Location = New System.Drawing.Point(8, 8)
			Me.GroupBox1.Name = "GroupBox1"
			Me.GroupBox1.Size = New System.Drawing.Size(440, 40)
			Me.GroupBox1.TabIndex = 2
			Me.GroupBox1.TabStop = False
			Me.GroupBox1.Text = "General"
			'
			'_supportImageAutoNumbering
			'
			Me._supportImageAutoNumbering.Location = New System.Drawing.Point(8, 16)
			Me._supportImageAutoNumbering.Name = "_supportImageAutoNumbering"
			Me._supportImageAutoNumbering.Size = New System.Drawing.Size(184, 16)
			Me._supportImageAutoNumbering.TabIndex = 0
			Me._supportImageAutoNumbering.Text = "Support Image Auto-Numbering"
			'
			'BroooopBox
			'
			Me.BroooopBox.Controls.Add(Me._encodingPicker)
			Me.BroooopBox.Location = New System.Drawing.Point(8, 52)
			Me.BroooopBox.Name = "BroooopBox"
			Me.BroooopBox.Size = New System.Drawing.Size(280, 72)
			Me.BroooopBox.TabIndex = 3
			Me.BroooopBox.TabStop = False
			Me.BroooopBox.Text = "Full Text Options"
			'
			'_encodingPicker
			'
			Me._encodingPicker.Location = New System.Drawing.Point(44, 28)
			Me._encodingPicker.Name = "_encodingPicker"
			Me._encodingPicker.SelectedEncoding = CType(resources.GetObject("_encodingPicker.SelectedEncoding"), System.Text.Encoding)
			Me._encodingPicker.Size = New System.Drawing.Size(200, 21)
			Me._encodingPicker.TabIndex = 0
			'
			'ImageImportSettingsForm
			'
			Me.AcceptButton = Me._okButton
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
			Me.CancelButton = Me._cancelButton
			Me.ClientSize = New System.Drawing.Size(456, 133)
			Me.Controls.Add(Me.BroooopBox)
			Me.Controls.Add(Me.GroupBox1)
			Me.Controls.Add(Me._cancelButton)
			Me.Controls.Add(Me._okButton)
			Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
			Me.MaximizeBox = False
			Me.MaximumSize = New System.Drawing.Size(464, 160)
			Me.MinimizeBox = False
			Me.MinimumSize = New System.Drawing.Size(464, 160)
			Me.Name = "ImageImportSettingsForm"
			Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
			Me.Text = "Image Import Settings"
			Me.GroupBox1.ResumeLayout(False)
			Me.BroooopBox.ResumeLayout(False)
			Me.ResumeLayout(False)

		End Sub

#End Region

		Public SelectedEncoding As System.Text.Encoding

		Public Event ImportSettingsFormOK(ByVal args As SettingsFormArgs)

		Private Sub _okButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _okButton.Click
			Me.Close()
			Dim args As New SettingsFormArgs
			args.SelectedFullTextEncoding = _encodingPicker.SelectedEncoding
			args.SupportAutoNumbering = _supportImageAutoNumbering.Checked
			RaiseEvent ImportSettingsFormOK(args)
		End Sub

		Private Sub _cancelButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _cancelButton.Click
			Me.Close()
		End Sub

		Public Class SettingsFormArgs
			Public SupportAutoNumbering As Boolean
			Public SelectedFullTextEncoding As System.Text.Encoding
		End Class
	End Class
End Namespace