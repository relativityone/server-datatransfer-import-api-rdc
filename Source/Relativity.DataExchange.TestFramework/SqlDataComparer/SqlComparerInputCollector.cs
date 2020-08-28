// <copyright file="SqlComparerInputCollector.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.SqlDataComparer
{
	using System;
	using System.IO;
	using System.Xml;

	public class SqlComparerInputCollector
	{
		private static readonly Lazy<SqlComparerInputCollector> _instanceLazy = new Lazy<SqlComparerInputCollector>(() => new SqlComparerInputCollector());

		private TestWorkspaceToCompareDto _testWorkspaceToCompareDto;

		private SqlComparerInputCollector()
		{
		}

		public static SqlComparerInputCollector Instance => _instanceLazy.Value;

		public void AddTestWorkspaceToCompare(TestWorkspaceToCompareDto dto, string outputFilePath)
		{
			_testWorkspaceToCompareDto = dto ?? throw new ArgumentNullException(nameof(dto));

			GenerateEmptyInputStructure(outputFilePath);
			AddTestWorkspaceToXml(outputFilePath);
		}

		private static void GenerateEmptyInputStructure(string filePath)
		{
			if (!File.Exists(filePath))
			{
				string outputDirectory = Directory.GetParent(filePath).FullName;
				Directory.CreateDirectory(outputDirectory);

				var settings = new XmlWriterSettings
					               {
						               Indent = true,
					               };
				using (var writer = XmlWriter.Create(filePath, settings))
				{
					writer.WriteStartDocument();
					writer.WriteStartElement("TestWorkspaces");
					writer.WriteEndElement();
					writer.WriteEndDocument();
				}
			}
		}

		private string CopyComparerConfigToOutputDirectoryAndGetFileName(string outputDirectory)
		{
			string comparerConfigFileName = Path.GetFileName(this._testWorkspaceToCompareDto.ComparerConfigFilePath);
			string destinationComparerConfigFilePath = Path.Combine(outputDirectory, comparerConfigFileName);
			File.Copy(this._testWorkspaceToCompareDto.ComparerConfigFilePath, destinationComparerConfigFilePath, overwrite: true);
			return destinationComparerConfigFilePath;
		}

		private void AddTestWorkspaceToXml(string xmlFilePath)
		{
			if (!File.Exists(xmlFilePath))
			{
				throw new InvalidOperationException($"File '{xmlFilePath}' you want to modify doesn't exist");
			}

			XmlDocument doc = new XmlDocument();
			doc.Load(xmlFilePath);

			XmlNode testWorkspaceNode = doc.CreateNode(XmlNodeType.Element, "TestWorkspace", null);

			XmlAttribute testName = doc.CreateAttribute("TestName");
			testName.Value = this._testWorkspaceToCompareDto.FullTestCaseName;
			XmlAttribute databaseName = doc.CreateAttribute("DatabaseName");
			databaseName.Value = this._testWorkspaceToCompareDto.DatabaseName;
			XmlAttribute comparerConfigFileName = doc.CreateAttribute("ComparerConfigFilePath");
			comparerConfigFileName.Value = CopyComparerConfigToOutputDirectoryAndGetFileName(Directory.GetParent(xmlFilePath).FullName);

			testWorkspaceNode.Attributes.Append(testName);
			testWorkspaceNode.Attributes.Append(databaseName);
			testWorkspaceNode.Attributes.Append(comparerConfigFileName);

			doc.DocumentElement.AppendChild(testWorkspaceNode);
			doc.Save(xmlFilePath);

			this._testWorkspaceToCompareDto = null;
		}
	}
}
