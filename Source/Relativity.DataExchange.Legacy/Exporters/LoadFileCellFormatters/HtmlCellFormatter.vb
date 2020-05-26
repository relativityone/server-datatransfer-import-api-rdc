Imports Relativity.DataExchange.Service

Namespace kCura.WinEDDS.Exporters

	<Obsolete>
	Public Class HtmlCellFormatter
		Implements ILoadFileCellFormatter
		Private _settings As kCura.WinEDDS.ExportFile
		Private Const ROW_PREFIX As String = "<tr>"
		Private Const ROW_SUFFIX As String = "</tr>"
		Private Const HyperlinkTagTemplate As String = "<a style='display:block' href='{0}'>{1}</a>"
		Private Const CellTagTemplate As String = "<td>{0}</td>"
		Public Sub New(ByVal settings As kCura.WinEDDS.ExportFile)
			_settings = settings
		End Sub

		Public Function TransformToCell(ByVal contents As String) As String Implements ILoadFileCellFormatter.TransformToCell
			contents = System.Web.HttpUtility.HtmlEncode(contents)
			Return String.Format("{0}{1}{2}", "<td>", contents, "</td>")
		End Function

		Private Function GetNativeHtmlString(ByVal artifact As Exporters.ObjectExportInfo, ByVal location As String) As String
			If _settings.ArtifactTypeID = ArtifactType.Document AndAlso artifact.NativeCount = 0 Then Return String.Empty
			If Not _settings.ArtifactTypeID = ArtifactType.Document AndAlso Not artifact.FileID > 0 Then Return String.Empty
			Return String.Format(HyperlinkTagTemplate, location, artifact.NativeFileName(_settings.AppendOriginalFileName))
		End Function

		Private Function GetPdfHtmlString(ByVal artifact As Exporters.ObjectExportInfo, ByVal location As String) As String
			If _settings.ArtifactTypeID <> ArtifactType.Document OrElse Not artifact.HasPdf Then Return String.Empty
			Return String.Format(HyperlinkTagTemplate, location, artifact.PdfFileName(artifact.IdentifierValue, _settings.AppendOriginalFileName))
		End Function

		Public ReadOnly Property RowPrefix() As String Implements ILoadFileCellFormatter.RowPrefix
			Get
				Return ROW_PREFIX
			End Get
		End Property

		Public ReadOnly Property RowSuffix() As String Implements ILoadFileCellFormatter.RowSuffix
			Get
				Return ROW_SUFFIX
			End Get
		End Property

		Private Function GetImagesHtmlString(ByVal artifact As Exporters.ObjectExportInfo) As String
			If artifact.Images.Count = 0 Then Return ""
			Dim retval As New System.Text.StringBuilder
			For Each image As Exporters.ImageExportInfo In artifact.Images
				Dim loc As String = image.TempLocation
				If Not _settings.VolumeInfo.CopyImageFilesFromRepository Then
					loc = image.SourceLocation
				End If
				retval.AppendFormat(HyperlinkTagTemplate, loc, image.FileName)
				If _settings.TypeOfImage = ExportFile.ImageType.MultiPageTiff OrElse _settings.TypeOfImage = ExportFile.ImageType.Pdf Then Exit For
			Next
			Return retval.ToString
		End Function

		Public Function CreateImageCell(ByVal artifact As Exporters.ObjectExportInfo) As String Implements ILoadFileCellFormatter.CreateImageCell
			If Not _settings.ExportImages OrElse _settings.ArtifactTypeID <> ArtifactType.Document Then Return String.Empty
			Return String.Format(CellTagTemplate, Me.GetImagesHtmlString(artifact))
		End Function

		Public Function CreateNativeCell(ByVal location As String, ByVal artifact As ObjectExportInfo) As String Implements ILoadFileCellFormatter.CreateNativeCell
			Return String.Format(CellTagTemplate, Me.GetNativeHtmlString(artifact, location))
		End Function

		Public Function CreatePdfCell(location As String, artifact As ObjectExportInfo) As String Implements ILoadFileCellFormatter.CreatePdfCell
			Return String.Format(CellTagTemplate, Me.GetPdfHtmlString(artifact, location))
		End Function
	End Class
End Namespace

