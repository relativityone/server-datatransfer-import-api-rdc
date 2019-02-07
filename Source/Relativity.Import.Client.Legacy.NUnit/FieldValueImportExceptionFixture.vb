Imports NUnit.Framework

<TestFixture()>
Public Class FieldValueImportExceptionFixture
	<SetUp()>
	Public Sub Setup()
		_ex = New kCura.WinEDDS.Exceptions.FieldValueImportException(5, "TestField", "More Stuff")
	End Sub

	<Test()>
	Public Sub TestFieldValueImportExceptionBase()
		Assert.IsTrue(TypeOf New kCura.WinEDDS.Exceptions.FieldValueImportException() Is kCura.Utility.ImporterExceptionBase)
	End Sub

	<Test()>
	Public Sub MessageContainsFieldNameIfProvidedInConstructor()
		Assert.AreEqual("Error in row 5, field ""TestField"".  More Stuff", _ex.Message)
	End Sub

	<Test()>
	Public Sub InnerExceptionIsNullIfNotProvided()
		Assert.IsNull(_ex.InnerException)
	End Sub

	<Test()>
	Public Sub FieldNameComesFromConstructor()
		Assert.AreEqual("TestField", _ex.FieldName)
	End Sub

	<Test()>
	Public Sub RowNumberComesFromConstructor()
		Assert.AreEqual(5, _ex.RowNumber)
	End Sub

	<Test()>
	Public Sub MessageContainsFieldNameAndInnerExceptionIfProvidedInConstructor()
		Dim inner As New Exception
		_ex = New kCura.WinEDDS.Exceptions.FieldValueImportException(inner, 6, "TestField", "Even More")
		Assert.AreSame(inner, _ex.InnerException)
	End Sub

	Private _ex As kCura.WinEDDS.Exceptions.FieldValueImportException
End Class
