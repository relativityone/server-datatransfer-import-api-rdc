Namespace kCura.Relativity.DataReaderClient

	Public Interface IImportNotifier
		Event OnComplete(ByVal jobReport As JobReport)
		Event OnFatalException(ByVal jobReport As JobReport)
		Event OnProgress(ByVal completedRow As Long)

	End Interface
End Namespace
