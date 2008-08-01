Namespace kCura.Windows.Process
	Public Class Controller
		Public Event HaltProcessEvent(ByVal processID As Guid)
		Public Event ExportServerErrorsEvent(ByVal exportLocation As String)
		Public Event ExportErrorReportEvent(ByVal exportLocation As String)
		Public Event ExportErrorFileEvent(ByVal exportLocation As String)

		Public Sub HaltProcess(ByVal processID As Guid)
			RaiseEvent HaltProcessEvent(processID)
		End Sub

		Public Sub ExportServerErrors(ByVal exportLocation As String)
			RaiseEvent ExportServerErrorsEvent(exportLocation)
		End Sub

		Public Sub ExportErrorReport(ByVal exportLocation As String)
			RaiseEvent ExportErrorReportEvent(exportLocation)
		End Sub

		Public Sub ExportErrorFile(ByVal exportLocation As String)
			RaiseEvent ExportErrorFileEvent(exportLocation)
		End Sub

	End Class
End Namespace