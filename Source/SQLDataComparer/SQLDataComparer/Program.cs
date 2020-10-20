using System;

namespace SQLDataComparer
{
	class Program
	{
		static int Main(string[] args)
		{
			if (args.Length < 6)
			{
				Console.WriteLine("Usage: SQLDataComparer.exe sqlServer username password configFile leftDatabase rightDatabase");
				return 0;
			}

			string server = args[0];
			string username = args[1];
			string password = args[2];

			string compareConfigPath = args[3];
			string leftDatabase = args[4];
			string rightDatabase = args[5];

			var sqlDataComparer = new SqlDataComparer(server, username, password);
			bool areIdentical = sqlDataComparer.Run(compareConfigPath, leftDatabase, rightDatabase);
			return areIdentical ? 0 : 1;
		}
	}
}
