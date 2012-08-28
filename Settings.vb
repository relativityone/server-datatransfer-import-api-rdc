Namespace kCura.Relativity.DataReaderClient
	Public Class Settings
		Inherits ImportSettingsBase

#Region "Constructors"
		Friend Sub New()
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
		''' If true, will not check to see if the specified extracted text file exists.
		''' </summary>
		Public Property DisableExtractedTextFileLocationValidation As Boolean

		''' <summary>
		''' Enables or disables native location validation for the current job
		''' </summary>
		''' <value>True: validation is disabled
		''' False: validation is enabled
		''' Nothing: validation will use the pre-configured value</value>
		Public Property DisableNativeLocationValidation As Boolean?

		''' <summary>
		''' Enables or disables native validation for the current job
		''' </summary>
		''' <value>True: validation is disabled
		''' False: validation is enabled
		''' Nothing: validation will use the pre-configured value</value>
		Public Property DisableNativeValidation As Boolean?

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

		Friend Property OnBehalfOfUserToken As String

		'TODO: This shouldn't be used as a progress indicator in ImportBulkArtifactJob
		' -Phil S. 10/04/11
		''' <summary>
		''' '
		''' </summary>
		Public Property RowCount() As Int32
#End Region

	End Class
End Namespace