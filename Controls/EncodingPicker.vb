Namespace kCura.EDDS.WinForm
	Public Class EncodingPicker
		Inherits System.Windows.Forms.UserControl

#Region " Windows Form Designer generated code "

		Public Sub New()
			MyBase.New()

			'This call is required by the Windows Form Designer.
			InitializeComponent()

			'Add any initialization after the InitializeComponent() call

		End Sub

		'UserControl overrides dispose to clean up the component list.
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
		Friend WithEvents Button1 As System.Windows.Forms.Button
		Friend WithEvents DropDown As System.Windows.Forms.ComboBox
		<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
			Me.DropDown = New System.Windows.Forms.ComboBox
			Me.Button1 = New System.Windows.Forms.Button
			Me.SuspendLayout()
			'
			'DropDown
			'
			Me.DropDown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
			Me.DropDown.Location = New System.Drawing.Point(0, 0)
			Me.DropDown.Name = "DropDown"
			Me.DropDown.Size = New System.Drawing.Size(176, 21)
			Me.DropDown.TabIndex = 0
			'
			'Button1
			'
			Me.Button1.Location = New System.Drawing.Point(176, 0)
			Me.Button1.Name = "Button1"
			Me.Button1.Size = New System.Drawing.Size(24, 21)
			Me.Button1.TabIndex = 1
			Me.Button1.Text = "..."
			'
			'EncodingPicker
			'
			Me.Controls.Add(Me.Button1)
			Me.Controls.Add(Me.DropDown)
			Me.Name = "EncodingPicker"
			Me.Size = New System.Drawing.Size(200, 21)
			Me.ResumeLayout(False)

		End Sub

#End Region
		Private WithEvents _frm As EncodingForm

		Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
			_frm = New EncodingForm
			_frm.DropDownToUpdate = Me.DropDown
			_frm.ShowDialog()
		End Sub

		Private Sub _frm_EncodingOK(ByVal ei As EncodingItem) Handles _frm.EncodingOK
			If Not _frm.DropDownToUpdate.Items.Contains(ei) Then
				_frm.DropDownToUpdate.Items.Add(ei)
			End If
			_frm.DropDownToUpdate.SelectedItem = ei
			Constants.AddDefaultEncoding(ei)
		End Sub

		Private Sub EncodingPicker_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
			DropDown.Items.Clear()
			DropDown.Items.AddRange(Constants.DefaultEncodings)
			DropDown.SelectedIndex = DropDown.Items.Count - 1
			If TypeOf Me.ParentForm Is ImageImportSettingsForm Then
				Me.SelectedEncoding = DirectCast(Me.ParentForm, ImageImportSettingsForm).SelectedEncoding
			End If
		End Sub

		Public Property SelectedEncoding() As System.Text.Encoding
			Get
				Return DirectCast(DropDown.SelectedItem, EncodingItem).Encoding
			End Get
			Set(ByVal value As System.Text.Encoding)
				Dim success As Boolean = False
				For Each ei As EncodingItem In DropDown.Items
					If value.CodePage = ei.CodePageId Then
						DropDown.SelectedItem = ei
						success = True
						Exit Property
					End If
				Next
				If Not success Then
					For Each ei As EncodingItem In Constants.AllEncodings
						If value.CodePage = ei.CodePageId Then
							DropDown.Items.Add(ei)
							DropDown.SelectedItem = ei
							Exit Property
						End If
					Next
				End If
			End Set
		End Property
	End Class



End Namespace
