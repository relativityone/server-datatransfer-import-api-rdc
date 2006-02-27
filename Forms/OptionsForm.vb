Namespace kCura.EDDS.WinForm
	Public Class OptionsForm
		Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "

		Public Sub New()
			MyBase.New()

			'This call is required by the Windows Form Designer.
			InitializeComponent()

			'Add any initialization after the InitializeComponent() call
			_application = kCura.EDDS.WinForm.Application.Instance

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
		Friend WithEvents TimeZoneGroupBox As System.Windows.Forms.GroupBox
		Friend WithEvents _timeZoneDropDown As System.Windows.Forms.ComboBox
		Friend WithEvents _okButton As System.Windows.Forms.Button
		Friend WithEvents _cancelButton As System.Windows.Forms.Button
		<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
			Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(OptionsForm))
			Me.TimeZoneGroupBox = New System.Windows.Forms.GroupBox
			Me._timeZoneDropDown = New System.Windows.Forms.ComboBox
			Me._okButton = New System.Windows.Forms.Button
			Me._cancelButton = New System.Windows.Forms.Button
			Me.TimeZoneGroupBox.SuspendLayout()
			Me.SuspendLayout()
			'
			'TimeZoneGroupBox
			'
			Me.TimeZoneGroupBox.Controls.Add(Me._timeZoneDropDown)
			Me.TimeZoneGroupBox.Location = New System.Drawing.Point(8, 8)
			Me.TimeZoneGroupBox.Name = "TimeZoneGroupBox"
			Me.TimeZoneGroupBox.Size = New System.Drawing.Size(536, 56)
			Me.TimeZoneGroupBox.TabIndex = 0
			Me.TimeZoneGroupBox.TabStop = False
			Me.TimeZoneGroupBox.Text = "Default Time Zone"
			'
			'_timeZoneDropDown
			'
			Me._timeZoneDropDown.Location = New System.Drawing.Point(8, 24)
			Me._timeZoneDropDown.Name = "_timeZoneDropDown"
			Me._timeZoneDropDown.Size = New System.Drawing.Size(520, 21)
			Me._timeZoneDropDown.TabIndex = 0
			Me._timeZoneDropDown.Text = "Select ..."
			'
			'_okButton
			'
			Me._okButton.Location = New System.Drawing.Point(384, 80)
			Me._okButton.Name = "_okButton"
			Me._okButton.TabIndex = 1
			Me._okButton.Text = "OK"
			'
			'_cancelButton
			'
			Me._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
			Me._cancelButton.Location = New System.Drawing.Point(472, 80)
			Me._cancelButton.Name = "_cancelButton"
			Me._cancelButton.TabIndex = 2
			Me._cancelButton.Text = "Cancel"
			'
			'OptionsForm
			'
			Me.AcceptButton = Me._okButton
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
			Me.CancelButton = Me._cancelButton
			Me.ClientSize = New System.Drawing.Size(552, 109)
			Me.Controls.Add(Me._cancelButton)
			Me.Controls.Add(Me._okButton)
			Me.Controls.Add(Me.TimeZoneGroupBox)
			Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
			Me.Name = "OptionsForm"
			Me.Text = " Options"
			Me.TimeZoneGroupBox.ResumeLayout(False)
			Me.ResumeLayout(False)

		End Sub

#End Region

		Friend WithEvents _application As kCura.EDDS.WinForm.Application

		Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
			InitializeTimeZoneDropDown()
		End Sub

		Public Sub InitializeTimeZoneDropDown()
			Dim dt As New System.Data.DataTable
			dt.Columns.Add(New DataColumn("Display"))
			dt.Columns.Add(New DataColumn("Value"))
			AddDateTimeRow(dt, "(GMT-12:00) - International Date Line West", -12)
			AddDateTimeRow(dt, "(GMT-11:00) - Midway Island, Samoa", -11)
			AddDateTimeRow(dt, "(GMT-10:00) - Hawaii", -10)
			AddDateTimeRow(dt, "(GMT-9:00) - Alaska", -9)
			AddDateTimeRow(dt, "(GMT-8:00) - Pacific Time", -8)
			AddDateTimeRow(dt, "(GMT-7:00) - Mountain Time", -7)
			AddDateTimeRow(dt, "(GMT-6:00) - Central Time", -6)
			AddDateTimeRow(dt, "(GMT-5:00) - Eastern Time", -5)
			AddDateTimeRow(dt, "(GMT-4:00) - Atlantic Time", -4)
			AddDateTimeRow(dt, "(GMT-3:00) - Brazilia", -3)
			AddDateTimeRow(dt, "(GMT-2:00) - Mid Atlantic", -2)
			AddDateTimeRow(dt, "(GMT-1:00) - Cape Verde Is.", -1)
			AddDateTimeRow(dt, "(GMT+0:00) - Dublin, London", 0)
			AddDateTimeRow(dt, "(GMT+1:00)", 1)
			AddDateTimeRow(dt, "(GMT+2:00)", 2)
			AddDateTimeRow(dt, "(GMT+3:00) - Baghdad", 3)
			AddDateTimeRow(dt, "(GMT+4:00)", 4)
			AddDateTimeRow(dt, "(GMT+5:00)", 5)
			AddDateTimeRow(dt, "(GMT+6:00)", 6)
			AddDateTimeRow(dt, "(GMT+7:00)", 7)
			AddDateTimeRow(dt, "(GMT+8:00)", 8)
			AddDateTimeRow(dt, "(GMT+9:00)", 9)
			AddDateTimeRow(dt, "(GMT+10:00)", 10)
			AddDateTimeRow(dt, "(GMT+11:00)", 11)
			AddDateTimeRow(dt, "(GMT+12:00) - International Date Time East", 12)
			_timeZoneDropDown.DataSource = dt
			_timeZoneDropDown.DisplayMember = "Display"
			_timeZoneDropDown.ValueMember = "Value"

			_timeZoneDropDown.SelectedValue = _application.TimeZoneOffset
		End Sub

		Private Sub AddDateTimeRow(ByVal dt As DataTable, ByVal display As String, ByVal value As Int32)
			Dim al As New ArrayList
			al.Add(display)
			al.Add(value)
			dt.Rows.Add(al.ToArray)
		End Sub


		Private Sub _okButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles _okButton.Click
			_application.TimeZoneOffset = CType(_timeZoneDropDown.SelectedValue, Int32)
			Me.Close()
		End Sub

		Private Sub _cancelButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles _cancelButton.Click
			Me.Close()
		End Sub
	End Class
End Namespace
