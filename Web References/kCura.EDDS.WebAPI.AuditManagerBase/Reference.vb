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
Namespace kCura.EDDS.WebAPI.AuditManagerBase
    
    '<remarks/>
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Web.Services.WebServiceBindingAttribute(Name:="AuditManagerSoap", [Namespace]:="http://www.kCura.com/EDDS/AuditManager"),  _
     System.Xml.Serialization.XmlIncludeAttribute(GetType(ImportStatistics))>  _
    Public Class AuditManager
        Inherits System.Web.Services.Protocols.SoapHttpClientProtocol
        
        '<remarks/>
        Public Sub New()
            MyBase.New
            Me.Url = "http://localhost/EDDSWebAPI/AuditManager.asmx"
        End Sub
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/AuditManager/CreateAuditRecord", RequestNamespace:="http://www.kCura.com/EDDS/AuditManager", ResponseNamespace:="http://www.kCura.com/EDDS/AuditManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function CreateAuditRecord(ByVal caseContextArtifactID As Integer, ByVal artifactID As Integer, ByVal action As Integer, ByVal details As String, ByVal origination As String) As Boolean
            Dim results() As Object = Me.Invoke("CreateAuditRecord", New Object() {caseContextArtifactID, artifactID, action, details, origination})
            Return CType(results(0),Boolean)
        End Function
        
        '<remarks/>
        Public Function BeginCreateAuditRecord(ByVal caseContextArtifactID As Integer, ByVal artifactID As Integer, ByVal action As Integer, ByVal details As String, ByVal origination As String, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("CreateAuditRecord", New Object() {caseContextArtifactID, artifactID, action, details, origination}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndCreateAuditRecord(ByVal asyncResult As System.IAsyncResult) As Boolean
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),Boolean)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/AuditManager/AuditImageImport", RequestNamespace:="http://www.kCura.com/EDDS/AuditManager", ResponseNamespace:="http://www.kCura.com/EDDS/AuditManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function AuditImageImport(ByVal appID As Integer, ByVal runId As String, ByVal isFatalError As Boolean, ByVal importStats As ImageImportStatistics) As Boolean
            Dim results() As Object = Me.Invoke("AuditImageImport", New Object() {appID, runId, isFatalError, importStats})
            Return CType(results(0),Boolean)
        End Function
        
        '<remarks/>
        Public Function BeginAuditImageImport(ByVal appID As Integer, ByVal runId As String, ByVal isFatalError As Boolean, ByVal importStats As ImageImportStatistics, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("AuditImageImport", New Object() {appID, runId, isFatalError, importStats}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndAuditImageImport(ByVal asyncResult As System.IAsyncResult) As Boolean
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),Boolean)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/AuditManager/AuditObjectImport", RequestNamespace:="http://www.kCura.com/EDDS/AuditManager", ResponseNamespace:="http://www.kCura.com/EDDS/AuditManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function AuditObjectImport(ByVal appID As Integer, ByVal runId As String, ByVal isFatalError As Boolean, ByVal importStats As ObjectImportStatistics) As Boolean
            Dim results() As Object = Me.Invoke("AuditObjectImport", New Object() {appID, runId, isFatalError, importStats})
            Return CType(results(0),Boolean)
        End Function
        
        '<remarks/>
        Public Function BeginAuditObjectImport(ByVal appID As Integer, ByVal runId As String, ByVal isFatalError As Boolean, ByVal importStats As ObjectImportStatistics, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("AuditObjectImport", New Object() {appID, runId, isFatalError, importStats}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndAuditObjectImport(ByVal asyncResult As System.IAsyncResult) As Boolean
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),Boolean)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/AuditManager/AuditExport", RequestNamespace:="http://www.kCura.com/EDDS/AuditManager", ResponseNamespace:="http://www.kCura.com/EDDS/AuditManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function AuditExport(ByVal appID As Integer, ByVal isFatalError As Boolean, ByVal exportStats As ExportStatistics) As Boolean
            Dim results() As Object = Me.Invoke("AuditExport", New Object() {appID, isFatalError, exportStats})
            Return CType(results(0),Boolean)
        End Function
        
        '<remarks/>
        Public Function BeginAuditExport(ByVal appID As Integer, ByVal isFatalError As Boolean, ByVal exportStats As ExportStatistics, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("AuditExport", New Object() {appID, isFatalError, exportStats}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndAuditExport(ByVal asyncResult As System.IAsyncResult) As Boolean
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),Boolean)
        End Function
    End Class
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.kCura.com/EDDS/AuditManager")>  _
    Public Class ImageImportStatistics
        Inherits ImportStatistics
        
        '<remarks/>
        Public ExtractedTextReplaced As Boolean
        
        '<remarks/>
        Public SupportImageAutoNumbering As Boolean
        
        '<remarks/>
        Public DestinationProductionArtifactID As Integer
    End Class
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.kCura.com/EDDS/AuditManager"),  _
     System.Xml.Serialization.XmlIncludeAttribute(GetType(ObjectImportStatistics)),  _
     System.Xml.Serialization.XmlIncludeAttribute(GetType(ImageImportStatistics))>  _
    Public MustInherit Class ImportStatistics
        
        '<remarks/>
        Public RepositoryConnection As RepositoryConnectionType
        
        '<remarks/>
        Public Overwrite As OverwriteType
        
        '<remarks/>
        Public OverlayIdentifierFieldArtifactID As Integer
        
        '<remarks/>
        Public DestinationFolderArtifactID As Integer
        
        '<remarks/>
        Public LoadFileName As String
        
        '<remarks/>
        Public StartLine As Integer
        
        '<remarks/>
        Public FilesCopiedToRepository As String
        
        '<remarks/>
        Public TotalFileSize As Long
        
        '<remarks/>
        Public TotalMetadataBytes As Long
        
        '<remarks/>
        Public NumberOfDocumentsCreated As Integer
        
        '<remarks/>
        Public NumberOfDocumentsUpdated As Integer
        
        '<remarks/>
        Public NumberOfFilesLoaded As Integer
        
        '<remarks/>
        Public NumberOfErrors As Long
        
        '<remarks/>
        Public NumberOfWarnings As Long
        
        '<remarks/>
        Public RunTimeInMilliseconds As Integer
        
        '<remarks/>
        Public SendNotification As Boolean
    End Class
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.kCura.com/EDDS/AuditManager")>  _
    Public Enum RepositoryConnectionType
        
        '<remarks/>
        Web
        
        '<remarks/>
        Direct
    End Enum
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.kCura.com/EDDS/AuditManager")>  _
    Public Enum OverwriteType
        
        '<remarks/>
        Append
        
        '<remarks/>
        Overlay
        
        '<remarks/>
        Both
    End Enum
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.kCura.com/EDDS/AuditManager")>  _
    Public Class ExportStatistics
        
        '<remarks/>
        Public Type As String
        
        '<remarks/>
        Public Fields() As Integer
        
        '<remarks/>
        Public DestinationFilesystemFolder As String
        
        '<remarks/>
        Public OverwriteFiles As Boolean
        
        '<remarks/>
        Public VolumePrefix As String
        
        '<remarks/>
        Public VolumeMaxSize As Long
        
        '<remarks/>
        Public SubdirectoryImagePrefix As String
        
        '<remarks/>
        Public SubdirectoryNativePrefix As String
        
        '<remarks/>
        Public SubdirectoryTextPrefix As String
        
        '<remarks/>
        Public SubdirectoryStartNumber As Long
        
        '<remarks/>
        Public SubdirectoryMaxFileCount As Long
        
        '<remarks/>
        Public FilePathSettings As String
        
        '<remarks/>
        Public Delimiter As Char
        
        '<remarks/>
        Public Bound As Char
        
        '<remarks/>
        Public NewlineProxy As Char
        
        '<remarks/>
        Public MultiValueDelimiter As Char
        
        '<remarks/>
        Public NestedValueDelimiter As Char
        
        '<remarks/>
        Public TextAndNativeFilesNamedAfterFieldID As Integer
        
        '<remarks/>
        Public AppendOriginalFilenames As Boolean
        
        '<remarks/>
        Public ExportImages As Boolean
        
        '<remarks/>
        Public ImageLoadFileFormat As ImageLoadFileFormatType
        
        '<remarks/>
        Public ImageFileType As ImageFileExportType
        
        '<remarks/>
        Public ExportNativeFiles As Boolean
        
        '<remarks/>
        Public MetadataLoadFileFormat As LoadFileFormat
        
        '<remarks/>
        Public MetadataLoadFileEncodingCodePage As Integer
        
        '<remarks/>
        Public ExportTextFieldAsFiles As Boolean
        
        '<remarks/>
        Public ExportedTextFileEncodingCodePage As Integer
        
        '<remarks/>
        Public ExportedTextFieldID As Integer
        
        '<remarks/>
        Public ExportMultipleChoiceFieldsAsNested As Boolean
        
        '<remarks/>
        Public TotalFileBytesExported As Long
        
        '<remarks/>
        Public TotalMetadataBytesExported As Long
        
        '<remarks/>
        Public ErrorCount As Integer
        
        '<remarks/>
        Public WarningCount As Integer
        
        '<remarks/>
        Public DocumentExportCount As Integer
        
        '<remarks/>
        Public FileExportCount As Integer
        
        '<remarks/>
        Public ImagesToExport As ImagesToExportType
        
        '<remarks/>
        Public ProductionPrecedence() As Integer
        
        '<remarks/>
        Public DataSourceArtifactID As Integer
        
        '<remarks/>
        Public SourceRootFolderID As Integer
        
        '<remarks/>
        Public RunTimeInMilliseconds As Integer
        
        '<remarks/>
        Public CopyFilesFromRepository As Boolean
        
        '<remarks/>
        Public StartExportAtDocumentNumber As Integer
        
        '<remarks/>
        Public VolumeStartNumber As Integer
        
        '<remarks/>
        Public ArtifactTypeID As Integer
    End Class
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.kCura.com/EDDS/AuditManager")>  _
    Public Enum ImageLoadFileFormatType
        
        '<remarks/>
        Opticon
        
        '<remarks/>
        Ipro
        
        '<remarks/>
        IproFullText
    End Enum
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.kCura.com/EDDS/AuditManager")>  _
    Public Enum ImageFileExportType
        
        '<remarks/>
        SinglePage
        
        '<remarks/>
        MultiPageTiff
        
        '<remarks/>
        PDF
    End Enum
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.kCura.com/EDDS/AuditManager")>  _
    Public Enum LoadFileFormat
        
        '<remarks/>
        Csv
        
        '<remarks/>
        Dat
        
        '<remarks/>
        Custom
        
        '<remarks/>
        Html
    End Enum
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.kCura.com/EDDS/AuditManager")>  _
    Public Enum ImagesToExportType
        
        '<remarks/>
        Original
        
        '<remarks/>
        Produced
        
        '<remarks/>
        Both
    End Enum
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.kCura.com/EDDS/AuditManager")>  _
    Public Class ObjectImportStatistics
        Inherits ImportStatistics
        
        '<remarks/>
        Public ArtifactTypeID As Integer
        
        '<remarks/>
        Public Delimiter As Char
        
        '<remarks/>
        Public Bound As Char
        
        '<remarks/>
        Public NewlineProxy As Char
        
        '<remarks/>
        Public MultiValueDelimiter As Char
        
        '<remarks/>
        Public LoadFileEncodingCodePageID As Integer
        
        '<remarks/>
        Public ExtractedTextFileEncodingCodePageID As Integer
        
        '<remarks/>
        Public FolderColumnName As String
        
        '<remarks/>
        Public FileFieldColumnName As String
        
        '<remarks/>
        Public ExtractedTextPointsToFile As Boolean
        
        '<remarks/>
        Public NumberOfChoicesCreated As Integer
        
        '<remarks/>
        Public NumberOfFoldersCreated As Integer
        
        '<remarks/>
        <System.Xml.Serialization.XmlArrayItemAttribute(IsNullable:=false, NestingLevel:=1)>  _
        Public FieldsMapped()() As Integer
        
        '<remarks/>
        Public NestedValueDelimiter As Char
    End Class
End Namespace
