Namespace kCura.WinEDDS
	Public Class ClientSideErrorCollection
		Private _backingStore As New System.Collections.Generic.Dictionary(Of Int64, System.Collections.Generic.List(Of String))
		Public Sub AddError(ByVal lineNumber As Int64, ByVal [error] As System.Exception)
			If Not _backingStore.ContainsKey(lineNumber) Then _backingStore.Add(lineNumber, New System.Collections.Generic.List(Of String))
			_backingStore(lineNumber).Add([error].Message)
		End Sub

		Public Sub Clear()
			_backingStore.Clear()
		End Sub

		Default Public ReadOnly Property ErrorsAt(ByVal lineNumber As Int64) As System.Collections.Generic.List(Of String)
			Get
				Dim retval As New System.Collections.Generic.List(Of String)
				If _backingStore.ContainsKey(lineNumber) Then retval = _backingStore(lineNumber)
				Return retval
			End Get
		End Property

		Public ReadOnly Property ContainsLine(ByVal lineNumber As Int64) As Boolean
			Get
				Return _backingStore.ContainsKey(lineNumber)
			End Get
		End Property
	End Class
End Namespace
