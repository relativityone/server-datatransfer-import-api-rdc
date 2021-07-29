Imports kCura.WinEDDS.Mapping
Imports Relativity.DataExchange.Service
Imports Relativity.DataTransfer.Legacy.SDK.ImportExport.V1

Namespace kCura.WinEDDS.Service.Replacement
    Public Class KeplerUserManager
        Inherits KeplerManager
        Implements IUserManager

        Public Sub New(serviceProxyFactory As IServiceProxyFactory, typeMapper As ITypeMapper, exceptionMapper As IServiceExceptionMapper, correlationIdFunc As Func(Of String))
            MyBase.New(serviceProxyFactory, typeMapper, exceptionMapper, correlationIdFunc)
        End Sub

        Public Sub Logout() Implements IUserManager.Logout
        End Sub

        Public Function RetrieveAllAssignableInCase(caseContextArtifactID As Integer) As DataSet Implements IUserManager.RetrieveAllAssignableInCase
            Return Execute(Async Function(s)
                Using service As IUserService = s.CreateProxyInstance(Of IUserService)
                                   Return Await service.RetrieveAllAssignableInCaseAsync(caseContextArtifactID, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                               End Using
            End Function)
        End Function

        Public Function Login(emailAddress As String, password As String) As Boolean Implements IUserManager.Login
            Return True
        End Function

        Public Function GenerateDistributedAuthenticationToken(Optional retryOnFailure As Boolean = True) As String Implements IUserManager.GenerateDistributedAuthenticationToken
            Return string.Empty
        End Function

        Public Function LoggedIn() As Boolean Implements IUserManager.LoggedIn
            Return True
        End Function

        Public Function AttemptReLogin(Optional retryOnFailure As Boolean = True) As Boolean Implements IReLoginService.AttemptReLogin
            Return True
        End Function
    End Class
End Namespace
