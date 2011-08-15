Namespace kCura.Relativity.DataReaderClient

	Public Class Settings
		Inherits ImportSettingsBase

#Region " Private Variables "

		Private _NewLineDelimiter As Char
		Private _MultiValueDelimiter As Char
		Private _NestedValueDelimiter As Char

		Private _FolderPathSourceFieldName As String

		Private _ParentObjectIdSourceFieldName As String

		Private _NativeFilePathSourceFieldName As String
		Private _rowCount As Int32

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
		''' ArtifactTypeId of the destination Relativity dynamic object
		''' </summary>
		Public Property ArtifactTypeId() As Int32

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
		''' Field name that contains a full path and filename to native files
		''' </summary>
		Public Property NativeFilePathSourceFieldName() As String
			Get
				Return _NativeFilePathSourceFieldName
			End Get
			Set(ByVal Value As String)
				_NativeFilePathSourceFieldName = Value
			End Set
		End Property

		Public Property RowCount() As Int32
			Get
				Return _rowCount
			End Get
			Set(ByVal Value As Int32)
				_rowCount = Value
			End Set
		End Property
#End Region

	End Class

End Namespace