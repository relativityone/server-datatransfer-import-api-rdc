Imports kCura.Utility

Namespace kCura.WinEDDS

	Public Class DefaultTimerKeeper
		Implements IImportTimeKeeper

		Private _isDisposed As Boolean
		Private ReadOnly _eventKey As String
		Private ReadOnly _timeKeeper As Timekeeper

		Public Sub New(timeKeeper As Timekeeper, eventKey As String)
			_eventKey = eventKey
			_timeKeeper = timeKeeper
			_isDisposed = False

			_timeKeeper.MarkStart(eventKey)
		End Sub

		Public Sub Dispose() Implements IDisposable.Dispose
			If(Not _isDisposed)
				_timeKeeper.MarkEnd(_eventKey)
				_isDisposed = True
			End If
		End Sub

	End Class
End Namespace