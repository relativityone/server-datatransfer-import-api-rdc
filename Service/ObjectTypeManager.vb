Namespace kCura.WinEDDS.Service
  Public Class ObjectTypeManager
    Inherits kCura.EDDS.WebAPI.ObjectTypeManagerBase.ObjectTypeManager

#Region " Constructor and Initialization "
    Protected Overrides Function GetWebRequest(ByVal uri As System.Uri) As System.Net.WebRequest
      Dim wr As System.Net.HttpWebRequest = DirectCast(MyBase.GetWebRequest(uri), System.Net.HttpWebRequest)
      wr.UnsafeAuthenticatedConnectionSharing = True
      wr.Credentials = Me.Credentials
      Return wr
    End Function

    Public Sub New(ByVal credentials As Net.NetworkCredential, ByVal cookieContainer As System.Net.CookieContainer)
      MyBase.New()
      Me.Credentials = credentials
      Me.CookieContainer = cookieContainer
      Me.Url = String.Format("{0}ObjectTypeManager.asmx", kCura.WinEDDS.Config.WebServiceURL)
      Me.Timeout = Settings.DefaultTimeOut
    End Sub

#End Region

#Region " Shadow Implementations "

    Public Shadows Function RetrieveAllUploadable(ByVal caseContextArtifactID As Int32) As System.Data.DataSet
      Dim tries As Int32 = 0
      While tries < Config.MaxReloginTries
        tries += 1
        Try
          If kCura.WinEDDS.Config.UsesWebAPI Then
            Return MyBase.RetrieveAllUploadable(caseContextArtifactID)
          End If
        Catch ex As System.Exception
          If TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
            Helper.AttemptReLogin(Me.Credentials, Me.CookieContainer, tries)
          Else
            Throw
          End If
        End Try
      End While
    End Function

#End Region

  End Class
End Namespace
