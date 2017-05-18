Imports kCura.Windows.Process
Imports Relativity

Namespace kCura.WinEDDS
	Public Interface IBulkLoadFileImporterFactory
		Function Create(args As LoadFile, processController As Controller, timeZoneOffset As Int32,
						initializeUploaders As Boolean, processID As Guid, doRetryLogic As Boolean, bulkLoadFileFieldDelimiter As String,
						cloudInstance As Boolean,
						ByVal Optional executionSource As ExecutionSource = ExecutionSource.Unknown) As BulkLoadFileImporter
	End Interface
End Namespace