using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using SQLDataComparer.Config;
using SQLDataComparer.ConfigCheck;
using SQLDataComparer.DataCompare;
using SQLDataComparer.DataLoad;
using SQLDataComparer.Log;
using SQLDataComparer.Model;

namespace SQLDataComparer
{
	public class SqlDataComparer
	{
		private readonly ConsoleLog _log;
		private readonly SqlConnectionStringBuilder _connectionStringBuilder;

		public SqlDataComparer(string server, string username, string password)
		{
			_log = new ConsoleLog(false);
			_connectionStringBuilder = new SqlConnectionStringBuilder
			{
				DataSource = server,
				UserID = username,
				Password = password,
				TrustServerCertificate = true,
				Encrypt = false,
				ApplicationIntent = ApplicationIntent.ReadWrite,
				MultiSubnetFailover = false,
				ConnectTimeout = 60,
			};
		}

		public bool Run(
			string compareConfigPath,
			string leftDatabase,
			string rightDatabase)
		{
			_connectionStringBuilder.InitialCatalog = leftDatabase;
			string leftDatabaseConnectionString = _connectionStringBuilder.ConnectionString;

			_connectionStringBuilder.InitialCatalog = rightDatabase;
			string rightDatabaseConnectionString = _connectionStringBuilder.ConnectionString;

			CompareConfig compareConfig;
			var xmlSerializer = new XmlSerializer(typeof(CompareConfig));
			using (var sr = File.OpenRead(compareConfigPath))
			{
				compareConfig = (CompareConfig)xmlSerializer.Deserialize(sr);
			}
			if (compareConfig?.TablesConfig == null || compareConfig.TablesConfig.Length == 0)
			{
				_log.LogError("Tables missing from the configuration");
				return false;
			}

			var configChecker = new ConfigChecker(_log, leftDatabaseConnectionString, rightDatabaseConnectionString);

			var dataLoader = new StreamDataLoader(_log, leftDatabaseConnectionString, rightDatabaseConnectionString);

			var dataComparer = new StreamDataComparer(_log, configChecker, dataLoader);

			Dictionary<ComparisonResultEnum, List<ComparisonResult>> data = dataComparer.CompareData(compareConfig).GroupBy(x => x.Result, y => y).ToDictionary(i => i.Key, j => j.ToList());

			if (data.ContainsKey(ComparisonResultEnum.Identical))
			{
				_log.LogWarning($"Identical: {data[ComparisonResultEnum.Identical].Count}");
			}

			bool areIdentical = true;
			if (data.ContainsKey(ComparisonResultEnum.LeftOnly))
			{
				areIdentical = false;
				_log.LogWarning($"LeftOnly : {data[ComparisonResultEnum.LeftOnly].Count}");
				foreach (var x in data[ComparisonResultEnum.LeftOnly])
				{
					_log.LogWarning($"{x.TableName} - {x.RowIdentifier} - {string.Join(";", x.DifferenceReasons)}");
				}
			}

			if (data.ContainsKey(ComparisonResultEnum.RightOnly))
			{
				areIdentical = false;
				_log.LogWarning($"RightOnly: {data[ComparisonResultEnum.RightOnly].Count}");
				foreach (var x in data[ComparisonResultEnum.RightOnly])
				{
					_log.LogWarning($"{x.TableName} - {x.RowIdentifier} - {string.Join(";", x.DifferenceReasons)}");
				}
			}

			if (data.ContainsKey(ComparisonResultEnum.Different))
			{
				areIdentical = false;
				_log.LogWarning($"Different: {data[ComparisonResultEnum.Different].Count}");

				foreach (string difference in data[ComparisonResultEnum.Different].SelectMany(x => x.DifferenceReasons))
				{
					_log.LogWarning(difference);
				}
			}

			return areIdentical;
		}
	}
}
