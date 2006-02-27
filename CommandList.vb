Namespace kCura.CommandLine
	Public Class CommandList
		Implements System.Collections.ICollection
		Dim _commandList As System.Collections.ArrayList

		Public Sub New()
			_commandList = New System.Collections.ArrayList
		End Sub

		Public Sub Add(ByVal command As kCura.CommandLine.Command)
			_commandList.Add(command)
		End Sub

		Default Public Property Index(ByVal listIndex As Int32) As kCura.CommandLine.Command
			Get
				Return CType(_commandList(listIndex), kCura.CommandLine.Command)
			End Get
			Set(ByVal value As kCura.CommandLine.Command)
				_commandList(listIndex) = value
			End Set
		End Property

		Public ReadOnly Property Count() As Integer Implements System.Collections.ICollection.Count
			Get
				Return _commandList.Count()
			End Get
		End Property

		Public ReadOnly Property IsSynchronized() As Boolean Implements System.Collections.ICollection.IsSynchronized
			Get
				Return _commandList.IsSynchronized
			End Get
		End Property

		Public ReadOnly Property SyncRoot() As Object Implements System.Collections.ICollection.SyncRoot
			Get
				Return _commandList.SyncRoot
			End Get
		End Property

		Public Function GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
			Return _commandList.GetEnumerator
		End Function

		Public Sub CopyTo(ByVal array As System.Array, ByVal index As Integer) Implements System.Collections.ICollection.CopyTo
			_commandList.CopyTo(array, index)
		End Sub

	End Class
End Namespace
