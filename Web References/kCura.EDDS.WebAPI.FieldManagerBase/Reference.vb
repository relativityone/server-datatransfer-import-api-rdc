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
Namespace kCura.EDDS.WebAPI.FieldManagerBase
    
    '<remarks/>
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Web.Services.WebServiceBindingAttribute(Name:="FieldManagerSoap", [Namespace]:="http://foley.com/EDDS/FieldManager"),  _
     System.Xml.Serialization.XmlIncludeAttribute(GetType(Artifact))>  _
    Public Class FieldManager
        Inherits System.Web.Services.Protocols.SoapHttpClientProtocol
        
        '<remarks/>
        Public Sub New()
            MyBase.New
            Me.Url = "http://localhost/EDDSWebAPI/FieldManager.asmx"
        End Sub
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://foley.com/EDDS/FieldManager/Create", RequestNamespace:="http://foley.com/EDDS/FieldManager", ResponseNamespace:="http://foley.com/EDDS/FieldManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function Create(ByVal caseContextArtifactID As Integer, ByVal fieldDTO As Field) As Integer
            Dim results() As Object = Me.Invoke("Create", New Object() {caseContextArtifactID, fieldDTO})
            Return CType(results(0),Integer)
        End Function
        
        '<remarks/>
        Public Function BeginCreate(ByVal caseContextArtifactID As Integer, ByVal fieldDTO As Field, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("Create", New Object() {caseContextArtifactID, fieldDTO}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndCreate(ByVal asyncResult As System.IAsyncResult) As Integer
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),Integer)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://foley.com/EDDS/FieldManager/Read", RequestNamespace:="http://foley.com/EDDS/FieldManager", ResponseNamespace:="http://foley.com/EDDS/FieldManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function Read(ByVal caseContextArtifactID As Integer, ByVal fieldArtifactID As Integer) As Field
            Dim results() As Object = Me.Invoke("Read", New Object() {caseContextArtifactID, fieldArtifactID})
            Return CType(results(0),Field)
        End Function
        
        '<remarks/>
        Public Function BeginRead(ByVal caseContextArtifactID As Integer, ByVal fieldArtifactID As Integer, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("Read", New Object() {caseContextArtifactID, fieldArtifactID}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndRead(ByVal asyncResult As System.IAsyncResult) As Field
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),Field)
        End Function
    End Class
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://foley.com/EDDS/FieldManager")>  _
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
        Public IsGroupByEnabled As Boolean
        
        '<remarks/>
        Public IsIndexEnabled As Boolean
        
        '<remarks/>
        Public IsConceptualEnabled As Boolean
        
        '<remarks/>
        Public DisplayValueTrue As String
        
        '<remarks/>
        Public DisplayValueFalse As String
        
        '<remarks/>
        Public Width As String
        
        '<remarks/>
        Public Wrapping As Boolean
        
        '<remarks/>
        Public PropogateValueToAllDuplicates As Boolean
        
        '<remarks/>
        Public PropogateValueToEntireGroup As Boolean
    End Class
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://foley.com/EDDS/FieldManager")>  _
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
    End Enum
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://foley.com/EDDS/FieldManager")>  _
    Public Enum FieldCategory
        
        '<remarks/>
        Generic
        
        '<remarks/>
        FullText
        
        '<remarks/>
        Identifier
        
        '<remarks/>
        Associative
        
        '<remarks/>
        Comments
        
        '<remarks/>
        GroupIdentifier
        
        '<remarks/>
        ProductionMarker
        
        '<remarks/>
        AutoCreate
        
        '<remarks/>
        DuplicateHash
        
        '<remarks/>
        FolderName
        
        '<remarks/>
        FileInfo
    End Enum
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://foley.com/EDDS/FieldManager"),  _
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
End Namespace
