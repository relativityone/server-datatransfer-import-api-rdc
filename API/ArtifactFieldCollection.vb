Namespace kCura.WinEDDS.Api
	Public Class ArtifactFieldCollection
		Implements ICollection
		Private _orderedList As New System.Collections.ArrayList
		Private _idLookup As New System.Collections.Hashtable
		Private _nameLookup As New System.Collections.Hashtable
		Private _typeLookup As New System.Collections.Hashtable
		Private _categoryLookup As New System.Collections.Hashtable
		Private _identifierField As ArtifactField
		Private _fileField As ArtifactField

#Region " ICollection Virtual Method Implementation "

		Public Sub CopyTo(ByVal array As System.Array, ByVal index As Integer) Implements System.Collections.ICollection.CopyTo
			_orderedList.CopyTo(array, index)
		End Sub

		Public ReadOnly Property Count() As Integer Implements System.Collections.ICollection.Count
			Get
				Return _orderedList.Count
			End Get
		End Property

		Public ReadOnly Property IsSynchronized() As Boolean Implements System.Collections.ICollection.IsSynchronized
			Get
				Return _orderedList.IsSynchronized
			End Get
		End Property

		Public ReadOnly Property SyncRoot() As Object Implements System.Collections.ICollection.SyncRoot
			Get
				Return _orderedList.SyncRoot
			End Get
		End Property

		Public Function GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
			Return _orderedList.GetEnumerator
		End Function

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
			_idLookup.Add(field.ArtifactID, field)
			_nameLookup.Add(field.DisplayName, field)
			If field.Category = Relativity.FieldCategory.Identifier Then _identifierField = field
			If field.Type = Relativity.FieldTypeHelper.FieldType.File Then _fileField = field
			If Not _categoryLookup.ContainsKey(field.Category) Then _categoryLookup.Add(field.Category, New System.Collections.ArrayList)
			DirectCast(_categoryLookup(field.Category), System.Collections.ArrayList).Add(field)
			If Not _typeLookup.ContainsKey(field.Type) Then _typeLookup.Add(field.Type, New System.Collections.ArrayList)
			DirectCast(_typeLookup(field.Type), System.Collections.ArrayList).Add(field)
			_orderedList.Sort(New ArtifactFieldNameComparer)
		End Sub

		Public Sub AddRange(ByVal fields As IEnumerable)
			For Each field As ArtifactField In fields
				Me.Add(field)
			Next
		End Sub

		Private Class ArtifactFieldNameComparer
			Implements System.Collections.IComparer
			Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer Implements System.Collections.IComparer.Compare
				Return System.String.Compare(DirectCast(x, ArtifactField).DisplayName, DirectCast(y, ArtifactField).DisplayName)
			End Function
		End Class
#End Region


	End Class
End Namespace

