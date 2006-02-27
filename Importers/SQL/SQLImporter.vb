Namespace kCura.WinEDDS
	Public Class SQLImporter
		Inherits kCura.EDDS.Import.ImporterBase

		Private _sQLImportSettings As SQLImportSettings

		Public Overrides Sub Import()

			Dim num As Int32 = 1000

			Me.StartTime = Date.Now
			Me.TotalRecords = num

			ReportProgress()

			ReportStatus(String.Empty, "Begin SQL Import")

			Dim i As Int32
			For i = 1 To num

				Dim importStatus As String = ""

				ReportStatus(i.ToString, "Begin record import")
				ReportStatus(i.ToString, "Extracinting fields")
				ReportStatus(i.ToString, "Creating database record")

				If i = 32 Then
					ReportWarning(i.ToString, "[xxx] field has been truncated")
					importStatus = "WithWarning"
				End If

				ReportStatus(i.ToString, "Uploading attached file")
				ReportStatus(i.ToString, "Indexing file")

				If i = 69 Then
					ReportError(i.ToString, "Error indexing file")
					importStatus = "WithError"
				End If

				ReportStatus(String.Empty, "End record Import")

				Me.TotalRecordsProcessed = Me.TotalRecordsProcessed + 1
				Select Case importStatus
					Case "WithWarning"
						Me.TotalRecordsProcessedWithWarnings = Me.TotalRecordsProcessedWithWarnings + 1
					Case "WithError"
						Me.TotalRecordsProcessedWithErrors = Me.TotalRecordsProcessedWithErrors + 1
				End Select

				ReportProgress()

			Next

			Me.EndTime = Date.Now
			ReportProgress()

			ReportStatus(String.Empty, "End SQL Import")

		End Sub

		Public Sub New(ByVal importSettings As kCura.WinEDDS.SQLImportSettings)
			_sQLImportSettings = importSettings
		End Sub

	End Class
End Namespace