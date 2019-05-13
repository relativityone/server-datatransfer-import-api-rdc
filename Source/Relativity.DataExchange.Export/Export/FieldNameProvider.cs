namespace Relativity.DataExchange.Export
{
	using kCura.WinEDDS.Exporters;

	public class FieldNameProvider : IFieldNameProvider
	{
		public virtual string GetDisplayName(kCura.WinEDDS.ViewFieldInfo fieldInfo)
		{
			return fieldInfo.DisplayName;
		}
	}
}