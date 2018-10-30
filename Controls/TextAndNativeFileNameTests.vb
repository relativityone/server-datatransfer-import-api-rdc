Imports System.Windows.Forms
Imports kCura.EDDS.WinForm.Forms
Imports NUnit.Framework
Namespace kCura.EDDS.WinForm.Tests
	<TestFixture()> Public Class TextAndNativeFileNameTests
		Inherits TextAndNativeFileNameForm

		Dim _items As ComboBox.ObjectCollection
		<OneTimeSetUp()> Public Sub SetUp()
			_items = New ComboBox.ObjectCollection(New ComboBox)
			_items.Add(New FieldSelection("Name1", 10))
			_items.Add(New FieldSelection("Something", 20))
			_items.Add(New FieldSelection("Long Name", 30))
		End Sub
		<Test()> Public Sub ShouldReturnFirstItemIfNoMatch()
			Dim result As Integer = GetIndexOfBestMatchFromList(_items, "anything")
			Assert.AreEqual(0, result)
		End Sub

		<Test()> Public Sub ShouldReturnFirstItemThatMatchesExactly()
			Dim result As Integer = GetIndexOfBestMatchFromList(_items, "Something")
			Assert.AreEqual(1, result)
		End Sub

		<Test()> Public Sub ShouldReturnFirstItemThatMatchesPartialy()
			Dim result As Integer = GetIndexOfBestMatchFromList(_items, "lon")
			Assert.AreEqual(2, result)
		End Sub
	End Class
End Namespace