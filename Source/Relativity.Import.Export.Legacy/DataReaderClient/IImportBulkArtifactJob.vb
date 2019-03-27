Namespace kCura.Relativity.DataReaderClient

	''' <summary>
	''' Describe the functionality for importing Artifacts into a workspace, setting import parameters, loading data, and retrieving messages from the OnMessage event.
	''' </summary>
	Public Interface IImportBulkArtifactJob

		''' <summary>
		''' Executes the DataReaderClient, which operates as an iterator over a data source.
		''' </summary>
		Sub Execute()

		''' <summary>
		''' Exports the error log file for an import job. This file is written only when errors occur.
		''' </summary>
		''' <param name="filePathAndName">Specifies a full path and a filename to contain the output.</param>
		Sub ExportErrorReport(ByVal filePathAndName As String)

		''' <summary>
		''' Exports the error file for an import job. This file is written only when errors occur.
		''' </summary>
		''' <param name="filePathAndName">The folder path and file name to export the error file</param>
		Sub ExportErrorFile(ByVal filePathAndName As String)

	End Interface

End Namespace