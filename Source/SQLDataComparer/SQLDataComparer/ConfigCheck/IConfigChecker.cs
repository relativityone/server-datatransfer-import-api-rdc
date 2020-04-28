using SQLDataComparer.Config;

namespace SQLDataComparer.ConfigCheck
{
	public interface IConfigChecker
	{
		bool CheckTableConfig(TableConfig table);
	}
}
