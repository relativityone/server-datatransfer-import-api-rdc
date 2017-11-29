Imports Castle.Windsor
NameSpace kCura.WinEDDS.Container
	Public Interface IContainerFactory
		Function Create(exporter As Exporter, columnHeader As String, columnNamesInOrder As String()) As IWindsorContainer
	End Interface
End NameSpace