Namespace kCura.Relativity.DataReaderClient

	Public Interface IImportNotifier
		''' <summary>
		''' Occurs when all the data for an import job has been processed.  Raised at the end of an import.
		''' </summary>
		''' <param name="jobReport">The JobReport describing the completed import job.</param>
		Event OnComplete(ByVal jobReport As JobReport)
		''' <summary>
		''' Occurs when an import job suffers a fatal exception and aborts.  Raised at the end of an import.
		''' </summary>
		''' <param name="jobReport">The JobReport describing the failed import job.</param>
		Event OnFatalException(ByVal jobReport As JobReport)
		''' <summary>
		''' Occurs when a record has been processed.
		''' </summary>
		''' <param name="completedRow">The processed record.</param>
		Event OnProgress(ByVal completedRow As Long)

	End Interface
End Namespace
