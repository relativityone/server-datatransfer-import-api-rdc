using System.Collections.Generic;
using System.Configuration;

namespace SQLDataComparer.Config
{
	public class MappingsConfig : ConfigurationElementCollection, IEnumerable<MappingConfig>
	{
		public MappingConfig this[int idx]
		{
			get { return (MappingConfig)BaseGet(idx); }
			set
			{
				if (BaseGet(idx) != null)
					BaseRemoveAt(idx);

				BaseAdd(idx, value);
			}
		}

		public new MappingConfig this[string key]
		{
			get { return (MappingConfig)BaseGet(key); }
			set
			{
				if (BaseGet(key) != null)
					BaseRemoveAt(BaseIndexOf(BaseGet(key)));

				BaseAdd(value);
			}
		}

		public void Add(MappingConfig mappingConfig)
		{
			BaseAdd(mappingConfig);
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new MappingConfig();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((MappingConfig)element).Name;
		}

		public new IEnumerator<MappingConfig> GetEnumerator()
		{
			foreach (var key in this.BaseGetAllKeys())
			{
				yield return (MappingConfig)BaseGet(key);
			}
		}
	}
}
