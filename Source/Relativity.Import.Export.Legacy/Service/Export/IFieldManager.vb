Namespace kCura.WinEDDS.Service.Export
	Public Interface IFieldManager
		Function Read(ByVal caseContextArtifactID As Int32, ByVal fieldArtifactID As Int32) As kCura.EDDS.WebAPI.FieldManagerBase.Field
	End Interface
End Namespace