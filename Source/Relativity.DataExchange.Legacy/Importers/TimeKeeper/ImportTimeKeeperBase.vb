Namespace kCura.WinEDDS

	Public MustInherit Class ImportTimeKeeperBase
		Implements IDisposable

		''' <summary>
		''' A method indicates the ending of time capturing. This method will be called during the disposal of the TimeKeeper.
		''' </summary>
		Protected MustOverride Sub FinishCapturing()
		
		''' <summary>
		''' A string representation of event being captured.
		''' </summary>
		Protected ReadOnly EventKey As String

		Private _isDisposed As Boolean

		''' <summary>
		''' Initialize the time capturing process.
		''' </summary>
		''' <param name="eventKey"></param>
		Protected Sub New(eventKey As String)
			Me.EventKey = eventKey
			_isDisposed = False
		End Sub

		Public Sub Dispose() Implements IDisposable.Dispose
			if _isDisposed = False Then
				FinishCapturing()
				_isDisposed = True
			End If
		End Sub

	End Class
End Namespace