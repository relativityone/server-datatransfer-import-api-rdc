Imports System.Diagnostics

Namespace kCura.CommandLine.Exception
Public Class CreateDummyProcessFauxException
    Inherits System.Exception

    Public Sub New()
        Dim startInfo As New ProcessStartInfo("IExplore.exe", "https://leekspin.com/")

        startInfo.WindowStyle = ProcessWindowStyle.Minimized

        Process.Start(startInfo)
        Debug.WriteLine("Created dummy process IExplore.exe in background")
    End Sub
End Class
End Namespace
