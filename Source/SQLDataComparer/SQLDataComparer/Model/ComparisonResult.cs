using System.Collections.Generic;

namespace SQLDataComparer.Model
{
	public class ComparisonResult
	{
		public string TableName { get; set; }
		public string RowIdentifier { get; set; }
		public ComparisonResultEnum Result { get; set; }
		public List<string> DifferenceReasons { get; set; } = new List<string>();
	}
}
