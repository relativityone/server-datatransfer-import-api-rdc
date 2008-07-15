Namespace kCura.WinEDDS.ImportHelpers

	Public Class MetaDocQueue
		Implements IEnumerable
		Private _list As System.Collections.ArrayList
		Private _weight As Int64
		Private Shared QUEUE_LENGTH_MAX As Int32 = 100
		Private Shared QUEUE_WEIGHT_MAX As Int64 = 52428800
		Public Sub New()
			_list = New System.Collections.ArrayList
		End Sub

		Public Function GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
			Return _list.GetEnumerator
		End Function

		Public ReadOnly Property Front() As MetaDocument
			Get
				Return DirectCast(_list(0), MetaDocument)
			End Get
		End Property

		Public Sub Push(ByVal mdoc As MetaDocument)
			_weight += mdoc.Size
			_list.Add(mdoc)
		End Sub

		Public ReadOnly Property Weight() As Int64
			Get
				Return _weight
			End Get
		End Property

		Public ReadOnly Property Length() As Int32
			Get
				Return _list.Count
			End Get
		End Property

		Public ReadOnly Property IsFull() As Boolean
			Get
				Return Me.Weight > Me.QUEUE_WEIGHT_MAX OrElse Me.Length > Me.QUEUE_LENGTH_MAX
			End Get
		End Property
		Public ReadOnly Property CanAdd() As Boolean
			Get
				Return Not (Me.Weight > Me.QUEUE_WEIGHT_MAX / 2 OrElse Me.Length > Me.QUEUE_LENGTH_MAX / 2)
			End Get
		End Property

		Public ReadOnly Property Pop() As MetaDocument
			Get
				Dim retval As MetaDocument = Me.Front
				_list.RemoveAt(0)
				_weight -= retval.Size
				Return retval
			End Get
		End Property
	End Class

End Namespace
