﻿Imports System
Imports Relativity.Import.Export.Services

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
		Dim version As System.Version = Relativity.Desktop.Client.Application.GetRelativityBuildVersion()
		If (Not version Is Nothing) Then
			' Prefer displaying the Relativity version...
			sb.Append($"Version {version.ToString()}" & nl)
		Else
			' Otherwise, use the assembly version for stand-alone installations.
			version = Relativity.Desktop.Client.Application.GetAssemblyVersion()
			sb.Append($"Version {version.ToString()} (stand-alone)" & nl)
		End If

		Const LicenseAgreement As String = "The programs included herein are subject to a restricted use license and can only be used in conjunction with this application."
		sb.Append(LicenseAgreement & nl)
		sb.Append("Copyright © " & System.DateTime.Now.Year & " Relativity ODA LLC")
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