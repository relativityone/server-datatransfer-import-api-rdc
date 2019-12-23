Imports System.Threading
Imports System.Threading.Tasks
Imports kCura.WinEDDS.Exporters

Namespace kCura.WinEDDS
	Public Interface IBatch
		Function ExportAsync(artifacts As ObjectExportInfo(), volumePredictions As VolumePredictions(), cancellationToken As CancellationToken) As Task
	End Interface

End Namespace
