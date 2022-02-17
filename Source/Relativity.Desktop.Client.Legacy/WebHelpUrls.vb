Namespace Relativity.Desktop.Client

	''' <summary>
	''' Defines all online web help URLs used by the application. 
	''' </summary>
	Public Class WebHelpUrls

		Private Const UrlPrefix As String = "https://help.relativity.com/"

		''' <summary>
		''' Gets the RDC home page URL.
		''' </summary>
		''' <param name="cloudInstance">
		''' The value indicating whether the Relativity instance is cloud-based.
		''' </param>
		''' <returns>
		''' The URL.
		''' </returns>
		Public Shared Function GetHomePageUrl(ByVal cloudInstance As Boolean) As String
			Dim productVersion As System.Version = Application.GetProductVersion()

			' Always direct the user to the R1 site when this application is installed stand-alone.
			If cloudInstance OrElse productVersion Is Nothing Then
				Return $"{UrlPrefix}RelativityOne/Content/Relativity/Relativity_Desktop_Client/Relativity_Desktop_Client.htm"
			Else
				Return $"{UrlPrefix}"
			End If
		End Function

		''' <summary>
		''' Gets the RDC compatibility page page URL.
		''' </summary>
		''' <returns>
		''' The URL.
		''' </returns>
		Public Shared Function GetCompatibilityPageUrl() As String
			' TODO: Update URL with the page once it's been staged.
			' TODO: This shouldn't require the cloudInstance flag since it must cover a range of releases.
			Return urlPrefix & "RelativityOne/Content/Relativity/Relativity_Desktop_Client/Relativity_Desktop_Client.htm"
		End Function
	End Class
End Namespace