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
Namespace kCura.EDDS.WebAPI.MultiCodeManagerBase
    
    '<remarks/>
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Web.Services.WebServiceBindingAttribute(Name:="MultiCodeManagerSoap", [Namespace]:="http://tempuri.org//MultiCodeManager"),  _
     System.Xml.Serialization.XmlIncludeAttribute(GetType(System.Object()))>  _
    Public Class MultiCodeManager
        Inherits System.Web.Services.Protocols.SoapHttpClientProtocol
        
        '<remarks/>
        Public Sub New()
            MyBase.New
            Me.Url = "http://localhost/EDDSWebAPI/MultiCodeManager.asmx"
        End Sub
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org//MultiCodeManager/DeleteFromMultiCodeArtifactByMultiCodeID", RequestNamespace:="http://tempuri.org//MultiCodeManager", ResponseNamespace:="http://tempuri.org//MultiCodeManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Sub DeleteFromMultiCodeArtifactByMultiCodeID(ByVal artifactID As Integer, ByVal multiCodeID As Integer)
            Me.Invoke("DeleteFromMultiCodeArtifactByMultiCodeID", New Object() {artifactID, multiCodeID})
        End Sub
        
        '<remarks/>
        Public Function BeginDeleteFromMultiCodeArtifactByMultiCodeID(ByVal artifactID As Integer, ByVal multiCodeID As Integer, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("DeleteFromMultiCodeArtifactByMultiCodeID", New Object() {artifactID, multiCodeID}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Sub EndDeleteFromMultiCodeArtifactByMultiCodeID(ByVal asyncResult As System.IAsyncResult)
            Me.EndInvoke(asyncResult)
        End Sub
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org//MultiCodeManager/CreateNewMultiCodeID", RequestNamespace:="http://tempuri.org//MultiCodeManager", ResponseNamespace:="http://tempuri.org//MultiCodeManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function CreateNewMultiCodeID(ByVal artifactID As Integer) As Integer
            Dim results() As Object = Me.Invoke("CreateNewMultiCodeID", New Object() {artifactID})
            Return CType(results(0),Integer)
        End Function
        
        '<remarks/>
        Public Function BeginCreateNewMultiCodeID(ByVal artifactID As Integer, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("CreateNewMultiCodeID", New Object() {artifactID}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndCreateNewMultiCodeID(ByVal asyncResult As System.IAsyncResult) As Integer
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),Integer)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org//MultiCodeManager/SetMultiCodeValues", RequestNamespace:="http://tempuri.org//MultiCodeManager", ResponseNamespace:="http://tempuri.org//MultiCodeManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Sub SetMultiCodeValues(ByVal multiCodeID As Integer, ByVal values() As Object)
            Me.Invoke("SetMultiCodeValues", New Object() {multiCodeID, values})
        End Sub
        
        '<remarks/>
        Public Function BeginSetMultiCodeValues(ByVal multiCodeID As Integer, ByVal values() As Object, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("SetMultiCodeValues", New Object() {multiCodeID, values}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Sub EndSetMultiCodeValues(ByVal asyncResult As System.IAsyncResult)
            Me.EndInvoke(asyncResult)
        End Sub
    End Class
End Namespace
