Namespace kCura.Windows.Process
	Public Class Controller
		Public Event HaltProcessEvent(ByVal processID As Guid)
		Public Sub HaltProcess(ByVal processID As Guid)
			RaiseEvent HaltProcessEvent(processID)
		End Sub
	End Class
End Namespace