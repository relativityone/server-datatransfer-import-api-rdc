Namespace kCura.WinEDDS
	Public Class ImportFileDirectorySettings
		Public BatesNumberPrefix As String
		Public BatesNumberSeed As Int32
		Public CaseInfo As kCura.EDDS.Types.CaseInfo
		Public DestinationFolderID As Int32
		Public FilePath As String
		Public FieldMappings() As FieldMap
		Public AttachFiles As Boolean
		Public ExtractFullTextFromFile As Boolean
		Public EnronImport As Boolean
		Public FileExtentionsToImport As String

		Public Class FieldMap
			Public FileField As String
			Public CaseField As String
		End Class

	End Class
End Namespace