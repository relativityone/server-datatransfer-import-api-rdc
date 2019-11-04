Imports kCura.WinEDDS
Imports Relativity.DataExchange

Friend NotInheritable Class WebApiCredentialSetter

	Public Shared Sub PopulateNativeLoadFile(tempLoadFile As LoadFile)

		If tempLoadFile.WebApiCredential Is Nothing Then
			tempLoadFile.WebApiCredential = New WebApiCredential() With {
				.TokenProvider = New NullAuthTokenProvider(),
				.Credential = tempLoadFile.Credentials
			}
		End If

	End Sub

	Public Shared Sub PopulateImageLoadFile(imageLoadFile As ImageLoadFile)

		If imageLoadFile.WebApiCredential Is Nothing Then
			imageLoadFile.WebApiCredential = New WebApiCredential() With {
				.TokenProvider = New NullAuthTokenProvider(),
				.Credential = imageLoadFile.Credential
				}
		End If
	End Sub

End Class
