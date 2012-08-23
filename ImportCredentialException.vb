Public Class ImportCredentialException
	Inherits System.Exception

	Public Sub New(ByVal message As String, ByVal userName As String, ByVal URL As String)
		MyBase.new(String.Format("{0}" + vbCrLf + "Username: {1}" + vbCrLf + "URL: {2}", message, userName, URL))

	End Sub
End Class
