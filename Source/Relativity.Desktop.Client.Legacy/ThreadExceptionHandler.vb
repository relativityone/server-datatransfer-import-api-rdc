Imports System.Web.Services.Protocols
Imports Relativity.Logging
Imports Relativity.OAuth2Client.Exceptions

Namespace Relativity.Desktop.Client
	Public Class ThreadExceptionHandler
		Private ReadOnly _logger As ILog

		Public Sub New (logger As ILog)
			Me._logger = logger
		End Sub

		Public Async Sub Application_ThreadException(ByVal sender As System.Object, ByVal e As Threading.ThreadExceptionEventArgs)
			Try
				If TypeOf e.Exception Is SoapException AndAlso e.Exception.ToString.IndexOf("NeedToReLoginException") <> -1 Then
					Await Application.Instance.NewLoginAsync()
				ElseIf TypeOf e.Exception Is LoginCanceledException Then
					'Login canceled, ignore
				Else
					ShowErrorDialog(e.Exception)
				End If
			Catch ex As Exception
				' Fatal error, terminate program
				_logger.LogFatal(ex, "Fatal Error")
				MessageBox.Show(String.Format("Fatal Error. {0}Message: {1} {2}", vbNewLine, ex.Message, vbNewLine), "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Stop)
			End Try
		End Sub


		Private Sub ShowErrorDialog(ByVal ex As System.Exception)
			Dim x As New ErrorDialog With {.Text = "Relativity Desktop Client Error"}
			Me._logger.LogError(ex, "Unexpected exception")
			x.Initialize(ex)
			If x.ShowDialog() <> DialogResult.OK Then
				Environment.Exit(1)
			End If
		End Sub

	End Class
End Namespace