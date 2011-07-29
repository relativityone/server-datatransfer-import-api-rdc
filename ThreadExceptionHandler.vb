Namespace kCura.EDDS.WinForm
	Public Class ThreadExceptionHandler
		Public Sub Application_ThreadException(ByVal sender As System.Object, ByVal e As Threading.ThreadExceptionEventArgs)
			Try
				' Exit the program if the user clicks Abort.
				Dim result As DialogResult = ShowThreadExceptionDialog(e.Exception)

				If (result = DialogResult.Abort) Then
					System.Windows.Forms.Application.Exit()
				End If
			Catch
				' Fatal error, terminate program
				Try
					MessageBox.Show("Fatal Error", "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Stop)
				Finally
					System.Windows.Forms.Application.Exit()
				End Try
			End Try
		End Sub


		Private Function ShowThreadExceptionDialog(ByVal ex As Exception) As DialogResult

			Dim errorMessage As String =
			"Unhandled Exception:" &
			vbCrLf & vbCrLf &
			ex.Message & vbCrLf & vbCrLf &
			ex.GetType().ToString() & vbCrLf & vbCrLf &
			"Stack Trace:" & vbCrLf &
			ex.StackTrace

			Return MessageBox.Show(errorMessage, "Application Error", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Stop)
		End Function

	End Class
End Namespace
