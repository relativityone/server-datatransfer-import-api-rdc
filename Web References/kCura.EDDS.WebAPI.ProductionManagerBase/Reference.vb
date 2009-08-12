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
Namespace kCura.EDDS.WebAPI.ProductionManagerBase
    
    '<remarks/>
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Web.Services.WebServiceBindingAttribute(Name:="ProductionManagerSoap", [Namespace]:="http://www.kCura.com/EDDS/ProductionManager"),  _
     System.Xml.Serialization.XmlIncludeAttribute(GetType(ProductionOrder)),  _
     System.Xml.Serialization.XmlIncludeAttribute(GetType(Artifact)),  _
     System.Xml.Serialization.XmlIncludeAttribute(GetType(System.Object()))>  _
    Public Class ProductionManager
        Inherits System.Web.Services.Protocols.SoapHttpClientProtocol
        
        '<remarks/>
        Public Sub New()
            MyBase.New
            Me.Url = "http://localhost/EDDSWebAPI/ProductionManager.asmx"
        End Sub
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/ProductionManager/RetrieveProducedByContextArtifactID", RequestNamespace:="http://www.kCura.com/EDDS/ProductionManager", ResponseNamespace:="http://www.kCura.com/EDDS/ProductionManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function RetrieveProducedByContextArtifactID(ByVal caseContextArtifactID As Integer) As System.Data.DataSet
            Dim results() As Object = Me.Invoke("RetrieveProducedByContextArtifactID", New Object() {caseContextArtifactID})
            Return CType(results(0),System.Data.DataSet)
        End Function
        
        '<remarks/>
        Public Function BeginRetrieveProducedByContextArtifactID(ByVal caseContextArtifactID As Integer, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("RetrieveProducedByContextArtifactID", New Object() {caseContextArtifactID}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndRetrieveProducedByContextArtifactID(ByVal asyncResult As System.IAsyncResult) As System.Data.DataSet
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),System.Data.DataSet)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/ProductionManager/RetrieveImportEligibleByContextArtifa"& _ 
"ctID", RequestNamespace:="http://www.kCura.com/EDDS/ProductionManager", ResponseNamespace:="http://www.kCura.com/EDDS/ProductionManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function RetrieveImportEligibleByContextArtifactID(ByVal caseContextArtifactID As Integer) As System.Data.DataSet
            Dim results() As Object = Me.Invoke("RetrieveImportEligibleByContextArtifactID", New Object() {caseContextArtifactID})
            Return CType(results(0),System.Data.DataSet)
        End Function
        
        '<remarks/>
        Public Function BeginRetrieveImportEligibleByContextArtifactID(ByVal caseContextArtifactID As Integer, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("RetrieveImportEligibleByContextArtifactID", New Object() {caseContextArtifactID}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndRetrieveImportEligibleByContextArtifactID(ByVal asyncResult As System.IAsyncResult) As System.Data.DataSet
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),System.Data.DataSet)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/ProductionManager/DoPostImportProcessing", RequestNamespace:="http://www.kCura.com/EDDS/ProductionManager", ResponseNamespace:="http://www.kCura.com/EDDS/ProductionManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Sub DoPostImportProcessing(ByVal caseContextArtifactID As Integer, ByVal productionArtifactID As Integer)
            Me.Invoke("DoPostImportProcessing", New Object() {caseContextArtifactID, productionArtifactID})
        End Sub
        
        '<remarks/>
        Public Function BeginDoPostImportProcessing(ByVal caseContextArtifactID As Integer, ByVal productionArtifactID As Integer, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("DoPostImportProcessing", New Object() {caseContextArtifactID, productionArtifactID}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Sub EndDoPostImportProcessing(ByVal asyncResult As System.IAsyncResult)
            Me.EndInvoke(asyncResult)
        End Sub
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/ProductionManager/DoPreImportProcessing", RequestNamespace:="http://www.kCura.com/EDDS/ProductionManager", ResponseNamespace:="http://www.kCura.com/EDDS/ProductionManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Sub DoPreImportProcessing(ByVal caseContextArtifactID As Integer, ByVal productionArtifactID As Integer)
            Me.Invoke("DoPreImportProcessing", New Object() {caseContextArtifactID, productionArtifactID})
        End Sub
        
        '<remarks/>
        Public Function BeginDoPreImportProcessing(ByVal caseContextArtifactID As Integer, ByVal productionArtifactID As Integer, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("DoPreImportProcessing", New Object() {caseContextArtifactID, productionArtifactID}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Sub EndDoPreImportProcessing(ByVal asyncResult As System.IAsyncResult)
            Me.EndInvoke(asyncResult)
        End Sub
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/ProductionManager/Read", RequestNamespace:="http://www.kCura.com/EDDS/ProductionManager", ResponseNamespace:="http://www.kCura.com/EDDS/ProductionManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function Read(ByVal caseContextArtifactID As Integer, ByVal productionArtifactID As Integer) As Production
            Dim results() As Object = Me.Invoke("Read", New Object() {caseContextArtifactID, productionArtifactID})
            Return CType(results(0),Production)
        End Function
        
        '<remarks/>
        Public Function BeginRead(ByVal caseContextArtifactID As Integer, ByVal productionArtifactID As Integer, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("Read", New Object() {caseContextArtifactID, productionArtifactID}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndRead(ByVal asyncResult As System.IAsyncResult) As Production
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),Production)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/ProductionManager/RetrieveProducedWithSecurity", RequestNamespace:="http://www.kCura.com/EDDS/ProductionManager", ResponseNamespace:="http://www.kCura.com/EDDS/ProductionManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function RetrieveProducedWithSecurity(ByVal caseContextArtifactID As Integer) As System.Data.DataSet
            Dim results() As Object = Me.Invoke("RetrieveProducedWithSecurity", New Object() {caseContextArtifactID})
            Return CType(results(0),System.Data.DataSet)
        End Function
        
        '<remarks/>
        Public Function BeginRetrieveProducedWithSecurity(ByVal caseContextArtifactID As Integer, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("RetrieveProducedWithSecurity", New Object() {caseContextArtifactID}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndRetrieveProducedWithSecurity(ByVal asyncResult As System.IAsyncResult) As System.Data.DataSet
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),System.Data.DataSet)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/ProductionManager/AddDocumentToProduction", RequestNamespace:="http://www.kCura.com/EDDS/ProductionManager", ResponseNamespace:="http://www.kCura.com/EDDS/ProductionManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function AddDocumentToProduction(ByVal caseContextArtifactID As Integer, ByVal productionArtifactID As Integer, ByVal documentArtifactID As Integer) As Boolean
            Dim results() As Object = Me.Invoke("AddDocumentToProduction", New Object() {caseContextArtifactID, productionArtifactID, documentArtifactID})
            Return CType(results(0),Boolean)
        End Function
        
        '<remarks/>
        Public Function BeginAddDocumentToProduction(ByVal caseContextArtifactID As Integer, ByVal productionArtifactID As Integer, ByVal documentArtifactID As Integer, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("AddDocumentToProduction", New Object() {caseContextArtifactID, productionArtifactID, documentArtifactID}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndAddDocumentToProduction(ByVal asyncResult As System.IAsyncResult) As Boolean
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),Boolean)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/ProductionManager/CreateProductionDocumentFiles", RequestNamespace:="http://www.kCura.com/EDDS/ProductionManager", ResponseNamespace:="http://www.kCura.com/EDDS/ProductionManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Sub CreateProductionDocumentFiles(ByVal caseContextArtifactID As Integer, ByVal productionDocumentFiles() As ProductionDocumentFileInfoBase, ByVal productionArtifactID As Integer, ByVal documentArtifactID As Integer)
            Me.Invoke("CreateProductionDocumentFiles", New Object() {caseContextArtifactID, productionDocumentFiles, productionArtifactID, documentArtifactID})
        End Sub
        
        '<remarks/>
        Public Function BeginCreateProductionDocumentFiles(ByVal caseContextArtifactID As Integer, ByVal productionDocumentFiles() As ProductionDocumentFileInfoBase, ByVal productionArtifactID As Integer, ByVal documentArtifactID As Integer, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("CreateProductionDocumentFiles", New Object() {caseContextArtifactID, productionDocumentFiles, productionArtifactID, documentArtifactID}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Sub EndCreateProductionDocumentFiles(ByVal asyncResult As System.IAsyncResult)
            Me.EndInvoke(asyncResult)
        End Sub
    End Class
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.kCura.com/EDDS/ProductionManager")>  _
    Public Class Production
        Inherits Artifact
        
        '<remarks/>
        Public Name As String
        
        '<remarks/>
        Public BeginBatesFieldArtifactID As Integer
        
        '<remarks/>
        Public EndBatesFieldArtifactID As Integer
        
        '<remarks/>
        Public ImageShrinkPercent As Integer
        
        '<remarks/>
        Public DateProduced As NullableDateTime
        
        '<remarks/>
        Public StatusCodeArtifactID As NullableInt32
        
        '<remarks/>
        Public BatesPrefix As String
        
        '<remarks/>
        Public BatesSuffix As String
        
        '<remarks/>
        Public BatesFormat As Integer
        
        '<remarks/>
        Public BatesStartNumber As Integer
        
        '<remarks/>
        Public LeftHeaderTypeCodeArtifactID As NullableInt32
        
        '<remarks/>
        Public LeftHeaderFieldArtifactID As NullableInt32
        
        '<remarks/>
        Public LeftHeaderFreeText As String
        
        '<remarks/>
        Public CenterHeaderTypeCodeArtifactID As NullableInt32
        
        '<remarks/>
        Public CenterHeaderFieldArtifactID As NullableInt32
        
        '<remarks/>
        Public CenterHeaderFreeText As String
        
        '<remarks/>
        Public RightHeaderTypeCodeArtifactID As NullableInt32
        
        '<remarks/>
        Public RightHeaderFieldArtifactID As NullableInt32
        
        '<remarks/>
        Public RightHeaderFreeText As String
        
        '<remarks/>
        Public LeftFooterTypeCodeArtifactID As NullableInt32
        
        '<remarks/>
        Public LeftFooterFieldArtifactID As NullableInt32
        
        '<remarks/>
        Public LeftFooterFreeText As String
        
        '<remarks/>
        Public CenterFooterTypeCodeArtifactID As NullableInt32
        
        '<remarks/>
        Public CenterFooterFieldArtifactID As NullableInt32
        
        '<remarks/>
        Public CenterFooterFreeText As String
        
        '<remarks/>
        Public RightFooterTypeCodeArtifactID As NullableInt32
        
        '<remarks/>
        Public RightFooterFieldArtifactID As NullableInt32
        
        '<remarks/>
        Public RightFooterFreeText As String
        
        '<remarks/>
        Public BurnAnnotations As Boolean
        
        '<remarks/>
        Public FontSize As Integer
        
        '<remarks/>
        Public ErrorFlagFieldArtifactID As Integer
        
        '<remarks/>
        Public ProductionOrder() As Object
        
        '<remarks/>
        Public MarkupSet As Integer
        
        '<remarks/>
        Public Imported As Boolean
        
        '<remarks/>
        Public AddImagePlaceholder As Boolean
        
        '<remarks/>
        Public ProductionFieldArtifactID As Integer
        
        '<remarks/>
        Public DocumentsHaveRedactions As Boolean
    End Class
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.kCura.com/EDDS/ProductionManager")>  _
    Public Class ProductionDocumentFileInfoBase
        
        '<remarks/>
        Public SourceGuid As String
        
        '<remarks/>
        Public ImageGuid As String
        
        '<remarks/>
        Public BatesNumber As String
        
        '<remarks/>
        Public ImageSize As Long
    End Class
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.kCura.com/EDDS/ProductionManager")>  _
    Public Class ProductionOrder
        
        '<remarks/>
        Public ProductionOrderID As Integer
        
        '<remarks/>
        Public ProductionID As Integer
        
        '<remarks/>
        Public ArtifactViewFieldID As Integer
        
        '<remarks/>
        Public Order As Integer
        
        '<remarks/>
        Public Direction As String
        
        '<remarks/>
        Public ColumnName As String
    End Class
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.kCura.com/EDDS/ProductionManager"),  _
     System.Xml.Serialization.XmlIncludeAttribute(GetType(Production))>  _
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
