Namespace kCura.WinEDDS.NUnit.TestObjectFactories
	Public Class QueryFieldFactory
		Private Shared _dt As System.Data.DataTable

		Private Function GenerateRow() As System.Data.DataRow
			If _dt Is Nothing Then
				_dt = New System.Data.DataTable
				_dt.Columns.Add("FieldArtifactID", GetType(Int32))
				_dt.Columns.Add("AvfId", GetType(Int32))
				_dt.Columns.Add("FieldCategoryID", GetType(Int32))
				_dt.Columns.Add("FieldTypeID", GetType(Int32))
				_dt.Columns.Add("ArtifactTypeID", GetType(Int32))
				_dt.Columns.Add("ArtifactTypeTableName", GetType(String))
				_dt.Columns.Add("FieldCodeTypeID", GetType(Int32))
				_dt.Columns.Add("FieldIsArtifactBaseField", GetType(Boolean))
				_dt.Columns.Add("DisplayName", GetType(String))
				_dt.Columns.Add("AvfColumnName", GetType(String))

				_dt.Columns.Add("AvfHeaderName", GetType(String))
				_dt.Columns.Add("FormatString", GetType(String))
				_dt.Columns.Add("AssociativeArtifactTypeID", GetType(Int32))
				_dt.Columns.Add("IsUnicodeEnabled", GetType(Boolean))
				_dt.Columns.Add("AllowHtml", GetType(Boolean))
				_dt.Columns.Add("AllowFieldName", GetType(String))
				_dt.Columns.Add("SourceFieldArtifactID", GetType(Int32))
				_dt.Columns.Add("SourceFieldArtifactTypeTableName", GetType(String))
				_dt.Columns.Add("SourceFieldDisplayName", GetType(String))
				_dt.Columns.Add("SourceFieldArtifactTypeID", GetType(Int32))
				_dt.Columns.Add("ConnectorFieldArtifactID", GetType(Int32))

				_dt.Columns.Add("ConnectorFieldName", GetType(String))
				_dt.Columns.Add("ConnectorFieldCategoryID", GetType(Int32))
				_dt.Columns.Add("ParentFileFieldArtifactID", GetType(Int32))
				_dt.Columns.Add("ParentFileFieldDisplayName", GetType(String))
				_dt.Columns.Add("IsLinked", GetType(Boolean))
				_dt.Columns.Add("ColumnSource", GetType(String))
				_dt.Columns.Add("DataSource", GetType(String))
				_dt.Columns.Add("RelationalTableName", GetType(String))
				_dt.Columns.Add("RelationalTableColumnName", GetType(String))
				_dt.Columns.Add("RelationalTableColumnName2", GetType(String))
				_dt.Columns.Add("EnableDataGrid", GetType(Boolean))
			End If
			Return _dt.NewRow
		End Function

		Public Function GenerateQueryField(
		 ByVal fieldArtifactID As Int32, _
		 ByVal avfId As Int32, _
		 ByVal fieldCategory As Relativity.FieldCategory, _
		 ByVal fieldType As Relativity.FieldTypeHelper.FieldType, _
		 ByVal artifactTypeId As Int32, _
		 ByVal artifactTypeTableName As String, _
		 ByVal fieldCodeTypeID As Int32, _
		 ByVal fieldIsArtifactBaseField As Boolean, _
		 ByVal displayName As String, _
		 ByVal avfColumnName As String, _
		 ByVal avfHeaderName As String, _
		 ByVal formatString As String, _
		 ByVal associativeArtifactTypeID As Int32, _
		 ByVal isUnicodeEnabled As Boolean, _
		 ByVal allowHtml As Boolean, _
		 ByVal allowFieldName As String, _
		 ByVal sourceFieldArtifactID As Int32, _
		 ByVal sourceFieldArtifactTypeTableName As String, _
		 ByVal sourceFieldDisplayName As String, _
		 ByVal sourceFieldArtifactTypeID As Int32, _
		 ByVal connectorFieldArtifactID As Int32, _
		 ByVal connectorFieldName As String, _
		 ByVal connectorFieldCategoryId As Int32, _
		 ByVal parentFileFieldArtifactID As Int32, _
		 ByVal parentFileFieldDisplayName As String, _
		 ByVal isLinked As Boolean, _
		 ByVal columnSource As String, _
		 ByVal dataSource As String, _
		 ByVal relationalTableName As String, _
		 ByVal relationalTableColumnName As String, _
		 ByVal relationalTableColumnName2 As String, _
		 ByVal enableDataGrid As Boolean _
		) As kCura.WinEDDS.ViewFieldInfo
			Dim row As System.Data.DataRow = Me.GenerateRow
			row("FieldArtifactId") = fieldArtifactID
			row("AvfId") = avfId
			row("FieldCategoryID") = CType(fieldCategory, Int32)
			row("FieldTypeID") = CType(fieldType, Int32)
			row("ArtifactTypeID") = artifactTypeId
			row("ArtifactTypeTableName") = artifactTypeTableName
			row("FieldCodeTypeID") = fieldCodeTypeID
			row("FieldIsArtifactBaseField") = fieldIsArtifactBaseField
			row("DisplayName") = displayName
			row("AvfColumnName") = avfColumnName
			row("AvfHeaderName") = avfHeaderName
			row("FormatString") = formatString
			row("AssociativeArtifactTypeID") = associativeArtifactTypeID
			row("IsUnicodeEnabled") = isUnicodeEnabled
			row("AllowHtml") = allowHtml
			row("AllowFieldName") = allowFieldName
			row("SourceFieldArtifactID") = sourceFieldArtifactID
			row("SourceFieldArtifactTypeTableName") = sourceFieldArtifactTypeTableName
			row("SourceFieldDisplayName") = sourceFieldDisplayName
			row("SourceFieldArtifactTypeID") = sourceFieldArtifactTypeID
			row("ConnectorFieldArtifactID") = connectorFieldArtifactID
			row("ConnectorFieldName") = connectorFieldName
			row("ConnectorFieldCategoryID") = connectorFieldCategoryId
			row("ParentFileFieldArtifactID") = parentFileFieldArtifactID
			row("ParentFileFieldDisplayName") = parentFileFieldDisplayName
			row("IsLinked") = isLinked
			row("ColumnSource") = columnSource
			row("DataSource") = dataSource
			row("RelationalTableName") = relationalTableName
			row("RelationalTableColumnName") = relationalTableColumnName
			row("RelationalTableColumnName2") = relationalTableColumnName2
			row("EnableDataGrid") = enableDataGrid
			Return New kCura.WinEDDS.ViewFieldInfo(row)
		End Function

		Public Function GetIdentifierQueryField() As kCura.WinEDDS.ViewFieldInfo
			Return Me.GenerateQueryField(5, 1000186, Relativity.FieldCategory.Identifier, Relativity.FieldTypeHelper.FieldType.Varchar, 10, "Document", -1, False, "Identifier", "Identifier", "Identifier", "", -1, True, False, "", -1, "", "Identifier", -1, -1, "", -1, -1, "", True, "MainTable", "Document", "", "", "", False)
		End Function

		Public Function GetExtractedTextField() As kCura.WinEDDS.ViewFieldInfo
			'BigData_ET_#
			Return Me.GenerateQueryField(6, 1000187, Relativity.FieldCategory.FullText, Relativity.FieldTypeHelper.FieldType.Text, 10, "Document", -1, False, "Extracted Text", "ExtractedText", "Extracted Text", "", -1, True, False, "", -1, "", "Extracted Text", -1, -1, "", -1, -1, "", False, "MainTable", "Document", "", "", "", False)
		End Function

		Public Function GetGenericLongTextField() As kCura.WinEDDS.ViewFieldInfo
			Return Me.GenerateQueryField(6, 1001440, Relativity.FieldCategory.FullText, Relativity.FieldTypeHelper.FieldType.Text, 10, "Document", -1, False, "Long Text", "LongText", "Long Text", "", -1, True, False, "", -1, "", "Long Text", -1, -1, "", -1, -1, "", False, "MainTable", "Document", "", "", "", False)
		End Function

		Public Function GetRenamedGenericLongTextField() As kCura.WinEDDS.ViewFieldInfo
			Return Me.GenerateQueryField(6, 1001440, Relativity.FieldCategory.FullText, Relativity.FieldTypeHelper.FieldType.Text, 10, "Document", -1, False, "OtherLong Text", "OtherLongText", "OtherLong Text", "", -1, True, False, "", -1, "", "OtherLong Text", -1, -1, "", -1, -1, "", False, "MainTable", "Document", "", "", "", False)
		End Function

		Public Function GetSameNameDifferentIdGenericLongTextField() As kCura.WinEDDS.ViewFieldInfo
			Return Me.GenerateQueryField(6, 1441000, Relativity.FieldCategory.FullText, Relativity.FieldTypeHelper.FieldType.Text, 10, "Document", -1, False, "Long Text", "LongText", "Long Text", "", -1, True, False, "", -1, "", "Long Text", -1, -1, "", -1, -1, "", False, "MainTable", "Document", "", "", "", False)
		End Function

		Public Function GetGenericBooleanField() As kCura.WinEDDS.ViewFieldInfo
			Return Me.GenerateQueryField(14497, 1000357, Relativity.FieldCategory.Generic, Relativity.FieldTypeHelper.FieldType.Boolean, 10, "Document", -1, False, "Bool", "Bool", "Bool", "", -1, False, False, "", -1, "", "Bool", -1, -1, "", -1, -1, "", False, "MainTable", "Document", "", "", "", False)
		End Function

		Public Function GetGenericSingleCodeField() As kCura.WinEDDS.ViewFieldInfo
			Return Me.GenerateQueryField(391085, 1001118, Relativity.FieldCategory.Generic, Relativity.FieldTypeHelper.FieldType.Code, 10, "Document", 1000097, False, "Single Choice", "SingleChoice", "Single Choice", "", -1, True, False, "", -1, "", "Single Choice", -1, -1, "", -1, -1, "", False, "MainTable", "Document", "", "", "", False)
		End Function

		Public Function GetGenericMultiCodeField() As kCura.WinEDDS.ViewFieldInfo
			Return Me.GenerateQueryField(391081, 1001117, Relativity.FieldCategory.Generic, Relativity.FieldTypeHelper.FieldType.MultiCode, 10, "Document", 1000096, False, "MCode", "MCode", "MCode", "", -1, False, False, "", -1, "", "MCode", -1, -1, "", -1, -1, "", False, "MainTable", "Document", "", "", "", False)
		End Function

		Public Function GetArtifactIdField() As kCura.WinEDDS.ViewFieldInfo
			Return Me.GenerateQueryField(13090, 1000248, Relativity.FieldCategory.Generic, Relativity.FieldTypeHelper.FieldType.Integer, 10, "Document", -1, True, "Artifact ID", "ArtifactID", "ArtifactID", "", -1, False, False, "", -1, "", "ArtifactID", -1, -1, "", -1, -1, "", False, "MainTable", "Document", "", "", "", False)
		End Function

		Public Function GetGenericNumericField(ByVal type As Relativity.FieldTypeHelper.FieldType) As kCura.WinEDDS.ViewFieldInfo
			Return Me.GenerateQueryField(13090, 1000248, Relativity.FieldCategory.Generic, type, 10, "Document", -1, False, "Decimal", "Decimal", "Decimal", "", -1, False, False, "", -1, "", "Decimal", -1, -1, "", -1, -1, "", False, "MainTable", "Document", "", "", "", False)
		End Function

		Public Function GetGenericUserField() As kCura.WinEDDS.ViewFieldInfo
			Return Me.GenerateQueryField(14490, 1000353, Relativity.FieldCategory.Generic, Relativity.FieldTypeHelper.FieldType.User, 10, "Document", -1, False, "User Field", "UserField", "User Field", "", -1, False, False, "", -1, "", "User Field", -1, -1, "", -1, -1, "", False, "MainTable", "Document", "", "", "", False)
		End Function

		Public Function GetGenericFileField() As kCura.WinEDDS.ViewFieldInfo
			Return Me.GenerateQueryField(393751, 1001134, Relativity.FieldCategory.Generic, Relativity.FieldTypeHelper.FieldType.File, 1000045, "Cmon", -1, False, "Attachment", "AttachmentName", "Attachment", "", -1, False, False, "", -1, "", "Filename", -1, -1, "", -1, 393751, "Attachment", False, "MetadataJoin", "File393751", "", "", "", False)
		End Function

		Public Function GetGenericObjectField() As kCura.WinEDDS.ViewFieldInfo
			Return Me.GenerateQueryField(504869, 1001267, Relativity.FieldCategory.Generic, Relativity.FieldTypeHelper.FieldType.Object, 10, "Document", -1, False, "WSP", "WSP", "WSP", "", 1000047, False, False, "", -1, "WithSpace", "Name", -1, -1, "", -1, -1, "", False, "MainTable", "o1000047_f504869", "", "", "", False)
		End Function

		Public Function GetGenericObjectsField() As kCura.WinEDDS.ViewFieldInfo
			Return Me.GenerateQueryField(505158, 1001278, Relativity.FieldCategory.Generic, Relativity.FieldTypeHelper.FieldType.Objects, 10, "Document", -1, False, "DcMon", "DcMon", "DcMon", "", 1000045, False, False, "", -1, "Cmon", "Name", -1, -1, "Name", -1, -1, "", False, "MainTable", "Document", "f505158f505168", "f505158ArtifactID", "f505168ArtifactID", False)
		End Function

		Public Function GenerateMultiReflectedUserField() As kCura.WinEDDS.ViewFieldInfo
			Return Me.GenerateQueryField(505162, 1001282, Relativity.FieldCategory.MultiReflected, Relativity.FieldTypeHelper.FieldType.User, 10, "Document", -1, False, "DcMon::User", "DcMonUser", "DcMon::User", "", 1000045, False, False, "", 504867, "Cmon", "User", 1000045, 505158, "Name", 0, -1, "", False, "MainTable", "Cmon", "f505158f505168", "f505158ArtifactID", "f505168ArtifactID", False)
		End Function

		Public Function GetGenericDateField() As kCura.WinEDDS.ViewFieldInfo
			Return Me.GenerateQueryField(504869, 1001267, Relativity.FieldCategory.Generic, Relativity.FieldTypeHelper.FieldType.Date, 10, "Document", -1, False, "Date", "Date", "Date", "d", -1, False, False, "", -1, "", "Date", -1, -1, "", -1, -1, "", False, "MainTable", "Document", "", "", "", False)
		End Function

		Public Function GetGenericDateTimeField() As kCura.WinEDDS.ViewFieldInfo
			Return Me.GenerateQueryField(504869, 1001267, Relativity.FieldCategory.Generic, Relativity.FieldTypeHelper.FieldType.Date, 10, "Document", -1, False, "Date", "Date", "Date", "g", -1, False, False, "", -1, "", "Date", -1, -1, "", -1, -1, "", False, "MainTable", "Document", "", "", "", False)
		End Function

		Public Function GetDocumentFolderNameField() As kCura.WinEDDS.ViewFieldInfo
			Return Me.GenerateQueryField(15, 1000196, Relativity.FieldCategory.FolderName, Relativity.FieldTypeHelper.FieldType.Varchar, 10, "Document", -1, False, "Folder Name", "FolderName", "Folder Name", "", -1, False, False, "", -1, "", "Name", -1, -1, "", -1, -1, "", False, "Artifact", "Folder", "", "", "", False)
		End Function

		Public Function GetChildDynamicObjectParentField() As kCura.WinEDDS.ViewFieldInfo
			Return Me.GenerateQueryField(516343, 1001339, Relativity.FieldCategory.ParentArtifact, Relativity.FieldTypeHelper.FieldType.Object, 1000053, "ProjectChild", -1, True, "Project", "Project", "Project", "", 1000052, False, False, "", -1, "Project", "Name", -1, -1, "", -1, -1, "", False, "MainTable", "o1000052_f516343", "", "", "", False)
		End Function

		Public Function GetAllDocumentFields() As ViewFieldInfo()
			Dim retval As New System.Collections.Generic.List(Of ViewFieldInfo)
			retval.Add(GetIdentifierQueryField)
			retval.Add(GetExtractedTextField)
			retval.Add(GetGenericBooleanField)
			retval.Add(GetGenericSingleCodeField)
			retval.Add(GetGenericMultiCodeField)
			retval.Add(GetArtifactIdField)
			retval.Add(GetGenericNumericField(Relativity.FieldTypeHelper.FieldType.Currency))
			retval.Add(GetGenericNumericField(Relativity.FieldTypeHelper.FieldType.Decimal))
			retval.Add(GetGenericNumericField(Relativity.FieldTypeHelper.FieldType.Integer))
			retval.Add(GetGenericUserField)
			retval.Add(GetGenericObjectField)
			retval.Add(GetGenericObjectsField)
			retval.Add(GenerateMultiReflectedUserField)
			retval.Add(GetGenericDateField)
			retval.Add(GetGenericDateTimeField)
			retval.Add(GetDocumentFolderNameField)
			Return retval.ToArray
		End Function
	End Class
End Namespace


