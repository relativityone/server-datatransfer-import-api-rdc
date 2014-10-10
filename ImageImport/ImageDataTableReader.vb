Namespace kCura.WinEDDS.ImportExtension
	Public Class ImageDataTableReader
		Implements kCura.WinEDDS.Api.IImageReader
		Private _currentRecordNumber As Int32 = -1
		Private _source As System.Data.DataTable
		Public Sub AdvanceRecord() Implements Api.IImageReader.AdvanceRecord
			_currentRecordNumber += 1
		End Sub

		Public Sub New(ByVal dataSource As System.Data.DataTable)
			_source = dataSource
			_source.DefaultView.Sort = "DocumentIdentifier ASC"
		End Sub

		Public Sub Cancel() Implements Api.IImageReader.Cancel

		End Sub

		Public Sub Close() Implements Api.IImageReader.Close

		End Sub

		Public Function CountRecords() As Long Implements Api.IImageReader.CountRecords
			Return _source.Rows.Count
		End Function

		Public ReadOnly Property CurrentRecordNumber() As Integer Implements Api.IImageReader.CurrentRecordNumber
			Get
				Return _currentRecordNumber
			End Get
		End Property

		Public Function GetImageRecord() As Api.ImageRecord Implements Api.IImageReader.GetImageRecord
			Dim row As System.Data.DataRow = _source.Rows(_currentRecordNumber)
			Dim retval As New Api.ImageRecord
			retval.BatesNumber = row("BatesNumber").ToString
			retval.FileLocation = row("FileLocation").ToString
			If _currentRecordNumber = 0 Then
				retval.IsNewDoc = True
			Else
				retval.IsNewDoc = Not (row("DocumentIdentifier").ToString = _source.Rows(_currentRecordNumber - 1)("DocumentIdentifier").ToString)
			End If
			_currentRecordNumber = _currentRecordNumber + 1
			Return retval
		End Function

		Public ReadOnly Property HasMoreRecords() As Boolean Implements Api.IImageReader.HasMoreRecords
			Get
				Return Not _currentRecordNumber > _source.Rows.Count - 1
			End Get
		End Property

		Public Sub Initialize() Implements Api.IImageReader.Initialize
		End Sub
	End Class
End Namespace

