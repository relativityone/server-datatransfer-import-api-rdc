Namespace kCura.Windows.Process
	Public Class Controller
		Public Event HaltProcessEvent(ByVal processID As Guid)
		Public Event ExportServerErrorsEvent(ByVal exportLocation As String)

		Public Sub HaltProcess(ByVal processID As Guid)
			RaiseEvent HaltProcessEvent(processID)
		End Sub

		Public Sub ExportServerErrors(ByVal exportLocation As String)
			RaiseEvent ExportServerErrorsEvent(exportLocation)
		End Sub
	End Class
End Namespace