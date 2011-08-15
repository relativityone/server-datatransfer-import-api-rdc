Namespace kCura.Relativity.DataReaderClient

	Public Class ImageSettings
		Inherits ImportSettingsBase

#Region " Private Variables "
		Private _DestinationFolderArtifactID As Int32

		Private _NewLineDelimiter As Char
		Private _MultiValueDelimiter As Char
		Private _NestedValueDelimiter As Char

		Private _FolderPathSourceFieldName As String

		Private _ParentObjectIdSourceFieldName As String

		Private _ImageFilePathSourceFieldName As String
		Private _autoNumberImages As Boolean
		Private _productionartifactid As Int32
		Private _forProduction As Boolean
		Private _identityFieldId As Int32

#End Region

#Region " constructors "
		Public Sub New()
			_MultiValueDelimiter = CType(";", Char)
			_NestedValueDelimiter = CType("\", Char)
			ExtractedTextFieldContainsFilePath = False
			OverlayIdentifierSourceFieldName = String.Empty
		End Sub
#End Region

#Region " Properties "

		''' <summary>
		''' '
		''' </summary>
		Public Property BeginBatesFieldArtifactID() As Int32

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

		''' <summary>
		''' Name of the field in the data source corresponding to the DocumentIdentifier field
		''' </summary>
		''' <value></value>
		Public Property DocumentIdentifierField() As String

		''' <summary>
		''' Name of the field in the data source corresponding to the FileLocation field
		''' </summary>
		Public Property FileLocationField() As String

#End Region

	End Class

End Namespace