using Relativity;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata
{
	public static class FieldUtils
	{
		public static bool IsTextField(this FieldTypeHelper.FieldType fieldType)
		{
			return fieldType == FieldTypeHelper.FieldType.Text || fieldType == FieldTypeHelper.FieldType.OffTableText;
		}
	}
}