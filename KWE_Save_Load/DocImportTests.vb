Imports NUnit.Framework
Imports kCura.WinEDDS.NUnit.TestObjectFactories

Namespace kCura.WinEDDS.NUnit
	<TestFixture()> Public Class DocImportTests

		Protected Property QueryFieldFactory As QueryFieldFactory = New QueryFieldFactory
		Public Const CORRECT_FOLDER_PATH As String = "\ValidExport\Location\"

		'Enums are being serialized as their underlying ints; ensure that existing enum ids aren't being changed

#Region "OverlayBehavior serialization"
		<Test()> Public Sub OverlayBehavior_FieldOverlayBehavior_MergeAll()
			Assert.AreEqual(CInt(kCura.WinEDDS.LoadFile.FieldOverlayBehavior.MergeAll), 1)
		End Sub

		<Test()> Public Sub OverlayBehavior_FieldOverlayBehavior_ReplaceAll()
			Assert.AreEqual(CInt(kCura.WinEDDS.LoadFile.FieldOverlayBehavior.ReplaceAll), 2)
		End Sub

		<Test()> Public Sub OverlayBehavior_FieldOverlayBehavior_UseRelativityDefaults()
			Assert.AreEqual(CInt(kCura.WinEDDS.LoadFile.FieldOverlayBehavior.UseRelativityDefaults), 0)
		End Sub

#End Region
	End Class
End Namespace