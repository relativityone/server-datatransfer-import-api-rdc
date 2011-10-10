Namespace kCura.WinEDDS.Api
	Public Interface IArtifactReader
		ReadOnly Property HasMoreRecords() As Boolean
		ReadOnly Property CurrentLineNumber() As Int32
		ReadOnly Property SizeInBytes() As Int64
		ReadOnly Property BytesProcessed() As Long

		Function ReadArtifact() As Api.ArtifactFieldCollection
		Function GetColumnNames(ByVal args As Object) As String()
		Function CountRecords() As Int64

		Sub AdvanceRecord()
		Sub Close()
		Function ManageErrorRecords(ByVal errorMessageFileLocation As String, ByVal prePushErrorLineNumbersFileName As String) As String
		Sub OnFatalErrorState()
		Sub Halt()

		Event OnIoWarning(ByVal e As kCura.WinEDDS.Api.IoWarningEventArgs)
		Event DataSourcePrep(ByVal e As Api.DataSourcePrepEventArgs)
		Event StatusMessage(ByVal message As String)
		Event FieldMapped(ByVal sourceField As String, ByVal workspaceField As String)

	End Interface
End Namespace

