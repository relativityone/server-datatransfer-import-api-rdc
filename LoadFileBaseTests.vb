Imports NUnit.Framework

Namespace kCura.WinEDDS.NUnit
	<TestFixture()>
	Public Class LoadFileBaseTests

		<Test()>
		Public Sub GetFieldStringValue_Throw_Error_Correct_Test()
			Dim value As String = "More Than 5 Chars"
			Dim len As Integer = value.length
			Dim maxLen As Integer = 5
			Dim thrownException As kCura.Utility.DelimitedFileImporter.InputStringExceedsFixedLengthException = Assert.Throws(Of kCura.Utility.DelimitedFileImporter.InputStringExceedsFixedLengthException)(Sub()
																																																				 kCura.Utility.ImprovedDelimitedFileImporter.ValidateStringForVarChar(value, 2, maxLen, 10, "Test")
																																																			 End Sub)
			Assert.AreEqual("Error in line 10, column ""C"".  The input value from the Test source field has a length of " & len & " character(s). This exceeds the limit for the Test field, which is currently set to " & maxLen & " character(s).", thrownException.Message)
		End Sub

		<Test()>
		Public Sub GetFieldStringValue_Return_Correct_Value_Not_Empty()
			Dim value As String = "More Than 5 Chars"
			Assert.AreEqual(value, kCura.Utility.ImprovedDelimitedFileImporter.ValidateStringForVarChar(value, 2, 100, 10, "Test"))
		End Sub

		<Test()>
		Public Sub GetFieldStringValue_Return_Correct_Value_Empty()
			Dim value As String = ""
			Assert.AreEqual(Nothing, kCura.Utility.ImprovedDelimitedFileImporter.ValidateStringForVarChar(value, 2, 100, 10, "Test"))
		End Sub

	End Class
End Namespace