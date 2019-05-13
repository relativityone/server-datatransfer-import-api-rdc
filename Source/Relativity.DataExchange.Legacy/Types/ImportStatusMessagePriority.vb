Namespace kCura.WinEDDS
	Public Class ImportStatusMessage
		Public Enum Priority
			Low = 1
			Medium = 2
			High = 4
		End Enum

		Public Shared Function IsHigh(ByVal priority As Int32) As Boolean
			Return (priority And ImportStatusMessage.Priority.High) > 0
		End Function

	End Class
End Namespace
