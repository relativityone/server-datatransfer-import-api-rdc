Namespace kCura.WinEDDS.Exceptions
	Public Class ErrorMessage
		Private _message As String
		Public Sub New(msg As String)
			_message = msg
		End Sub
		Public Overrides Function ToString() As String
			Return _message
		End Function
	End Class
End Namespace
