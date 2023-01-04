﻿Namespace kCura.Relativity.DataReaderClient

	''' <summary>
	''' Describes events that the import process can raise.
	''' </summary>
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
		''' <summary>
		''' Occurs when a status message needs to be presented to the user related to the Process.
		''' </summary>
		Event OnProcessProgress(ByVal processStatus As FullStatus)
		''' <summary>
		''' Occurs when a batch is processed.
		''' </summary>
		''' <param name="batchReport">The batch report.</param>
		Event OnBatchComplete(ByVal batchReport As BatchReport)
	End Interface
End Namespace
