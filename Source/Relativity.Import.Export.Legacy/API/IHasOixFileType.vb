Imports Relativity.Import.Export

Namespace kCura.WinEDDS.Api
	Public Interface IHasOixFileType
		Function GetFileIdInfo() As FileIdInfo
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
		Function HasFileIdInfo() As Boolean

		ReadOnly Property FileName As IHasFileName
		ReadOnly Property FileSize As IHasFileSize
		ReadOnly Property FileIdInfo As IHasOixFileType
	End Interface
End Namespace
