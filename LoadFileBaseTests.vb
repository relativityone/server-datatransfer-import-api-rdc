Imports NUnit.Framework

Namespace kCura.WinEDDS.NUnit
	<TestFixture()>
 Public Class LoadFileBaseTests

		<Test(), ExpectedException(GetType(kCura.Utility.DelimitedFileImporter.InputStringExceedsFixedLengthException), ExpectedMessage:="Error in line 10, column ""C"".  Input length exceeds maximum set length of 5 for the Test field.")>
  Public Sub GetFieldStringValue_Throw_Error_Correct_Test()
			kCura.Utility.ImprovedDelimitedFileImporter.ValidateStringForVarChar("Morethat5CharsForsureeee", 2, 5, 10, "Test")
  End Sub

		<Test()>
  Public Sub GetFieldStringValue_Return_Correct_Value_Not_Empty()
			Dim value As String = "Morethat5CharsForsureeee"
			Assert.AreEqual(value, kCura.Utility.ImprovedDelimitedFileImporter.ValidateStringForVarChar(value, 2, 100, 10, "Test"))
  End Sub

		<Test()>
  Public Sub GetFieldStringValue_Return_Correct_Value_Empty()
			Dim value As String = ""
			Assert.AreEqual(Nothing, kCura.Utility.ImprovedDelimitedFileImporter.ValidateStringForVarChar(value, 2, 100, 10, "Test"))
  End Sub

 End Class
End NameSpace