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
Namespace kCura.EDDS.WebAPI.SearchManagerBase
    
    '<remarks/>
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Web.Services.WebServiceBindingAttribute(Name:="SearchManagerSoap", [Namespace]:="http://www.kCura.com/EDDS/SearchManager")>  _
    Public Class SearchManager
        Inherits System.Web.Services.Protocols.SoapHttpClientProtocol
        
        '<remarks/>
        Public Sub New()
            MyBase.New
            Me.Url = "http://localhost/EDDSWebAPI/SearchManager.asmx"
        End Sub
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/SearchManager/CountSearchByArtifactID", RequestNamespace:="http://www.kCura.com/EDDS/SearchManager", ResponseNamespace:="http://www.kCura.com/EDDS/SearchManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function CountSearchByArtifactID(ByVal searchArtifactID As Integer) As Integer
            Dim results() As Object = Me.Invoke("CountSearchByArtifactID", New Object() {searchArtifactID})
            Return CType(results(0),Integer)
        End Function
        
        '<remarks/>
        Public Function BeginCountSearchByArtifactID(ByVal searchArtifactID As Integer, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("CountSearchByArtifactID", New Object() {searchArtifactID}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndCountSearchByArtifactID(ByVal asyncResult As System.IAsyncResult) As Integer
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),Integer)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/SearchManager/SearchBySearchArtifactID", RequestNamespace:="http://www.kCura.com/EDDS/SearchManager", ResponseNamespace:="http://www.kCura.com/EDDS/SearchManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function SearchBySearchArtifactID(ByVal searchArtifactID As Integer, ByVal start As Integer, ByVal finish As Integer) As System.Data.DataSet
            Dim results() As Object = Me.Invoke("SearchBySearchArtifactID", New Object() {searchArtifactID, start, finish})
            Return CType(results(0),System.Data.DataSet)
        End Function
        
        '<remarks/>
        Public Function BeginSearchBySearchArtifactID(ByVal searchArtifactID As Integer, ByVal start As Integer, ByVal finish As Integer, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("SearchBySearchArtifactID", New Object() {searchArtifactID, start, finish}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndSearchBySearchArtifactID(ByVal asyncResult As System.IAsyncResult) As System.Data.DataSet
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),System.Data.DataSet)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/SearchManager/RetrieveNativesForSearch", RequestNamespace:="http://www.kCura.com/EDDS/SearchManager", ResponseNamespace:="http://www.kCura.com/EDDS/SearchManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function RetrieveNativesForSearch(ByVal artifactID As Integer, ByVal documentArtifactIDs As String) As System.Data.DataSet
            Dim results() As Object = Me.Invoke("RetrieveNativesForSearch", New Object() {artifactID, documentArtifactIDs})
            Return CType(results(0),System.Data.DataSet)
        End Function
        
        '<remarks/>
        Public Function BeginRetrieveNativesForSearch(ByVal artifactID As Integer, ByVal documentArtifactIDs As String, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("RetrieveNativesForSearch", New Object() {artifactID, documentArtifactIDs}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndRetrieveNativesForSearch(ByVal asyncResult As System.IAsyncResult) As System.Data.DataSet
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),System.Data.DataSet)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/SearchManager/RetrieveFullTextFilesForSearch", RequestNamespace:="http://www.kCura.com/EDDS/SearchManager", ResponseNamespace:="http://www.kCura.com/EDDS/SearchManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function RetrieveFullTextFilesForSearch(ByVal artifactID As Integer, ByVal documentArtifactIDs As String) As System.Data.DataSet
            Dim results() As Object = Me.Invoke("RetrieveFullTextFilesForSearch", New Object() {artifactID, documentArtifactIDs})
            Return CType(results(0),System.Data.DataSet)
        End Function
        
        '<remarks/>
        Public Function BeginRetrieveFullTextFilesForSearch(ByVal artifactID As Integer, ByVal documentArtifactIDs As String, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("RetrieveFullTextFilesForSearch", New Object() {artifactID, documentArtifactIDs}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndRetrieveFullTextFilesForSearch(ByVal asyncResult As System.IAsyncResult) As System.Data.DataSet
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),System.Data.DataSet)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/SearchManager/RetrieveViewsByContextArtifactID", RequestNamespace:="http://www.kCura.com/EDDS/SearchManager", ResponseNamespace:="http://www.kCura.com/EDDS/SearchManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function RetrieveViewsByContextArtifactID(ByVal contextArtifactID As Integer, ByVal isSearch As Boolean) As System.Data.DataSet
            Dim results() As Object = Me.Invoke("RetrieveViewsByContextArtifactID", New Object() {contextArtifactID, isSearch})
            Return CType(results(0),System.Data.DataSet)
        End Function
        
        '<remarks/>
        Public Function BeginRetrieveViewsByContextArtifactID(ByVal contextArtifactID As Integer, ByVal isSearch As Boolean, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("RetrieveViewsByContextArtifactID", New Object() {contextArtifactID, isSearch}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndRetrieveViewsByContextArtifactID(ByVal asyncResult As System.IAsyncResult) As System.Data.DataSet
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),System.Data.DataSet)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/SearchManager/RetrieveSearchFields", RequestNamespace:="http://www.kCura.com/EDDS/SearchManager", ResponseNamespace:="http://www.kCura.com/EDDS/SearchManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function RetrieveSearchFields(ByVal viewArtifactID As Integer) As System.Data.DataSet
            Dim results() As Object = Me.Invoke("RetrieveSearchFields", New Object() {viewArtifactID})
            Return CType(results(0),System.Data.DataSet)
        End Function
        
        '<remarks/>
        Public Function BeginRetrieveSearchFields(ByVal viewArtifactID As Integer, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("RetrieveSearchFields", New Object() {viewArtifactID}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndRetrieveSearchFields(ByVal asyncResult As System.IAsyncResult) As System.Data.DataSet
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),System.Data.DataSet)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/SearchManager/CountSearchByParentArtifactID", RequestNamespace:="http://www.kCura.com/EDDS/SearchManager", ResponseNamespace:="http://www.kCura.com/EDDS/SearchManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function CountSearchByParentArtifactID(ByVal parentArtifactID As Integer, ByVal searchSubFolders As Boolean) As Integer
            Dim results() As Object = Me.Invoke("CountSearchByParentArtifactID", New Object() {parentArtifactID, searchSubFolders})
            Return CType(results(0),Integer)
        End Function
        
        '<remarks/>
        Public Function BeginCountSearchByParentArtifactID(ByVal parentArtifactID As Integer, ByVal searchSubFolders As Boolean, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("CountSearchByParentArtifactID", New Object() {parentArtifactID, searchSubFolders}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndCountSearchByParentArtifactID(ByVal asyncResult As System.IAsyncResult) As Integer
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),Integer)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/SearchManager/SearchByParentArtifactID", RequestNamespace:="http://www.kCura.com/EDDS/SearchManager", ResponseNamespace:="http://www.kCura.com/EDDS/SearchManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function SearchByParentArtifactID(ByVal parentArtifactID As Integer, ByVal searchSubFolders As Boolean, ByVal start As Integer, ByVal finish As Integer) As System.Data.DataSet
            Dim results() As Object = Me.Invoke("SearchByParentArtifactID", New Object() {parentArtifactID, searchSubFolders, start, finish})
            Return CType(results(0),System.Data.DataSet)
        End Function
        
        '<remarks/>
        Public Function BeginSearchByParentArtifactID(ByVal parentArtifactID As Integer, ByVal searchSubFolders As Boolean, ByVal start As Integer, ByVal finish As Integer, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("SearchByParentArtifactID", New Object() {parentArtifactID, searchSubFolders, start, finish}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndSearchByParentArtifactID(ByVal asyncResult As System.IAsyncResult) As System.Data.DataSet
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),System.Data.DataSet)
        End Function
    End Class
End Namespace
