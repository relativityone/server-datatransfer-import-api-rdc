Namespace kCura.WinEDDS

	''' <summary>
	''' A manager class that provides a functionality to capture time and report duration.
	''' </summary>
	Public Interface ITimeKeeperManager

		''' <summary>
		''' Creates a time keeper to which manage duration logs of a given event.
		''' </summary>
		''' <param name="eventKey">The string that represents the capturing event.</param>
		''' <returns>A time keeper object</returns>
		Function CaptureTime(eventKey As String) As IImportTimeKeeper

		''' <summary>
		''' Generates a CSV report of files being imported to the specified directory, with individual items represented as columns.
		''' </summary>
		''' <param name="filenameSuffix">The suffix with which to look up files.</param>
		''' <param name="directory">The directory in which to store the result.</param>
		Sub GenerateCsvReportItemsAsRows(filenameSuffix As String, directory As String)

	End Interface

End Namespace