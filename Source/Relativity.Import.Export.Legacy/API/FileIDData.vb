Imports Relativity.Import.Export.Io

Namespace kCura.WinEDDS.Api
	Friend Class FileIDData
		Inherits FileTypeIdInfo
		Public Sub New(fileID As Int32, fileType As String)
			MyBase.New(fileID, fileType)
		End Sub
	End Class
End Namespace