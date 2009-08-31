Namespace kCura.WinEDDS.Types
	Public Class SingleChoiceCollection
		Implements ICollection
		Private _ht As New System.Collections.Hashtable
		Private _maxOrder As Int32

		Public ReadOnly Property MaxOrder() As Int32
			Get
				Return _maxOrder
			End Get
		End Property

		Default Public ReadOnly Property Item(ByVal name As String) As kCura.EDDS.Types.ChoiceInfo
			Get
				Dim retval As Object = _ht(name.ToLower.Trim)
				If retval Is Nothing Then Return Nothing
				Return DirectCast(retval, kCura.EDDS.Types.ChoiceInfo)
			End Get
		End Property

		Public Sub Add(ByVal value As kCura.EDDS.Types.ChoiceInfo)
			Dim key As String = value.Name.ToLower.Trim
			If Not _ht.ContainsKey(key) Then _ht.Add(key, value)
			_maxOrder = System.Math.Max(value.Order, _maxOrder)
		End Sub

		Public Sub CopyTo(ByVal array As System.Array, ByVal index As Integer) Implements System.Collections.ICollection.CopyTo
			_ht.Values.CopyTo(array, index)
		End Sub

		Public ReadOnly Property Count() As Integer Implements System.Collections.ICollection.Count
			Get
				Return _ht.Count
			End Get
		End Property

		Public ReadOnly Property IsSynchronized() As Boolean Implements System.Collections.ICollection.IsSynchronized
			Get
				Return _ht.Values.IsSynchronized
			End Get
		End Property

		Public ReadOnly Property SyncRoot() As Object Implements System.Collections.ICollection.SyncRoot
			Get
				Return _ht.Values.SyncRoot
			End Get
		End Property

		Public Function GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
			Return _ht.Values.GetEnumerator
		End Function
	End Class
End Namespace
