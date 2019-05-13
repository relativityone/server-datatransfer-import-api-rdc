Namespace kCura.WinEDDS.Exporters
	Public Class VolumePredictions
		Public Property NativeFileCount As Long = 0
		Public Property ImageFileCount As Long = 0
		Public Property TextFileCount As Long = 0

		Public Property NativeFilesSize As Long = 0
		Public Property ImageFilesSize As Long = 0
		Public Property TextFilesSize As Long = 0

		Public ReadOnly Property TotalFileSize As Long
			Get
				Return NativeFilesSize + ImageFilesSize + TextFilesSize
			End Get
		End Property
	End Class
End Namespace