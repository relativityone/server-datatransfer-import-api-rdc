using System.Collections.Generic;
using System.Configuration;

namespace SQLDataComparer.Config
{
	public class TablesConfig : ConfigurationElementCollection, IEnumerable<TableConfig>
	{
		public TableConfig this[int idx]
		{
			get { return (TableConfig)BaseGet(idx); }
			set
			{
				if (BaseGet(idx) != null)
					BaseRemoveAt(idx);

				BaseAdd(idx, value);
			}
		}

		public new TableConfig this[string key]
		{
			get { return (TableConfig)BaseGet(key); }
			set
			{
				if (BaseGet(key) != null)
					BaseRemoveAt(BaseIndexOf(BaseGet(key)));

				BaseAdd(value);
			}
		}

		public void Add(TableConfig tableConfig)
		{
			BaseAdd(tableConfig);
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new TableConfig();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((TableConfig)element).Name;
		}

		public new IEnumerator<TableConfig> GetEnumerator()
		{
			foreach (var key in this.BaseGetAllKeys())
			{
				yield return (TableConfig)BaseGet(key);
			}
		}
	}
}
