﻿'------------------------------------------------------------------------------
' <autogenerated>
'     This code was generated by a tool.
'     Runtime Version: 1.1.4322.2300
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
'This source code was auto-generated by Microsoft.VSDesigner, Version 1.1.4322.2300.
'
Namespace kCura.EDDS.WebAPI.FileManagerBase
    
    '<remarks/>
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Web.Services.WebServiceBindingAttribute(Name:="FileManagerSoap", [Namespace]:="http://www.kCura.com/EDDS/FileManager")>  _
    Public Class FileManager
        Inherits System.Web.Services.Protocols.SoapHttpClientProtocol
        
        '<remarks/>
        Public Sub New()
            MyBase.New
            Me.Url = "http://localhost/EDDS/WebAPI/FileManager.asmx"
        End Sub
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/FileManager/RetrieveByProductionArtifactIDForProduction"& _ 
"", RequestNamespace:="http://www.kCura.com/EDDS/FileManager", ResponseNamespace:="http://www.kCura.com/EDDS/FileManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function RetrieveByProductionArtifactIDForProduction(ByVal productionArtifactID As Integer) As System.Data.DataSet
            Dim results() As Object = Me.Invoke("RetrieveByProductionArtifactIDForProduction", New Object() {productionArtifactID})
            Return CType(results(0),System.Data.DataSet)
        End Function
        
        '<remarks/>
        Public Function BeginRetrieveByProductionArtifactIDForProduction(ByVal productionArtifactID As Integer, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("RetrieveByProductionArtifactIDForProduction", New Object() {productionArtifactID}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndRetrieveByProductionArtifactIDForProduction(ByVal asyncResult As System.IAsyncResult) As System.Data.DataSet
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),System.Data.DataSet)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/FileManager/RetrieveFileGuidsByDocumentArtifactIDAndPro"& _ 
"ductionArtifactID", RequestNamespace:="http://www.kCura.com/EDDS/FileManager", ResponseNamespace:="http://www.kCura.com/EDDS/FileManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function RetrieveFileGuidsByDocumentArtifactIDAndProductionArtifactID(ByVal documentArtifactID As Integer, ByVal productionArtifactID As Integer) As String()
            Dim results() As Object = Me.Invoke("RetrieveFileGuidsByDocumentArtifactIDAndProductionArtifactID", New Object() {documentArtifactID, productionArtifactID})
            Return CType(results(0),String())
        End Function
        
        '<remarks/>
        Public Function BeginRetrieveFileGuidsByDocumentArtifactIDAndProductionArtifactID(ByVal documentArtifactID As Integer, ByVal productionArtifactID As Integer, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("RetrieveFileGuidsByDocumentArtifactIDAndProductionArtifactID", New Object() {documentArtifactID, productionArtifactID}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndRetrieveFileGuidsByDocumentArtifactIDAndProductionArtifactID(ByVal asyncResult As System.IAsyncResult) As String()
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),String())
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/FileManager/ReturnFileGuidsForOriginalImages", RequestNamespace:="http://www.kCura.com/EDDS/FileManager", ResponseNamespace:="http://www.kCura.com/EDDS/FileManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function ReturnFileGuidsForOriginalImages(ByVal documentArtifactID As Integer) As String()
            Dim results() As Object = Me.Invoke("ReturnFileGuidsForOriginalImages", New Object() {documentArtifactID})
            Return CType(results(0),String())
        End Function
        
        '<remarks/>
        Public Function BeginReturnFileGuidsForOriginalImages(ByVal documentArtifactID As Integer, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("ReturnFileGuidsForOriginalImages", New Object() {documentArtifactID}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndReturnFileGuidsForOriginalImages(ByVal asyncResult As System.IAsyncResult) As String()
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),String())
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/FileManager/Create", RequestNamespace:="http://www.kCura.com/EDDS/FileManager", ResponseNamespace:="http://www.kCura.com/EDDS/FileManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function Create(ByVal fileDTO As File) As String
            Dim results() As Object = Me.Invoke("Create", New Object() {fileDTO})
            Return CType(results(0),String)
        End Function
        
        '<remarks/>
        Public Function BeginCreate(ByVal fileDTO As File, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("Create", New Object() {fileDTO}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndCreate(ByVal asyncResult As System.IAsyncResult) As String
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),String)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/FileManager/CreateImages", RequestNamespace:="http://www.kCura.com/EDDS/FileManager", ResponseNamespace:="http://www.kCura.com/EDDS/FileManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Sub CreateImages(ByVal files() As FileInfoBase, ByVal documentArtifactID As Integer, ByVal contextArtifactID As Integer)
            Me.Invoke("CreateImages", New Object() {files, documentArtifactID, contextArtifactID})
        End Sub
        
        '<remarks/>
        Public Function BeginCreateImages(ByVal files() As FileInfoBase, ByVal documentArtifactID As Integer, ByVal contextArtifactID As Integer, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("CreateImages", New Object() {files, documentArtifactID, contextArtifactID}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Sub EndCreateImages(ByVal asyncResult As System.IAsyncResult)
            Me.EndInvoke(asyncResult)
        End Sub
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/FileManager/CreateNatives", RequestNamespace:="http://www.kCura.com/EDDS/FileManager", ResponseNamespace:="http://www.kCura.com/EDDS/FileManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Sub CreateNatives(ByVal files() As FileInfoBase, ByVal documentArtifactIDs() As Integer)
            Me.Invoke("CreateNatives", New Object() {files, documentArtifactIDs})
        End Sub
        
        '<remarks/>
        Public Function BeginCreateNatives(ByVal files() As FileInfoBase, ByVal documentArtifactIDs() As Integer, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("CreateNatives", New Object() {files, documentArtifactIDs}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Sub EndCreateNatives(ByVal asyncResult As System.IAsyncResult)
            Me.EndInvoke(asyncResult)
        End Sub
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/FileManager/GetRotation", RequestNamespace:="http://www.kCura.com/EDDS/FileManager", ResponseNamespace:="http://www.kCura.com/EDDS/FileManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function GetRotation(ByVal artifactID As Integer, ByVal guid As String) As Integer
            Dim results() As Object = Me.Invoke("GetRotation", New Object() {artifactID, guid})
            Return CType(results(0),Integer)
        End Function
        
        '<remarks/>
        Public Function BeginGetRotation(ByVal artifactID As Integer, ByVal guid As String, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("GetRotation", New Object() {artifactID, guid}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndGetRotation(ByVal asyncResult As System.IAsyncResult) As Integer
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),Integer)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/FileManager/SetRotation", RequestNamespace:="http://www.kCura.com/EDDS/FileManager", ResponseNamespace:="http://www.kCura.com/EDDS/FileManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Sub SetRotation(ByVal artifactID As Integer, ByVal guid As String, ByVal rotation As Integer)
            Me.Invoke("SetRotation", New Object() {artifactID, guid, rotation})
        End Sub
        
        '<remarks/>
        Public Function BeginSetRotation(ByVal artifactID As Integer, ByVal guid As String, ByVal rotation As Integer, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("SetRotation", New Object() {artifactID, guid, rotation}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Sub EndSetRotation(ByVal asyncResult As System.IAsyncResult)
            Me.EndInvoke(asyncResult)
        End Sub
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/FileManager/GetFullTextGuidsByDocumentArtifactIdAndType"& _ 
"", RequestNamespace:="http://www.kCura.com/EDDS/FileManager", ResponseNamespace:="http://www.kCura.com/EDDS/FileManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function GetFullTextGuidsByDocumentArtifactIdAndType(ByVal documentArtifactID As Integer, ByVal TypeId As Integer) As String
            Dim results() As Object = Me.Invoke("GetFullTextGuidsByDocumentArtifactIdAndType", New Object() {documentArtifactID, TypeId})
            Return CType(results(0),String)
        End Function
        
        '<remarks/>
        Public Function BeginGetFullTextGuidsByDocumentArtifactIdAndType(ByVal documentArtifactID As Integer, ByVal TypeId As Integer, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("GetFullTextGuidsByDocumentArtifactIdAndType", New Object() {documentArtifactID, TypeId}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndGetFullTextGuidsByDocumentArtifactIdAndType(ByVal asyncResult As System.IAsyncResult) As String
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),String)
        End Function
    End Class
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.kCura.com/EDDS/FileManager")>  _
    Public Class File
        
        '<remarks/>
        Public Guid As String
        
        '<remarks/>
        Public Order As Integer
        
        '<remarks/>
        Public Type As Integer
        
        '<remarks/>
        Public Filename As String
        
        '<remarks/>
        Public DocumentArtifactID As Integer
        
        '<remarks/>
        Public Rotation As Integer
    End Class
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.kCura.com/EDDS/FileManager")>  _
    Public Class FileInfoBase
        
        '<remarks/>
        Public FileName As String
        
        '<remarks/>
        Public FileGuid As String
    End Class
End Namespace
