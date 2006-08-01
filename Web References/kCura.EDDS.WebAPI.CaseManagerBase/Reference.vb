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
Namespace kCura.EDDS.WebAPI.CaseManagerBase
    
    '<remarks/>
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Web.Services.WebServiceBindingAttribute(Name:="CaseManagerSoap", [Namespace]:="http://www.kCura.com/EDDS/CaseManager")>  _
    Public Class CaseManager
        Inherits System.Web.Services.Protocols.SoapHttpClientProtocol
        
        '<remarks/>
        Public Sub New()
            MyBase.New
            Me.Url = "http://localhost/EDDS/WebAPI/CaseManager.asmx"
        End Sub
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/CaseManager/RetrieveAll", RequestNamespace:="http://www.kCura.com/EDDS/CaseManager", ResponseNamespace:="http://www.kCura.com/EDDS/CaseManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function RetrieveAll() As System.Data.DataSet
            Dim results() As Object = Me.Invoke("RetrieveAll", New Object(-1) {})
            Return CType(results(0),System.Data.DataSet)
        End Function
        
        '<remarks/>
        Public Function BeginRetrieveAll(ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("RetrieveAll", New Object(-1) {}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndRetrieveAll(ByVal asyncResult As System.IAsyncResult) As System.Data.DataSet
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),System.Data.DataSet)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/CaseManager/Create", RequestNamespace:="http://www.kCura.com/EDDS/CaseManager", ResponseNamespace:="http://www.kCura.com/EDDS/CaseManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function Create(ByVal caseDTO As CaseInfo) As Integer
            Dim results() As Object = Me.Invoke("Create", New Object() {caseDTO})
            Return CType(results(0),Integer)
        End Function
        
        '<remarks/>
        Public Function BeginCreate(ByVal caseDTO As CaseInfo, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("Create", New Object() {caseDTO}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndCreate(ByVal asyncResult As System.IAsyncResult) As Integer
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),Integer)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/CaseManager/Delete", RequestNamespace:="http://www.kCura.com/EDDS/CaseManager", ResponseNamespace:="http://www.kCura.com/EDDS/CaseManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function Delete(ByVal caseArtifactID As Integer) As Boolean
            Dim results() As Object = Me.Invoke("Delete", New Object() {caseArtifactID})
            Return CType(results(0),Boolean)
        End Function
        
        '<remarks/>
        Public Function BeginDelete(ByVal caseArtifactID As Integer, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("Delete", New Object() {caseArtifactID}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndDelete(ByVal asyncResult As System.IAsyncResult) As Boolean
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),Boolean)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/CaseManager/Read", RequestNamespace:="http://www.kCura.com/EDDS/CaseManager", ResponseNamespace:="http://www.kCura.com/EDDS/CaseManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function Read(ByVal caseArtifactID As Integer) As CaseInfo
            Dim results() As Object = Me.Invoke("Read", New Object() {caseArtifactID})
            Return CType(results(0),CaseInfo)
        End Function
        
        '<remarks/>
        Public Function BeginRead(ByVal caseArtifactID As Integer, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("Read", New Object() {caseArtifactID}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndRead(ByVal asyncResult As System.IAsyncResult) As CaseInfo
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),CaseInfo)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/CaseManager/RetrieveCaseDownloadURI", RequestNamespace:="http://www.kCura.com/EDDS/CaseManager", ResponseNamespace:="http://www.kCura.com/EDDS/CaseManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function RetrieveCaseDownloadURI(ByVal contextArtifactID As Integer) As String
            Dim results() As Object = Me.Invoke("RetrieveCaseDownloadURI", New Object() {contextArtifactID})
            Return CType(results(0),String)
        End Function
        
        '<remarks/>
        Public Function BeginRetrieveCaseDownloadURI(ByVal contextArtifactID As Integer, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("RetrieveCaseDownloadURI", New Object() {contextArtifactID}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndRetrieveCaseDownloadURI(ByVal asyncResult As System.IAsyncResult) As String
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),String)
        End Function
    End Class
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.kCura.com/EDDS/CaseManager")>  _
    Public Class CaseInfo
        
        '<remarks/>
        Public ArtifactID As Integer
        
        '<remarks/>
        Public Name As String
        
        '<remarks/>
        Public EmailAddress As String
        
        '<remarks/>
        Public MatterArtifactID As Integer
        
        '<remarks/>
        Public StatusCodeArtifactID As Integer
        
        '<remarks/>
        Public RootFolderID As Integer
        
        '<remarks/>
        Public RootArtifactID As Integer
        
        '<remarks/>
        Public DownloadHandlerURL As String
        
        '<remarks/>
        Public DocumentPath As String
    End Class
End Namespace
