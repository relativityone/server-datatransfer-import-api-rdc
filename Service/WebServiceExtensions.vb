Namespace kCura.WinEDDS.Service
	Public Module WebServiceExtensions

		<System.Runtime.CompilerServices.Extension>
		Public Function RetryOnReLoginException(Of T)(ByVal input As System.Web.Services.Protocols.SoapHttpClientProtocol, serviceCall As Func(Of T), Optional ByVal retryOnFailure As Boolean = True) As T
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					Return serviceCall()
				Catch ex As System.Exception
					If retryOnFailure AndAlso TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(input.Credentials, input.CookieContainer, tries, False)
					Else
						Throw
					End If
				End Try
			End While
			Return Nothing
		End Function

		<System.Runtime.CompilerServices.Extension>
		Public Sub RetryOnReLoginException(ByVal input As System.Web.Services.Protocols.SoapHttpClientProtocol, serviceCall As Action, Optional ByVal retryOnFailure As Boolean = True)
			Dim tries As Int32 = 0
			While tries < Config.MaxReloginTries
				tries += 1
				Try
					serviceCall()
					Exit Sub
				Catch ex As System.Exception
					If retryOnFailure AndAlso TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("NeedToReLoginException") <> -1 AndAlso tries < Config.MaxReloginTries Then
						Helper.AttemptReLogin(input.Credentials, input.CookieContainer, tries, False)
					Else
						Throw
					End If
				End Try
			End While
		End Sub

	End Module
End NameSpace