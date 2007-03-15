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
Namespace kCura.EDDS.WebAPI.UserManagerBase
    
    '<remarks/>
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Web.Services.WebServiceBindingAttribute(Name:="UserManagerSoap", [Namespace]:="http://www.kCura.com/EDDS/UserManager")>  _
    Public Class UserManager
        Inherits System.Web.Services.Protocols.SoapHttpClientProtocol
        
        '<remarks/>
        Public Sub New()
            MyBase.New
            Me.Url = "http://localhost/EDDSWebAPI/UserManager.asmx"
        End Sub
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/UserManager/UpdateImageViewerDefaultViewMode", RequestNamespace:="http://www.kCura.com/EDDS/UserManager", ResponseNamespace:="http://www.kCura.com/EDDS/UserManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Sub UpdateImageViewerDefaultViewMode(ByVal defaultViewMode As String)
            Me.Invoke("UpdateImageViewerDefaultViewMode", New Object() {defaultViewMode})
        End Sub
        
        '<remarks/>
        Public Function BeginUpdateImageViewerDefaultViewMode(ByVal defaultViewMode As String, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("UpdateImageViewerDefaultViewMode", New Object() {defaultViewMode}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Sub EndUpdateImageViewerDefaultViewMode(ByVal asyncResult As System.IAsyncResult)
            Me.EndInvoke(asyncResult)
        End Sub
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/UserManager/UpdateOIXDocumentMode", RequestNamespace:="http://www.kCura.com/EDDS/UserManager", ResponseNamespace:="http://www.kCura.com/EDDS/UserManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Sub UpdateOIXDocumentMode(ByVal mode As Integer)
            Me.Invoke("UpdateOIXDocumentMode", New Object() {mode})
        End Sub
        
        '<remarks/>
        Public Function BeginUpdateOIXDocumentMode(ByVal mode As Integer, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("UpdateOIXDocumentMode", New Object() {mode}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Sub EndUpdateOIXDocumentMode(ByVal asyncResult As System.IAsyncResult)
            Me.EndInvoke(asyncResult)
        End Sub
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/UserManager/Login", RequestNamespace:="http://www.kCura.com/EDDS/UserManager", ResponseNamespace:="http://www.kCura.com/EDDS/UserManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function Login(ByVal emailAddress As String, ByVal password As String) As Boolean
            Dim results() As Object = Me.Invoke("Login", New Object() {emailAddress, password})
            Return CType(results(0),Boolean)
        End Function
        
        '<remarks/>
        Public Function BeginLogin(ByVal emailAddress As String, ByVal password As String, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("Login", New Object() {emailAddress, password}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndLogin(ByVal asyncResult As System.IAsyncResult) As Boolean
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),Boolean)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/UserManager/LoginWithAuthenticationToken", RequestNamespace:="http://www.kCura.com/EDDS/UserManager", ResponseNamespace:="http://www.kCura.com/EDDS/UserManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function LoginWithAuthenticationToken(ByVal authenticationToken As String) As String
            Dim results() As Object = Me.Invoke("LoginWithAuthenticationToken", New Object() {authenticationToken})
            Return CType(results(0),String)
        End Function
        
        '<remarks/>
        Public Function BeginLoginWithAuthenticationToken(ByVal authenticationToken As String, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("LoginWithAuthenticationToken", New Object() {authenticationToken}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndLoginWithAuthenticationToken(ByVal asyncResult As System.IAsyncResult) As String
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),String)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/UserManager/GenerateAuthenticationToken", RequestNamespace:="http://www.kCura.com/EDDS/UserManager", ResponseNamespace:="http://www.kCura.com/EDDS/UserManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function GenerateAuthenticationToken() As String
            Dim results() As Object = Me.Invoke("GenerateAuthenticationToken", New Object(-1) {})
            Return CType(results(0),String)
        End Function
        
        '<remarks/>
        Public Function BeginGenerateAuthenticationToken(ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("GenerateAuthenticationToken", New Object(-1) {}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndGenerateAuthenticationToken(ByVal asyncResult As System.IAsyncResult) As String
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),String)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/UserManager/GetLatestAuthenticationToken", RequestNamespace:="http://www.kCura.com/EDDS/UserManager", ResponseNamespace:="http://www.kCura.com/EDDS/UserManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function GetLatestAuthenticationToken() As String
            Dim results() As Object = Me.Invoke("GetLatestAuthenticationToken", New Object(-1) {})
            Return CType(results(0),String)
        End Function
        
        '<remarks/>
        Public Function BeginGetLatestAuthenticationToken(ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("GetLatestAuthenticationToken", New Object(-1) {}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndGetLatestAuthenticationToken(ByVal asyncResult As System.IAsyncResult) As String
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),String)
        End Function
    End Class
End Namespace
