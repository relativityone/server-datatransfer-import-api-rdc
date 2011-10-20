Imports System.Xml.Serialization
Imports System.Xml

Namespace kCura.EDDS.WinForm
	Public Class ThreadExceptionHandler
		Public Sub Application_ThreadException(ByVal sender As System.Object, ByVal e As Threading.ThreadExceptionEventArgs)
			Try
				ShowErrorDialog(e.Exception)
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
