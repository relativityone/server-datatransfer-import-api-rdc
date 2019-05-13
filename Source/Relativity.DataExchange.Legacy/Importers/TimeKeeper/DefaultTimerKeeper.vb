Imports Relativity.DataExchange

Namespace kCura.WinEDDS

	Public Class DefaultTimerKeeper
		Inherits ImportTimeKeeperBase

		Private ReadOnly _timeKeeper As Timekeeper2

		Public Sub New(timeKeeper As Timekeeper2, eventKey As String)
			MyBase.New(eventKey)
			_timeKeeper = timeKeeper
			_timeKeeper.MarkStart(EventKey)
		End Sub

		Protected Overrides Sub FinishCapturing()
			_timeKeeper.MarkEnd(EventKey)
		End Sub

	End Class
End Namespace