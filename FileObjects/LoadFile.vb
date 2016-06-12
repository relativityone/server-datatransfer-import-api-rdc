Imports System.Collections.Generic

Namespace kCura.WinEDDS
	<Serializable()> Public Class LoadFile
		Implements System.Runtime.Serialization.ISerializable

		Public DestinationFolderID As Integer
		Public FilePath As String
		Public FirstLineContainsHeaders As Boolean
		Public LoadNativeFiles As Boolean
		Public RecordDelimiter As Char
		Public QuoteDelimiter As Char
		Public NewlineDelimiter As Char
		Public MultiRecordDelimiter As Char
		Public OverwriteDestination As String
		'Public SelectedFields() As kCura.WinEDDS.DocumentField
		Public FieldMap As kCura.WinEDDS.LoadFileFieldMap
		Public NativeFilePathColumn As String
		Public GroupIdentifierColumn As String
		Public DataGridIDColumn As String
		Public SelectedIdentifierField As kCura.WinEDDS.DocumentField

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
		Public ForceFolderPreview As Boolean
		Public OIFileIdMapped As Boolean
		Public OIFileIdColumnName As String
		Public OIFileTypeColumnName As String
		Public FileSizeMapped As Boolean
		Public FileSizeColumn As String
		Public FileNameColumn As String
		Public OverlayBehavior As FieldOverlayBehavior?

		<NonSerialized()> Public ObjectFieldIdListContainsArtifactId As IList(Of Int32)
		<NonSerialized()> Public ExtractedTextFileEncodingName As String
		<NonSerialized()> Public CaseDefaultPath As String = ""
		<NonSerialized()> Public Credentials As Net.NetworkCredential
		<NonSerialized()> Public _cookieContainer As System.Net.CookieContainer
		<NonSerialized()> Public CaseInfo As Relativity.CaseInfo
		<NonSerialized()> Public SelectedCasePath As String = ""
		<NonSerialized()> Public CopyFilesToDocumentRepository As Boolean = True
		'<NonSerialized()> Public Identity As Relativity.Core.EDDSIdentity

		Public Enum FieldOverlayBehavior
			UseRelativityDefaults = 0
			MergeAll = 1
			ReplaceAll = 2
		End Enum

		Public Property CookieContainer() As System.Net.CookieContainer
			Get
				Return _cookieContainer
			End Get
			Set(ByVal value As System.Net.CookieContainer)
				_cookieContainer = value
			End Set
		End Property

		Public Sub New()
			Me.FilePath = String.Empty
			Me.RecordDelimiter = ChrW(20)
			Me.QuoteDelimiter = ChrW(254)
			Me.NewlineDelimiter = ChrW(174)
			Me.MultiRecordDelimiter = ChrW(59)
			Me.HierarchicalValueDelimiter = "\"c
			Me.FirstLineContainsHeaders = True
			Me.FieldMap = New LoadFileFieldMap
			Me.OverlayBehavior = Nothing
		End Sub

		Public Sub GetObjectData(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext) Implements System.Runtime.Serialization.ISerializable.GetObjectData
			'info.AddValue("CaseInfo", Me.CaseInfo, CaseInfo.GetType)
			info.AddValue("FilePath", Me.FilePath, GetType(String))
			info.AddValue("FirstLineContainsHeaders", Me.FirstLineContainsHeaders, GetType(Boolean))
			info.AddValue("LoadNativeFiles", Me.LoadNativeFiles, GetType(Boolean))

			If Me.OverlayBehavior Is Nothing OrElse Not Me.OverlayBehavior.HasValue Then
				info.AddValue("OverlayBehavior", Nothing, GetType(Integer))
			Else
				info.AddValue("OverlayBehavior", Me.OverlayBehavior, GetType(Integer))
			End If

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
			info.AddValue("DataGridIDColumn", Me.DataGridIDColumn, GetType(String))
			info.AddValue("HierarchicalValueDelimiter", AscW(Me.HierarchicalValueDelimiter), GetType(Integer))
			If Me.SourceFileEncoding Is Nothing Then
				info.AddValue("SourceFileEncoding", -1, GetType(Int32))
			Else
				info.AddValue("SourceFileEncoding", Me.SourceFileEncoding.CodePage, GetType(Int32))
			End If
			info.AddValue("ArtifactTypeID", Me.ArtifactTypeID, GetType(Int32))
			info.AddValue("StartLineNumber", Me.StartLineNumber, GetType(Int64))
			info.AddValue("IdentityFieldId", Me.IdentityFieldId, GetType(Int32))
			info.AddValue("SendEmailOnLoadCompletion", Me.SendEmailOnLoadCompletion, GetType(Boolean))
			info.AddValue("ForceFolderPreview", Me.ForceFolderPreview, GetType(Boolean))
			If Me.FullTextColumnContainsFileLocation Then
				If Me.ExtractedTextFileEncoding Is Nothing Then
					info.AddValue("ExtractedTextFileEncoding", -1, GetType(Int32))
				Else
					info.AddValue("ExtractedTextFileEncoding", Me.ExtractedTextFileEncoding.CodePage, GetType(Int32))
				End If
			End If
		End Sub

		Private Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal Context As System.Runtime.Serialization.StreamingContext)
			With info
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

				Try
					Me.DataGridIDColumn = DirectCast(info.GetValue("DataGridIDColumn", GetType(String)), String)
				Catch
					Me.DataGridIDColumn = ""
				End Try

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
					Dim codePageID As Int32 = info.GetInt32("SourceFileEncoding")
					If codePageID = -1 Then
						Me.SourceFileEncoding = Nothing
					Else
						Me.SourceFileEncoding = System.Text.Encoding.GetEncoding(codePageID)
					End If
				Catch
					Me.SourceFileEncoding = System.Text.Encoding.Default
				End Try
				Try
					Dim codePageID As Int32 = info.GetInt32("ExtractedTextFileEncoding")
					If codePageID = -1 Then
						Me.ExtractedTextFileEncoding = Nothing
					Else
						Me.ExtractedTextFileEncoding = System.Text.Encoding.GetEncoding(codePageID)
					End If
				Catch
					Me.ExtractedTextFileEncoding = Nothing
				End Try
				Try
					Me.ArtifactTypeID = info.GetInt32("ArtifactTypeID")
				Catch
					Me.ArtifactTypeID = Relativity.ArtifactType.Document
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
				Try
					Me.ForceFolderPreview = info.GetBoolean("ForceFolderPreview")
				Catch
					Me.ForceFolderPreview = kCura.WinEDDS.Config.ForceFolderPreview
				End Try

				Try
					Me.OverlayBehavior = New Nullable(Of FieldOverlayBehavior)(CType(info.GetInt32("OverlayBehavior"), FieldOverlayBehavior))
				Catch
					Me.OverlayBehavior = Nothing
				End Try

			End With
		End Sub

	End Class
End Namespace