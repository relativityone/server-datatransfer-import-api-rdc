Imports System.Runtime.Serialization
Imports FileNaming.CustomFileNaming
Imports Relativity.DataExchange
Imports Relativity.DataExchange.Service

Namespace kCura.WinEDDS
	''' <summary>
	''' Container class for all export settings
	''' </summary>
	''' <remarks>
	''' Writable properties that are not marked as ReadFromExisting are used in saving/loading these files to disk.  If one needs to add a property, and it needs to be save-able, make sure to add those settings to the serialize/deserialize methods.
	''' </remarks>
	<Serializable()> Public Class ExportFile
		Implements ISerializable

#Region " Members "

		Protected _loadFilesPrefix As String
		Protected _startAtDocument As Int32 = 0
		Private _artifactTypeId As Int32

#End Region

#Region "Public Properties"
		Public ReadOnly Property ArtifactTypeID() As Int32
			Get
				Return _artifactTypeId
			End Get
		End Property

		Public Property LoadFilesPrefix() As String
			Get
				Return _loadFilesPrefix
			End Get
			Set(ByVal value As String)
				_loadFilesPrefix = kCura.WinEDDS.Utility.GetFilesystemSafeName(value)
			End Set
		End Property

		Public Property ImagePrecedence() As Pair()

		<ReadFromExisting()> Public Property CaseInfo() As CaseInfo

		Public ReadOnly Property CaseArtifactID() As Int32
			Get
				Return Me.CaseInfo.ArtifactID
			End Get
		End Property

		<ReadFromExisting()> Public Property DataTable() As System.Data.DataTable

		<ReadFromExisting()> Public Property ArtifactAvfLookup() As Specialized.HybridDictionary

		Public Property NestedValueDelimiter() As Char

		<ReadFromExisting()> Public Property TypeOfExport() As ExportType

		Public Property FolderPath() As String

		Public Property ArtifactID() As Int32

		Public Property ViewID() As Int32
			
		Public Property Overwrite() As Boolean

		Public Property RecordDelimiter() As Char

		Public Property QuoteDelimiter() As Char

		Public Property NewlineDelimiter() As Char

		Public Property MultiRecordDelimiter() As Char

		<ReadFromExisting()> Public Property Credential() As Net.NetworkCredential

		<ReadFromExisting()> Public Property CookieContainer() As System.Net.CookieContainer

		Public Property ExportFullText() As Boolean

		Public Property ExportFullTextAsFile() As Boolean

		Public Property ExportNative() As Boolean

		Public Property ExportPdf() As Boolean

		Public Property LogFileFormat() As kCura.WinEDDS.LoadFileType.FileFormat?

		Public Property RenameFilesToIdentifier() As Boolean

		Public Property IdentifierColumnName() As String


		Public Property LoadFileExtension() As String

		Public Property VolumeInfo() As kCura.WinEDDS.Exporters.VolumeInfo

		Public Property ExportImages() As Boolean

		Public Property ExportNativesToFileNamedFrom() As kCura.WinEDDS.ExportNativeWithFilenameFrom = ExportNativeWithFilenameFrom.Identifier

		Public Property FilePrefix() As String

		Public Property TypeOfExportedFilePath() As ExportedFilePathType

		Public Property TypeOfImage() As ImageType?

		Public Property AppendOriginalFileName() As Boolean

		Public Property LoadFileIsHtml() As Boolean

		<ReadFromExisting()> Public Property AllExportableFields() As WinEDDS.ViewFieldInfo()

		Public Property SelectedViewFields() As WinEDDS.ViewFieldInfo()

		Public Property MulticodesAsNested() As Boolean

		Public Property SelectedTextFields() As WinEDDS.ViewFieldInfo()

		Public Property LoadFileEncoding() As System.Text.Encoding

		Public Property TextFileEncoding() As System.Text.Encoding

		Public Property VolumeDigitPadding() As Int32

		Public Property SubdirectoryDigitPadding() As Int32

		Public Property StartAtDocumentNumber() As Int32
			Get
				Return _startAtDocument
			End Get
			Set(ByVal value As Int32)
				_startAtDocument = value
			End Set
		End Property

		<ReadFromExisting()> Public Property FileField() As DocumentField

		Public ReadOnly Property HasFileField() As Boolean
			Get
				Return Not FileField Is Nothing
			End Get
		End Property

		Public Property ObjectTypeName As String

		Public Property UseCustomFileNaming() As Boolean
		Public Property CustomFileNaming() As CustomFileNameDescriptorModel
