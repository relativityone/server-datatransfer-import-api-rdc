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
Namespace kCura.EDDS.WebAPI.RelativityManagerBase
    
    '<remarks/>
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Web.Services.WebServiceBindingAttribute(Name:="RelativityManagerSoap", [Namespace]:="http://www.kCura.com/EDDS/RelativityManager")>  _
    Public Class RelativityManager
        Inherits System.Web.Services.Protocols.SoapHttpClientProtocol
        
        '<remarks/>
        Public Sub New()
            MyBase.New
            Me.Url = "http://localhost/EDDSWebAPI/RelativityManager.asmx"
        End Sub
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/RelativityManager/RetrieveRelativityVersion", RequestNamespace:="http://www.kCura.com/EDDS/RelativityManager", ResponseNamespace:="http://www.kCura.com/EDDS/RelativityManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function RetrieveRelativityVersion() As String
            Dim results() As Object = Me.Invoke("RetrieveRelativityVersion", New Object(-1) {})
            Return CType(results(0),String)
        End Function
        
        '<remarks/>
        Public Function BeginRetrieveRelativityVersion(ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("RetrieveRelativityVersion", New Object(-1) {}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndRetrieveRelativityVersion(ByVal asyncResult As System.IAsyncResult) As String
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),String)
        End Function
    End Class
End Namespace
