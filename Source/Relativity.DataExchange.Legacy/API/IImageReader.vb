Namespace kCura.WinEDDS.Api
	Public Interface IImageReader
		Sub AdvanceRecord()
		Sub Cancel()
		Sub Close()
		Sub Initialize()
		Function CountRecords() As Int64
		ReadOnly Property CurrentRecordNumber() As Int32
		Function GetImageRecord() As ImageRecord
		ReadOnly Property HasMoreRecords() As Boolean
	End Interface
End Namespace
