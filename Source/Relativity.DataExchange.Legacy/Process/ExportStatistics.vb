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
		Public Overrides Function ToDictionary() As IDictionary
			Dim retval As New System.Collections.Specialized.HybridDictionary
			If Not Me.FileTime = 0 Then
				retval.Add("Average file transfer rate", ToFileSizeSpecification(Me.FileBytes / TimeSpan.FromTicks(Me.FileTime).TotalSeconds) & "/sec")
			End If

			If Not Me.MetadataTime = 0 AndAlso Not Me.MetadataBytes = 0 Then
				retval.Add("Average long text transfer rate", ToFileSizeSpecification(Me.MetadataBytes / TimeSpan.FromTicks(Me.MetadataTime).TotalHours) & "/hr")
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
			Dim statisticsDict As System.Collections.Generic.Dictionary(Of String, Object) = New System.Collections.Generic.Dictionary(Of String, Object) From
				    {
					    {BatchCountKey, Me.BatchCount},
					    {DocsCountKey, Me.DocCount},
					    {MetadataBytesKey, Me.MetadataBytes},
					    {MetadataFilesTransferredKey, Me.MetadataFilesTransferredCount},
					    {MetadataThroughputKey, Me.MetadataThroughput},
					    {MetadataTimeKey, TimeSpan.FromTicks(Me.MetadataTime)},
					    {NativeFileBytesKey, Me.FileBytes},
					    {NativeFileThroughputKey, Me.FileThroughput},
					    {NativeFileTimeKey, TimeSpan.FromTicks(Me.FileTime)},
					    {NativeFilesTransferredKey, Me.NativeFilesTransferredCount}
				    }

			Dim pair As DictionaryEntry
			For Each pair In Me.ToDictionary()
				statisticsDict.Add(pair.Key.ToString(), pair.Value)
			Next

			Return statisticsDict
		End Function
	End Class
End Namespace