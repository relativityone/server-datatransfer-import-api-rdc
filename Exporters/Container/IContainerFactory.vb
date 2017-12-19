Imports Castle.Windsor
NameSpace kCura.WinEDDS.Container
	Public Interface IContainerFactory
		Function Create(exporter As Exporter, columnNamesInOrder As String(), useOldExport As Boolean) As IWindsorContainer
	End Interface
End NameSpace