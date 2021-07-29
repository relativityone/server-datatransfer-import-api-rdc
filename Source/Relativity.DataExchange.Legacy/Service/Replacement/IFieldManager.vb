Namespace kCura.WinEDDS.Service.Replacement
    Public Interface IFieldManager
        Inherits Export.IFieldManager
        Inherits IDisposable
        Shadows Function Read(ByVal caseContextArtifactID As Int32, ByVal fieldArtifactID As Int32) As kCura.EDDS.WebAPI.FieldManagerBase.Field
    End Interface
End Namespace
