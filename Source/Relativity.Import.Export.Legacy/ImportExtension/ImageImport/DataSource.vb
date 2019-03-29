Namespace kCura.WinEDDS.ImportExtension
	Public Class Datasource
		Implements kCura.WinEDDS.Api.IImageReader
		Private _currentRecordNumber As Int32 = -1

		Public Sub AdvanceRecord() Implements kCura.WinEDDS.Api.IImageReader.AdvanceRecord
			_currentRecordNumber += 1
		End Sub

		Public Sub Cancel() Implements kCura.WinEDDS.Api.IImageReader.Cancel

		End Sub

		Public Sub Close() Implements kCura.WinEDDS.Api.IImageReader.Close

		End Sub

		Public Function CountRecords() As Long Implements kCura.WinEDDS.Api.IImageReader.CountRecords
			Return 20
		End Function

		Public ReadOnly Property CurrentRecordNumber() As Integer Implements kCura.WinEDDS.Api.IImageReader.CurrentRecordNumber
			Get
				Return _currentRecordNumber
			End Get
		End Property

		Public Function GetImageRecord() As kCura.WinEDDS.Api.ImageRecord Implements kCura.WinEDDS.Api.IImageReader.GetImageRecord
			_currentRecordNumber += 1
			Dim retval As New kCura.WinEDDS.Api.ImageRecord
			retval.BatesNumber = "FAKELOL" + _currentRecordNumber.ToString.PadLeft(4, "0"c)
			retval.FileLocation = "\\Chiprodfs01\Testing\TestingData\Testing Load Files\Documents\Images\Common Images (151)\FED000001.tif"
			retval.IsNewDoc = (_currentRecordNumber - 1) Mod 5 = 0
			Return retval
		End Function

		Public ReadOnly Property HasMoreRecords() As Boolean Implements kCura.WinEDDS.Api.IImageReader.HasMoreRecords
			Get
				Return _currentRecordNumber < Me.CountRecords
			End Get
		End Property

		Public Sub Initialize() Implements kCura.WinEDDS.Api.IImageReader.Initialize
		End Sub
	End Class
End Namespace