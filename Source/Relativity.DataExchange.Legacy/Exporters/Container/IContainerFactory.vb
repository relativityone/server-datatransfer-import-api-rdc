Imports Castle.Windsor
Imports kCura.WinEDDS.Exporters

Namespace kCura.WinEDDS.Container
	Public Interface IContainerFactory
		Inherits IDisposable
		Function Create(exporter As Exporter, columnNamesInOrder As String(), useOldExport As Boolean, loadFileHeaderFormatterFactory As ILoadFileHeaderFormatterFactory) As IWindsorContainer
	End Interface
End Namespace