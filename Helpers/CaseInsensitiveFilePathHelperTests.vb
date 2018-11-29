Imports kCura.WinEDDS.Helpers
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions
Imports NUnit.Framework

Namespace kCura.WinEDDS.NUnit.Helpers
	<TestFixture>
	Public Class CaseInsensitiveFilePathHelperTests
		Private _filePathHelper As CaseSensitiveFilePathHelper
		Private _systemIoMock As ISystemIoFileWrapper

		Private Const _SAMPLE_PATH As String = "\\dir\somePath.ext"

		<SetUp> Public Sub SetUp()
			_systemIoMock = Substitute.For(Of ISystemIoFileWrapper)()
			_systemIoMock.GetExtension(Arg.Any(Of String)).ReturnsForAnyArgs(function(info) System.IO.Path.GetExtension(info.Arg(Of String)()))
			_systemIoMock.ChangeExtension(Arg.Any(Of String), Arg.Any(Of String)).ReturnsForAnyArgs(function(info) System.IO.Path.ChangeExtension(info.ArgAt(Of String)(0), info.ArgAt(Of String)(1)))

			_filePathHelper = New CaseSensitiveFilePathHelper(_systemIoMock)
		End Sub

		<Test>
		Public Sub ShouldThrowWhenArgumentIsNothing()
			Assert.Throws(Of ArgumentException)(sub() _filePathHelper.GetExistingFilePath(Nothing))
		End Sub

		<Test>
		Public Sub ShouldThrowWhenArgumentIsEmpty()
			Assert.Throws(Of ArgumentException)(sub() _filePathHelper.GetExistingFilePath(String.Empty))
		End Sub

		<Test>
		Public Sub ShouldReturnSamePathIfOriginalOneExists()
			_systemIoMock.Exists(_SAMPLE_PATH).Returns(True)

			Dim actual As String = _filePathHelper.GetExistingFilePath(_SAMPLE_PATH)

			Assert.AreEqual(actual, _SAMPLE_PATH)
		End Sub


		<Test>
		Public Sub ShouldReturnNothingWhenOriginalDoesNotExist()
			_systemIoMock.Exists(_SAMPLE_PATH).Returns(False)

			Dim actual As String = _filePathHelper.GetExistingFilePath(_SAMPLE_PATH)

			Assert.IsNull(actual)
		End Sub
	End Class
End NameSpace