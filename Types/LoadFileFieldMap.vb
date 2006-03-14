Namespace kCura.WinEDDS
	<Serializable()> Public Class LoadFileFieldMap
		Implements IEnumerable
		Private _al As ArrayList

		Default Public ReadOnly Property Item(ByVal index As Int32) As LoadFileFieldMapItem
			Get
				Return DirectCast(_al(index), LoadFileFieldMapItem)
			End Get
		End Property

		Public ReadOnly Property DocumentFields() As DocumentField()
			Get
				Dim retval(_al.Count - 1) As DocumentField
				Dim i As Int32
				For i = 0 To _al.Count - 1
					retval(i) = DirectCast(_al(i), LoadFileFieldMapItem).DocumentField
				Next
				Return retval
			End Get
		End Property

		Public ReadOnly Property Count() As Int32
			Get
				Return _al.Count
			End Get
		End Property

		Public Sub Add(ByVal item As LoadFileFieldMapItem)
			_al.Add(item)
		End Sub

		Public Sub AddAt(ByVal item As LoadFileFieldMapItem, ByVal index As Int32)
			_al.Insert(index, item)
		End Sub

		Public Function ToArray() As LoadFileFieldMapItem()
			Return DirectCast(_al.ToArray(GetType(LoadFileFieldMapItem)), LoadFileFieldMapItem())
		End Function

		Public Function GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
			Return _al.GetEnumerator
		End Function

		Public Sub New()
			_al = New ArrayList
		End Sub

#Region "Item"
		<Serializable()> Public Class LoadFileFieldMapItem
			Private _documentField As kCura.WinEDDS.DocumentField
			Private _nativeFileColumnIndex As Int32


			Public Property DocumentField() As kCura.WinEDDS.DocumentField
				Get
					Return _documentField
				End Get
				Set(ByVal value As kCura.WinEDDS.DocumentField)
					_documentField = value
				End Set
			End Property

			Public Property NativeFileColumnIndex() As Int32
				Get
					Return _nativeFileColumnIndex
				End Get
				Set(ByVal value As Int32)
					_nativeFileColumnIndex = value
				End Set
			End Property
			Public Sub New(ByVal docField As DocumentField, ByVal columnIndex As Int32)
				Me.DocumentField = docField
				Me.NativeFileColumnIndex = columnIndex
			End Sub
		End Class
#End Region

	End Class
End Namespace