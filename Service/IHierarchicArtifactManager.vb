Namespace kCura.WinEDDS.Service
	Public Interface IHierarchicArtifactManager
		Function Create(ByVal caseContextArtifactID As Int32, ByVal parentArtifactID As Int32, ByVal name As String) As Int32
		Function RetrieveArtifacts(ByVal caseContextArtifactID As Int32, ByVal rootArtifactID As Int32) As System.Data.DataSet
	End Interface
End Namespace

