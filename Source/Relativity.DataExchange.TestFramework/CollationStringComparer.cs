// ----------------------------------------------------------------------------
// <copyright file="CollationStringComparer.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework
{
	using System.Collections.Generic;
	using System.Data.SqlTypes;

	public class CollationStringComparer : IEqualityComparer<string>
	{
		private int lcid;
		private SqlCompareOptions sqlCompareOptions;

		private CollationStringComparer(int lcid, SqlCompareOptions sqlCompareOptions)
		{
			this.lcid = lcid;
			this.sqlCompareOptions = sqlCompareOptions;
		}

		public static CollationStringComparer SQL_Latin1_General_CP1_CI_AS
		{
			get
			{
				return new CollationStringComparer(0x0409, SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreWidth | SqlCompareOptions.IgnoreKanaType);
			}
		}

		public static bool Compare(string x, string y)
		{
			CollationStringComparer comparer = new CollationStringComparer(0x0409, SqlCompareOptions.IgnoreCase | SqlCompareOptions.IgnoreWidth | SqlCompareOptions.IgnoreKanaType);
			return comparer.Equals(x, y);
		}

		public bool Equals(string x, string y)
		{
			SqlString xw = new SqlString(x, this.lcid, this.sqlCompareOptions);
			SqlString yw = new SqlString(y, this.lcid, this.sqlCompareOptions);
			return xw.CompareTo(yw) == 0;
		}

		public int GetHashCode(string obj)
		{
			SqlString objw = new SqlString(obj, this.lcid, this.sqlCompareOptions);
			return objw.GetHashCode();
		}
	}
}
