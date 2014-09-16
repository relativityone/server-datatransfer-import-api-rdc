Imports Rhino.Mocks
Imports NUnit.Framework
Imports kCura.WinEDDS.ImportExtension
Imports Relativity

Imports kCura.WinEDDS

Public Class DataReaderReaderFixture

#Region " Members "
	Dim _testObj As kCura.WinEDDS.Service.FieldQuery = Nothing
	Private _mockRepo As MockRepository = Nothing

	Private _stubReader As System.Data.IDataReader
	Private _stubFieldMap As kCura.WinEDDS.LoadFile


#End Region

#Region "Setup And Teardown"

	<SetUp()>
	Public Sub SetUp()

		_stubReader = MockRepository.GenerateStub(Of System.Data.IDataReader)()
		_stubReader.Stub(Function(dr) dr.IsClosed()).Return(False)
		_stubReader.Stub(Function(dr) dr.FieldCount()).Return(10)



	End Sub

	<TearDown()> Public Sub TearDown()
		_testObj = Nothing
		_stubReader = Nothing
		_mockRepo = Nothing

	End Sub

#End Region

	<TestCase()>
	Public Sub TestSetFieldValue()


		Dim _testReader As New StubDataReaderReader(New DataReaderReaderInitializationArgs(Nothing, 0), Nothing, _stubReader)

		' integer
		Dim field As New Api.ArtifactField("Test", 1, Relativity.FieldTypeHelper.FieldType.Integer, 0, 0, 20, 0, FieldInfo.StorageLocationChoice.SQL)
		_testReader.SetFieldValueTest(field, "12345")
		Assert.True(field.Value.Equals(12345))

		Try
			_testReader.SetFieldValueTest(field, "xyzzy")
			Assert.Fail()
		Catch ex As Exception
			Assert.True(TypeOf ex Is System.FormatException)
		End Try

		_testReader.SetFieldValueTest(field, " ")
		Assert.True(field.Value Is Nothing)

		' boolean
		field.Type = FieldTypeHelper.FieldType.Boolean
		_testReader.SetFieldValueTest(field, "true")
		Assert.True(field.Value.Equals(True))

		_testReader.SetFieldValueTest(field, "no")
		Assert.True(field.Value.Equals(False))

		_testReader.SetFieldValueTest(field, "")
		Assert.True(field.Value Is Nothing)

		' currency
		field.Type = FieldTypeHelper.FieldType.Currency
		_testReader.SetFieldValueTest(field, "109.37")
		Assert.True(field.Value.Equals(109.37D))

		Try
			_testReader.SetFieldValueTest(field, "xyzzy")
			Assert.Fail()
		Catch ex As Exception
			Assert.True(TypeOf ex Is System.FormatException)
		End Try

		_testReader.SetFieldValueTest(field, " ")
		Assert.True(field.Value Is Nothing)


		' date
		field.Type = FieldTypeHelper.FieldType.Date
		_testReader.SetFieldValueTest(field, "10/31/2005")
		Assert.True(field.Value.Equals(New DateTime(2005, 10, 31)))

		Try
			_testReader.SetFieldValueTest(field, "10312005")
			Assert.Fail()
		Catch Ex As Exception
			Assert.True(TypeOf Ex Is System.SystemException)
		End Try

		_testReader.SetFieldValueTest(field, "20051031")
		Assert.True(field.Value.Equals(New DateTime(2005, 10, 31)))

		_testReader.SetFieldValueTest(field, "Oct 31, 2005")
		Assert.True(field.Value.Equals(New DateTime(2005, 10, 31)))

		_testReader.SetFieldValueTest(field, "")
		Assert.True(field.Value Is Nothing)

		' decimal
		field.Type = FieldTypeHelper.FieldType.Decimal
		_testReader.SetFieldValueTest(field, "3.1415926")
		Assert.True(field.Value.Equals(3.1415926D))

		Try
			_testReader.SetFieldValueTest(field, "xyzzy")
			Assert.Fail()
		Catch ex As Exception
			Assert.True(TypeOf ex Is System.FormatException)
		End Try

		_testReader.SetFieldValueTest(field, "")
		Assert.True(field.Value Is Nothing)


	End Sub

	<Test()>
	Public Sub SetFieldValueInvokerProducesFieldValueImportExceptions()
		Dim idx As Integer = 13
		Dim inner As Exception = New Exception("throw from indexer")
		_stubReader.Stub(Function(r) r.Item(idx)).Throw(inner)

		Dim rdr As New StubDataReaderReader(New DataReaderReaderInitializationArgs(Nothing, 0), Nothing, _stubReader)

		Dim displayName As String = "theNameIsInteresting"
		Dim field As New Api.ArtifactField(displayName, 1, FieldTypeHelper.FieldType.Integer, 0, 0, 20, 0, FieldInfo.StorageLocationChoice.SQL)

		Dim lineNum As Long = 1313

		Try
			rdr.SetCurrentLine(lineNum)
			rdr.SetFieldValueInvokerForTesting(idx, field, displayName)
			Assert.Fail("Should not have gotten here: SetFieldValue() should have thrown.")
		Catch ex As kCura.WinEDDS.Exceptions.FieldValueImportException
			' note the linenum+1 because the datareaderreader is zero based, but the exceptions should be one based
			Assert.AreEqual(lineNum + 1, ex.RowNumber)
			Assert.AreEqual(displayName, ex.FieldName)
			Assert.AreSame(inner, ex.InnerException)
			Assert.AreEqual(String.Format("Error in row {0}, field ""{1}"".  {2}", lineNum + 1, displayName, inner.Message), ex.Message)
		End Try
	End Sub
End Class