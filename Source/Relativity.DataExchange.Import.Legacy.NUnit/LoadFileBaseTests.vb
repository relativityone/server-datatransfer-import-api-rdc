' -----------------------------------------------------------------------------------------------------
' <copyright file="LoadFileBaseTests.cs" company="Relativity ODA LLC">
'   © Relativity All Rights Reserved.
' </copyright>
' -----------------------------------------------------------------------------------------------------

Imports NUnit.Framework
Imports Relativity.DataExchange.Data

Namespace Relativity.DataExchange.Import.NUnit

	<TestFixture>
	Public Class LoadFileBaseTests

		<Test>
		Public Sub GetFieldStringValue_Throw_Error_Correct_Test()
			Dim value As String = "More Than 5 Chars"
			Dim len As Integer = value.length
			Dim maxLen As Integer = 5
			Dim thrownException = Assert.Throws(Of StringImporterException)(Sub()
				DelimitedFileImporter2.ValidateStringForVarChar(value, 2, maxLen, 10, "Test")
			End Sub)
			Assert.AreEqual("Error in line 10, column ""C"". There was an error attempting to import this record due to the length of a metadata field not being large enough. The maximum length of the ""Test"" field is currently set to " & maxLen & " characters, but the records metadata's length is " & len & " characters. Increase the maximum length of the ""Test"" field to at least " & len & " characters and retry.", thrownException.Message)
		End Sub

		<Test>
		Public Sub GetFieldStringValue_Return_Correct_Value_Not_Empty()
			Dim value As String = "More Than 5 Chars"
			Assert.AreEqual(value, DelimitedFileImporter2.ValidateStringForVarChar(value, 2, 100, 10, "Test"))
		End Sub

		<Test>
		Public Sub GetFieldStringValue_Return_Correct_Value_Empty()
			Dim value As String = ""
			Assert.AreEqual(Nothing, DelimitedFileImporter2.ValidateStringForVarChar(value, 2, 100, 10, "Test"))
		End Sub

	End Class
End Namespace