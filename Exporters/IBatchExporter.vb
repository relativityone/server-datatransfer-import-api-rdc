Imports System.Threading
Imports kCura.WinEDDS.Exporters

Namespace kCura.WinEDDS
	Public Interface IBatchExporter
		Sub Export(artifacts As ObjectExportInfo(), records As Object(), cancellationToken As CancellationToken)
	End Interface

End Namespace
