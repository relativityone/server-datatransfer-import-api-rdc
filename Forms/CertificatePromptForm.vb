Public Class CertificatePromptForm
	''' <summary>
	''' The user has confirmed they are OK with untrusted certificates. This tries logging in again after explicitly allowing them
	''' </summary>
	''' <remarks></remarks>
	Public Event AllowUntrustedCertificates As Action
	Public Event DenyUntrustedCertificates As Action

	Private Sub LoginForm_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
		Me.Focus()
	End Sub

	Private Sub CloseRDC_Click(sender As Object, e As EventArgs) Handles CloseButton.Click
		RaiseEvent DenyUntrustedCertificates()
	End Sub

	Private Sub AllowUntrustedCertificates_Click(sender As Object, e As EventArgs) Handles AllowButton.Click
		Close()
		RaiseEvent AllowUntrustedCertificates()
	End Sub
End Class