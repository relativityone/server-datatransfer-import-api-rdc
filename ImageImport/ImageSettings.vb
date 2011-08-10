Namespace kCura.Relativity.DataReaderClient

	Public Class ImageSettings

#Region " Private Variables "
		Private _dataSourceBatesNumberField As String
		Private _dataSourceDocumentIdentifierField As String
		Private _dataSourceFileLocation As String

		Private _RelativityUsername As String
		Private _RelativityPassword As String
		Private _CaseArtifactID As Int32
		Private _ArtifactTypeId As Int32

		Private _DestinationFolderArtifactID As Int32

		Private _NewLineDelimiter As Char
		Private _MultiValueDelimiter As Char
		Private _NestedValueDelimiter As Char

		Private _OverwriteMode As OverwriteModeEnum

		Private _OverlayIdentifierSourceFieldName As String

		Private _FolderPathSourceFieldName As String

		Private _ParentObjectIdSourceFieldName As String

		Private _ImageFilePathSourceFieldName As String
		Private _NativeFileCopyMode As NativeFileCopyModeEnum
		Private _ExtractedTextFieldContainsFilePath As Boolean
		Private _ExtractedTextEncoding As System.Text.Encoding
		Private _autoNumberImages As Boolean
		Private _productionartifactid As Int32
		Private _copyfilestodocumentrepository As Boolean
		Private _forProduction As Boolean
		Private _identityFieldId As Int32

#End Region

