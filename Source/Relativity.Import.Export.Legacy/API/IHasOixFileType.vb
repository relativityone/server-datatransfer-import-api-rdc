Namespace kCura.WinEDDS.Api
	Public Interface IHasOixFileType
		Function GetFileIDData() As FileIDData
	End Interface

	Public Interface IHasFileSize
		Function GetFileSize() As Long
	End Interface

	Public Interface IHasFileName
		Function GetFileName() As String
	End Interface

	Public Interface IInjectableFieldCollection
		Function HasFileSize() As Boolean
		Function HasFileName() As Boolean
		Function HasFileIdData() As Boolean

		ReadOnly Property FileName As IHasFileName
		ReadOnly Property FileSize As IHasFileSize
		ReadOnly Property FileIdData As IHasOixFileType
	End Interface
End Namespace
