Imports System.Net

Namespace kCura.WinEDDS.Service
	Public Interface IBulkImportManager
		Function BulkImportImage(ByVal appID As Int32, ByVal settings As kCura.EDDS.WebAPI.BulkImportManagerBase.ImageLoadInfo, ByVal inRepository As Boolean) As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults
		Function BulkImportProductionImage(ByVal appID As Int32, ByVal settings As kCura.EDDS.WebAPI.BulkImportManagerBase.ImageLoadInfo, ByVal productionKeyFieldArtifactID As Int32, ByVal inRepository As Boolean) As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults
		Function BulkImportNative(ByVal appID As Int32, ByVal settings As kCura.EDDS.WebAPI.BulkImportManagerBase.NativeLoadInfo, ByVal inRepository As Boolean, ByVal includeExtractedTextEncoding As Boolean) As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults
		Function BulkImportObjects(ByVal appID As Int32, ByVal settings As kCura.EDDS.WebAPI.BulkImportManagerBase.ObjectLoadInfo, ByVal inRepository As Boolean) As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults
		Function DisposeTempTables(ByVal appID As Int32, ByVal runId As String) As Object
		Function GenerateNonImageErrorFiles(ByVal appID As Integer, ByVal runID As String, ByVal artifactTypeID As Integer, ByVal writeHeader As Boolean, ByVal keyFieldID As Integer) As Global.Relativity.MassImport.ErrorFileKey
		Function NativeRunHasErrors(ByVal appID As Integer, ByVal runId As String) As Boolean
		Property CookieContainer As CookieContainer
		Property Credentials As ICredentials
	End Interface
End Namespace