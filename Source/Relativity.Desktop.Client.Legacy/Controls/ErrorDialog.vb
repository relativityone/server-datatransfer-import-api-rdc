Imports System.Web.Services.Protocols
Imports System.Windows.Forms

Namespace Relativity.Desktop.Client
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
		Friend WithEvents txtBoxFriendlyError As System.Windows.Forms.TextBox
		Friend WithEvents LinkLabel1 As System.Windows.Forms.LinkLabel
		<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
			Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ErrorDialog))
			Me.LinkLabel1 = New System.Windows.Forms.LinkLabel()
			Me.txtBoxFriendlyError = New System.Windows.Forms.TextBox()
			Me.SuspendLayout
			'
			'LinkLabel1
			'
			Me.LinkLabel1.Location = New System.Drawing.Point(1, 62)
			Me.LinkLabel1.Margin = New System.Windows.Forms.Padding(3, 0, 3, 15)
			Me.LinkLabel1.Name = "LinkLabel1"
			Me.LinkLabel1.Size = New System.Drawing.Size(176, 20)
			Me.LinkLabel1.TabIndex = 4
			Me.LinkLabel1.TabStop = true
			Me.LinkLabel1.Text = "Copy Error Text to Clipboard"
			'
			'txtBoxFriendlyError
			'
			Me.txtBoxFriendlyError.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
				Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
			Me.txtBoxFriendlyError.BackColor = System.Drawing.SystemColors.ScrollBar
			Me.txtBoxFriendlyError.Location = New System.Drawing.Point(4, 1)
			Me.txtBoxFriendlyError.Multiline = true
			Me.txtBoxFriendlyError.Name = "txtBoxFriendlyError"
			Me.txtBoxFriendlyError.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
			Me.txtBoxFriendlyError.Size = New System.Drawing.Size(315, 45)
			Me.txtBoxFriendlyError.TabIndex = 5
			'
			'ErrorDialog
			'
			Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
			Me.ClientSize = New System.Drawing.Size(304, 116)
			Me.Controls.Add(Me.txtBoxFriendlyError)
			Me.Controls.Add(Me.LinkLabel1)
			Me.Icon = CType(resources.GetObject("$this.Icon"),System.Drawing.Icon)
			Me.MaximizeBox = false
			Me.MaximumSize = New System.Drawing.Size(500, 116)
			Me.MinimizeBox = false
			Me.MinimumSize = New System.Drawing.Size(304, 116)
			Me.Name = "ErrorDialog"
			Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
			Me.Text = "Error"
			Me.ResumeLayout(false)
			Me.PerformLayout
		End Sub
	#End Region

		Public Sub Initialize(ByVal ex As System.Exception)
			Initialize(ex, ex.Message)
		End Sub

		Public Sub Initialize(ByVal ex As System.Exception, ByVal errorMessage As String)
			If TypeOf ex Is SoapException Then
				Try
					Dim MrSoapy As SoapException = CType(ex, SoapException)
					txtBoxFriendlyError.Text = MrSoapy.Detail("ExceptionMessage").InnerText
					Return
				Catch ex2 As System.Exception	'MrSoapy not soapified :(
					'cascade to a default rendering
				End Try
			End If
			txtBoxFriendlyError.Text = errorMessage
		End Sub

		Public Sub InitializeSoapExceptionWithCustomMessage(ByVal ex As SoapException, ByVal errorMessage As String)
			Try
				If String.IsNullOrEmpty(errorMessage) Then
					txtBoxFriendlyError.Text = ex.Detail("ExceptionMessage").InnerText
				Else
					txtBoxFriendlyError.Text = errorMessage
				End If
				Return
			Catch ex2 As System.Exception
				txtBoxFriendlyError.Text = errorMessage
			End Try
		End Sub

		Private Sub LinkLabel1_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
			System.Windows.Forms.Clipboard.SetText(txtBoxFriendlyError.Text)
		End Sub

		Private Sub ErrorDialog_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
            Me.DialogResult = DialogResult.OK
        End Sub

	End Class
End Namespace