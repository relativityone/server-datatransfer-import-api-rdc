Namespace kCura.WinEDDS.Exporters
	Public Class NullUserNotification
		Implements IUserNotification

		Private Sub Log(message As String)
			Console.WriteLine(message)
		End Sub

		Public Sub Alert(message As String) Implements IUserNotification.Alert
			Log(message)
		End Sub

		Public Sub AlertCriticalError(message As String) Implements IUserNotification.AlertCriticalError
			Log("Critical: " & message)
		End Sub

		Public Function AlertWarningSkippable(message As String) As Boolean Implements IUserNotification.AlertWarningSkippable
			Log("Skippable: " & message)
			Return True
		End Function
	End Class
End Namespace