﻿Imports Relativity.DataTransfer.MessageService

Namespace kCura.WinEDDS.Monitoring
	Public Class TransferJobThroughputMessage
		Inherits TransferJobMessageBase
		Public Property RecordsPerSecond As Double
		Public Property BytesPerSecond As Double
	End Class
End Namespace