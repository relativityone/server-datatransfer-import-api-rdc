Imports System.Collections.Generic
Imports Relativity.DataTransfer.MessageService
Imports Relativity.DataTransfer.MessageService.MetricsManager.APM

Namespace kCura.WinEDDS.Monitoring
	Public class TransferJobMessageBase
		Implements IMessage, IMetricMetadata

		Public Property JobType As String
		Public Property TransferMode As String

		Public Property CorellationID As String Implements IMetricMetadata.CorellationID
		Public Property CustomData As Dictionary(Of String,Object) Implements IMetricMetadata.CustomData
		Public Property WorkspaceID As Integer Implements IMetricMetadata.WorkspaceID
		Public Property UnitOfMeasure As String Implements IMetricMetadata.UnitOfMeasure

		Public Sub New()
			CustomData = New Dictionary(Of String,Object)
		End Sub

	end class
End NameSpace