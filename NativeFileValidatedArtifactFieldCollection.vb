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
End Namespace