Imports Castle.Windsor
Imports kCura.WinEDDS.Exporters
Imports kCura.WinEDDS.Service.Export

NameSpace kCura.WinEDDS.Container
	Public Interface IContainerFactory
		Function Create(exportSettings As ExportFile, columns As ArrayList, columnHeader As String, columnNamesInOrder As String(), exportManager As IExportManager, userNotification As IUserNotification, fileNameProvider As IFileNameProvider) As IWindsorContainer
	End Interface
End NameSpace