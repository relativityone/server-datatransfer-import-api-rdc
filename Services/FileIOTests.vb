Imports System.Net
Imports System.Web.Services.Protocols	'used by SoapException
Imports Rhino.Mocks
Imports NUnit.Framework
Imports kCura.WinEDDS.Service

Namespace kCura.WinEDDS.NUnit.Services
	'<TestFixture()>
	Public Class FileIOTests

#Region " Members "
		Dim _testObj As kCura.WinEDDS.Service.FileIO = Nothing
		Private _mockRepo As MockRepository = Nothing
		Dim _mockCredentials As ICredentials
		Dim _mockCookieContainer As CookieContainer

		Dim _keyPathExistsAlready As Boolean
		Dim _keyValExistsAlready As Boolean
#End Region

#Region "Setup And Teardown"

		<SetUp()> Public Sub SetUp()
			_mockRepo = New MockRepository()
			_mockCredentials = _mockRepo.DynamicMock(Of ICredentials)()
			_mockCookieContainer = _mockRepo.DynamicMock(Of CookieContainer)()

			_keyPathExistsAlready = RegKeyHelper.SubKeyPathExists(RegKeyHelper.RelativityKeyPath)
			_keyValExistsAlready = False
			If _keyPathExistsAlready = True Then
				_keyValExistsAlready = RegKeyHelper.SubKeyExists(RegKeyHelper.RelativityKeyPath, RegKeyHelper.RelativityServiceURLKey)
			End If

			If _keyValExistsAlready = False Then
				RegKeyHelper.CreateKeyWithValueOnPath(Not _keyPathExistsAlready, RegKeyHelper.RelativityKeyPath, RegKeyHelper.RelativityServiceURLKey, RegKeyHelper.RelativityDefaultServiceURL)
			End If

			_testObj = New kCura.WinEDDS.Service.FileIO(_mockCredentials, _mockCookieContainer)
		End Sub

		<TearDown()> Public Sub TearDown()
			_testObj = Nothing
			_mockCredentials = Nothing
			_mockCookieContainer = Nothing
			_mockRepo = Nothing

			If _keyValExistsAlready = False Then
				RegKeyHelper.RemoveKeyPath(RegKeyHelper.RelativityKeyPath)
			End If
		End Sub

