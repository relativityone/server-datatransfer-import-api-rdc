Imports System.Xml.Serialization
Imports System.Xml

Namespace kCura.EDDS.WinForm
	Public Class ThreadExceptionHandler
		Public Sub Application_ThreadException(ByVal sender As System.Object, ByVal e As Threading.ThreadExceptionEventArgs)
			Try
				' Exit the program if the user clicks Abort.
				Dim result As DialogResult = ShowThreadExceptionDialog(e.Exception)

				If (result = DialogResult.Abort) Then
					System.Windows.Forms.Application.Exit()
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


		Private Function ShowThreadExceptionDialog(ByVal ex As Exception) As DialogResult

			Dim parsedException As System.Exception = ParseExceptionForMoreInfo(ex)

			Dim errorMessage As String =
			"Unhandled Exception:" &
			vbCrLf & vbCrLf &
			ex.Message & vbCrLf & vbCrLf &
			ex.GetType().ToString() & vbCrLf & vbCrLf &
			"Stack Trace:" & vbCrLf &
			ex.StackTrace

			Return MessageBox.Show(errorMessage, "Application Error", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Stop)
		End Function

		Private Function ParseExceptionForMoreInfo(ByVal ex As Exception) As Exception
			Dim resultException As System.Exception = ex

			If TypeOf ex Is System.Web.Services.Protocols.SoapException Then
				Dim soapException As System.Web.Services.Protocols.SoapException = DirectCast(ex, System.Web.Services.Protocols.SoapException)
				Dim xs As New XmlSerializer(GetType(Relativity.SoapExceptionDetail))
				Dim doc As New System.Xml.XmlDocument
				doc.LoadXml(soapException.Detail.OuterXml)
				Dim xr As XmlReader = doc.CreateNavigator.ReadSubtree
				Dim detailedException As Relativity.SoapExceptionDetail = TryCast(xs.Deserialize(xr), Relativity.SoapExceptionDetail)
			End If

			Return resultException
		End Function

	End Class
End Namespace
