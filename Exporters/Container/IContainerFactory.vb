Imports Castle.Windsor
NameSpace kCura.WinEDDS.Container
	Public Interface IContainerFactory
		Function Create(exporter As Exporter, columnNamesInOrder As String(), useOldExport As Boolean, columnHeaderString As String) As IWindsorContainer
	End Interface
End NameSpace