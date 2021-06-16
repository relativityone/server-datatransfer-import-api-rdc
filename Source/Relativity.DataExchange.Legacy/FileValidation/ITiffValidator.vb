Namespace kCura.WinEDDS

	Public Interface ITiffValidator

		Function ValidateTiffTags(ByVal filePath As String) As ImageValidationResult

	End Interface

End Namespace