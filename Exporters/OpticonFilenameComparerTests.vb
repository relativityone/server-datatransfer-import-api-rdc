Imports kCura.WinEDDS.Exporters
Imports NSubstitute
Imports NUnit.Framework
Imports Relativity.Logging

Namespace kCura.WinEDDS.NUnit.Exporters
    <TestFixture> Public Class OpticonFilenameComparerTests

        Private ReadOnly _pageLinesDoc2 As List(Of String) = New List(Of String) From {
            "TEST0000002,VOL01,.\VOL01\IMAGES\IMG001\TEST0000002.tif,Y,,,19",
            "TEST0000002,VOL01,.\VOL01\IMAGES\IMG001\TEST0000002_2.tif,,,,",
            "TEST0000002,VOL01,.\VOL01\IMAGES\IMG001\TEST0000002_10.tif,,,,"
        }

        Private ReadOnly _pageLinesDoc4 As List(Of String) = New List(Of String) From {
            "TEST0000004,VOL01,.\VOL01\IMAGES\IMG001\TEST0000004.tif,Y,,,123",
            "TEST0000004,VOL01,.\VOL01\IMAGES\IMG001\TEST0000004_2.tif,,,,",
            "TEST0000004,VOL01,.\VOL01\IMAGES\IMG001\TEST0000004_3.tif,,,,",
            "TEST0000004,VOL01,.\VOL01\IMAGES\IMG001\TEST0000004_10.tif,,,,",
            "TEST0000004,VOL01,.\VOL01\IMAGES\IMG001\TEST0000004_15.tif,,,,",
            "TEST0000004,VOL01,.\VOL01\IMAGES\IMG001\TEST0000004_100.tif,,,,",
            "TEST0000004,VOL01,.\VOL01\IMAGES\IMG001\TEST0000004_110.tif,,,,"
        }

		Private ReadOnly _malformedPageLines As List(Of String) = New List(Of String) From {
			"TEST0000005,VOL01,.\VOL01\IMAGES\IMG001\TEST0000004_1.tif,Y,,",
			"TEST0000005,VOL01,.\VOL01\IMAGES\IMG001\TEST0000004_2.tif,,",
			"TEST0000005,VOL01,.\VOL01\IMAGES\IMG001\TEST0000004_3.tif,",
			"TEST0000005,VOL01,.\VOL01\IMAGES\IMG001\TEST0000004_4.tif"
		}

        Private _logger As ILog
        Private _comparer As OpticonFilenameComparer

        <SetUp> Public Sub SetUp()
			_logger = Substitute.For(Of ILog)
            _comparer = New OpticonFilenameComparer(_logger)
        End Sub

        <Test> Public Sub OneDocumentPagesAreSortedCorrectly()
            For i As Integer = 0 To _pageLinesDoc4.Count - 1
                For j As Integer = 0 To _pageLinesDoc4.Count - 1
                    Dim comparisonResult As Integer = _comparer.Compare(_pageLinesDoc4(i), _pageLinesDoc4(j))
                    Dim message As String = $"{_pageLinesDoc4(i)} <-> {_pageLinesDoc4(j)}, i: {i}, j: {j}, result: {comparisonResult}"
                    If i = j Then
                        Assert.AreEqual(0, comparisonResult, message)
                    ElseIf i < j Then
                        Assert.Less(comparisonResult, 0, message)
                    Else
                        Assert.Greater(comparisonResult, 0, message)
                    End If
                Next
            Next
			_logger.DidNotReceive().LogError(Arg.Any(Of String), Arg.Any(Of Object()))
        End Sub

        <Test> Public Sub DifferentDocumentPagesAreSortedCorrectly()
            For i As Integer = 0 To _pageLinesDoc2.Count - 1
                For j As Integer = 0 To _pageLinesDoc4.Count - 1
					Dim comparisonResult As Integer = _comparer.Compare(_pageLinesDoc2(i), _pageLinesDoc4(j))
					Dim message As String = $"{_pageLinesDoc2(i)} <-> {_pageLinesDoc4(j)}, i: {i}, j: {j}, result: {comparisonResult}"
					Assert.Less(comparisonResult, 0, message)
                Next
            Next
	        _logger.DidNotReceive().LogError(Arg.Any(Of String), Arg.Any(Of Object()))
        End Sub

		<Test> Public Sub MalformedDocumentPagesAreSortedCorrectlyAndErrorIsLogged()
			For i As Integer = 0 To _malformedPageLines.Count - 1
				For j As Integer = 0 To _malformedPageLines.Count - 1
					Dim comparisonResult As Integer = _comparer.Compare(_malformedPageLines(i), _malformedPageLines(j))
					Dim message As String = $"{_malformedPageLines(i)} <-> {_malformedPageLines(j)}, i: {i}, j: {j}, result: {comparisonResult}"
					If i = j Then
						Assert.AreEqual(0, comparisonResult, message)
					ElseIf i < j Then
						Assert.Less(comparisonResult, 0, message)
					Else
						Assert.Greater(comparisonResult, 0, message)
					End If
				Next
			Next
			_logger.Received(2 * _malformedPageLines.Count * _malformedPageLines.Count).LogError(OpticonFilenameComparer.InvalidFileColumnCount, Arg.Any(Of Object()))
		End Sub

    End Class
End Namespace