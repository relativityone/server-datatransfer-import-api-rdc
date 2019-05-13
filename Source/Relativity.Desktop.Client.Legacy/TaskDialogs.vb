Imports Microsoft.WindowsAPICodePack.Dialogs
Imports Relativity.DataExchange

Namespace Relativity.Desktop.Client

	''' <summary>
	''' Defines all task dialogs used by the application.
	''' </summary>
	Public Class TaskDialogs

		Private Const Caption As String = "Relativity Desktop Client"

		''' <summary>
		''' Shows the Relativity not supported task dialog and returns a value indicating whether to try a new URL.
		''' </summary>
		''' <param name="parent">
		''' The parent owner.
		''' </param>
		''' <param name="exception">
		''' The exception.
		''' </param>
		''' <returns>
		''' <see langword="true" /> to try a new URL; otherwise, <see langword="false" />.
		''' </returns>
		Public Shared Function ShowRelativityNotSupportedTaskDialog(ByVal parent As IntPtr, ByVal exception As RelativityNotSupportedException) As Boolean
			Try
				Dim helpUrl As String = WebHelpUrls.GetCompatibilityPageUrl()
				Using taskDialog As TaskDialog = New TaskDialog()
					taskDialog.Caption = Caption
					taskDialog.FooterIcon = TaskDialogStandardIcon.Information
					taskDialog.FooterText = $"<A HREF=""{helpUrl}"">View RDC compatibility details on the web</A>"
					taskDialog.HyperlinksEnabled = True
					taskDialog.Icon = TaskDialogStandardIcon.Warning
					taskDialog.InstructionText = "Do you want to try another URL?"
					taskDialog.OwnerWindowHandle = parent
					taskDialog.StandardButtons = TaskDialogStandardButtons.None
					taskDialog.StartupLocation = TaskDialogStartupLocation.CenterOwner
					taskDialog.Text = exception.Message
					Dim yesLink As TaskDialogCommandLink = New TaskDialogCommandLink With {.Name = "yes", .Default = True, .Text = "Yes", .Instruction = "Enter a new URL"}
					Dim noLink As TaskDialogCommandLink = New TaskDialogCommandLink With {.Name = "no", .Default = False, .Text = "No", .Instruction = "Exit this application"}
					taskDialog.Controls.Add(yesLink)
					taskDialog.Controls.Add(noLink)
					Dim result As Boolean = False
					AddHandler taskDialog.HyperlinkClick,
						Sub(sender As Object, e As TaskDialogHyperlinkClickedEventArgs)
							Try
								System.Diagnostics.Process.Start(e.LinkText)
							Catch ex As Exception
								MessageBox.Show($"Failed to view the RDC compatibility details page in the default browser. Error: {ex.Message}", "View Page Failed", MessageBoxButtons.OK, MessageBoxIcon.Error)
							End Try
						End SUb
					AddHandler yesLink.Click,
						Sub(sender As Object, e As EventArgs)
							result = True
							taskDialog.Close()
						End Sub
					AddHandler noLink.Click,
						Sub(sender As Object, e As EventArgs)
							result = False
							taskDialog.Close()
						End Sub
					taskDialog.Show()
					Return result
				End Using
			Catch ex As Exception
				' Fallback to the original message box.
				Dim message As String = exception.Message + vbCrLf + vbCrLf + "Try a new URL?"
				Dim result As MsgBoxResult = MsgBox(message, MsgBoxStyle.YesNo, Caption)
				Return result = MsgBoxResult.Yes
			End Try
		End Function
	End Class
End Namespace