#End Region

#Region " Serialization "

		Public Sub GetObjectData(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext) Implements System.Runtime.Serialization.ISerializable.GetObjectData
			info.AddValue("ArtifactID", Me.ArtifactID, GetType(Int32))
			info.AddValue("LoadFilesPrefix", System.Web.HttpUtility.HtmlEncode(Me.LoadFilesPrefix), GetType(String))
			info.AddValue("NestedValueDelimiter", AscW(Me.NestedValueDelimiter), GetType(Int32))
			info.AddValue("TypeOfExport", CInt(Me.TypeOfExport), GetType(Int32))
			info.AddValue("FolderPath", Me.FolderPath, GetType(String))
			info.AddValue("ViewID", Me.ViewID, GetType(Int32))
			info.AddValue("Overwrite", Me.Overwrite, GetType(Boolean))
			info.AddValue("RecordDelimiter", AscW(Me.RecordDelimiter), GetType(Int32))
			info.AddValue("QuoteDelimiter", AscW(Me.QuoteDelimiter), GetType(Int32))
			info.AddValue("NewlineDelimiter", AscW(Me.NewlineDelimiter), GetType(Int32))
			info.AddValue("MultiRecordDelimiter", AscW(Me.MultiRecordDelimiter), GetType(Int32))
			info.AddValue("ExportFullText", Me.ExportFullText, GetType(Boolean))
			info.AddValue("ExportFullTextAsFile", Me.ExportFullTextAsFile, GetType(Boolean))
			info.AddValue("ExportNative", Me.ExportNative, GetType(Boolean))
			info.AddValue("LogFileFormat", If(Me.LogFileFormat.HasValue, CInt(Me.LogFileFormat.Value).ToString, String.Empty), GetType(String))
			info.AddValue("RenameFilesToIdentifier", Me.RenameFilesToIdentifier, GetType(Boolean))
			info.AddValue("IdentifierColumnName", Me.IdentifierColumnName, GetType(String))
			info.AddValue("LoadFileExtension", Me.LoadFileExtension, GetType(String))
			info.AddValue("ExportImages", Me.ExportImages, GetType(Boolean))
			info.AddValue("ExportNativesToFileNamedFrom", CInt(Me.ExportNativesToFileNamedFrom), GetType(Int32))
			info.AddValue("FilePrefix", Me.FilePrefix, GetType(String))
			info.AddValue("TypeOfExportedFilePath", CInt(Me.TypeOfExportedFilePath), GetType(Int32))
			info.AddValue("TypeOfImage", If(Me.TypeOfImage.HasValue, CInt(Me.TypeOfImage.Value).ToString, String.Empty), GetType(String))
			info.AddValue("AppendOriginalFileName", Me.AppendOriginalFileName, GetType(Boolean))
			info.AddValue("LoadFileIsHtml", Me.LoadFileIsHtml, GetType(Boolean))
			info.AddValue("MulticodesAsNested", Me.MulticodesAsNested, GetType(Boolean))
			info.AddValue("LoadFileEncoding", If(Me.LoadFileEncoding Is Nothing, -1, Me.LoadFileEncoding.CodePage), GetType(Int32))
			info.AddValue("TextFileEncoding", If(Me.TextFileEncoding Is Nothing, -1, Me.TextFileEncoding.CodePage), GetType(Int32))
			info.AddValue("VolumeDigitPadding", Me.VolumeDigitPadding, GetType(Int32))
			info.AddValue("SubdirectoryDigitPadding", Me.SubdirectoryDigitPadding, GetType(Int32))
			info.AddValue("StartAtDocumentNumber", Me.StartAtDocumentNumber, GetType(Int32))
			info.AddValue("VolumeInfo", Me.VolumeInfo, GetType(kCura.WinEDDS.Exporters.VolumeInfo))
			info.AddValue("SelectedTextFields", Me.SelectedTextFields, GetType(kCura.WinEDDS.ViewFieldInfo()))
			info.AddValue("ImagePrecedence", Me.ImagePrecedence, GetType(kCura.WinEDDS.Pair()))
			info.AddValue("SelectedViewFields", Me.SelectedViewFields, GetType(kCura.WinEDDS.ViewFieldInfo()))
			info.AddValue("ObjectTypeName", Me.ObjectTypeName, GetType(String))
			info.AddValue("UseCustomFileNaming", Me.UseCustomFileNaming, GetType(Boolean))
			info.AddValue("CustomFileNaming", Me.CustomFileNaming, GetType(CustomFileNameDescriptorModel))
		End Sub

		Protected Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)
			With info
				Me.ArtifactID = info.GetInt32("ArtifactID")
				Me.LoadFilesPrefix = System.Web.HttpUtility.HtmlDecode(info.GetString("LoadFilesPrefix"))
				Me.NestedValueDelimiter = ChrW(info.GetInt32("NestedValueDelimiter"))
				Me.TypeOfExport = CType(info.GetInt32("TypeOfExport"), kCura.WinEDDS.ExportFile.ExportType)
				Me.FolderPath = info.GetString("FolderPath")
				Me.ViewID = info.GetInt32("ViewID")
				Me.Overwrite = info.GetBoolean("Overwrite")
				Me.RecordDelimiter = ChrW(info.GetInt32("RecordDelimiter"))
				Me.QuoteDelimiter = ChrW(info.GetInt32("QuoteDelimiter"))
				Me.NewlineDelimiter = ChrW(info.GetInt32("NewlineDelimiter"))
				Me.MultiRecordDelimiter = ChrW(info.GetInt32("MultiRecordDelimiter"))
				Me.ExportFullText = info.GetBoolean("ExportFullText")
				Me.ExportFullTextAsFile = info.GetBoolean("ExportFullTextAsFile")
				Me.ExportNative = info.GetBoolean("ExportNative")
				With NullableTypesHelper.ToNullableInt32(info.GetString("LogFileFormat"))
					Me.LogFileFormat = Nothing
					If .HasValue Then Me.LogFileFormat = CType(.Value, kCura.WinEDDS.LoadFileType.FileFormat)
				End With
				Me.RenameFilesToIdentifier = info.GetBoolean("RenameFilesToIdentifier")
				Me.IdentifierColumnName = info.GetString("IdentifierColumnName")
				Me.LoadFileExtension = info.GetString("LoadFileExtension")
				Me.ExportImages = info.GetBoolean("ExportImages")
				Me.ExportNativesToFileNamedFrom = CType(info.GetInt32("ExportNativesToFileNamedFrom"), kCura.WinEDDS.ExportNativeWithFilenameFrom)
				Me.FilePrefix = info.GetString("FilePrefix")
				Me.TypeOfExportedFilePath = CType(info.GetInt32("TypeOfExportedFilePath"), kCura.WinEDDS.ExportFile.ExportedFilePathType)
				With NullableTypesHelper.ToNullableInt32(info.GetString("TypeOfImage"))
					Me.TypeOfImage = Nothing
					If .HasValue Then Me.TypeOfImage = CType(.Value, kCura.WinEDDS.ExportFile.ImageType)
				End With
				Me.AppendOriginalFileName = info.GetBoolean("AppendOriginalFileName")
				Me.LoadFileIsHtml = info.GetBoolean("LoadFileIsHtml")
				Me.MulticodesAsNested = info.GetBoolean("MulticodesAsNested")
				Dim encod As Int32 = info.GetInt32("LoadFileEncoding")
				Me.LoadFileEncoding = If(encod > 0, System.Text.Encoding.GetEncoding(encod), Nothing)
				encod = info.GetInt32("TextFileEncoding")
				Me.TextFileEncoding = If(encod > 0, System.Text.Encoding.GetEncoding(encod), Nothing)
				Me.VolumeDigitPadding = info.GetInt32("VolumeDigitPadding")
				Me.SubdirectoryDigitPadding = info.GetInt32("SubdirectoryDigitPadding")
				Me.StartAtDocumentNumber = info.GetInt32("StartAtDocumentNumber")
				Me.VolumeInfo = CType(info.GetValue("VolumeInfo", GetType(kCura.WinEDDS.Exporters.VolumeInfo)), kCura.WinEDDS.Exporters.VolumeInfo)
				Try
					Me.SelectedTextFields = DirectCast(info.GetValue("SelectedTextFields", GetType(kCura.WinEDDS.ViewFieldInfo())), kCura.WinEDDS.ViewFieldInfo())
				Catch
					Dim field As kCura.WinEDDS.ViewFieldInfo = DirectCast(info.GetValue("SelectedTextField", GetType(kCura.WinEDDS.ViewFieldInfo)), kCura.WinEDDS.ViewFieldInfo)
					Me.SelectedTextFields = If(field Is Nothing, Nothing, {field})
				End Try
				Me.ImagePrecedence = DirectCast(info.GetValue("ImagePrecedence", GetType(kCura.WinEDDS.Pair())), kCura.WinEDDS.Pair())
				Me.SelectedViewFields = DirectCast(info.GetValue("SelectedViewFields", GetType(kCura.WinEDDS.ViewFieldInfo())), kCura.WinEDDS.ViewFieldInfo())
				Me.ObjectTypeName = info.GetString("ObjectTypeName")
				Try
					Me.UseCustomFileNaming = info.GetBoolean("UseCustomFileNaming")
				Catch ex As SerializationException
					Me.UseCustomFileNaming = False
				End Try
				Try
					Me.CustomFileNaming = DirectCast(info.GetValue("CustomFileNaming", GetType(CustomFileNameDescriptorModel)), CustomFileNameDescriptorModel)
				Catch ex As SerializationException
					Me.CustomFileNaming = Nothing
				End Try
			End With
		End Sub

