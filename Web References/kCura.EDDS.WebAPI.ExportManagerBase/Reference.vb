﻿'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:4.0.30319.1
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
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
'This source code was auto-generated by Microsoft.VSDesigner, Version 4.0.30319.1.
'
Namespace kCura.EDDS.WebAPI.ExportManagerBase
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1"),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Web.Services.WebServiceBindingAttribute(Name:="ExportManagerSoap", [Namespace]:="http://www.kCura.com/EDDS/ExportManager"),  _
     System.Xml.Serialization.XmlIncludeAttribute(GetType(Object()))>  _
    Partial Public Class ExportManager
        Inherits System.Web.Services.Protocols.SoapHttpClientProtocol
        
        Private InitializeSearchExportOperationCompleted As System.Threading.SendOrPostCallback
        
        Private InitializeFolderExportOperationCompleted As System.Threading.SendOrPostCallback
        
        Private InitializeProductionExportOperationCompleted As System.Threading.SendOrPostCallback
        
        Private RetrieveResultsBlockOperationCompleted As System.Threading.SendOrPostCallback
        
        Private useDefaultCredentialsSetExplicitly As Boolean
        
        '''<remarks/>
        Public Sub New()
            MyBase.New
            Me.Url = Global.My.MySettings.Default.kCura_WinEDDS_kCura_EDDS_WebAPI_ExportManagerBase_ExportManager
            If (Me.IsLocalFileSystemWebService(Me.Url) = true) Then
                Me.UseDefaultCredentials = true
                Me.useDefaultCredentialsSetExplicitly = false
            Else
                Me.useDefaultCredentialsSetExplicitly = true
            End If
        End Sub
        
        Public Shadows Property Url() As String
            Get
                Return MyBase.Url
            End Get
            Set
                If (((Me.IsLocalFileSystemWebService(MyBase.Url) = true)  _
                            AndAlso (Me.useDefaultCredentialsSetExplicitly = false))  _
                            AndAlso (Me.IsLocalFileSystemWebService(value) = false)) Then
                    MyBase.UseDefaultCredentials = false
                End If
                MyBase.Url = value
            End Set
        End Property
        
        Public Shadows Property UseDefaultCredentials() As Boolean
            Get
                Return MyBase.UseDefaultCredentials
            End Get
            Set
                MyBase.UseDefaultCredentials = value
                Me.useDefaultCredentialsSetExplicitly = true
            End Set
        End Property
        
        '''<remarks/>
        Public Event InitializeSearchExportCompleted As InitializeSearchExportCompletedEventHandler
        
        '''<remarks/>
        Public Event InitializeFolderExportCompleted As InitializeFolderExportCompletedEventHandler
        
        '''<remarks/>
        Public Event InitializeProductionExportCompleted As InitializeProductionExportCompletedEventHandler
        
        '''<remarks/>
        Public Event RetrieveResultsBlockCompleted As RetrieveResultsBlockCompletedEventHandler
        
        '''<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/ExportManager/InitializeSearchExport", RequestNamespace:="http://www.kCura.com/EDDS/ExportManager", ResponseNamespace:="http://www.kCura.com/EDDS/ExportManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function InitializeSearchExport(ByVal appID As Integer, ByVal searchArtifactID As Integer, ByVal avfIds() As Integer, ByVal startAtRecord As Integer) As InitializationResults
            Dim results() As Object = Me.Invoke("InitializeSearchExport", New Object() {appID, searchArtifactID, avfIds, startAtRecord})
            Return CType(results(0),InitializationResults)
        End Function
        
        '''<remarks/>
        Public Function BeginInitializeSearchExport(ByVal appID As Integer, ByVal searchArtifactID As Integer, ByVal avfIds() As Integer, ByVal startAtRecord As Integer, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("InitializeSearchExport", New Object() {appID, searchArtifactID, avfIds, startAtRecord}, callback, asyncState)
        End Function
        
        '''<remarks/>
        Public Function EndInitializeSearchExport(ByVal asyncResult As System.IAsyncResult) As InitializationResults
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),InitializationResults)
        End Function
        
        '''<remarks/>
        Public Overloads Sub InitializeSearchExportAsync(ByVal appID As Integer, ByVal searchArtifactID As Integer, ByVal avfIds() As Integer, ByVal startAtRecord As Integer)
            Me.InitializeSearchExportAsync(appID, searchArtifactID, avfIds, startAtRecord, Nothing)
        End Sub
        
        '''<remarks/>
        Public Overloads Sub InitializeSearchExportAsync(ByVal appID As Integer, ByVal searchArtifactID As Integer, ByVal avfIds() As Integer, ByVal startAtRecord As Integer, ByVal userState As Object)
            If (Me.InitializeSearchExportOperationCompleted Is Nothing) Then
                Me.InitializeSearchExportOperationCompleted = AddressOf Me.OnInitializeSearchExportOperationCompleted
            End If
            Me.InvokeAsync("InitializeSearchExport", New Object() {appID, searchArtifactID, avfIds, startAtRecord}, Me.InitializeSearchExportOperationCompleted, userState)
        End Sub
        
        Private Sub OnInitializeSearchExportOperationCompleted(ByVal arg As Object)
            If (Not (Me.InitializeSearchExportCompletedEvent) Is Nothing) Then
                Dim invokeArgs As System.Web.Services.Protocols.InvokeCompletedEventArgs = CType(arg,System.Web.Services.Protocols.InvokeCompletedEventArgs)
                RaiseEvent InitializeSearchExportCompleted(Me, New InitializeSearchExportCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState))
            End If
        End Sub
        
        '''<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/ExportManager/InitializeFolderExport", RequestNamespace:="http://www.kCura.com/EDDS/ExportManager", ResponseNamespace:="http://www.kCura.com/EDDS/ExportManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function InitializeFolderExport(ByVal appID As Integer, ByVal viewArtifactID As Integer, ByVal parentArtifactID As Integer, ByVal includeSubFolders As Boolean, ByVal avfIds() As Integer, ByVal startAtRecord As Integer, ByVal artifactTypeID As Integer) As InitializationResults
            Dim results() As Object = Me.Invoke("InitializeFolderExport", New Object() {appID, viewArtifactID, parentArtifactID, includeSubFolders, avfIds, startAtRecord, artifactTypeID})
            Return CType(results(0),InitializationResults)
        End Function
        
        '''<remarks/>
        Public Function BeginInitializeFolderExport(ByVal appID As Integer, ByVal viewArtifactID As Integer, ByVal parentArtifactID As Integer, ByVal includeSubFolders As Boolean, ByVal avfIds() As Integer, ByVal startAtRecord As Integer, ByVal artifactTypeID As Integer, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("InitializeFolderExport", New Object() {appID, viewArtifactID, parentArtifactID, includeSubFolders, avfIds, startAtRecord, artifactTypeID}, callback, asyncState)
        End Function
        
        '''<remarks/>
        Public Function EndInitializeFolderExport(ByVal asyncResult As System.IAsyncResult) As InitializationResults
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),InitializationResults)
        End Function
        
        '''<remarks/>
        Public Overloads Sub InitializeFolderExportAsync(ByVal appID As Integer, ByVal viewArtifactID As Integer, ByVal parentArtifactID As Integer, ByVal includeSubFolders As Boolean, ByVal avfIds() As Integer, ByVal startAtRecord As Integer, ByVal artifactTypeID As Integer)
            Me.InitializeFolderExportAsync(appID, viewArtifactID, parentArtifactID, includeSubFolders, avfIds, startAtRecord, artifactTypeID, Nothing)
        End Sub
        
        '''<remarks/>
        Public Overloads Sub InitializeFolderExportAsync(ByVal appID As Integer, ByVal viewArtifactID As Integer, ByVal parentArtifactID As Integer, ByVal includeSubFolders As Boolean, ByVal avfIds() As Integer, ByVal startAtRecord As Integer, ByVal artifactTypeID As Integer, ByVal userState As Object)
            If (Me.InitializeFolderExportOperationCompleted Is Nothing) Then
                Me.InitializeFolderExportOperationCompleted = AddressOf Me.OnInitializeFolderExportOperationCompleted
            End If
            Me.InvokeAsync("InitializeFolderExport", New Object() {appID, viewArtifactID, parentArtifactID, includeSubFolders, avfIds, startAtRecord, artifactTypeID}, Me.InitializeFolderExportOperationCompleted, userState)
        End Sub
        
        Private Sub OnInitializeFolderExportOperationCompleted(ByVal arg As Object)
            If (Not (Me.InitializeFolderExportCompletedEvent) Is Nothing) Then
                Dim invokeArgs As System.Web.Services.Protocols.InvokeCompletedEventArgs = CType(arg,System.Web.Services.Protocols.InvokeCompletedEventArgs)
                RaiseEvent InitializeFolderExportCompleted(Me, New InitializeFolderExportCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState))
            End If
        End Sub
        
        '''<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/ExportManager/InitializeProductionExport", RequestNamespace:="http://www.kCura.com/EDDS/ExportManager", ResponseNamespace:="http://www.kCura.com/EDDS/ExportManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function InitializeProductionExport(ByVal appID As Integer, ByVal productionArtifactID As Integer, ByVal avfIds() As Integer, ByVal startAtRecord As Integer) As InitializationResults
            Dim results() As Object = Me.Invoke("InitializeProductionExport", New Object() {appID, productionArtifactID, avfIds, startAtRecord})
            Return CType(results(0),InitializationResults)
        End Function
        
        '''<remarks/>
        Public Function BeginInitializeProductionExport(ByVal appID As Integer, ByVal productionArtifactID As Integer, ByVal avfIds() As Integer, ByVal startAtRecord As Integer, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("InitializeProductionExport", New Object() {appID, productionArtifactID, avfIds, startAtRecord}, callback, asyncState)
        End Function
        
        '''<remarks/>
        Public Function EndInitializeProductionExport(ByVal asyncResult As System.IAsyncResult) As InitializationResults
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),InitializationResults)
        End Function
        
        '''<remarks/>
        Public Overloads Sub InitializeProductionExportAsync(ByVal appID As Integer, ByVal productionArtifactID As Integer, ByVal avfIds() As Integer, ByVal startAtRecord As Integer)
            Me.InitializeProductionExportAsync(appID, productionArtifactID, avfIds, startAtRecord, Nothing)
        End Sub
        
        '''<remarks/>
        Public Overloads Sub InitializeProductionExportAsync(ByVal appID As Integer, ByVal productionArtifactID As Integer, ByVal avfIds() As Integer, ByVal startAtRecord As Integer, ByVal userState As Object)
            If (Me.InitializeProductionExportOperationCompleted Is Nothing) Then
                Me.InitializeProductionExportOperationCompleted = AddressOf Me.OnInitializeProductionExportOperationCompleted
            End If
            Me.InvokeAsync("InitializeProductionExport", New Object() {appID, productionArtifactID, avfIds, startAtRecord}, Me.InitializeProductionExportOperationCompleted, userState)
        End Sub
        
        Private Sub OnInitializeProductionExportOperationCompleted(ByVal arg As Object)
            If (Not (Me.InitializeProductionExportCompletedEvent) Is Nothing) Then
                Dim invokeArgs As System.Web.Services.Protocols.InvokeCompletedEventArgs = CType(arg,System.Web.Services.Protocols.InvokeCompletedEventArgs)
                RaiseEvent InitializeProductionExportCompleted(Me, New InitializeProductionExportCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState))
            End If
        End Sub
        
        '''<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/ExportManager/RetrieveResultsBlock", RequestNamespace:="http://www.kCura.com/EDDS/ExportManager", ResponseNamespace:="http://www.kCura.com/EDDS/ExportManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function RetrieveResultsBlock(ByVal appID As Integer, ByVal runId As System.Guid, ByVal artifactTypeID As Integer, ByVal avfIds() As Integer, ByVal chunkSize As Integer, ByVal displayMulticodesAsNested As Boolean, ByVal multiValueDelimiter As Char, ByVal nestedValueDelimiter As Char) As Object()
            Dim results() As Object = Me.Invoke("RetrieveResultsBlock", New Object() {appID, runId, artifactTypeID, avfIds, chunkSize, displayMulticodesAsNested, multiValueDelimiter, nestedValueDelimiter})
            Return CType(results(0),Object())
        End Function
        
        '''<remarks/>
        Public Function BeginRetrieveResultsBlock(ByVal appID As Integer, ByVal runId As System.Guid, ByVal artifactTypeID As Integer, ByVal avfIds() As Integer, ByVal chunkSize As Integer, ByVal displayMulticodesAsNested As Boolean, ByVal multiValueDelimiter As Char, ByVal nestedValueDelimiter As Char, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("RetrieveResultsBlock", New Object() {appID, runId, artifactTypeID, avfIds, chunkSize, displayMulticodesAsNested, multiValueDelimiter, nestedValueDelimiter}, callback, asyncState)
        End Function
        
        '''<remarks/>
        Public Function EndRetrieveResultsBlock(ByVal asyncResult As System.IAsyncResult) As Object()
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),Object())
        End Function
        
        '''<remarks/>
        Public Overloads Sub RetrieveResultsBlockAsync(ByVal appID As Integer, ByVal runId As System.Guid, ByVal artifactTypeID As Integer, ByVal avfIds() As Integer, ByVal chunkSize As Integer, ByVal displayMulticodesAsNested As Boolean, ByVal multiValueDelimiter As Char, ByVal nestedValueDelimiter As Char)
            Me.RetrieveResultsBlockAsync(appID, runId, artifactTypeID, avfIds, chunkSize, displayMulticodesAsNested, multiValueDelimiter, nestedValueDelimiter, Nothing)
        End Sub
        
        '''<remarks/>
        Public Overloads Sub RetrieveResultsBlockAsync(ByVal appID As Integer, ByVal runId As System.Guid, ByVal artifactTypeID As Integer, ByVal avfIds() As Integer, ByVal chunkSize As Integer, ByVal displayMulticodesAsNested As Boolean, ByVal multiValueDelimiter As Char, ByVal nestedValueDelimiter As Char, ByVal userState As Object)
            If (Me.RetrieveResultsBlockOperationCompleted Is Nothing) Then
                Me.RetrieveResultsBlockOperationCompleted = AddressOf Me.OnRetrieveResultsBlockOperationCompleted
            End If
            Me.InvokeAsync("RetrieveResultsBlock", New Object() {appID, runId, artifactTypeID, avfIds, chunkSize, displayMulticodesAsNested, multiValueDelimiter, nestedValueDelimiter}, Me.RetrieveResultsBlockOperationCompleted, userState)
        End Sub
        
        Private Sub OnRetrieveResultsBlockOperationCompleted(ByVal arg As Object)
            If (Not (Me.RetrieveResultsBlockCompletedEvent) Is Nothing) Then
                Dim invokeArgs As System.Web.Services.Protocols.InvokeCompletedEventArgs = CType(arg,System.Web.Services.Protocols.InvokeCompletedEventArgs)
                RaiseEvent RetrieveResultsBlockCompleted(Me, New RetrieveResultsBlockCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState))
            End If
        End Sub
        
        '''<remarks/>
        Public Shadows Sub CancelAsync(ByVal userState As Object)
            MyBase.CancelAsync(userState)
        End Sub
        
        Private Function IsLocalFileSystemWebService(ByVal url As String) As Boolean
            If ((url Is Nothing)  _
                        OrElse (url Is String.Empty)) Then
                Return false
            End If
            Dim wsUri As System.Uri = New System.Uri(url)
            If ((wsUri.Port >= 1024)  _
                        AndAlso (String.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) = 0)) Then
                Return true
            End If
            Return false
        End Function
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.1"),  _
     System.SerializableAttribute(),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.kCura.com/EDDS/ExportManager")>  _
    Partial Public Class InitializationResults
        
        Private runIdField As System.Guid
        
        Private rowCountField As Long
        
        Private columnNamesField() As String
        
        '''<remarks/>
        Public Property RunId() As System.Guid
            Get
                Return Me.runIdField
            End Get
            Set
                Me.runIdField = value
            End Set
        End Property
        
        '''<remarks/>
        Public Property RowCount() As Long
            Get
                Return Me.rowCountField
            End Get
            Set
                Me.rowCountField = value
            End Set
        End Property
        
        '''<remarks/>
        Public Property ColumnNames() As String()
            Get
                Return Me.columnNamesField
            End Get
            Set
                Me.columnNamesField = value
            End Set
        End Property
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")>  _
    Public Delegate Sub InitializeSearchExportCompletedEventHandler(ByVal sender As Object, ByVal e As InitializeSearchExportCompletedEventArgs)
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1"),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code")>  _
    Partial Public Class InitializeSearchExportCompletedEventArgs
        Inherits System.ComponentModel.AsyncCompletedEventArgs
        
        Private results() As Object
        
        Friend Sub New(ByVal results() As Object, ByVal exception As System.Exception, ByVal cancelled As Boolean, ByVal userState As Object)
            MyBase.New(exception, cancelled, userState)
            Me.results = results
        End Sub
        
        '''<remarks/>
        Public ReadOnly Property Result() As InitializationResults
            Get
                Me.RaiseExceptionIfNecessary
                Return CType(Me.results(0),InitializationResults)
            End Get
        End Property
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")>  _
    Public Delegate Sub InitializeFolderExportCompletedEventHandler(ByVal sender As Object, ByVal e As InitializeFolderExportCompletedEventArgs)
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1"),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code")>  _
    Partial Public Class InitializeFolderExportCompletedEventArgs
        Inherits System.ComponentModel.AsyncCompletedEventArgs
        
        Private results() As Object
        
        Friend Sub New(ByVal results() As Object, ByVal exception As System.Exception, ByVal cancelled As Boolean, ByVal userState As Object)
            MyBase.New(exception, cancelled, userState)
            Me.results = results
        End Sub
        
        '''<remarks/>
        Public ReadOnly Property Result() As InitializationResults
            Get
                Me.RaiseExceptionIfNecessary
                Return CType(Me.results(0),InitializationResults)
            End Get
        End Property
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")>  _
    Public Delegate Sub InitializeProductionExportCompletedEventHandler(ByVal sender As Object, ByVal e As InitializeProductionExportCompletedEventArgs)
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1"),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code")>  _
    Partial Public Class InitializeProductionExportCompletedEventArgs
        Inherits System.ComponentModel.AsyncCompletedEventArgs
        
        Private results() As Object
        
        Friend Sub New(ByVal results() As Object, ByVal exception As System.Exception, ByVal cancelled As Boolean, ByVal userState As Object)
            MyBase.New(exception, cancelled, userState)
            Me.results = results
        End Sub
        
        '''<remarks/>
        Public ReadOnly Property Result() As InitializationResults
            Get
                Me.RaiseExceptionIfNecessary
                Return CType(Me.results(0),InitializationResults)
            End Get
        End Property
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")>  _
    Public Delegate Sub RetrieveResultsBlockCompletedEventHandler(ByVal sender As Object, ByVal e As RetrieveResultsBlockCompletedEventArgs)
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1"),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code")>  _
    Partial Public Class RetrieveResultsBlockCompletedEventArgs
        Inherits System.ComponentModel.AsyncCompletedEventArgs
        
        Private results() As Object
        
        Friend Sub New(ByVal results() As Object, ByVal exception As System.Exception, ByVal cancelled As Boolean, ByVal userState As Object)
            MyBase.New(exception, cancelled, userState)
            Me.results = results
        End Sub
        
        '''<remarks/>
        Public ReadOnly Property Result() As Object()
            Get
                Me.RaiseExceptionIfNecessary
                Return CType(Me.results(0),Object())
            End Get
        End Property
    End Class
End Namespace
