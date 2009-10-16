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
Namespace kCura.EDDS.WebAPI.TemplateManagerBase
    
    '<remarks/>
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Web.Services.WebServiceBindingAttribute(Name:="TemplateManagerSoap", [Namespace]:="http://www.kCura.com/EDDS/TemplateManager")>  _
    Public Class TemplateManager
        Inherits System.Web.Services.Protocols.SoapHttpClientProtocol
        
        '<remarks/>
        Public Sub New()
            MyBase.New
            Me.Url = "http://localhost/EDDSWebAPI/TemplateManager.asmx"
        End Sub
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/TemplateManager/InstallTemplate", RequestNamespace:="http://www.kCura.com/EDDS/TemplateManager", ResponseNamespace:="http://www.kCura.com/EDDS/TemplateManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function InstallTemplate(ByVal template As System.Xml.XmlNode, ByVal installationParameters As ApplicationInstallationParameters) As ApplicationInstallationResult
            Dim results() As Object = Me.Invoke("InstallTemplate", New Object() {template, installationParameters})
            Return CType(results(0),ApplicationInstallationResult)
        End Function
        
        '<remarks/>
        Public Function BeginInstallTemplate(ByVal template As System.Xml.XmlNode, ByVal installationParameters As ApplicationInstallationParameters, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("InstallTemplate", New Object() {template, installationParameters}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndInstallTemplate(ByVal asyncResult As System.IAsyncResult) As ApplicationInstallationResult
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),ApplicationInstallationResult)
        End Function
    End Class
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.kCura.com/EDDS/TemplateManager")>  _
    Public Class ApplicationInstallationParameters
        
        '<remarks/>
        Public CaseId As Integer
        
        '<remarks/>
        Public Password As String
        
        '<remarks/>
        Public UserName As String
    End Class
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.kCura.com/EDDS/TemplateManager")>  _
    Public Class ApplicationArtifact
        
        '<remarks/>
        Public ArtifactId As Integer
        
        '<remarks/>
        Public Name As String
        
        '<remarks/>
        Public Type As ApplicationArtifactType
    End Class
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.kCura.com/EDDS/TemplateManager")>  _
    Public Enum ApplicationArtifactType
        
        '<remarks/>
        [Case]
        
        '<remarks/>
        Code
        
        '<remarks/>
        Field
        
        '<remarks/>
        Layout
        
        '<remarks/>
        [Object]
        
        '<remarks/>
        Rule
        
        '<remarks/>
        Sync
        
        '<remarks/>
        Tab
        
        '<remarks/>
        View
    End Enum
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.kCura.com/EDDS/TemplateManager")>  _
    Public Class ApplicationInstallationResult
        
        '<remarks/>
        Public ApplicationArtifacts() As ApplicationArtifact
        
        '<remarks/>
        Public ExceptionMessage As String
        
        '<remarks/>
        Public Success As Boolean
    End Class
End Namespace
