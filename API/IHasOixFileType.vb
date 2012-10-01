Namespace kCura.WinEDDS.Api
	Public Interface IHasOixFileType
		Function GetFileIDData() As FileIDData
	End Interface

	Public Interface IHasFileSize
		Function GetFileSize() As Nullable(Of Long)
	End Interface
End Namespace
