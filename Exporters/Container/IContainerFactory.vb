Imports Castle.Windsor
Imports kCura.WinEDDS.Exporters
Imports kCura.WinEDDS.Service.Export

NameSpace kCura.WinEDDS.Container
	Public Interface IContainerFactory
		Function Create(exportSettings As ExportFile, exportManager As IExportManager, userNotification As IUserNotification) As IWindsorContainer
	End Interface
End NameSpace