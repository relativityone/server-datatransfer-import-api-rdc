Namespace kCura.WinEDDS.Service
	Public Class FieldSpecificCodeManager
		Implements Service.IHierarchicArtifactManager
		Private _codeManager As kCura.WinEDDS.Service.CodeManager
		Private _codeTypeID As Int32
		Public Sub New(ByVal codeManager As kCura.WinEDDS.Service.CodeManager, ByVal codeTypeID As Int32)
			_codeManager = codeManager
			_codeTypeID = codeTypeID
		End Sub

		Public Function Create(ByVal caseContextArtifactID As Integer, ByVal parentArtifactID As Integer, ByVal name As String) As Integer Implements IHierarchicArtifactManager.Create
			Dim code As kCura.EDDS.WebAPI.CodeManagerBase.Code = _codeManager.CreateNewCodeDTOProxy(_codeTypeID, name, 1, parentArtifactID)
			Return _codeManager.Create(caseContextArtifactID, code)
		End Function

		Public Function RetrieveArtifacts(ByVal caseContextArtifactID As Integer, ByVal rootArtifactID As Integer) As System.Data.DataSet Implements IHierarchicArtifactManager.RetrieveArtifacts
			Return _codeManager.GetAllForHierarchical(caseContextArtifactID, _codeTypeID)
		End Function
	End Class
End Namespace

