Imports System.Runtime.CompilerServices
Imports Relativity.DataTransfer.MessageService.MetricsManager.APM

Namespace Monitoring
	Module MetricMetadataExtensions
		<Extension()>
			Function GetValueOrDefault(Of T)(ByVal this As IMetricMetadata, ByVal key As String) As T
			Dim value As Object = Nothing
			Return If(this.CustomData.TryGetValue(key, value), CType(value, T), Nothing)
		End Function
	End Module
End Namespace