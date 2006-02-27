Imports NullableTypes
Namespace kCura.WinEDDS.NUnit
	Public Class Helper

		Public Function GenerateEmptyDocFieldCollection() As kCura.WinEDDS.DocumentFieldCollection
			Return New kCura.WinEDDS.DocumentFieldCollection
		End Function

		Public Function GenerateTestDocFieldCollection() As kCura.WinEDDS.DocumentFieldCollection
			Dim dfc As New kCura.WinEDDS.DocumentFieldCollection
      dfc.Add(New kCura.WinEDDS.DocumentField("A", 1, 1, 1, NullableInt32.Null, NullableInt32.Null))
      dfc.Add(New kCura.WinEDDS.DocumentField("B", 2, 2, 2, NullableInt32.Null, NullableInt32.Null))
      dfc.Add(New kCura.WinEDDS.DocumentField("C", 3, 3, 3, NullableInt32.Null, NullableInt32.Null))
      dfc.Add(New kCura.WinEDDS.DocumentField("D", 4, 4, 4, NullableInt32.Null, NullableInt32.Null))
      dfc.Add(New kCura.WinEDDS.DocumentField("E", 5, 5, 5, NullableInt32.Null, NullableInt32.Null))
			Return dfc
		End Function

		Public Function GenerateTestDocFieldArray() As kCura.WinEDDS.DocumentField()
      Dim retval As kCura.WinEDDS.DocumentField() = { _
      New kCura.WinEDDS.DocumentField("A", 1, 1, 1, NullableInt32.Null, NullableInt32.Null), _
      New kCura.WinEDDS.DocumentField("B", 2, 2, 2, NullableInt32.Null, NullableInt32.Null), _
      New kCura.WinEDDS.DocumentField("C", 3, 3, 3, NullableInt32.Null, NullableInt32.Null), _
      New kCura.WinEDDS.DocumentField("D", 4, 4, 4, NullableInt32.Null, NullableInt32.Null), _
      New kCura.WinEDDS.DocumentField("E", 5, 5, 5, NullableInt32.Null, NullableInt32.Null)}
			Return retval
		End Function

		Public Function GetA1DocField() As kCura.WinEDDS.DocumentField
      Return New kCura.WinEDDS.DocumentField("A", 1, 1, 1, NullableInt32.Null, NullableInt32.Null)
		End Function
		Public Function GetB2DocField() As kCura.WinEDDS.DocumentField
      Return New kCura.WinEDDS.DocumentField("B", 2, 2, 2, NullableInt32.Null, NullableInt32.Null)
		End Function
		Public Function GetC3DocField() As kCura.WinEDDS.DocumentField
      Return New kCura.WinEDDS.DocumentField("C", 3, 3, 3, NullableInt32.Null, NullableInt32.Null)
		End Function
		Public Function GetD4DocField() As kCura.WinEDDS.DocumentField
      Return New kCura.WinEDDS.DocumentField("D", 4, 4, 4, NullableInt32.Null, NullableInt32.Null)
		End Function
		Public Function GetE5DocField() As kCura.WinEDDS.DocumentField
      Return New kCura.WinEDDS.DocumentField("E", 5, 5, 5, NullableInt32.Null, NullableInt32.Null)
		End Function

	End Class
End Namespace

