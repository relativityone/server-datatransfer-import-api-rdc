﻿// -----------------------------------------------------------------------------------------------------
// <copyright file="NativeFileNameViewFieldsHelperTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Export.Client.NUnit
{
    using FileNaming.CustomFileNaming;

    using global::NUnit.Framework;

	using kCura.WinEDDS;
    using kCura.WinEDDS.FileNaming.CustomFileNaming;

    using Relativity.Import.Export.TestFramework;

    public class NativeFileNameViewFieldsHelperTests
	{
		private readonly INativeFileNameViewFieldsHelper _helper = new NativeFileNameViewFieldsHelper();

		[Test]
		public void ShouldPopulateSelectedNativesFieldInExportFileGivenGoodData()
		{
			ViewFieldInfo[] fields = { new QueryFieldFactory().GetArtifactIdField() };

			var part = new FirstFieldDescriptorPart(fields[0].FieldArtifactId);
			var model = new CustomFileNameDescriptorModel(part);

			var exportFile = new ExtendedExportFile(0)
			{
				AllExportableFields = fields,
				CustomFileNaming = model
			};

			_helper.PopulateNativeFileNameViewFields(exportFile);
			Assert.AreEqual("Artifact ID", exportFile.SelectedNativesNameViewFields[0].DisplayName);
		}

		[Test]
		public void ShouldPopulateFieldsInOrderTheyWereAdded()
		{
			const int numOfFields = 2;
			var qf = new QueryFieldFactory();
			ViewFieldInfo[] fields = { qf.GetExtractedTextField(), qf.GetArtifactIdField() };

			var field1 = new FirstFieldDescriptorPart(fields[0].FieldArtifactId);
			var part2 = new FieldDescriptorPart(fields[1].FieldArtifactId);
			var field2 = new ExtendedDescriptorPart(new SeparatorDescriptorPart("-"), part2);

			var model = new CustomFileNameDescriptorModel(field1, field2);

			var exportFile = new ExtendedExportFile(0)
			{
				AllExportableFields = fields,
				CustomFileNaming = model
			};

			_helper.PopulateNativeFileNameViewFields(exportFile);
			Assert.AreEqual(numOfFields, exportFile.SelectedNativesNameViewFields.Count);
			Assert.AreEqual("Extracted Text", exportFile.SelectedNativesNameViewFields[0].DisplayName);
			Assert.AreEqual("Artifact ID", exportFile.SelectedNativesNameViewFields[1].DisplayName);
		}

		[Test]
		public void ShouldLeaveEmptyListWhenCustomFileNamingIsDisabled()
		{
			var exportFile = new ExtendedExportFile(0);

			_helper.PopulateNativeFileNameViewFields(exportFile);
			Assert.AreEqual(0, exportFile.SelectedNativesNameViewFields.Count);
		}

		[Test]
		public void ShouldLeaveEmptyListWhenFieldDoesntExistInExportFile()
		{
			var qf = new QueryFieldFactory();
			ViewFieldInfo[] fields = { };

			var part = new FirstFieldDescriptorPart(qf.GetArtifactIdField().FieldArtifactId);
			var model = new CustomFileNameDescriptorModel(part);

			var exportFile = new ExtendedExportFile(0)
			{
				AllExportableFields = fields,
				CustomFileNaming = model
			};

			_helper.PopulateNativeFileNameViewFields(exportFile);
			Assert.AreEqual(0, exportFile.SelectedNativesNameViewFields.Count);
		}
	}
}