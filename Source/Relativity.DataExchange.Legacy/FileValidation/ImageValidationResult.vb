Namespace kCura.WinEDDS
	Public Class ImageValidationResult

		Public Sub New (isValid As Boolean, ByRef message As String)
			Me.IsValid = isValid
			Me.Message = message
		End Sub

		Public Property IsValid As Boolean
		Property Message As String
	End Class

End Namespace