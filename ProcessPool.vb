Namespace kCura.Windows.Process
	Public Class ProcessPool

		'Private _threadCollection As ArrayList
		Private _threadCollection As Hashtable

		Public Function StartProcess(ByVal process As ProcessBase) As System.Guid
			Dim key As Guid = Guid.NewGuid
			Dim thread As System.Threading.Thread
			process.ProcessID = key
			thread = New System.Threading.Thread(AddressOf process.StartProcess)
			thread.Start()
			_threadCollection.Add(key, thread)
			Return key
		End Function

		Public Sub AbortProcess(ByVal processGuid As System.Guid)
			Dim thread As System.Threading.Thread = CType(_threadCollection(processGuid), System.Threading.Thread)
			thread.Abort()
			' TODO: this will not work, need to use hashtable instead
			_threadCollection.Remove(processGuid)
		End Sub

		Public Sub RemoveProcess(ByVal processGuid As System.Guid)
			Dim thread As System.Threading.Thread = CType(_threadCollection(processGuid), System.Threading.Thread)
			If thread.ThreadState <> Threading.ThreadState.Stopped Then
				thread.Abort()
			End If
			_threadCollection.Remove(processGuid)
		End Sub

		Public Sub New()
			_threadCollection = New Hashtable
		End Sub

	End Class

End Namespace