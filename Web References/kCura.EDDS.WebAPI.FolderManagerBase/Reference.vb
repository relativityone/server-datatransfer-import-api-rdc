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
Namespace kCura.EDDS.WebAPI.FolderManagerBase
    
    '<remarks/>
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Web.Services.WebServiceBindingAttribute(Name:="FolderManagerSoap", [Namespace]:="http://www.kCura.com/EDDS/FolderManager"),  _
     System.Xml.Serialization.XmlIncludeAttribute(GetType(Artifact))>  _
    Public Class FolderManager
        Inherits System.Web.Services.Protocols.SoapHttpClientProtocol
        
        '<remarks/>
        Public Sub New()
            MyBase.New
            Me.Url = "http://localhost/EDDSWebAPI/FolderManager.asmx"
        End Sub
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/FolderManager/RetrieveAllByCaseID", RequestNamespace:="http://www.kCura.com/EDDS/FolderManager", ResponseNamespace:="http://www.kCura.com/EDDS/FolderManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function RetrieveAllByCaseID(ByVal caseContextArtifactID As Integer) As System.Data.DataSet
            Dim results() As Object = Me.Invoke("RetrieveAllByCaseID", New Object() {caseContextArtifactID})
            Return CType(results(0),System.Data.DataSet)
        End Function
        
        '<remarks/>
        Public Function BeginRetrieveAllByCaseID(ByVal caseContextArtifactID As Integer, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("RetrieveAllByCaseID", New Object() {caseContextArtifactID}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndRetrieveAllByCaseID(ByVal asyncResult As System.IAsyncResult) As System.Data.DataSet
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),System.Data.DataSet)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/FolderManager/RetrieveFolderAndDescendants", RequestNamespace:="http://www.kCura.com/EDDS/FolderManager", ResponseNamespace:="http://www.kCura.com/EDDS/FolderManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function RetrieveFolderAndDescendants(ByVal caseContextArtifactID As Integer, ByVal folderID As Integer) As System.Data.DataSet
            Dim results() As Object = Me.Invoke("RetrieveFolderAndDescendants", New Object() {caseContextArtifactID, folderID})
            Return CType(results(0),System.Data.DataSet)
        End Function
        
        '<remarks/>
        Public Function BeginRetrieveFolderAndDescendants(ByVal caseContextArtifactID As Integer, ByVal folderID As Integer, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("RetrieveFolderAndDescendants", New Object() {caseContextArtifactID, folderID}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndRetrieveFolderAndDescendants(ByVal asyncResult As System.IAsyncResult) As System.Data.DataSet
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),System.Data.DataSet)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/FolderManager/Read", RequestNamespace:="http://www.kCura.com/EDDS/FolderManager", ResponseNamespace:="http://www.kCura.com/EDDS/FolderManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function Read(ByVal caseContextArtifactID As Integer, ByVal folderArtifactID As Integer) As Folder
            Dim results() As Object = Me.Invoke("Read", New Object() {caseContextArtifactID, folderArtifactID})
            Return CType(results(0),Folder)
        End Function
        
        '<remarks/>
        Public Function BeginRead(ByVal caseContextArtifactID As Integer, ByVal folderArtifactID As Integer, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("Read", New Object() {caseContextArtifactID, folderArtifactID}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndRead(ByVal asyncResult As System.IAsyncResult) As Folder
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),Folder)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/FolderManager/ReadID", RequestNamespace:="http://www.kCura.com/EDDS/FolderManager", ResponseNamespace:="http://www.kCura.com/EDDS/FolderManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function ReadID(ByVal caseContextArtifactID As Integer, ByVal parentArtifactID As Integer, ByVal name As String) As Integer
            Dim results() As Object = Me.Invoke("ReadID", New Object() {caseContextArtifactID, parentArtifactID, name})
            Return CType(results(0),Integer)
        End Function
        
        '<remarks/>
        Public Function BeginReadID(ByVal caseContextArtifactID As Integer, ByVal parentArtifactID As Integer, ByVal name As String, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("ReadID", New Object() {caseContextArtifactID, parentArtifactID, name}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndReadID(ByVal asyncResult As System.IAsyncResult) As Integer
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),Integer)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/FolderManager/Retrieve", RequestNamespace:="http://www.kCura.com/EDDS/FolderManager", ResponseNamespace:="http://www.kCura.com/EDDS/FolderManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function Retrieve(ByVal caseContextArtifactID As Integer, ByVal parentArtifactID As Integer, ByVal folderName As String) As Integer
            Dim results() As Object = Me.Invoke("Retrieve", New Object() {caseContextArtifactID, parentArtifactID, folderName})
            Return CType(results(0),Integer)
        End Function
        
        '<remarks/>
        Public Function BeginRetrieve(ByVal caseContextArtifactID As Integer, ByVal parentArtifactID As Integer, ByVal folderName As String, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("Retrieve", New Object() {caseContextArtifactID, parentArtifactID, folderName}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndRetrieve(ByVal asyncResult As System.IAsyncResult) As Integer
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),Integer)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/FolderManager/Create", RequestNamespace:="http://www.kCura.com/EDDS/FolderManager", ResponseNamespace:="http://www.kCura.com/EDDS/FolderManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function Create(ByVal caseContextArtifactID As Integer, ByVal parentArtifactID As Integer, ByVal folderName As String) As Integer
            Dim results() As Object = Me.Invoke("Create", New Object() {caseContextArtifactID, parentArtifactID, folderName})
            Return CType(results(0),Integer)
        End Function
        
        '<remarks/>
        Public Function BeginCreate(ByVal caseContextArtifactID As Integer, ByVal parentArtifactID As Integer, ByVal folderName As String, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("Create", New Object() {caseContextArtifactID, parentArtifactID, folderName}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndCreate(ByVal asyncResult As System.IAsyncResult) As Integer
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),Integer)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/FolderManager/Exists", RequestNamespace:="http://www.kCura.com/EDDS/FolderManager", ResponseNamespace:="http://www.kCura.com/EDDS/FolderManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function Exists(ByVal caseContextArtifactID As Integer, ByVal folderArtifactID As Integer) As Boolean
            Dim results() As Object = Me.Invoke("Exists", New Object() {caseContextArtifactID, folderArtifactID})
            Return CType(results(0),Boolean)
        End Function
        
        '<remarks/>
        Public Function BeginExists(ByVal caseContextArtifactID As Integer, ByVal folderArtifactID As Integer, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("Exists", New Object() {caseContextArtifactID, folderArtifactID}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndExists(ByVal asyncResult As System.IAsyncResult) As Boolean
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),Boolean)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/FolderManager/RetrieveIntitialChunk", RequestNamespace:="http://www.kCura.com/EDDS/FolderManager", ResponseNamespace:="http://www.kCura.com/EDDS/FolderManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function RetrieveIntitialChunk(ByVal caseContextArtifactID As Integer) As System.Data.DataSet
            Dim results() As Object = Me.Invoke("RetrieveIntitialChunk", New Object() {caseContextArtifactID})
            Return CType(results(0),System.Data.DataSet)
        End Function
        
        '<remarks/>
        Public Function BeginRetrieveIntitialChunk(ByVal caseContextArtifactID As Integer, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("RetrieveIntitialChunk", New Object() {caseContextArtifactID}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndRetrieveIntitialChunk(ByVal asyncResult As System.IAsyncResult) As System.Data.DataSet
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),System.Data.DataSet)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/FolderManager/RetrieveNextChunk", RequestNamespace:="http://www.kCura.com/EDDS/FolderManager", ResponseNamespace:="http://www.kCura.com/EDDS/FolderManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function RetrieveNextChunk(ByVal caseContextArtifactID As Integer, ByVal lastFolderID As Integer) As System.Data.DataSet
            Dim results() As Object = Me.Invoke("RetrieveNextChunk", New Object() {caseContextArtifactID, lastFolderID})
            Return CType(results(0),System.Data.DataSet)
        End Function
        
        '<remarks/>
        Public Function BeginRetrieveNextChunk(ByVal caseContextArtifactID As Integer, ByVal lastFolderID As Integer, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("RetrieveNextChunk", New Object() {caseContextArtifactID, lastFolderID}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndRetrieveNextChunk(ByVal asyncResult As System.IAsyncResult) As System.Data.DataSet
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),System.Data.DataSet)
        End Function
    End Class
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.kCura.com/EDDS/FolderManager")>  _
    Public Class Folder
        Inherits Artifact
        
        '<remarks/>
        Public Name As String
    End Class
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.kCura.com/EDDS/FolderManager"),  _
     System.Xml.Serialization.XmlIncludeAttribute(GetType(Folder))>  _
    Public Class Artifact
        
        '<remarks/>
        Public ArtifactID As Integer
        
        '<remarks/>
        Public ArtifactTypeID As Integer
        
        '<remarks/>
        Public ParentArtifactID As NullableInt32
        
        '<remarks/>
        Public ContainerID As NullableInt32
        
        '<remarks/>
        Public AccessControlListID As Integer
        
        '<remarks/>
        Public AccessControlListIsInherited As Boolean
        
        '<remarks/>
        Public Keywords As String
        
        '<remarks/>
        Public Notes As String
        
        '<remarks/>
        Public TextIdentifier As String
        
        '<remarks/>
        Public LastModifiedOn As Date
        
        '<remarks/>
        Public LastModifiedBy As Integer
        
        '<remarks/>
        Public CreatedBy As Integer
        
        '<remarks/>
        Public CreatedOn As Date
        
        '<remarks/>
        Public DeleteFlag As Boolean
    End Class
End Namespace
