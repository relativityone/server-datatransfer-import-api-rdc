Imports kCura.Utility
Imports NUnit.Framework
Namespace kCura.WinEDDS.NUnit
	<TestFixture()> _
	Public Class DocumentField
		<Test()> Public Sub Constructors()
			Dim record As New kCura.WinEDDS.NUnit.TestMethodRecord

      Dim df1 As New kCura.WinEDDS.DocumentField("Name 1", 1, 2, 3, New NullableTypes.NullableInt32(4), NullableTypes.NullableInt32.Null)
			With df1
				record.TestDescription = "New(MemberValues) - FieldName"
				record.RunTextCompareTest("Name 1", .FieldName)

				record.TestDescription = "New(MemberValues) - FieldID"
				record.RunIntegerCompareTest(1, .FieldID)

				record.TestDescription = "New(MemberValues) - FieldTypeID"
				record.RunIntegerCompareTest(2, .FieldTypeID)

				record.TestDescription = "New(MemberValues) - FieldCategoryID"
				record.RunIntegerCompareTest(3, .FieldCategoryID)

				record.TestDescription = "New(MemberValues) - CodeTypeID"
				record.RunIntegerCompareTest(4, .CodeTypeID.Value)
			End With
			Dim df2 As New kCura.WinEDDS.DocumentField(df1)
			With df2
				record.TestDescription = "New(DocumentField) - FieldName"
				record.RunTextCompareTest("Name 1", .FieldName)

				record.TestDescription = "New(DocumentField) - FieldID"
				record.RunIntegerCompareTest(1, .FieldID)

				record.TestDescription = "New(DocumentField) - FieldTypeID"
				record.RunIntegerCompareTest(2, .FieldTypeID)

				record.TestDescription = "New(DocumentField) - FieldCategoryID"
				record.RunIntegerCompareTest(3, .FieldCategoryID)

				record.TestDescription = "New(DocumentField) - CodeTypeID"
				record.RunIntegerCompareTest(4, .CodeTypeID.Value)

			End With
		End Sub

	End Class
End Namespace
