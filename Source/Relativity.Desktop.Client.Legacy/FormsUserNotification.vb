Imports kCura.WinEDDS.Exporters

Namespace kCura.EDDS.WinForm
	Public Class FormsUserNotification
		Implements IUserNotification
		Public Function AlertWarningSkippable(message As String) As Boolean Implements IUserNotification.AlertWarningSkippable
			Dim proceed As Boolean = True
			Select Case MsgBox(message.ToString, MsgBoxStyle.OkCancel, "Relativity Desktop Client")
				Case MsgBoxResult.Cancel
					proceed = False
			End Select
			Return proceed
		End Function

		Public Sub AlertCriticalError(message As String) Implements IUserNotification.AlertCriticalError
			MsgBox(message, MsgBoxStyle.Critical, "Error")
		End Sub
		Public Sub Alert(message As String) Implements IUserNotification.Alert
			MsgBox(message)
		End Sub

	End Class

End Namespace