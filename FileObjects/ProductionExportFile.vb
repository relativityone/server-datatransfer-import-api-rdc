Namespace kCura.WinEDDS
	Public Class ProductionExportFile
		Protected _caseInfo As kCura.WinEDDS.CaseInfo
		Protected _productionList As System.Data.DataTable
		Protected _folderPath As String
		Protected _productionArtifactID As Int32
		Protected _overwrite As Boolean
		Protected _credential As Net.NetworkCredential

		Public Property CaseInfo() As kCura.WinEDDS.CaseInfo
			Get
				Return _caseInfo
			End Get
			Set(ByVal value As kCura.WinEDDS.CaseInfo)
				_caseInfo = value
			End Set
		End Property

		Public Property ProductionList() As System.Data.DataTable
			Get
				Return _productionList
			End Get
			Set(ByVal value As System.Data.DataTable)
				_productionList = value
			End Set
		End Property

		Public Property FolderPath() As String
			Get
				Return _folderPath
			End Get
			Set(ByVal value As String)
				_folderPath = value
			End Set
		End Property

		Public Property ProductionArtifactID() As Int32
			Get
				Return _productionArtifactID
			End Get
			Set(ByVal value As Int32)
				_productionArtifactID = value
			End Set
		End Property

		Public Property Overwrite() As Boolean
			Get
				Return _overwrite
			End Get
			Set(ByVal value As Boolean)
				_overwrite = value
			End Set
		End Property

		Public Property Credential() As Net.NetworkCredential
			Get
				Return _credential
			End Get
			Set(ByVal value As Net.NetworkCredential)
				_credential = value
			End Set
		End Property
	End Class
End Namespace