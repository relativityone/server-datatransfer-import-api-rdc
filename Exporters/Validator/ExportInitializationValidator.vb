Namespace kCura.WinEDDS.Exporters.Validator

	Public Class ExportInitializationValidator
		Public Function IsValid(ByVal settings As ExportFile) As String
			Dim msg As New System.Text.StringBuilder
			If String.IsNullOrWhiteSpace(settings.FolderPath) OrElse Not System.IO.Directory.Exists(settings.FolderPath) Then
				If String.IsNullOrWhiteSpace(settings.FolderPath) Then
					AppendErrorMessage(msg, "Export destination folder empty")
				Else
					AppendErrorMessage(msg, "Export destination folder does not exist")
				End If
			End If
			If settings.ExportImages Then
				If Not settings.LogFileFormat.HasValue Then
					AppendErrorMessage(msg, "No image data file format selected")
				End If
				If Not settings.TypeOfImage.HasValue Then
					AppendErrorMessage(msg, "No image file type selected")
				End If
			End If
			If settings.TypeOfExport = ExportFile.ExportType.Production Then
				If settings.ExportNative Then
					If settings.ExportNativesToFileNamedFrom = ExportNativeWithFilenameFrom.Select Then
						AppendErrorMessage(msg, "No file name source selected")
					End If
				End If
			End If
			If settings.LoadFileEncoding Is Nothing Then
				AppendErrorMessage(msg, "No encoding selected for metadata file.")
			End If
			If settings.ExportFullTextAsFile Then
				If settings.TextFileEncoding Is Nothing Then
					AppendErrorMessage(msg, "No encoding selected for text field files.")
				End If
				If settings.SelectedTextFields IsNot Nothing AndAlso Not settings.SelectedTextFields.Any() Then
					AppendErrorMessage(msg, "When exporting text field as files is selected, you must set text precedence.")
				End If
			End If
			Dim createVolume As Boolean = settings.ExportImages OrElse settings.ExportNative

			If createVolume Then
				Dim info As VolumeInfo = If(settings.VolumeInfo, New VolumeInfo())
				If String.IsNullOrWhiteSpace(info.SubdirectoryImagePrefix) Then AppendErrorMessage(msg, "Subdirectory Image Prefix cannot be blank.")
				If String.IsNullOrWhiteSpace(info.SubdirectoryFullTextPrefix) Then AppendErrorMessage(msg, "Subdirectory Text Prefix cannot be blank.")
				If info.SubdirectoryMaxSize < 1 Then AppendErrorMessage(msg, "Subdirectory Max Size must be greater than zero.")
				If String.IsNullOrWhiteSpace(info.SubdirectoryNativePrefix) Then AppendErrorMessage(msg, "Subdirectory Native Prefix cannot be blank.")
				If info.SubdirectoryStartNumber < 1 Then AppendErrorMessage(msg, "Subdirectory Start Number must be greater than zero.")
				If info.VolumeMaxSize < 1 Then AppendErrorMessage(msg, "Volume Max Size must be greater than zero.")
				If String.IsNullOrWhiteSpace(info.VolumePrefix) Then AppendErrorMessage(msg, "Volume Prefix cannot be blank.")
				If info.VolumeStartNumber < 1 Then AppendErrorMessage(msg, "Volume Start Number must be greater than zero.")
			End If
			If msg.ToString.Trim <> String.Empty Then
				msg.Insert(0, "The following issues need to be addressed before continuing:" & vbNewLine & vbNewLine)
			End If
			Return msg.ToString()
		End Function

		Private Sub AppendErrorMessage(ByVal msg As System.Text.StringBuilder, ByVal errorText As String)
			msg.Append(" - ").Append(errorText).Append(vbNewLine)
		End Sub


	End Class
End Namespace
