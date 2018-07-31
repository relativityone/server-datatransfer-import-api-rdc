Imports kCura.Utility

Namespace kCura.WinEDDS
	Public Class DefaultTimeKeeperManager
		Implements ITimeKeeperManager

		Private ReadOnly _timekeeper As New Timekeeper

		Public Function CaptureTime(eventKey As String) As IImportTimeKeeper Implements ITimeKeeperManager.CaptureTime
			Return New DefaultTimerKeeper(_timekeeper, eventKey)
		End Function

		Public Sub GenerateCsvReportItemsAsRows(filenameSuffix As String, directory As String) Implements ITimeKeeperManager.GenerateCsvReportItemsAsRows
			_timekeeper.GenerateCsvReportItemsAsRows("_winedds", "C:\")
		End Sub
	End Class

End Namespace
