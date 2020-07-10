using System.Collections.Generic;
using System.Xml;
using SQLDataComparer.Log;
using SQLDataComparer.Model;

namespace SQLDataComparer.DataCompare
{
	public class AuditRowEqualityComparer : RowMappingEqualityComparer
	{
		public AuditRowEqualityComparer(ILog log, Dictionary<string, string> mappingTable, string tableName) 
			: base(log, "ArtifactID", mappingTable, tableName)
		{
		}

		public override List<string> MatchAndCompareRowsValues(List<Row> leftRows, List<Row> rightRows)
		{
			var differences = new List<string>();
			var matchedRows = new Dictionary<int, int>();

			for (int i = 0; i < leftRows.Count; i++)
			{
				int matchedTo = -1;
				List<string> differencesOneRow = null;

				for (int j = 0; j < rightRows.Count; j++)
				{
					if (!matchedRows.ContainsKey(j))
					{
						List<string> differencesWithCurrentRow = GetDifferences(leftRows[i], rightRows[j]);

						if (differencesOneRow == null || differencesWithCurrentRow.Count < differencesOneRow.Count)
						{
							matchedTo = j;
							differencesOneRow = differencesWithCurrentRow;
						}
					}
				}

				if (matchedTo != -1)
				{
					matchedRows.Add(matchedTo, i);

					if (differencesOneRow?.Count > 0)
					{
						differences.AddRange(differencesOneRow);
					}
				}
			}

			return differences;
		}

		protected override List<string> GetDifferences(Row leftRow, Row rightRow)
		{
			var differences = new List<string>();

			if (leftRow.Values.Count != rightRow.Values.Count)
			{
				differences.Add($"{_tableName}.{leftRow.Id} : Number of columns, left: {leftRow.Values.Count}, right: {rightRow.Values.Count}");
			}

			differences.AddRange(MatchAndCompareDetails(leftRow[2], rightRow[2], leftRow.Id));
			
			return differences;
		}

		protected List<string> MatchAndCompareDetails(string leftDetails, string rightDetails, string rowId)
		{
			var differences = new List<string>();

			if (!string.IsNullOrEmpty(leftDetails) && !string.IsNullOrEmpty(rightDetails))
			{
				var leftXml = new XmlDocument();
				var rightXml = new XmlDocument();

				leftXml.LoadXml(leftDetails);
				rightXml.LoadXml(rightDetails);

				XmlNodeList leftNodes = leftXml.DocumentElement.ChildNodes;
				XmlNodeList rightNodes = rightXml.DocumentElement.ChildNodes;

				if (leftNodes.Count != rightNodes.Count)
				{
					differences.Add($"{_tableName}.{rowId} : Number of audit elements, left: {leftNodes.Count}, right: {rightNodes.Count}");
				}
				
				var matchedDetails = new Dictionary<int, int>();

				for (int i = 0; i < leftNodes.Count; i++)
				{
					int matchedTo = -1;
					List<string> differencesOneDetail = null;

					for (int j = 0; j < rightNodes.Count; j++)
					{
						if (!matchedDetails.ContainsKey(j))
						{
							if (AreMatching(leftNodes[i], rightNodes[j], rowId))
							{
								List<string> differencesWithCurrentDetail = GetTypeDifferences(leftNodes[i], rightNodes[j], rowId);
								
								if (differencesOneDetail == null || differencesWithCurrentDetail.Count < differencesOneDetail.Count)
								{
									matchedTo = j;
									differencesOneDetail = differencesWithCurrentDetail;
								}
							}
						}
					}

					if (matchedTo != -1)
					{
						matchedDetails.Add(matchedTo, i);

						if (differencesOneDetail?.Count > 0)
						{
							differences.AddRange(differencesOneDetail);
						}
					}
					else
					{
						differences.Add($"{_tableName}.{rowId} : Audit row not matched");
					}
				}
			}

			return differences;
		}

		protected bool AreMatching(XmlNode leftNode, XmlNode rightNode, string rowId)
		{
			string leftValue = leftNode.Attributes["id"].Value;
			string rightValue = rightNode.Attributes["id"].Value;

			if(GetMappingDifferences(leftValue, rightValue, rowId)?.Count > 0)
			{
				return false;
			}

			leftValue = leftNode.Attributes["name"].Value;
			rightValue = rightNode.Attributes["name"].Value;

			if (leftValue != rightValue)
			{
				return false;
			}

			return true;
		}

		protected List<string> GetDetailsDifferences(XmlNode leftNode, XmlNode rightNode, string rowId)
		{
			var differences = new List<string>();

			string leftValue = leftNode.Attributes["id"].Value;
			string rightValue = rightNode.Attributes["id"].Value;

			differences.AddRange(GetMappingDifferences(leftValue, rightValue, rowId));

			leftValue = leftNode.Attributes["name"].Value;
			rightValue = rightNode.Attributes["name"].Value;

			if (leftValue != rightValue)
			{
				differences.Add($"{_tableName}.{rowId} : Different Name, left: {leftValue}, right: {rightValue}");
			}
			
			differences.AddRange(GetTypeDifferences(leftNode, rightNode, rowId));

			return differences;
		}

		protected List<string> GetTypeDifferences(XmlNode leftNode, XmlNode rightNode, string rowId)
		{
			var differences = new List<string>();

			string leftValue = leftNode.Attributes["type"].Value;
			string rightValue = rightNode.Attributes["type"].Value;

			if (leftValue != rightValue)
			{
				differences.Add($"{_tableName}.{rowId} : Different Type, left: {leftValue}, right: {rightValue}");
			}
			else
			{
				differences.AddRange(GetInnerDifferences(leftNode.ChildNodes, rightNode.ChildNodes, rowId));
			}

			return differences;
		}

		protected List<string> GetInnerDifferences(XmlNodeList leftNodes, XmlNodeList rightNodes, string rowId)
		{
			var differences = new List<string>();

			if (leftNodes.Count != rightNodes.Count)
			{
				differences.Add($"{_tableName}.{rowId} : Number of audit elements, left: {leftNodes.Count}, right: {rightNodes.Count}");
			}
			else
			{
				for (int i = 0; i < leftNodes.Count; i++)
				{
					string leftValue = leftNodes[i].InnerText;
					string rightValue = rightNodes[i].InnerText;

					differences.AddRange(GetMappingDifferences(leftValue, rightValue, rowId));
				}
			}

			return differences;
		}
	}
}
