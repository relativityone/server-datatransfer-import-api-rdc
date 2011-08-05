Imports System
Imports System.Xml
Imports System.Web.Services.Protocols

Public Class ErrorDialog
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
	Friend WithEvents _errorText As System.Windows.Forms.TextBox
	Friend WithEvents txtBoxFriendlyError As System.Windows.Forms.TextBox
	Friend WithEvents LinkLabel1 As System.Windows.Forms.LinkLabel
	<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
		Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ErrorDialog))
		Me._okButton = New System.Windows.Forms.Button()
		Me._cancelButton = New System.Windows.Forms.Button()
		Me._errorText = New System.Windows.Forms.TextBox()
		Me.LinkLabel1 = New System.Windows.Forms.LinkLabel()
		Me.txtBoxFriendlyError = New System.Windows.Forms.TextBox()
		Me.SuspendLayout()
		'
		'_okButton
		'
		Me._okButton.Location = New System.Drawing.Point(157, 52)
		Me._okButton.Name = "_okButton"
		Me._okButton.Size = New System.Drawing.Size(68, 23)
		Me._okButton.TabIndex = 1
		Me._okButton.Text = "Continue"
		'
		'_cancelButton
		'
		Me._cancelButton.Location = New System.Drawing.Point(225, 52)
		Me._cancelButton.Name = "_cancelButton"
		Me._cancelButton.Size = New System.Drawing.Size(68, 23)
		Me._cancelButton.TabIndex = 2
		Me._cancelButton.Text = "Quit"
		'
		'_errorText
		'
		Me._errorText.Location = New System.Drawing.Point(4, 76)
		Me._errorText.Multiline = True
		Me._errorText.Name = "_errorText"
		Me._errorText.ReadOnly = True
		Me._errorText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
		Me._errorText.Size = New System.Drawing.Size(288, 192)
		Me._errorText.TabIndex = 3
		Me._errorText.Visible = False
		'
		'LinkLabel1
		'
		Me.LinkLabel1.Location = New System.Drawing.Point(4, 60)
		Me.LinkLabel1.Name = "LinkLabel1"
		Me.LinkLabel1.Size = New System.Drawing.Size(126, 16)
		Me.LinkLabel1.TabIndex = 4
		Me.LinkLabel1.TabStop = True
		Me.LinkLabel1.Text = "Show Error Text"
		'
		'txtBoxFriendlyError
		'
		Me.txtBoxFriendlyError.BackColor = System.Drawing.SystemColors.ScrollBar
		Me.txtBoxFriendlyError.Location = New System.Drawing.Point(4, 1)
		Me.txtBoxFriendlyError.Multiline = True
		Me.txtBoxFriendlyError.Name = "txtBoxFriendlyError"
		Me.txtBoxFriendlyError.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
		Me.txtBoxFriendlyError.Size = New System.Drawing.Size(288, 45)
		Me.txtBoxFriendlyError.TabIndex = 5
		'
		'ErrorDialog
		'
		Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
		Me.ClientSize = New System.Drawing.Size(296, 89)
		Me.ControlBox = False
		Me.Controls.Add(Me.txtBoxFriendlyError)
		Me.Controls.Add(Me.LinkLabel1)
		Me.Controls.Add(Me._errorText)
		Me.Controls.Add(Me._cancelButton)
		Me.Controls.Add(Me._okButton)
		Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
		Me.MaximizeBox = False
		Me.MaximumSize = New System.Drawing.Size(304, 116)
		Me.MinimizeBox = False
		Me.MinimumSize = New System.Drawing.Size(304, 116)
		Me.Name = "ErrorDialog"
		Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
		Me.Text = "Error"
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub

#End Region

	Private Property IsFatalException As Boolean

	Public Sub Initialize(ByVal ex As System.Exception)
		Initialize(ex, ex.Message)
	End Sub

	Public Sub Initialize(ByVal ex As System.Exception, ByVal isFatalError As Boolean)
		Initialize(ex, ex.Message)
		Me.IsFatalException = isFatalError
		Me._okButton.Visible = False
		Me._cancelButton.Text = "Close"
	End Sub

	Public Sub Initialize(ByVal ex As System.Exception, ByVal errorMessage As String)
		If TypeOf ex Is SoapException Then
			Try
				Dim MrSoapy As SoapException = CType(ex, SoapException)
				txtBoxFriendlyError.Text = MrSoapy.Detail("ExceptionMessage").InnerText
				_errorText.Text = MrSoapy.Detail("ExceptionFullText").InnerText
				Return
			Catch ex2 As System.Exception	'MrSoapy not soapified :(
				'cascade to a default rendering
			End Try
		End If
		txtBoxFriendlyError.Text = errorMessage
		_errorText.Text = errorMessage & vbNewLine & "-----" & vbNewLine & ex.ToString
	End Sub

	Private Sub LinkLabel1_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
		If LinkLabel1.Text.ToLower = "show error text" Then
			ExpandExceptionDetails()
		Else
			CollapseExceptionDetails()
		End If
	End Sub

	Private Sub CollapseExceptionDetails()
		LinkLabel1.Text = "Show Error Text"
		Me.Width = 304
		Me.Height = 116
		Me.MaximumSize = New System.Drawing.Size(304, 116)
		Me.MinimumSize = New System.Drawing.Size(304, 116)
		_errorText.Focus()
		_errorText.Visible = False
	End Sub

	Private Sub ExpandExceptionDetails()
		LinkLabel1.Text = "Hide Error Text"
		Me.Width = 304
		Me.Height = 308
		Me.MaximumSize = New System.Drawing.Size(304, 308)
		Me.MinimumSize = New System.Drawing.Size(304, 308)
		_errorText.Visible = True
		_errorText.Focus()
		_errorText.SelectAll()
	End Sub

	Private Sub _errorText_VisibleChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _errorText.VisibleChanged
		_errorText.SelectAll()
	End Sub

	Private Sub _okButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _okButton.Click
		Me.DialogResult = Windows.Forms.DialogResult.OK
		Me.Close()
	End Sub

	Private Sub _cancelButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _cancelButton.Click
		If Me.IsFatalException Then
			Me.DialogResult = Windows.Forms.DialogResult.OK
		Else
			Me.DialogResult = Windows.Forms.DialogResult.Cancel
		End If
		Me.Close()
	End Sub

	Private Sub ErrorDialog_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
		If Me.IsFatalException Then
			ExpandExceptionDetails()
		End If
	End Sub
End Class
