using System.Collections.Generic;
using System.Configuration;

namespace SQLDataComparer.Config
{
	public class IgnoreConfig : ConfigurationElementCollection, IEnumerable<SingleIgnoreConfig>
	{
		public SingleIgnoreConfig this[int idx]
		{
			get { return (SingleIgnoreConfig)BaseGet(idx); }
			set
			{
				if (BaseGet(idx) != null)
					BaseRemoveAt(idx);

				BaseAdd(idx, value);
			}
		}

		public new SingleIgnoreConfig this[string key]
		{
			get { return (SingleIgnoreConfig)BaseGet(key); }
			set
			{
				if (BaseGet(key) != null)
					BaseRemoveAt(BaseIndexOf(BaseGet(key)));

				BaseAdd(value);
			}
		}

		public void Add(SingleIgnoreConfig singleIgnoreConfig)
		{
			BaseAdd(singleIgnoreConfig);
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new SingleIgnoreConfig();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((SingleIgnoreConfig)element).Name;
		}

		public new IEnumerator<SingleIgnoreConfig> GetEnumerator()
		{
			foreach (var key in this.BaseGetAllKeys())
			{
				yield return (SingleIgnoreConfig)BaseGet(key);
			}
		}
	}
}
