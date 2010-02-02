Namespace kCura.WinEDDS
	<Serializable()> Public Class LoadFile
		Implements System.Runtime.Serialization.ISerializable

		Public DestinationFolderID As Integer
		Public FilePath As String
		Public FirstLineContainsHeaders As Boolean
		Public LoadNativeFiles As Boolean
		Public ExtractFullTextFromNativeFile As Boolean
		Public RecordDelimiter As Char
		Public QuoteDelimiter As Char
		Public NewlineDelimiter As Char
		Public MultiRecordDelimiter As Char
		Public OverwriteDestination As String
		'Public SelectedFields() As kCura.WinEDDS.DocumentField
		Public FieldMap As kCura.WinEDDS.LoadFileFieldMap
		Public NativeFilePathColumn As String
		Public GroupIdentifierColumn As String
		Public SelectedIdentifierField As kCura.WinEDDS.DocumentField
		Public ExtractMD5HashFromNativeFile As Boolean
		Public CreateFolderStructure As Boolean
		Public FolderStructureContainedInColumn As String
		Public FullTextColumnContainsFileLocation As Boolean
		Public ArtifactTypeID As Integer
		Public HierarchicalValueDelimiter As Char
		Public PreviewCodeCount As New System.Collections.Specialized.HybridDictionary
		Public SourceFileEncoding As System.Text.Encoding
		Public ExtractedTextFileEncoding As System.Text.Encoding
		Public StartLineNumber As Int64
		Public IdentityFieldId As Int32 = -1
		Public SendEmailOnLoadCompletion As Boolean
		<NonSerialized()> Public ExtractedTextFileEncodingName As String
		<NonSerialized()> Public CaseDefaultPath As String = ""
		<NonSerialized()> Public Credentials As Net.NetworkCredential
		<NonSerialized()> Public _cookieContainer As System.Net.CookieContainer
		<NonSerialized()> Public CaseInfo As kCura.EDDS.Types.CaseInfo
		<NonSerialized()> Public SelectedCasePath As String = ""
		<NonSerialized()> Public CopyFilesToDocumentRepository As Boolean = True
		'<NonSerialized()> Public Identity As kCura.EDDS.EDDSIdentity

		Public Property CookieContainer() As System.Net.CookieContainer
			Get
				Return _cookieContainer
			End Get
			Set(ByVal value As System.Net.CookieContainer)
				_cookieContainer = value
			End Set
		End Property

		Public Sub New()
			Me.FilePath = "Select file to load..."
			Me.RecordDelimiter = ChrW(20)
			Me.QuoteDelimiter = ChrW(254)
			Me.NewlineDelimiter = ChrW(174)
			Me.MultiRecordDelimiter = ChrW(59)
			Me.HierarchicalValueDelimiter = "\"c
			Me.FirstLineContainsHeaders = True
			Me.FieldMap = New LoadFileFieldMap
		End Sub

		Public Sub GetObjectData(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext) Implements System.Runtime.Serialization.ISerializable.GetObjectData
			'info.AddValue("CaseInfo", Me.CaseInfo, CaseInfo.GetType)
			info.AddValue("FilePath", Me.FilePath, GetType(String))
			info.AddValue("FirstLineContainsHeaders", Me.FirstLineContainsHeaders, GetType(Boolean))
			info.AddValue("LoadNativeFiles", Me.LoadNativeFiles, GetType(Boolean))
			info.AddValue("ExtractFullTextFromNativeFile", Me.ExtractFullTextFromNativeFile, GetType(Boolean))
			info.AddValue("ExtractMD5HashFromNativeFile", Me.ExtractMD5HashFromNativeFile, GetType(Boolean))
			info.AddValue("RecordDelimiter", AscW(Me.RecordDelimiter), GetType(Integer))
			info.AddValue("QuoteDelimiter", AscW(Me.QuoteDelimiter), GetType(Integer))
			info.AddValue("NewlineDelimiter", AscW(Me.NewlineDelimiter), GetType(Integer))
			info.AddValue("MultiRecordDelimiter", AscW(Me.MultiRecordDelimiter), GetType(Integer))
			info.AddValue("OverwriteDestination", Me.OverwriteDestination, GetType(Boolean))
			info.AddValue("FieldMap", Me.FieldMap, GetType(kCura.WinEDDS.LoadFileFieldMap))
			info.AddValue("NativeFilePathColumn", Me.NativeFilePathColumn, GetType(String))
			info.AddValue("SelectedIdentifierField", Me.SelectedIdentifierField, GetType(kCura.WinEDDS.DocumentField))
			info.AddValue("FolderStructureContainedInColumn", Me.FolderStructureContainedInColumn, GetType(String))
			info.AddValue("CreateFolderStructure", Me.CreateFolderStructure, GetType(Boolean))
			info.AddValue("FullTextColumnContainsFileLocation", Me.FullTextColumnContainsFileLocation, GetType(Boolean))
			info.AddValue("GroupIdentifierColumn", Me.GroupIdentifierColumn, GetType(String))
			info.AddValue("HierarchicalValueDelimiter", AscW(Me.HierarchicalValueDelimiter), GetType(Integer))
			info.AddValue("SourceFileEncoding", Me.SourceFileEncoding.CodePage, GetType(Int32))
			info.AddValue("ArtifactTypeID", Me.ArtifactTypeID, GetType(Int32))
			info.AddValue("StartLineNumber", Me.StartLineNumber, GetType(Int64))
			info.AddValue("IdentityFieldId", Me.IdentityFieldId, GetType(Int32))
			info.AddValue("SendEmailOnLoadCompletion", Me.SendEmailOnLoadCompletion, GetType(Boolean))
			If Me.FullTextColumnContainsFileLocation Then info.AddValue("ExtractedTextFileEncoding", Me.ExtractedTextFileEncoding.CodePage, GetType(Int32))
		End Sub

		Private Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal Context As System.Runtime.Serialization.StreamingContext)
			With info
				'Me.CaseInfo = DirectCast(info.GetValue("CaseInfo", GetType(kCura.EDDS.Types.CaseInfo)), kCura.EDDS.Types.CaseInfo)
				Me.FilePath = info.GetString("FilePath")
				Try
					If Not System.IO.File.Exists(Me.FilePath) Then
						Me.FilePath = ""
					End If
				Catch ex As System.Exception
					Me.FilePath = ""
				End Try
				Me.NativeFilePathColumn = info.GetString("NativeFilePathColumn")
				Me.FirstLineContainsHeaders = info.GetBoolean("FirstLineContainsHeaders")
				Me.LoadNativeFiles = info.GetBoolean("LoadNativeFiles")
				Me.ExtractFullTextFromNativeFile = info.GetBoolean("ExtractFullTextFromNativeFile")
				Me.ExtractMD5HashFromNativeFile = info.GetBoolean("ExtractMD5HashFromNativeFile")
				Me.OverwriteDestination = info.GetString("OverwriteDestination")

				Me.RecordDelimiter = ChrW(info.GetInt32("RecordDelimiter"))
				Me.QuoteDelimiter = ChrW(info.GetInt32("QuoteDelimiter"))
				Me.NewlineDelimiter = ChrW(info.GetInt32("NewlineDelimiter"))
				Me.MultiRecordDelimiter = ChrW(info.GetInt32("MultiRecordDelimiter"))

				Me.SelectedIdentifierField = DirectCast(info.GetValue("SelectedIdentifierField", GetType(kCura.WinEDDS.DocumentField)), kCura.WinEDDS.DocumentField)
				Try
					Me.GroupIdentifierColumn = DirectCast(info.GetValue("GroupIdentifierColumn", GetType(String)), String)
				Catch
					Me.GroupIdentifierColumn = ""
				End Try
				'Me.SelectedFields = DirectCast(info.GetValue("SelectedFields", GetType(kCura.WinEDDS.DocumentField())), kCura.WinEDDS.DocumentField())
				Me.FieldMap = DirectCast(info.GetValue("FieldMap", GetType(kCura.WinEDDS.LoadFileFieldMap)), LoadFileFieldMap)

				Me.FolderStructureContainedInColumn = info.GetString("FolderStructureContainedInColumn")
				Me.CreateFolderStructure = info.GetBoolean("CreateFolderStructure")
				Try
					Me.FullTextColumnContainsFileLocation = info.GetBoolean("FullTextColumnContainsFileLocation")
				Catch ex As System.Exception
					Me.FullTextColumnContainsFileLocation = False
				End Try
				Dim hval As Int32 = 0
				Try
					hval = info.GetInt32("HierarchicalValueDelimiter")
				Catch ex As Exception
				End Try
				If hval = 0 Then hval = AscW("\"c)
				Try
					Me.HierarchicalValueDelimiter = ChrW(hval)
				Catch ex As Exception
					Me.HierarchicalValueDelimiter = "\"c
				End Try
				Try
					Me.SourceFileEncoding = System.Text.Encoding.GetEncoding(info.GetInt32("SourceFileEncoding"))
				Catch
					Me.SourceFileEncoding = System.Text.Encoding.Default
				End Try
				Try
					Me.ExtractedTextFileEncoding = System.Text.Encoding.GetEncoding(info.GetInt32("ExtractedTextFileEncoding"))
				Catch
					Me.ExtractedTextFileEncoding = System.Text.Encoding.Default
				End Try
				Try
					Me.ArtifactTypeID = info.GetInt32("ArtifactTypeID")
				Catch
					Me.ArtifactTypeID = kCura.EDDS.Types.ArtifactType.Document
				End Try
				Try
					Me.StartLineNumber = info.GetInt64("StartLineNumber")
				Catch
					Me.StartLineNumber = 0
				End Try
				Try
					Me.IdentityFieldId = info.GetInt32("IdentityFieldId")
				Catch
					Me.IdentityFieldId = -1
				End Try
				Try
					Me.SendEmailOnLoadCompletion = info.GetBoolean("SendEmailOnLoadCompletion")
				Catch
					Me.SendEmailOnLoadCompletion = kCura.WinEDDS.Service.Settings.SendEmailOnLoadCompletion
				End Try
			End With
		End Sub

	End Class
End Namespace