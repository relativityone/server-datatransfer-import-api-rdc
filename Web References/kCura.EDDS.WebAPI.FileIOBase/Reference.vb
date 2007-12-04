﻿'------------------------------------------------------------------------------
' <autogenerated>
'     This code was generated by a tool.
'     Runtime Version: 1.1.4322.573
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
'This source code was auto-generated by Microsoft.VSDesigner, Version 1.1.4322.573.
'
Namespace kCura.EDDS.WebAPI.FileIOBase
    
    '<remarks/>
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Web.Services.WebServiceBindingAttribute(Name:="FileIOSoap", [Namespace]:="http://foley.com/EDDS/FileManager")>  _
    Public Class FileIO
        Inherits System.Web.Services.Protocols.SoapHttpClientProtocol
        
        '<remarks/>
        Public Sub New()
            MyBase.New
            Me.Url = "http://localhost/EDDSWebAPI/FileIO.asmx"
        End Sub
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://foley.com/EDDS/FileManager/BeginFill", RequestNamespace:="http://foley.com/EDDS/FileManager", ResponseNamespace:="http://foley.com/EDDS/FileManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function BeginFill(ByVal caseContextArtifactID As Integer, <System.Xml.Serialization.XmlElementAttribute(DataType:="base64Binary")> ByVal b() As Byte, ByVal documentDirectory As String, ByVal fileGuid As String) As String
            Dim results() As Object = Me.Invoke("BeginFill", New Object() {caseContextArtifactID, b, documentDirectory, fileGuid})
            Return CType(results(0),String)
        End Function
        
        '<remarks/>
        Public Function BeginBeginFill(ByVal caseContextArtifactID As Integer, ByVal b() As Byte, ByVal documentDirectory As String, ByVal fileGuid As String, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("BeginFill", New Object() {caseContextArtifactID, b, documentDirectory, fileGuid}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndBeginFill(ByVal asyncResult As System.IAsyncResult) As String
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),String)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://foley.com/EDDS/FileManager/FileFill", RequestNamespace:="http://foley.com/EDDS/FileManager", ResponseNamespace:="http://foley.com/EDDS/FileManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function FileFill(ByVal caseContextArtifactID As Integer, ByVal documentDirectory As String, ByVal fileName As String, <System.Xml.Serialization.XmlElementAttribute(DataType:="base64Binary")> ByVal b() As Byte) As Boolean
            Dim results() As Object = Me.Invoke("FileFill", New Object() {caseContextArtifactID, documentDirectory, fileName, b})
            Return CType(results(0),Boolean)
        End Function
        
        '<remarks/>
        Public Function BeginFileFill(ByVal caseContextArtifactID As Integer, ByVal documentDirectory As String, ByVal fileName As String, ByVal b() As Byte, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("FileFill", New Object() {caseContextArtifactID, documentDirectory, fileName, b}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndFileFill(ByVal asyncResult As System.IAsyncResult) As Boolean
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),Boolean)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://foley.com/EDDS/FileManager/RemoveFill", RequestNamespace:="http://foley.com/EDDS/FileManager", ResponseNamespace:="http://foley.com/EDDS/FileManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Sub RemoveFill(ByVal caseContextArtifactID As Integer, ByVal documentDirectory As String, ByVal fileName As String)
            Me.Invoke("RemoveFill", New Object() {caseContextArtifactID, documentDirectory, fileName})
        End Sub
        
        '<remarks/>
        Public Function BeginRemoveFill(ByVal caseContextArtifactID As Integer, ByVal documentDirectory As String, ByVal fileName As String, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("RemoveFill", New Object() {caseContextArtifactID, documentDirectory, fileName}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Sub EndRemoveFill(ByVal asyncResult As System.IAsyncResult)
            Me.EndInvoke(asyncResult)
        End Sub
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://foley.com/EDDS/FileManager/ReadFileAsString", RequestNamespace:="http://foley.com/EDDS/FileManager", ResponseNamespace:="http://foley.com/EDDS/FileManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function ReadFileAsString(ByVal path As String) As <System.Xml.Serialization.XmlElementAttribute(DataType:="base64Binary")> Byte()
            Dim results() As Object = Me.Invoke("ReadFileAsString", New Object() {path})
            Return CType(results(0),Byte())
        End Function
        
        '<remarks/>
        Public Function BeginReadFileAsString(ByVal path As String, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("ReadFileAsString", New Object() {path}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndReadFileAsString(ByVal asyncResult As System.IAsyncResult) As Byte()
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),Byte())
        End Function
    End Class
End Namespace
