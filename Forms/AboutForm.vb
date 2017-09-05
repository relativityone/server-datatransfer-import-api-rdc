Imports System

Public Class AboutForm
    Inherits System.Windows.Forms.Form

    Const INITIAL_FORM_HEIGHT As Int32 = 240
    Const EXPANDED_FORM_HEIGHT As Int32 = 450

    Private Sub AboutForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'Clean up UI from design mode
        Me.MainTextLabel.BorderStyle = BorderStyle.None
        DividerCtrl.Visible = False
        CopyrightTextBox.Visible = False
        Me.Height = INITIAL_FORM_HEIGHT

        Dim sb As New System.Text.StringBuilder
        Dim nl As String = System.Environment.NewLine & System.Environment.NewLine
        Dim bitness As String = String.Empty

        ' Determine build bitness
        If IntPtr.Size = 8 Then
            bitness = "64-bit"
        ElseIf IntPtr.Size = 4 Then
            bitness = "32-bit"
        End If

        sb.Append(String.Format("Relativity Desktop Client {0} {1}", bitness, nl))
        sb.Append("Version " & System.Reflection.Assembly.GetExecutingAssembly.GetName.Version.ToString & nl)
        sb.Append(Relativity.Constants.LICENSE_AGREEMENT_TEXT & nl)
        sb.Append("Copyright © " & System.DateTime.Now.Year & " kCura LLC")
        Me.MainTextLabel.Text = sb.ToString()

        CopyrightTextBox.Text = My.Resources.CopyrightInfo
    End Sub

    Private Sub OKBtn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OKBtn.Click
        Me.Close()
    End Sub

    Private Sub CopyrightBtn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CopyrightBtn.Click
        CopyrightTextBox.Visible = True
        CopyrightBtn.Enabled = False
        Me.Height = EXPANDED_FORM_HEIGHT
        DividerCtrl.Visible = True
        CopyrightTextBox.SelectionLength = 0
    End Sub

End Class
