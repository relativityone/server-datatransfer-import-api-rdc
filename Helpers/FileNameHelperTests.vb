Imports NUnit.Framework

<TestFixture> Public Class FileNameHelperTests
	<Test> Public Sub ItShouldAddExtensionWhenIsMissing()
		'Arrage
		Dim fileName As String = "file"
		Dim extension As String = ".html" 

		'Act
		Dim result As String = FileNameHelper.AppendExtensionToFileWhenMissing(fileName, extension)

		'Assert
		Dim expectedFileName As String = fileName & extension
		Assert.AreEqual(expectedFileName, result)
	End Sub

	<Test> Public Sub ItShouldAddExtensionWhenIsOtherExtensionIsPresent()
		'Arrage
		Dim fileName As String = "file.txt"
		Dim extension As String = ".html" 

		'Act
		Dim result As String = FileNameHelper.AppendExtensionToFileWhenMissing(fileName, extension)

		'Assert
		Dim expectedFileName As String = fileName & extension
		Assert.AreEqual(expectedFileName, result)
	End Sub

	<Test> Public Sub ItShouldAddExtensionToEmptyFileName()
		'Arrage
		Dim fileName As String = ""
		Dim extension As String = ".html" 

		'Act
		Dim result As String = FileNameHelper.AppendExtensionToFileWhenMissing(fileName, extension)

		'Assert
		Dim expectedFileName As String = fileName & extension
		Assert.AreEqual(expectedFileName, result)
	End Sub

	<TestCase("index.html")>
	<TestCase("index.HTML")>
	<TestCase("index.hTMl")>
	Public Sub ItShouldNotAddExtensionWhenIsAlreadyPresent(fileName As String)
		'Arrage
		Dim extension As String = ".html" 

		'Act
		Dim result As String = FileNameHelper.AppendExtensionToFileWhenMissing(fileName, extension)

		'Assert
		Assert.AreEqual(fileName, result)
	End Sub

	<Test> Public Sub ItShouldReturnNullWhenFileNameIsNull()
		'Arrage
		Dim fileName As String = Nothing
		Dim extension As String = ".html" 

		'Act
		Dim result As String = FileNameHelper.AppendExtensionToFileWhenMissing(fileName, extension)

		'Assert
		Assert.IsNull(result)
	End Sub

	<Test> Public Sub ItShouldReturnFileNameWhenExtensionIsNull()
		'Arrage
		Dim fileName As String = "filename"
		Dim extension As String = Nothing

		'Act
		Dim result As String = FileNameHelper.AppendExtensionToFileWhenMissing(fileName, extension)

		'Assert
		Dim expectedFileName As String = fileName
		Assert.AreEqual(expectedFileName, result)
	End Sub

	<Test> Public Sub ItShouldReturnFileNameWhenExtensionIsEmpty()
		'Arrage
		Dim fileName As String = "filename"
		Dim extension As String = ""

		'Act
		Dim result As String = FileNameHelper.AppendExtensionToFileWhenMissing(fileName, extension)

		'Assert
		Dim expectedFileName As String = fileName
		Assert.AreEqual(expectedFileName, result)
	End Sub

	<Test> Public Sub ItShouldReturnFileNameWhenExtensionIsWhitespace()
		'Arrage
		Dim fileName As String = "filename"
		Dim extension As String = "   "

		'Act
		Dim result As String = FileNameHelper.AppendExtensionToFileWhenMissing(fileName, extension)

		'Assert
		Dim expectedFileName As String = fileName
		Assert.AreEqual(expectedFileName, result)
	End Sub
End Class
