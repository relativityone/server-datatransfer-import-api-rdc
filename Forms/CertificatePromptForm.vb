Public Class CertificatePromptForm

	Private Sub LoginForm_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
		Me.Focus()
	End Sub

	Private Sub CloseRDC_Click(sender As Object, e As EventArgs) Handles CloseButton.Click
		Environment.Exit(0)
	End Sub

	Private Sub AllowUntrustedCertificates_Click(sender As Object, e As EventArgs) Handles AllowButton.Click

	End Sub
End Class