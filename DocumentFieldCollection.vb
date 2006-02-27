Imports NullableTypes
Imports NUnit.Framework
Namespace kCura.WinEDDS.NUnit
	<TestFixture()> Public Class DocumentFieldCollection
		Private Helper As New Helper

		<Test()> Public Sub Names()
			Dim record As New TestMethodRecord
			Dim dfc As kCura.WinEDDS.DocumentFieldCollection = Helper.GenerateTestDocFieldCollection
			Dim names As String() = dfc.Names
			record.TestDescription = "Names - 0"
			record.RunTextCompareTest("A", names(0))
			record.TestDescription = "Names - 1"
			record.RunTextCompareTest("B", names(1))
			record.TestDescription = "Names - 2"
			record.RunTextCompareTest("C", names(2))
			record.TestDescription = "Names - 3"
			record.RunTextCompareTest("D", names(3))
			record.TestDescription = "Names - 4"
			record.RunTextCompareTest("E", names(4))
		End Sub

		<Test()> Public Sub ConstructionAndAggregation()
			Dim record As New TestMethodRecord
			Dim dfc As kCura.WinEDDS.DocumentFieldCollection = Helper.GenerateEmptyDocFieldCollection
			record.TestDescription = "New"
			record.RunIntegerCompareTest(0, dfc.Count)
			record.TestDescription = "Add - count"
      Dim expected As New kCura.WinEDDS.DocumentField("a", 1, 1, 1, New NullableTypes.NullableInt32(1), NullableInt32.Null)
      dfc.Add(New kCura.WinEDDS.DocumentField("a", 1, 1, 1, New NullableTypes.NullableInt32(1), NullableInt32.Null))
			record.RunIntegerCompareTest(dfc.Count, 1)
			record.TestDescription = "Item - text index"
			record.RunDocumentFieldCompareTest(expected, dfc.Item("a"))
			record.TestDescription = "Item - integer index"
			record.RunDocumentFieldCompareTest(expected, dfc.Item(1))

			dfc = New kCura.WinEDDS.DocumentFieldCollection
			dfc.AddRange(Helper.GenerateTestDocFieldArray)
			record.TestDescription = "AddRange - Count"
			record.RunIntegerCompareTest(5, dfc.Count)

			expected = Helper.GetA1DocField
			record.TestDescription = "AddRange - text index A"
			record.RunDocumentFieldCompareTest(expected, dfc.Item("A"))
			record.TestDescription = "AddRange - integer index 1"
			record.RunDocumentFieldCompareTest(expected, dfc.Item(1))
			expected = Helper.GetB2DocField
			record.TestDescription = "AddRange - text index B"
			record.RunDocumentFieldCompareTest(expected, dfc.Item("B"))
			record.TestDescription = "AddRange - integer index 2"
			record.RunDocumentFieldCompareTest(expected, dfc.Item(2))
			expected = Helper.GetC3DocField
			record.TestDescription = "AddRange - text index C"
			record.RunDocumentFieldCompareTest(expected, dfc.Item("C"))
			record.TestDescription = "AddRange - integer index 3"
			record.RunDocumentFieldCompareTest(expected, dfc.Item(3))
			expected = Helper.GetD4DocField
			record.TestDescription = "AddRange - text index D"
			record.RunDocumentFieldCompareTest(expected, dfc.Item("D"))
			record.TestDescription = "AddRange - integer index 4"
			record.RunDocumentFieldCompareTest(expected, dfc.Item(4))
			expected = Helper.GetE5DocField
			record.TestDescription = "AddRange - text index E"
			record.RunDocumentFieldCompareTest(expected, dfc.Item("E"))
			record.TestDescription = "AddRange - integer index 5"
			record.RunDocumentFieldCompareTest(expected, dfc.Item(5))
		End Sub

		<Test()> _
		Public Sub IdentifierFieldsAndNames()
			Dim record As New TestMethodRecord
			Dim dfc As kCura.WinEDDS.DocumentFieldCollection = Helper.GenerateTestDocFieldCollection
			Dim dfs As kCura.WinEDDS.DocumentField() = dfc.IdentifierFields
			record.TestDescription = "Identifier Fields - count"
			record.RunIntegerCompareTest(1, dfs.Length)
			record.TestDescription = "Identifier Fields - content"
      record.RunDocumentFieldCompareTest(dfs(0), New kCura.WinEDDS.DocumentField("B", 2, 2, 2, NullableInt32.Null, NullableInt32.Null))
      Dim secondDFCID As New kCura.WinEDDS.DocumentField("B1", 6, 0, 2, New NullableInt32(2), NullableInt32.Null)
			dfc.Add(secondDFCID)
			dfs = dfc.IdentifierFields
			record.TestDescription = "Identifier Fields - count"
			record.RunIntegerCompareTest(2, dfs.Length)
			record.TestDescription = "Identifier Fields - content1"
			record.RunDocumentFieldCompareTest(dfs(0), secondDFCID)
			record.TestDescription = "Identifier Fields - content2"
      record.RunDocumentFieldCompareTest(dfs(1), New kCura.WinEDDS.DocumentField("B", 2, 2, 2, NullableInt32.Null, NullableInt32.Null))
			Dim names As String() = dfc.IdentifierFieldNames
			record.TestDescription = "Identifier Field Names 0"
			record.RunTextCompareTest("B", names(0))
			record.TestDescription = "Identifier Field Names 1"
			record.RunTextCompareTest("B1", names(1))
		End Sub

		<Test()> _
		Public Sub Exists()
			Dim record As New TestMethodRecord
			Dim dfc As kCura.WinEDDS.DocumentFieldCollection = Helper.GenerateTestDocFieldCollection
			record.TestDescription = "Exists by ID - True"
			record.RunTest(dfc.Exists(1))
			record.TestDescription = "Exists by ID - False"
			record.RunTest(Not dfc.Exists(666))
			record.TestDescription = "Exists by String - True"
			record.RunTest(dfc.Exists("A"))
			record.TestDescription = "Exists by String - False"
			record.RunTest(Not dfc.Exists("Goat-Horned Lord"))
		End Sub

	End Class
End Namespace