#End Region

		<Test>
		Public Sub ConvertSoapPermissionError()
			Dim errorMsg As String = "Insufficient Permissions! Please ask your Relativity Administrator to allow you import permission."
			Dim mockEx As MockInsufficientAccessControlListPermissions = New MockInsufficientAccessControlListPermissions(errorMsg)
			Dim mockSoapEx As System.Exception = Helper.Soapify(mockEx)
			Dim relativityEx As System.Exception = _testObj.ConvertExpectedSoapExceptionToRelativityException(mockSoapEx)
			Assert.IsTrue(TypeOf relativityEx Is BulkImportManager.InsufficientPermissionsForImportException)
			Assert.AreEqual(relativityEx.Message, "Error: " + mockEx.Message + vbCrLf)
		End Sub

		<Test>
		Public Sub ConvertArgumentException()
			Dim errorMsg As String = "documentDirectory is not valid"
			Dim mockEx As MockArgumentException = New MockArgumentException(errorMsg)
			Dim mockSoapEx As System.Exception = Helper.Soapify(mockEx)
			Dim relativityEx As System.Exception = _testObj.ConvertExpectedSoapExceptionToRelativityException(mockSoapEx)
			Assert.IsTrue(TypeOf relativityEx Is System.ArgumentException)
			Assert.AreEqual(relativityEx.Message, "Error: " + mockEx.Message + vbCrLf)
		End Sub

		<Test>
		Public Sub ConvertOtherSoapException()
			Dim otherEx As NullReferenceException = New NullReferenceException()
			Dim mockSoapEx As System.Exception = Helper.Soapify(otherEx)
			Dim relativityEx As System.Exception = _testObj.ConvertExpectedSoapExceptionToRelativityException(mockSoapEx)
			Assert.IsNull(relativityEx)
		End Sub


		<Test>
		Public Sub ConvertOtherException()
			Dim otherEx As NullReferenceException = New NullReferenceException()
			Dim relativityEx As System.Exception = _testObj.ConvertExpectedSoapExceptionToRelativityException(otherEx)
			Assert.IsNull(relativityEx)
		End Sub
	End Class


	Class Helper
		Const kServiceUrlPageFormat As String = "{0}FileIO.asmx"

		Public Shared Function Soapify(ex As System.Exception) As SoapException
			Dim tempdoc As New System.Xml.XmlDocument

			Dim soapExDetail As New Relativity.SoapExceptionDetail(ex)
			Dim modifier As SoapDetailModifierForMockException = TryCast(ex, SoapDetailModifierForMockException)
			If modifier IsNot Nothing Then
				modifier.ModifySoapDetailsToEmulateThisExceptionType(soapExDetail)
			End If

			Dim xmlSerializer As New System.Xml.Serialization.XmlSerializer(GetType(Relativity.SoapExceptionDetail))
			'Empty namespace so there are no additional atributes on the root element
			Dim ns As New System.Xml.Serialization.XmlSerializerNamespaces()
			ns.Add("", "")
			Using stringWriter As New System.IO.StringWriter()
				' Make the serialization, writing it into the StringWriter.
				xmlSerializer.Serialize(stringWriter, soapExDetail, ns)
				tempdoc.LoadXml(stringWriter.ToString)
			End Using

			Dim url As String = String.Format(kServiceUrlPageFormat, "http://RelativityWebAPI/")
			Dim xmlNodeDetails As System.Xml.XmlNode = tempdoc.ChildNodes(1)
			Return New SoapException(ex.Message, SoapException.ServerFaultCode, url, xmlNodeDetails)
		End Function

		Public Shared Sub ModifyRelativitySoapDetails(textToReplace As String, replaceWith As String, exDetail As Relativity.SoapExceptionDetail)
			exDetail.ExceptionType = exDetail.ExceptionType.Replace(textToReplace, replaceWith)
			exDetail.ExceptionFullText = exDetail.ExceptionFullText.Replace(textToReplace, replaceWith)
			exDetail.ExceptionMessage = exDetail.ExceptionMessage.Replace(textToReplace, replaceWith)
		End Sub
	End Class

	Public Interface SoapDetailModifierForMockException
		Sub ModifySoapDetailsToEmulateThisExceptionType(exDetail As Relativity.SoapExceptionDetail)
	End Interface

	Public Class MockInsufficientAccessControlListPermissions
		Inherits System.Exception
		Implements SoapDetailModifierForMockException

		Public Sub New(ByVal message As String)
			MyBase.New(message)
		End Sub

		Public Sub ModifySoapDetailsToThisExceptionType(exDetail As Relativity.SoapExceptionDetail) Implements SoapDetailModifierForMockException.ModifySoapDetailsToEmulateThisExceptionType
			Helper.ModifyRelativitySoapDetails("kCura.WinEDDS.NUnit.Services.MockInsufficientAccessControlListPermissions", "Relativity.Core.Exception.InsufficientAccessControlListPermissions", exDetail)
		End Sub
	End Class

	Public Class MockArgumentException
		Inherits System.Exception
		Implements SoapDetailModifierForMockException

		Public Sub New(ByVal message As String)
			MyBase.New(message)
		End Sub

		Public Sub ModifySoapDetailsToThisExceptionType(exDetail As Relativity.SoapExceptionDetail) Implements SoapDetailModifierForMockException.ModifySoapDetailsToEmulateThisExceptionType
			Helper.ModifyRelativitySoapDetails("kCura.WinEDDS.NUnit.Services.MockArgumentException", "System.ArgumentException", exDetail)
		End Sub
	End Class
End Namespace






