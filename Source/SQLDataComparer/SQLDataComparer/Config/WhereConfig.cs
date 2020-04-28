using System.Collections.Generic;
using System.Configuration;

namespace SQLDataComparer.Config
{
	public class WhereConfig : ConfigurationElementCollection, IEnumerable<SingleWhereConfig>
	{
		public SingleWhereConfig this[int idx]
		{
			get { return (SingleWhereConfig)BaseGet(idx); }
			set
			{
				if (BaseGet(idx) != null)
					BaseRemoveAt(idx);

				BaseAdd(idx, value);
			}
		}

		public new SingleWhereConfig this[string key]
		{
			get { return (SingleWhereConfig)BaseGet(key); }
			set
			{
				if (BaseGet(key) != null)
					BaseRemoveAt(BaseIndexOf(BaseGet(key)));

				BaseAdd(value);
			}
		}

		public void Add(SingleWhereConfig singleWhereConfig)
		{
			BaseAdd(singleWhereConfig);
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new SingleWhereConfig();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((SingleWhereConfig)element).Name;
		}

		public new IEnumerator<SingleWhereConfig> GetEnumerator()
		{
			foreach (var key in this.BaseGetAllKeys())
			{
				yield return (SingleWhereConfig)BaseGet(key);
			}
		}
	}
}
