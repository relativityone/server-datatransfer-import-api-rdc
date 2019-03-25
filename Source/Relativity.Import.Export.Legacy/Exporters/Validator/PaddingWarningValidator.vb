
Namespace kCura.WinEDDS.Exporters.Validator
	Public Class PaddingWarningValidator
		Private _messages As String = String.Empty
		Public ReadOnly Property ErrorMessages As String
			Get
				Return _messages
			End Get
		End Property
		Public Function IsValid(settings As ExportFile, recommendedVolumeLabelPaddingWidth As Int32, recommendedSubdirectoryLabelPaddingWidth As Int32) As Boolean
			Dim couldExportExceedLabelPadding As Boolean = Not (recommendedVolumeLabelPaddingWidth <= settings.VolumeDigitPadding AndAlso recommendedSubdirectoryLabelPaddingWidth <= settings.SubdirectoryDigitPadding)
			Dim arePhysicalFilesBeingExported As Boolean =
				(settings.ExportFullText AndAlso settings.ExportFullTextAsFile) OrElse
				(settings.ExportImages AndAlso settings.VolumeInfo.CopyImageFilesFromRepository) OrElse
				(settings.ExportNative AndAlso settings.VolumeInfo.CopyNativeFilesFromRepository)
			Dim isInvalid As Boolean = couldExportExceedLabelPadding AndAlso arePhysicalFilesBeingExported
			If isInvalid Then
				Dim message As New System.Text.StringBuilder
				If recommendedVolumeLabelPaddingWidth > settings.VolumeDigitPadding Then message.AppendFormat("The selected volume padding of {0} is less than the recommended volume padding {1} for this export" & vbNewLine, settings.VolumeDigitPadding, recommendedVolumeLabelPaddingWidth)
				If recommendedSubdirectoryLabelPaddingWidth > settings.SubdirectoryDigitPadding Then message.AppendFormat("The selected subdirectory padding of {0} is less than the recommended subdirectory padding {1} for this export" & vbNewLine, settings.SubdirectoryDigitPadding, recommendedSubdirectoryLabelPaddingWidth)
				message.Append("Continue with this selection?")
				_messages = message.ToString()
			End If
			Return Not isInvalid
		End Function
	End Class
End Namespace
