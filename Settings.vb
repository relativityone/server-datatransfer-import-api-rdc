Namespace kCura.Relativity.DataReaderClient
	Public Class Settings
		Inherits ImportSettingsBase

#Region "Constructors"
		Public Sub New()
			MyBase.New()
			MultiValueDelimiter = CType(";", Char)
			NestedValueDelimiter = CType("\", Char)
		End Sub
#End Region

#Region "Properties"
		''' <summary>
		''' ArtifactTypeId of the destination Relativity dynamic object
		''' </summary>
		Public Property ArtifactTypeId() As Int32

		''' <summary>
		''' Field delimiter to use when writing out the bulk load file. Line
		''' delimiters will be this value plus a line feed.
		''' </summary>
		Public Property BulkLoadFileFieldDelimiter() As String

		''' <summary>
		''' If true, tries to use "Control Number" for the SelectedIdentifierField and
		''' ignores SelectedIdentifierField. If false and SelectedIdentifierField is not
		''' set, will use the default identifier field.
		''' </summary>
		Public Property DisableControlNumberCompatibilityMode() As Boolean

		''' <summary>
		''' Field name that contains ???
		''' </summary>
		Public Property FolderPathSourceFieldName() As String

		''' <summary>
		''' Delimiter to separate multiple values such as two different single-choice field values
		''' </summary>
		Public Property MultiValueDelimiter() As Char

		''' <summary>
		''' Field name that contains a full path and filename to native files
		''' </summary>
		Public Property NativeFilePathSourceFieldName() As String

		''' <summary>
		''' Delimiter to separate nested values such as choices and child choices on a multi-choice field
		''' </summary>
		Public Property NestedValueDelimiter() As Char

		''' <summary>
		''' Field name which contains the unique identifier of a records parent object record
		''' </summary>
		Public Property ParentObjectIdSourceFieldName() As String

		''' <summary>
		''' '
		''' </summary>
		Public Property RowCount() As Int32
#End Region

	End Class
End Namespace