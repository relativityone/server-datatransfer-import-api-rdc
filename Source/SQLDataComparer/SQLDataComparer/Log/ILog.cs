using System.Data.SqlClient;

namespace SQLDataComparer.Log
{
	public interface ILog
	{
		void LogQuery(string query, SqlParameterCollection parameters);
		void LogInfo(string message);
		void LogWarning(string message);
		void LogError(string message);
	}
}
