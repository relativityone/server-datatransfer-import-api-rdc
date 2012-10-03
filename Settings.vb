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

		''' <summary>
		''' To skip file identification in the Import API, set this property to True.
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks>If this value is True, both <see cref="OIFileIdColumnName">OIFileIdColumnName</see> and  <see cref="OIFileIdColumnName">OIFileTypeColumnName</see> must be set.</remarks>
		Public Property OIFileIdMapped As Boolean

		''' <summary>
		''' Set this property to the value that indicates the column in the <see cref="SourceIDataReader.SourceData">SourceData</see> property which contains the OutsideInFileId.
		''' To determine the file id, call kCura.OI.FileID.Manager.Instance.GetFileIDDataByFilePath(string filePath).  
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks>To use this property, <see cref="OIFileIdMapped">OIFileIdMapped</see> must be set to True.</remarks>
		Public Property OIFileIdColumnName As String

		''' <summary>
		''' Set this property to the value that indicates the column in the <see cref="SourceIDataReader.SourceData">SourceData</see> property which contains the OutsideInFileType.
		''' To determine the file type, call kCura.OI.FileID.Manager.Instance.GetFileIDDataByFilePath(string filePath).
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks>To use this property, <see cref="OIFileIdMapped">OIFileIdMapped</see> must be set to True.</remarks>
		Public Property OIFileTypeColumnName As String


		''' <summary>
		''' To skip file size checking in the Import API, set this property to True.
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks>If this value is True, <see cref="FileSizeColumn">OIFileIdColumnNameFileSizeColumn</see>  must be mapped.</remarks>
		Public Property FileSizeMapped As Boolean

		''' <summary>
		''' Set this property to the value that indicates the column in the <see cref="SourceIDataReader.SourceData">SourceData</see> property which contains the FileSize.
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks> To use this property, <see cref="FileSizeMapped">FileSizeMapped</see> must be set to True.</remarks>
		Public Property FileSizeColumn As String
#End Region

	End Class
End Namespace