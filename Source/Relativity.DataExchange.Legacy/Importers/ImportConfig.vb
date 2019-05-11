Imports Relativity.DataExchange

Namespace kCura.WinEDDS.Importers
	Public Class ImportConfig
		Implements IImportConfig

		Public ReadOnly Property EnableCaseSensitiveSearchOnImport As Boolean Implements IImportConfig.EnableCaseSensitiveSearchOnImport
			Get
				Return AppSettings.Instance.EnableCaseSensitiveSearchOnImport
			End Get
		End Property
	End Class
End NameSpace