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
		Public Sub New()
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
		''' Auto number Images yes/no
		''' </summary>
		Public Property AutoNumberImages() As Boolean

		''' <summary>
		''' Name of the field in the data source corresponding to the BatesNumber field
		''' </summary>
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
		''' Enables or disables image location validation for the current job
		''' </summary>
		''' <value>True: validation is disabled
		''' False: validation is enabled
		''' Nothing: validation will use the pre-configured value</value>
		Public Property DisableImageLocationValidation As Boolean?

		''' <summary>
		''' Enables or disables image type validation for the current job
		''' </summary>
		''' <value>True: validation is disabled
		''' False: validation is enabled
		''' Nothing: validation will use the pre-configured value</value>
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
		''' Name of the field in the data source corresponding to the FileLocation field
		''' </summary>
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
		''' Field name that contains ???
		''' </summary>
		Public Property FolderPathSourceFieldName() As String

		''' <summary>
		''' Set to true if you want the import to go into a production
		''' </summary>
		Public Property ForProduction() As Boolean

		''' <summary>
		''' Field name that contains a full path and filename to image files
		''' </summary>
		Public Property ImageFilePathSourceFieldName() As String

		''' <summary>
		''' If this is set then images will load into selected production
		''' </summary>
		Public Property ProductionArtifactID() As Int32
#End Region

	End Class
End Namespace