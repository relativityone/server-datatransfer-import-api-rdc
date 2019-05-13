Namespace kCura.WinEDDS
	Public Class FolderInfo
		Public ArtifactID As Int32
		Public Type As String
		Public Path As String
		Public Sub New(ByVal nodeArtifactId As Int32, ByVal nodeType As String)
			ArtifactID = nodeArtifactId
			Type = nodeType
		End Sub
	End Class
End Namespace
