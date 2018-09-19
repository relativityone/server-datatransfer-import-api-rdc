Imports kCura.WinEDDS.Exporters
Imports NUnit.Framework

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

        Private _comparer As OpticonFilenameComparer

        <SetUp> Public Sub SetUp()
            _comparer = New OpticonFilenameComparer()
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
        End Sub

        <Test> Public Sub DifferentDocumentPagesAreSortedCorrectly()
            For i As Integer = 0 To _pageLinesDoc2.Count - 1
                For j As Integer = 0 To _pageLinesDoc4.Count - 1
					Dim comparisonResult As Integer = _comparer.Compare(_pageLinesDoc2(i), _pageLinesDoc4(j))
					Dim message As String = $"{_pageLinesDoc2(i)} <-> {_pageLinesDoc4(j)}, i: {i}, j: {j}, result: {comparisonResult}"
					Assert.Less(comparisonResult, 0, message)
                Next
            Next
        End Sub

    End Class
End Namespace