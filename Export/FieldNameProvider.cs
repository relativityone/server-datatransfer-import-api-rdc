
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export
{
	public class FieldNameProvider : IFieldNameProvider
	{
		public virtual string GetDisplayName(ViewFieldInfo fieldInfo)
		{
			return fieldInfo.DisplayName;
		}
	}
}
