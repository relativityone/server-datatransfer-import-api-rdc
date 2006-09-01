Namespace kCura.WinEDDS
	Public Class TrustAllCertificatePolicy
		Implements System.Net.ICertificatePolicy

		Public Sub New()

		End Sub

		Public Function CheckValidationResult(ByVal srvPoint As System.Net.ServicePoint, ByVal certificate As System.Security.Cryptography.X509Certificates.X509Certificate, ByVal request As System.Net.WebRequest, ByVal certificateProblem As Integer) As Boolean Implements System.Net.ICertificatePolicy.CheckValidationResult
			Return True
		End Function

	End Class
End Namespace