Imports NUnit.Framework
Namespace kCura.WinEDDS.NUnit
	<TestFixture()> Public Class LoadFileImporter
		Private Helper As New LoadFileHelper

		<Test()> Public Sub Constructors()
			Console.WriteLine("Begin [LoadFileImporter] [Constructors] tests")
			Dim record As New TestMethodRecord
			record.TestDescription = "Constructor"
			Dim importer As New kCura.WinEDDS.LoadFileImporter(Helper.GetSampleLoadFileObjectIgnoreUploading, Nothing, 0, False)
			record.RunTest(True)
			Console.WriteLine("End [LoadFileImporter] [Constructors] tests")
			Console.WriteLine("")
		End Sub

		<Test()> Public Sub SetFieldValue()
			Console.WriteLine("Begin [LoadFileImporter] [SetFieldValue] tests")
			Dim record As New TestMethodRecord
			Dim importer As New kCura.WinEDDS.LoadFileImporter(Helper.GetSampleLoadFileObjectIgnoreUploading, Nothing, 0, False)
			Dim testFields As kCura.WinEDDS.DocumentField() = Helper.GetSampleSelectedFields()
			Dim field As kCura.WinEDDS.DocumentField
			record.TestDescription = "Generic Varchar - Correct"
			field = testFields(Helper.FieldDescription.VarCharGeneric)
			importer.SetFieldValue(field, "Test", -1)
			record.RunTextCompareTest("Test", field.Value)

			record.TestDescription = "Generic Date - Correct"
			field = testFields(Helper.FieldDescription.Date)
			importer.SetFieldValue(field, "3/3/2005 12:00PM", -1)
			record.RunDateTimeCompareTest(DateTime.Parse("3/3/2005 12:00PM"), DateTime.Parse(field.Value))

			record.TestDescription = "Generic Currency - Correct"
			field = testFields(Helper.FieldDescription.Currency)
			importer.SetFieldValue(field, "12.50", -1)
			record.RunDecimalCompareTest(12.5D, Decimal.Parse(field.Value))

			record.TestDescription = "Generic decimal - Correct"
			field = testFields(Helper.FieldDescription.Decimal)
			importer.SetFieldValue(field, "10.77", -1)
			record.RunDecimalCompareTest(10.77D, Decimal.Parse(field.Value))

			record.TestDescription = "Generic Integer - Correct"
			field = testFields(Helper.FieldDescription.Integer)
			importer.SetFieldValue(field, "666", -1)
			record.RunIntegerCompareTest(666, Int32.Parse(field.Value))

			record.TestDescription = "Generic Text - Correct"
			field = testFields(Helper.FieldDescription.TextGeneric)
			importer.SetFieldValue(field, "Alpha Beta Gamma", -1)
			record.RunTextCompareTest("Alpha Beta Gamma", field.Value)

			record.TestDescription = "Generic Boolean - Correct"
			field = testFields(Helper.FieldDescription.Boolean)
			importer.SetFieldValue(field, "True", -1)
			record.RunTest(Boolean.Parse(field.Value))

			record.TestDescription = "Generic Code - Correct"
			field = testFields(Helper.FieldDescription.Code)
			importer.AllCodeTypes = New kCura.Data.DataView(Helper.GenerateSampleCodeTypeDataTable)
			importer.AllCodes = New kCura.Data.DataView(Helper.GenerateSampleCodeDataTable)
			importer.SetFieldValue(field, "Alpha", -1)
			record.RunTextCompareTest("401", field.Value)

			record.TestDescription = "Generic MultiCode Select One - Correct"
			field = testFields(Helper.FieldDescription.MultiCode)
			importer.AllCodeTypes = New kCura.Data.DataView(Helper.GenerateSampleCodeTypeDataTable)
			importer.AllCodes = New kCura.Data.DataView(Helper.GenerateSampleCodeDataTable)
			importer.SetFieldValue(field, "uruz", -1)
			record.RunTextCompareTest("408", field.Value)

			record.TestDescription = "Generic MultiCode Select Two - Correct"
			field = testFields(Helper.FieldDescription.MultiCode)
			importer.AllCodeTypes = New kCura.Data.DataView(Helper.GenerateSampleCodeTypeDataTable)
			importer.AllCodes = New kCura.Data.DataView(Helper.GenerateSampleCodeDataTable)
			importer.SetFieldValue(field, "fehu;uruz", -1)
			record.RunTextCompareTest("407;408", field.Value)

			record.TestDescription = "Generic MultiCode Select Three - Correct"
			field = testFields(Helper.FieldDescription.MultiCode)
			importer.AllCodeTypes = New kCura.Data.DataView(Helper.GenerateSampleCodeTypeDataTable)
			importer.AllCodes = New kCura.Data.DataView(Helper.GenerateSampleCodeDataTable)
			importer.SetFieldValue(field, "fehu;uruz;thurisaz", -1)
			record.RunTextCompareTest("407;408;409", field.Value)
			Console.WriteLine("End [LoadFileImporter] [SetFieldValue] tests")
			Console.WriteLine("")
		End Sub
	End Class
End Namespace