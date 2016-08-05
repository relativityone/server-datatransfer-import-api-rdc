Imports kCura.WinEDDS.Api
Namespace kCura.WinEDDS.ImportExtension
	Public Class NativeFileValidatedArtifactFieldCollection
		Inherits ArtifactFieldCollection
		Implements IHasOixFileType

		Public Property OixFileID As FileIDData
		Public Function GetFileIDData() As FileIDData Implements Api.IHasOixFileType.GetFileIDData
			Return Me.OixFileID
		End Function
	End Class

	Public Class FileSizePopulatedArtifactFieldCollection
		Inherits ArtifactFieldCollection
		Implements IHasFileSize

		Public Property FileSize() As Long
		Public Function GetFileSize() As Long Implements Api.IHasFileSize.GetFileSize
			Return FileSize
		End Function
	End Class

	Public Class NativeFilePopulatedArtifactFieldCollection
		Inherits NativeFileValidatedArtifactFieldCollection
		Implements IHasFileSize

		Public Property FileSize() As Long
		Public Function GetFileSize() As Long Implements Api.IHasFileSize.GetFileSize
			Return FileSize
		End Function
	End Class
End Namespace