#Region " constructors "
		Public Sub New()
			_MultiValueDelimiter = CType(";", Char)
			_NestedValueDelimiter = CType("\", Char)
			_ExtractedTextFieldContainsFilePath = False
			_OverlayIdentifierSourceFieldName = String.Empty
		End Sub
#End Region

#Region " Properties "
		''' <summary>
		''' Username to log into the destination Relativity instance
		''' </summary>
		Public Property RelativityUsername() As String
			Get
				Return _RelativityUsername
			End Get
			Set(ByVal Value As String)
				_RelativityUsername = Value
			End Set
		End Property

		''' <summary>
		''' Password to log into the destination Relativity instance
		''' </summary>
		Public Property RelativityPassword() As String
			Get
				Return _RelativityPassword
			End Get
			Set(ByVal Value As String)
				_RelativityPassword = Value
			End Set
		End Property

		''' <summary>
		''' ArtifactId of the destination Relativity case
		''' </summary>
		Public Property CaseArtifactId() As Int32
			Get
				Return _CaseArtifactID
			End Get
			Set(ByVal Value As Int32)
				_CaseArtifactID = Value
			End Set
		End Property

		''' <summary>
		''' ArtifactTypeId of the destination Relativity dynamic object
		''' </summary>
		Public Property ArtifactTypeId() As Int32
			Get
				Return _ArtifactTypeId
			End Get
			Set(ByVal Value As Int32)
				_ArtifactTypeId = Value
			End Set
		End Property

		Public Property DestinationFolderArtifactID() As Int32
			Get
				Return _DestinationFolderArtifactID
			End Get
			Set(ByVal Value As Int32)
				_DestinationFolderArtifactID = Value
			End Set
		End Property

		''' <summary>
		''' Delimiter to separate multiple values such as two different single-choice field values
		''' </summary>
		Public Property MultiValueDelimiter() As Char
			Get
				Return _MultiValueDelimiter
			End Get
			Set(ByVal Value As Char)
				_MultiValueDelimiter = Value
			End Set
		End Property

		''' <summary>
		''' Delimiter to separate nested values such as choices and child choices on a multi-choice field
		''' </summary>
		Public Property NestedValueDelimiter() As Char
			Get
				Return _NestedValueDelimiter
			End Get
			Set(ByVal Value As Char)
				_NestedValueDelimiter = Value
			End Set
		End Property

		''' <summary>
		''' Determines if records should be appended or overlayed
		''' </summary>
		Public Property OverwriteMode() As OverwriteModeEnum
			Get
				Return _OverwriteMode
			End Get
			Set(ByVal Value As OverwriteModeEnum)
				_OverwriteMode = Value
			End Set
		End Property

		''' <summary>
		''' Field name to identify matching records when overlaying records
		''' </summary>
		Public Property OverlayIdentifierSourceFieldName() As String
			Get
				Return _OverlayIdentifierSourceFieldName
			End Get
			Set(ByVal Value As String)
				_OverlayIdentifierSourceFieldName = Value
			End Set
		End Property

		''' <summary>
		''' Field name that contains ???
		''' </summary>
		Public Property FolderPathSourceFieldName() As String
			Get
				Return _FolderPathSourceFieldName
			End Get
			Set(ByVal Value As String)
				_FolderPathSourceFieldName = Value
			End Set
		End Property

		''' <summary>
		''' Field name which contains the unique identifier of a records parent object record
		''' </summary>
		Public Property ParentObjectIdSourceFieldName() As String
			Get
				Return _ParentObjectIdSourceFieldName
			End Get
			Set(ByVal Value As String)
				_ParentObjectIdSourceFieldName = Value
			End Set
		End Property

		''' <summary>
		''' Field name that contains a full path and filename to image files
		''' </summary>
		Public Property ImageFilePathSourceFieldName() As String
			Get
				Return _ImageFilePathSourceFieldName
			End Get
			Set(ByVal Value As String)
				_ImageFilePathSourceFieldName = Value
			End Set
		End Property

		''' <summary>
		''' Sets whether native files are copied to the destination Relativity instance or whether they are used as links
		''' </summary>
		Public Property NativeFileCopyMode() As NativeFileCopyModeEnum
			Get
				Return _NativeFileCopyMode
			End Get
			Set(ByVal Value As NativeFileCopyModeEnum)
				_NativeFileCopyMode = Value
			End Set
		End Property

		''' <summary>
		''' Sets whether the "Extracted Text" field contains paths to extracted text files or contains the actual extracted text
		''' </summary>
		Public Property ExtractedTextFieldContainsFilePath() As Boolean
			Get
				Return _ExtractedTextFieldContainsFilePath
			End Get
			Set(ByVal Value As Boolean)
				_ExtractedTextFieldContainsFilePath = Value
			End Set
		End Property

		''' <summary>
		''' Sets the encoding of the extracted text files
		''' </summary>
		Public Property ExtractedTextEncoding() As System.Text.Encoding
			Get
				Return _ExtractedTextEncoding
			End Get
			Set(ByVal Value As System.Text.Encoding)
				_ExtractedTextEncoding = Value
			End Set
		End Property

		''' <summary>
		''' Auto number Images yes/no
		''' </summary>
		Public Property AutoNumberImages() As Boolean
			Get
				Return _autoNumberImages
			End Get
			Set(ByVal Value As Boolean)
				_autoNumberImages = Value
			End Set
		End Property

		''' <summary>
		''' If this is set then images will load into selected production
		''' </summary>
		Public Property ProductionArtifactID() As Int32
			Get
				Return _productionartifactid
			End Get
			Set(ByVal Value As Int32)
				_productionartifactid = Value
			End Set
		End Property

		''' <summary>
		''' Copy file or create links
		''' </summary>
		Public Property CopyFilesToDocumentRepository() As Boolean
			Get
				Return _copyfilestodocumentrepository
			End Get
			Set(ByVal value As Boolean)
				_copyfilestodocumentrepository = value
			End Set
		End Property

		''' <summary>
		''' Set to true if you want the import to go into a production
		''' </summary>
		Public Property ForProduction() As Boolean
			Get
				Return _forProduction
			End Get
			Set(ByVal value As Boolean)
				_forProduction = value
			End Set
		End Property

		''' <summary>
		''' The key field that can only be set on Overwrite only
		''' </summary>
		Public Property IdentityFieldId() As Int32
			Get
				Return _identityFieldId
			End Get
			Set(ByVal value As Int32)
				_identityFieldId = value
			End Set
		End Property

		''' <summary>
		''' Name of the field in the data source corresponding to the BatesNumber field
		''' </summary>
		Public Property BatesNumberField() As String
			Get
				Return _dataSourceBatesNumberField
			End Get
			Set(value As String)
				_dataSourceBatesNumberField = value
			End Set
		End Property

		''' <summary>
		''' Name of the field in the data source corresponding to the DocumentIdentifier field
		''' </summary>
		''' <value></value>
		Public Property DocumentIdentifierField() As String
			Get
				Return _dataSourceDocumentIdentifierField
			End Get
			Set(value As String)
				_dataSourceDocumentIdentifierField = value
			End Set
		End Property

		''' <summary>
		''' Name of the field in the data source corresponding to the FileLocation field
		''' </summary>
		Public Property FileLocationField() As String
			Get
				Return _dataSourceFileLocation
			End Get
			Set(value As String)
				_dataSourceFileLocation = value
			End Set
		End Property
#End Region

	End Class

End Namespace