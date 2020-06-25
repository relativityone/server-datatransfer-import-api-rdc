// <copyright file="SqlComparerInputCollector.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.SqlDataComparer
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Xml;

	public class SqlComparerInputCollector
	{
		private static readonly Lazy<SqlComparerInputCollector> _instanceLazy = new Lazy<SqlComparerInputCollector>(() => new SqlComparerInputCollector());
		private List<TestWorkspaceToCompareDto> _items;

		private SqlComparerInputCollector()
		{
		}

		public static SqlComparerInputCollector Instance => _instanceLazy.Value;

		public void Initialize()
		{
			_items = new List<TestWorkspaceToCompareDto>();
		}

		public void AddTestWorkspaceToCompare(TestWorkspaceToCompareDto dto)
		{
			if (_items == null)
			{
				throw new InvalidOperationException("Cannot add test when object is not initialized.");
			}

			dto = dto ?? throw new ArgumentNullException(nameof(dto));
			_items.Add(dto);
		}

		public void SaveComparerInput(string outputDirectory, bool massImportImprovementsToggle)
		{
			string outputFileName = $"sqlComparerInput-{massImportImprovementsToggle}.xml";
			GenerateAndSaveInput(outputDirectory, outputFileName);
			this._items = null;
		}

		private static string CopyComparerConfigToOutputDirectoryAndGetFileName(
			string outputDirectory,
			TestWorkspaceToCompareDto item)
		{
			string comparerConfigFileName = Path.GetFileName(item.ComparerConfigFilePath);
			string destinationComparerConfigFilePath = Path.Combine(outputDirectory, comparerConfigFileName);
			File.Copy(item.ComparerConfigFilePath, destinationComparerConfigFilePath, overwrite: true);
			return comparerConfigFileName;
		}

		private void GenerateAndSaveInput(string outputDirectory, string outputFileName)
		{
			Directory.CreateDirectory(outputDirectory);
			var file = Path.Combine(outputDirectory, outputFileName);

			var settings = new XmlWriterSettings
			{
				Indent = true,
			};
			using (var writer = XmlWriter.Create(file, settings))
			{
				writer.WriteStartDocument();

				writer.WriteStartElement("TestWorkspaces");
				foreach (TestWorkspaceToCompareDto item in _items)
				{
					writer.WriteStartElement("TestWorkspace");
					writer.WriteAttributeString("TestName", item.FullTestCaseName);
					writer.WriteAttributeString("DatabaseName", item.DatabaseName);

					string comparerConfigFileName = CopyComparerConfigToOutputDirectoryAndGetFileName(outputDirectory, item);
					writer.WriteAttributeString("ComparerConfigFilePath", comparerConfigFileName);
					writer.WriteEndElement();
				}

				writer.WriteEndElement();
				writer.WriteEndDocument();
			}
		}
	}
}
