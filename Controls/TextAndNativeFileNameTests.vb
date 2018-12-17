Imports System.Windows.Forms
Imports kCura.EDDS.WinForm.Forms
Imports NUnit.Framework
Namespace kCura.EDDS.WinForm.Tests
	<TestFixture()> Public Class TextAndNativeFileNameTests
		Inherits TextAndNativeFileNameForm

		Dim _items As ComboBox.ObjectCollection = New ComboBox.ObjectCollection(New ComboBox)
		<OneTimeSetUp()> Public Sub SetUp()
			_items.Add(New FieldSelection("Name1", 10))
			_items.Add(New FieldSelection("Something", 20))
			_items.Add(New FieldSelection("Long Name", 30))
		End Sub

		<TestCase("anything", 0)>
		<TestCase("Something", 1)>
		<TestCase("lon", 2)>
		Public Sub ShouldReturnProperMatchIndex(str As String, index As Integer)
			Dim result As Integer = GetIndexOfBestMatchFromList(_items, str)
			Assert.AreEqual(index, result)
		End Sub

		<Test()>
		Public Sub ShouldThrowExceptionIfItemListIstEmpty()
			Dim itemsList As ComboBox.ObjectCollection = New ComboBox.ObjectCollection(New ComboBox)
			Dim ex As ArgumentException = Assert.Throws(Of ArgumentException)(Sub() GetIndexOfBestMatchFromList(itemsList, ""))
			Assert.AreEqual(ex.Message, "The list cannot be empty!")
		End Sub

		<Test()>
		Public Sub ShouldThrowExceptionIfListIsNull()
			Dim ex As ArgumentException = Assert.Throws(Of ArgumentException)(Sub() GetIndexOfBestMatchFromList(Nothing, ""))
			Assert.AreEqual(ex.Message, "The list is not initialized!")
		End Sub
	End Class
End Namespace