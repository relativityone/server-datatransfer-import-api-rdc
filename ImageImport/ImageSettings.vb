Namespace kCura.Relativity.DataReaderClient
	Public Class ImageSettings
		Inherits ImportSettingsBase

#Region "Private Variables"
		Private _batesNumberField As String
		Private ReadOnly _batesNumberFieldDefault As String
		Private _documentIDField As String
		Private ReadOnly _documentIDFieldDefault As String
		Private _fileLocationField As String
		Private ReadOnly _fileLocationFieldDefault As String
#End Region

#Region "Constructors"
		Friend Sub New()
			MyBase.New()

			_batesNumberFieldDefault = "BatesNumber"
			_documentIDFieldDefault = "DocumentIdentifier"
			_fileLocationFieldDefault = "FileLocation"
		End Sub
#End Region

#Region "Properties"
		''' <summary>
		''' TODO: Remove. Only here for the NUnit project.
		''' </summary>
		Public Property ArtifactTypeId() As Int32

		''' <summary>
		''' Determines whether a page number is appended to a page-level identifier. 
		''' </summary>
		''' <remarks>When this property is set to True, a new incremental number (such as 01, 02) is added to the page-level identifier, creating a unique page number.</remarks>
		Public Property AutoNumberImages() As Boolean

		''' <summary>
		''' Defines a column name in the source DataTable, which maps to a Field used as a unique identifier in Relativity.
		''' </summary>
		''' <remarks>This unique identifier may be called Bates Number or Control Number in a database.</remarks>
		Public Property BatesNumberField() As String
			Get
				If _batesNumberField Is Nothing Then
					Return _batesNumberFieldDefault
				End If

				Return _batesNumberField
			End Get
			Set(value As String)
				_batesNumberField = value
			End Set
		End Property

		''' <summary>
		''' '
		''' </summary>
		Public Property BeginBatesFieldArtifactID() As Int32

		''' <summary>
		''' Enables or disables validation of image location for the current job. 
		''' </summary>
		''' <remarks>Set this property to True if you want validation disabled. By default, this property is set to False, so validation is enabled. </remarks>
		Public Property DisableImageLocationValidation As Boolean?

		''' <summary>
		''' Enables or disables validation of the image type for the current job. 
		''' </summary>
		''' <remarks>Set this property to True if you want validation disabled. By default, this property is set to False, so validation is enabled. </remarks>
		Public Property DisableImageTypeValidation As Boolean?

		''' <summary>
		''' Name of the field in the data source corresponding to the DocumentIdentifier field
		''' </summary>
		Public Property DocumentIdentifierField() As String
			Get
				If _documentIDField Is Nothing Then
					Return _documentIDFieldDefault
				End If

				Return _documentIDField
			End Get
			Set(value As String)
				_documentIDField = value
			End Set
		End Property

		''' <summary>
		''' Indicates the column name in the source DataTable that maps to the FileLocation field in Relativity. 
		''' </summary>
		''' <remarks>In the data set, this Field value should contain a fully-qualified file location path and file name, such as C:\images\image1.jpg, or \\servername\images\image1.jpg. The Import API doesn't support multi-page TIFFs.</remarks>
		Public Property FileLocationField() As String
			Get
				If _fileLocationField Is Nothing Then
					Return _fileLocationFieldDefault
				End If

				Return _fileLocationField
			End Get
			Set(value As String)
				_fileLocationField = value
			End Set
		End Property

		''' <summary>
		''' Indicates the Field name that contains the subfolder used for document imports.
		''' </summary>
		Public Property FolderPathSourceFieldName() As String

		''' <summary>
		''' To load images into a production, set this property to True. 
		''' </summary>
		''' <remarks>If you want to load images into a specific production, set the ProductionArtifactID.</remarks>
		Public Property ForProduction() As Boolean

		''' <summary>
		''' Field name that contains a full path and filename to image files
		''' </summary>
		Public Property ImageFilePathSourceFieldName() As String

		''' <summary>
		''' Indicates a valid ArtifactID for a production set. Used in conjunction with the ForProduction property.
		''' </summary>
		''' <remarks>If you want to load images into a specific production, set this property to the ArtifactID of a production set, and set the ForProduction property to True. Use the GetProductionSets method to return a list of ArtifactIDs for available production sets. </remarks>
		Public Property ProductionArtifactID() As Int32
#End Region

	End Class
End Namespace