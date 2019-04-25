Imports System.Collections.Generic
Imports System.Net
Imports kCura.Relativity.Client.DTOs
Imports Relativity.Import.Export
Imports Relativity.Import.Export.Services
Imports Relativity.Services.Objects
Imports Relativity.Services.Objects.DataContracts
Imports Relativity.Services.Pipeline
Imports Relativity.Services.ServiceProxy

Public Class ApplicationVersionService 
	Implements IApplicationVersionService

	Private ReadOnly _cred As Credentials
	Private ReadOnly _webApiUrl As String
	Private ReadOnly _restApiUrl As String
	Private ReadOnly _rsApiUrl As String

	Public Sub New(cred As Credentials, webApiUrl As String,  restApiUrl As String, rsApiUrl As String)
		_cred = cred
		_webApiUrl = webApiUrl
		_restApiUrl = restApiUrl
		_rsApiUrl = rsApiUrl
	End Sub

	Public Function RetrieveRelativityVersion() As Version Implements IApplicationVersionService.RetrieveRelativityVersion
		System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 OR SecurityProtocolType.Tls OR SecurityProtocolType.Tls11 OR SecurityProtocolType.Tls12

		Dim serviceFactorySettings As ServiceFactorySettings = new ServiceFactorySettings(
			New Uri(_rsApiUrl), New Uri(_restApiUrl), _cred) With 
		{
			.ProtocolVersion = WireProtocolVersion.V2
		}

		Dim serviceFactory As ServiceFactory = new ServiceFactory(serviceFactorySettings)

		Using objectManager As IObjectManager = serviceFactory.CreateProxy(Of IObjectManager)()
			Dim queryRequest As QueryRequest = New QueryRequest() With
				{
					.Fields = New List(Of FieldRef) From { New FieldRef() With {.Name = FieldFieldNames.Name} },
					.ObjectType = New ObjectTypeRef() With { .ArtifactTypeID = ArtifactType.InstanceSetting }
				}
		
			Dim result As QueryResult  = objectManager.QueryAsync(-1, queryRequest, 1, 100).GetAwaiter().GetResult()

			Console.WriteLine($"{result.Objects(0) }")
			Return New Version()
		End Using
	End Function

	Public Function RetrieveImportExportWebApiVersion() As Version Implements IApplicationVersionService.RetrieveImportExportWebApiVersion
		Throw New NotImplementedException
	End Function
End Class
