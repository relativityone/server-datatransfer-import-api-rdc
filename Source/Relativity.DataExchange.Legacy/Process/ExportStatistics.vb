Namespace kCura.WinEDDS
	Public Class ExportStatistics
		Inherits Statistics
		Public Overrides Function ToDictionary() As IDictionary
			Dim retval As New System.Collections.Specialized.HybridDictionary
			If Not Me.FileTime = 0 Then retval.Add("Average file transfer rate", ToFileSizeSpecification(Me.FileBytes / (Me.FileTime / TimeSpan.TicksPerSecond)) & "/sec")
			If Not Me.MetadataTime = 0 AndAlso Not Me.MetadataBytes = 0 Then retval.Add("Average metadata transfer rate (includes SQL processing)", ToFileSizeSpecification(Me.MetadataBytes / (Me.MetadataTime / TimeSpan.TicksPerSecond)) & "/sec")
			Return retval
		End Function
	End Class
End Namespace
