Imports kCura.Utility

Namespace kCura.WinEDDS

	Public Class DefaultTimerKeeper
		Inherits ImportTimeKeeperBase

		Private ReadOnly _timeKeeper As Timekeeper

		Public Sub New(timeKeeper As Timekeeper, eventKey As String)
			MyBase.New(eventKey)
			_timeKeeper = timeKeeper
		End Sub

		Protected Overrides Sub StartCapturing()
			_timeKeeper.MarkStart(EventKey)
		End Sub

		Protected Overrides Sub FinishCapturing()
			_timeKeeper.MarkEnd(EventKey)
		End Sub

	End Class
End Namespace