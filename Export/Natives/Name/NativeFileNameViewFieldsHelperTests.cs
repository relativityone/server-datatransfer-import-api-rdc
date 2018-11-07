using FileNaming.CustomFileNaming;
using kCura.WinEDDS.FileNaming.CustomFileNaming;
using kCura.WinEDDS.NUnit.TestObjectFactories;
using NUnit.Framework;

namespace kCura.WinEDDS.Core.NUnit.Export.Natives.Name
{
	public class NativeFileNameViewFieldsHelperTests
	{
		private readonly INativeFileNameViewFieldsHelper _helper = new NativeFileNameViewFieldsHelper();

		[Test]
		public void ShouldPopulateSelectedNativesFieldInExportFileGivenGoodData()
		{
			ViewFieldInfo[] fields = {new QueryFieldFactory().GetArtifactIdField()};

			var part = new FieldDescriptorPart(fields[0].FieldArtifactId);
			var model = new CustomFileNameDescriptorModel(part);

			var exportFile = new ExtendedExportFile(0)
			{
				AllExportableFields = fields,
				CustomFileNaming = model
			};

			_helper.PopulateNativeFileNameViewFields(exportFile);
			Assert.AreEqual("Artifact ID", exportFile.SelectedNativesNameViewFields[0].DisplayName);
		}
	}
}