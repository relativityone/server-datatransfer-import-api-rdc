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
Namespace kCura.EDDS.WebAPI.TemplateManagerBase
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1"),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Web.Services.WebServiceBindingAttribute(Name:="TemplateManagerSoap", [Namespace]:="http://www.kCura.com/EDDS/TemplateManager"),  _
     System.Xml.Serialization.XmlIncludeAttribute(GetType(ApplicationBase))>  _
    Partial Public Class TemplateManager
        Inherits System.Web.Services.Protocols.SoapHttpClientProtocol
        
        Private InstallTemplateOperationCompleted As System.Threading.SendOrPostCallback
        
        Private useDefaultCredentialsSetExplicitly As Boolean
        
        '''<remarks/>
        Public Sub New()
            MyBase.New
            Me.Url = "http://localhost/RelativityWebApi/TemplateManager.asmx"
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
        Public Event InstallTemplateCompleted As InstallTemplateCompletedEventHandler
        
        '''<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.kCura.com/EDDS/TemplateManager/InstallTemplate", RequestNamespace:="http://www.kCura.com/EDDS/TemplateManager", ResponseNamespace:="http://www.kCura.com/EDDS/TemplateManager", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)>  _
        Public Function InstallTemplate(ByVal template As System.Xml.XmlNode, ByVal installationParameters As ApplicationInstallationParameters) As ApplicationInstallationResult
            Dim results() As Object = Me.Invoke("InstallTemplate", New Object() {template, installationParameters})
            Return CType(results(0),ApplicationInstallationResult)
        End Function
        
        '''<remarks/>
        Public Function BeginInstallTemplate(ByVal template As System.Xml.XmlNode, ByVal installationParameters As ApplicationInstallationParameters, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("InstallTemplate", New Object() {template, installationParameters}, callback, asyncState)
        End Function
        
        '''<remarks/>
        Public Function EndInstallTemplate(ByVal asyncResult As System.IAsyncResult) As ApplicationInstallationResult
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),ApplicationInstallationResult)
        End Function
        
        '''<remarks/>
        Public Overloads Sub InstallTemplateAsync(ByVal template As System.Xml.XmlNode, ByVal installationParameters As ApplicationInstallationParameters)
            Me.InstallTemplateAsync(template, installationParameters, Nothing)
        End Sub
        
        '''<remarks/>
        Public Overloads Sub InstallTemplateAsync(ByVal template As System.Xml.XmlNode, ByVal installationParameters As ApplicationInstallationParameters, ByVal userState As Object)
            If (Me.InstallTemplateOperationCompleted Is Nothing) Then
                Me.InstallTemplateOperationCompleted = AddressOf Me.OnInstallTemplateOperationCompleted
            End If
            Me.InvokeAsync("InstallTemplate", New Object() {template, installationParameters}, Me.InstallTemplateOperationCompleted, userState)
        End Sub
        
        Private Sub OnInstallTemplateOperationCompleted(ByVal arg As Object)
            If (Not (Me.InstallTemplateCompletedEvent) Is Nothing) Then
                Dim invokeArgs As System.Web.Services.Protocols.InvokeCompletedEventArgs = CType(arg,System.Web.Services.Protocols.InvokeCompletedEventArgs)
                RaiseEvent InstallTemplateCompleted(Me, New InstallTemplateCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState))
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
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.kCura.com/EDDS/TemplateManager")>  _
    Partial Public Class ApplicationInstallationParameters
        
        Private caseIdField As Integer
        
        '''<remarks/>
        Public Property CaseId() As Integer
            Get
                Return Me.caseIdField
            End Get
            Set
                Me.caseIdField = value
            End Set
        End Property
    End Class
    
    '''<remarks/>
    <System.Xml.Serialization.XmlIncludeAttribute(GetType(Application)),  _
     System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.1"),  _
     System.SerializableAttribute(),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.kCura.com/EDDS/TemplateManager")>  _
    Partial Public Class ApplicationBase
        
        Private guidField As System.Guid
        
        Private inDevelopmentField As Boolean
        
        Private nameField As String
        
        Private versionField As String
        
        '''<remarks/>
        Public Property Guid() As System.Guid
            Get
                Return Me.guidField
            End Get
            Set
                Me.guidField = value
            End Set
        End Property
        
        '''<remarks/>
        Public Property InDevelopment() As Boolean
            Get
                Return Me.inDevelopmentField
            End Get
            Set
                Me.inDevelopmentField = value
            End Set
        End Property
        
        '''<remarks/>
        Public Property Name() As String
            Get
                Return Me.nameField
            End Get
            Set
                Me.nameField = value
            End Set
        End Property
        
        '''<remarks/>
        Public Property Version() As String
            Get
                Return Me.versionField
            End Get
            Set
                Me.versionField = value
            End Set
        End Property
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.1"),  _
     System.SerializableAttribute(),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.kCura.com/EDDS/TemplateManager")>  _
    Partial Public Class Application
        Inherits ApplicationBase
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.1"),  _
     System.SerializableAttribute(),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.kCura.com/EDDS/TemplateManager")>  _
    Partial Public Class ApplicationArtifact
        
        Private workspaceIDField As Integer
        
        Private applicationsField() As Application
        
        Private artifactIdField As Integer
        
        Private guidField As System.Guid
        
        Private nameField As String
        
        Private typeField As ApplicationArtifactType
        
        Private parentArtifactField As ApplicationArtifact
        
        Private conflictArtifactField As ApplicationArtifact
        
        Private statusField As StatusCode
        
        Private statusMessageField As String
        
        Private statusDetailsField As String
        
        '''<remarks/>
        Public Property WorkspaceID() As Integer
            Get
                Return Me.workspaceIDField
            End Get
            Set
                Me.workspaceIDField = value
            End Set
        End Property
        
        '''<remarks/>
        Public Property Applications() As Application()
            Get
                Return Me.applicationsField
            End Get
            Set
                Me.applicationsField = value
            End Set
        End Property
        
        '''<remarks/>
        Public Property ArtifactId() As Integer
            Get
                Return Me.artifactIdField
            End Get
            Set
                Me.artifactIdField = value
            End Set
        End Property
        
        '''<remarks/>
        Public Property Guid() As System.Guid
            Get
                Return Me.guidField
            End Get
            Set
                Me.guidField = value
            End Set
        End Property
        
        '''<remarks/>
        Public Property Name() As String
            Get
                Return Me.nameField
            End Get
            Set
                Me.nameField = value
            End Set
        End Property
        
        '''<remarks/>
        Public Property Type() As ApplicationArtifactType
            Get
                Return Me.typeField
            End Get
            Set
                Me.typeField = value
            End Set
        End Property
        
        '''<remarks/>
        Public Property ParentArtifact() As ApplicationArtifact
            Get
                Return Me.parentArtifactField
            End Get
            Set
                Me.parentArtifactField = value
            End Set
        End Property
        
        '''<remarks/>
        Public Property ConflictArtifact() As ApplicationArtifact
            Get
                Return Me.conflictArtifactField
            End Get
            Set
                Me.conflictArtifactField = value
            End Set
        End Property
        
        '''<remarks/>
        Public Property Status() As StatusCode
            Get
                Return Me.statusField
            End Get
            Set
                Me.statusField = value
            End Set
        End Property
        
        '''<remarks/>
        Public Property StatusMessage() As String
            Get
                Return Me.statusMessageField
            End Get
            Set
                Me.statusMessageField = value
            End Set
        End Property
        
        '''<remarks/>
        Public Property StatusDetails() As String
            Get
                Return Me.statusDetailsField
            End Get
            Set
                Me.statusDetailsField = value
            End Set
        End Property
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.1"),  _
     System.SerializableAttribute(),  _
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.kCura.com/EDDS/TemplateManager")>  _
    Public Enum ApplicationArtifactType
        
        '''<remarks/>
        Code
        
        '''<remarks/>
        Field
        
        '''<remarks/>
        Layout
        
        '''<remarks/>
        [Object]
        
        '''<remarks/>
        Rule
        
        '''<remarks/>
        Sync
        
        '''<remarks/>
        Tab
        
        '''<remarks/>
        View
        
        '''<remarks/>
        Script
    End Enum
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.1"),  _
     System.SerializableAttribute(),  _
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.kCura.com/EDDS/TemplateManager")>  _
    Public Enum StatusCode
        
        '''<remarks/>
        UnknownError
        
        '''<remarks/>
        Created
        
        '''<remarks/>
        Updated
        
        '''<remarks/>
        NameConflict
        
        '''<remarks/>
        FriendlyNameConflict
        
        '''<remarks/>
        SharedByLockedApp
        
        '''<remarks/>
        MultipleFileField
    End Enum
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.1"),  _
     System.SerializableAttribute(),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.kCura.com/EDDS/TemplateManager")>  _
    Partial Public Class ApplicationInstallationResult
        
        Private messageField As String
        
        Private detailsField As String
        
        Private newApplicationArtifactsField() As ApplicationArtifact
        
        Private successField As Boolean
        
        Private updatedApplicationArtifactsField() As ApplicationArtifact
        
        Private statusApplicationArtifactsField() As ApplicationArtifact
        
        Private totalWorkspacesField As Integer
        
        Private workspaceIDField As Integer
        
        Private workspaceNameField As String
        
        '''<remarks/>
        Public Property Message() As String
            Get
                Return Me.messageField
            End Get
            Set
                Me.messageField = value
            End Set
        End Property
        
        '''<remarks/>
        Public Property Details() As String
            Get
                Return Me.detailsField
            End Get
            Set
                Me.detailsField = value
            End Set
        End Property
        
        '''<remarks/>
        Public Property NewApplicationArtifacts() As ApplicationArtifact()
            Get
                Return Me.newApplicationArtifactsField
            End Get
            Set
                Me.newApplicationArtifactsField = value
            End Set
        End Property
        
        '''<remarks/>
        Public Property Success() As Boolean
            Get
                Return Me.successField
            End Get
            Set
                Me.successField = value
            End Set
        End Property
        
        '''<remarks/>
        Public Property UpdatedApplicationArtifacts() As ApplicationArtifact()
            Get
                Return Me.updatedApplicationArtifactsField
            End Get
            Set
                Me.updatedApplicationArtifactsField = value
            End Set
        End Property
        
        '''<remarks/>
        Public Property StatusApplicationArtifacts() As ApplicationArtifact()
            Get
                Return Me.statusApplicationArtifactsField
            End Get
            Set
                Me.statusApplicationArtifactsField = value
            End Set
        End Property
        
        '''<remarks/>
        Public Property TotalWorkspaces() As Integer
            Get
                Return Me.totalWorkspacesField
            End Get
            Set
                Me.totalWorkspacesField = value
            End Set
        End Property
        
        '''<remarks/>
        Public Property WorkspaceID() As Integer
            Get
                Return Me.workspaceIDField
            End Get
            Set
                Me.workspaceIDField = value
            End Set
        End Property
        
        '''<remarks/>
        Public Property WorkspaceName() As String
            Get
                Return Me.workspaceNameField
            End Get
            Set
                Me.workspaceNameField = value
            End Set
        End Property
    End Class
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")>  _
    Public Delegate Sub InstallTemplateCompletedEventHandler(ByVal sender As Object, ByVal e As InstallTemplateCompletedEventArgs)
    
    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1"),  _
     System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code")>  _
    Partial Public Class InstallTemplateCompletedEventArgs
        Inherits System.ComponentModel.AsyncCompletedEventArgs
        
        Private results() As Object
        
        Friend Sub New(ByVal results() As Object, ByVal exception As System.Exception, ByVal cancelled As Boolean, ByVal userState As Object)
            MyBase.New(exception, cancelled, userState)
            Me.results = results
        End Sub
        
        '''<remarks/>
        Public ReadOnly Property Result() As ApplicationInstallationResult
            Get
                Me.RaiseExceptionIfNecessary
                Return CType(Me.results(0),ApplicationInstallationResult)
            End Get
        End Property
    End Class
End Namespace
