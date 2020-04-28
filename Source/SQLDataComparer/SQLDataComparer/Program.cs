using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLDataComparer.Config;
using SQLDataComparer.ConfigCheck;
using SQLDataComparer.DataCompare;
using SQLDataComparer.DataLoad;
using SQLDataComparer.Log;
using SQLDataComparer.Model;

namespace SQLDataComparer
{
	class Program
	{
		static void Main(string[] args)
		{
			Run();
			
			Console.ReadKey();
		}

		static void Run()
		{
			var log = new ConsoleLog(false);

			var connectionStrings = ConfigurationManager.ConnectionStrings;

			if (connectionStrings == null || connectionStrings.Count == 0)
			{
				log.LogError("Invalid connection strings' configuration");
				return;
			}

			var compareConfig = (CompareConfig)ConfigurationManager.GetSection("compareConfig");

			if (compareConfig?.TablesConfig == null || compareConfig.TablesConfig.Count == 0)
			{
				log.LogError("Tables missing from the configuration");
				return;
			}

			var configChecker = new ConfigChecker(log, connectionStrings["LeftDatabase"].ConnectionString, connectionStrings["RightDatabase"].ConnectionString);
			
			var dataLoader = new StreamDataLoader(log, connectionStrings["LeftDatabase"].ConnectionString, connectionStrings["RightDatabase"].ConnectionString);

			var dataComparer = new StreamDataComparer(log, configChecker, dataLoader);

			Dictionary<ComparisonResultEnum, List<ComparisonResult>> data = dataComparer.CompareData(compareConfig).GroupBy(x=>x.Result, y=>y).ToDictionary(i=>i.Key, j=>j.ToList());

			if (data.ContainsKey(ComparisonResultEnum.Identical))
			{
				log.LogWarning($"Identical: {data[ComparisonResultEnum.Identical].Count}");
			}

			if (data.ContainsKey(ComparisonResultEnum.LeftOnly))
			{
				log.LogWarning($"LeftOnly : {data[ComparisonResultEnum.LeftOnly].Count}");
			}

			if (data.ContainsKey(ComparisonResultEnum.RightOnly))
			{
				log.LogWarning($"RightOnly: {data[ComparisonResultEnum.RightOnly].Count}");
			}

			if (data.ContainsKey(ComparisonResultEnum.Different))
			{
				log.LogWarning($"Different: {data[ComparisonResultEnum.Different].Count}");

				foreach (string difference in data[ComparisonResultEnum.Different].SelectMany(x => x.DifferenceReasons))
				{
					log.LogWarning(difference);
				}
			}

		}
	}
}
