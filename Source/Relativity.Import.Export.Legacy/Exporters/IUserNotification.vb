Namespace kCura.WinEDDS.Exporters
	Public Interface IUserNotification
		Sub Alert(message As String)
		Sub AlertCriticalError(message As String)
		''' <summary>
		'''	Used to wrap an "OK/Cancel" workflow; Implementers should return TRUE if the process is to continue, FALSE if not
		''' </summary>
		''' <param name="message">the message to display to the user to get input</param>
		''' <returns>True if the process is to continue, false if it doesn't</returns>
		Function AlertWarningSkippable(message As String) As Boolean
	End Interface
End Namespace
