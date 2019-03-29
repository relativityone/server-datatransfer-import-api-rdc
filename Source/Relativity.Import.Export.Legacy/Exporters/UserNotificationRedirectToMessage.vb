Namespace kCura.WinEDDS.Exporters

	Public Class EventBackedUserNotification
		Implements IUserNotification
		Private _exporter As Exporter
		Public Sub New(e As Exporter)
			If e Is Nothing Then Throw New ArgumentNullException(NameOf(e), $"Cannot intitialize an {NameOf(EventBackedUserNotification)} instance with a null {NameOf(Exporter)}")
			_exporter = e
		End Sub
		Public Sub Alert(message As String) Implements IUserNotification.Alert
			_exporter.WriteWarning(message)
		End Sub

		Public Sub AlertCriticalError(message As String) Implements IUserNotification.AlertCriticalError
			_exporter.WriteFatalError(message, New System.Exception(message))
		End Sub

		Public Function AlertWarningSkippable(message As String) As Boolean Implements IUserNotification.AlertWarningSkippable
			_exporter.WriteWarning(message)
			Return True
		End Function
	End Class
End Namespace
