Imports System.Threading
Imports System.Threading.Tasks
Imports Relativity.DataExchange

Namespace kCura.WinEDDS.Service.Replacement
    Public Class KeplerApplicationVersionService
        Implements IApplicationVersionService
        
        Private ReadOnly _applicationVersionService As ApplicationVersionService

        Public Sub New(instance As RelativityInstanceInfo, appSettings As IAppSettings, logger As Global.Relativity.Logging.ILog)
            _applicationVersionService = New ApplicationVersionService(instance, appSettings, logger)
        End Sub

        Public Function GetRelativityVersionAsync(token As CancellationToken) As Task(Of Version) Implements IApplicationVersionService.GetRelativityVersionAsync
            Return _applicationVersionService.GetRelativityVersionAsync(token)
        End Function

        Public Function GetImportExportWebApiVersionAsync(token As CancellationToken) As Task(Of Version) Implements IApplicationVersionService.GetImportExportWebApiVersionAsync
            Return Task.FromResult(System.Version.Parse("1.1.0.0"))
        End Function
    End Class
End Namespace
