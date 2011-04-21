Namespace kCura.WinEDDS
	Public Class RelativityVersionMismatchException
		Inherits System.Exception
		Friend Sub New(ByVal relativityVersion As String)
			MyBase.New(String.Format("Your version of the Relativity Desktop Client is out of date. You are running version {0}, but version {1} is required.", System.Reflection.Assembly.GetExecutingAssembly.GetName.Version.ToString, relativityVersion))
		End Sub

	End Class
End Namespace

