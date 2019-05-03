Imports Relativity.Import.Export
Imports Relativity.Import.Export.Io

Namespace kCura.WinEDDS.Api
	Public Class FileIDData
		Inherits FileIdInfo
		Public Sub New(fileID As Int32, fileType As String)
			MyBase.New(fileID, fileType)
		End Sub
	End Class
End Namespace