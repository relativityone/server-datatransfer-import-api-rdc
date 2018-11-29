Imports System.IO
Imports System.Reflection
Imports kCura.WinEDDS
Imports kCura.WinEDDS.Exporters
Imports NSubstitute
Imports NUnit.Framework

<TestFixture> Public Class OriginalFileNameProviderTests
	Private Const _FILE_NAME_FIELD_ARTIFACT_ID As Integer = 534343
	Private Const _FILE_NAME_COLUMN_NAME As String = "FileName"
	Private _fieldLookupService As IFieldLookupService
	Private _numberOfCallsToWriteWarning As Integer

	<SetUp> Public Sub SetUp()
		_fieldLookupService = Substitute.For(Of IFieldLookupService)()
		_numberOfCallsToWriteWarning = 0
	End Sub

	<Test> Public Sub ItShouldNotAddFileNameFieldWhenRequestedFieldsListIsNothing()
		'Arrange
		Dim exportableField As ViewFieldInfo() = CreateMappableFieldsArrayWithFileName()

		'Act
		Dim isFileNamePresent As Boolean = OriginalFileNameProvider.ExtendFieldRequestByFileNameIfNecessary(exportableField, Nothing)

		'Assert
		Assert.IsFalse(isFileNamePresent)
	End Sub

	<Test> Public Sub ItShouldAddFileNameFieldWhenRequestedFieldsListIsEmpty()
		'Arrange
		Dim requestedFields As List(Of Integer) = New List(Of Integer)()
		Dim exportableField As ViewFieldInfo() = CreateMappableFieldsArrayWithFileName()

		'Act
		Dim isFileNamePresent As Boolean = OriginalFileNameProvider.ExtendFieldRequestByFileNameIfNecessary(exportableField, requestedFields)

		'Assert
		Assert.IsTrue(isFileNamePresent)
		Assert.Contains(_FILE_NAME_FIELD_ARTIFACT_ID, requestedFields)
	End Sub

	<Test> Public Sub ItShouldAddFileNameFieldWhenRequestedFieldsListDoesNotContainFileName()
		'Arrange
		Dim requestedFields As List(Of Integer) = CreateFieldsListWithoutFileNameFieldPresent()
		Dim exportableField As ViewFieldInfo() = CreateMappableFieldsArrayWithFileName()

		'Act
		Dim isFileNamePresent As Boolean = OriginalFileNameProvider.ExtendFieldRequestByFileNameIfNecessary(exportableField, requestedFields)

		'Assert
		Assert.IsTrue(isFileNamePresent)
		Assert.Contains(_FILE_NAME_FIELD_ARTIFACT_ID, requestedFields)
	End Sub

	<Test> Public Sub ItShouldNotAddFileNameFieldWhenRequestedFieldsListContainsFileName()
		'Arrange
		Dim requestedFields As List(Of Integer) = CreateFieldsListWithFileNameFieldPresent()
		Dim exportableField As ViewFieldInfo() = CreateMappableFieldsArrayWithFileName()

		'Act
		Dim isFileNamePresent As Boolean = OriginalFileNameProvider.ExtendFieldRequestByFileNameIfNecessary(exportableField, requestedFields)

		'Assert
		Assert.IsTrue(isFileNamePresent)
		Assert.AreEqual(1, requestedFields.Where(Function(i) _FILE_NAME_FIELD_ARTIFACT_ID.Equals(i)).Count)
	End Sub

	<Test> Public Sub ItShouldNotAddFileNameFieldWhenFileNameFieldIsNotPresent()
		'Arrange
		Dim requestedFields As List(Of Integer) = CreateFieldsListWithoutFileNameFieldPresent()
		Dim exportableField As ViewFieldInfo() = CreateMappableFieldsArrayWithoutFileName()

		'Act
		Dim isFileNamePresent As Boolean = OriginalFileNameProvider.ExtendFieldRequestByFileNameIfNecessary(exportableField, requestedFields)

		'Assert
		Assert.IsFalse(isFileNamePresent)
	End Sub

	<Test> Public Sub ItShouldNotAddFileNameFieldWhenExportableFieldsArrayIsNothing()
		'Arrange
		Dim requestedFields As List(Of Integer) = CreateFieldsListWithFileNameFieldPresent()

		'Act
		Dim isFileNamePresent As Boolean = OriginalFileNameProvider.ExtendFieldRequestByFileNameIfNecessary(Nothing, requestedFields)

		'Assert
		Assert.IsFalse(isFileNamePresent)
		Assert.AreEqual(requestedFields, CreateFieldsListWithFileNameFieldPresent())
	End Sub

	<Test> Public Sub ItShouldReturnFileTableFileNameWhenFileNameFieldIsNotPresent()
		'Arrange
		Dim fileNameFromFileTable As String = "CN0001.txt"
		Dim fileNameProvider As OriginalFileNameProvider = New OriginalFileNameProvider(False, _fieldLookupService, AddressOf WriteWarning)
		Dim metadata As Object() = New Object() {}
		Dim nativeFileData As DataRowView = CreateFileDataRowView(fileNameFromFileTable)
		'Act
		Dim actualFileName As String = fileNameProvider.GetOriginalFileName(metadata, nativeFileData)

		'Assert
		Assert.AreEqual(fileNameFromFileTable, actualFileName)
	End Sub

	<Test> Public Sub ItShouldReturnMetadataFileNameWhenFileNameFieldIsPresentAndValueIsCorrect()
		'Arrange
		Dim fileNameFromFileTable As String = "CN0001.txt"
		Dim fileNameFromDocumentTable As String = "originalFile.html"
		Dim fileNameProvider As OriginalFileNameProvider = New OriginalFileNameProvider(True, _fieldLookupService, AddressOf WriteWarning)
		Dim metadata As Object() = New Object() {fileNameFromDocumentTable}
		_fieldLookupService.GetOrdinalIndex(_FILE_NAME_COLUMN_NAME).Returns(0)
		Dim nativeFileData As DataRowView = CreateFileDataRowView(fileNameFromFileTable)

		'Act
		Dim actualFileName As String = fileNameProvider.GetOriginalFileName(metadata, nativeFileData)

		'Assert
		Assert.AreEqual(fileNameFromDocumentTable, actualFileName)
	End Sub

	<Test> Public Sub ItShouldNotWriteWarningWhenMetadataFileNameIsPresentAndValueIsCorrect()
		'Arrange
		Dim fileNameFromFileTable As String = "CN0001.txt"
		Dim fileNameFromDocumentTable As String = "originalFile.html"
		Dim fileNameProvider As OriginalFileNameProvider = New OriginalFileNameProvider(True, _fieldLookupService, AddressOf WriteWarning)
		Dim metadata As Object() = New Object() {fileNameFromDocumentTable}
		_fieldLookupService.GetOrdinalIndex(_FILE_NAME_COLUMN_NAME).Returns(0)
		Dim nativeFileData As DataRowView = CreateFileDataRowView(fileNameFromFileTable)

		'Act
		Dim actualFileName As String = fileNameProvider.GetOriginalFileName(metadata, nativeFileData)

		'Assert
		Assert.AreEqual(0, _numberOfCallsToWriteWarning)
	End Sub

	<Test> Public Sub ItShouldReturnFileTableFileNameWhenMetadataFileNameIsPresentButFileNameFlagIsFalse()
		'Arrange
		Dim fileNameFromFileTable As String = "CN0001.txt"
		Dim fileNameFromDocumentTable As String = "originalFile.html"
		Dim fileNameProvider As OriginalFileNameProvider = New OriginalFileNameProvider(False, _fieldLookupService, AddressOf WriteWarning)
		Dim metadata As Object() = New Object() {fileNameFromDocumentTable}
		_fieldLookupService.GetOrdinalIndex(_FILE_NAME_COLUMN_NAME).Returns(0)
		Dim nativeFileData As DataRowView = CreateFileDataRowView(fileNameFromFileTable)

		'Act
		Dim actualFileName As String = fileNameProvider.GetOriginalFileName(metadata, nativeFileData)

		'Assert
		Assert.AreEqual(fileNameFromFileTable, actualFileName)
	End Sub

	<Test> Public Sub ItShouldNotWriteWarningWhenMetadataFileNamedIsPresentButFileNameFlagIsFalse()
		'Arrange
		Dim fileNameFromFileTable As String = "CN0001.txt"
		Dim fileNameFromDocumentTable As String = "originalFile.html"
		Dim fileNameProvider As OriginalFileNameProvider = New OriginalFileNameProvider(False, _fieldLookupService, AddressOf WriteWarning)
		Dim metadata As Object() = New Object() {fileNameFromDocumentTable}
		_fieldLookupService.GetOrdinalIndex(_FILE_NAME_COLUMN_NAME).Returns(0)
		Dim nativeFileData As DataRowView = CreateFileDataRowView(fileNameFromFileTable)

		'Act
		Dim actualFileName As String = fileNameProvider.GetOriginalFileName(metadata, nativeFileData)

		'Assert
		Assert.AreEqual(0, _numberOfCallsToWriteWarning)
	End Sub

	<Test> Public Sub ItShouldReturnFileTableFileNameWhenMetadataFileNameIsPresentButEmpty()
		'Arrange
		Dim fileNameFromFileTable As String = "CN0001.txt"
		Dim fileNameFromDocumentTable As String = ""
		Dim fileNameProvider As OriginalFileNameProvider = New OriginalFileNameProvider(True, _fieldLookupService, AddressOf WriteWarning)
		Dim metadata As Object() = New Object() {fileNameFromDocumentTable}
		_fieldLookupService.GetOrdinalIndex(_FILE_NAME_COLUMN_NAME).Returns(0)
		Dim nativeFileData As DataRowView = CreateFileDataRowView(fileNameFromFileTable)

		'Act
		Dim actualFileName As String = fileNameProvider.GetOriginalFileName(metadata, nativeFileData)

		'Assert
		Assert.AreEqual(fileNameFromFileTable, actualFileName)
		Assert.AreEqual(1, _numberOfCallsToWriteWarning)
	End Sub

	<Test> Public Sub ItShouldReturnFileTableFileNameWhenMetadataFileNameIsPresentButNull()
		'Arrange
		Dim fileNameFromFileTable As String = "CN0001.txt"
		Dim fileNameFromDocumentTable As String = Nothing
		Dim fileNameProvider As OriginalFileNameProvider = New OriginalFileNameProvider(True, _fieldLookupService, AddressOf WriteWarning)
		Dim metadata As Object() = New Object() {fileNameFromDocumentTable}
		_fieldLookupService.GetOrdinalIndex(_FILE_NAME_COLUMN_NAME).Returns(0)
		Dim nativeFileData As DataRowView = CreateFileDataRowView(fileNameFromFileTable)

		'Act
		Dim actualFileName As String = fileNameProvider.GetOriginalFileName(metadata, nativeFileData)

		'Assert
		Assert.AreEqual(fileNameFromFileTable, actualFileName)
		Assert.AreEqual(1, _numberOfCallsToWriteWarning)
	End Sub

	<Test> Public Sub ItShouldReturnDocumentFileNameEvenWhenInvalidCharactersArePresent
		For Each invalidCharacter As String In Path.GetInvalidFileNameChars()
			ItShouldReturnDocumentFileNameEvenWhenInvalidCharactersArePresent(invalidCharacter)
		Next
	End Sub

	<Test> Public Sub ItShouldWriteWarningWhenMetadataFileNameIsEmpty()
		'Arrange
		Dim fileNameFromFileTable As String = "CN0001.txt"
		Dim fileNameFromDocumentTable As String = ""
		Dim fileNameProvider As OriginalFileNameProvider = New OriginalFileNameProvider(True, _fieldLookupService, AddressOf WriteWarning)
		Dim metadata As Object() = New Object() {fileNameFromDocumentTable}
		_fieldLookupService.GetOrdinalIndex(_FILE_NAME_COLUMN_NAME).Returns(0)
		Dim nativeFileData As DataRowView = CreateFileDataRowView(fileNameFromFileTable)

		'Act
		fileNameProvider.GetOriginalFileName(metadata, nativeFileData)

		'Assert
		Assert.AreEqual(1, _numberOfCallsToWriteWarning)
	End Sub

	<Test> Public Sub ItShouldWriteWarningForInvalidMetadataFileName()
		'Arrange
		Dim fileNameFromFileTable As String = "CN0001.txt"
		Dim fileNameFromDocumentTable As String = "originalFile.html"
		Dim fileNameProvider As OriginalFileNameProvider = New OriginalFileNameProvider(True, _fieldLookupService, AddressOf WriteWarning)
		Dim validMetadata As Object() = New Object() {fileNameFromDocumentTable}
		Dim invalidMetadata As Object() = New Object() {""}
		_fieldLookupService.GetOrdinalIndex(_FILE_NAME_COLUMN_NAME).Returns(0)
		Dim nativeFileData As DataRowView = CreateFileDataRowView(fileNameFromFileTable)

		'Act
		fileNameProvider.GetOriginalFileName(invalidMetadata, nativeFileData)
		fileNameProvider.GetOriginalFileName(validMetadata, nativeFileData)
		fileNameProvider.GetOriginalFileName(invalidMetadata, nativeFileData)

		'Assert
		Assert.AreEqual(2, _numberOfCallsToWriteWarning)
	End Sub

	<Test> Public Sub ItShouldReturnCorrectFileNameForSequenceOfValidAndInvalidMetadataFileNameValues()
		'Arrange
		Dim fileNameFromFileTable As String = "CN0001.txt"
		Dim fileNameFromDocumentTable As String = "originalFile.html"
		Dim fileNameProvider As OriginalFileNameProvider = New OriginalFileNameProvider(True, _fieldLookupService, AddressOf WriteWarning)
		Dim validMetadata As Object() = New Object() {fileNameFromDocumentTable}
		Dim invalidMetadata As Object() = New Object() {""}
		_fieldLookupService.GetOrdinalIndex(_FILE_NAME_COLUMN_NAME).Returns(0)
		Dim nativeFileData As DataRowView = CreateFileDataRowView(fileNameFromFileTable)

		'Act
		Dim firstResult As String = fileNameProvider.GetOriginalFileName(invalidMetadata, nativeFileData)
		Dim secondResult As String = fileNameProvider.GetOriginalFileName(validMetadata, nativeFileData)
		Dim thirdResult As String = fileNameProvider.GetOriginalFileName(invalidMetadata, nativeFileData)

		'Assert
		Assert.AreEqual(fileNameFromFileTable, firstResult)
		Assert.AreEqual(fileNameFromDocumentTable, secondResult)
		Assert.AreEqual(fileNameFromFileTable, thirdResult)
	End Sub

	Private Sub WriteWarning(text As String)
		_numberOfCallsToWriteWarning += 1
	End Sub

	Private Function CreateMappableFieldsArrayWithoutFileName() As ViewFieldInfo()
		Return New ViewFieldInfo() {
			CreateViewFieldInfo("Id", "Id"),
			CreateViewFieldInfo("Extracted Text", "ExtractedText"),
			CreateViewFieldInfo("File Size", "FileSize")
		}
	End Function

	Private Function CreateMappableFieldsArrayWithFileName() As ViewFieldInfo()
		Return New ViewFieldInfo() {
			CreateViewFieldInfo("Id", "Id"),
			CreateViewFieldInfo("Extracted Text", "ExtractedText"),
			CreateViewFieldInfo("File Name", "FileName"),
			CreateViewFieldInfo("File Size", "FileSize")
		}
	End Function

	Private Function CreateFieldsListWithoutFileNameFieldPresent() As List(Of Integer)
		Return New List(Of Integer) From {1, 2, 7, 3}
	End Function

	Private Function CreateFieldsListWithFileNameFieldPresent() As List(Of Integer)
		Return New List(Of Integer) From {1, 2, _FILE_NAME_FIELD_ARTIFACT_ID, 7, 3}
	End Function

	Private Function CreateViewFieldInfo(displayName As String, avfColumnName As String) As ViewFieldInfo
		Dim dr As DataRow = CreateViewFieldInfoDataRowWithDefaultValues()
		dr("DisplayName") = displayName
		dr("AvfColumnName") = avfColumnName
		dr("AvfId") = _FILE_NAME_FIELD_ARTIFACT_ID
		Return New ViewFieldInfo(dr)
	End Function

	Private Function CreateViewFieldInfoDataRowWithDefaultValues() As DataRow
		Dim properties As PropertyInfo() = GetType(ViewFieldInfo).GetProperties()
		Dim dt As DataTable = New DataTable()
		For Each propertyInfo As PropertyInfo In properties
			dt.Columns.Add(propertyInfo.Name)
		Next
		dt.Columns.Add("FieldCategoryID")
		dt.Columns.Add("SourceFieldDisplayName")
		dt.Columns.Add("FieldTypeID")
		dt.Columns.Add("ConnectorFieldCategoryID")
		dt.Columns.Add("ReflectedFieldArtifactTypeIdentifierColumnName")
		dt.Columns.Add("ReflectedFieldArtifactTypeConnectorFieldName")
		dt.Columns.Add("ReflectedConnectorArtifactTypeIdentifierColumnName")

		Dim dr As DataRow = dt.NewRow()
		For Each p As PropertyInfo In properties
			dr(p.Name) = CreateDefaultValue(p.PropertyType)
		Next
		dr("FieldCategoryID") = 0
		dr("SourceFieldDisplayName") = Nothing
		dr("FieldTypeID") = 0
		dr("ConnectorFieldCategoryID") = 0
		dr("ReflectedFieldArtifactTypeIdentifierColumnName") = String.Empty
		dr("ReflectedFieldArtifactTypeConnectorFieldName") = String.Empty
		dr("ReflectedConnectorArtifactTypeIdentifierColumnName") = String.Empty
		Return dr
	End Function

	Private Function CreateDefaultValue(propertyType As Type) As Object
		If propertyType.IsValueType Then
			Return Activator.CreateInstance(propertyType)
		ElseIf propertyType.IsAssignableFrom(GetType(String)) Then
			Return String.Empty
		Else
			Return Nothing
		End If
	End Function

	Private Function CreateFileDataRowView(fileName As String) As DataRowView
		Dim dt As DataTable = New DataTable()
		dt.Columns.Add("Filename")

		Dim row As DataRow = dt.NewRow()
		row("Filename") = fileName
		dt.Rows.Add(row)

		Dim dv As DataView = dt.DefaultView
		Return dv(0)
	End Function

	Private Sub ItShouldReturnDocumentFileNameEvenWhenInvalidCharactersArePresent(invalidCharacter As String)
		'Arrange
		Dim fileNameFromFileTable As String = "CN0001.txt"
		Dim fileNameFromDocumentTable As String = "file" + invalidCharacter + "name.txt"
		Dim fileNameProvider As OriginalFileNameProvider = New OriginalFileNameProvider(True, _fieldLookupService, AddressOf WriteWarning)
		Dim metadata As Object() = New Object() {fileNameFromDocumentTable}
		_fieldLookupService.GetOrdinalIndex(_FILE_NAME_COLUMN_NAME).Returns(0)
		Dim nativeFileData As DataRowView = CreateFileDataRowView(fileNameFromFileTable)

		'Act
		Dim actualFileName As String = fileNameProvider.GetOriginalFileName(metadata, nativeFileData)

		'Assert
		Assert.AreEqual(fileNameFromDocumentTable, actualFileName)
	End Sub
End Class
