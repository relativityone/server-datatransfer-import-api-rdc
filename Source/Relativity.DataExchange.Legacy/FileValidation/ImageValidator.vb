Imports OutsideIn

Namespace kCura.WinEDDS

	Public Class ImageValidator
		Implements IImageValidator
		Public Function IsImageValid(filePath As String, tiffValidator As ITiffValidator, fileInspector As IFileInspector) As ImageValidationResult Implements IImageValidator.IsImageValid
			Try
				Return ValidateFileOrThrow(filePath, tiffValidator, fileInspector)
			Catch ex As Exception
				Return New ImageValidationResult(False, ex.Message)
			End Try
		End Function

		Private Function ValidateFileOrThrow(filePath As String, tiffValidator As ITiffValidator, fileInspector As IFileInspector) As ImageValidationResult
			If (filePath Is Nothing)
				Throw New ArgumentNullException(nameof(filePath))
			End If

			If (tiffValidator Is Nothing)
				Throw New ArgumentNullException(nameof(tiffValidator))
			End If

			If (fileInspector Is Nothing)
				Throw New ArgumentNullException(nameof(fileInspector))
			End If

			If (filePath = String.Empty)
				Return New ImageValidationResult(False, "filePath argument is empty")
			End If

			If (System.IO.File.Exists(filePath))
				Return ValidateExistingFile(filePath, tiffValidator, fileInspector)
			Else 
				Return New ImageValidationResult(False, $"File {filePath} doesn't exist")
			End If

		End Function

		Private Function ValidateExistingFile(filePath As String, tiffValidator As ITiffValidator, fileInspector As IFileInspector) As ImageValidationResult
			
			Dim fileType As FileFormat = fileInspector.GetFileFormatByFilePath(filePath)
			If(fileType.GetId() = 6210)
				Return New ImageValidationResult(False, $"File {filePath} is empty")
			ElseIf(Not fileType.Equals(FileFormat.FI_TIFF) AndAlso Not fileType.Equals(FileFormat.FI_JPEGFIF))
				Return New ImageValidationResult(False, $"File {filePath} must be TIFF or JPEG")
			ElseIf(fileType.Equals(FileFormat.FI_TIFF))
				Return tiffValidator.ValidateTiffTags(filePath)
			Else
				Return New ImageValidationResult(True, $"The JPEG image file {filePath} is valid")
			End If

		End Function
	End Class
End Namespace