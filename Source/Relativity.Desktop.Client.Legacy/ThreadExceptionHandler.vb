Imports Relativity.OAuth2Client.Exceptions

Namespace Relativity.Desktop.Client
	Public Class ThreadExceptionHandler
		Public Async Sub Application_ThreadException(ByVal sender As System.Object, ByVal e As Threading.ThreadExceptionEventArgs)
			Try
				If TypeOf e.Exception Is System.Web.Services.Protocols.SoapException AndAlso e.Exception.ToString.IndexOf("NeedToReLoginException") <> -1 Then
					Await Global.Relativity.Desktop.Client.Application.Instance.NewLoginAsync()
				ElseIf TypeOf e.Exception Is LoginCanceledException Then
					'Login canceled, ignore
				Else
					ShowErrorDialog(e.Exception)
				End If
			Catch ex As System.Exception
				' Fatal error, terminate program
				Try
					MessageBox.Show(String.Format("Fatal Error. {0}Message: {1} {2} Stack Trace: {3}", vbNewLine, ex.Message, vbNewLine, ex.StackTrace), "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Stop)
				Finally
					'System.Windows.Forms.Application.Exit()
				End Try
			End Try
		End Sub


		Private Sub ShowErrorDialog(ByVal ex As System.Exception)
			Dim x As New ErrorDialog With {.Text = "Relativity Desktop Client Error"}
			x.Initialize(ex)
			If x.ShowDialog() <> DialogResult.OK Then
				Environment.Exit(1)
			End If
		End Sub

	End Class
End Namespace