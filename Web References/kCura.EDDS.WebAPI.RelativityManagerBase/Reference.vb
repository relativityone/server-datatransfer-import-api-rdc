﻿'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:4.0.30319.239
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict Off
Option Explicit On

Imports System
Imports System.ComponentModel
Imports System.Data
Imports System.Diagnostics
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Xml.Serialization

'
'This source code was auto-generated by Microsoft.VSDesigner, Version 4.0.30319.239.
'
Namespace kCura.EDDS.WebAPI.RelativityManagerBase
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1"),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Web.Services.WebServiceBindingAttribute(Name:="RelativityManagerSoap", [Namespace]:="http://www.kCura.com/EDDS/RelativityManager")>  _
    Partial Public Class RelativityManager
        Inherits System.Web.Services.Protocols.SoapHttpClientProtocol
        
        Private RetrieveCurrencySymbolOperationCompleted As System.Threading.SendOrPostCallback
        
        Private RetrieveRelativityVersionOperationCompleted As System.Threading.SendOrPostCallback
        
        Private ValidateSuccessfulLoginOperationCompleted As System.Threading.SendOrPostCallback
        
        Private GetServerTimezoneOffsetOperationCompleted As System.Threading.SendOrPostCallback
        
        Private IsAuditingEnabledOperationCompleted As System.Threading.SendOrPostCallback
        
        Private IsImportEmailNotificationEnabledOperationCompleted As System.Threading.SendOrPostCallback
        
        Private RetrieveRdcConfigurationOperationCompleted As System.Threading.SendOrPostCallback
        
        Private PingOperationCompleted As System.Threading.SendOrPostCallback
        
        Private ReceiveTextOperationCompleted As System.Threading.SendOrPostCallback
        
        Private useDefaultCredentialsSetExplicitly As Boolean
        
        '''<remarks/>
        Public Sub New()
            MyBase.New
            Me.Url = "http://localhost/RelativityWebApi/RelativityManager.asmx"
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
        Public Event RetrieveCurrencySymbolCompleted As RetrieveCurrencySymbolCompletedEventHandler
        
        '''<remarks/>
        Public Event RetrieveRelativityVersionCompleted As RetrieveRelativityVersionCompletedEventHandler
        
        '''<remarks/>
        Public Event ValidateSuccessfulLoginCompleted As ValidateSuccessfulLoginCompletedEventHandler
        
        '''<remarks/>
        Public Event GetServerTimezoneOffsetCompleted As GetServerTimezoneOffsetCompletedEventHandler
        
        '''<remarks/>
        Public Event IsAuditingEnabledCompleted As IsAuditingEnabledCompletedEventHandler
        
        '''<remarks/>
        Public Event IsImportEmailNotificationEnabledCompleted As IsImportEmailNotificationEnabledCompletedEventHandler
        
        '''<remarks/>
        Public Event RetrieveRdcConfigurationCompleted As RetrieveRdcConfigurationCompletedEventHandler
        
        '''<remarks/>
        Public Event PingCompleted As PingCompletedEventHandler
        
        '''<remarks/>
        Public Event ReceiveTextCompleted As ReceiveTextCompletedEventHandler
        
        '''<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/RelativityManager/RetrieveCurrencySymbol", RequestNamespace:="http://www.kCura.com/EDDS/RelativityManager", ResponseNamespace:="http://www.kCura.com/EDDS/RelativityManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function RetrieveCurrencySymbol() As String
            Dim results() As Object = Me.Invoke("RetrieveCurrencySymbol", New Object(-1) {})
            Return CType(results(0),String)
        End Function
        
        '''<remarks/>
        Public Function BeginRetrieveCurrencySymbol(ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("RetrieveCurrencySymbol", New Object(-1) {}, callback, asyncState)
        End Function
        
        '''<remarks/>
        Public Function EndRetrieveCurrencySymbol(ByVal asyncResult As System.IAsyncResult) As String
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),String)
        End Function
        
        '''<remarks/>
        Public Overloads Sub RetrieveCurrencySymbolAsync()
            Me.RetrieveCurrencySymbolAsync(Nothing)
        End Sub
        
        '''<remarks/>
        Public Overloads Sub RetrieveCurrencySymbolAsync(ByVal userState As Object)
            If (Me.RetrieveCurrencySymbolOperationCompleted Is Nothing) Then
                Me.RetrieveCurrencySymbolOperationCompleted = AddressOf Me.OnRetrieveCurrencySymbolOperationCompleted
            End If
            Me.InvokeAsync("RetrieveCurrencySymbol", New Object(-1) {}, Me.RetrieveCurrencySymbolOperationCompleted, userState)
        End Sub
        
        Private Sub OnRetrieveCurrencySymbolOperationCompleted(ByVal arg As Object)
            If (Not (Me.RetrieveCurrencySymbolCompletedEvent) Is Nothing) Then
                Dim invokeArgs As System.Web.Services.Protocols.InvokeCompletedEventArgs = CType(arg,System.Web.Services.Protocols.InvokeCompletedEventArgs)
                RaiseEvent RetrieveCurrencySymbolCompleted(Me, New RetrieveCurrencySymbolCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState))
            End If
        End Sub
        
        '''<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/RelativityManager/RetrieveRelativityVersion", RequestNamespace:="http://www.kCura.com/EDDS/RelativityManager", ResponseNamespace:="http://www.kCura.com/EDDS/RelativityManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function RetrieveRelativityVersion() As String
            Dim results() As Object = Me.Invoke("RetrieveRelativityVersion", New Object(-1) {})
            Return CType(results(0),String)
        End Function
        
        '''<remarks/>
        Public Function BeginRetrieveRelativityVersion(ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("RetrieveRelativityVersion", New Object(-1) {}, callback, asyncState)
        End Function
        
        '''<remarks/>
        Public Function EndRetrieveRelativityVersion(ByVal asyncResult As System.IAsyncResult) As String
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),String)
        End Function
        
        '''<remarks/>
        Public Overloads Sub RetrieveRelativityVersionAsync()
            Me.RetrieveRelativityVersionAsync(Nothing)
        End Sub
        
        '''<remarks/>
        Public Overloads Sub RetrieveRelativityVersionAsync(ByVal userState As Object)
            If (Me.RetrieveRelativityVersionOperationCompleted Is Nothing) Then
                Me.RetrieveRelativityVersionOperationCompleted = AddressOf Me.OnRetrieveRelativityVersionOperationCompleted
            End If
            Me.InvokeAsync("RetrieveRelativityVersion", New Object(-1) {}, Me.RetrieveRelativityVersionOperationCompleted, userState)
        End Sub
        
        Private Sub OnRetrieveRelativityVersionOperationCompleted(ByVal arg As Object)
            If (Not (Me.RetrieveRelativityVersionCompletedEvent) Is Nothing) Then
                Dim invokeArgs As System.Web.Services.Protocols.InvokeCompletedEventArgs = CType(arg,System.Web.Services.Protocols.InvokeCompletedEventArgs)
                RaiseEvent RetrieveRelativityVersionCompleted(Me, New RetrieveRelativityVersionCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState))
            End If
        End Sub
        
        '''<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/RelativityManager/ValidateSuccessfulLogin", RequestNamespace:="http://www.kCura.com/EDDS/RelativityManager", ResponseNamespace:="http://www.kCura.com/EDDS/RelativityManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function ValidateSuccessfulLogin() As Boolean
            Dim results() As Object = Me.Invoke("ValidateSuccessfulLogin", New Object(-1) {})
            Return CType(results(0),Boolean)
        End Function
        
        '''<remarks/>
        Public Function BeginValidateSuccessfulLogin(ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("ValidateSuccessfulLogin", New Object(-1) {}, callback, asyncState)
        End Function
        
        '''<remarks/>
        Public Function EndValidateSuccessfulLogin(ByVal asyncResult As System.IAsyncResult) As Boolean
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),Boolean)
        End Function
        
        '''<remarks/>
        Public Overloads Sub ValidateSuccessfulLoginAsync()
            Me.ValidateSuccessfulLoginAsync(Nothing)
        End Sub
        
        '''<remarks/>
        Public Overloads Sub ValidateSuccessfulLoginAsync(ByVal userState As Object)
            If (Me.ValidateSuccessfulLoginOperationCompleted Is Nothing) Then
                Me.ValidateSuccessfulLoginOperationCompleted = AddressOf Me.OnValidateSuccessfulLoginOperationCompleted
            End If
            Me.InvokeAsync("ValidateSuccessfulLogin", New Object(-1) {}, Me.ValidateSuccessfulLoginOperationCompleted, userState)
        End Sub
        
        Private Sub OnValidateSuccessfulLoginOperationCompleted(ByVal arg As Object)
            If (Not (Me.ValidateSuccessfulLoginCompletedEvent) Is Nothing) Then
                Dim invokeArgs As System.Web.Services.Protocols.InvokeCompletedEventArgs = CType(arg,System.Web.Services.Protocols.InvokeCompletedEventArgs)
                RaiseEvent ValidateSuccessfulLoginCompleted(Me, New ValidateSuccessfulLoginCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState))
            End If
        End Sub
        
        '''<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/RelativityManager/GetServerTimezoneOffset", RequestNamespace:="http://www.kCura.com/EDDS/RelativityManager", ResponseNamespace:="http://www.kCura.com/EDDS/RelativityManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function GetServerTimezoneOffset() As Integer
            Dim results() As Object = Me.Invoke("GetServerTimezoneOffset", New Object(-1) {})
            Return CType(results(0),Integer)
        End Function
        
        '''<remarks/>
        Public Function BeginGetServerTimezoneOffset(ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("GetServerTimezoneOffset", New Object(-1) {}, callback, asyncState)
        End Function
        
        '''<remarks/>
        Public Function EndGetServerTimezoneOffset(ByVal asyncResult As System.IAsyncResult) As Integer
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),Integer)
        End Function
        
        '''<remarks/>
        Public Overloads Sub GetServerTimezoneOffsetAsync()
            Me.GetServerTimezoneOffsetAsync(Nothing)
        End Sub
        
        '''<remarks/>
        Public Overloads Sub GetServerTimezoneOffsetAsync(ByVal userState As Object)
            If (Me.GetServerTimezoneOffsetOperationCompleted Is Nothing) Then
                Me.GetServerTimezoneOffsetOperationCompleted = AddressOf Me.OnGetServerTimezoneOffsetOperationCompleted
            End If
            Me.InvokeAsync("GetServerTimezoneOffset", New Object(-1) {}, Me.GetServerTimezoneOffsetOperationCompleted, userState)
        End Sub
        
        Private Sub OnGetServerTimezoneOffsetOperationCompleted(ByVal arg As Object)
            If (Not (Me.GetServerTimezoneOffsetCompletedEvent) Is Nothing) Then
                Dim invokeArgs As System.Web.Services.Protocols.InvokeCompletedEventArgs = CType(arg,System.Web.Services.Protocols.InvokeCompletedEventArgs)
                RaiseEvent GetServerTimezoneOffsetCompleted(Me, New GetServerTimezoneOffsetCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState))
            End If
        End Sub
        
        '''<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/RelativityManager/IsAuditingEnabled", RequestNamespace:="http://www.kCura.com/EDDS/RelativityManager", ResponseNamespace:="http://www.kCura.com/EDDS/RelativityManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function IsAuditingEnabled() As Boolean
            Dim results() As Object = Me.Invoke("IsAuditingEnabled", New Object(-1) {})
            Return CType(results(0),Boolean)
        End Function
        
        '''<remarks/>
        Public Function BeginIsAuditingEnabled(ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("IsAuditingEnabled", New Object(-1) {}, callback, asyncState)
        End Function
        
        '''<remarks/>
        Public Function EndIsAuditingEnabled(ByVal asyncResult As System.IAsyncResult) As Boolean
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),Boolean)
        End Function
        
        '''<remarks/>
        Public Overloads Sub IsAuditingEnabledAsync()
            Me.IsAuditingEnabledAsync(Nothing)
        End Sub
        
        '''<remarks/>
        Public Overloads Sub IsAuditingEnabledAsync(ByVal userState As Object)
            If (Me.IsAuditingEnabledOperationCompleted Is Nothing) Then
                Me.IsAuditingEnabledOperationCompleted = AddressOf Me.OnIsAuditingEnabledOperationCompleted
            End If
            Me.InvokeAsync("IsAuditingEnabled", New Object(-1) {}, Me.IsAuditingEnabledOperationCompleted, userState)
        End Sub
        
        Private Sub OnIsAuditingEnabledOperationCompleted(ByVal arg As Object)
            If (Not (Me.IsAuditingEnabledCompletedEvent) Is Nothing) Then
                Dim invokeArgs As System.Web.Services.Protocols.InvokeCompletedEventArgs = CType(arg,System.Web.Services.Protocols.InvokeCompletedEventArgs)
                RaiseEvent IsAuditingEnabledCompleted(Me, New IsAuditingEnabledCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState))
            End If
        End Sub
        
        '''<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/RelativityManager/IsImportEmailNotificationEnabled", RequestNamespace:="http://www.kCura.com/EDDS/RelativityManager", ResponseNamespace:="http://www.kCura.com/EDDS/RelativityManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function IsImportEmailNotificationEnabled() As Boolean
            Dim results() As Object = Me.Invoke("IsImportEmailNotificationEnabled", New Object(-1) {})
            Return CType(results(0),Boolean)
        End Function
        
        '''<remarks/>
        Public Function BeginIsImportEmailNotificationEnabled(ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("IsImportEmailNotificationEnabled", New Object(-1) {}, callback, asyncState)
        End Function
        
        '''<remarks/>
        Public Function EndIsImportEmailNotificationEnabled(ByVal asyncResult As System.IAsyncResult) As Boolean
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),Boolean)
        End Function
        
        '''<remarks/>
        Public Overloads Sub IsImportEmailNotificationEnabledAsync()
            Me.IsImportEmailNotificationEnabledAsync(Nothing)
        End Sub
        
        '''<remarks/>
        Public Overloads Sub IsImportEmailNotificationEnabledAsync(ByVal userState As Object)
            If (Me.IsImportEmailNotificationEnabledOperationCompleted Is Nothing) Then
                Me.IsImportEmailNotificationEnabledOperationCompleted = AddressOf Me.OnIsImportEmailNotificationEnabledOperationCompleted
            End If
            Me.InvokeAsync("IsImportEmailNotificationEnabled", New Object(-1) {}, Me.IsImportEmailNotificationEnabledOperationCompleted, userState)
        End Sub
        
        Private Sub OnIsImportEmailNotificationEnabledOperationCompleted(ByVal arg As Object)
            If (Not (Me.IsImportEmailNotificationEnabledCompletedEvent) Is Nothing) Then
                Dim invokeArgs As System.Web.Services.Protocols.InvokeCompletedEventArgs = CType(arg,System.Web.Services.Protocols.InvokeCompletedEventArgs)
                RaiseEvent IsImportEmailNotificationEnabledCompleted(Me, New IsImportEmailNotificationEnabledCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState))
            End If
        End Sub
        
        '''<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/RelativityManager/RetrieveRdcConfiguration", RequestNamespace:="http://www.kCura.com/EDDS/RelativityManager", ResponseNamespace:="http://www.kCura.com/EDDS/RelativityManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function RetrieveRdcConfiguration() As System.Data.DataSet
            Dim results() As Object = Me.Invoke("RetrieveRdcConfiguration", New Object(-1) {})
            Return CType(results(0),System.Data.DataSet)
        End Function
        
        '''<remarks/>
        Public Function BeginRetrieveRdcConfiguration(ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("RetrieveRdcConfiguration", New Object(-1) {}, callback, asyncState)
        End Function
        
        '''<remarks/>
        Public Function EndRetrieveRdcConfiguration(ByVal asyncResult As System.IAsyncResult) As System.Data.DataSet
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),System.Data.DataSet)
        End Function
        
        '''<remarks/>
        Public Overloads Sub RetrieveRdcConfigurationAsync()
            Me.RetrieveRdcConfigurationAsync(Nothing)
        End Sub
        
        '''<remarks/>
        Public Overloads Sub RetrieveRdcConfigurationAsync(ByVal userState As Object)
            If (Me.RetrieveRdcConfigurationOperationCompleted Is Nothing) Then
                Me.RetrieveRdcConfigurationOperationCompleted = AddressOf Me.OnRetrieveRdcConfigurationOperationCompleted
            End If
            Me.InvokeAsync("RetrieveRdcConfiguration", New Object(-1) {}, Me.RetrieveRdcConfigurationOperationCompleted, userState)
        End Sub
        
        Private Sub OnRetrieveRdcConfigurationOperationCompleted(ByVal arg As Object)
            If (Not (Me.RetrieveRdcConfigurationCompletedEvent) Is Nothing) Then
                Dim invokeArgs As System.Web.Services.Protocols.InvokeCompletedEventArgs = CType(arg,System.Web.Services.Protocols.InvokeCompletedEventArgs)
                RaiseEvent RetrieveRdcConfigurationCompleted(Me, New RetrieveRdcConfigurationCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState))
            End If
        End Sub
        
        '''<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/RelativityManager/Ping", RequestNamespace:="http://www.kCura.com/EDDS/RelativityManager", ResponseNamespace:="http://www.kCura.com/EDDS/RelativityManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function Ping() As String
            Dim results() As Object = Me.Invoke("Ping", New Object(-1) {})
            Return CType(results(0),String)
        End Function
        
        '''<remarks/>
        Public Function BeginPing(ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("Ping", New Object(-1) {}, callback, asyncState)
        End Function
        
        '''<remarks/>
        Public Function EndPing(ByVal asyncResult As System.IAsyncResult) As String
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),String)
        End Function
        
        '''<remarks/>
        Public Overloads Sub PingAsync()
            Me.PingAsync(Nothing)
        End Sub
        
        '''<remarks/>
        Public Overloads Sub PingAsync(ByVal userState As Object)
            If (Me.PingOperationCompleted Is Nothing) Then
                Me.PingOperationCompleted = AddressOf Me.OnPingOperationCompleted
            End If
            Me.InvokeAsync("Ping", New Object(-1) {}, Me.PingOperationCompleted, userState)
        End Sub
        
        Private Sub OnPingOperationCompleted(ByVal arg As Object)
            If (Not (Me.PingCompletedEvent) Is Nothing) Then
                Dim invokeArgs As System.Web.Services.Protocols.InvokeCompletedEventArgs = CType(arg,System.Web.Services.Protocols.InvokeCompletedEventArgs)
                RaiseEvent PingCompleted(Me, New PingCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState))
            End If
        End Sub
        
        '''<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/RelativityManager/ReceiveText", RequestNamespace:="http://www.kCura.com/EDDS/RelativityManager", ResponseNamespace:="http://www.kCura.com/EDDS/RelativityManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function ReceiveText(ByVal text As String) As Boolean
            Dim results() As Object = Me.Invoke("ReceiveText", New Object() {text})
            Return CType(results(0),Boolean)
        End Function
        
        '''<remarks/>
        Public Function BeginReceiveText(ByVal text As String, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("ReceiveText", New Object() {text}, callback, asyncState)
        End Function
        
        '''<remarks/>
        Public Function EndReceiveText(ByVal asyncResult As System.IAsyncResult) As Boolean
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),Boolean)
        End Function
        
        '''<remarks/>
        Public Overloads Sub ReceiveTextAsync(ByVal text As String)
            Me.ReceiveTextAsync(text, Nothing)
        End Sub
        
        '''<remarks/>
        Public Overloads Sub ReceiveTextAsync(ByVal text As String, ByVal userState As Object)
            If (Me.ReceiveTextOperationCompleted Is Nothing) Then
                Me.ReceiveTextOperationCompleted = AddressOf Me.OnReceiveTextOperationCompleted
            End If
            Me.InvokeAsync("ReceiveText", New Object() {text}, Me.ReceiveTextOperationCompleted, userState)
        End Sub
        
        Private Sub OnReceiveTextOperationCompleted(ByVal arg As Object)
            If (Not (Me.ReceiveTextCompletedEvent) Is Nothing) Then
                Dim invokeArgs As System.Web.Services.Protocols.InvokeCompletedEventArgs = CType(arg,System.Web.Services.Protocols.InvokeCompletedEventArgs)
                RaiseEvent ReceiveTextCompleted(Me, New ReceiveTextCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState))
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
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")>  _
    Public Delegate Sub RetrieveCurrencySymbolCompletedEventHandler(ByVal sender As Object, ByVal e As RetrieveCurrencySymbolCompletedEventArgs)
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1"),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code")>  _
    Partial Public Class RetrieveCurrencySymbolCompletedEventArgs
        Inherits System.ComponentModel.AsyncCompletedEventArgs
        
        Private results() As Object
        
        Friend Sub New(ByVal results() As Object, ByVal exception As System.Exception, ByVal cancelled As Boolean, ByVal userState As Object)
            MyBase.New(exception, cancelled, userState)
            Me.results = results
        End Sub
        
        '''<remarks/>
        Public ReadOnly Property Result() As String
            Get
                Me.RaiseExceptionIfNecessary
                Return CType(Me.results(0),String)
            End Get
        End Property
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")>  _
    Public Delegate Sub RetrieveRelativityVersionCompletedEventHandler(ByVal sender As Object, ByVal e As RetrieveRelativityVersionCompletedEventArgs)
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1"),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code")>  _
    Partial Public Class RetrieveRelativityVersionCompletedEventArgs
        Inherits System.ComponentModel.AsyncCompletedEventArgs
        
        Private results() As Object
        
        Friend Sub New(ByVal results() As Object, ByVal exception As System.Exception, ByVal cancelled As Boolean, ByVal userState As Object)
            MyBase.New(exception, cancelled, userState)
            Me.results = results
        End Sub
        
        '''<remarks/>
        Public ReadOnly Property Result() As String
            Get
                Me.RaiseExceptionIfNecessary
                Return CType(Me.results(0),String)
            End Get
        End Property
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")>  _
    Public Delegate Sub ValidateSuccessfulLoginCompletedEventHandler(ByVal sender As Object, ByVal e As ValidateSuccessfulLoginCompletedEventArgs)
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1"),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code")>  _
    Partial Public Class ValidateSuccessfulLoginCompletedEventArgs
        Inherits System.ComponentModel.AsyncCompletedEventArgs
        
        Private results() As Object
        
        Friend Sub New(ByVal results() As Object, ByVal exception As System.Exception, ByVal cancelled As Boolean, ByVal userState As Object)
            MyBase.New(exception, cancelled, userState)
            Me.results = results
        End Sub
        
        '''<remarks/>
        Public ReadOnly Property Result() As Boolean
            Get
                Me.RaiseExceptionIfNecessary
                Return CType(Me.results(0),Boolean)
            End Get
        End Property
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")>  _
    Public Delegate Sub GetServerTimezoneOffsetCompletedEventHandler(ByVal sender As Object, ByVal e As GetServerTimezoneOffsetCompletedEventArgs)
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1"),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code")>  _
    Partial Public Class GetServerTimezoneOffsetCompletedEventArgs
        Inherits System.ComponentModel.AsyncCompletedEventArgs
        
        Private results() As Object
        
        Friend Sub New(ByVal results() As Object, ByVal exception As System.Exception, ByVal cancelled As Boolean, ByVal userState As Object)
            MyBase.New(exception, cancelled, userState)
            Me.results = results
        End Sub
        
        '''<remarks/>
        Public ReadOnly Property Result() As Integer
            Get
                Me.RaiseExceptionIfNecessary
                Return CType(Me.results(0),Integer)
            End Get
        End Property
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")>  _
    Public Delegate Sub IsAuditingEnabledCompletedEventHandler(ByVal sender As Object, ByVal e As IsAuditingEnabledCompletedEventArgs)
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1"),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code")>  _
    Partial Public Class IsAuditingEnabledCompletedEventArgs
        Inherits System.ComponentModel.AsyncCompletedEventArgs
        
        Private results() As Object
        
        Friend Sub New(ByVal results() As Object, ByVal exception As System.Exception, ByVal cancelled As Boolean, ByVal userState As Object)
            MyBase.New(exception, cancelled, userState)
            Me.results = results
        End Sub
        
        '''<remarks/>
        Public ReadOnly Property Result() As Boolean
            Get
                Me.RaiseExceptionIfNecessary
                Return CType(Me.results(0),Boolean)
            End Get
        End Property
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")>  _
    Public Delegate Sub IsImportEmailNotificationEnabledCompletedEventHandler(ByVal sender As Object, ByVal e As IsImportEmailNotificationEnabledCompletedEventArgs)
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1"),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code")>  _
    Partial Public Class IsImportEmailNotificationEnabledCompletedEventArgs
        Inherits System.ComponentModel.AsyncCompletedEventArgs
        
        Private results() As Object
        
        Friend Sub New(ByVal results() As Object, ByVal exception As System.Exception, ByVal cancelled As Boolean, ByVal userState As Object)
            MyBase.New(exception, cancelled, userState)
            Me.results = results
        End Sub
        
        '''<remarks/>
        Public ReadOnly Property Result() As Boolean
            Get
                Me.RaiseExceptionIfNecessary
                Return CType(Me.results(0),Boolean)
            End Get
        End Property
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")>  _
    Public Delegate Sub RetrieveRdcConfigurationCompletedEventHandler(ByVal sender As Object, ByVal e As RetrieveRdcConfigurationCompletedEventArgs)
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1"),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code")>  _
    Partial Public Class RetrieveRdcConfigurationCompletedEventArgs
        Inherits System.ComponentModel.AsyncCompletedEventArgs
        
        Private results() As Object
        
        Friend Sub New(ByVal results() As Object, ByVal exception As System.Exception, ByVal cancelled As Boolean, ByVal userState As Object)
            MyBase.New(exception, cancelled, userState)
            Me.results = results
        End Sub
        
        '''<remarks/>
        Public ReadOnly Property Result() As System.Data.DataSet
            Get
                Me.RaiseExceptionIfNecessary
                Return CType(Me.results(0),System.Data.DataSet)
            End Get
        End Property
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")>  _
    Public Delegate Sub PingCompletedEventHandler(ByVal sender As Object, ByVal e As PingCompletedEventArgs)
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1"),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code")>  _
    Partial Public Class PingCompletedEventArgs
        Inherits System.ComponentModel.AsyncCompletedEventArgs
        
        Private results() As Object
        
        Friend Sub New(ByVal results() As Object, ByVal exception As System.Exception, ByVal cancelled As Boolean, ByVal userState As Object)
            MyBase.New(exception, cancelled, userState)
            Me.results = results
        End Sub
        
        '''<remarks/>
        Public ReadOnly Property Result() As String
            Get
                Me.RaiseExceptionIfNecessary
                Return CType(Me.results(0),String)
            End Get
        End Property
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")>  _
    Public Delegate Sub ReceiveTextCompletedEventHandler(ByVal sender As Object, ByVal e As ReceiveTextCompletedEventArgs)
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1"),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code")>  _
    Partial Public Class ReceiveTextCompletedEventArgs
        Inherits System.ComponentModel.AsyncCompletedEventArgs
        
        Private results() As Object
        
        Friend Sub New(ByVal results() As Object, ByVal exception As System.Exception, ByVal cancelled As Boolean, ByVal userState As Object)
            MyBase.New(exception, cancelled, userState)
            Me.results = results
        End Sub
        
        '''<remarks/>
        Public ReadOnly Property Result() As Boolean
            Get
                Me.RaiseExceptionIfNecessary
                Return CType(Me.results(0),Boolean)
            End Get
        End Property
    End Class
End Namespace
