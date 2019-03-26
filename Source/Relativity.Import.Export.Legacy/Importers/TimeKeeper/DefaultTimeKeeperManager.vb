﻿Imports Relativity.Import.Export

Namespace kCura.WinEDDS
	Public Class DefaultTimeKeeperManager
		Implements ITimeKeeperManager

		Private ReadOnly _timekeeper As New Timekeeper

		Public Function CaptureTime(eventKey As String) As ImportTimeKeeperBase Implements ITimeKeeperManager.CaptureTime
			Return New DefaultTimerKeeper(_timekeeper, eventKey)
		End Function

		Public Sub GenerateCsvReportItemsAsRows(filenameSuffix As String, directory As String) Implements ITimeKeeperManager.GenerateCsvReportItemsAsRows
			_timekeeper.GenerateCsvReportItemsAsRows(filenameSuffix, directory)
		End Sub
	End Class

End Namespace
