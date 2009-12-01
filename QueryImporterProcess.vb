Namespace kCura.WinEDDS.ImportExtension
	Public Class QueryImporterProcess
		Inherits kCura.WinEDDS.ImportLoadFileProcess

		Public Overrides Function GetLoadFileImporter() As kCura.WinEDDS.BulkLoadFileImporter
			Return New QueryImporter(Me.LoadFile)
		End Function

	End Class
End Namespace

