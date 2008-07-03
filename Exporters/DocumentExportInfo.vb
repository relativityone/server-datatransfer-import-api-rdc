Namespace kCura.WinEDDS.Exporters
	Public Class DocumentExportInfo
		Private _images As System.Collections.ArrayList
		Private _native As Object
		Private _totalFileSize As Int64
		Private _totalNumberOfFiles As Int64
		Private _documentArtifactID As Int32
		Private _hasFullText As Boolean
		Private _identifierValue As String = ""
		Private _nativeExtension As String = ""
		Private _nativeFileGuid As String = ""
		Private _dataRow As System.Data.DataRow
		Private _nativeTempLocation As String = ""
		Private _productionBeginBates As String = ""
		Private _originalFileName As String = ""
		Private _nativeSourceLocation As String = ""

		Public Property DataRow() As System.Data.DataRow
			Get
				Return _dataRow
			End Get
			Set(ByVal value As System.Data.DataRow)
				_dataRow = value
			End Set
		End Property

		Public Property Images() As System.Collections.ArrayList
			Get
				Return _images
			End Get
			Set(ByVal value As System.Collections.ArrayList)
				_images = value
			End Set
		End Property

		Public Property Native() As Object
			Get
				Return _native
			End Get
			Set(ByVal value As Object)
				_native = value
			End Set
		End Property

		Public Property HasFullText() As Boolean
			Get
				Return _hasFullText
			End Get
			Set(ByVal value As Boolean)
				_hasFullText = value
			End Set
		End Property

		Public Property TotalFileSize() As Int64
			Get
				Return _totalFileSize
			End Get
			Set(ByVal value As Int64)
				_totalFileSize = value
			End Set
		End Property

		Public Property TotalNumberOfFiles() As Int64
			Get
				Return _totalNumberOfFiles
			End Get
			Set(ByVal value As Int64)
				_totalNumberOfFiles = value
			End Set
		End Property

		Public Property IdentifierValue() As String
			Get
				Return _identifierValue
			End Get
			Set(ByVal value As String)
				_identifierValue = value
			End Set
		End Property

		Public Property DocumentArtifactID() As Int32
			Get
				Return _documentArtifactID
			End Get
			Set(ByVal value As Int32)
				_documentArtifactID = value
			End Set
		End Property

		Public Property NativeExtension() As String
			Get
				Return _nativeExtension
			End Get
			Set(ByVal value As String)
				_nativeExtension = value
			End Set
		End Property

		Public Property NativeFileGuid() As String
			Get
				Return _nativeFileGuid
			End Get
			Set(ByVal value As String)
				_nativeFileGuid = value
			End Set
		End Property

		Public Property NativeTempLocation() As String
			Get
				Return _nativeTempLocation
			End Get
			Set(ByVal value As String)
				_nativeTempLocation = value
			End Set
		End Property

		Public Property NativeSourceLocation() As String
			Get
				Return _nativeSourceLocation
			End Get
			Set(ByVal value As String)
				_nativeSourceLocation = value
			End Set
		End Property

		Public Function NativeFileName(ByVal appendToOriginal As Boolean) As String
			Dim retval As String
			If appendToOriginal Then
				retval = IdentifierValue & "_" & OriginalFileName
			Else
				If Not NativeExtension = "" Then
					retval = IdentifierValue & "." & NativeExtension
				Else
					retval = IdentifierValue
				End If
			End If
			Return kCura.Utility.File.ConvertIllegalCharactersInFilename(retval)
		End Function


		Public Property OriginalFileName() As String
			Get
				Return _originalFileName
			End Get
			Set(ByVal value As String)
				_originalFileName = value
			End Set
		End Property

		Public ReadOnly Property ProductionBeginBatesFileName(ByVal appendToOriginal As Boolean) As String
			Get
				Dim retval As String
				If appendToOriginal Then
					retval = ProductionBeginBates & "_" & OriginalFileName
				Else
					If Not NativeExtension = "" Then
						retval = ProductionBeginBates & "." & NativeExtension
					Else
						retval = ProductionBeginBates
					End If
				End If
				Return kCura.Utility.File.ConvertIllegalCharactersInFilename(retval)
			End Get
		End Property

		Public ReadOnly Property NativeCount() As Int64
			Get
				If Me.NativeFileGuid = "" Then
					Return 0
				Else
					Return 1
				End If
			End Get
		End Property

		Public ReadOnly Property ImageCount() As Int64
			Get
				If Me.Images Is Nothing Then Return 0
				Return Me.Images.Count
			End Get
		End Property

		Public Property ProductionBeginBates() As String
			Get
				Return _productionBeginBates
			End Get
			Set(ByVal value As String)
				_productionBeginBates = value
			End Set
		End Property

	End Class
End Namespace