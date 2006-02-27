Namespace kCura.WinEDDS
	Public Class SQLImportProcess
    Inherits kCura.Windows.Process.ProcessBase
		Protected WithEvents _sqlImporter As SQLImporter
		Public SQLImportSettings As SQLImportSettings

		Protected Overrides Sub Execute()
			_sqlImporter = New kCura.WinEDDS.SQLImporter(SQLImportSettings)
			_sqlImporter.Import()
		End Sub

		Private Sub _sqlImporter_OnStatusEvent(ByVal msgId As String, ByVal eventMessage As String) Handles _sqlImporter.OnStatusEvent
			MyBase.ProcessObserver.RaiseStatusEvent(msgId, eventMessage)
		End Sub

		Private Sub _sqlImporter_OnWarningEvent(ByVal msgid As String, ByVal eventMessage As String) Handles _sqlImporter.OnWarningEvent
			MyBase.ProcessObserver.RaiseWarningEvent(msgid, eventMessage)
		End Sub

		Private Sub _sqlImporter_OnErrorEvent(ByVal msgId As String, ByVal eventMessage As String) Handles _sqlImporter.OnErrorEvent
			MyBase.ProcessObserver.RaiseErrorEvent(msgId, eventMessage)
		End Sub

		Private Sub _sqlImporter_OnProgressStatus(ByVal totalRecords As Integer, ByVal totalRecordsProcessed As Integer, ByVal totalRecordsProcessedWithWarnings As Integer, ByVal totalRecordsProcessedWithErrors As Integer, ByVal startTime As Date, ByVal endTime As Date) Handles _sqlImporter.OnProgressStatus
			MyBase.ProcessObserver.RaiseProgressEvent(totalRecords, totalRecordsProcessed, totalRecordsProcessedWithWarnings, totalRecordsProcessedWithErrors, startTime, endTime)
		End Sub

	End Class
End Namespace