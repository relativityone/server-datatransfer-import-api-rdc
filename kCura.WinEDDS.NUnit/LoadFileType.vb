Imports NUnit.Framework
Namespace kCura.WinEDDS.NUnit
	<TestFixture()> Public Class LoadFileType
		<Test()> Public Sub FileFormat_Opticon()
			Assert.AreEqual(CInt(kCura.WinEDDS.LoadFileType.FileFormat.Opticon), 0)
		End Sub

		<Test()> Public Sub FileFormat_Ipro()
			Assert.AreEqual(CInt(kCura.WinEDDS.LoadFileType.FileFormat.IPRO), 1)
		End Sub

		<Test()> Public Sub FileFormat_IproFullText()
			Assert.AreEqual(CInt(kCura.WinEDDS.LoadFileType.FileFormat.IPRO_FullText), 2)
		End Sub

		<Test()> Public Sub FileFormat_Range()
			Dim min, max As Int32?
			Dim count As Int32 = 0
			For Each e As kCura.WinEDDS.LoadFileType.FileFormat In System.Enum.GetValues(GetType(kCura.WinEDDS.LoadFileType.FileFormat))
				If Not min.HasValue Then min = CInt(e)
				If Not max.HasValue Then max = CInt(e)
				min = System.Math.Min(min.Value, CInt(e))
				max = System.Math.Max(max.Value, CInt(e))
				count += 1
			Next
			Assert.AreEqual(3, count)
			Assert.AreEqual(2, max, "All image load file formats should be tested")
			Assert.AreEqual(0, min, "All image load file formats should be tested")
		End Sub


	End Class
End Namespace