Imports System.Threading
Imports kCura.WinEDDS.Exporters

Namespace kCura.WinEDDS
	Public Interface IBatch
		Sub Export(artifacts As ObjectExportInfo(), volumePredictions As VolumePredictions(), cancellationToken As CancellationToken)
	End Interface

End Namespace
