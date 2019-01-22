Imports kCura.WinEDDS.Importers

Namespace kCura.WinEDDS.Helpers
	Public Class ConfigurableFilePathHelper
		Implements IFilePathHelper

		Private ReadOnly _importConfig As IImportConfig
		Private ReadOnly _fileSystem As kCura.WinEDDS.TApi.IFileSystem

		Public Sub New()
			Me.New(new ImportConfig(), kCura.WinEDDS.TApi.FileSystem.Instance)
		End Sub

		Public Sub New(importConfig As IImportConfig, fileSystem As kCura.WinEDDS.TApi.IFileSystem)
			_importConfig = importConfig
			_fileSystem = fileSystem
		End Sub

		Private ReadOnly _internalFilePathHelperLazy As Lazy(Of IFilePathHelper) = New Lazy(Of IFilePathHelper)(AddressOf FilePathHelperFactory)	

		Private Function FilePathHelperFactory() As IFilePathHelper
			Return If(_importConfig.EnableCaseSensitiveSearchOnImport,
				CType(New CaseSensitiveFilePathHelper(_fileSystem), IFilePathHelper),
				New CaseInsensitiveFilePathHelper(_fileSystem))
		End Function

		Public Function GetExistingFilePath(path As String) As String Implements IFilePathHelper.GetExistingFilePath
			Return _internalFilePathHelperLazy.Value.GetExistingFilePath(path)
		End Function
	End Class
End NameSpace