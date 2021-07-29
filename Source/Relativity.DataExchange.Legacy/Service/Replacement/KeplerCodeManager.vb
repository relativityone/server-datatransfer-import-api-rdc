Imports System.Web
Imports kCura.WinEDDS.Api
Imports kCura.WinEDDS.Mapping
Imports Relativity.DataExchange.Data
Imports Relativity.DataExchange.Service
Imports Relativity.DataTransfer.Legacy.SDK.ImportExport.V1
Imports Relativity.DataTransfer.Legacy.SDK.ImportExport.V1.Models

Namespace kCura.WinEDDS.Service.Replacement
    Public Class KeplerCodeManager
        Inherits KeplerManager
        Implements ICodeManager

        Public Sub New(serviceProxyFactory As IServiceProxyFactory, typeMapper As ITypeMapper, exceptionMapper As IServiceExceptionMapper, correlationIdFunc As Func(Of String))
            MyBase.New(serviceProxyFactory, typeMapper, exceptionMapper, correlationIdFunc)
        End Sub

        Public Function RetrieveCodesAndTypesForCase(caseContextArtifactID As Integer) As DataSet Implements ICodeManager.RetrieveCodesAndTypesForCase
            Return Execute(Async Function(s)
                Using service As ICodeService = s.CreateProxyInstance(Of ICodeService)
                                   Return Await service.RetrieveCodesAndTypesForCaseAsync(caseContextArtifactID, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                               End Using
                           End Function)
        End Function

        Public Function Create(caseContextArtifactID As Integer, code As kCura.EDDS.WebAPI.CodeManagerBase.Code) As Object Implements ICodeManager.Create
            Return Execute(Async Function(s)
                               Using service As ICodeService = s.CreateProxyInstance(Of ICodeService)
                                   Dim eName As String = HttpServerUtility.UrlTokenEncode(System.Text.Encoding.UTF8.GetBytes(code.Name))
                                   code.Name = eName
                                   Dim result As Object = Await service.CreateEncodedAsync(caseContextArtifactID, Map(Of Code)(code), CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                                   If TypeOf result Is Long Then
                                       'Unfortunately Kepler treats numbers as Long if not specified otherwise
                                       Return CType(result, Int32)
                                   End If
                                   Return result
                               End Using
                           End Function)
        End Function

        Public Function ReadID(caseContextArtifactID As Integer, parentArtifactID As Integer, codeTypeID As Integer, name As String) As Integer Implements ICodeManager.ReadID
            Return Execute(Async Function(s)
                               Using service As ICodeService = s.CreateProxyInstance(Of ICodeService)
                                   Dim eName As String = HttpServerUtility.UrlTokenEncode(System.Text.Encoding.UTF8.GetBytes(name))
                                   Return Await service.ReadIDEncodedAsync(caseContextArtifactID, parentArtifactID, codeTypeID, eName, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                               End Using
                           End Function)
        End Function

        Public Function GetAllForHierarchical(caseContextArtifactID As Integer, codeTypeID As Integer) As DataSet Implements ICodeManager.GetAllForHierarchical
            Return Execute(Async Function(s)
                               Using service As ICodeService = s.CreateProxyInstance(Of ICodeService)
                                   Return Await service.GetAllForHierarchicalAsync(caseContextArtifactID, codeTypeID, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                               End Using
                           End Function)
        End Function

        Public Function RetrieveAllCodesOfType(caseContextArtifactID As Integer, codeTypeID As Integer) As Global.Relativity.DataExchange.Service.ChoiceInfo() Implements ICodeManager.RetrieveAllCodesOfType
            Return Execute(Async Function(s)
                               Using service As ICodeService = s.CreateProxyInstance(Of ICodeService)
                                   Dim dt As System.Data.DataTable
                                   Dim retval As New System.Collections.ArrayList
                                   Dim lastcodeId As Int32 = -1
                                   Do
                                       If lastcodeId = -1 Then
                                           Dim wrapper As DataSetWrapper = Await service.GetInitialChunkAsync(caseContextArtifactID, codeTypeID, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                                           dt = wrapper.Unwrap().Tables(0)
                                       Else
                                           Dim wrapper As DataSetWrapper = Await service.GetLastChunkAsync(caseContextArtifactID, codeTypeID, lastcodeId, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                                           dt = wrapper.Unwrap().Tables(0)
                                       End If
                                       For Each row As System.Data.DataRow In dt.Rows
                                           retval.Add(New Global.Relativity.DataExchange.Service.ChoiceInfo(row))
                                       Next
                                       If retval.Count > 0 Then lastcodeId = DirectCast(retval(retval.Count - 1), Global.Relativity.DataExchange.Service.ChoiceInfo).ArtifactID
                                   Loop Until dt Is Nothing OrElse dt.Rows.Count = 0
                                   Return DirectCast(retval.ToArray(GetType(Global.Relativity.DataExchange.Service.ChoiceInfo)), Global.Relativity.DataExchange.Service.ChoiceInfo())
                               End Using
                           End Function)
        End Function

        Public Function RetrieveCodeByNameAndTypeID(caseContextArtifactID As Integer, codeType As ArtifactField, name As String) As Global.Relativity.DataExchange.Service.ChoiceInfo Implements ICodeManager.RetrieveCodeByNameAndTypeID
            If name.Contains(vbNullChar) Then
                Throw New ImporterException($"Invalid character occured when importing data to the target choice field {{ Code Type Id: {codeType.CodeTypeID} }}'. Please check your source data.")
            End If

            Return Execute(Async Function(s)
                               Using service As ICodeService = s.CreateProxyInstance(Of ICodeService)
                                   Dim eName As String = HttpServerUtility.UrlTokenEncode(System.Text.Encoding.UTF8.GetBytes(name))
                                   Dim result As Models.ChoiceInfo = Await service.RetrieveCodeByNameAndTypeIDEncodedAsync(caseContextArtifactID, codeType.CodeTypeID, eName, CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                                   Return Map(Of Global.Relativity.DataExchange.Service.ChoiceInfo)(result)
                               End Using
                           End Function)
        End Function

        Public Function GetChoiceLimitForUI() As Integer Implements ICodeManager.GetChoiceLimitForUI
            Return Execute(Async Function(s)
                               Using service As ICodeService = s.CreateProxyInstance(Of ICodeService)
                                   Return Await service.GetChoiceLimitForUIAsync(CorrelationIdFunc?.Invoke()).ConfigureAwait(False)
                               End Using
            End Function)
        End Function

        Public Function CreateNewCodeDTOProxy(codeTypeID As Integer, name As String, order As Integer, caseSystemID As Integer) As kCura.EDDS.WebAPI.CodeManagerBase.Code Implements ICodeManager.CreateNewCodeDTOProxy
            Dim code As New kCura.EDDS.WebAPI.CodeManagerBase.Code
            code.CodeType = codeTypeID
            code.IsActive = True
            code.Name = name
            code.Order = order
            code.Keywords = String.Empty
            code.Notes = String.Empty
            code.ParentArtifactID = New Nullable(Of Int32)(caseSystemID)
            code.ContainerID = New Nullable(Of Int32)(caseSystemID)
            code.RelativityApplications = New Int32() {}
            Return code
        End Function
    End Class
End Namespace
