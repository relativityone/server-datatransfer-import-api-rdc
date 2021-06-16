Namespace kCura.WinEDDS

	Public Interface IImageValidator
		Function IsImageValid(ByVal filePath As String, ByVal tiffValidator As ITiffValidator, ByVal fileInspector As IFileInspector) As ImageValidationResult
	End Interface

End Namespace