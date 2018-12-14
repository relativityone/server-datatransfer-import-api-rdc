Imports kCura.WinEDDS.Exporters
Imports NUnit.Framework

<TestFixture> Public Class ObjectExportInfoTests
	Private Const _IDENTIFIER As String = "AZIPPER003232"
	Private Const _ORIGINAL_FILE_NAME_XSL As String= "originalFile.xsl"
	Private Const _ORIGINAL_FILE_NAME_TXT As String= "originalFile.tXT"
	Private Const _PRODUCTION_BEGIN_BATES As String = "PROD001"
	Private Const _TEXT_EXTENSION As String = ".txt"

	<Test> Public Sub ItShouldReturnIdentifierValueWhenNameAfterIdentifierAndNotTryProduction()
		'Arrange
		Dim sut As ObjectExportInfo = New ObjectExportInfo()
		sut.IdentifierValue = _IDENTIFIER
		sut.OriginalFileName = _ORIGINAL_FILE_NAME_XSL
		sut.ProductionBeginBates = _PRODUCTION_BEGIN_BATES

		'Act
		Dim textFileName As String = sut.FullTextFileName(True, False)

		'Assert
		Dim expectedFileName As String = _IDENTIFIER & _TEXT_EXTENSION
		Assert.AreEqual(expectedFileName, textFileName)
	End Sub

	<Test> Public Sub ItShouldReturnIdentifierValueWhenNameAfterIdentifierAndTryProductionAndProductionBeginBatesIsEmpty()
		'Arrange
		Dim sut As ObjectExportInfo = New ObjectExportInfo()
		sut.IdentifierValue = _IDENTIFIER
		sut.OriginalFileName = _ORIGINAL_FILE_NAME_XSL
		sut.ProductionBeginBates = ""

		'Act
		Dim textFileName As String = sut.FullTextFileName(True, True)

		'Assert
		Dim expectedFileName As String = _IDENTIFIER & _TEXT_EXTENSION
		Assert.AreEqual(expectedFileName, textFileName)
	End Sub

	<Test> Public Sub ItShouldReturnProductionBeginBatesValueWhenNameAfterIdentifierAndTryProductionAndProductionBeginBatesNotEmpty()
		'Arrange
		Dim sut As ObjectExportInfo = New ObjectExportInfo()
		sut.IdentifierValue = _IDENTIFIER
		sut.OriginalFileName = _ORIGINAL_FILE_NAME_XSL
		sut.ProductionBeginBates = _PRODUCTION_BEGIN_BATES

		'Act
		Dim textFileName As String = sut.FullTextFileName(True, True)

		'Assert
		Dim expectedFileName As String = _PRODUCTION_BEGIN_BATES & _TEXT_EXTENSION
		Assert.AreEqual(expectedFileName, textFileName)
	End Sub

	<Test> Public Sub ItShouldReturnIdentifierValueWhenNotNameAfterIdentifierAndTryProductionAndProductionBeginBatesIsEmpty()
		'Arrange
		Dim sut As ObjectExportInfo = New ObjectExportInfo()
		sut.IdentifierValue = _IDENTIFIER
		sut.OriginalFileName = _ORIGINAL_FILE_NAME_XSL
		sut.ProductionBeginBates = ""

		'Act
		Dim textFileName As String = sut.FullTextFileName(False, True)

		'Assert
		Dim expectedFileName As String = _IDENTIFIER & _TEXT_EXTENSION
		Assert.AreEqual(expectedFileName, textFileName)
	End Sub

	<TestCase(_PRODUCTION_BEGIN_BATES)> <TestCase("")> <TestCase(Nothing)> Public Sub ItShouldReturnProductionBeginBatesValueWhenNotNameAfterIdentifierAndNotTryProduction(productionBeginBates As String)
		'Arrange
		Dim sut As ObjectExportInfo = New ObjectExportInfo()
		sut.IdentifierValue = _IDENTIFIER
		sut.OriginalFileName = _ORIGINAL_FILE_NAME_XSL
		sut.ProductionBeginBates = productionBeginBates

		'Act
		Dim textFileName As String = sut.FullTextFileName(False, False)

		'Assert
		Dim expectedFileName As String =If(productionBeginBates, "") &_TEXT_EXTENSION
		Assert.AreEqual(expectedFileName, textFileName)
	End Sub

	<Test> Public Sub ItShouldReturnIdentifierValueWhenNameAfterIdentifierAndNoTryProductionAndIdentifierIsEmpty()
		'Arrange
		Dim sut As ObjectExportInfo = New ObjectExportInfo()
		sut.IdentifierValue = ""
		sut.OriginalFileName = _ORIGINAL_FILE_NAME_XSL
		sut.ProductionBeginBates = _PRODUCTION_BEGIN_BATES

		'Act
		Dim textFileName As String = sut.FullTextFileName(True, False)

		'Assert
		Dim expectedFileName As String = _TEXT_EXTENSION
		Assert.AreEqual(expectedFileName, textFileName)
	End Sub

	<Test> Public Sub ItShouldAppendOriginalFileNameToIdentifierValue()
		'Arrange
		Dim sut As ObjectExportInfo = New ObjectExportInfo()
		sut.IdentifierValue = _IDENTIFIER
		sut.OriginalFileName = _ORIGINAL_FILE_NAME_XSL
		sut.ProductionBeginBates = _PRODUCTION_BEGIN_BATES

		'Act
		Dim textFileName As String = sut.FullTextFileName(True, False, True)

		'Assert
		Dim expectedFileName As String = _IDENTIFIER & "_" & _ORIGINAL_FILE_NAME_XSL & _TEXT_EXTENSION
		Assert.AreEqual(expectedFileName, textFileName)
	End Sub

	<Test> Public Sub ItShouldReplaceInvalidFieldNameCharacters()
		'Arrange
		Const invalidFileName As String = "invali/d"
		Const expectedOriginalFileName As String = "invali_d"
		Dim sut As ObjectExportInfo = New ObjectExportInfo()
		sut.IdentifierValue = _IDENTIFIER
		sut.OriginalFileName = invalidFileName
		sut.ProductionBeginBates = _PRODUCTION_BEGIN_BATES

		'Act
		Dim textFileName As String = sut.FullTextFileName(True, False, True)

		'Assert
		Dim expectedFileName As String = _IDENTIFIER & "_" & expectedOriginalFileName & _TEXT_EXTENSION
		Assert.AreEqual(expectedFileName, textFileName)
	End Sub

	<Test> Public Sub ItShouldAppendOriginalFileNameToProductionBeginBatesValue()
		'Arrange
		Dim sut As ObjectExportInfo = New ObjectExportInfo()
		sut.IdentifierValue = _IDENTIFIER
		sut.OriginalFileName = _ORIGINAL_FILE_NAME_XSL
		sut.ProductionBeginBates = _PRODUCTION_BEGIN_BATES

		'Act
		Dim textFileName As String = sut.FullTextFileName(False, True, True)

		'Assert
		Dim expectedFileName As String = _PRODUCTION_BEGIN_BATES & "_" & _ORIGINAL_FILE_NAME_XSL & _TEXT_EXTENSION
		Assert.AreEqual(expectedFileName, textFileName)
	End Sub

	<Test> Public Sub ItShouldNotAppendDoubleTextExtension()
		'Arrange
		Dim sut As ObjectExportInfo = New ObjectExportInfo()
		sut.IdentifierValue = _IDENTIFIER
		sut.OriginalFileName = _ORIGINAL_FILE_NAME_TXT
		sut.ProductionBeginBates = _PRODUCTION_BEGIN_BATES

		'Act
		Dim textFileName As String = sut.FullTextFileName(True, False, True)

		'Assert
		Dim expectedFileName As String = _IDENTIFIER & "_" & _ORIGINAL_FILE_NAME_TXT
		Assert.AreEqual(expectedFileName, textFileName)
	End Sub

	<Test> Public Sub ItShouldAppendOriginalFileNameForProductionBeginBatesFileNamePropertyWhenFlagIsTrue()
		'Arrange
		Dim sut As ObjectExportInfo = New ObjectExportInfo()
		sut.OriginalFileName = _ORIGINAL_FILE_NAME_TXT
		sut.ProductionBeginBates = _PRODUCTION_BEGIN_BATES
		Dim appendToOriginal As Boolean = True
		Dim tryProductionBegBates As Boolean = False

		'Act
		Dim textFileName As String = sut.ProductionBeginBatesFileName(appendToOriginal, tryProductionBegBates)

		'Assert
		Dim expectedFileName As String = _PRODUCTION_BEGIN_BATES & "_" & _ORIGINAL_FILE_NAME_TXT
		Assert.AreEqual(expectedFileName, textFileName)
	End Sub

	<Test> Public Sub ItShouldNotAppendOriginalFileNameForProductionBeginBatesFileNamePropertyWhenFlagIsFalse()
		'Arrange
		Dim sut As ObjectExportInfo = New ObjectExportInfo()
		sut.OriginalFileName = _ORIGINAL_FILE_NAME_TXT
		sut.ProductionBeginBates = _PRODUCTION_BEGIN_BATES
		Dim appendToOriginal As Boolean = False
		Dim tryProductionBegBates As Boolean = False

		'Act
		Dim textFileName As String = sut.ProductionBeginBatesFileName(appendToOriginal, tryProductionBegBates)

		'Assert
		Dim expectedFileName As String = _PRODUCTION_BEGIN_BATES
		Assert.AreEqual(expectedFileName, textFileName)
	End Sub


End Class
