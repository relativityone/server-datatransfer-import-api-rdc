﻿'------------------------------------------------------------------------------
' <autogenerated>
'     This code was generated by a tool.
'     Runtime Version: 1.1.4322.2407
'
'     Changes to this file may cause incorrect behavior and will be lost if 
'     the code is regenerated.
' </autogenerated>
'------------------------------------------------------------------------------

Option Strict Off
Option Explicit On

Imports System
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Xml.Serialization

'
'This source code was auto-generated by Microsoft.VSDesigner, Version 1.1.4322.2407.
'
Namespace kCura.EDDS.WebAPI.BulkImportManagerBase
    
    '<remarks/>
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Web.Services.WebServiceBindingAttribute(Name:="BulkImportManagerSoap", [Namespace]:="http://www.kCura.com/EDDS/BulkImportManager")>  _
    Public Class BulkImportManager
        Inherits System.Web.Services.Protocols.SoapHttpClientProtocol
        
        '<remarks/>
        Public Sub New()
            MyBase.New
            Me.Url = "http://localhost/EDDSWebAPI/BulkImportManager.asmx"
        End Sub
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/BulkImportManager/BulkImportImage", RequestNamespace:="http://www.kCura.com/EDDS/BulkImportManager", ResponseNamespace:="http://www.kCura.com/EDDS/BulkImportManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function BulkImportImage(ByVal appID As Integer, ByVal bulkFileName As String, ByVal uploadFullText As Boolean, ByVal overwrite As OverwriteType, ByVal destinationFolderArtifactID As Integer, ByVal repository As String, ByVal useBulk As Boolean, ByVal runID As String, ByVal keyFieldID As Integer) As Object
            Dim results() As Object = Me.Invoke("BulkImportImage", New Object() {appID, bulkFileName, uploadFullText, overwrite, destinationFolderArtifactID, repository, useBulk, runID, keyFieldID})
            Return CType(results(0),Object)
        End Function
        
        '<remarks/>
        Public Function BeginBulkImportImage(ByVal appID As Integer, ByVal bulkFileName As String, ByVal uploadFullText As Boolean, ByVal overwrite As OverwriteType, ByVal destinationFolderArtifactID As Integer, ByVal repository As String, ByVal useBulk As Boolean, ByVal runID As String, ByVal keyFieldID As Integer, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("BulkImportImage", New Object() {appID, bulkFileName, uploadFullText, overwrite, destinationFolderArtifactID, repository, useBulk, runID, keyFieldID}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndBulkImportImage(ByVal asyncResult As System.IAsyncResult) As Object
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),Object)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/BulkImportManager/BulkImportProductionImage", RequestNamespace:="http://www.kCura.com/EDDS/BulkImportManager", ResponseNamespace:="http://www.kCura.com/EDDS/BulkImportManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function BulkImportProductionImage(ByVal appID As Integer, ByVal bulkFileName As String, ByVal uploadFullText As Boolean, ByVal overwrite As OverwriteType, ByVal destinationFolderArtifactID As Integer, ByVal repository As String, ByVal productionArtifactID As Integer, ByVal useBulk As Boolean, ByVal runID As String, ByVal productionKeyFieldArtifactID As Integer) As Object
            Dim results() As Object = Me.Invoke("BulkImportProductionImage", New Object() {appID, bulkFileName, uploadFullText, overwrite, destinationFolderArtifactID, repository, productionArtifactID, useBulk, runID, productionKeyFieldArtifactID})
            Return CType(results(0),Object)
        End Function
        
        '<remarks/>
        Public Function BeginBulkImportProductionImage(ByVal appID As Integer, ByVal bulkFileName As String, ByVal uploadFullText As Boolean, ByVal overwrite As OverwriteType, ByVal destinationFolderArtifactID As Integer, ByVal repository As String, ByVal productionArtifactID As Integer, ByVal useBulk As Boolean, ByVal runID As String, ByVal productionKeyFieldArtifactID As Integer, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("BulkImportProductionImage", New Object() {appID, bulkFileName, uploadFullText, overwrite, destinationFolderArtifactID, repository, productionArtifactID, useBulk, runID, productionKeyFieldArtifactID}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndBulkImportProductionImage(ByVal asyncResult As System.IAsyncResult) As Object
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),Object)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/BulkImportManager/GenerateImageErrorFiles", RequestNamespace:="http://www.kCura.com/EDDS/BulkImportManager", ResponseNamespace:="http://www.kCura.com/EDDS/BulkImportManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function GenerateImageErrorFiles(ByVal appID As Integer, ByVal importKey As String, ByVal writeHeader As Boolean, ByVal keyFieldID As Integer) As ErrorFileKey
            Dim results() As Object = Me.Invoke("GenerateImageErrorFiles", New Object() {appID, importKey, writeHeader, keyFieldID})
            Return CType(results(0),ErrorFileKey)
        End Function
        
        '<remarks/>
        Public Function BeginGenerateImageErrorFiles(ByVal appID As Integer, ByVal importKey As String, ByVal writeHeader As Boolean, ByVal keyFieldID As Integer, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("GenerateImageErrorFiles", New Object() {appID, importKey, writeHeader, keyFieldID}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndGenerateImageErrorFiles(ByVal asyncResult As System.IAsyncResult) As ErrorFileKey
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),ErrorFileKey)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/BulkImportManager/ImageRunHasErrors", RequestNamespace:="http://www.kCura.com/EDDS/BulkImportManager", ResponseNamespace:="http://www.kCura.com/EDDS/BulkImportManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function ImageRunHasErrors(ByVal appID As Integer, ByVal runId As String) As Boolean
            Dim results() As Object = Me.Invoke("ImageRunHasErrors", New Object() {appID, runId})
            Return CType(results(0),Boolean)
        End Function
        
        '<remarks/>
        Public Function BeginImageRunHasErrors(ByVal appID As Integer, ByVal runId As String, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("ImageRunHasErrors", New Object() {appID, runId}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndImageRunHasErrors(ByVal asyncResult As System.IAsyncResult) As Boolean
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),Boolean)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/BulkImportManager/BulkImportNative", RequestNamespace:="http://www.kCura.com/EDDS/BulkImportManager", ResponseNamespace:="http://www.kCura.com/EDDS/BulkImportManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function BulkImportNative(ByVal appID As Integer, ByVal settings As NativeLoadInfo) As Object
            Dim results() As Object = Me.Invoke("BulkImportNative", New Object() {appID, settings})
            Return CType(results(0),Object)
        End Function
        
        '<remarks/>
        Public Function BeginBulkImportNative(ByVal appID As Integer, ByVal settings As NativeLoadInfo, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("BulkImportNative", New Object() {appID, settings}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndBulkImportNative(ByVal asyncResult As System.IAsyncResult) As Object
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),Object)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/BulkImportManager/BulkImportObjects", RequestNamespace:="http://www.kCura.com/EDDS/BulkImportManager", ResponseNamespace:="http://www.kCura.com/EDDS/BulkImportManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function BulkImportObjects(ByVal appID As Integer, ByVal settings As ObjectLoadInfo) As Object
            Dim results() As Object = Me.Invoke("BulkImportObjects", New Object() {appID, settings})
            Return CType(results(0),Object)
        End Function
        
        '<remarks/>
        Public Function BeginBulkImportObjects(ByVal appID As Integer, ByVal settings As ObjectLoadInfo, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("BulkImportObjects", New Object() {appID, settings}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndBulkImportObjects(ByVal asyncResult As System.IAsyncResult) As Object
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),Object)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/BulkImportManager/GenerateNativeErrorFiles", RequestNamespace:="http://www.kCura.com/EDDS/BulkImportManager", ResponseNamespace:="http://www.kCura.com/EDDS/BulkImportManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function GenerateNativeErrorFiles(ByVal appID As Integer, ByVal runID As String, ByVal artifactTypeID As Integer, ByVal writeHeader As Boolean) As ErrorFileKey
            Dim results() As Object = Me.Invoke("GenerateNativeErrorFiles", New Object() {appID, runID, artifactTypeID, writeHeader})
            Return CType(results(0),ErrorFileKey)
        End Function
        
        '<remarks/>
        Public Function BeginGenerateNativeErrorFiles(ByVal appID As Integer, ByVal runID As String, ByVal artifactTypeID As Integer, ByVal writeHeader As Boolean, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("GenerateNativeErrorFiles", New Object() {appID, runID, artifactTypeID, writeHeader}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndGenerateNativeErrorFiles(ByVal asyncResult As System.IAsyncResult) As ErrorFileKey
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),ErrorFileKey)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/BulkImportManager/NativeRunHasErrors", RequestNamespace:="http://www.kCura.com/EDDS/BulkImportManager", ResponseNamespace:="http://www.kCura.com/EDDS/BulkImportManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function NativeRunHasErrors(ByVal appID As Integer, ByVal runId As String) As Boolean
            Dim results() As Object = Me.Invoke("NativeRunHasErrors", New Object() {appID, runId})
            Return CType(results(0),Boolean)
        End Function
        
        '<remarks/>
        Public Function BeginNativeRunHasErrors(ByVal appID As Integer, ByVal runId As String, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("NativeRunHasErrors", New Object() {appID, runId}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndNativeRunHasErrors(ByVal asyncResult As System.IAsyncResult) As Boolean
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),Boolean)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/BulkImportManager/DisposeTempTables", RequestNamespace:="http://www.kCura.com/EDDS/BulkImportManager", ResponseNamespace:="http://www.kCura.com/EDDS/BulkImportManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function DisposeTempTables(ByVal appID As Integer, ByVal runId As String) As Object
            Dim results() As Object = Me.Invoke("DisposeTempTables", New Object() {appID, runId})
            Return CType(results(0),Object)
        End Function
        
        '<remarks/>
        Public Function BeginDisposeTempTables(ByVal appID As Integer, ByVal runId As String, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("DisposeTempTables", New Object() {appID, runId}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndDisposeTempTables(ByVal asyncResult As System.IAsyncResult) As Object
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),Object)
        End Function
    End Class
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.kCura.com/EDDS/BulkImportManager")>  _
    Public Enum OverwriteType
        
        '<remarks/>
        Append
        
        '<remarks/>
        Overlay
        
        '<remarks/>
        Both
    End Enum
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.kCura.com/EDDS/BulkImportManager")>  _
    Public Class ErrorFileKey
        
        '<remarks/>
        Public OpticonKey As String
        
        '<remarks/>
        Public LogKey As String
    End Class
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.kCura.com/EDDS/BulkImportManager"),  _
     System.Xml.Serialization.XmlIncludeAttribute(GetType(ObjectLoadInfo))>  _
    Public Class NativeLoadInfo
        
        '<remarks/>
        Public MappedFields() As FieldInfo
        
        '<remarks/>
        Public Overlay As OverwriteType
        
        '<remarks/>
        Public Repository As String
        
        '<remarks/>
        Public RunID As String
        
        '<remarks/>
        Public DataFileName As String
        
        '<remarks/>
        Public UseBulkDataImport As Boolean
        
        '<remarks/>
        Public UploadFiles As Boolean
        
        '<remarks/>
        Public CodeFileName As String
        
        '<remarks/>
        Public KeyFieldArtifactID As Integer
    End Class
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.kCura.com/EDDS/BulkImportManager")>  _
    Public Class FieldInfo
        
        '<remarks/>
        Public ArtifactID As Integer
        
        '<remarks/>
        Public Category As FieldCategory
        
        '<remarks/>
        Public Type As FieldType
        
        '<remarks/>
        Public DisplayName As String
        
        '<remarks/>
        Public TextLength As Integer
        
        '<remarks/>
        Public CodeTypeID As Integer
        
        '<remarks/>
        Public FormatString As String
        
        '<remarks/>
        Public IsUnicodeEnabled As Boolean
    End Class
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.kCura.com/EDDS/BulkImportManager")>  _
    Public Enum FieldCategory
        
        '<remarks/>
        Generic
        
        '<remarks/>
        FullText
        
        '<remarks/>
        Identifier
        
        '<remarks/>
        Reflected
        
        '<remarks/>
        Comments
        
        '<remarks/>
        Relational
        
        '<remarks/>
        ProductionMarker
        
        '<remarks/>
        AutoCreate
        
        '<remarks/>
        FileSize
        
        '<remarks/>
        FolderName
        
        '<remarks/>
        FileInfo
        
        '<remarks/>
        ParentArtifact
        
        '<remarks/>
        MarkupSetMarker
        
        '<remarks/>
        GenericSystem
        
        '<remarks/>
        MultiReflected
        
        '<remarks/>
        Batch
    End Enum
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.kCura.com/EDDS/BulkImportManager")>  _
    Public Enum FieldType
        
        '<remarks/>
        Varchar
        
        '<remarks/>
        [Integer]
        
        '<remarks/>
        [Date]
        
        '<remarks/>
        [Boolean]
        
        '<remarks/>
        [Text]
        
        '<remarks/>
        Code
        
        '<remarks/>
        [Decimal]
        
        '<remarks/>
        Currency
        
        '<remarks/>
        MultiCode
        
        '<remarks/>
        File
        
        '<remarks/>
        [Object]
        
        '<remarks/>
        User
        
        '<remarks/>
        LayoutText
        
        '<remarks/>
        Objects
    End Enum
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.kCura.com/EDDS/BulkImportManager")>  _
    Public Class ObjectLoadInfo
        Inherits NativeLoadInfo
        
        '<remarks/>
        Public ArtifactTypeID As Integer
    End Class
End Namespace
