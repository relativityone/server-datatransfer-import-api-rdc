Imports System.Net
Imports Relativity.Import.Export.Services

Namespace kCura.WinEDDS.Service
	Public Interface IBulkImportManager
		Property CookieContainer As CookieContainer
		Property Credentials As ICredentials
		Function BulkImportImage(ByVal appID As Int32, ByVal settings As kCura.EDDS.WebAPI.BulkImportManagerBase.ImageLoadInfo, ByVal inRepository As Boolean) As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults
		Function BulkImportProductionImage(ByVal appID As Int32, ByVal settings As kCura.EDDS.WebAPI.BulkImportManagerBase.ImageLoadInfo, ByVal productionKeyFieldArtifactID As Int32, ByVal inRepository As Boolean) As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults
		Function BulkImportNative(ByVal appID As Int32, ByVal settings As kCura.EDDS.WebAPI.BulkImportManagerBase.NativeLoadInfo, ByVal inRepository As Boolean, ByVal includeExtractedTextEncoding As Boolean) As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults
		Function BulkImportObjects(ByVal appID As Int32, ByVal settings As kCura.EDDS.WebAPI.BulkImportManagerBase.ObjectLoadInfo, ByVal inRepository As Boolean) As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults
		Function DisposeTempTables(ByVal appID As Int32, ByVal runId As String) As Object
		Function ImageRunHasErrors(artifactID As Integer, runId As String) As Boolean
		Function GenerateImageErrorFiles(ByVal appID As Integer, ByVal importKey As String, ByVal writeHeader As Boolean, ByVal keyFieldID As Integer) As ErrorFileKey
		Function GenerateNonImageErrorFiles(ByVal appID As Integer, ByVal runID As String, ByVal artifactTypeID As Integer, ByVal writeHeader As Boolean, ByVal keyFieldID As Integer) As ErrorFileKey
		Function NativeRunHasErrors(ByVal appID As Integer, ByVal runId As String) As Boolean
	End Interface
End Namespace