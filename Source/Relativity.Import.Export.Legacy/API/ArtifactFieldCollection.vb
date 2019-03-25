Namespace kCura.WinEDDS.Api
	Public Class ArtifactFieldCollection
		Implements System.Collections.Generic.ICollection(Of ArtifactField)
		Private _orderedList As New System.Collections.Generic.List(Of ArtifactField)
		Private _idLookup As New System.Collections.Hashtable
		Private _nameLookup As New System.Collections.Hashtable
		Private _typeLookup As New System.Collections.Hashtable
		Private _categoryLookup As New System.Collections.Hashtable
		Private _identifierField As ArtifactField
		Private _fileField As ArtifactField
		Private _outOfOrder As Boolean = True


		Private Sub EnsureSortTheList()
			If _outOfOrder Then
				_orderedList.Sort(New ArtifactFieldNameComparer)
				_outOfOrder = False
			End If
		End Sub
#Region " ICollection Virtual Method Implementation "
		Public Sub Add1(item As ArtifactField) Implements System.Collections.Generic.ICollection(Of ArtifactField).Add
			Me.Add(item)
		End Sub

		Public Sub Clear() Implements System.Collections.Generic.ICollection(Of ArtifactField).Clear
			_orderedList.Clear()
			_idLookup.Clear()
			_nameLookup.Clear()
			_typeLookup.Clear()
			_categoryLookup.Clear()
		End Sub

		Public Function Contains(item As ArtifactField) As Boolean Implements System.Collections.Generic.ICollection(Of ArtifactField).Contains
			Return _orderedList.Contains(item)
		End Function

		Public Sub CopyTo(array() As ArtifactField, arrayIndex As Integer) Implements System.Collections.Generic.ICollection(Of ArtifactField).CopyTo
			Me.EnsureSortTheList()
			_orderedList.CopyTo(array, arrayIndex)
		End Sub

		Public ReadOnly Property Count As Integer Implements System.Collections.Generic.ICollection(Of ArtifactField).Count
			Get
				Return _orderedList.Count
			End Get
		End Property

		Public ReadOnly Property IsReadOnly As Boolean Implements System.Collections.Generic.ICollection(Of ArtifactField).IsReadOnly
			Get
				Return False
			End Get
		End Property

		Public Function Remove(item As ArtifactField) As Boolean Implements System.Collections.Generic.ICollection(Of ArtifactField).Remove
			Dim retval As Boolean = _orderedList.Remove(item)
			_outOfOrder = True
			Return retval
		End Function

		Public Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of ArtifactField) Implements System.Collections.Generic.IEnumerable(Of ArtifactField).GetEnumerator
			Me.EnsureSortTheList()
			Return _orderedList.GetEnumerator
		End Function

		Public Function GetEnumerator1() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
			Me.EnsureSortTheList()
			Return _orderedList.GetEnumerator
		End Function
		'Public Sub CopyTo(ByVal array As System.Array, ByVal index As Integer) Implements System.Collections.ICollection.CopyTo
		'	_orderedList.CopyTo(array, index)
		'End Sub

		'Public ReadOnly Property Count() As Integer Implements System.Collections.ICollection.Count
		'	Get
		'		Return _orderedList.Count
		'	End Get
		'End Property

		'Public ReadOnly Property IsSynchronized() As Boolean Implements System.Collections.ICollection.IsSynchronized
		'	Get
		'		Return _orderedList.IsSynchronized
		'	End Get
		'End Property

		'Public ReadOnly Property SyncRoot() As Object Implements System.Collections.ICollection.SyncRoot
		'	Get
		'		Return _orderedList.SyncRoot
		'	End Get
		'End Property

		'Public Function GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
		'	Return _orderedList.GetEnumerator
		'End Function

#End Region

#Region " Accessors "

		Default Public ReadOnly Property Item(ByVal artifactId As Int32) As ArtifactField
			Get
				Dim retval As Object = _idLookup(artifactId)
				If retval Is Nothing Then Return Nothing
				Return DirectCast(retval, ArtifactField)
			End Get
		End Property

		Default Public ReadOnly Property Item(ByVal displayName As String) As ArtifactField
			Get
				Dim retval As Object = _nameLookup(displayName)
				If retval Is Nothing Then Return Nothing
				Return DirectCast(retval, ArtifactField)
			End Get
		End Property

		Public ReadOnly Property IdentifierField() As ArtifactField
			Get
				Return _identifierField
			End Get
		End Property

		Public ReadOnly Property FileField() As ArtifactField
			Get
				Return _fileField
			End Get
		End Property

		Public ReadOnly Property FieldList(ByVal type As Relativity.FieldTypeHelper.FieldType) As ArtifactField()
			Get
				If Not _typeLookup.Contains(type) Then Return New ArtifactField() {}
				Return DirectCast(DirectCast(_typeLookup(type), ArrayList).ToArray(GetType(ArtifactField)), ArtifactField())
			End Get
		End Property

		Public ReadOnly Property FieldList(ByVal category As Relativity.FieldCategory) As ArtifactField()
			Get
				If Not _categoryLookup.Contains(category) Then Return New ArtifactField() {}
				Return DirectCast(DirectCast(_categoryLookup(category), ArrayList).ToArray(GetType(ArtifactField)), ArtifactField())
			End Get
		End Property

#End Region

#Region " Add/Remove "

		Public Sub Add(ByVal field As ArtifactField)
			_orderedList.Add(field)
			_outOfOrder = True
			_idLookup.Add(field.ArtifactID, field)
			_nameLookup.Add(field.DisplayName, field)
			If field.Category = Relativity.FieldCategory.Identifier Then _identifierField = field
			If field.Type = Relativity.FieldTypeHelper.FieldType.File Then _fileField = field
			If Not _categoryLookup.ContainsKey(field.Category) Then _categoryLookup.Add(field.Category, New System.Collections.ArrayList)
			DirectCast(_categoryLookup(field.Category), System.Collections.ArrayList).Add(field)
			If Not _typeLookup.ContainsKey(field.Type) Then _typeLookup.Add(field.Type, New System.Collections.ArrayList)
			DirectCast(_typeLookup(field.Type), System.Collections.ArrayList).Add(field)
		End Sub

		Public Sub AddRange(ByVal fields As IEnumerable)
			For Each field As ArtifactField In fields
				Me.Add(field)
			Next
		End Sub

		Private Class ArtifactFieldNameComparer
			Implements System.Collections.Generic.IComparer(Of ArtifactField)

			Public Function Compare1(x As ArtifactField, y As ArtifactField) As Integer Implements System.Collections.Generic.IComparer(Of ArtifactField).Compare
				Return System.String.Compare(x.DisplayName, y.DisplayName)
			End Function
		End Class
#End Region



	End Class
End Namespace

