Public Class ErrorMessage
	Private _message As System.String
	Public Sub New(ex As System.Exception)
		_message = ex.Message
	End Sub
	Public Overrides Function ToString() As String
		Return _message
	End Function
End Class
