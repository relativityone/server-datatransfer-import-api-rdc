Namespace kCura.Relativity.DataReaderClient

	''' <summary>
	'''  Provides settings to manage the import process.
	''' </summary>
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
		''' Indicates the ArtifactTypeId of the destination object. This property is required.
		''' </summary>
		Public Property ArtifactTypeId() As Int32

		''' <summary>
		''' Field delimiter to use when writing out the bulk load file. Line
		''' delimiters will be this value plus a line feed.
		''' </summary>
		Public Property BulkLoadFileFieldDelimiter() As String

		''' <summary>
		''' If True, tries to use "Control Number" for the SelectedIdentifierField and
		''' ignores SelectedIdentifierField. If False and SelectedIdentifierField is not
		''' set, uses the default identifier field.
		''' </summary>
		Public Property DisableControlNumberCompatibilityMode() As Boolean

		''' <summary>
		''' Enables or disables validation of the extracted text file location. 
		''' </summary>
		''' <remarks>Set this property to True if you want validation disabled. If validation is disabled and an extracted text file doesn't exist, the current job will fail. By default, this property is set to False, so validation is enabled.</remarks>
		Public Property DisableExtractedTextFileLocationValidation As Boolean

		''' <summary>
		''' Enables or disables the validation of the native file location for the current import job. 
		''' </summary>
		''' <value>Set this property to True if you want validation disabled. By default, this property is set to False, so validation is enabled.</value>
		Public Property DisableNativeLocationValidation As Boolean?

		''' <summary>
		''' Enables or disables validation of the native file type for the current job. 
		''' </summary>
		''' <remarks>Set this property to True if you want validation disabled. By default, this property is set to False, so validation is enabled. 
		''' Before disabling validation, you may want to confirm that your files are all supported types and that the native file paths in the load files are correct. 
		''' Otherwise, you may encounter the following issues: When file type validation is disabled, you won't receive any warnings about unsupported file types, so these files may be imaged and result in errors. 
		''' Disabled file type validation also causes the application to set the Relativity Native Types field to Unknown Format. 
		''' When file location validation is disabled, the Import API may temporarily stop and then restart if the load file includes paths that do not contain files. 
		''' The Import API will also stop when the load file doesn't include a path for a native file, but it will load any files prior to encountering this issue.</remarks>
		Public Property DisableNativeValidation As Boolean?

		''' <summary>
		''' Indicates the name of a metadata field used to build the folder structure for the workspace.  All folders are built under the Import Destination folder, indicated by the DestinationFolderArtifactID.
		''' </summary>
		Public Property FolderPathSourceFieldName() As String

		''' <summary>
		''' Represents the delimiter used to separate multiple values, such as different single-choice field values.
		''' </summary>
		Public Property MultiValueDelimiter() As Char

		''' <summary>
		''' Indicates the name of the Field that contains the full path and filename for the native files.
		''' </summary>
		Public Property NativeFilePathSourceFieldName() As String

		''' <summary>
		''' Represents the delimiter used to separate nested values, such as choices or child choices on a multi-choice field.
		''' </summary>
		Public Property NestedValueDelimiter() As Char

		Friend Property OnBehalfOfUserToken As String

		''' <summary>
		''' This property is not used and will be unavailable in future releases.
		''' </summary>
		<Obsolete()>
		Public Property RowCount() As Int32

		''' <summary>
		''' To skip file identification, set this property to True.
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks>If this value is True, both <see cref="OIFileIdColumnName">OIFileIdColumnName</see> and  <see cref="OIFileIdColumnName">OIFileTypeColumnName</see> must be set.</remarks>
		Public Property OIFileIdMapped As Boolean

		''' <summary>
		''' Indicates the column that contains the OutsideInFileId on the <see cref="SourceIDataReader.SourceData">SourceData</see> property. Used in conjunction with the 
		'''		<see cref="OIFileIdColumnName">OIFileIdMapped</see> and <see cref="OIFileIdColumnName">FileSizeMapped</see> properties.
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks>If <see cref="OIFileIdMapped">OIFileIdMapped</see> or <see cref="OIFileIdMapped">FileSizeMapped</see> is set to True, set this property to the value that indicates the 
		'''		<see cref="SourceIDataReader.SourceData">SourceData</see> column that contains the OutsideInFileId. To determine the file ID, call the 
		'''		kCura.OI.FileID.Manager.Instance.GetFileIDDataByFilePath() method.</remarks>
		Public Property OIFileIdColumnName As String

		''' <summary>
		''' Indicates the column that contains the OutsideInFileType on the <see cref="SourceIDataReader.SourceData">SourceData</see> property. Used in conjunction with the 
		'''		<see cref="OIFileIdColumnName">OIFileIdMapped</see> property.
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks>If the <see cref="OIFileIdMapped">OIFileIdMapped</see> property is to True, set this property to the value that indicates the 
		'''		<see cref="SourceIDataReader.SourceData">SourceData</see> column that contains the OutsideInFileType. To determine the file type, call the 
		'''		kCura.OI.FileID.Manager.Instance.GetFileIDDataByFilePath() method. </remarks>
		Public Property OIFileTypeColumnName As String

		''' <summary>
		''' To skip file size checking, set this property to True.
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks>If this value is True, <see cref="OIFileIdColumnName">OIFileIdColumnName</see> and <see cref="FileSizeColumn">FileSizeColumn</see> must be mapped.</remarks>
		Public Property FileSizeMapped As Boolean

		''' <summary>
		''' Indicates the column that contains the FileSize on the <see cref="SourceIDataReader.SourceData">SourceData</see> property.
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks> To use this property, <see cref="FileSizeMapped">FileSizeMapped</see> must be set to True.</remarks>
		Public Property FileSizeColumn As String
#End Region

	End Class
End Namespace