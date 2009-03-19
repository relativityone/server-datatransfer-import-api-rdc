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
Namespace kCura.EDDS.WebAPI.DocumentManagerBase
    
    '<remarks/>
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Web.Services.WebServiceBindingAttribute(Name:="DocumentManagerSoap", [Namespace]:="http://foley.com/EDDS/DocumentManager"),  _
     System.Xml.Serialization.XmlIncludeAttribute(GetType(Artifact)),  _
     System.Xml.Serialization.XmlIncludeAttribute(GetType(Field()))>  _
    Public Class DocumentManager
        Inherits System.Web.Services.Protocols.SoapHttpClientProtocol
        
        '<remarks/>
        Public Sub New()
            MyBase.New
            Me.Url = "http://localhost/EDDSWebAPI/DocumentManager.asmx"
        End Sub
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://foley.com/EDDS/DocumentManager/GetAllDocumentsForCase", RequestNamespace:="http://foley.com/EDDS/DocumentManager", ResponseNamespace:="http://foley.com/EDDS/DocumentManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function GetAllDocumentsForCase(ByVal caseContextArtifactID As Integer) As System.Data.DataSet
            Dim results() As Object = Me.Invoke("GetAllDocumentsForCase", New Object() {caseContextArtifactID})
            Return CType(results(0),System.Data.DataSet)
        End Function
        
        '<remarks/>
        Public Function BeginGetAllDocumentsForCase(ByVal caseContextArtifactID As Integer, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("GetAllDocumentsForCase", New Object() {caseContextArtifactID}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndGetAllDocumentsForCase(ByVal asyncResult As System.IAsyncResult) As System.Data.DataSet
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),System.Data.DataSet)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://foley.com/EDDS/DocumentManager/NewFileDTO", RequestNamespace:="http://foley.com/EDDS/DocumentManager", ResponseNamespace:="http://foley.com/EDDS/DocumentManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function NewFileDTO() As File
            Dim results() As Object = Me.Invoke("NewFileDTO", New Object(-1) {})
            Return CType(results(0),File)
        End Function
        
        '<remarks/>
        Public Function BeginNewFileDTO(ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("NewFileDTO", New Object(-1) {}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndNewFileDTO(ByVal asyncResult As System.IAsyncResult) As File
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),File)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://foley.com/EDDS/DocumentManager/CreateEmptyDocument", RequestNamespace:="http://foley.com/EDDS/DocumentManager", ResponseNamespace:="http://foley.com/EDDS/DocumentManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function CreateEmptyDocument(ByVal caseContextArtifactID As Integer, ByVal parentFolderID As Integer, <System.Xml.Serialization.XmlElementAttribute(DataType:="base64Binary")> ByVal identifierValue() As Byte, ByVal identifierColumn As String, ByVal fullTextBuilder As FullTextBuilderDTO) As Integer
            Dim results() As Object = Me.Invoke("CreateEmptyDocument", New Object() {caseContextArtifactID, parentFolderID, identifierValue, identifierColumn, fullTextBuilder})
            Return CType(results(0),Integer)
        End Function
        
        '<remarks/>
        Public Function BeginCreateEmptyDocument(ByVal caseContextArtifactID As Integer, ByVal parentFolderID As Integer, ByVal identifierValue() As Byte, ByVal identifierColumn As String, ByVal fullTextBuilder As FullTextBuilderDTO, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("CreateEmptyDocument", New Object() {caseContextArtifactID, parentFolderID, identifierValue, identifierColumn, fullTextBuilder}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndCreateEmptyDocument(ByVal asyncResult As System.IAsyncResult) As Integer
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),Integer)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://foley.com/EDDS/DocumentManager/Read", RequestNamespace:="http://foley.com/EDDS/DocumentManager", ResponseNamespace:="http://foley.com/EDDS/DocumentManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function Read(ByVal caseContextArtifactID As Integer, ByVal artifactID As Integer, ByVal fieldArtifactIds() As Integer) As Document
            Dim results() As Object = Me.Invoke("Read", New Object() {caseContextArtifactID, artifactID, fieldArtifactIds})
            Return CType(results(0),Document)
        End Function
        
        '<remarks/>
        Public Function BeginRead(ByVal caseContextArtifactID As Integer, ByVal artifactID As Integer, ByVal fieldArtifactIds() As Integer, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("Read", New Object() {caseContextArtifactID, artifactID, fieldArtifactIds}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndRead(ByVal asyncResult As System.IAsyncResult) As Document
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),Document)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://foley.com/EDDS/DocumentManager/Create", RequestNamespace:="http://foley.com/EDDS/DocumentManager", ResponseNamespace:="http://foley.com/EDDS/DocumentManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function Create(ByVal caseContextArtifactID As Integer, ByVal docDTO As Document, ByVal files() As File) As Integer
            Dim results() As Object = Me.Invoke("Create", New Object() {caseContextArtifactID, docDTO, files})
            Return CType(results(0),Integer)
        End Function
        
        '<remarks/>
        Public Function BeginCreate(ByVal caseContextArtifactID As Integer, ByVal docDTO As Document, ByVal files() As File, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("Create", New Object() {caseContextArtifactID, docDTO, files}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndCreate(ByVal asyncResult As System.IAsyncResult) As Integer
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),Integer)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://foley.com/EDDS/DocumentManager/Update", RequestNamespace:="http://foley.com/EDDS/DocumentManager", ResponseNamespace:="http://foley.com/EDDS/DocumentManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function Update(ByVal caseContextArtifactID As Integer, ByVal docDTO As Document, ByVal files() As File) As Integer
            Dim results() As Object = Me.Invoke("Update", New Object() {caseContextArtifactID, docDTO, files})
            Return CType(results(0),Integer)
        End Function
        
        '<remarks/>
        Public Function BeginUpdate(ByVal caseContextArtifactID As Integer, ByVal docDTO As Document, ByVal files() As File, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("Update", New Object() {caseContextArtifactID, docDTO, files}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndUpdate(ByVal asyncResult As System.IAsyncResult) As Integer
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),Integer)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://foley.com/EDDS/DocumentManager/AddFullTextToDocument", RequestNamespace:="http://foley.com/EDDS/DocumentManager", ResponseNamespace:="http://foley.com/EDDS/DocumentManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function AddFullTextToDocument(ByVal caseContextArtifactID As Integer, ByVal documentArtifactID As Integer, ByVal fullTextBuilder As FullTextBuilderDTO) As Boolean
            Dim results() As Object = Me.Invoke("AddFullTextToDocument", New Object() {caseContextArtifactID, documentArtifactID, fullTextBuilder})
            Return CType(results(0),Boolean)
        End Function
        
        '<remarks/>
        Public Function BeginAddFullTextToDocument(ByVal caseContextArtifactID As Integer, ByVal documentArtifactID As Integer, ByVal fullTextBuilder As FullTextBuilderDTO, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("AddFullTextToDocument", New Object() {caseContextArtifactID, documentArtifactID, fullTextBuilder}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndAddFullTextToDocument(ByVal asyncResult As System.IAsyncResult) As Boolean
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),Boolean)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://foley.com/EDDS/DocumentManager/GetDocumentArtifactIDFromIdentifier", RequestNamespace:="http://foley.com/EDDS/DocumentManager", ResponseNamespace:="http://foley.com/EDDS/DocumentManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function GetDocumentArtifactIDFromIdentifier(ByVal caseContextArtifactID As Integer, ByVal identifier As String, ByVal fieldDisplayName As String) As Integer
            Dim results() As Object = Me.Invoke("GetDocumentArtifactIDFromIdentifier", New Object() {caseContextArtifactID, identifier, fieldDisplayName})
            Return CType(results(0),Integer)
        End Function
        
        '<remarks/>
        Public Function BeginGetDocumentArtifactIDFromIdentifier(ByVal caseContextArtifactID As Integer, ByVal identifier As String, ByVal fieldDisplayName As String, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("GetDocumentArtifactIDFromIdentifier", New Object() {caseContextArtifactID, identifier, fieldDisplayName}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndGetDocumentArtifactIDFromIdentifier(ByVal asyncResult As System.IAsyncResult) As Integer
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),Integer)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://foley.com/EDDS/DocumentManager/GetDocumentArtifactIDFromFieldValue", RequestNamespace:="http://foley.com/EDDS/DocumentManager", ResponseNamespace:="http://foley.com/EDDS/DocumentManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function GetDocumentArtifactIDFromFieldValue(ByVal caseContextArtifactID As Integer, ByVal identifier As String, ByVal fieldDisplayName As String) As Integer
            Dim results() As Object = Me.Invoke("GetDocumentArtifactIDFromFieldValue", New Object() {caseContextArtifactID, identifier, fieldDisplayName})
            Return CType(results(0),Integer)
        End Function
        
        '<remarks/>
        Public Function BeginGetDocumentArtifactIDFromFieldValue(ByVal caseContextArtifactID As Integer, ByVal identifier As String, ByVal fieldDisplayName As String, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("GetDocumentArtifactIDFromFieldValue", New Object() {caseContextArtifactID, identifier, fieldDisplayName}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndGetDocumentArtifactIDFromFieldValue(ByVal asyncResult As System.IAsyncResult) As Integer
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),Integer)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://foley.com/EDDS/DocumentManager/ReadFromIdentifier", RequestNamespace:="http://foley.com/EDDS/DocumentManager", ResponseNamespace:="http://foley.com/EDDS/DocumentManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function ReadFromIdentifier(ByVal caseContextArtifactID As Integer, ByVal fieldDisplayName As String, ByVal identifier As String, ByVal fieldArtifactIds() As Integer) As Document
            Dim results() As Object = Me.Invoke("ReadFromIdentifier", New Object() {caseContextArtifactID, fieldDisplayName, identifier, fieldArtifactIds})
            Return CType(results(0),Document)
        End Function
        
        '<remarks/>
        Public Function BeginReadFromIdentifier(ByVal caseContextArtifactID As Integer, ByVal fieldDisplayName As String, ByVal identifier As String, ByVal fieldArtifactIds() As Integer, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("ReadFromIdentifier", New Object() {caseContextArtifactID, fieldDisplayName, identifier, fieldArtifactIds}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndReadFromIdentifier(ByVal asyncResult As System.IAsyncResult) As Document
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),Document)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://foley.com/EDDS/DocumentManager/ReadFromIdentifierWithFileList", RequestNamespace:="http://foley.com/EDDS/DocumentManager", ResponseNamespace:="http://foley.com/EDDS/DocumentManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function ReadFromIdentifierWithFileList(ByVal caseContextArtifactID As Integer, ByVal fieldDisplayName As String, ByVal identifier As String, ByVal fieldArtifactIds() As Integer) As FullDocumentInfo
            Dim results() As Object = Me.Invoke("ReadFromIdentifierWithFileList", New Object() {caseContextArtifactID, fieldDisplayName, identifier, fieldArtifactIds})
            Return CType(results(0),FullDocumentInfo)
        End Function
        
        '<remarks/>
        Public Function BeginReadFromIdentifierWithFileList(ByVal caseContextArtifactID As Integer, ByVal fieldDisplayName As String, ByVal identifier As String, ByVal fieldArtifactIds() As Integer, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("ReadFromIdentifierWithFileList", New Object() {caseContextArtifactID, fieldDisplayName, identifier, fieldArtifactIds}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndReadFromIdentifierWithFileList(ByVal asyncResult As System.IAsyncResult) As FullDocumentInfo
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),FullDocumentInfo)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://foley.com/EDDS/DocumentManager/ClearImagesFromDocument", RequestNamespace:="http://foley.com/EDDS/DocumentManager", ResponseNamespace:="http://foley.com/EDDS/DocumentManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Sub ClearImagesFromDocument(ByVal caseContextArtifactID As Integer, ByVal documentArtifactID As Integer)
            Me.Invoke("ClearImagesFromDocument", New Object() {caseContextArtifactID, documentArtifactID})
        End Sub
        
        '<remarks/>
        Public Function BeginClearImagesFromDocument(ByVal caseContextArtifactID As Integer, ByVal documentArtifactID As Integer, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("ClearImagesFromDocument", New Object() {caseContextArtifactID, documentArtifactID}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Sub EndClearImagesFromDocument(ByVal asyncResult As System.IAsyncResult)
            Me.EndInvoke(asyncResult)
        End Sub
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://foley.com/EDDS/DocumentManager/GetDocumentDirectoryByCaseArtifactID", RequestNamespace:="http://foley.com/EDDS/DocumentManager", ResponseNamespace:="http://foley.com/EDDS/DocumentManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function GetDocumentDirectoryByCaseArtifactID(ByVal caseArtifactID As Integer) As String
            Dim results() As Object = Me.Invoke("GetDocumentDirectoryByCaseArtifactID", New Object() {caseArtifactID})
            Return CType(results(0),String)
        End Function
        
        '<remarks/>
        Public Function BeginGetDocumentDirectoryByCaseArtifactID(ByVal caseArtifactID As Integer, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("GetDocumentDirectoryByCaseArtifactID", New Object() {caseArtifactID}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndGetDocumentDirectoryByCaseArtifactID(ByVal asyncResult As System.IAsyncResult) As String
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),String)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://foley.com/EDDS/DocumentManager/GetPrintImageGuids", RequestNamespace:="http://foley.com/EDDS/DocumentManager", ResponseNamespace:="http://foley.com/EDDS/DocumentManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function GetPrintImageGuids(ByVal caseContextArtifactID As Integer, ByVal artifactID As Integer, ByVal orderedProductionIDList() As Integer) As System.Guid()
            Dim results() As Object = Me.Invoke("GetPrintImageGuids", New Object() {caseContextArtifactID, artifactID, orderedProductionIDList})
            Return CType(results(0),System.Guid())
        End Function
        
        '<remarks/>
        Public Function BeginGetPrintImageGuids(ByVal caseContextArtifactID As Integer, ByVal artifactID As Integer, ByVal orderedProductionIDList() As Integer, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("GetPrintImageGuids", New Object() {caseContextArtifactID, artifactID, orderedProductionIDList}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndGetPrintImageGuids(ByVal asyncResult As System.IAsyncResult) As System.Guid()
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),System.Guid())
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://foley.com/EDDS/DocumentManager/AllHaveOriginalImages", RequestNamespace:="http://foley.com/EDDS/DocumentManager", ResponseNamespace:="http://foley.com/EDDS/DocumentManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function AllHaveOriginalImages(ByVal caseContextArtifactID As Integer, ByVal artifactIDs() As Integer) As Boolean
            Dim results() As Object = Me.Invoke("AllHaveOriginalImages", New Object() {caseContextArtifactID, artifactIDs})
            Return CType(results(0),Boolean)
        End Function
        
        '<remarks/>
        Public Function BeginAllHaveOriginalImages(ByVal caseContextArtifactID As Integer, ByVal artifactIDs() As Integer, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("AllHaveOriginalImages", New Object() {caseContextArtifactID, artifactIDs}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndAllHaveOriginalImages(ByVal asyncResult As System.IAsyncResult) As Boolean
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),Boolean)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://foley.com/EDDS/DocumentManager/GetIdentifierFromDocumentArtifactID", RequestNamespace:="http://foley.com/EDDS/DocumentManager", ResponseNamespace:="http://foley.com/EDDS/DocumentManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function GetIdentifierFromDocumentArtifactID(ByVal caseContextArtifactID As Integer, ByVal artifactID As Integer) As String
            Dim results() As Object = Me.Invoke("GetIdentifierFromDocumentArtifactID", New Object() {caseContextArtifactID, artifactID})
            Return CType(results(0),String)
        End Function
        
        '<remarks/>
        Public Function BeginGetIdentifierFromDocumentArtifactID(ByVal caseContextArtifactID As Integer, ByVal artifactID As Integer, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("GetIdentifierFromDocumentArtifactID", New Object() {caseContextArtifactID, artifactID}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndGetIdentifierFromDocumentArtifactID(ByVal asyncResult As System.IAsyncResult) As String
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),String)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://foley.com/EDDS/DocumentManager/RetrieveAllUnsupportedOiFileIds", RequestNamespace:="http://foley.com/EDDS/DocumentManager", ResponseNamespace:="http://foley.com/EDDS/DocumentManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function RetrieveAllUnsupportedOiFileIds() As Integer()
            Dim results() As Object = Me.Invoke("RetrieveAllUnsupportedOiFileIds", New Object(-1) {})
            Return CType(results(0),Integer())
        End Function
        
        '<remarks/>
        Public Function BeginRetrieveAllUnsupportedOiFileIds(ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("RetrieveAllUnsupportedOiFileIds", New Object(-1) {}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndRetrieveAllUnsupportedOiFileIds(ByVal asyncResult As System.IAsyncResult) As Integer()
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),Integer())
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://foley.com/EDDS/DocumentManager/ReadAsNameValueCollectionFromLayoutArtifact"& _ 
"ID", RequestNamespace:="http://foley.com/EDDS/DocumentManager", ResponseNamespace:="http://foley.com/EDDS/DocumentManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function ReadAsNameValueCollectionFromLayoutArtifactID(ByVal caseContextArtifactID As Integer, ByVal artifactID As Integer, ByVal layoutArtifactID As Integer, ByVal utcOffset As Integer, ByVal locale As String) As NameValuePair()
            Dim results() As Object = Me.Invoke("ReadAsNameValueCollectionFromLayoutArtifactID", New Object() {caseContextArtifactID, artifactID, layoutArtifactID, utcOffset, locale})
            Return CType(results(0),NameValuePair())
        End Function
        
        '<remarks/>
        Public Function BeginReadAsNameValueCollectionFromLayoutArtifactID(ByVal caseContextArtifactID As Integer, ByVal artifactID As Integer, ByVal layoutArtifactID As Integer, ByVal utcOffset As Integer, ByVal locale As String, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("ReadAsNameValueCollectionFromLayoutArtifactID", New Object() {caseContextArtifactID, artifactID, layoutArtifactID, utcOffset, locale}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndReadAsNameValueCollectionFromLayoutArtifactID(ByVal asyncResult As System.IAsyncResult) As NameValuePair()
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),NameValuePair())
        End Function
    End Class
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://foley.com/EDDS/DocumentManager")>  _
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
        
        '<remarks/>
        Public Redactions() As Redaction
        
        '<remarks/>
        Public Identifier As String
        
        '<remarks/>
        Public Location As String
    End Class
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://foley.com/EDDS/DocumentManager")>  _
    Public Class Redaction
        
        '<remarks/>
        Public ID As Integer
        
        '<remarks/>
        Public FileGuid As String
        
        '<remarks/>
        Public X As Integer
        
        '<remarks/>
        Public Y As Integer
        
        '<remarks/>
        Public Width As Integer
        
        '<remarks/>
        Public Height As Integer
        
        '<remarks/>
        Public Type As String
        
        '<remarks/>
        Public MarkupSetArtifactID As Integer
    End Class
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://foley.com/EDDS/DocumentManager")>  _
    Public Class DocumentAgentFlags
        
        '<remarks/>
        Public UpdateFullText As Boolean
        
        '<remarks/>
        Public IndexStatus As Integer
    End Class
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://foley.com/EDDS/DocumentManager")>  _
    Public Class FullDocumentInfo
        
        '<remarks/>
        Public DocumentDTO As Document
        
        '<remarks/>
        Public FileList() As File
    End Class
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://foley.com/EDDS/DocumentManager")>  _
    Public Class Document
        Inherits Artifact
        
        '<remarks/>
        Public Fields() As Field
        
        '<remarks/>
        Public DocumentAgentFlags As DocumentAgentFlags
        
        '<remarks/>
        Public HasImages As Boolean
        
        '<remarks/>
        Public HasNative As Boolean
        
        '<remarks/>
        Public HasProducedImages As Boolean
        
        '<remarks/>
        Public NativeGUID As String
    End Class
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://foley.com/EDDS/DocumentManager")>  _
    Public Class Field
        Inherits Artifact
        
        '<remarks/>
        Public FieldArtifactTypeID As Integer
        
        '<remarks/>
        Public DisplayName As String
        
        '<remarks/>
        Public FieldTypeID As Integer
        
        '<remarks/>
        Public FieldType As FieldType
        
        '<remarks/>
        Public FieldCategoryID As Integer
        
        '<remarks/>
        Public FieldCategory As FieldCategory
        
        '<remarks/>
        Public ArtifactViewFieldID As Integer
        
        '<remarks/>
        Public CodeTypeID As NullableInt32
        
        '<remarks/>
        Public MaxLength As NullableInt32
        
        '<remarks/>
        Public IsRequired As Boolean
        
        '<remarks/>
        Public IsRemovable As Boolean
        
        '<remarks/>
        Public IsEditable As Boolean
        
        '<remarks/>
        Public IsVisible As Boolean
        
        '<remarks/>
        Public IsArtifactBaseField As Boolean
        
        '<remarks/>
        Public Value As Object
        
        '<remarks/>
        Public TableName As String
        
        '<remarks/>
        Public ColumnName As String
        
        '<remarks/>
        Public IsReadOnlyInLayout As Boolean
        
        '<remarks/>
        Public FilterType As String
        
        '<remarks/>
        Public FieldDisplayTypeID As Integer
        
        '<remarks/>
        Public Rows As Integer
        
        '<remarks/>
        Public IsLinked As Boolean
        
        '<remarks/>
        Public FormatString As String
        
        '<remarks/>
        Public RepeatColumn As NullableInt32
        
        '<remarks/>
        Public AssociativeArtifactTypeID As NullableInt32
        
        '<remarks/>
        Public IsAvailableToAssociativeObjects As Boolean
        
        '<remarks/>
        Public IsAvailableInChoiceTree As Boolean
        
        '<remarks/>
        Public IsGroupByEnabled As Boolean
        
        '<remarks/>
        Public IsIndexEnabled As Boolean
        
        '<remarks/>
        Public DisplayValueTrue As String
        
        '<remarks/>
        Public DisplayValueFalse As String
        
        '<remarks/>
        Public Width As String
        
        '<remarks/>
        Public Wrapping As Boolean
        
        '<remarks/>
        Public LinkLayoutArtifactID As Integer
        
        '<remarks/>
        Public NameValue As String
        
        '<remarks/>
        Public LinkType As Boolean
        
        '<remarks/>
        Public UseUnicodeEncoding As Boolean
        
        '<remarks/>
        Public AllowHtml As Boolean
        
        '<remarks/>
        Public IsSortable As Boolean
        
        '<remarks/>
        Public FriendlyName As String
        
        '<remarks/>
        Public RelationalIndexViewArtifactID As NullableInt32
    End Class
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://foley.com/EDDS/DocumentManager")>  _
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
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://foley.com/EDDS/DocumentManager")>  _
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
    End Enum
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://foley.com/EDDS/DocumentManager"),  _
     System.Xml.Serialization.XmlIncludeAttribute(GetType(Field)),  _
     System.Xml.Serialization.XmlIncludeAttribute(GetType(Document))>  _
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
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://foley.com/EDDS/DocumentManager")>  _
    Public Class NameValuePair
        
        '<remarks/>
        Public Name As String
        
        '<remarks/>
        Public Value As String
    End Class
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://foley.com/EDDS/DocumentManager")>  _
    Public Class FullTextBuilderDTO
        
        '<remarks/>
        Public FilePointer As String
        
        '<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(DataType:="base64Binary")>  _
        Public FullText() As Byte
        
        '<remarks/>
        Public Pages() As Integer
    End Class
End Namespace
