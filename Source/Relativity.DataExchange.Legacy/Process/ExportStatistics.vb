Namespace kCura.WinEDDS
	Public Class ExportStatistics
		Inherits Statistics

		''' <summary>
		''' Converts this instance into a dictionary containing limited name/value pairs.
		''' </summary>
		''' <remarks>
		''' The NV pairs are displayed in the RDC. Do NOT modify unless you intend to change the RDC display!
		''' </remarks>
		''' <returns>
		''' The <see cref="IDictionary"/> instance.
		''' </returns>
		Public Overrides Function ToDictionaryForProgress() As IDictionary
			Dim retval As New System.Collections.Specialized.HybridDictionary
			If Not Me.FileTransferDuration.Equals(TimeSpan.Zero) Then
				retval.Add("Average file transfer rate", ToFileSizeSpecification(Me.FileTransferredBytes / Me.FileTransferDuration.TotalSeconds) & "/sec")
			End If

			If Not Me.MetadataTransferDuration.Equals(TimeSpan.Zero) AndAlso Not Me.MetadataTransferredBytes = 0 Then
				retval.Add("Average long text transfer rate", ToFileSizeSpecification(Me.MetadataTransferredBytes / Me.MetadataTransferDuration.TotalHours) & "/hr")
			End If

			Return retval
		End Function

		''' <summary>
		''' Converts this instance into a dictionary containing relevant name/value pairs for logging purposes.
		''' </summary>
		''' <returns>
		''' The <see cref="IDictionary"/> instance.
		''' </returns>
		Public Overrides Function ToDictionaryForLogs() As System.Collections.Generic.IDictionary(Of String, Object)

			' Note: Removed all mass import NV pairs.
			Dim statisticsDict As System.Collections.Generic.IDictionary(Of String, Object) = MyBase.ToDictionaryForLogs()

			Dim pair As DictionaryEntry
			For Each pair In Me.ToDictionaryForProgress()
				statisticsDict.Add(pair.Key.ToString(), pair.Value)
			Next

			Return statisticsDict
		End Function
	End Class
End Namespace