#End Region

#Region " Constructors "


		Public Sub New(ByVal artifactTypeID As Int32)
			Me.RecordDelimiter = ChrW(20)
			Me.QuoteDelimiter = ChrW(254)
			Me.NewlineDelimiter = ChrW(174)
			Me.MultiRecordDelimiter = ChrW(59)
			Me.NestedValueDelimiter = "\"c
			Me.MulticodesAsNested = True
			_artifactTypeId = artifactTypeID
		End Sub

#End Region

#Region " Enums "


		Public Enum ExportType
			Production = 0
			ArtifactSearch = 1
			ParentSearch = 2
			AncestorSearch = 3
		End Enum

		Public Enum ExportedFilePathType
			Relative = 0
			Absolute = 1
			Prefix = 2
		End Enum

		Public Enum ImageType
			[Select] = -1
			SinglePage = 0
			MultiPageTiff = 1
			Pdf = 2
		End Enum
		Public Class ImageTypeParser
			Public Function Parse(ByVal s As String) As ImageType?
				If String.IsNullOrEmpty(s) Then Return Nothing
				Dim retval As New ImageType?
				Select Case s
					Case "Single-page TIF/JPG"
						retval = ImageType.SinglePage
					Case "Multi-page TIF"
						retval = ImageType.MultiPageTiff
					Case "PDF"
						retval = ImageType.Pdf
				End Select
				Return retval
			End Function
		End Class
#End Region

	End Class
End Namespace