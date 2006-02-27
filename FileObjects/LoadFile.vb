Namespace kCura.WinEDDS
	<Serializable()> Public Class LoadFile

		Implements System.Runtime.Serialization.ISerializable

		<NonSerialized()> Public CaseInfo As kCura.EDDS.Types.CaseInfo
		Public DestinationFolderID As Integer
		Public FilePath As String
		Public FirstLineContainsHeaders As Boolean
		Public LoadNativeFiles As Boolean
		Public ExtractFullTextFromNativeFile As Boolean
		Public RecordDelimiter As Char
		Public QuoteDelimiter As Char
		Public NewlineDelimiter As Char
		Public MultiRecordDelimiter As Char
		Public OverwriteDestination As Boolean
		Public SelectedFields() As kCura.WinEDDS.DocumentField
		Public NativeFilePathColumn As String
		Public SelectedIdentifierField As kCura.WinEDDS.DocumentField

		<NonSerialized()> Public Credentials As Net.NetworkCredential

		Public Sub New()
			Me.FilePath = "Select file to load..."
			Me.RecordDelimiter = Chr(20)
			Me.QuoteDelimiter = Chr(254)
			Me.NewlineDelimiter = Chr(174)
			Me.MultiRecordDelimiter = Chr(59)
			Me.FirstLineContainsHeaders = True
		End Sub

		Public Sub GetObjectData(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext) Implements System.Runtime.Serialization.ISerializable.GetObjectData
			'info.AddValue("CaseInfo", Me.CaseInfo, CaseInfo.GetType)
			info.AddValue("FilePath", Me.FilePath, GetType(String))
			info.AddValue("FirstLineContainsHeaders", Me.FirstLineContainsHeaders, GetType(Boolean))
			info.AddValue("LoadNativeFiles", Me.LoadNativeFiles, GetType(Boolean))
			info.AddValue("ExtractFullTextFromNativeFile", Me.ExtractFullTextFromNativeFile, GetType(Boolean))
			info.AddValue("RecordDelimiter", AscW(Me.RecordDelimiter), GetType(Integer))
			info.AddValue("QuoteDelimiter", AscW(Me.QuoteDelimiter), GetType(Integer))
			info.AddValue("NewlineDelimiter", AscW(Me.NewlineDelimiter), GetType(Integer))
			info.AddValue("MultiRecordDelimiter", AscW(Me.MultiRecordDelimiter), GetType(Integer))
			info.AddValue("OverwriteDestination", Me.OverwriteDestination, GetType(Boolean))
			info.AddValue("SelectedFields", Me.SelectedFields, GetType(kCura.WinEDDS.DocumentField()))
			info.AddValue("NativeFilePathColumn", Me.NativeFilePathColumn, GetType(String))
			info.AddValue("SelectedIdentifierField", Me.SelectedIdentifierField, GetType(kCura.WinEDDS.DocumentField))
		End Sub

		Private Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal Context As System.Runtime.Serialization.StreamingContext)
			With info
				'Me.CaseInfo = DirectCast(info.GetValue("CaseInfo", GetType(kCura.EDDS.Types.CaseInfo)), kCura.EDDS.Types.CaseInfo)
				Me.FilePath = info.GetString("FilePath")
				Me.NativeFilePathColumn = info.GetString("NativeFilePathColumn")

				Me.FirstLineContainsHeaders = info.GetBoolean("FirstLineContainsHeaders")
				Me.LoadNativeFiles = info.GetBoolean("LoadNativeFiles")
				Me.ExtractFullTextFromNativeFile = info.GetBoolean("ExtractFullTextFromNativeFile")
				Me.OverwriteDestination = info.GetBoolean("OverwriteDestination")

				Me.RecordDelimiter = ChrW(info.GetInt32("RecordDelimiter"))
				Me.QuoteDelimiter = ChrW(info.GetInt32("QuoteDelimiter"))
				Me.NewlineDelimiter = ChrW(info.GetInt32("NewlineDelimiter"))
				Me.MultiRecordDelimiter = ChrW(info.GetInt32("MultiRecordDelimiter"))

				Me.SelectedIdentifierField = DirectCast(info.GetValue("SelectedIdentifierField", GetType(kCura.WinEDDS.DocumentField)), kCura.WinEDDS.DocumentField)
				Me.SelectedFields = DirectCast(info.GetValue("SelectedFields", GetType(kCura.WinEDDS.DocumentField())), kCura.WinEDDS.DocumentField())
			End With
		End Sub
	End Class
End Namespace