Imports System.Reflection
Imports kCura.WinEDDS
Imports kCura.WinEDDS.Exporters
Imports NUnit.Framework

<TestFixture> Public Class OriginalFileNameProviderTests
	Private Const _FILE_NAME_FIELD_ARTIFACT_ID As Integer = 534343

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
		Assert.AreEqual(1, requestedFields.Where(function(i) _FILE_NAME_FIELD_ARTIFACT_ID.Equals(i)).Count)
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
		Return New List(Of Integer) From { 1, 2, 7, 3 }
	End Function

	Private Function CreateFieldsListWithFileNameFieldPresent() As List(Of Integer)
			Return New List(Of Integer) From { 1, 2, _FILE_NAME_FIELD_ARTIFACT_ID, 7, 3 }
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
End Class
