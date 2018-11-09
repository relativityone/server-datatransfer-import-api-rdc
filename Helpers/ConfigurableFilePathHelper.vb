Imports kCura.WinEDDS.Importers

Namespace kCura.WinEDDS.Helpers
	Public Class ConfigurableFilePathHelper
		Implements IFilePathHelper

		Private ReadOnly _importConfig As IImportConfig
		Private ReadOnly _systemIoWrapper As ISystemIoFileWrapper

		Public Sub New()
			Me.New(new ImportConfig(), New SystemIoFileWrapper())
		End Sub

		Public Sub New(importConfig As IImportConfig, systemIoWrapper As ISystemIoFileWrapper)
			_importConfig = importConfig
			_systemIoWrapper = systemIoWrapper
		End Sub

		Private ReadOnly _internalFilePathHelperLazy As Lazy(Of IFilePathHelper) = New Lazy(Of IFilePathHelper)(AddressOf FilePathHelperFactory)	

		Private Function FilePathHelperFactory() As IFilePathHelper
			Return If(_importConfig.EnableCaseSensitiveSearchOnImport,
				New CaseSensitiveFilePathHelper(_systemIoWrapper),
				New CaseInsensitiveFilePathHelper(_systemIoWrapper))
		End Function

		Public Function GetExistingFilePath(path As String) As String Implements IFilePathHelper.GetExistingFilePath
			Return _internalFilePathHelperLazy.Value.GetExistingFilePath(path)
		End Function
	End Class
End NameSpace