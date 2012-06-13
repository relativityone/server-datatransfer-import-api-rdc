Imports System
Imports System.Xml
Imports System.Web.Services.Protocols
Imports System.Windows.Forms

Namespace kCura.Windows.Forms
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
		Friend WithEvents _errorText As System.Windows.Forms.TextBox
		Friend WithEvents txtBoxFriendlyError As System.Windows.Forms.TextBox
		Friend WithEvents _MoreDetailsButton As System.Windows.Forms.Button
		Friend WithEvents LinkLabel1 As System.Windows.Forms.LinkLabel
		<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
			Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ErrorDialog))
			Me._errorText = New System.Windows.Forms.TextBox()
			Me.LinkLabel1 = New System.Windows.Forms.LinkLabel()
			Me.txtBoxFriendlyError = New System.Windows.Forms.TextBox()
			Me._MoreDetailsButton = New System.Windows.Forms.Button()
			Me.SuspendLayout()
			'
			'_errorText
			'
			Me._errorText.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
									Or System.Windows.Forms.AnchorStyles.Left) _
									Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me._errorText.Location = New System.Drawing.Point(4, 88)
			Me._errorText.Multiline = True
			Me._errorText.Name = "_errorText"
			Me._errorText.ReadOnly = True
			Me._errorText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
			Me._errorText.Size = New System.Drawing.Size(315, 18)
			Me._errorText.TabIndex = 3
			Me._errorText.Visible = False
			'
			'LinkLabel1
			'
			Me.LinkLabel1.Location = New System.Drawing.Point(1, 62)
			Me.LinkLabel1.Margin = New System.Windows.Forms.Padding(3, 0, 3, 15)
			Me.LinkLabel1.Name = "LinkLabel1"
			Me.LinkLabel1.Size = New System.Drawing.Size(176, 20)
			Me.LinkLabel1.TabIndex = 4
			Me.LinkLabel1.TabStop = True
			Me.LinkLabel1.Text = "Copy Error Text to Clipboard"
			'
			'txtBoxFriendlyError
			'
			Me.txtBoxFriendlyError.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
									Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me.txtBoxFriendlyError.BackColor = System.Drawing.SystemColors.ScrollBar
			Me.txtBoxFriendlyError.Location = New System.Drawing.Point(4, 1)
			Me.txtBoxFriendlyError.Multiline = True
			Me.txtBoxFriendlyError.Name = "txtBoxFriendlyError"
			Me.txtBoxFriendlyError.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
			Me.txtBoxFriendlyError.Size = New System.Drawing.Size(315, 45)
			Me.txtBoxFriendlyError.TabIndex = 5
			'
			'_MoreDetailsButton
			'
			Me._MoreDetailsButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me._MoreDetailsButton.AutoSize = True
			Me._MoreDetailsButton.Location = New System.Drawing.Point(240, 59)
			Me._MoreDetailsButton.MinimumSize = New System.Drawing.Size(78, 23)
			Me._MoreDetailsButton.Name = "_MoreDetailsButton"
			Me._MoreDetailsButton.Size = New System.Drawing.Size(78, 23)
			Me._MoreDetailsButton.TabIndex = 6
			Me._MoreDetailsButton.Text = "More Details"
			'
			'ErrorDialog
			'
			Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
			Me.ClientSize = New System.Drawing.Size(323, 113)
			Me.Controls.Add(Me._MoreDetailsButton)
			Me.Controls.Add(Me.txtBoxFriendlyError)
			Me.Controls.Add(Me.LinkLabel1)
			Me.Controls.Add(Me._errorText)
			Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
			Me.MaximizeBox = False
			Me.MaximumSize = New System.Drawing.Size(500, 500)
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
			Me._errorText.Hide()
			CollapseExceptionDetails()
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

		Public Sub InitializeSoapExceptionWithCustomMessage(ByVal ex As SoapException, ByVal errorMessage As String)
			Try
				Dim MrSoapy As SoapException = CType(ex, SoapException)
				If String.IsNullOrEmpty(errorMessage) Then
					txtBoxFriendlyError.Text = MrSoapy.Detail("ExceptionMessage").InnerText
				Else
					txtBoxFriendlyError.Text = errorMessage
				End If
				_errorText.Text = MrSoapy.Detail("ExceptionFullText").InnerText
				Return
			Catch ex2 As System.Exception	'MrSoapy not soapified :(
				'use a default rendering
				txtBoxFriendlyError.Text = errorMessage
				_errorText.Text = errorMessage & vbNewLine & "-----" & vbNewLine & ex.ToString
			End Try
		End Sub

		Private Sub LinkLabel1_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
			System.Windows.Forms.Clipboard.SetText(_errorText.Text)
		End Sub

		Private Sub _MoreDetailsButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles _MoreDetailsButton.Click
			If _MoreDetailsButton.Text.ToLower = "more details" Then
				ExpandExceptionDetails()
			Else
				CollapseExceptionDetails()
			End If
		End Sub

		Private Sub CollapseExceptionDetails()
			_MoreDetailsButton.Text = "More Details"
			Me.Height = 140
			_errorText.Focus()
			_errorText.Visible = False
			_errorText.Hide()
		End Sub

		Private Sub ExpandExceptionDetails()
			_MoreDetailsButton.Text = "Hide Details"
			Me.Height = 317
			_errorText.Visible = True
			_errorText.Show()
			_errorText.Focus()
			_errorText.Select(0, 0)
		End Sub

		Private Sub _errorText_VisibleChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _errorText.VisibleChanged
			_errorText.Select(0, 0)
		End Sub

		Private Sub ErrorDialog_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
			Me.DialogResult = Windows.Forms.DialogResult.OK
		End Sub

		Private Sub ErrorDialog_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
			If Me.IsFatalException Then
				ExpandExceptionDetails()
			End If
		End Sub



	End Class
End Namespace