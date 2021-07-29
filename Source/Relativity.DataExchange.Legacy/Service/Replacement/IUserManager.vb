Imports Relativity.DataExchange.Service

Namespace kCura.WinEDDS.Service.Replacement
    Public interface IUserManager
        Inherits IDisposable
        Inherits IReLoginService
        Shadows Function RetrieveAllAssignableInCase(ByVal caseContextArtifactID As Int32) As System.Data.DataSet
        Shadows Function Login(ByVal emailAddress As String, ByVal password As String) As Boolean
        Shadows Function GenerateDistributedAuthenticationToken(Optional ByVal retryOnFailure As Boolean = True) As String
        Shadows Function LoggedIn() As Boolean
        Shadows Sub Logout()
    end interface
End NameSpace