Public Class CertificatePromptForm
	''' <summary>
	''' The user has confirmed they are OK with untrusted certificates. This tries logging in again after explicitly allowing them
	''' </summary>
	''' <remarks></remarks>
	Public Event AllowUntrustedCertificates As Action
	Public Event DenyUntrustedCertificates As Action

	Private CertsAllowed As Nullable(Of Boolean)

	Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
		Me.Focus()
		MyBase.OnLoad(e)
	End Sub
	
	Protected Overrides Sub OnClosing(ByVal e As System.ComponentModel.CancelEventArgs)
		If(Not CertsAllowed.HasValue)
			CertsAllowed = False
			RaiseEvent DenyUntrustedCertificates()
		End If
		MyBase.OnClosing(e)
	End Sub

	Private Sub CloseRDC_Click(sender As Object, e As EventArgs) Handles CloseButton.Click
		CertsAllowed = False
		RaiseEvent DenyUntrustedCertificates()
	End Sub

	Private Sub AllowUntrustedCertificates_Click(sender As Object, e As EventArgs) Handles AllowButton.Click
		CertsAllowed = True
		Close()
		RaiseEvent AllowUntrustedCertificates()
	End Sub
End Class