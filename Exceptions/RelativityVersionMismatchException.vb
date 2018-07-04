Namespace kCura.WinEDDS
	Public Class RelativityVersionMismatchException
		Inherits System.Exception
        Public Sub New(ByVal relativityVersion As String)
            MyBase.New(String.Format("Your version of the Relativity Desktop Client ({0}) is out of date. Please make sure you are running correct RDC version ({1}) or specified correct WebService URL for Relativity.", System.Reflection.Assembly.GetExecutingAssembly.GetName.Version.ToString, relativityVersion))
        End Sub

    End Class
End Namespace

