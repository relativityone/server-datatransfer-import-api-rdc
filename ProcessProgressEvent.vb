Namespace kCura.Windows.Process
	Public Class ProcessProgressEvent

		Public StartTime As DateTime
		Public EndTime As DateTime

		Public TotalRecords As Int32
		Public TotalRecordsProcessed As Int32
		Public TotalRecordsProcessedWithWarnings As Int32
		Public TotalRecordsProcessedWithErrors As Int32

		Public Sub New(ByVal totRecs As Int32, ByVal totRecsProc As Int32, ByVal totRecsProcWarn As Int32, ByVal totRecsProcErr As Int32, ByVal sTime As DateTime, ByVal eTime As DateTime)
			Me.TotalRecords = totRecs
			Me.TotalRecordsProcessed = totRecsProc
			Me.TotalRecordsProcessedWithWarnings = totRecsProcWarn
			Me.TotalRecordsProcessedWithErrors = totRecsProcErr
			Me.StartTime = sTime
			Me.EndTime = eTime
		End Sub

	End Class
End Namespace