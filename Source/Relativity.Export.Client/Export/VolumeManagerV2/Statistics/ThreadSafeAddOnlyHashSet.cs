using System.Collections.Generic;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics
{
	public class ThreadSafeAddOnlyHashSet<T>
	{
		private readonly ICollection<T> _internalHashSet = new HashSet<T>();
		private readonly object _lockToken = new object();

		public void Add(T item)
		{
			lock (_lockToken)
			{
				_internalHashSet.Add(item);
			}
		}

		public bool Contains(T item)
		{
			lock (_lockToken)
			{
				return _internalHashSet.Contains(item);
			}
		}

		public int Count
		{
			get
			{
				lock (_lockToken)
				{
					return _internalHashSet.Count;
				}
			}
		}
	}
}