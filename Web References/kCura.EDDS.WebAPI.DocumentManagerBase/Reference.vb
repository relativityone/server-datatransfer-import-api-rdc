﻿'------------------------------------------------------------------------------
' <autogenerated>
'     This code was generated by a tool.
'     Runtime Version: 1.1.4322.2032
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
'This source code was auto-generated by Microsoft.VSDesigner, Version 1.1.4322.2032.
'
Namespace kCura.EDDS.WebAPI.DocumentManagerBase
    
    '<remarks/>
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Web.Services.WebServiceBindingAttribute(Name:="DocumentManagerSoap", [Namespace]:="http://foley.com/EDDS/DocumentManager"),  _
     System.Xml.Serialization.XmlIncludeAttribute(GetType(Artifact))>  _
    Public Class DocumentManager
        Inherits System.Web.Services.Protocols.SoapHttpClientProtocol
        
        '<remarks/>
        Public Sub New()
            MyBase.New
            Me.Url = "http://localhost/EDDS/WebAPI/DocumentManager.asmx"
        End Sub
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://foley.com/EDDS/DocumentManager/GetAllDocumentsForCase", RequestNamespace:="http://foley.com/EDDS/DocumentManager", ResponseNamespace:="http://foley.com/EDDS/DocumentManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function GetAllDocumentsForCase(ByVal caseID As Integer) As System.Data.DataSet
            Dim results() As Object = Me.Invoke("GetAllDocumentsForCase", New Object() {caseID})
            Return CType(results(0),System.Data.DataSet)
        End Function
        
        '<remarks/>
        Public Function BeginGetAllDocumentsForCase(ByVal caseID As Integer, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("GetAllDocumentsForCase", New Object() {caseID}, callback, asyncState)
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
        Public Function CreateEmptyDocument(ByVal parentFolderID As Integer, <System.Xml.Serialization.XmlElementAttribute(DataType:="base64Binary")> ByVal identifierValue() As Byte, ByVal fullTextFileName As String, ByVal identifierColumn As String) As Integer
            Dim results() As Object = Me.Invoke("CreateEmptyDocument", New Object() {parentFolderID, identifierValue, fullTextFileName, identifierColumn})
            Return CType(results(0),Integer)
        End Function
        
        '<remarks/>
        Public Function BeginCreateEmptyDocument(ByVal parentFolderID As Integer, ByVal identifierValue() As Byte, ByVal fullTextFileName As String, ByVal identifierColumn As String, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("CreateEmptyDocument", New Object() {parentFolderID, identifierValue, fullTextFileName, identifierColumn}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndCreateEmptyDocument(ByVal asyncResult As System.IAsyncResult) As Integer
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),Integer)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://foley.com/EDDS/DocumentManager/ReadFromDocumentID", RequestNamespace:="http://foley.com/EDDS/DocumentManager", ResponseNamespace:="http://foley.com/EDDS/DocumentManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function ReadFromDocumentID(ByVal documentID As Integer, ByVal contextArtifactID As Integer) As Document
            Dim results() As Object = Me.Invoke("ReadFromDocumentID", New Object() {documentID, contextArtifactID})
            Return CType(results(0),Document)
        End Function
        
        '<remarks/>
        Public Function BeginReadFromDocumentID(ByVal documentID As Integer, ByVal contextArtifactID As Integer, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("ReadFromDocumentID", New Object() {documentID, contextArtifactID}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndReadFromDocumentID(ByVal asyncResult As System.IAsyncResult) As Document
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),Document)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://foley.com/EDDS/DocumentManager/DeleteNative", RequestNamespace:="http://foley.com/EDDS/DocumentManager", ResponseNamespace:="http://foley.com/EDDS/DocumentManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function DeleteNative(ByVal documentDTO As Document) As Boolean
            Dim results() As Object = Me.Invoke("DeleteNative", New Object() {documentDTO})
            Return CType(results(0),Boolean)
        End Function
        
        '<remarks/>
        Public Function BeginDeleteNative(ByVal documentDTO As Document, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("DeleteNative", New Object() {documentDTO}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndDeleteNative(ByVal asyncResult As System.IAsyncResult) As Boolean
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),Boolean)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://foley.com/EDDS/DocumentManager/Create", RequestNamespace:="http://foley.com/EDDS/DocumentManager", ResponseNamespace:="http://foley.com/EDDS/DocumentManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function Create(ByVal docDTO As Document) As Integer
            Dim results() As Object = Me.Invoke("Create", New Object() {docDTO})
            Return CType(results(0),Integer)
        End Function
        
        '<remarks/>
        Public Function BeginCreate(ByVal docDTO As Document, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("Create", New Object() {docDTO}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndCreate(ByVal asyncResult As System.IAsyncResult) As Integer
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),Integer)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://foley.com/EDDS/DocumentManager/Update", RequestNamespace:="http://foley.com/EDDS/DocumentManager", ResponseNamespace:="http://foley.com/EDDS/DocumentManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function Update(ByVal docDTO As Document) As Integer
            Dim results() As Object = Me.Invoke("Update", New Object() {docDTO})
            Return CType(results(0),Integer)
        End Function
        
        '<remarks/>
        Public Function BeginUpdate(ByVal docDTO As Document, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("Update", New Object() {docDTO}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndUpdate(ByVal asyncResult As System.IAsyncResult) As Integer
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),Integer)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://foley.com/EDDS/DocumentManager/CreateRange", RequestNamespace:="http://foley.com/EDDS/DocumentManager", ResponseNamespace:="http://foley.com/EDDS/DocumentManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Sub CreateRange(ByVal docDTOs() As Document)
            Me.Invoke("CreateRange", New Object() {docDTOs})
        End Sub
        
        '<remarks/>
        Public Function BeginCreateRange(ByVal docDTOs() As Document, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("CreateRange", New Object() {docDTOs}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Sub EndCreateRange(ByVal asyncResult As System.IAsyncResult)
            Me.EndInvoke(asyncResult)
        End Sub
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://foley.com/EDDS/DocumentManager/UpdateRange", RequestNamespace:="http://foley.com/EDDS/DocumentManager", ResponseNamespace:="http://foley.com/EDDS/DocumentManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Sub UpdateRange(ByVal docDTOs() As Document)
            Me.Invoke("UpdateRange", New Object() {docDTOs})
        End Sub
        
        '<remarks/>
        Public Function BeginUpdateRange(ByVal docDTOs() As Document, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("UpdateRange", New Object() {docDTOs}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Sub EndUpdateRange(ByVal asyncResult As System.IAsyncResult)
            Me.EndInvoke(asyncResult)
        End Sub
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://foley.com/EDDS/DocumentManager/AddFullTextToDocumentFromFile", RequestNamespace:="http://foley.com/EDDS/DocumentManager", ResponseNamespace:="http://foley.com/EDDS/DocumentManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function AddFullTextToDocumentFromFile(ByVal contextArtifactID As Integer, ByVal documentID As Integer, ByVal fullTextFileName As String) As Boolean
            Dim results() As Object = Me.Invoke("AddFullTextToDocumentFromFile", New Object() {contextArtifactID, documentID, fullTextFileName})
            Return CType(results(0),Boolean)
        End Function
        
        '<remarks/>
        Public Function BeginAddFullTextToDocumentFromFile(ByVal contextArtifactID As Integer, ByVal documentID As Integer, ByVal fullTextFileName As String, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("AddFullTextToDocumentFromFile", New Object() {contextArtifactID, documentID, fullTextFileName}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndAddFullTextToDocumentFromFile(ByVal asyncResult As System.IAsyncResult) As Boolean
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),Boolean)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://foley.com/EDDS/DocumentManager/GetDocumentIDFromIdentifier", RequestNamespace:="http://foley.com/EDDS/DocumentManager", ResponseNamespace:="http://foley.com/EDDS/DocumentManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function GetDocumentIDFromIdentifier(ByVal identifier As String, ByVal fieldDisplayName As String, ByVal caseID As Integer) As Integer
            Dim results() As Object = Me.Invoke("GetDocumentIDFromIdentifier", New Object() {identifier, fieldDisplayName, caseID})
            Return CType(results(0),Integer)
        End Function
        
        '<remarks/>
        Public Function BeginGetDocumentIDFromIdentifier(ByVal identifier As String, ByVal fieldDisplayName As String, ByVal caseID As Integer, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("GetDocumentIDFromIdentifier", New Object() {identifier, fieldDisplayName, caseID}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndGetDocumentIDFromIdentifier(ByVal asyncResult As System.IAsyncResult) As Integer
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),Integer)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://foley.com/EDDS/DocumentManager/ReadFromIdentifier", RequestNamespace:="http://foley.com/EDDS/DocumentManager", ResponseNamespace:="http://foley.com/EDDS/DocumentManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function ReadFromIdentifier(ByVal caseID As Integer, ByVal fieldDisplayName As String, ByVal identifier As String) As Document
            Dim results() As Object = Me.Invoke("ReadFromIdentifier", New Object() {caseID, fieldDisplayName, identifier})
            Return CType(results(0),Document)
        End Function
        
        '<remarks/>
        Public Function BeginReadFromIdentifier(ByVal caseID As Integer, ByVal fieldDisplayName As String, ByVal identifier As String, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("ReadFromIdentifier", New Object() {caseID, fieldDisplayName, identifier}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndReadFromIdentifier(ByVal asyncResult As System.IAsyncResult) As Document
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),Document)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://foley.com/EDDS/DocumentManager/UpdateFullTextWithCrackedText", RequestNamespace:="http://foley.com/EDDS/DocumentManager", ResponseNamespace:="http://foley.com/EDDS/DocumentManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Sub UpdateFullTextWithCrackedText(ByVal documentArtifactID As Integer, ByVal fileGuid As String)
            Me.Invoke("UpdateFullTextWithCrackedText", New Object() {documentArtifactID, fileGuid})
        End Sub
        
        '<remarks/>
        Public Function BeginUpdateFullTextWithCrackedText(ByVal documentArtifactID As Integer, ByVal fileGuid As String, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("UpdateFullTextWithCrackedText", New Object() {documentArtifactID, fileGuid}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Sub EndUpdateFullTextWithCrackedText(ByVal asyncResult As System.IAsyncResult)
            Me.EndInvoke(asyncResult)
        End Sub
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://foley.com/EDDS/DocumentManager/ClearImagesFromDocument", RequestNamespace:="http://foley.com/EDDS/DocumentManager", ResponseNamespace:="http://foley.com/EDDS/DocumentManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Sub ClearImagesFromDocument(ByVal caseID As Integer, ByVal documentID As Integer)
            Me.Invoke("ClearImagesFromDocument", New Object() {caseID, documentID})
        End Sub
        
        '<remarks/>
        Public Function BeginClearImagesFromDocument(ByVal caseID As Integer, ByVal documentID As Integer, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("ClearImagesFromDocument", New Object() {caseID, documentID}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Sub EndClearImagesFromDocument(ByVal asyncResult As System.IAsyncResult)
            Me.EndInvoke(asyncResult)
        End Sub
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://foley.com/EDDS/DocumentManager/GetDocumentDirectoryByContextArtifactID", RequestNamespace:="http://foley.com/EDDS/DocumentManager", ResponseNamespace:="http://foley.com/EDDS/DocumentManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function GetDocumentDirectoryByContextArtifactID(ByVal contextArtifactID As Integer) As String
            Dim results() As Object = Me.Invoke("GetDocumentDirectoryByContextArtifactID", New Object() {contextArtifactID})
            Return CType(results(0),String)
        End Function
        
        '<remarks/>
        Public Function BeginGetDocumentDirectoryByContextArtifactID(ByVal contextArtifactID As Integer, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("GetDocumentDirectoryByContextArtifactID", New Object() {contextArtifactID}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndGetDocumentDirectoryByContextArtifactID(ByVal asyncResult As System.IAsyncResult) As String
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),String)
        End Function
        
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
        Public Function GetPrintImageGuids(ByVal artifactID As Integer, ByVal orderedProductionIDList() As Integer) As System.Guid()
            Dim results() As Object = Me.Invoke("GetPrintImageGuids", New Object() {artifactID, orderedProductionIDList})
            Return CType(results(0),System.Guid())
        End Function
        
        '<remarks/>
        Public Function BeginGetPrintImageGuids(ByVal artifactID As Integer, ByVal orderedProductionIDList() As Integer, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("GetPrintImageGuids", New Object() {artifactID, orderedProductionIDList}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndGetPrintImageGuids(ByVal asyncResult As System.IAsyncResult) As System.Guid()
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),System.Guid())
        End Function
    End Class
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://foley.com/EDDS/DocumentManager")>  _
    Public Class File
        
        '<remarks/>
        Public Guid As String
        
        '<remarks/>
        Public DocumentID As Integer
        
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
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://foley.com/EDDS/DocumentManager")>  _
    Public Class DocumentAgentFlags
        
        '<remarks/>
        Public UpdateFullText As Boolean
        
        '<remarks/>
        Public IndexStatus As Integer
    End Class
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://foley.com/EDDS/DocumentManager"),  _
     System.Xml.Serialization.XmlIncludeAttribute(GetType(Document)),  _
     System.Xml.Serialization.XmlIncludeAttribute(GetType(Field))>  _
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
    Public Class Document
        Inherits Artifact
        
        '<remarks/>
        Public Fields() As Field
        
        '<remarks/>
        Public DocumentID As Integer
        
        '<remarks/>
        Public Files() As File
        
        '<remarks/>
        Public DocumentAgentFlags As DocumentAgentFlags
    End Class
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://foley.com/EDDS/DocumentManager")>  _
    Public Class Field
        Inherits Artifact
        
        '<remarks/>
        Public ArtifactViewFieldID As Integer
        
        '<remarks/>
        Public DisplayName As String
        
        '<remarks/>
        Public IsRequired As Boolean
        
        '<remarks/>
        Public Removable As Boolean
        
        '<remarks/>
        Public CodeArtifactTypeID As NullableInt32
        
        '<remarks/>
        Public MaxLength As NullableInt32
        
        '<remarks/>
        Public FieldTypeID As Integer
        
        '<remarks/>
        Public IsEditable As Boolean
        
        '<remarks/>
        Public Visible As Boolean
        
        '<remarks/>
        Public FieldCategoryID As Integer
        
        '<remarks/>
        Public AddToFullText As Boolean
        
        '<remarks/>
        Public AddToConceptualText As Boolean
        
        '<remarks/>
        Public IsArtifactBaseField As Boolean
        
        '<remarks/>
        Public Value As Object
        
        '<remarks/>
        Public FieldType As FieldType
        
        '<remarks/>
        Public ColumnName As String
        
        '<remarks/>
        Public IsReadOnlyInLayout As Boolean
        
        '<remarks/>
        Public FilterType As String
        
        '<remarks/>
        Public FieldDisplayTypeID As Integer
        
        '<remarks/>
        Public IsLinked As Boolean
        
        '<remarks/>
        Public FormatString As String
        
        '<remarks/>
        Public RepeatColumn As NullableInt32
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
    End Enum
End Namespace
