﻿' -----------------------------------------------------------------------------------------------------
' <copyright file="ExportNativeWithFilenameFrom.cs" company="Relativity ODA LLC">
'   © Relativity All Rights Reserved.
' </copyright>
' -----------------------------------------------------------------------------------------------------

Imports NUnit.Framework

Namespace Relativity.Import.Client.NUnit

	<TestFixture>
	Public Class ExportNativeWithFilenameFrom

		<Test>
		Public Sub ExportNativeWithFilenameFrom_Identifier()
			Assert.AreEqual(CInt(kCura.WinEDDS.ExportNativeWithFilenameFrom.Identifier), 0)
		End Sub

		<Test>
		Public Sub ExportNativeWithFilenameFrom_Production()
			Assert.AreEqual(CInt(kCura.WinEDDS.ExportNativeWithFilenameFrom.Production), 1)
		End Sub

		<Test>
		Public Sub ExportNativeWithFilenameFrom_Select()
			Assert.AreEqual(CInt(kCura.WinEDDS.ExportNativeWithFilenameFrom.Select), 2)
		End Sub

		<Test>
		Public Sub ExportNativeWithFilenameFrom_Range()
			Dim min As Int32? = Nothing
			Dim max As Int32? = Nothing
			Dim count As Int32 = 0
			For Each e As kCura.WinEDDS.ExportNativeWithFilenameFrom In System.Enum.GetValues(GetType(kCura.WinEDDS.ExportNativeWithFilenameFrom))
				If Not min.HasValue Then min = CInt(e)
				If Not max.HasValue Then max = CInt(e)
				min = System.Math.Min(min.Value, CInt(e))
				max = System.Math.Max(max.Value, CInt(e))
				count += 1
			Next
			Assert.AreEqual(4, count, "All native file name sources should be tested")
			Assert.AreEqual(3, max, "All native file name sources should be tested")
			Assert.AreEqual(0, min, "All native file name sources should be tested")
		End Sub
	End Class
End Namespace