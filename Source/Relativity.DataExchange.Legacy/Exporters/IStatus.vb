Imports Relativity.DataExchange.Process

Namespace kCura.WinEDDS
	Public Interface IStatus
		Sub WriteError(ByVal line As String)
		Sub WriteImgProgressError(ByVal artifact As Exporters.ObjectExportInfo, ByVal imageIndex As Int32, ByVal ex As System.Exception, Optional ByVal notes As String = "")
		Sub WriteStatusLine(ByVal e As EventType2, ByVal line As String, ByVal isEssential As Boolean)
		Sub WriteStatusLineWithoutDocCount(ByVal e As EventType2, ByVal line As String, ByVal isEssential As Boolean)
		Sub WriteWarning(ByVal line As String)
		Sub WriteWarningWithoutDocCount(ByVal line As String)
		Sub WriteUpdate(ByVal line As String, Optional ByVal isEssential As Boolean = True)
		Sub UpdateDocumentExportedCount(count As Int32)
		Sub UpdateFilesExportedCount(nativeCount As Int32, pdfCount As Int32, imageCount As Int32, longTextCount As Int32)
	End Interface

End Namespace
