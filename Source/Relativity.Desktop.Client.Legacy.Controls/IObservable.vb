Namespace kCura.Windows.Process.Generic
	Public Interface IObservable(Of T)
		Event OnProcessFatalException(ByVal ex As Exception)
		Event OnProcessEvent(ByVal evt As ProcessEvent(Of T))
		Event OnProcessProgressEvent(ByVal evt As ProcessProgressEvent)
		Event OnProcessComplete(ByVal closeForm As Boolean, ByVal exportFilePath As String, ByVal exportLogs As Boolean)
	End Interface
End Namespace