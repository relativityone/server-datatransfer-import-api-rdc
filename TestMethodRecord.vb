Imports NUnit.Framework
Imports kCura.Utility.NullableTypesHelper
Imports kCura.WinEDDS.TestHarness.Fixtures
Namespace kCura.WinEDDS.NUnit
	Public Class TestMethodRecord
		Inherits kCura.UnitTest.NUnit.TestMethodRecord

		Public Sub RunDocumentFieldCompareTest(ByVal expected As kCura.WinEDDS.DocumentField, ByVal actual As kCura.WinEDDS.DocumentField)
			Dim testPassed As Boolean
			testPassed = (expected.FieldCategoryID = actual.FieldCategoryID)
			testPassed = testPassed AndAlso (expected.FieldID = actual.FieldID)
			testPassed = testPassed AndAlso (expected.FieldName = actual.FieldName)
			testPassed = testPassed AndAlso (expected.FieldTypeID = actual.FieldTypeID)
			testPassed = testPassed AndAlso (ToEmptyStringOrValue(expected.CodeTypeID) = ToEmptyStringOrValue(actual.CodeTypeID))
			RunTest(testPassed, expected.ToDisplayString, actual.ToDisplayString)
		End Sub

		Public Sub RunDirectiveFileTest(ByVal directiveFilePath As String)
			Dim cred As Net.NetworkCredential = DirectCast(System.Net.CredentialCache.DefaultCredentials, Net.NetworkCredential)
			System.Net.ServicePointManager.CertificatePolicy = New kCura.WinEDDS.TrustAllCertificatePolicy
			Dim cookieContainer As New System.Net.CookieContainer
			Dim parser As New DirectiveFileParser(directiveFilePath)
			Dim settings As TestSettings = parser.ReadFile()
			If settings Is Nothing Then Throw New System.Exception("Setting creation failed")
			Dim caseBuilder As New caseBuilder(cred, settings, cookieContainer)
			Dim testRunner As New testRunner(cred, settings, cookieContainer)
			Dim caseInfo As kCura.EDDS.Types.CaseInfo = caseBuilder.BuildCase()
			Dim actualResults As TestResults = testRunner.RunTest(caseInfo)
			Dim caseManager As New kCura.WinEDDS.Service.CaseManager(cred, cookieContainer, Nothing)
			caseManager.Delete(caseInfo.ArtifactID)
			Dim testpassed As Boolean = TestResults.op_Equality(settings.ExpectedResults, actualResults)
			If testpassed Then
				RunTest(True)
			Else
				RunTest(False, settings.ExpectedResults.ToDisplayString, actualResults.ToDisplayString)
			End If
		End Sub

	End Class

End Namespace