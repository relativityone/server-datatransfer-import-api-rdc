Imports Relativity.DataExchange.Service

Namespace kCura.WinEDDS.Service.Replacement
    Public interface ICodeManager
        Inherits IDisposable
        Shadows Function RetrieveCodesAndTypesForCase(ByVal caseContextArtifactID As Int32) As System.Data.DataSet
        Shadows Function Create(ByVal caseContextArtifactID As Int32, ByVal code As kCura.EDDS.WebAPI.CodeManagerBase.Code) As Object
        Shadows Function ReadID(ByVal caseContextArtifactID As Int32, ByVal parentArtifactID As Int32, ByVal codeTypeID As Int32, ByVal name As String) As Int32
        Shadows Function GetAllForHierarchical(ByVal caseContextArtifactID As Integer, ByVal codeTypeID As Integer) As System.Data.DataSet
        Shadows Function RetrieveAllCodesOfType(ByVal caseContextArtifactID As Int32, ByVal codeTypeID As Int32) As ChoiceInfo()
        Shadows Function RetrieveCodeByNameAndTypeID(ByVal caseContextArtifactID As Int32, ByVal codeType As kCura.WinEDDS.Api.ArtifactField, ByVal name As String) As ChoiceInfo
        Shadows Function GetChoiceLimitForUI() As Int32

        Function CreateNewCodeDTOProxy(ByVal codeTypeID As Int32, ByVal name As String, ByVal order As Int32, ByVal caseSystemID As Int32) As kCura.EDDS.WebAPI.CodeManagerBase.Code
    end interface
End NameSpace